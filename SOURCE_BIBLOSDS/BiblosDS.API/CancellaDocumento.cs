using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Cancella documento a partire dall'id
    /// </summary>
    /// <example>
    ///  var request = new CancellaDocumentoRequest
    ///  {
    ///  Chiave = "IVA0000001",
    ///  IdClient = "desktop",
    ///  IdRichiesta = "20081128000001",
    ///  IdCliente = "ClienteTest",
    ///  TipoDocumento = "DDT_ATH_TEST2",
    ///  IdDocumento = Guid.NewGuid(),
    ///  Token = token.TokenInfo.Token
    ///  };
    /// </example>
    [DataContract]
    public class CancellaDocumentoRequest : RequestIdBase
    {
        
    }

    /// <summary>
    /// Risposta alla richiesta di cancellazione documento da IdDocumento
    /// </summary>
    /// <remarks>
    /// La richiesta ha successo se il codice della risposta <see cref="CodiceEsito">CodiceEsito</see> è <see cref="CodiceErrore">CodiceErrore.NessunErrore</see>; 
    /// </remarks>
    [DataContract]
    public class CancellaDocumentoResponse : ResponseBase
    {
        [DataMember]
        public DocumentoItem Documento { get; set; }
    }
}