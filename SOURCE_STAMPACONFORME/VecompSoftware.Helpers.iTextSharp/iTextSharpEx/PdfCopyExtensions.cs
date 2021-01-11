using iTextSharp.text.pdf;

namespace VecompSoftware.Helpers.iTextSharp.iTextSharpEx
{
    public static class PdfCopyExtensions
    {

        public static void AddAllPages(this PdfCopy source, ref PdfReader reader)
        {
            foreach (var p in reader.PageNumbers())
                source.AddPage(source.GetImportedPage(reader, p));
        }

    }
}
