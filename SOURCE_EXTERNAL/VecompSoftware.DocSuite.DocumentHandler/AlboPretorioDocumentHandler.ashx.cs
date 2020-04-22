using System;
using System.Configuration;
using System.Web;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuite.DocumentHandler
{
    /// <summary>
    /// Summary description for AlboPretorioResolutionDocumentHandler
    /// </summary>
    /// 
    public class AlboPretorioDocumentHandler : BaseDocumentHandler, IHttpHandler
    {

        private static short? _onlinePublicationInterval = null;

        public static short OnlinePublicationInterval
        {
            get
            {
                if (!_onlinePublicationInterval.HasValue)
                {
                    short onlinePublicationInterval = 15;
                    if (!short.TryParse(ConfigurationManager.AppSettings["OnlinePublicationInterval"], out onlinePublicationInterval))
                    {
                        throw new Exception("Parameter OnlinePublicationInterval doesn't have a valid short format value", null);
                    }
                    _onlinePublicationInterval = onlinePublicationInterval;
                }
                return _onlinePublicationInterval.Value;
            }
        }

        private static bool? _viewLockedPdfConfiguration = null;
        public static bool ViewLockedPdfConfiguration
        {
            get
            {
                if(!_viewLockedPdfConfiguration.HasValue)
                {
                    bool viewLockedPdfConfiguration = false;
                    if(!bool.TryParse(ConfigurationManager.AppSettings["ViewLockedPdfConfiguration"], out viewLockedPdfConfiguration))
                    {
                        return false;
                    }
                    _viewLockedPdfConfiguration = viewLockedPdfConfiguration;
                }
                return _viewLockedPdfConfiguration.Value;
            }
        }

        #region [ Constructor ]
        public AlboPretorioDocumentHandler()
            :base()
        {
            ViewLockedPdf = ViewLockedPdfConfiguration;
        }
        #endregion

        #region [ Methods ]

        public override bool CheckValidity(HttpContext context, Resolution resolution)
        {
            if (resolution == null)
            {
                ElaborateException(context);
                return false;
            }

            if (!resolution.PublishingDate.HasValue)
            {
                ElaborateException(context);
                return false;
            }

            return true;
        }
        #endregion
    }
}