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
        public void FailedAssertionWithoutMessage()
        {
            Assert.That(Fixtures.Handle("assert-message-is-true"), Is.EqualTo(new[]
            {
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">IsTrueZonderBericht</span>"}}""",
                """{"command":"escalate-status","status":{"enum":"wrong"}}""",
                """{"command":"close-testcase","accepted":false}""",
                """{"command":"close-context"}""",
            }));
        }

        [Test]
        public void ErrorAfterPassedAssertion()
        {
            Assert.That(Fixtures.Handle("exception-after-assert"), Is.EqualTo(new[]
            {
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">NulEnNegatief</span>"}}""",
                """{"command":"start-test","expected":"0"}""",
                """{"command":"close-test","status":{"enum":"correct"},"generated":"0"}""",
                """{"command":"append-message","message":{"format":"plain","description":"System.ArgumentException : b mag niet negatief zijn"}}""",
                """{"command":"escalate-status","status":{"enum":"runtime error"}}""",
                """{"command":"close-testcase","accepted":false}""",
                """{"command":"close-context"}""",
            }));
        }

        [Test]
        public void WarningAssertion()
        {
            Assert.That(Fixtures.Handle("assert-warn"), Is.EqualTo(new[]
            {
                """{"command":"start-judgement"}""",
                """{"command":"start-tab","title":"Tests"}""",
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">Waarschuwing</span>"}}""",
                """{"command":"append-message","message":{"format":"plain","description":"opgepast"}}""",
                """{"command":"start-test","expected":"2"}""",
                """{"command":"close-test","status":{"enum":"correct"},"generated":"2"}""",
                """{"command":"close-testcase"}""",
                """{"command":"close-context"}""",
                """{"command":"close-tab"}""",
                """{"command":"close-judgement"}""",
            }));
        }

        [Test]
        public void AssertPass()
        {
            Assert.That(Fixtures.Handle("assert-pass"), Is.EqualTo(new[]
            {
                """{"command":"start-judgement"}""",
                """{"command":"start-tab","title":"Tests"}""",
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">Geslaagd</span>"}}""",
                """{"command":"close-testcase","accepted":true}""",
                """{"command":"close-context"}""",
                """{"command":"close-tab"}""",
                """{"command":"close-judgement"}""",
            }));
        }

        [Test]
        public void TestWithoutAssertions()
        {
            Assert.That(Fixtures.Handle("no-assertions"), Is.EqualTo(new[]
            {
                """{"command":"start-judgement"}""",
                """{"command":"start-tab","title":"Tests"}""",
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">ZonderAsserts</span>"}}""",
                """{"command":"close-testcase","accepted":true}""",
                """{"command":"close-context"}""",
                """{"command":"close-tab"}""",
                """{"command":"close-judgement"}""",
            }));
        }

        [Test]
        public void AssertIgnoreInEnglish()
        {
            Assert.That(Fixtures.Handle("assert-ignore", "en"), Is.EqualTo(new[]
            {
                """{"command":"start-judgement"}""",
                """{"command":"start-tab","title":"Tests"}""",
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">Genegeerd</span>"}}""",
                """{"command":"append-message","message":{"format":"plain","description":"Test skipped"}}""",
                """{"command":"close-testcase","accepted":true}""",
                """{"command":"close-context"}""",
                """{"command":"close-tab"}""",
                """{"command":"close-judgement"}""",
            }));
        }

        [Test]
        public void AssertIgnoreInDutch()
        {
            Assert.That(Fixtures.Handle("assert-ignore", "nl"), Is.EqualTo(new[]
            {
                """{"command":"start-judgement"}""",
                """{"command":"start-tab","title":"Tests"}""",
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">Genegeerd</span>"}}""",
                """{"command":"append-message","message":{"format":"plain","description":"Test overgeslagen"}}""",
                """{"command":"close-testcase","accepted":true}""",
                """{"command":"close-context"}""",
                """{"command":"close-tab"}""",
                """{"command":"close-judgement"}""",
            }));
        }

        [Test]
        public void AssertInconclusive()
        {
            Assert.That(Fixtures.Handle("assert-inconclusive"), Is.EqualTo(new[]
            {
                """{"command":"start-judgement"}""",
                """{"command":"start-tab","title":"Tests"}""",
                """{"command":"start-context"}""",
                """{"command":"start-testcase","description":{"format":"html","description":"<span class=\"code\">Onbeslist</span>"}}""",
                """{"command":"append-message","message":{"format":"plain","description":"onduidelijk"}}""",
                """{"command":"escalate-status","status":{"enum":"wrong"}}""",
                """{"command":"close-testcase","accepted":false}""",
                """{"command":"close-context"}""",
                """{"command":"close-tab"}""",
                """{"command":"close-judgement"}""",
            }));
        }
    }
}
