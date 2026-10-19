using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace judge_csharp
{
    // Passes the judge's output through line by line and keeps track of the Dodona levels it left open.
    public sealed class LevelTracker : TextWriter
    {
        private readonly TextWriter Inner;
        private readonly StringBuilder Line = new StringBuilder();
        private readonly Stack<string> Open = new Stack<string>();

        public LevelTracker(TextWriter inner)
        {
            Inner = inner;
        }

        public override Encoding Encoding => Inner.Encoding;

        // Outermost first: "judgement", "tab", "context", "testcase", "test".
        public IReadOnlyList<string> OpenLevels => Open.Reverse().ToList();

        public override void Write(char value)
        {
            if (value != '\n')
            {
                Line.Append(value);
                return;
            }
            string line = Line.ToString().TrimEnd('\r');
            Line.Clear();
            Track(line);
            Inner.WriteLine(line);
        }

        private void Track(string line)
        {
            string command = (string)JObject.Parse(line)["command"];
            if (command.StartsWith("start-"))
                Open.Push(command.Substring("start-".Length));
            else if (command.StartsWith("close-"))
                Open.TryPop(out _);
        }

        public override void Flush()
        {
            Inner.Flush();
        }
    }
}
