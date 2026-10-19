using System;

public static class Oplossing
{
    public static int Som(int a, int b)
    {
        if (b < 0)
        {
            throw new ArgumentException("b mag niet negatief zijn");
        }
        return a + b;
    }
}
