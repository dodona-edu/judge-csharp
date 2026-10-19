using System;
using System.Text;

namespace judge_csharp
{
    public static class Program
    {
        public const string ChildFlag = "--test-process";

        // judge-csharp.dll <test assembly> [language]
        public static int Main(string[] args)
        {
            if (args[0] == ChildFlag)
            {
                TestProcess.Run(args[1], args[2], new FrameworkControllerRunner());
                return 0;
            }

            string testAssemblyPath = args[0];
            string language = args.Length > 1 ? args[1] : "en";
            Console.OutputEncoding = new UTF8Encoding(false);
            using (var child = ChildProcess.Start(testAssemblyPath))
                Judge.Run(child.Events, child.WaitForExit, Console.Out, language);
            return 0;
        }
    }
}
