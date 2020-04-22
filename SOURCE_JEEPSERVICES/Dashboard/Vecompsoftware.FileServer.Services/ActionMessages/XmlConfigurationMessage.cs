using System;
using VecompSoftware.JeepService.Common;

namespace Vecompsoftware.FileServer.Services.ActionMessages
{
	[Serializable]
	public class XmlConfigurationMessage : ActionMessage
	{
        /// <summary>
        /// JeepConfig.xml
        /// </summary>
        public Configuration JeepConfig { get; set; }

        /// <summary>
        /// Path del JeepConfig.xml
        /// </summary>
        public string JeepConfigPath { get; set; }
	}
}
