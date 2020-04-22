using System;

namespace Vecompsoftware.FileServer.Services.ActionMessages
{
	[Serializable]
	public class ServiceConfigurationMessage : ActionMessage
	{
        /// <summary>
        /// JeepService.exe.config
        /// </summary>
        public byte[] JeepServiceConfig { get; set; }

        /// <summary>
        /// Path del JeepService.exe
        /// </summary>
        public string JeepServicePath { get; set; }

        /// <summary>
        /// Definisce se la configurazione sia protetta o meno
        /// </summary>
        public bool IsProtected { get; set; }
	}
}
