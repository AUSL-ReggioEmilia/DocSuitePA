using System;
using System.Collections.Generic;

namespace Vecompsoftware.FileServer.Services.ActionMessages
{
	[Serializable]
	public class DictionaryMessage : ActionMessage
	{
        /// <summary>
        /// FileVersionInfo dell'assembly
        /// </summary>
		public Dictionary<String,String> Properties { get; set; }

        public DictionaryMessage()
        {
            Properties = new Dictionary<string, string>();
        }
	}
}
