using System;
using System.Threading;

static class RowVersion
{
    static long counter;
    public static Guid GetGuid(this byte[] rowVersion)
    {
        return new Guid(rowVersion);
    }
    public static string GetString(this byte[] rowVersion)
    {
        return new Guid(rowVersion).ToString();
    }

    public static byte[] New()
    {
        return Guid.NewGuid().ToByteArray();
    }
    public static ulong NewLong()
    {
        return (ulong) Interlocked.Increment(ref counter);
    }
}