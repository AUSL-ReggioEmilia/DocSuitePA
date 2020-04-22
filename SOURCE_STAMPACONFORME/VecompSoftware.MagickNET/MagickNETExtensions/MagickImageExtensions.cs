using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ImageMagick;
using VecompSoftware.Commons;
using VecompSoftware.Commons.CommonEx;

namespace VecompSoftware.MagickNET
{
    public static class MagickImageExtensions
    {

        public static byte[] GetContent(this MagickImage source)
        {
            using (var ms = new MemoryStream())
            {
                source.Write(ms);
                return ms.ToDeepCopyArray();
            }
        }

        public static MagickImage Clarify(this MagickImage source, double fuzz)
        {
            source.ColorFuzz = new Percentage(fuzz);
            source.Opaque(Color.DarkGray, Color.White);
            return source;
        }
        public static MagickImage Crop(this MagickImage source, int side)
        {
            source.Crop(side, side);
            return source;
        }
        public static MagickImage InvokeMethod(this MagickImage source, MagickTransformer arguments)
        {
            ReflectionHelper.InvokeMethod(source, arguments.Name, arguments.Parameters, typeof(MagickImageExtensions));
            return source;
        }
        public static MagickImage InvokeMethod(this MagickImage source, List<MagickTransformer> arguments)
        {
            foreach (var transformer in arguments)
                source = source.InvokeMethod(transformer);
            return source;
        }

    }
}
