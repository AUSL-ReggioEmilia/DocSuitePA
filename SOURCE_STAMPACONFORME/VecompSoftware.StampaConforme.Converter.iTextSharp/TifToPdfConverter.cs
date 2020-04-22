using iTextSharp.text;
using iTextSharp.text.pdf;
using log4net;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using VecompSoftware.Commons.BiblosDS.Objects.Converters;
using VecompSoftware.Commons.BiblosDS.Objects.Enums;
using VecompSoftware.Commons.CommonEx;
using VecompSoftware.Helpers.iTextSharp.iTextSharpEx;
using iTextSharpText = iTextSharp.text;

namespace VecompSoftware.StampaConforme.Converter.iTextSharp
{
    public class TifToPdfConverter : IConverter
    {
        #region [ Fields ]
        ILog _logger = LogManager.GetLogger(typeof(TifToPdfConverter));
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public TifToPdfConverter()
        {

        }
        #endregion

        #region [ Methods ]
        public byte[] Convert(byte[] fileSource, string filename, string extReq, AttachConversionMode mode)
        {
            try
            {
                using (Document doc = new Document())
                using (MemoryStream ms = new MemoryStream())
                using (PdfWriter writer = PdfWriter.GetInstance(doc, ms))
                using (MemoryStream bitmapStream = new MemoryStream(fileSource))
                {
                    doc.Open();
                    Bitmap bitmapImage = new Bitmap(bitmapStream);
                    int bitmapPages = bitmapImage.GetFrameCount(FrameDimension.Page);
                    iTextSharpText.Image imageInstance;
                    for (int page = 0; page < bitmapPages; page++)
                    {
                        bitmapImage.SelectActiveFrame(FrameDimension.Page, page);
                        using (MemoryStream imageStream = new MemoryStream())
                        {
                            try
                            {
                                bitmapImage.Save(imageStream, ImageFormat.Png);
                                imageInstance = iTextSharpText.Image.GetInstance(imageStream.GetBuffer());
                            }
                            catch
                            {
                                bitmapImage.Save(imageStream, ImageFormat.Tiff);
                                imageInstance = iTextSharpText.Image.GetInstance(imageStream.GetBuffer());
                            }
                        }

                        imageInstance.SetAbsolutePosition(0, 0);
                        doc.NewPage(imageInstance);
                        doc.PageSize.ToA4WithRotation();
                        doc.Add(imageInstance);
                    }
                    doc.Close();
                    return ms.ToDeepCopyArray();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        public string GetVersion()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
