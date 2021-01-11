using System;
using VecompSoftware.JeepService.Common;

namespace Vecompsoftware.FileServer.Services.Messages
{
	[Serializable]
	public class ConfigurationMessage : ActionMessage
	{
        /// <summary>
        /// Versione del Servizio
        /// </summary>
        public Configuration JeepConfig { get; set; }
	}
}
