using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(VecompSoftware.BiblosDS.WindowsService.WebAPI.Startup))]

namespace VecompSoftware.BiblosDS.WindowsService.WebAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAPI(app);
        }
    }
}
