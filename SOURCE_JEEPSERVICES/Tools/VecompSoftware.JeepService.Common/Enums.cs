using log4net.Core;

namespace VecompSoftware.JeepService.Common
{
    public enum TimerType
    {
        Single,
        Multiple,
        Timerange
    }

    public enum LogLevel
    {
        Info,
        Error,
        All,
        Debug,
        Off,
        Warn
    }

    public static class Enums
    {
        public static Level ToLevel(this LogLevel source)
        {
            switch (source)
            {
                case LogLevel.All:
                    return Level.All;
                case LogLevel.Debug:
                    return Level.Debug;
                case LogLevel.Error:
                    return Level.Error;
                case LogLevel.Info:
                    return Level.Info;
                case LogLevel.Off:
                    return Level.Off;
                case LogLevel.Warn:
                    return Level.Warn;
                default:
                    return Level.Info;
            }
        }
    }
}
