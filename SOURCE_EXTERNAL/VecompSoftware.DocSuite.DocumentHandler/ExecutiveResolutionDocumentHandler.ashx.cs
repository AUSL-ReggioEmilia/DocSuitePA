using System.Web;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuite.DocumentHandler
{
    /// <summary>
    /// Summary description for ExecutiveResolutionDocumentHandler
    /// </summary>
    public class ExecutiveResolutionDocumentHandler : BaseDocumentHandler, IHttpHandler
    {
        #region [ Methods ]

        public override bool CheckValidity(HttpContext context, Resolution resolution)
        {
            if (resolution == null)
            {
                ElaborateException(context);
                return false;
            }

            if (!resolution.EffectivenessDate.HasValue)
            {
                ElaborateException(context);
                return false;
            }
            return true;
        }
        #endregion
    }
}