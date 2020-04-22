using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Richiesta di informazione allegati documenti
    /// </summary>
    [DataContract]
    public class InfoAllegatiRequest : RequestIdBase
    {
    }

    /// <summary>
    /// Risposta richiesta allegati documenti
    /// </summary>
    [DataContract]
    public class InfoAllegatiResponse : ResponseBase
    {
        /// <summary>
        /// Elenco degli allegati associati al documento richiesto.
        /// </summary>
        [DataMember]
        public List<AllegatoItem> Allegati { get; set; }
    }

    /// <summary>
    /// Metadato
    /// </summary>
    [DataContract]
    public sealed class AllegatoItem
    {
        [DataMember]
        public Guid IdAllegato { get; set; }
        [DataMember]
        public string Nome { get; set; }
        [DataMember]
        public Operazione Stato { get; set; }     
    }
}
