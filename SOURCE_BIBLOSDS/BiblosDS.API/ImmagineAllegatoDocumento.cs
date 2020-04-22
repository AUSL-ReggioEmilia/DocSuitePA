using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Esito della richiesta contenuto allegato documento.
    /// </summary>
    [DataContract]
    public class ImmagineAllegatoDocumentoResponse : ResponseBase
    {
        /// <summary>
        /// File (vedere proprietà "Blob").
        /// </summary>
        [DataMember]
        public FileItem File { get; set; }

        /// <summary>
        /// Informazioni sull'allegato
        /// </summary>
        [DataMember]
        public AllegatoItem Allegato { get; set; }
    }

    /// <summary>
    /// Richiede il contenuto (array di bytes) dell'allegato documento.
    /// </summary>
    [DataContract]
    public class ImmagineAllegatoDocumentoRequest: RequestBase
    {
        [DataMember]
        public Guid IdAllegato { get; set; }
    }
}
