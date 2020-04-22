using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Cancellazione documento da chiave
    /// </summary>
    /// <example>
    ///  var request = new CancellaDocumentoChiaveRequest
    ///  {
    ///  IdClient = "ClienteTest",
    ///  IdRichiesta = "20081128000001",
    ///  IdCliente = "ClienteTest",
    ///  TipoDocumento = "DDT_ATH_TEST2",
    ///  Chiave = "IVA0000001",
    ///  Token = token.TokenInfo.Token
    ///  };
    /// </example>
    /// <remarks>
    /// Il campo <see cref="Chiave">Chiave</see> deve essere popolato
    /// </remarks>
    [DataContract]
    public class CancellaDocumentoChiaveRequest : RequestBase
    {
        
    }

    /// <summary>
    /// Risposta alla richiesta di cancellazione documento da chiave.
    /// </summary>
    /// <remarks>
    /// La richiesta ha successo se il codice della risposta <see cref="CodiceEsito">CodiceEsito</see> è <see cref="CodiceErrore">CodiceErrore.NessunErrore</see>; 
    /// </remarks>
    [DataContract]
    public class CancellaDocumentoChiaveResponse : CancellaDocumentoResponse
    {        
    }
}