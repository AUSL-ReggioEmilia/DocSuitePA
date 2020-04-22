using System;

namespace Vecompsoftware.FileServer.Services.ActionMessages
{
	[Serializable]
	public class ActionMessage
	{
        /// <summary>
        /// Messaggio generato come esito di un'azione
        /// </summary>
		public string Message { get; set; }
	}
}
