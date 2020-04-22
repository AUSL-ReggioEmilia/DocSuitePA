using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.BiblosDS.WindowsService.WebAPI
{
    public class WebAPIBuilder
    {
        public static void BuildWebAPI(string webApiUrl)
        {
            WebApp.Start<Startup>(webApiUrl);
        }
    }
}
