
using System;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Exceptions;

namespace VecompSoftware.ServiceContract.BiblosDS.Documents
{
    /// <summary>
    /// Servizio pubblico di gestione documenti
    /// </summary>  
    [ServiceContract(Namespace = "http://Vecomp.BiblosDs.Documents")]
    public partial interface IDocuments : IBiblosDSServiceContract
    {
        /// <summary>
        /// Blob del document
        /// In formato binario (by default)
        /// </summary>
        /// <param name="idDocument">Identificativo univoco del documento</param>
        /// <param name="version">se presente e > 0, indica la versione richiesta; se non trovata, se lastVersion = true ritorna l'ultima versione, eccezione NotFound altrimenti</param>
        /// <param name="outputFormat"><see cref="DocumentContentFormat"/> per formati di output supportato (bynary by default)</param>    
        /// <param name="lastVersion">se true indica che in caso di assenza della versione ricercata si vuole l'ultima versione</param>
        /// <returns>document content (Blob in formato binario di default)</returns>    
        [OperationContract, FaultContract(typeof(BiblosDsException))]             
        DocumentContent GetDocumentContent(Guid idDocument, decimal? version, DocumentContentFormat outputFormat, bool? lastVersion);

        /// <summary>
        /// Verifica il docuemnto in quali server è presente
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns>
        /// <see cref="Document">Document</see> con il dettaglio dei server <see cref="Document.DocumentInServer">DocumentInServer</see>
        /// </returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        Document GetDocumentInServer(Guid idDocument);

       
        /// <summary>
        /// Documento pdf conforme all'originale
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="outputFormat"></param>
        /// <param name="xmlLabel"></param>
        /// <returns>
        /// document content (Blob in formato binario di default)
        /// </returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]      
        DocumentContent GetDocumentConformContent(Guid idDocument, DocumentContentFormat outputFormat, string xmlLabel);

        /// <summary>
        /// Informazioni e metadati associati al documento 
        /// </summary>
        /// <param name="idDocument">guid del documento o della catena documentale</param>
        /// <param name="version">versione > 0 richiesta</param>    
        /// <param name="lastVersion">se true indica che nel caso la versione non esista si vuole l'ultima versione, se false in assenza della versione eccezione NotFound</param>
        /// <returns>Lista di <see cref="Document">documenti</see> legati alla catena</returns>
        /// <example>
        /// <BiblosDS Server="w2003bip" Archive="FattureTipo"> 
        /// <Chain Id="33"> 
        /// <Document Id="Guid" Order="" CreateDate="2008/10/03" IsConservated="True/Fale" Size="56465" IsVisible="1" FileName="pdf" Key="xxxxx">
        /// <Attribute Name="AnnoIVA" AutoInc="True/False" Required="True/False" IsEnumerator="True/False">2000</Attribute> 			
        /// <Attribute Name="Signature" AutoInc="True/False" Required="True/False" IsEnumerator="True/False">NS Protocollo n.633586454091550814c:\Temp\T.pdf del 04/07/2003</Attribute> 
        /// </Document> 
        /// </Chain>
        /// </BiblosDS>
        /// </example>
        /// <remarks>funziona sia con guid documenti che con guid catene documentali</remarks>    
        [OperationContract, FaultContract(typeof(BiblosDsException))]      
        BindingList<Document> GetDocumentInfo(Guid idDocument, decimal? version, bool? lastVersion);


        /// <summary>
        /// Aggiunta di una catena documentale vuota (intesta la cartella logica)
        /// </summary>
        /// <param name="archiveName">nome dell'archivio</param>
        /// <param name="attributeValues">metadati della catena documetale (cartella logica)</param>        
        /// <returns>
        /// Id-Parent di tutti i <see cref="Document">Document</see> inseriti.
        /// </returns>
        /// <remarks>
        /// Metodo Transazionale (si apre una catena da confermare con il metodo ConfirmDocument)        
        /// </remarks>        
        [OperationContract, FaultContract(typeof(BiblosDsException))]      
        Guid CreateDocumentChain(string archiveName, BindingList<DocumentAttributeValue> attributeValues);

        /// <summary>
        /// Chiude la transazione.
        /// Se un parent non è confermato non può andare in
        /// conservazione.
        /// </summary>
        /// <param name="idDocument">Id del documento da confermare</param>
        /// <returns>true se l'operazione è andata a buon fine.</returns>
        /// <remarks>Rename Chain to Container...</remarks>   
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        bool ConfirmDocument(Guid idDocument);

        /// <summary>
        /// Effettua l'operazione di detach del documento
        /// 
        /// </summary>
        /// <param name="archive">Nome archivio</param>
        /// <param name="chain">IDBiblos </param>
        /// <param name="Enum">Enumerator</param>
        /// <returns>numero di detach effettuati.</returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        [Obsolete("This is a message describing why this method is obsolete. Use: DocumentDetach", false)]
        int DetachDocument(string archive, int chain, int Enum);

        /// <summary>
        /// Aggiunta di un documento ad una catena documentale
        /// </summary>
        /// <param name="document"><see cref="Document">Documento</see></param>
        /// <param name="idParent">IdChain del parent della catena documentale</param>
        /// <param name="inputFormat"><see cref="DocumentContentFormat">Formato</see> di input supportato</param>
        /// <remarks>
        /// Metodo Transazionale
        /// (nell'inserimento vengono salvati nella tabella di Transito tutti i document passati)   
        /// Usare IdDocument per chiudere la transazione.
        /// </remarks>
        /// <example>
        /// <code>
        /// Document doc = new Document();
        /// doc.Name = prova.pdf;
        /// doc.Attributes    = new AttributesValues[2];
        /// doc.Attributes[0] = new AttributeValue { Attribute = new Attribute { Name = "Numero" }, Value = "10" }
        /// doc.Attributes[1] = new AttributeValue { Attribute = new Attribute { Name = "Data" }, Value = DateTime.Now }
        /// doc.Content = new Content { Blob = myData }
        /// </code>
        /// </example>
        /// <returns>
        /// <see cref="Document">Document</see>
        /// con le informazioni del documento inserito
        /// </returns>    
        [OperationContract, FaultContract(typeof(BiblosDsException))]  
        Document AddDocumentToChain(Document document, Guid? idParent, DocumentContentFormat inputFormat);

        /// <summary>
        /// Inserisce un documento, clonandone il blob da uno già presente.
        /// </summary>
        /// <param name="document">Documento da inserire.</param>
        /// <param name="idParent">Padre catena di destinazione, se nullo crea una nuova catena.</param>
        /// <param name="inputFormat">Formato di input.</param>
        /// <param name="cloneable">Documento di cui clonare il blob.</param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        Document CloneDocumentToChain(Document document, Guid? idParent, DocumentContentFormat inputFormat, Document cloneable);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        Document AddDocumentToChainPDFCrypted(Document document, Guid? idParent, DocumentContentFormat inputFormat);

        /// <summary>
        /// Add document to master server
        /// </summary>
        /// <param name="document"></param>
        /// <returns>
        /// Document added
        /// </returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        DocumentServer AddDocumentToMaster(Document document);

        /// <summary>
        /// Aggiunta di un documento ad una catena documentale eseguendo un workflow.
        /// </summary>
        /// <param name="document">
        /// <see cref="Document">Documento</see> specificando <see cref="DocumentArchive">l'archive</see> (almeno Archive.Name)
        /// e il file <see cref="DocumentContent">(Content)</see>
        /// in formato binario</param>
        /// <param name="idParent">IdChain del parent della catena documentale</param>
        /// <param name="inputFormat"><see cref="DocumentContentFormat">Formato</see> di input supportato</param>
        /// <param name="uriWorkflow">Workflow da chiamare</param>
        /// <remarks>
        /// Metodo Transazionale
        /// (nell'inserimento vengono salvati nella tabella di Transito
        /// tutti i document passati)
        /// </remarks>
        /// <example>
        /// <code>
        /// Document doc = new Document();
        /// doc.Name = prova.pdf;
        /// doc.Attributes    = new AttributesValues[2];
        /// doc.Attributes[0] = new AttributeValue { Attribute = new Attribute { Name = "Numero" }, Value = "10" }
        /// doc.Attributes[1] = new AttributeValue { Attribute = new Attribute { Name = "Data" }, Value = DateTime.Now }
        /// doc.Content = new Content { Blob = myData }
        /// </code>
        /// </example>
        /// <returns>
        /// <see cref="Document">Documento</see> inserito
        /// </returns>    
        [OperationContract, FaultContract(typeof(BiblosDsException))]      
        Document AddDocumentChainWithWorkflow(Document document, Guid? idParent, DocumentContentFormat inputFormat, string uriWorkflow);


        /// <summary>
        /// Schema della definizione degli attributi (metadati) per l'archivio
        /// </summary>    
        /// <param name="archiveName"></param>
        /// <returns>
        /// Lista di <see cref="DocumentAttribute">Attributi</see> definiti per l'archivio.
        /// </returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]      
        BindingList<DocumentAttribute> GetAttributesDefinition(string archiveName);

        /// <summary>
        /// Chiama CheckMetadata
        /// Verifica che esista uno Storage e StorageArea configurati per i metadati
        /// </summary>
        /// <param name="AttributeValues"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        bool CheckIsReady(Document Document);

        /// <summary>
        /// Controlla l'integrità del documento, rispetto alla firma interna e alla impronta SH1 salvata in fase di inserimento e firma dei metadati
        /// Il controllo viene comunque fatto in fase di consultazione. 
        /// </summary>
        /// <param name="document">
        /// <see cref="Document">Documento</see> su cui fare la verifica.
        /// </param>    
        /// <param name="forceSign">se True viene forzata la rifirma dei metadati</param>
        /// <returns></returns>    
        [OperationContract, FaultContract(typeof(BiblosDsException))]      
        bool CheckIntegrity(Document document, bool? forceSign);

        /// <summary>
        /// Attributi e le informazioni di tutti i documenti di una catena documntale
        /// </summary>
        /// <param name="idChain">guid del capo catena</param>
        /// <param name="version">versione > 0 richiesta</param>
        /// <param name="lastVersion">se true indica che in assenza della versione richiesta si è interessati all'ultima versione, se false in assenza eccezione NotFound</param>
        /// <returns>
        /// Lista di <see cref="Document">Documenti</see> che appartengono alla catena documentale
        /// </returns>    
        [OperationContract, FaultContract(typeof(BiblosDsException))]      
        BindingList<Document> GetChainInfo(Guid idChain, decimal? version, bool? lastVersion);

        /// <summary>
        /// Attribuiti e informazioni di tutti i documenti di una catena documentale ad una specifica versione
        /// </summary>
        /// <param name="idParent">Identificativo univoco della catena documentale</param>
        /// <param name="isVisible">se true ritorna solo i documenti visibili</param>
        /// <param name="version">versione > 0 desiderata</param>
        /// <param name="lastVersion">se true indica che in assenza della versione richiesta si è interessati all'ultima versione, se false in assenza eccezione NotFound</param> 
        /// <returns>Lista di <see cref="Document">Documenti</see> legati alla catena documentale</returns>
        /// <remarks>
        /// se version minore 0 e se non ha corripondenza ritorna l'ultima versione        
        /// </remarks>    
        [OperationContract, FaultContract(typeof(BiblosDsException))]      
        BindingList<Document> GetChainInfoDetails(Guid idParent, bool? isVisible, decimal? version, bool? lastVersion);

        /// <summary>
        /// Estrazione in modalità di modifica esclusiva di un documento 
        /// </summary>
        /// <param name="idDocument">guid del documento</param>
        /// <param name="userId">username dell'utente che ha in checkout il documento</param>
        /// <param name="outputFormat"><see cref="DocumentContentFormat">formati</see> di output supportati</param>
        /// <returns>contenuto del documento nel formato richiesto (bynry by default)</returns>
        /// <remarks>il checkout è sempre sull'ultima versione esistente, in quanto non sono gestite alberature di versioni</remarks>  
        [OperationContract, FaultContract(typeof(BiblosDsException))]      
        DocumentContent CheckOutDocumentContent(Guid idDocument, string userId, DocumentContentFormat outputFormat);

        /// <summary>
        /// Estrazione in modalità di modifica esclusiva di un documento 
        /// </summary>
        /// <param name="idDocument">guid del documento</param>
        /// <param name="userId">username dell'utente che ha in checkout il documento</param>
        /// <param name="outputFormat"><see cref="DocumentContentFormat">formati</see> di output supportati</param>
        /// <param name="returnContent">True per ritornare il <see cref="DocumentContent">contenuto</see> del documento, False altrimenti</param>
        /// <returns>
        /// <see cref="Document">Documento</see> in uso esclusivo fino al CheckIn.
        /// </returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]      
        Document CheckOutDocument(Guid idDocument, string userId, DocumentContentFormat outputFormat, bool? returnContent);

        /// <summary>
        /// annulla lo stato di modifica esclusiva di un documento
        /// </summary>
        /// <param name="idDocument">Id del documenti estratto.</param>
        /// <param name="userId">UserId dell'utente che ha effettuato l'estrazione.</param>
        /// <remarks>il documento e i metadati non subiscono modifiche le versioni succeve inserite sono annullate</remarks>    
        [OperationContract, FaultContract(typeof(BiblosDsException))]      
        void UndoCheckOutDocument(Guid idDocument, string userId);

        /// <summary>
        /// Modifica il documento e/o i metadati e ritorna il <see cref="Document">Document</see> del nuovo documento
        /// </summary>
        /// <param name="document">Documento da inserire con Content (Contenuto del file in formato binario di default) e Nuovi Attributes Value</param>    
        /// <param name="userId">utente che esegue il checkin, deve corrispondere con l'utente che ha effettuato il checkout</param>
        /// <param name="inputFormat"><see cref="DocumentContentFormat">formato</see> su input supportato</param>
        /// <param name="version">opzionale, il nuovo numero di versione attributo, se inferiore all'ultima versione eccezione NotAllowed</param>
        /// <returns>
        /// <see cref="Document">Document</see>: documento inserito
        /// </returns>
        /// <remarks>Documento e/o metadati inseriti hanno in automatico una nuova versione</remarks>  
        [OperationContract, FaultContract(typeof(BiblosDsException))]      
        Document CheckInDocument(Document document, string userId, DocumentContentFormat inputFormat, decimal? version);

        /// <summary>
        /// List of configured Archive
        /// </summary>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        BindingList<DocumentArchive> GetArchives();
      
        /// <summary>
        /// Lista di storage disponibili.
        /// </summary>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]      
        BindingList<DocumentStorage> GetStorages();

        /// <summary>
        /// Lista di storage area configurate per lo storage
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]      
        BindingList<DocumentStorageArea> GetStorageAreas(DocumentStorage storage);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        bool IsAlive();

        /// <summary>
        /// torna la versione di BiblosDSVersion , utile al client per controlli
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string BiblosDSVersion(); 

        /// <summary>
        /// Recupera l'IdDocuument (Guid) del documento a partire dall'idBiblos di compatibilita
        /// </summary>
        /// <param name="archiveName"></param>
        /// <param name="idBiblos"></param>
        /// <returns>
        /// IdDocuument
        /// </returns>
        /// <remarks>
        /// IdDocuument è necessario per la chiamata alla GetDocumentInfo
        /// </remarks>
        /// <example>
        /// var idDocument = svc.GetDocumentId("ArchivioTest", 1);
        /// var document = svc.GetDocumentInfo(idDocument, null, true);
        /// </example>
        [OperationContract, FaultContract(typeof(BiblosDsException))]      
        Guid GetDocumentId(string archiveName, int idBiblos);

        /// <summary>
        /// Cancellazione logica di una catena
        /// </summary>
        /// <param name="idChain"></param>
        /// <param name="visibility"></param>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void SetVisibleChain(Guid idChain, bool visibility);

        /// <summary>
        /// Cancellazione logica di un documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="visibility"></param>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void SetVisibleDocument(Guid idDocument, bool visibility);

        /// <summary>
        /// Verifica se un documento è firmato
        /// </summary>
        /// <param name="idDocumenti"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        bool IsDocumentSigned(Guid idDocument);

        /// <summary>
        /// Recupero della lista di firme associate ad un documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        BindingList<DocumentCertificate> GetDocumentSigned(Guid idDocument);

        /// <summary>
        /// Creazione links di legami documentali
        /// </summary>
        /// <param name="IdDocument">Id del documento da legare (ordine non rilevante)</param>
        /// <param name="IdDocumentLink">Id del documento da legare (ordine non rilevante)</param>
        /// <returns>Esito: positivo (<code>TRUE</code>) se tutto e' andato a buon fine, negativo (<code>FALSE</code>) se i documenti da collegare non esistono in banca dati.</returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        bool AddDocumentLink(Guid IdDocument, Guid IdDocumentLink);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        bool DeleteDocumentLink(Guid IdDocument, Guid IdDocumentLink);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        BindingList<Document> GetDocumentLinks(Guid IdDocument);

        /// <summary>
        /// Add attach file to document
        /// </summary>
        /// <param name="IdDocument">Id document to attach file</param>
        /// <param name="attach">DocumentAttach to attach</param>
        /// <remarks>
        /// La catena di allegati è legata alla catena documentale
        /// IdParentDocument
        /// </remarks>
        /// <returns>DocumentAttach convertito</returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        DocumentAttach AddDocumentAttach(Guid IdDocument, DocumentAttach attach);

        /// <summary>
        /// Conferma dell'allegato dal client.
        /// </summary>
        /// <param name="IdDocumentAttach"></param>
        /// <returns>Esito operazione.</returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        bool ConfirmDocumentAttach(Guid IdDocumentAttach);

        /// <summary>
        /// Lista di Attach collagati alla catena.
        /// </summary>
        /// <param name="IdDocument"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        BindingList<DocumentAttach> GetDocumentAttachs(Guid IdDocument);

        /// <summary>
        /// Recupera il contenuto del documento dallo storage
        /// </summary>
        /// <param name="IdDocument"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        DocumentContent GetDocumentAttachContent(Guid IdDocumentAttach);

        /// <summary>
        /// Eliminazione logica
        /// </summary>
        /// <param name="IdDocument"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void SetVisibleDocumentAttach(Guid idDocumentAttach, bool visible);

        /// <summary>
        /// Torna le statistiche di un dato archivio.
        /// Informazioni ritornate:
        ///  - Numero di documenti archiviati;
        ///  - Volume dell'archivio in bytes;
        ///  - Numero di conservazioni [in stato CHIUSO] effettuate;
        ///  - Numero di supporti fisici inviati \ spediti.
        /// </summary>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        ArchiveStatistics GetArchiveStatistics(Guid idArchive);

        #region [ Refactoring ]

        /// <summary>
        /// Recupera l'ultima versione del documento.
        /// </summary>
        /// <param name="idDocument">Id del documento da recuperare.</param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        Document GetDocumentLatestVersion(Guid idDocument);

        /// <summary>
        /// Recupera tutti i documenti all'ultima versione presenti nella catena.
        /// </summary>
        /// <param name="idParent">Id della catena di cui recuperare i documenti.</param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        BindingList<Document> GetDocumentChildren(Guid idParent);

        #endregion

    }
}
