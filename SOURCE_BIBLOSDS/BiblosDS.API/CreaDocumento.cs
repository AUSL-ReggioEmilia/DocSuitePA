using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Richiesta di creazione documento
    /// </summary>
    /// <example>
    /// var request = new CreaDocumentoRequest 
    /// {
    /// IdClient = "desktop", 
    /// IdRichiesta = "20081128000001",
    /// IdCliente = "TeraSoftware", 
    /// TipoDocumento = "DDT_ATH_TEST2", 
    /// Chiave = "IVA0000001",
    /// File = new FileItem { Nome = "", Blob = File.ReadAllBytes(@"C:\Lavori\Docs\BiblosDS\Scansione61058.pdf.PDF") } ,
    /// Token = token.TokenInfo.Token
    /// };
    /// 
    /// request.Metadati.Add(new MetadatoItem { Name = "TipoDocumento", Value = "IVA" });
    /// request.Metadati.Add(new MetadatoItem { Name = "Tipologia", Value = "IVA" });
    /// request.Metadati.Add(new MetadatoItem { Name = "ProgressivoDocumento", Value = 1 });
    /// request.Metadati.Add(new MetadatoItem { Name = "NomeArchivio", Value = "Test" });
    /// request.Metadati.Add(new MetadatoItem { Name = "idBiblos", Value = 1 });
    /// request.Metadati.Add(new MetadatoItem { Name = "DataInserimentoDocumento", Value = DateTime.Now });
    /// </example>
    [DataContract]
    public class CreaDocumentoRequest : RequestBase
    {        
        /// <summary>
        /// File da caricare
        /// </summary>
        /// <remarks>
        /// Opzionale
        /// </remarks>
        [DataMember]
        public FileItem File { get; set; }
        /// <summary>
        /// Cancella il file che si trova in <see cref="PathFileImmagine">PathFileImmagine</see> se l'operazione è andata a buon fine
        /// </summary>
        [DataMember]
        public bool CancellaFile { get; set; }        
        /// <summary>
        /// Coppia chiave valore dei metadati, un valore per ogni campo da inserire (definito in base al tipo documento)
        /// </summary>
        [DataMember]
        public List<MetadatoItem> Metadati { get; set; }       

        public CreaDocumentoRequest()
        {
            Metadati = new List<MetadatoItem>();
        }
    }

    /// <summary>
    /// Risposta alla richiesta di creazione documento
    /// </summary>
    /// <remarks>
    /// La richiesta ha successo se il codice della risposta <see cref="CodiceEsito">CodiceEsito</see> è <see cref="CodiceErrore">CodiceErrore.NessunErrore</see>; 
    /// </remarks>
    [DataContract]
    public class CreaDocumentoResponse: ResponseBase
    {
        /// <summary>
        /// Coppia chiave valore dei metadati, un valore per ogni campo da inserire (definito in base al tipo documento)
        /// </summary>
        [DataMember]
        public List<MetadatoItem> Metadati { get; set; }
        /// <summary>
        /// Informazioni legate al documento.
        /// </summary>
        [DataMember]
        public DocumentoItem Documento { get; set; }
    }

    /// <summary>
    /// File
    /// </summary>
    /// <remarks>
    /// Specificare sempre il Nome e il Blob
    /// </remarks>
    [DataContract]
    public sealed class FileItem
    {
        /// <summary>
        /// Nome del file caricato
        /// </summary>
        /// <remarks>
        /// Dato obbligatorio, è richiesto il nome del file comprensivo di estensione oppure il nome file completo.
        /// </remarks>
        [DataMember]
        public string Nome { get; set; }
        [DataMember]
        public string Gruppo { get; set; }
        /// <summary>
        /// ByteArray con il contenuto del file
        /// </summary>
        [DataMember]
        public byte[] Blob { get; set; }
        /// <summary>
        /// Dimensione, in bytes, del file.
        /// </summary>
        /// <remarks>Questo valore coincide con la lunghezza dell'array "Blob", che rappresenta il contenuto del file.</remarks>
        [DataMember]
        [Obsolete("Riservato ad uso futuro.", true)]
        public int Dimensione { get; set; }
    }

    /// <summary>
    /// Metadato
    /// </summary>
    [DataContract]
    public sealed class DocumentoItem
    {
        /// <summary>
        /// Codice univoco identificativo del documento.
        /// </summary>
        [DataMember]
        public Guid IdDocumento { get; set; }

        [DataMember]
        public string Nome { get; set; }
        /// <summary>
        /// Stato del trattamento cui è soggetto il documento.
        /// </summary>
         [DataMember]
        public Operazione Stato { get; set; }   
        /// <summary>
        /// Elenco dei metadati rappresentanti le chiavi del documento.
        /// </summary>
         [DataMember]
        public List<MetadatoItem> Chiavi { get; set; }
    }
}
