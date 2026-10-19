using NUnit.Framework;

[TestFixture]
public class Tests
{
    [TestCase(1, 2)]
    [TestCase(2, 4)]
    public void Opvolger(int a, int b)
    {
        Assert.AreEqual(b, a + Oplossing.F() - 1);
    }
}
