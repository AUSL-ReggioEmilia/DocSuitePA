using System;
using System.Web;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuite.DocumentHandler
{
    /// <summary>
    /// Summary description for PublishedResolutionDocumentHandler
    /// </summary>
    public class PublishedResolutionDocumentHandler : BaseDocumentHandler, IHttpHandler
    {
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

            if (resolution.EffectivenessDate.HasValue && resolution.EffectivenessDate.Value <= DateTime.UtcNow.Date.AddDays(1).AddMilliseconds(-3))
            {
                ElaborateException(context);
                return false;
            }

            return true;
        }
        #endregion
    }
}