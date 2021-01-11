using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class Extension
    {
        public static bool HasValue(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        public static bool IsSignedFile(this string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || fileName.Length < 3)
                return false;
            return String.Compare(fileName.Substring(fileName.Length - 3, 2), "P7", true) == 0 || String.Compare(fileName.Substring(fileName.Length - 3, 2), "m7", true) == 0 || String.Compare(fileName.Substring(fileName.Length - 3, 2), "ts", true) == 0;
        }
    }
}
