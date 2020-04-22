using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Modello per la richiesta di creazione legame documenti tramite chiave.
    /// </summary>
    [DataContract]
    public class CreaLegameDocumentiChiaveRequest : RequestBase
    {      
        /// <summary>
        /// Chiave del documento.
        /// </summary>
        [DataMember]
        public string ChiaveDocumentoLink { get; set; }

        /// <summary>
        /// Tipologia.
        /// </summary>
        [DataMember]
        public string TipoDocumentoLink { get; set; }
    }
    /// <summary>
    /// Modello per la risposta al trattamento di creazione legame documenti tramite chiave.
    /// </summary>
    [DataContract]
    public class CreaLegameDocumentiChiaveResponse : ResponseBase
    {
       
    }
}
