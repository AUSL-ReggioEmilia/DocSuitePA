using System;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.PeriodicFascicle.CustomExceptions
{
    public class PeriodicFascicleNotFoundException : Exception
    {
        public PeriodicFascicleNotFoundException()
        {
        }

        public PeriodicFascicleNotFoundException(string message)
            : base(message)
        {
        }

        public PeriodicFascicleNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
