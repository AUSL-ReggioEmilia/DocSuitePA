using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using VecompSoftware.DocSuite.WebAPI.Common.Configurations;

namespace VecompSoftware.DocSuite.Public.WebAPI.Handlers
{
    public class AzureAuthorizeAttribute : AuthorizeAttribute
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public AzureAuthorizeAttribute()
        {
        }
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Methods ]
        public override void OnAuthorization(HttpActionContext context)
        {
            if (!WebApiConfiguration.AzureEnabled)
            {
                return;
            }
            base.OnAuthorization(context);
        }

        #endregion
    }
}