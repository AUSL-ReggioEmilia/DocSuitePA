using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Informazioni su un documento per chiave
    /// </summary>
    /// <example>
    /// var request = new InfoDocumentoRequest
    /// {
    /// IdClient = "desktop",
    /// IdRichiesta = "20081128000001",
    /// IdCliente = "ClienteTest",
    /// TipoDocumento = "DDT_ATH_TEST2",
    /// Token = token.TokenInfo.Token,
    /// IdDocumento = new Guid("d4d7a0dc-5cf2-4813-a1d7-0006b53c7799"),
    /// };
    /// </example>
    /// <remarks>
    /// Il campo <see cref="IdDocumento">IdDocumento</see> è richiesto
    /// </remarks>
    [DataContract]
    public class InfoDocumentoRequest : RequestIdBase
    {        
    }

    /// <summary>
    /// Risposta alla richiesta InfoDocumento
    /// </summary>
    /// <remarks>
    /// La richiesta ha successo se il codice della risposta <see cref="CodiceEsito">CodiceEsito</see> è <see cref="CodiceErrore">CodiceErrore.NessunErrore</see>; 
    /// </remarks>
    [DataContract]
    public class InfoDocumentoResponse : ResponseBase
    {
        [DataMember]
        public StatoDocumento StatoDocumento { get; set; }
        [DataMember]
        public DocumentoItem Documento { get; set; }
    }
}