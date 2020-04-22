using System.IO;

namespace VecompSoftware.Common
{
    public static class MemoryStreamExtensions
    {

        public static void CloseQuietly(this MemoryStream source)
        {
            if (source == null)
                return;
            source.Close();
        }
        public static byte[] ToDeepCopyArray(this MemoryStream source)
        {
            var raw = source.ToArray();
            var result = new byte[raw.Length];
            raw.CopyTo(result, 0);
            return result;
        }

    }
}
