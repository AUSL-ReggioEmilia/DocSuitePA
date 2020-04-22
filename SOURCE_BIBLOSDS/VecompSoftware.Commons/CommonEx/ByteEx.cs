using System.Security.Cryptography;

namespace VecompSoftware.Common
{
    public static class ByteEx
    {

        public static byte[] ComputeSHA1(this byte[] source)
        {
            using (var provider = new SHA1CryptoServiceProvider())
                return provider.ComputeHash(source);
        }

    }
}
