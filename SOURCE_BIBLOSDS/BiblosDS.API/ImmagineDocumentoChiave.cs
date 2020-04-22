using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Richiesta del contenuto di un documento (il file che lo rappresenta) a partire dalla chiave.
    /// </summary>
    [DataContract]
    public class ImmagineDocumentoChiaveRequest: RequestBase
    {
    }

    /// <summary>
    /// Risposta alle richieste del contenuto di un documento a partire dalla chiave.
    /// </summary>
    [DataContract]
    public class ImmagineDocumentoChiaveResponse : ResponseBase
    {
        /// <summary>
        /// File
        /// </summary>
        [DataMember]
        public FileItem File { get; set; }

        /// <summary>
        /// Informazioni sul documento
        /// </summary>
        [DataMember]
        public DocumentoItem Documento { get; set; }
    }
}
