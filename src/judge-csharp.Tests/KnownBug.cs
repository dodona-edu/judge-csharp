using System;
using NUnit.Framework;

namespace judge_csharp.Tests
{
    public static class KnownBug
    {
        // The body asserts the intended behaviour. While it fails the test is skipped with the bug as reason;
        // once it passes the test fails until the marker is removed.
        public static void Expect(string description, Action body)
        {
            try
            {
                body();
            }
            catch (Exception)
            {
                Assert.Ignore("Known bug: " + description);
            }
            Assert.Fail("Known bug fixed: remove the KnownBug.Expect marker");
        }
    }
}
