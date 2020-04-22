using VecompSoftware.Commons.BiblosDS.Objects.Enums;
using VecompSoftware.Commons.Configuration;
using VecompSoftware.Helpers.iTextSharp;

namespace VecompSoftware.CompliantPrinting
{
    public static class CompliantTransformerFactory
    {

        public static CompliantTransformer Default()
        {
            var result = new CompliantTransformer()
            {
                AttachConversionMode = AttachConversionMode.Default,
                PdfTransformer = PdfTransformerFactory.Default()
            };
            result.PdfTransformer.UnethicalReading = ConfigurationHelper.GetValueOrDefault<bool>("CompliantPrinting.EnableUnethicalReading");
            return result;
        }
        public static CompliantTransformer ToA4Size()
        {
            var result = new CompliantTransformer()
            {
                AttachConversionMode = AttachConversionMode.Default,
                PdfTransformer = PdfTransformerFactory.ToA4Size()
            };
            result.PdfTransformer.UnethicalReading = ConfigurationHelper.GetValueOrDefault<bool>("CompliantPrinting.EnableUnethicalReading");
            return result;
        }

    }
}
