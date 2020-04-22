using iTextSharp.text;

namespace VecompSoftware.Helpers.iTextSharp
{
    public static class PdfTransformerFactory
    {

        public static PdfTransformer Default()
        {
            return new PdfTransformer();
        }
        public static PdfTransformer ToA4Size()
        {
            return new PdfTransformer() { DestinationPageSize = PageSize.A4 };
        }

    }
}
