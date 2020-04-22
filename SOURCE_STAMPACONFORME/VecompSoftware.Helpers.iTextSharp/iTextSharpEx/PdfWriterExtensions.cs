using iTextSharp.text.pdf;

namespace VecompSoftware.Helpers.iTextSharp.iTextSharpEx
{
    public static class PdfWriterExtensions
    {

        public static void CloseQuietly(this PdfWriter source)
        {
            if (source == null)
                return;
            source.Close();
        }

    }
}
