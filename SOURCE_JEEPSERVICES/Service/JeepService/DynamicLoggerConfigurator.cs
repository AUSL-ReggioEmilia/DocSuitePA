using System;
using log4net;
using log4net.Repository.Hierarchy;
using log4net.Core;
using log4net.Appender;
using log4net.Layout;

namespace JeepService
{
    public class DynamicLoggerConfigurator
    {
        public static IAppender AddAppender(string appenderName, int maxBakups = 10, string maxFileSize = "1024KB")
        {
            var patternLayout = new PatternLayout
            {
                ConversionPattern = "%d{yyyy-MM-dd HH:mm:ss.fff} [%t] %-5p %c - %m%n"
            };
            patternLayout.ActivateOptions();

            var roller = new RollingFileAppender
            {
                Name = appenderName,
                AppendToFile = true,
                File = String.Format(@"logs/{0}.log", appenderName),
                Layout = patternLayout,
                MaxSizeRollBackups = maxBakups,
                MaximumFileSize = maxFileSize,
                RollingStyle = RollingFileAppender.RollingMode.Size,
                StaticLogFileName = true,
                LockingModel = new FileAppender.MinimalLock()
            };
            roller.ActivateOptions();
            return roller;
        }

        public static void AddLogger(string loggerName, IAppender appender, Level logLevel = null)
        {
            var logger = (Logger)LogManager.GetLogger(loggerName).Logger;
            logger.AddAppender(appender);
            logger.Level = logLevel ?? Level.Info;
        }

        public static void AddCompleteLogger(string loggerName, Level logLevel = null, int maxBakups = 10,
            string maxFileSize = "1024KB")
        {
            AddLogger(loggerName, AddAppender(loggerName), logLevel);
        }

        public static void Configured()
        {
            ((Hierarchy)LogManager.GetRepository()).Configured = true;
        }
    }
}