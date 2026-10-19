using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace judge_csharp.Tests
{
    [TestFixture]
    public class OutputHandlerTests
    {
        [Test]
        public void PassedAssertions()
        {
            Assert.That(Fixtures.Handle("som-correct"), Is.EqualTo(new[]
            {
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">NulEnNegatief</span>"}}""",
                """{"command":"start-test","expected":"0"}""",
                """{"command":"close-test","status":{"enum":"correct"},"generated":"0"}""",
                """{"command":"start-test","expected":"-1"}""",
                """{"command":"close-test","status":{"enum":"correct"},"generated":"-1"}""",
                """{"command":"close-testcase"}""",
                """{"command":"close-context"}""",
            }));
        }

        [Test]
        public void FailedAssertion()
        {
            Assert.That(Fixtures.Handle("som-wrong"), Is.EqualTo(new[]
            {
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">NulEnNegatief</span>"}}""",
                """{"command":"start-test","expected":"0"}""",
                """{"command":"close-test","status":{"enum":"correct"},"generated":"0"}""",
                """{"command":"start-test","expected":"-1"}""",
                """{"command":"close-test","status":{"enum":"wrong"},"generated":"5"}""",
                """{"command":"close-testcase"}""",
                """{"command":"close-context"}""",
            }));
        }

        [Test]
        public void ExceptionBeforeAssert()
        {
            Assert.That(Fixtures.Handle("exception-before-assert"), Is.EqualTo(new[]
            {
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">PositieveGetallen</span>"}}""",
                """{"command":"append-message","message":{"format":"plain","description":"System.InvalidOperationException : kan niet optellen"}}""",
                """{"command":"escalate-status","status":{"enum":"runtime error"}}""",
                """{"command":"close-testcase","accepted":false}""",
                """{"command":"close-context"}""",
            }));
        }

        [Test]
        public void FailWithMessage()
        {
            Assert.That(Fixtures.Handle("assert-message-fail"), Is.EqualTo(new[]
            {
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">FailMetBericht</span>"}}""",
                """{"command":"start-test","expected":""}""",
                """{"command":"append-message","message":{"format":"plain","description":"niet gelukt"}}""",
                """{"command":"close-test","status":{"enum":"wrong"},"generated":""}""",
                """{"command":"close-testcase"}""",
                """{"command":"close-context"}""",
            }));
        }

        [Test]
        public void AreEqualWithMessage()
        {
            Assert.That(Fixtures.Handle("assert-message-are-equal"), Is.EqualTo(new[]
            {
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">AreEqualMetBericht</span>"}}""",
                """{"command":"start-test","expected":"3"}""",
                """{"command":"append-message","message":{"format":"plain","description":"de som klopt niet"}}""",
                """{"command":"close-test","status":{"enum":"wrong"},"generated":"2"}""",
                """{"command":"close-testcase"}""",
                """{"command":"close-context"}""",
            }));
        }

        [Test]
        public void FailedAssertionWithoutMessageIsNotAccepted()
        {
            Assert.That(Fixtures.Handle("assert-message-is-true"), Does.Contain("""{"command":"close-testcase","accepted":false}"""));
        }

        [Test]
        public void ErrorAssertionIsHandled()
        {
            KnownBug.Expect("assertion result \"Error\" crashes the judge", () => AssertCompletes("exception-after-assert"));
        }

        [Test]
        public void WarningAssertionIsHandled()
        {
            KnownBug.Expect("assertion result \"Warning\" crashes the judge", () => AssertCompletes("assert-warn", wholeRun: true));
        }

        [Test]
        public void AssertPassIsHandled()
        {
            KnownBug.Expect("a passed test-case without assertions crashes the judge (Assert.Pass)", () => AssertCompletes("assert-pass", wholeRun: true));
        }

        [Test]
        public void AssertIgnoreIsHandled()
        {
            KnownBug.Expect("a skipped test-case without assertions crashes the judge (Assert.Ignore)", () => AssertCompletes("assert-ignore", wholeRun: true));
        }

        [Test]
        public void AssertInconclusiveIsHandled()
        {
            KnownBug.Expect("an inconclusive test-case without assertions crashes the judge (Assert.Inconclusive)", () => AssertCompletes("assert-inconclusive", wholeRun: true));
        }

        [Test]
        public void TestWithoutAssertionsIsHandled()
        {
            KnownBug.Expect("a passed test-case without assertions crashes the judge (no Assert call)", () => AssertCompletes("no-assertions", wholeRun: true));
        }

        // Whatever the final mapping is: no exception, a well-formed close-testcase and, for a whole run, a close-judgement.
        private static void AssertCompletes(string fixture, bool wholeRun = false)
        {
            string[] lines = Fixtures.Handle(fixture);
            var commands = lines.Select(line => (string)JObject.Parse(line)["command"]).ToList();

            Assert.That(commands, Contains.Item("close-testcase"));
            JToken accepted = lines.Select(JObject.Parse).First(line => (string)line["command"] == "close-testcase")["accepted"];
            Assert.That(accepted == null || accepted.Type == JTokenType.Boolean, "accepted must be absent or a boolean");
            if (wholeRun)
                Assert.That(commands.Last(), Is.EqualTo("close-judgement"));
        }
    }
}
