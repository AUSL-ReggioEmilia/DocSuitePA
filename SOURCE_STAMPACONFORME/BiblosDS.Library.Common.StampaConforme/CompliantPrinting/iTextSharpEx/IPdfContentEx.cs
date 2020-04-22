using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Linq;
using VecompSoftware.Commons.BiblosInterfaces;
using VecompSoftware.Commons.CommonEx;
using VecompSoftware.GhostscriptSharp;
using VecompSoftware.Helpers.iTextSharp;
using VecompSoftware.Helpers.iTextSharp.iTextSharpEx;

namespace VecompSoftware.CompliantPrinting
{
    public static class IPdfContentEx
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(IPdfContentEx));


        /// <summary>
        /// 
        /// Se il file va in errore, viene ritornato il file originale e viene loggato l'errore.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool TryToFlattenedPdf(this IPdfContent source, out PdfContentInfo content)
        {
            content = null;
            bool result = false;
            try
            {
                using (var gs = GhostscriptSharpHelper.GetSession().GenerateOutput(source))
                {
                    var images = gs.GetFiles()
                        .Where(f => f.EndsWith(".png"))
                        .Select(i => Image.GetInstance(i));

                    using (var doc = new Document())
                    using (var ms = new MemoryStream())
                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {
                        doc.Open();
                        foreach (var png in images)
                        {
                            png.SetAbsolutePosition(0, 0);
                            doc.NewPage(png);
                            doc.PageSize.ToA4WithRotation();
                            doc.Add(png);
                        }
                        doc.Close();
                        content = new PdfContentInfo(ms.ToDeepCopyArray());
                        result = true;
                    }
                }
            }
            catch(System.Exception ex)
            {
                string error = string.Format("Il file '{0}' ha riscontrato il seguente errore: {1}. Verrà resituita la versione originale del file.", source.FullName, ex.Message);
                logger.Error(error, ex);
                content = new PdfContentInfo(source);                
            }
            finally
            {
                logger.Debug(string.Format("TryToFlattenedPdf result: {0}.", result));
            }
            return result;
        }
    }
}
