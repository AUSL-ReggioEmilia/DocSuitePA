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
    [ServiceContract(Namespace = "http://Vecomp.BiblosDs._Documents")]
    public interface _IDocuments
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
        [OperationContract]        
        DocumentContent GetDocumentContent(Guid idDocument, decimal? version, DocumentContentFormat outputFormat, bool? lastVersion);

       
        /// <summary>
        /// Documento pdf conforme all'originale
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="outputFormat"></param>
        /// <param name="xmlLabel"></param>
        /// <returns>
        /// document content (Blob in formato binario di default)
        /// </returns>
        [OperationContract]
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
        [OperationContract]
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
        [OperationContract]
        Guid CreateDocumentChain(string archiveName, BindingList<DocumentAttributeValue> attributeValues);

        /// <summary>
        /// Chiude la transazione.
        /// Se un parent non è confermato non può andare in
        /// conservazione.
        /// </summary>
        /// <param name="idDocument">Id del documento da confermare</param>
        /// <returns>true se l'operazione è andata a buon fine.</returns>
        /// <remarks>Rename Chain to Container...</remarks>   
        [OperationContract]
        bool ConfirmDocument(Guid idDocument);

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
        [OperationContract]
        Document AddDocumentToChain(Document document, Guid? idParent, DocumentContentFormat inputFormat);

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
        [OperationContract]
        Document AddDocumentChainWithWorkflow(Document document, Guid? idParent, DocumentContentFormat inputFormat, string uriWorkflow);


        /// <summary>
        /// Schema della definizione degli attributi (metadati) per l'archivio
        /// </summary>    
        /// <param name="archiveName"></param>
        /// <returns>
        /// Lista di <see cref="DocumentAttribute">Attributi</see> definiti per l'archivio.
        /// </returns>
        [OperationContract]
        BindingList<DocumentAttribute> GetAttributesDefinition(string archiveName);

        /// <summary>
        /// Chiama CheckMetadata
        /// Verifica che esista uno Storage e StorageArea configurati per i metadati
        /// </summary>
        /// <param name="document">guid del documento da verificare</param>
        /// <returns>true se la definizione dei metadati e delle rule porta alla determinazione di uno storage e di una storage area</returns>  
        [OperationContract]
        bool CheckIsReady(Document document);

        /// <summary>
        /// Controlla l'integrità del documento, rispetto alla firma interna e alla impronta SH1 salvata in fase di inserimento e firma dei metadati
        /// Il controllo viene comunque fatto in fase di consultazione. 
        /// </summary>
        /// <param name="document">
        /// <see cref="Document">Documento</see> su cui fare la verifica.
        /// </param>    
        /// <param name="forceSign">se True viene forzata la rifirma dei metadati</param>
        /// <returns></returns>    
        [OperationContract]
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
        [OperationContract]
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
        [OperationContract]
        BindingList<Document> GetChainInfoDetails(Guid idParent, bool? isVisible, decimal? version, bool? lastVersion);

        /// <summary>
        /// Estrazione in modalità di modifica esclusiva di un documento 
        /// </summary>
        /// <param name="idDocument">guid del documento</param>
        /// <param name="userId">username dell'utente che ha in checkout il documento</param>
        /// <param name="outputFormat"><see cref="DocumentContentFormat">formati</see> di output supportati</param>
        /// <returns>contenuto del documento nel formato richiesto (bynry by default)</returns>
        /// <remarks>il checkout è sempre sull'ultima versione esistente, in quanto non sono gestite alberature di versioni</remarks>  
        [OperationContract]
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
        [OperationContract]
        Document CheckOutDocument(Guid idDocument, string userId, DocumentContentFormat outputFormat, bool? returnContent);

        /// <summary>
        /// annulla lo stato di modifica esclusiva di un documento
        /// </summary>
        /// <param name="idDocument">Id del documenti estratto.</param>
        /// <param name="userId">UserId dell'utente che ha effettuato l'estrazione.</param>
        /// <remarks>il documento e i metadati non subiscono modifiche le versioni succeve inserite sono annullate</remarks>    
        [OperationContract]
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
        [OperationContract]
        Document CheckInDocument(Document document, string userId, DocumentContentFormat inputFormat, decimal? version);

        /// <summary>
        /// List of available archives
        /// </summary>
        /// <returns>
        /// Lista di <see cref="DocumentArchive">Archives</see>
        /// </returns>
        [OperationContract]
        BindingList<DocumentArchive> GetArchives();

        /// <summary>
        /// Lista di storage disponibili.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        BindingList<DocumentStorage> GetStorages();

        /// <summary>
        /// Lista di storage area configurate per lo storage
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<DocumentStorageArea> GetStorageAreas(DocumentStorage storage);

        /// <summary>
        /// Chiama la IsAlive dei servizi WCF e verifica che siano attivi 
        /// </summary>
        /// <returns>un messaggio riepilogativo delle eccezioni e conferme ottenute</returns>    
        [OperationContract]
        bool IsAlive();

        /// <summary>
        /// Cancellazione logica di una catena
        /// </summary>
        /// <param name="idChain"></param>
        /// <param name="visibility"></param>
        [OperationContract]
        void SetVisibleChain(Guid idChain, bool visibility);

        /// <summary>
        /// Cancellazione logica di un documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="visibility"></param>
        [OperationContract]
        void SetVisibleDocument(Guid idDocument, bool visibility);

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
        [OperationContract]
        Guid GetDocumentId(string archiveName, int idBiblos);


        /// <summary>
        /// Verifica se un documento è firmato
        /// </summary>
        /// <param name="idDocumenti"></param>
        /// <returns></returns>
        [OperationContract]
        bool IsDocumentSigned(Guid idDocument);

        /// <summary>
        /// Recupero della lista di firme associate ad un documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<DocumentCertificate> GetDocumentSigned(Guid idDocument);

        /// <summary>
        /// Creazione links di legami documentali
        /// </summary>
        /// <param name="IdDocument">Id del documento da legare (ordine non rilevante)</param>
        /// <param name="IdDocumentLink">Id del documento da legare (ordine non rilevante)</param>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void AddDocumentLink(Guid IdDocument, Guid IdDocumentLink);

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
        void SetVisibleDocumentAttach(Guid IdDocument, bool visible);
    }
}