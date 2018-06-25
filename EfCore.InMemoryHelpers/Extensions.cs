using System;

static class RowVersion
{
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
}