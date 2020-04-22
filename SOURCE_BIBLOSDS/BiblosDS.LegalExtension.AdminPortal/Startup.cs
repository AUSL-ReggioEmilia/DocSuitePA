using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BiblosDS.LegalExtension.AdminPortal.Startup))]
namespace BiblosDS.LegalExtension.AdminPortal
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
