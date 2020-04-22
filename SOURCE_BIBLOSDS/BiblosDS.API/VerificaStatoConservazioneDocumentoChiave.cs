using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    [DataContract]
    public class VerificaStatoConservazioneDocumentoChiaveRequest : RequestBase
    {
    }

    [DataContract]
    public class VerificaStatoConservazioneDocumentoChiaveResponse : VerificaStatoConservazioneDocumentoResponse
    {
        
    }

     [DataContract]
    public class VerificaStatoConservazioneResponse : ResponseBase
    {        
        /// <summary>
        /// Elenco dei metadati rappresentanti le chiavi del documento.
        /// </summary>
        [DataMember]         
        public StatoDocumento StatoDocumento { get; set; }
      
        [DataMember]
        public bool Conservato { get; set; }
    }
}
