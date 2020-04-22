using System;
using System.ServiceProcess;

namespace Vecompsoftware.FileServer.Services.ActionMessages
{
	[Serializable]
	public class ServiceStatusMessage : ActionMessage
	{
        /// <summary>
        /// Il servizio (eseguibile) è presente
        /// </summary>
		public bool Exists { get; set; }

        /// <summary>
        /// Il servizio è installato
        /// </summary>
        public bool Installed { get; set; }

        /// <summary>
        /// Status effettivo del servizio
        /// </summary>
        public ServiceControllerStatus Status { get; set; }

        /// <summary>
        /// Versione del Servizio
        /// </summary>
        public string Version { get; set; }
	}
}
