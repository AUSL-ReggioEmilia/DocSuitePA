using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Modello per la richiesta di manipolazione allegati.
    /// </summary>
    [DataContract]
    public class AggiungiAllegatoRequest : RequestIdBase
    {
        /// <summary>
        /// Allegato da processare.
        /// </summary>
        [DataMember]
        public FileItem Allegato { get; set; }
    }
    /// <summary>
    /// Modello di risposta per il ritorno da un'operazione di manipolazione allegati.
    /// </summary>
    [DataContract]
    public class AggiungiAllegatoResponse : ResponseBase
    {
    }
}
