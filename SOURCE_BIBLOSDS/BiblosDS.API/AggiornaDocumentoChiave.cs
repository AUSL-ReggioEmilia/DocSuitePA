using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Richiesta di aggiornamento documento tramite chiave    
    /// </summary>
    /// <remarks>
    /// Il campo <see cref="Chiave">Chiave</see> deve essere valorizzato
    /// </remarks>
    /// <example>
    /// var request = new AggiornaDocumentoChiaveRequest
    /// {
    /// IdDocumento = new Guid("d4d7a0dc-5cf2-4813-a1d7-0006b53c7799"),
    /// IdClient = "ClienteTest",
    /// IdRichiesta = "20081128000001",
    /// IdCliente = "ClienteTest",
    /// TipoDocumento = "DDT_ATH_TEST2",
    /// Chiave = "IVA0000001",
    /// Token = token.TokenInfo.Token
    /// };
    /// </example>
    [DataContract]
    public class AggiornaDocumentoChiaveRequest : RequestBase
    {
        public AggiornaDocumentoChiaveRequest(): base()
        {
            Metadati = new List<MetadatoItem>();
        }
        [DataMember]
        public bool ForzaModifica { get; set; }
        [DataMember]
        public List<MetadatoItem> Metadati { get; set; }
        [DataMember]
        public FileItem File { get; set; }
    }

    /// <summary>
    /// Risposta alla richiesta AggiornaDocumentoChiave
    /// </summary>
    /// <remarks>
    /// La richiesta ha successo se il codice della risposta <see cref="CodiceEsito">CodiceEsito</see> è <see cref="CodiceErrore">CodiceErrore.NessunErrore</see>; 
    /// </remarks>
    [DataContract]
    public class AggiornaDocumentoChiaveResponse : AggiornaDocumentoResponse
    {        
    }
}