using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace judge_csharp
{
    // Starts this judge again as the test process and receives its events over an anonymous pipe, never over its stdout.
    public sealed class ChildProcess : IDisposable
    {
        private readonly Process Process;
        private readonly AnonymousPipeServerStream Pipe;

        public EventReader Events { get; }

        private ChildProcess(Process process, AnonymousPipeServerStream pipe)
        {
            Process = process;
            Pipe = pipe;
            Events = new EventReader(pipe);
        }

        public static ChildProcess Start(string testAssemblyPath)
        {
            var pipe = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
            var info = new ProcessStartInfo(Environment.ProcessPath)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                StandardOutputEncoding = new UTF8Encoding(false),
            };
            info.ArgumentList.Add(typeof(ChildProcess).Assembly.Location);
            info.ArgumentList.Add(Program.ChildFlag);
            info.ArgumentList.Add(pipe.GetClientHandleAsString());
            info.ArgumentList.Add(testAssemblyPath);

            Process process = Process.Start(info);
            // Without this the pipe never reaches end of stream when the test process dies.
            pipe.DisposeLocalCopyOfClientHandle();
            // What the test process writes to its stdout outside NUnit's capture goes to stderr, out of the judge's output.
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                    Console.Error.WriteLine(e.Data);
            };
            process.BeginOutputReadLine();
            return new ChildProcess(process, pipe);
        }

        public int WaitForExit()
        {
            Process.WaitForExit();
            return Process.ExitCode;
        }

        public void Dispose()
        {
            Pipe.Dispose();
            Process.Dispose();
        }
    }
}
