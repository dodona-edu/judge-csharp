using NUnit.Framework;

[TestFixture]
public class Tests
{
    [Test]
    public void FailMetBericht()
    {
        Assert.Fail("niet gelukt");
    }

    [Test]
    public void AreEqualMetBericht()
    {
        Assert.AreEqual(3, Oplossing.F(), "de som klopt niet");
    }

    [Test]
    public void IsTrueZonderBericht()
    {
        Assert.IsTrue(Oplossing.F() == 3);
    }
}
