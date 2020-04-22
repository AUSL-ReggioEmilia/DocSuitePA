using iTextSharp.text;
using iTextSharp.text.pdf;
using log4net;
using System;
using System.IO;
using VecompSoftware.Commons.BiblosDS.Objects.Converters;
using VecompSoftware.Commons.BiblosDS.Objects.Enums;
using VecompSoftware.Commons.CommonEx;

namespace VecompSoftware.StampaConforme.Converter.iTextSharp
{
    public class TxtToPdfConverter : IConverter
    {
        #region [ Fields ]
        ILog _logger = LogManager.GetLogger(typeof(ImageToPdfConverter));
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public TxtToPdfConverter()
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
                    using (MemoryStream txtStream = new MemoryStream(fileSource))
                    using (TextReader reader = new StreamReader(txtStream))
                    {
                        doc.Add(new Paragraph(reader.ReadToEnd()));
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
