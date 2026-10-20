using NUnit.Framework;

[TestFixture]
public class Tests
{
    [Test]
    public void AreEqualMetArgumenten()
    {
        Assert.AreEqual(3, Oplossing.F(), "de som van {0} en {1} klopt niet", 1, 2);
    }

    [Test]
    public void ThatMetArgumenten()
    {
        Assert.That(Oplossing.F(), Is.EqualTo(3), "verwacht {0}", 3);
    }
}
