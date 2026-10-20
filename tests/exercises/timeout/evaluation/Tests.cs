using NUnit.Framework;

[TestFixture]
public class Tests
{
    [Test, Timeout(500)]
    public void Oneindig()
    {
        Oplossing.Lus();
    }

    [Test, Timeout(5000)]
    public void OpTijd()
    {
        Assert.AreEqual(2, Oplossing.F());
    }

    [Test]
    public void Daarna()
    {
        Assert.AreEqual(2, Oplossing.F());
    }
}
