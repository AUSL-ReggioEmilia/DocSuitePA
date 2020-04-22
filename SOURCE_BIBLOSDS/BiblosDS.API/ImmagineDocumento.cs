using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Modello di richiesta del file del documento.
    /// </summary>
    [DataContract]
    public class ImmagineDocumentoRequest: RequestIdBase
    {

    }

    /// <summary>
    /// Modello di risposta in seguito ad una richiesta del file rappresentante un documento.
    /// </summary>
    [DataContract]
    public class ImmagineDocumentoResponse: ResponseBase
    {
        /// <summary>
        /// File che rappresenta il documento richiesto.
        /// </summary>
        /// <remarks>La proprietà "Blob", che è un array di bytes, rappresenta il file del documento in formato binario.</remarks>
        [DataMember]
        public FileItem File { get; set; }

        /// <summary>
        /// Informazioni sul documento
        /// </summary>
        [DataMember]
        public DocumentoItem Documento { get; set; }
    }
}
