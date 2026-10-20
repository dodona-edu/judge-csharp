using System.Collections.Generic;
using System.Linq;
using judge_csharp.Feedback;
using judge_csharp.Json;
using Newtonsoft.Json;

namespace judge_csharp
{
    // The output that closes a judgement whose test process ended before NUnit finished the run.
    public static class EarlyEnd
    {
        public const int Killed = 137;

        public static IEnumerable<string> Close(IReadOnlyList<string> openLevels, int exitCode, string language)
        {
            var lines = new List<string>();
            var levels = openLevels.ToList();
            if (levels.Count == 0)
            {
                lines.Add(JSONGenerator.StartJudgement());
                levels.Add("judgement");
            }

            // The container's memory limit is the only thing in the sandbox that kills the test process.
            Status status = exitCode == Killed ? Status.MEMORY_LIMIT_EXCEEDED : Status.RUNTIME_ERROR;
            if (status == Status.RUNTIME_ERROR)
                lines.Add(JSONGenerator.AppendMessage(Format.PLAIN, StoppedMessage(exitCode, language)));
            lines.Add(JSONGenerator.EscalateStatus(status));

            for (int i = levels.Count - 1; i >= 0; i--)
                lines.Add(CloseLevel(levels[i], status));
            return lines;
        }

        private static string StoppedMessage(int exitCode, string language)
        {
            return language == "nl"
                ? $"De testen stopten voortijdig: het testproces eindigde met exitcode {exitCode}."
                : $"The tests stopped early: the test process exited with code {exitCode}.";
        }

        private static string CloseLevel(string level, Status status)
        {
            switch (level)
            {
                case "test":
                    return JSONGenerator.CloseTest(status, "");
                case "testcase":
                    return JSONGenerator.CloseTestcase(false);
                case "context":
                    return JSONGenerator.CloseContext();
                case "tab":
                    return JSONGenerator.CloseTab();
                default:
                    return JsonConvert.SerializeObject(new Dictionary<string, object>
                    {
                        { "command", "close-judgement" },
                        { "accepted", false },
                    });
            }
        }
    }
}
