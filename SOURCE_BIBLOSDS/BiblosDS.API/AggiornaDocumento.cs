using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Request di aggiornamento documento
    /// </summary>
    /// <example>
    ///  var request = new AggiornaDocumentoRequest
    ///  {
    ///  IdClient = "desktop",
    ///  IdRichiesta = "20081128000001",
    ///  IdCliente = "TeraSoftware",
    ///  TipoDocumento = "DDT_ATH_TEST2",    
    ///  IdDocumento = new Guid("dbd44243-d746-4e5d-b3c4-2caf4cf9ac32"),
    ///  Token = token.TokenInfo.Token,
    ///  File = new FileItem
    ///  {
    ///  Nome = "pippo.txt",
    ///  Blob = File.ReadAllBytes(@"C:\Lavori\Docs\BiblosDS\original.pdf.PDF")
    ///  }
    ///  };
    /// </example>
    /// <remarks>
    /// il campo <see cref="IdDocumento">IdDocumento</see> deve essere popolato.
    /// </remarks>
    [DataContract]
    public class AggiornaDocumentoRequest : RequestIdBase
    {
        public AggiornaDocumentoRequest()
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
    /// Risposta della richiesta di aggiornamento documento.
    /// </summary>
    /// <remarks>
    /// La richiesta ha successo se il codice della risposta <see cref="CodiceEsito">CodiceEsito</see> è <see cref="CodiceErrore">CodiceErrore.NessunErrore</see>; 
    /// </remarks>
    [DataContract]
    public class AggiornaDocumentoResponse : ResponseBase
    {
        [DataMember]
        public DocumentoItem Documento { get; set; }
        [DataMember]
        public List<MetadatoItem> Metadati { get; set; }
        [DataMember]
        public FileItem File { get; set; }
    }
}