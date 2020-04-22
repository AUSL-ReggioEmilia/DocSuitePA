using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Modello di richiesta per la cancellazione di un allegato appartenente ad un dato documento.
    /// </summary>
    [DataContract]
    public class CancellaAllegatoDocumentoRequest : RequestIdBase
    {
        [DataMember]
        public Guid IdAllegato { get; set; }
    }
    /// <summary>
    /// Modello di risposta alle operazione di cancellazione degli allegati di un documento
    /// </summary>
    [DataContract]
    public class CancellaAllegatoDocumentoResponse : ResponseBase
    {
    }
}
