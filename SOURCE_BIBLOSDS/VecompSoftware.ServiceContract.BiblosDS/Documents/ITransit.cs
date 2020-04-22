using System;
using System.ComponentModel;
using System.ServiceModel;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Response;

namespace VecompSoftware.ServiceContract.BiblosDS.Documents
{
    /// <summary>
    /// Servizio pubblico di gestione documenti in transito
    /// </summary>
    [ServiceContract(Namespace = "http://Vecomp.BiblosDs.Transit")]
    public interface ITransit
    {
        /// <summary>
        /// Lista di <see cref="Document">documenti</see> contenuti in transito per l'archivio indicato o per tutti gli archivi
        /// </summary>
        /// <param name="archiveName">Nome dell'archivio di cui si vogliono i documenti nel transito</param>
        /// <param name="skip">Numero di record da saltare.</param>
        /// <param name="take">Numero massimo di record da ritornare.</param>
        /// <returns>
        /// Response contenente la lista di <see cref="Document">documenti</see> in transito + numero di record presenti in banca dati.
        /// </returns>
        [OperationContract]
        DocumentResponse GetTransitListDocumentsPaged(string archiveName, int skip, int take);

        [OperationContract]
        DocumentResponse GetTransitServerListDocumentsPaged(string archiveName, string serverName, int skip, int take);

        /// <summary>
        ///  Processa i documenti rimasti pendenti nel transito
        /// </summary>
        /// <param name="archiveName">Nome dell'archivio di cui si vogliono i documenti nel transito</param>
        /// <returns>
        /// true se non ci sono più documenti nel transito
        /// </returns>
        [OperationContract]
        bool StoreTransitArchiveDocuments(string archiveName);

        /// <summary>
        /// Processa i documenti rimasti pendenti nel transito 
        /// </summary>
        /// <returns>true se non ci sono più documenti nel transito</returns>
        [OperationContract]
        bool StoreTransitDocuments();        

        ///<summary>
        ///Processa il documento con idDocument passato
        ///</summary>
        ///<param name="idDocument">Id del documento da processare</param>
        ///<returns>
        ///true processo ok, false processo failed
        ///</returns>
        [OperationContract]
        bool StoreTransitDocument(Guid idDocument);

        [OperationContract]
        bool StoreTransitServerDocument(Guid idDocument, string serverName);
    }
}