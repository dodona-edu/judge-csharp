using System;
using System.Collections.Generic;

public static class Oplossing
{
    public static List<int> Lijst()
    {
        return new List<int> { 1, 2 };
    }

    public static int Getal()
    {
        return 2;
    }

    public static string Tekst()
    {
        return "abc";
    }

    public static void Kapot()
    {
        throw new InvalidOperationException("kapot");
    }
}
