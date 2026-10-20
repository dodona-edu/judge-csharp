using System;
using System.IO;
using NUnit.Framework;

namespace judge_csharp.Tests
{
    [TestFixture]
    public class JudgeTests
    {
        const string StartRun = "<start-run/>";
        const string StartFixture = """<start-suite id="0-1000" name="SomTests" fullname="SomTests" type="TestFixture"/>""";
        const string StartTest = """<start-test id="0-1001" name="BeideNegatief" fullname="SomTests.BeideNegatief" type="TestMethod"/>""";
        const string TestCase = """<test-case id="0-1001" name="BeideNegatief" result="Passed"><assertions><assertion result="Passed"><expected><![CDATA[-7]]></expected><actual><![CDATA[-7]]></actual></assertion></assertions></test-case>""";
        const string EndFixture = """<test-suite type="TestFixture" id="0-1000" name="SomTests" result="Passed"/>""";
        const string TestRun = "<test-run/>";

        // Runs the judge on the events as the test process would send them, and returns the lines it wrote.
        static string[] Judge(int exitCode, string language, params string[] events)
        {
            return Judge(exitCode, language, Frames(events));
        }

        static string[] Judge(int exitCode, string language, byte[] channel)
        {
            var output = new StringWriter();
            judge_csharp.Judge.Run(new EventReader(new MemoryStream(channel)), () => exitCode, output, language);
            return output.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
        }

        static byte[] Frames(params string[] events)
        {
            var stream = new MemoryStream();
            var writer = new EventWriter(stream);
            foreach (string report in events)
                writer.Send(report);
            return stream.ToArray();
        }

        [Test]
        public void CompleteRunIgnoresTheExitCode()
        {
            Assert.That(Judge(134, "en", StartRun, StartFixture, StartTest, TestCase, EndFixture, TestRun), Is.EqualTo(new[]
            {
                """{"command":"start-judgement"}""",
                """{"command":"start-tab","title":"SomTests"}""",
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">BeideNegatief</span>"}}""",
                """{"command":"start-test","expected":"-7"}""",
                """{"command":"close-test","status":{"enum":"correct"},"generated":"-7"}""",
                """{"command":"close-testcase"}""",
                """{"command":"close-context"}""",
                """{"command":"close-tab"}""",
                """{"command":"close-judgement"}""",
            }));
        }

        [Test]
        public void KilledDuringATestIsMemoryLimitExceeded()
        {
            Assert.That(Judge(137, "en", StartRun, StartFixture, StartTest), Is.EqualTo(new[]
            {
                """{"command":"start-judgement"}""",
                """{"command":"start-tab","title":"SomTests"}""",
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">BeideNegatief</span>"}}""",
                """{"command":"escalate-status","status":{"enum":"memory limit exceeded"}}""",
                """{"command":"close-testcase","accepted":false}""",
                """{"command":"close-context"}""",
                """{"command":"close-tab"}""",
                """{"command":"close-judgement","accepted":false}""",
            }));
        }

        [Test]
        public void ExitDuringATestIsARuntimeError()
        {
            Assert.That(Judge(0, "nl", StartRun, StartFixture, StartTest), Is.EqualTo(new[]
            {
                """{"command":"start-judgement"}""",
                """{"command":"start-tab","title":"SomTests"}""",
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">BeideNegatief</span>"}}""",
                """{"command":"append-message","message":{"format":"plain","description":"De testen stopten voortijdig: het testproces eindigde met exitcode 0."}}""",
                """{"command":"escalate-status","status":{"enum":"runtime error"}}""",
                """{"command":"close-testcase","accepted":false}""",
                """{"command":"close-context"}""",
                """{"command":"close-tab"}""",
                """{"command":"close-judgement","accepted":false}""",
            }));
        }

        [Test]
        public void ExitBetweenFixturesClosesTheJudgement()
        {
            Assert.That(Judge(1, "en", StartRun, StartFixture, StartTest, TestCase, EndFixture), Is.EqualTo(new[]
            {
                """{"command":"start-judgement"}""",
                """{"command":"start-tab","title":"SomTests"}""",
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">BeideNegatief</span>"}}""",
                """{"command":"start-test","expected":"-7"}""",
                """{"command":"close-test","status":{"enum":"correct"},"generated":"-7"}""",
                """{"command":"close-testcase"}""",
                """{"command":"close-context"}""",
                """{"command":"close-tab"}""",
                """{"command":"append-message","message":{"format":"plain","description":"The tests stopped early: the test process exited with code 1."}}""",
                """{"command":"escalate-status","status":{"enum":"runtime error"}}""",
                """{"command":"close-judgement","accepted":false}""",
            }));
        }

        [Test]
        public void DeathBeforeTheRunStartsStillGivesAJudgement()
        {
            Assert.That(Judge(134, "en"), Is.EqualTo(new[]
            {
                """{"command":"start-judgement"}""",
                """{"command":"append-message","message":{"format":"plain","description":"The tests stopped early: the test process exited with code 134."}}""",
                """{"command":"escalate-status","status":{"enum":"runtime error"}}""",
                """{"command":"close-judgement","accepted":false}""",
            }));
        }

        [Test]
        public void EventCutOffByTheKillIsDropped()
        {
            byte[] complete = Frames(StartRun, StartFixture, StartTest);
            byte[] cut = Frames(TestCase);
            byte[] channel = new byte[complete.Length + cut.Length / 2];
            complete.CopyTo(channel, 0);
            Array.Copy(cut, 0, channel, complete.Length, cut.Length / 2);

            Assert.That(Judge(137, "en", channel), Is.EqualTo(Judge(137, "en", StartRun, StartFixture, StartTest)));
        }

        [Test]
        public void EventsKeepTheirNewlines()
        {
            const string failure = "<test-case id=\"0-1001\" name=\"BeideNegatief\" result=\"Failed\"><failure><message><![CDATA[eerste\ntweede]]></message></failure></test-case>";
            Assert.That(Judge(0, "en", StartRun, StartFixture, StartTest, failure, EndFixture, TestRun), Has.Member(
                """{"command":"append-message","message":{"format":"plain","description":"eerste\ntweede"}}"""));
        }

        [Test]
        public void TrackerFollowsTheOpenLevels()
        {
            var tracker = new LevelTracker(new StringWriter());
            tracker.WriteLine("""{"command":"start-judgement"}""");
            tracker.WriteLine("""{"command":"start-tab","title":"SomTests"}""");
            tracker.WriteLine("""{"command":"start-context"}""");
            tracker.WriteLine("""{"command":"start-testcase","description":"x"}""");
            tracker.WriteLine("""{"command":"start-test","expected":"1"}""");
            tracker.WriteLine("""{"command":"append-message","message":"y"}""");
            Assert.That(tracker.OpenLevels, Is.EqualTo(new[] { "judgement", "tab", "context", "testcase", "test" }));

            tracker.WriteLine("""{"command":"close-test","status":{"enum":"correct"},"generated":"1"}""");
            tracker.WriteLine("""{"command":"close-testcase"}""");
            Assert.That(tracker.OpenLevels, Is.EqualTo(new[] { "judgement", "tab", "context" }));
        }

        [Test]
        public void OpenTestClosesWithTheEndStatus()
        {
            Assert.That(EarlyEnd.Close(new[] { "judgement", "tab", "context", "testcase", "test" }, EarlyEnd.Killed, "en"), Is.EqualTo(new[]
            {
                """{"command":"escalate-status","status":{"enum":"memory limit exceeded"}}""",
                """{"command":"close-test","status":{"enum":"memory limit exceeded"},"generated":""}""",
                """{"command":"close-testcase","accepted":false}""",
                """{"command":"close-context"}""",
                """{"command":"close-tab"}""",
                """{"command":"close-judgement","accepted":false}""",
            }));
        }
    }
}
