using System;

namespace VecompSoftware.JeepService.PosteWeb
{
    /// <summary> Errori gestiti del servizio. </summary>
    public class PosteWebException : Exception
    {
        public PosteWebException(string format, params object[] args) : base(string.Format(format, args)) { }
    }
}
