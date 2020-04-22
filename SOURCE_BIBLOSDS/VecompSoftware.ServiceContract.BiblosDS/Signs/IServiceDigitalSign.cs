using System.ComponentModel;
using System.ServiceModel;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Enums;
using System;

namespace VecompSoftware.ServiceContract.BiblosDS.Signs
{
    [ServiceContract]    
    public interface IServiceDigitalSign
    {
        /// <summary>
        /// torna la collezione dei certificati di firma e il certificato in scadenza più breve
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="Content"></param>
        /// <param name="FirstCertificate"></param>
        /// <returns></returns>
        [OperationContract]        
        BindingList<DocumentCertificate> GetAllExpireDates(string fileName, DocumentContent Content, out DocumentCertificate FirstCertificate);

        [OperationContract]
        DocumentContent CalculateBlobHash(DocumentContent Content, ComputeHashType computeHash);

        [OperationContract]
        DocumentContent GetContent(string FileName, DocumentContent Content);

        [OperationContract]
        DocumentContent TimeStampDocument(string FileName, DocumentContent Content, bool InfoCamereFormat);

        /// <summary>
        /// Inserimento di una marca temporale ad un documento archiviato
        /// </summary>
        /// <param name="idDocument">Guid del documento</param>
        /// <param name="timeStampAccount">Account(Comped) per la richiesta delle marche temporali</param>
        /// <param name="version">se specificata la nuova versione del documento, eccezione NotAllowed se inferiore alla versione corrente</param>
        [OperationContract]
        void TimeStampDigitalDocument(Guid idDocument, string timeStampAccount, decimal? version);
        
        //TODO Valutare se ritornare un TimeStamp con errori in formato Exceptions
        [OperationContract]        
        string GetTimeStampAvailable(string Service, string User, string Password);

        [OperationContract]
        DocumentContent AddRawSignature(string FileName, DocumentContent SignerCert, DocumentContent SignedDigest, DocumentContent Content);

        /// <summary>
        /// Ricongiunzione e creazione di un documento P7M da firma lato client dell'impronta
        /// </summary>
        /// <param name="idDocument">Guid del documento</param>
        /// <param name="docDigest">contenuto base64 del contenuto del documento</param>
        /// <param name="signerCert">certficato di firma semplificato</param>
        /// <param name="version">se specificata la nuova versione del documento, eccezione NotAllowed se inferiore alla versione corrente</param>
        /// <remarks>Il comprotamento del metodo, oltre alla firma lato server corrisponde ad una operazione di checkin checkout del documento</remarks>
        [OperationContract]
        void SignDigitalDocument(Guid idDocument, string docDigest, DocumentCertificate signerCert, decimal? version);

        /// <summary>
        /// metodo per controllo disponibilità del servizio
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool IsAlive();

        /// <summary>
        /// Verifica del numero di marche temporali disponibili per l'account specificato
        /// </summary>
        /// <param name="timestampAccount">Account(Comped) per la richiesta delle marche temporali</param>
        /// <returns></returns>
        [OperationContract]
        int GetDigitalTimestampAvailable(string timestampAccount);

        /// <summary>
        /// Catena con le impronte Hash per la firma lato client dei documenti identificati da IdDocuments
        /// </summary>
        /// <param name="idDocuments">lista degli identificativi dei documenti di cui si vuole l'impronta per la firma</param>
        /// <returns>array delle impronte documentali</returns>
        [OperationContract]
        string[] GetDocumentsHash(Guid[] idDocuments);


        /// <summary>
        /// Verifica se il contenuto è firmato digitalmente 
        /// </summary>
        /// <param name="content">DocumentContent passato dal client</param>
        /// <returns></returns>
        [OperationContract]
        bool IsBlobSigned(DocumentContent content);
    }
}
