using VecompSoftware.Commons.BiblosDS.Objects.Enums;
using VecompSoftware.Commons.Configuration;
using VecompSoftware.Helpers.iTextSharp;

namespace VecompSoftware.CompliantPrinting
{
    public class CompliantTransformer
    {

        #region [ Constructors ]

        public CompliantTransformer()
        {
            RenderSignatures = true;
            PdfTransformer = PdfTransformerFactory.Default();
        }

        #endregion

        #region [ Fields ]

        private AttachConversionMode _attachConversionMode;
        private PdfTransformer _pdfTransformer;

        #endregion

        #region [ Properties ]

        public AttachConversionMode AttachConversionMode
        {
            get
            {
                if (_attachConversionMode == AttachConversionMode.Default)
                    return ConfigurationHelper.GetValueOrDefault<AttachConversionMode>("DefaultAttachConversionMode", AttachConversionMode.Default);
                return _attachConversionMode;
            }
            set { _attachConversionMode = value; }
        }

        public bool RenderSignatures { get; set; }

        public PdfTransformer PdfTransformer
        {
            get
            {
                _pdfTransformer.PdfA = _pdfTransformer.PdfA && !ConfigurationHelper.GetValueOrDefault<bool>("CompliantPrinting.DisablePdfAConversion");
                _pdfTransformer.UnethicalReading = _pdfTransformer.UnethicalReading && ConfigurationHelper.GetValueOrDefault<bool>("CompliantPrinting.EnableUnethicalReading");

                return _pdfTransformer;
            }
            set { _pdfTransformer = value; }
        }

        public string CustomStyleContent { get; set; }

        #endregion

    }
}
