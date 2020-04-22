using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(VecompSoftware.DocSuite.Private.WebAPI.Startup))]

namespace VecompSoftware.DocSuite.Private.WebAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            ConfigureAPI(appBuilder);
        }
    }
}
