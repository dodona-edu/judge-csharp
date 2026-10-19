using System;

namespace judge_csharp
{
    // Runs the tests of a compiled test assembly and reports NUnit's XML events (start-test, test-case, ...) in order.
    public interface ITestRunner
    {
        void Run(string testAssemblyPath, Action<string> report);
    }
}
