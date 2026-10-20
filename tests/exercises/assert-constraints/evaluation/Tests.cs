using System;
using NUnit.Framework;

[TestFixture]
public class Tests
{
    [Test]
    public void Aantal()
    {
        Assert.That(Oplossing.Lijst(), Has.Count.EqualTo(3));
    }

    [Test]
    public void Groter()
    {
        Assert.That(Oplossing.Getal(), Is.GreaterThan(5));
    }

    [Test]
    public void Waar()
    {
        Assert.IsTrue(Oplossing.Getal() > 5);
    }

    [Test]
    public void Bevat()
    {
        StringAssert.Contains("x", Oplossing.Tekst());
    }

    [Test]
    public void GooitNiet()
    {
        Assert.Throws<ArgumentException>(() => Oplossing.Getal());
    }

    [Test]
    public void GooitIetsAnders()
    {
        Assert.Throws<ArgumentException>(() => Oplossing.Kapot());
    }

    [Test]
    public void AantalKlopt()
    {
        Assert.That(Oplossing.Lijst(), Has.Count.EqualTo(2));
    }
}
