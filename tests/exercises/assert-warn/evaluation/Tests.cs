using NUnit.Framework;

[TestFixture]
public class Tests
{
    [Test]
    public void Waarschuwing()
    {
        Assert.Warn("opgepast");
        Assert.AreEqual(2, Oplossing.F());
    }
}
