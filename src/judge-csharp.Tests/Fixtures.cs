using System;
using System.IO;
using System.Xml;
using judge_csharp.EventHandling;

namespace judge_csharp.Tests
{
    public static class Fixtures
    {
        // Feeds the recorded NUnit events of fixtures/{name}.xml to the handler one by one and returns the JSON lines it wrote.
        public static string[] Handle(string name)
        {
            var doc = new XmlDocument();
            doc.Load(Path.Combine(AppContext.BaseDirectory, "fixtures", name + ".xml"));

            var output = new StringWriter();
            var handler = new OutputHandler(output);
            foreach (XmlElement report in doc.DocumentElement.ChildNodes)
                handler.Handle(report.OuterXml);
            return output.ToString().Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
