using iTextSharp.text.pdf;

namespace VecompSoftware.Helpers.iTextSharp.iTextSharpEx
{
    public static class PdfDictionaryExtensions
    {

        public static bool IsNullOrEmpty(this PdfDictionary source)
        {
            return source == null || source.Size <= 0;
        }

    }
}
