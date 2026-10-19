using NUnit.Framework;

[TestFixture]
public class SomTests
{
    [Test]
    public void PositieveGetallen()
    {
        Assert.AreEqual(5, Oplossing.Som(2, 3));
    }

    [Test]
    public void NulEnNegatief()
    {
        Assert.AreEqual(0, Oplossing.Som(0, 0));
        Assert.AreEqual(-1, Oplossing.Som(2, -3));
    }

    [Test]
    public void BeideNegatief()
    {
        Assert.AreEqual(-7, Oplossing.Som(-3, -4));
    }
}
