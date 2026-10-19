using NUnit.Framework;

[TestFixture]
public class Tests
{
    [SetUp]
    public void Voorbereiding()
    {
        Oplossing.F();
    }

    [Test]
    public void Test()
    {
        Assert.AreEqual(2, 2);
    }
}
