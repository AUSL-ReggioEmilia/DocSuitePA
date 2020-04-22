using System.Collections.Generic;
using ImageMagick;

namespace VecompSoftware.MagickNET
{
    public static class ListExtensions
    {

        public static List<MagickTransformer> Add(this List<MagickTransformer> source, string name, object[] parameters)
        {
            if (source == null)
                source = new List<MagickTransformer>();

            source.Add(new MagickTransformer { Name = name, Parameters = parameters });
            return source;
        }
        public static List<MagickTransformer> Add(this List<MagickTransformer> source, string name, object parameter)
        {
            return source.Add(name, new object[] { parameter });
        }
        public static List<MagickTransformer> Add(this List<MagickTransformer> source, string name)
        {
            return source.Add(name, null);
        }

        public static List<MagickTransformer> AddDeskew(this List<MagickTransformer> source, int threshold)
        {
            return source.Add("Deskew", new Percentage(threshold));
        }
        

    }
}
