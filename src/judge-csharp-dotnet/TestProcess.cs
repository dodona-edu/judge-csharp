using System;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace judge_csharp
{
    // The child process: runs the student's tests and streams the events to the judge over the inherited pipe.
    public static class TestProcess
    {
        public static void Run(string pipeHandle, string testAssemblyPath, ITestRunner runner)
        {
            UseInvariantCulture();
            Console.OutputEncoding = new UTF8Encoding(false);

            var pipe = new AnonymousPipeClientStream(PipeDirection.Out, pipeHandle);
            var channel = new EventWriter(pipe);
            // The engine's run events, which the framework itself does not send.
            channel.Send("<start-run/>");
            runner.Run(Path.GetFullPath(testAssemblyPath), channel.Send);
            channel.Send("<test-run/>");
            pipe.Dispose();
            // Ends the process even when student code left foreground threads running.
            Environment.Exit(0);
        }

        private static void UseInvariantCulture()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        }
    }
}
