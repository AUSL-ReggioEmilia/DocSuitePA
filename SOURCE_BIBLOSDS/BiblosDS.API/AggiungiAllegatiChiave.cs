using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Modello di richiesta per trattamenti che coinvolgono gli allegati con chiave.
    /// </summary>
    [DataContract]
    public class AggiungiAllegatoChiaveRequest : RequestBase
    {
        /// <summary>
        /// Allegato da processare.
        /// </summary>
        [DataMember]
        public FileItem Allegato { get; set; }
    }
    /// <summary>
    /// Modello di risposta in seguito alla richiesta del trattamento che coinvolge gli allegati con chiave.
    /// </summary>
    [DataContract]
    public class AggiungiAllegatoChiaveResponse : AggiungiAllegatoResponse
    {
    }

}
