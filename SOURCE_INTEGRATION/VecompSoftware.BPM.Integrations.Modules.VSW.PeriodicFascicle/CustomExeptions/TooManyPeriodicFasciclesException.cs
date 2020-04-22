using System;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.PeriodicFascicle.CustomExceptions
{
    public class TooManyPeriodicFasciclesException : Exception
    {
        public TooManyPeriodicFasciclesException()
            : base()
        {
        }

        public TooManyPeriodicFasciclesException(string message)
            : base(message)
        {
        }

        public TooManyPeriodicFasciclesException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
