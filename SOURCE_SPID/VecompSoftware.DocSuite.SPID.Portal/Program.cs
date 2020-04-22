using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System.Net;

namespace VecompSoftware.DocSuite.SPID.Portal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NLogBuilder.ConfigureNLog("NLog.config");
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
#if DEBUG
            builder.UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, 5000);
                    options.Listen(IPAddress.Loopback, 44355, listenOptions =>
                    {
                        listenOptions.UseHttps("cert/mycertdevelopers.pfx", "P@ssw0rd!");
                        //listenOptions.UseConnectionLogging();
                    });
                });
#endif
            return builder
                .UseWebRoot("wwwroot/dist")
                .Build();
        }
    }
}
