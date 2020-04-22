using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Informazioni del documento per chiave
    /// </summary>
    /// <remarks>
    /// Il campo <see cref="Chiave">Chiave</see> è richiesto
    /// </remarks>
    /// <example>
    ///  var request = new InfoDocumentoChiaveRequest
    ///  {    
    ///  IdClient = "ClienteTest",
    ///  IdRichiesta = "20081128000001",
    ///  IdCliente = "ClienteTest",
    ///  TipoDocumento = "DDT_ATH_TEST2",
    ///  Chiave = "IVA0000001",
    ///  Token = token.TokenInfo.Token
    ///  };
    /// </example>
    [DataContract]
    public class InfoDocumentoChiaveRequest : InfoDocumentoRequest
    {       
    }

    /// <summary>
    /// Risposta alla richiesta InfoDocumento
    /// </summary>
    /// <remarks>
    /// La richiesta ha successo se il codice della risposta <see cref="CodiceEsito">CodiceEsito</see> è <see cref="CodiceErrore">CodiceErrore.NessunErrore</see>; 
    /// </remarks>
    [DataContract]
    public class InfoDocumentoChiaveResponse : InfoDocumentoResponse
    {
    }
}