using NUnit.Framework;

[TestFixture]
public class TekstTests
{
    [Test]
    public void LegeTekst()
    {
        Assert.AreEqual("", Oplossing.Leeg());
    }

    [Test]
    public void SpecialeTekens()
    {
        Assert.AreEqual("a<b>&\u0001", Oplossing.Speciaal());
    }
}
