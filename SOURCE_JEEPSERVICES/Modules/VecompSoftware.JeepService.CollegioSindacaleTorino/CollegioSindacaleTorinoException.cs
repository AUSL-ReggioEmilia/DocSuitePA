using System;

namespace VecompSoftware.JeepService.CollegioSindacaleTorino
{
    /// <summary> Errori gestiti del servizio. </summary>
    public class CollegioSindacaleTorinoException : Exception
    {
        public CollegioSindacaleTorinoException(string message, Exception exception) : base(message, exception) { }

        public CollegioSindacaleTorinoException(string format, params object[] args) : base(string.Format(format, args)) { }
    }
}
