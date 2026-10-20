using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace nunit_fork.Tests
{
    // The fork itself runs these tests, so they check the <assertion> elements the judge reads.
    [TestFixture]
    public class AssertionResultTests
    {
        static TNode Recorded(TestDelegate assertion)
        {
            using (new TestExecutionContext.IsolatedContext())
            {
                var result = new TestCaseResult((TestMethod)TestExecutionContext.CurrentContext.CurrentTest);
                TestExecutionContext.CurrentContext.CurrentResult = result;
                assertion();
                return result.ToXml(true).SelectSingleNode("assertions/assertion");
            }
        }

        static void AssertRecorded(TestDelegate assertion, string result, string expected, string actual)
        {
            TNode node = Recorded(assertion);
            Assert.AreEqual(result, node.Attributes["result"]);
            Assert.AreEqual(expected, node.SelectSingleNode("expected")?.Value);
            Assert.AreEqual(actual, node.SelectSingleNode("actual")?.Value);
        }

        [Test]
        public void FailingPropertyConstraintRecordsItsDescription()
        {
            AssertRecorded(() => Assert.That(new List<int> { 1, 2 }, Has.Count.EqualTo(3)), "Failed", "property Count equal to 3", "2");
        }

        [Test]
        public void FailingComparisonRecordsItsDescription()
        {
            AssertRecorded(() => Assert.That(2, Is.GreaterThan(5)), "Failed", "greater than 5", "2");
        }

        [Test]
        public void FailingIsTrueRecordsTrueAndFalse()
        {
            AssertRecorded(() => Assert.IsTrue(false), "Failed", "True", "False");
        }

        [Test]
        public void FailingStringContainsRecordsTheQuotedActualString()
        {
            AssertRecorded(() => StringAssert.Contains("x", "abc"), "Failed", "String containing \"x\"", "\"abc\"");
        }

        [Test]
        public void ThrowsWithoutAnExceptionRecordsNull()
        {
            AssertRecorded(() => Assert.Throws<ArgumentException>(() => { }), "Failed", "<System.ArgumentException>", "null");
        }

        [Test]
        public void FailingEqualityKeepsTheRawValues()
        {
            AssertRecorded(() => Assert.AreEqual("abc", "abd"), "Failed", "abc", "abd");
        }

        [Test]
        public void PassingConstraintRecordsNoValues()
        {
            AssertRecorded(() => Assert.That(new List<int> { 1, 2 }, Has.Count.EqualTo(2)), "Passed", null, null);
        }
    }
}
