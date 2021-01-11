using System;
using System.ServiceModel;
using BiblosDS.Library.Common.Objects;
using System.ComponentModel;
using BiblosDS.Library.Common.Exceptions;
using BiblosDS.Library.Common.Objects.Response;
using System.Collections.Generic;
using VecompSoftware.DataContract.Documents;

namespace VecompSoftware.ServiceContract.BiblosDS.Documents
{    
    public partial interface IDocuments : IBiblosDSServiceContract
    {
        /// <summary>
        /// Blob del document
        /// In formato binary
        /// </summary>
        /// <param name="IdDocument">Id del documento da reperire</param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]        
        DocumentContent GetDocumentContentById(Guid IdDocument);        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdDocument"></param>
        /// <returns></returns>
        /// <example>
        /// <BiblosDS Server="w2003bip" Archive="FattureTipo"> 
	    /// <Chain Id="33"> 
	    /// <Document Id="Guid" Order="" CreateDate="2008/10/03" IsConservated="True/Fale" Size="56465" IsVisible="1" FileName="pdf" Key="xxxxx">
		/// <Attribute Name="AnnoIVA" AutoInc="True/False"? Required="True/False" IsEnumerator="True/False"?>2000</Attribute> 			
		/// <Attribute Name="Signature" AutoInc="True/False" Required="True/False" IsEnumerator="True/False">NS Protocollo n.633586454091550814c:\Temp\T.pdf del 04/07/2003</Attribute> 
	    /// </Document> 
	    /// </Chain>
        /// </BiblosDS>
        /// </example>
        [OperationContract, FaultContract(typeof(BiblosDsException))]        
        Document GetDocumentInfoById(Guid IdDocument);

        /// <summary>
        /// Ritorna un oggetto di tipo <see cref="BiblosDS.Library.Common.Objects.Response.DocumentResponse"/> contenente un elenco di documenti dati i loro ID + conteggio totale di record in db.
        /// Pilotando i parametri "skip" e "take" è possibile limitare il numero di elementi ritornati e/o saltare i primi "take" record.
        /// </summary>
        /// <param name="idDocuments">Elenco di ID di documenti da ritornare.</param>
        /// <param name="skip">Numero di records sa saltare perchè già ritornati. Se 0 vengono presi tutti i record a partire dal primo.</param>
        /// <param name="take">Numero massimo di record da ritornare. Se -1 vengono tornati TUTTI i record.</param>
        /// <returns>Documenti presenti in db trovati per corrispondenza di chiave primaria.</returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        DocumentResponse GetDocumentsInfoByIdPaged(BindingList<Guid> idDocuments, int skip, int take);

        /// <summary>
        /// Aggiunta di un documento
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="IdParent"></param>
        /// <returns>
        /// Parent di tutti i <see cref="Document">Document</see>
        /// inseriti.
        /// </returns>
        /// <remarks>
        /// Metodo Transazionale
        /// (nell'inserimento vengono salvati nella tabella di Transito
        /// tutti i document passati)
        /// </remarks>
        [OperationContract, FaultContract(typeof(BiblosDsException))]        
        Document InsertDocumentChain(Document Document);

        /// <summary>
        /// Chiude la transazione sull'InsertDocument
        /// Se un parent non è confermato non può andare in
        /// conservazione.
        /// </summary>
        /// <param name="IdParent"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]        
        bool ConfirmChain(Guid IdParent);      

        /// <summary>
        /// MOdifica di un documento
        /// </summary>
        /// <param name="Document"></param>
        [OperationContract, FaultContract(typeof(BiblosDsException))]        
        void UpdateDocument(Document Document);

        /// <summary>
        /// Modifica il nome di un documento.
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentName"></param>
        [OperationContract, FaultContract(typeof(BiblosDsException))]        
        void UpdateDocumentName(Guid idDocument, string documentName);
            

        [OperationContract, FaultContract(typeof(BiblosDsException))]        
        bool CheckMetaData(Document Document);

        /// <summary>
        /// Catena di documenti
        /// </summary>
        /// <param name="IdDocument"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]        
        BindingList<Document> GetChainInfoById(Guid IdDocument);                    

        /// <summary>
        /// Struttura di metadati di un determinato archivio.
        /// </summary>
        /// <param name="IdArchive"></param>
        /// <returns></returns>
        /// <example>
        /// CDATA[
        /// <BiblosDS Server="w2003bip" Archive="FattureTipo"> 	    
        /// <Attributes>
		/// <Attribute Name="AnnoIVA" AutoInc="True/False"? Required="True/False" IsEnumerator="True/False"?></Attribute> 			
		/// <Attribute Name="Signature" AutoInc="True/False" Required="True/False" IsEnumerator="True/False"></Attribute> 
        /// </Attributes> 	    
        /// </BiblosDS>]
        /// </example>
        [OperationContract, FaultContract(typeof(BiblosDsException))]                
        BindingList<DocumentAttribute> GetMetadataStructure(Guid IdArchive);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        [Obsolete("This is a message describing why this method is obsolete. Use: CheckOutDocument", false)]
        Document DocumentCheckOut(Guid IdDocument, bool latestVersion, string UserId);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        [Obsolete("This is a message describing why this method is obsolete. Use: UndoCheckOutDocument", false)]
        void DocumentUndoCheckOut(Guid IdDocument, string UserId);

        /// <summary>
        /// CheckIn solo degli attributi.
        /// Modifica solo gli attributi.
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]        
        Guid DocumentAttributeCheckIn(Document Document, string UserId);

        /// <summary>
        /// Modifica il documento e ritorna l'IdDocument del nuovo documento
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="UserId"></param>
        /// <returns>
        /// IdDocument del documento inserito
        /// </returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        [Obsolete("This is a message describing why this method is obsolete. Use: CheckInDocument", false)]
        Guid DocumentCheckIn(Document Document, string UserId);      

        /// <summary>
        /// Muve the document between two archive
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="Archive"></param>
        /// <param name="ForceDelete">
        /// If true delete perform the phisical delete from the old archive
        /// (All the history is destroy)
        /// else create a new document in the new archive
        /// </param>
        [OperationContract, FaultContract(typeof(BiblosDsException))]        
        void DocumentMoveToArchive(Guid IdDocument, DocumentArchive Archive, bool? ForceDelete);

        /// <summary>
        /// Muve the document between two storage of the same archive
        /// </summary>
        /// <param name="IdDocument"></param>
        /// <param name="Archive"></param>
        /// <param name="ForceDelete"></param>
        [OperationContract, FaultContract(typeof(BiblosDsException))]        
        void DocumentMoveToStorage(Guid IdDocument, DocumentStorage Storage, bool? ForceDelete);

        /// <summary>
        /// Retry to put the document into the configured storage. 
        /// </summary>
        /// <param name="IdDocument"></param>
        [OperationContract, FaultContract(typeof(BiblosDsException))]        
        void DocumentMoveFromTransito(Guid IdDocument);

        /// <summary>
        /// Riapplica gli attributi del db allo storage
        /// </summary>
        /// <param name="IdDocument"></param>
        [OperationContract, FaultContract(typeof(BiblosDsException))]        
        void RestoreAttribute(Guid IdDocument);

        [OperationContract, FaultContract(typeof(BiblosDsException))]        
        void SignDocument(Guid IdDocument, DocumentContent SignerCert, DocumentContent SignedDigest, DocumentContent Content);

        /// <summary>
        /// Recupera la lista delle informazioni di firma presenti nel documento
        /// </summary>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        BindingList<DocumentSignInfo> GetDocumentSignInfo(Guid IdDocument);

        /// <summary>
        /// Permette di eseguire il detach di un documento specifico
        /// </summary>
        /// <param name="document">Documento di cui eseguire detach</param>
        /// <returns>Documento aggiornato</returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        Document DocumentDetach(Document document);

        /// <summary>
        /// Rimuove l'indicizzazione fulltext per tutti i documenti della catena specificata.
        /// </summary>
        /// <param name="idChain"></param>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void RemoveFullTextDataForChain(Guid idChain);

        /// <summary>
        /// Rimuove l'indicizzazione fulltext per uno specifico documento.
        /// </summary>
        /// <param name="idDocument"></param>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void RemoveFullTextDataForDocument(Guid idDocument);

        /// <summary>
        /// Genera le informazioni necessarie al popolamento dell'indice fulltext per tutti i documenti di una catena specifica.
        /// </summary>
        /// <param name="idChain"></param>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void AlignFullTextDataForChain(Guid idChain);

        /// <summary>
        /// Genera le informazioni necessarie al popolamento dell'indice fulltext per un documento specifico.
        /// </summary>
        /// <param name="idDocument"></param>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void AlignFullTextDataForDocument(Guid idDocument);

        /// <summary>
        /// Recupera un documento ignorando la verifica sullo status del documento stesso
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        Document GetDocumentInfoIgnoreState(Guid idDocument);

        /// <summary>
        /// Recupera la lista dei documenti associati ad una specifica catena documentale ignorando la verifica sullo status
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        BindingList<Document> GetDocumentChildrenIgnoreState(Guid idParent);

        /// <summary>
        /// Dato un id catena, verifica se esiste almeno un documento attivo associato
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        bool HasActiveDocuments(Guid idParent);
    }
}
