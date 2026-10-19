using NUnit.Framework;

[TestFixture]
public class Tests
{
    [Test]
    public void NietAscii()
    {
        Assert.AreEqual("é ü — ✓", Oplossing.Tekst());
    }

    [Test]
    public void BerichtNietAscii()
    {
        Assert.AreEqual(1, Oplossing.Een(), "de som klopt niet — ✓");
    }
}
