using System;
using System.Collections.Generic;
using System.IO;
using ImageMagick;

namespace VecompSoftware.MagickNET
{
    public static class MagickNETHelper
    {

        public static void Initialize()
        {
            var config = Path.Combine(Environment.CurrentDirectory, @"ImageMagick\config");
            ImageMagick.MagickNET.Initialize(config);
        }

        private static byte[] Clean(MagickImage source, List<MagickTransformer> arguments)
        {
            source.InvokeMethod(arguments);
            return source.GetContent();
        }
        public static byte[] Clean(string fileName, List<MagickTransformer> arguments)
        {
            return Clean(new MagickImage(fileName), arguments);
        }
        public static byte[] Clean(string fileName)
        {
            return Clean(fileName, MagickTransformerFactory.Default());
        }
        public static byte[] Clean(byte[] content, List<MagickTransformer> arguments)
        {
            return Clean(new MagickImage(content), arguments);
        }
        public static byte[] Clean(byte[] content)
        {
            return Clean(content, MagickTransformerFactory.Default());
        }

    }
}
