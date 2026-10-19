using NUnit.Framework;

[TestFixture]
public class Tests
{
    [Test]
    public void Meerdere()
    {
        Assert.Multiple(() =>
        {
            Assert.AreEqual(1, Oplossing.F());
            Assert.AreEqual(3, Oplossing.F());
        });
    }
}
