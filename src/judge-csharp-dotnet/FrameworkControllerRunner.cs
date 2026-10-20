using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace judge_csharp
{
    // Drives the nunit.framework.dll next to the test assembly through its FrameworkController, the entry point NUnit's engine uses.
    // Bound by reflection, so the judge itself is built against no NUnit version.
    public sealed class FrameworkControllerRunner : ITestRunner
    {
        public void Run(string testAssemblyPath, Action<string> report)
        {
            string directory = Path.GetDirectoryName(testAssemblyPath);
            Assembly framework = Assembly.LoadFrom(Path.Combine(directory, "nunit.framework.dll"));
            Type controllerType = framework.GetType("NUnit.Framework.Api.FrameworkController", throwOnError: true);

            IDictionary settings = new Dictionary<string, object> { { "WorkDirectory", directory } };
            object controller = Activator.CreateInstance(controllerType, testAssemblyPath, "0-", settings);
            controllerType.GetMethod("LoadTests", Type.EmptyTypes).Invoke(controller, null);
            controllerType.GetMethod("RunTests", new[] { typeof(Action<string>), typeof(string) })
                .Invoke(controller, new object[] { report, "<filter/>" });
        }
    }
}
