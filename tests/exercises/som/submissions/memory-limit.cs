using System;
using System.Runtime.InteropServices;

public static class Oplossing
{
    // Unmanaged memory is outside the garbage collector's heap limit, so only the container's memory limit stops it.
    public static int Som(int a, int b)
    {
        const int block = 1 << 20;
        for (int i = 0; i < 1024; i++)
        {
            IntPtr memory = Marshal.AllocHGlobal(block);
            for (int offset = 0; offset < block; offset += 4096)
                Marshal.WriteByte(memory, offset, 1);
        }
        return a + b;
    }
}
