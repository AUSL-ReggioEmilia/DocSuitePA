using Microsoft.AspNetCore.Builder;
using VecompSoftware.DocSuite.SPID.Logging.Middleware;

namespace VecompSoftware.DocSuite.SPID.Logging.Configurations
{
    public static class LoggingApplicationBuilderExtension
    {
        public static void UseSPIDLogging(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
