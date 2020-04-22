using System;
using System.Drawing.Drawing2D;
using iTextSharp.text;
using iTextSharp.text.pdf;
using VecompSoftware.Helpers.iTextSharp.iTextSharpEx;

namespace VecompSoftware.Helpers.iTextSharp
{
    public class PdfTransformer
    {

        #region [ Constructors ]

        public PdfTransformer(PdfImportedPage origin, Rectangle originPageSize)
        {
            SetOrigin(origin, originPageSize);
        }
        public PdfTransformer()
        {
            SetOrigin(null, null);
        }

        #endregion

        #region [ Fields ]

        private PdfImportedPage _originPage;
        private Rectangle _originPageSize;
        private Rectangle _destinationPageSize;
        private int? _contentScaling;

        #endregion

        #region [ Properties ]

        public PdfImportedPage OriginPage
        {
            get
            {
                if (_originPage == null)
                    throw new ArgumentNullException("VecompSoftware.iTextSharp: OriginPage non inizializzato.");
                return _originPage;
            }
        }
        public Rectangle OriginPageSize
        {
            get
            {
                if (_originPageSize == null)
                    throw new ArgumentNullException("VecompSoftware.iTextSharp: OriginPageSize non inizializzato.");
                return _originPageSize;
            }
        }

        public Rectangle DestinationPageSize
        {
            get
            {
                if (_destinationPageSize == null)
                    return OriginPageSize.Replicate();

                _destinationPageSize = OriginPageSize.ToPageSizeWithRotation(_destinationPageSize);
                return _destinationPageSize;
            }
            set { _destinationPageSize = value; }
        }
        public int ContentScaling
        {
            get { return _contentScaling.GetValueOrDefault(100); }
            set { _contentScaling = value; }
        }
        public bool PdfA { get; set; }

        public bool UnethicalReading { get; set; }

        #endregion

        #region [ Methods ]

        public PdfTransformer SetOrigin(PdfImportedPage originPage, Rectangle originPageSize)
        {
            _originPage = originPage;
            _originPageSize = originPageSize;
            return this;
        }
        public PdfTransformer ClearScaling()
        {
            _destinationPageSize = null;
            _contentScaling = null;
            return this;
        }
        public Matrix GetMatrix()
        {
            float widthFactor;
            float heightFactor;
            float factor;

            var scaleFactor = (float)ContentScaling / 100;
            var scaleX = 1F;
            var scaleY = scaleFactor;

            switch (DestinationPageSize.Rotation)
            {
                case 90:
                    widthFactor = DestinationPageSize.Width / OriginPage.Height;
                    heightFactor = DestinationPageSize.Height / OriginPage.Width;
                    factor = Math.Min(widthFactor, heightFactor);
                    factor = (float)Math.Round(factor, 2);
                    return new Matrix(0, -factor * scaleX, factor * scaleY, 0, 0, DestinationPageSize.Height * scaleFactor);

                case 180:
                    widthFactor = DestinationPageSize.Height / OriginPage.Height;
                    heightFactor = DestinationPageSize.Width / OriginPage.Width;
                    factor = Math.Min(widthFactor, heightFactor);
                    factor = (float)Math.Round(factor, 2);
                    return new Matrix(-factor * scaleX, 0, 0, -factor * scaleY, DestinationPageSize.Width, DestinationPageSize.Height * scaleFactor);

                case 270:
                    widthFactor = DestinationPageSize.Width / OriginPage.Height;
                    heightFactor = DestinationPageSize.Height / OriginPage.Width;
                    factor = Math.Min(widthFactor, heightFactor);
                    factor = (float)Math.Round(factor, 2);
                    return new Matrix(0, factor * scaleY, -factor * scaleX, 0, DestinationPageSize.Width, 0);

                default:
                    widthFactor = DestinationPageSize.Width / OriginPage.Width;
                    heightFactor = DestinationPageSize.Height / OriginPage.Height;
                    factor = Math.Min(widthFactor, heightFactor);
                    factor = (float)Math.Round(factor, 2);
                    return new Matrix(factor * scaleX, 0, 0, factor * scaleY, 0, 0);
            }
        }

        #endregion

    }
}
