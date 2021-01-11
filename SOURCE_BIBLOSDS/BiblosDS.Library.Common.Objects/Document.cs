using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using BiblosDS.Library.Common.Enums;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "Document", Namespace = "http://BiblosDS/2009/10/Document")]
    [KnownType(typeof(DocumentAttach))]
    public partial class Document : BiblosDSObject
    {
        /// <summary>
        /// Id di correlazione Chain in BiblosDS    
        /// </summary>
        [DataMember]
        public Nullable<int> IdBiblos { get; set; }

        /// <summary>
        /// Id del documento
        /// </summary>
        [DataMember]
        public Guid IdDocument { get; set; }

        /// <summary>
        /// Id della conservazione sostitutiva.
        /// </summary>
        [DataMember]
        public Nullable<Guid> IdPreservation { get; set; }

        /// <summary>
        /// Parent documet 
        /// </summary>
        [DataMember]
        public Document DocumentParent { get; set; }

        /// <summary>
        /// Relation to the previous version
        /// </summary>
        public Document DocumentParentVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int ChainOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public decimal Version { get; set; }

        /// <summary>
        /// Versione salvata su storage (se supportata)
        /// </summary>
        public Nullable<decimal> StorageVersion { get; set; }

        /// <summary>
        /// Use for the logica documents
        /// <example>A document who is stored one time</example>
        /// </summary>
        [DataMember]
        public Document DocumentLink { get; set; }
        /// <summary>
        /// Certificate to store the documet
        /// </summary>
        [DataMember]
        public DocumentCertificate Certificate { get; set; }

        /// <summary>
        /// Firma dei metadati
        /// <see cref="DocumentAttributeValue">DocumentAttributeValue</see>
        /// </summary>
        [DataMember]
        public string SignHeader { get; set; }

        [DataMember]
        public string FullSign { get; set; }

        [DataMember]
        public string DocumentHash { get; set; }

        [DataMember]
        public bool? IsLinked { get; set; }

        [DataMember]
        public bool IsVisible { get; set; }

        [DataMember]
        public bool? IsConservated { get; set; }

        [DataMember]
        public DateTime? DateExpire { get; set; }

        [DataMember]
        public DateTime? DateCreated { get; set; }

        [DataMember]
        public DateTime? DateMain { get; set; }
        /// <summary>
        /// File name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public long? Size { get; set; }

        /// <summary>
        /// List of Attribute of the file
        /// </summary>
        [DataMember]
        public BindingList<DocumentAttributeValue> AttributeValues { get; set; }

        [DataMember]
        public DocumentStorageArea StorageArea { get; set; }

        [DataMember]
        public DocumentArchive Archive { get; set; }

        [DataMember]
        public DocumentStorage Storage { get; set; }

        [DataMember]
        public DocumentContent Content { get; set; }

        public bool? IsConfirmed { get; set; }

        public DocumentNodeType NodeType { get; set; }

        [DataMember]
        public Status Status { get; set; }

        [DataMember]
        public bool IsCheckOut { get; set; }

        /// <summary>
        /// Define if the deocument is Detched
        /// </summary>
        public bool? IsDetached { get; set; }

        [DataMember]
        public string IdUserCheckOut { get; set; }

        [DataMember]
        public string PrimaryKeyValue { get; set; }

        [DataMember]
        public BindingList<DocumentPermission> Permissions { get; set; }

        [DataMember]
        public Preservation Preservation { get; set; }

        [DataMember]
        public DocumentContent ThumbnailContent { get; set; }

        //[DataMember]
        public string IdThumbnail { get; set; }

        //[DataMember]
        public string IdPdf { get; set; }

        /// <summary>
        /// Id lotto di versamento.
        /// </summary>
        [DataMember]
        public Nullable<Guid> IdAwardBatch { get; set; }

        /// <summary>
        /// Indice di progressione della conservazione.
        /// </summary>
        public long? PreservationIndex { get; set; }

        public BindingList<DocumentCache> Cache { get; set; }

        /// <summary>
        ///IdArchiveCertificate
        /// </summary>
        [DataMember]
        public Guid? IdArchiveCertificate { get; set; }
        #region Constructor

        /// <summary>
        /// Default constructor
        /// Inizialize the new Id
        /// </summary>        
        public Document()
        {
            this.IdDocument = Guid.NewGuid();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocument"></param>
        public Document(Guid IdDocument)
        {
            this.IdDocument = IdDocument;
        }

        public Document(Guid IdDocument, int IdBiblos,
            Guid? IdParentBiblos,
            Guid? IdStorageArea, Guid? IdStorage, Guid IdArchive,
            int ChainOrder, decimal Version, Guid? IdDocumentLink, Guid? IdCertificate,
            string SignHeader, string FullSign, string DocumentHash, bool? IsLinked,
            bool IsVisible, short? IsConservated, DateTime? DateExpire, string FileName,
            long? Size,
            DocumentNodeType NodeType,
            short? IsConfirmed,
            Status Status,
            short IsCheckOut)
        {
            this.IdDocument = IdDocument;
            this.IdBiblos = IdBiblos;
            if (IdParentBiblos != null)
                this.DocumentParent = new Document((Guid)IdParentBiblos);
            if (IdStorageArea != null)
                this.StorageArea = new DocumentStorageArea((Guid)IdStorageArea);
            if (IdStorage != null)
                this.Storage = new DocumentStorage((Guid)IdStorage);
            this.Archive = new DocumentArchive();
            this.ChainOrder = ChainOrder;
            this.Version = Version;
            if (IdDocumentLink != null)
                this.DocumentLink = new Document((Guid)IdDocumentLink);
            if (IdCertificate != null)
                this.Certificate = new DocumentCertificate((Guid)IdCertificate);
            this.SignHeader = SignHeader;
            this.FullSign = FullSign;
            this.DocumentHash = DocumentHash;
            this.IsLinked = IsLinked;
            this.IsVisible = IsVisible;
            this.IsConservated = IsConservated.Equals(1);
            this.DateExpire = DateExpire;
            this.Name = FileName;
            this.Size = Size;
            this.NodeType = NodeType;
            this.IsConfirmed = IsConfirmed.Equals(1);
            this.Status = Status;
            this.IsCheckOut = IsCheckOut.Equals(1);
            this.IsRemoved = (IsDetached.HasValue && IsDetached.Value) || (Status != null && Status.IdStatus == (int)DocumentStatus.RemovedFromStorage);
        }

        public Document(Guid IdDocument, int IdBiblos,
            Guid? IdParentBiblos,
            DocumentStorageArea StorageArea, DocumentStorage Storage, DocumentArchive Archive,
            int ChainOrder, decimal Version, Guid? IdDocumentLink, Guid? IdCertificate,
            string SignHeader, string FullSign, string DocumentHash, bool? IsLinked,
            bool IsVisible, short? IsConservated, DateTime? DateExpire, string FileName,
            long? Size,
            DocumentNodeType NodeType,
            short? IsConfirmed,
            Status Status,
            short IsCheckOut)
        {
            this.IdDocument = IdDocument;
            this.IdBiblos = IdBiblos;
            if (IdParentBiblos != null)
                this.DocumentParent = new Document((Guid)IdParentBiblos);
            this.StorageArea = StorageArea;
            this.Archive = Archive;
            this.Storage = Storage;
            this.ChainOrder = ChainOrder;
            this.Version = Version;
            if (IdDocumentLink != null)
                this.DocumentLink = new Document((Guid)IdDocumentLink);
            if (IdCertificate != null)
                this.Certificate = new DocumentCertificate((Guid)IdCertificate);
            this.SignHeader = SignHeader;
            this.FullSign = FullSign;
            this.DocumentHash = DocumentHash;
            this.IsLinked = IsLinked;
            this.IsVisible = IsVisible;
            this.IsConservated = IsConservated.Equals(1);
            this.DateExpire = DateExpire;
            this.Name = FileName;
            this.Size = Size;
            this.NodeType = NodeType;
            this.IsConfirmed = IsConfirmed.Equals(1);
            this.Status = Status;
            this.IsCheckOut = IsCheckOut.Equals(1);
            this.IsRemoved = (IsDetached.HasValue && IsDetached.Value) || (Status != null && Status.IdStatus == (int)DocumentStatus.RemovedFromStorage);
        }
        #endregion

        [DataMember]
        public string PreservationName { get; set; }

        [DataMember]
        public bool IsLatestVersion { get; set; }

        [DataMember]
        public List<DocumentServer> DocumentInServer { get; set; }

        [DataMember]
        public bool IsRemoved { get; set; }
    }

    public class DocumentCache
    {
        public string FileName { get; set; }
        public string Signature { get; set; }
        public string ServerName { get; set; }

        public Guid IdDocument { get; set; }
    }
}
