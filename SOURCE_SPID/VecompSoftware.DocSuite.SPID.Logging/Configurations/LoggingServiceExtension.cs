using Microsoft.Extensions.DependencyInjection;
using VecompSoftware.DocSuite.SPID.Logging.Core;

namespace VecompSoftware.DocSuite.SPID.Logging.Configurations
{
    public static class LoggingServiceExtension
    {
        public static void AddSPIDLogging(this IServiceCollection services)
        {
            services.AddTransient<ILoggingService, LoggingService>();
        }
    }
}
