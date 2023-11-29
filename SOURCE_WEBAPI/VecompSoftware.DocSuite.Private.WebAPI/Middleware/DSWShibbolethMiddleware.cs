using Microsoft.Owin;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.Helpers.Shibboleth;

namespace VecompSoftware.DocSuite.Private.WebAPI.Middleware
{
    public class DSWShibbolethMiddleware : OwinMiddleware
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public DSWShibbolethMiddleware(OwinMiddleware next)
            : base(next)
        {
        }
        #endregion

        #region [ Methods ]
        public override async Task Invoke(IOwinContext context)
        {
            ShibbolethServerVariablePrincipal principal = new ShibbolethServerVariablePrincipal();
            Thread.CurrentPrincipal = principal;
            HttpContext.Current.User = principal;
            await Next.Invoke(context);
        }
        #endregion        
    }
}