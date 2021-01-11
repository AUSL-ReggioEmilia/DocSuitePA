using iTextSharp.text.pdf;

namespace VecompSoftware.Helpers.iTextSharp.iTextSharpEx
{
    public static class PdfContentByteExtensions
    {

        public static void TransformTo(this PdfContentByte source, PdfTransformer transformer)
        {
            var matrix = transformer.GetMatrix();
            source.AddTemplate(transformer.OriginPage, matrix);
        }

    }
}
