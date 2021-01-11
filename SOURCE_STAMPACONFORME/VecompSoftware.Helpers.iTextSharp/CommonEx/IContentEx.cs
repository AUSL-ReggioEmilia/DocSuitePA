using VecompSoftware.Commons.BiblosInterfaces;

namespace VecompSoftware.Helpers.iTextSharp.CommonEx
{
    public static class IContentEx
    {

        public static PdfContentInfo ToPdfContent(this IContent source)
        {
            return new PdfContentInfo(source);
        }

    }
}
