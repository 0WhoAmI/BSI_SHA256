namespace Lab4
{
    public static class ByteExtensions
    {
        public static string ToHex(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}