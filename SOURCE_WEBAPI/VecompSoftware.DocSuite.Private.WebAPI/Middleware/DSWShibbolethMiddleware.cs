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
        private readonly IParameterEnvService _parameterEnvService;
        #endregion

        #region [ Constructor ]
        public DSWShibbolethMiddleware(OwinMiddleware next)
            : base(next)
        {
            _parameterEnvService = (IParameterEnvService)UnityConfig.GetConfiguredContainer().GetService(typeof(IParameterEnvService));
        }
        #endregion

        #region [ Methods ]
        public override async Task Invoke(IOwinContext context)
        {
            if (_parameterEnvService.ShibbolethEnabled)
            {
                ShibbolethServerVariablePrincipal principal = new ShibbolethServerVariablePrincipal();
                Thread.CurrentPrincipal = principal;
                HttpContext.Current.User = principal;
            }
            await Next.Invoke(context);
        }
        #endregion        
    }
}