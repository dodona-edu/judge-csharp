using System;
using System.IO;
using judge_csharp.EventHandling;

namespace judge_csharp
{
    public static class Judge
    {
        // Feeds the test process's events to the OutputHandler until the run ends, then closes what an early end left open.
        public static void Run(EventReader events, Func<int> waitForExit, TextWriter output, string language)
        {
            var tracker = new LevelTracker(output);
            var handler = new OutputHandler(tracker, language);
            bool finished = false;
            string report;
            while (!finished && (report = events.Receive()) != null)
                finished = !handler.Handle(report);

            int exitCode = waitForExit();
            if (finished)
                return;
            foreach (string line in EarlyEnd.Close(tracker.OpenLevels, exitCode, language))
                output.WriteLine(line);
        }
    }
}
