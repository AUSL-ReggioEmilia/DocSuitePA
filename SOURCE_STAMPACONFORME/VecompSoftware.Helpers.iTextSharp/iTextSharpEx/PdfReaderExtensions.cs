using System.Collections.Generic;
using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using VecompSoftware.Commons.CommonEx;

namespace VecompSoftware.Helpers.iTextSharp.iTextSharpEx
{
    public static class PdfReaderExtensions
    {

        public static void CloseQuietly(this PdfReader source)
        {
            if (source == null)
                return;
            source.Close();
        }

        public static bool HasAnnotations(this PdfReader source, int pageNum)
        {
            var dict = source.GetPageN(pageNum);
            if (dict.IsNullOrEmpty())
                return false;

            var annots = dict.GetAsArray(PdfName.ANNOTS);
            if (annots.IsNullOrEmpty())
                return false;

            return annots.Length > 0;
        }
        public static bool HasAnnotations(this PdfReader source)
        {
            return source.PageNumbers().Any(source.HasAnnotations);
        }

        public static bool XfaPresent(this PdfReader source)
        {
            var xfa = new XfaForm(source);
            return xfa.XfaPresent;
        }

        public static IEnumerable<int> PageNumbers(this PdfReader source)
        {
            return Enumerable.Range(1, source.NumberOfPages);
        }

        public static string GetTextFromPage(this PdfReader source, int pageNum)
        {
            return PdfTextExtractor.GetTextFromPage(source, pageNum, new SimpleTextExtractionStrategy());
        }
        public static Dictionary<int, string> GetTextFromAllPages(this PdfReader source)
        {
            return source.PageNumbers().ToDictionary(k => k, v => source.GetTextFromPage(v));
        }

        public static bool IsScanned(this PdfReader source, int pageNum)
        {
            return string.IsNullOrEmpty(source.GetTextFromPage(pageNum));
        }
        public static IEnumerable<int> GetScannedPageNumbers(this PdfReader source)
        {
            return source.PageNumbers().Where(p => source.IsScanned(p));
        }


    }
}