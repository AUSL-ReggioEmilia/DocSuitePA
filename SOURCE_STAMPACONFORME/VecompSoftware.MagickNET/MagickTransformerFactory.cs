using System.Collections.Generic;

namespace VecompSoftware.MagickNET
{
    public static class MagickTransformerFactory
    {

        public static List<MagickTransformer> Default()
        {
            return new List<MagickTransformer>()
                .Add("Crop", 1000)
                .Add("ReduceNoise", 5)
                .Add("Despeckle")
                .Add("Normalize")
                .Add("Clarify", 25000)
                .Add("Sharpen")
                .Add("Contrast")
                .AddDeskew(40);
        }

    }
}
