using System;
namespace VecompSoftware.DocSuiteWeb.API
{
    internal static class StringEx
    {
        public static byte[] ToBytes(this string source)
        {
            var bytes = new byte[source.Length * sizeof(char)];
            Buffer.BlockCopy(source.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string ToBase64(this string source)
        {
            var bytes = source.ToBytes();
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(this string source)
        {
            var bytes = Convert.FromBase64String(source);
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}
