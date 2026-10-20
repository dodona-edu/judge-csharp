using System;
using System.IO;
using NUnit.Framework;

[TestFixture]
public class WhitespaceTests
{
    [Test]
    public void LegeRegel()
    {
        var writer = new StringWriter();
        Console.SetOut(writer);
        Oplossing.Print();
        Assert.AreEqual("    \n", writer.ToString());
    }
}
