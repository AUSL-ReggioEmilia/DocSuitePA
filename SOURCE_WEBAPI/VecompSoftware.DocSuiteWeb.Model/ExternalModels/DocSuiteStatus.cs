using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Model.ExternalModels
{
    public enum DocSuiteStatus : short
    {
        /// <summary>
        /// Non valido
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// Attivo
        /// </summary>
        Activated = 1,
        /// <summary>
        /// Anullato
        /// </summary>
        Canceled = 2,
        /// <summary>
        /// Inviato
        /// </summary>
        Sended = 4,
        /// <summary>
        /// Ricevuto
        /// </summary>
        Received = 8,
        /// <summary>
        /// Rifiutato
        /// </summary>
        Rejected = 16,
    }
}
