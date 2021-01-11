using iTextSharp.text;
using iTextSharp.text.pdf;
using log4net;
using System;
using System.IO;
using VecompSoftware.Commons.BiblosDS.Objects.Converters;
using VecompSoftware.Commons.BiblosDS.Objects.Enums;
using VecompSoftware.Commons.CommonEx;
using VecompSoftware.Helpers.iTextSharp.iTextSharpEx;

namespace VecompSoftware.StampaConforme.Converter.iTextSharp
{
    public class ImageToPdfConverter : IConverter
    {
        #region [ Fields ]
        ILog _logger = LogManager.GetLogger(typeof(ImageToPdfConverter));
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public ImageToPdfConverter()
        {

        }
        #endregion

        #region [ Methods ]
        public byte[] Convert(byte[] fileSource, string fileExtension, string extReq, AttachConversionMode mode)
        {
            try
            {
                using (Document doc = new Document())
                using (MemoryStream ms = new MemoryStream())
                using (PdfWriter writer = PdfWriter.GetInstance(doc, ms))
                {
                    doc.Open();
                    Image imageInstance = Image.GetInstance(fileSource);
                    imageInstance.SetAbsolutePosition(0, 0);
                    doc.NewPage(imageInstance);
                    doc.PageSize.ToA4WithRotation();
                    doc.Add(imageInstance);
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
