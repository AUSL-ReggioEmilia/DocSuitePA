using System;
using System.ServiceModel;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Response;

namespace VecompSoftware.ServiceContract.BiblosDS.Documents
{
    /// <summary>
    /// Servizio pubblico di ricerca documentale
    /// </summary>
    [ServiceContract(Namespace = "http://Vecomp.BiblosDs.ContentSearch")]
    public interface IContentSearch
    {
        ///<summary>
        ///Chiama la IsAlive dei servizi WCF e verifica che siano attivi
        ///</summary>
        ///<returns></returns>
        [OperationContract]
        bool IsAlive();

        
        /// <summary>
        /// Ricerca di documenti da query
        /// </summary>        
        /// <param name="attributeConditions">
        /// <see cref="DocumentCondition">Condizioni</see> di ricerca per le ricerche nei metadati
        /// </param>
        /// <param name="contentConditions">
        /// <see cref="DocumentCondition">Condizioni</see> di recerca per le ricercge nel contenuto dei file (ove lo storage lo supporta)
        /// </param>
        /// <param name="inferConditions">
        /// <see cref="DocumentCondition">Condizioni</see> di ricerca inferenziale
        /// </param>
        /// <param name="skip">Numero di documenti da saltare</param>
        /// <param name="take">Numero di documenti da restituire</param>
        /// <returns>
        /// <see cref="DocumentResponse">Risultati delle ricerca</see> che soddisfano le condizioni
        /// </returns>
        ///<example>
        ///<code>
        ///DocumentCondition condition = new DocumentCondition();
        ///condition.Name = "Anno";
        ///condition.Value = 2010;
        ///condition.Operator = "IsEqualTo";
        ///condition.Condition = "And";
        ///.
        ///.
        ///.
        ///condition.Conditions = new BindingList{ new DocumentCondition{......}, new DocumentCondition{......} };
        ///</code>
        ///</example>
        [OperationContract]
        DocumentResponse SearchQueryPaged(BindingList<DocumentCondition> AttributeConditions, BindingList<DocumentCondition> ContentConditions, BindingList<DocumentCondition> InferConditions, int? skip, int? take);
        /// <summary>
        /// Ricerca di documenti da query
        /// </summary>        
        /// <param name="attributeConditions">
        /// <see cref="DocumentCondition">Condizioni</see> di ricerca per le ricerche nei metadati
        /// </param>
        /// <param name="contentConditions">
        /// <see cref="DocumentCondition">Condizioni</see> di recerca per le ricercge nel contenuto dei file (ove lo storage lo supporta)
        /// </param>
        /// <param name="inferConditions">
        /// <see cref="DocumentCondition">Condizioni</see> di ricerca inferenziale
        /// </param>
        /// <returns>
        /// Lista di <see cref="Document">docuemnti</see> che soddisfano le condizioni
        /// </returns>
        ///<example>
        ///<code>
        ///DocumentCondition condition = new DocumentCondition();
        ///condition.Name = "Anno";
        ///condition.Value = 2010;
        ///condition.Operator = "IsEqualTo";
        ///condition.Condition = "And";
        ///.
        ///.
        ///.
        ///condition.Conditions = new BindingList{ new DocumentCondition{......}, new DocumentCondition{......} };
        ///</code>
        ///</example>
        [OperationContract]
        BindingList<Document> SearchQuery(BindingList<DocumentCondition> attributeConditions, BindingList<DocumentCondition> contentConditions, BindingList<DocumentCondition> inferConditions);

        [OperationContract]
        BindingList<Document> SearchQueryLatestVersion(BindingList<DocumentCondition> attributeConditions, BindingList<DocumentCondition> contentConditions, BindingList<DocumentCondition> inferConditions);

        ///<summary>
        /// Ricerca di documenti da query in un arcivio
        ///</summary>        
        ///<param name="archiveName"><see cref="DocumentArchive">Archive</see> in cui effettuare la ricerca</param>
        ///<param name="attributeConditions"><see cref="DocumentCondition">Condizioni</see> di ricerca sugli attributi</param>
        ///<param name="contentConditions"><see cref="DocumentCondition">Condizioni</see> di ricerca sui file (ove lo storage lo permette)</param>
        ///<param name="inferConditions"><see cref="DocumentCondition">Condizioni</see> inferenziali di ricerca</param>
        ///<returns>
        ///Lista di <see cref="Document">docuemnti</see> che soddisfano le condizioni
        ///</returns>
        ///<example>
        ///<code>
        ///DocumentCondition condition = new DocumentCondition();
        ///condition.Name = "Anno";
        ///condition.Value = 2010;
        ///condition.Operator = "IsEqualTo";
        ///condition.Condition = "And";
        ///.
        ///.
        ///.
        ///condition.Conditions = new BindingList{ new DocumentCondition{......}, new DocumentCondition{......} };
        ///</code>
        ///</example>
        [OperationContract]
        DocumentResponse SearchQueryContext(string archiveName, BindingList<DocumentCondition> attributeConditions, BindingList<DocumentCondition> contentConditions, BindingList<DocumentCondition> inferConditions, int? skip, int? take);

        [OperationContract]
        DocumentResponse SearchQueryContextLatestVersion(string archiveName, BindingList<DocumentCondition> attributeConditions, BindingList<DocumentCondition> contentConditions, BindingList<DocumentCondition> inferConditions, int? skip, int? take);

        /// <summary>
        /// Interroga un arcivio e ritorna tutte le catene configurate paginate
        /// </summary>
        /// <param name="archiveName"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="docunentsInArchiveCount"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<Document> GetAllDocumentChains(string archiveName, int skip, int take, out int docunentsInArchiveCount);

        /// <summary>
        /// Lista di tutti i documenti di un archivio
        /// </summary>
        /// <param name="archiveName"></param>
        /// <param name="visible">Permette di filtrare solo i documenti visibili o non</param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="docunentsInArchiveCount"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<Document> GetAllDocuments(string archiveName, bool visible, int skip, int take, out int docunentsInArchiveCount);

        /// <summary>
        /// Lista di tutti i documenti in un archivio con preview
        /// </summary>
        /// <param name="archiveName"></param>
        /// <param name="visible"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="docunentsInArchiveCount"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<Document> GetAllDocumentsWithThumbnail(string archiveName, bool visible, int skip, int take, out int docunentsInArchiveCount);

        #region [ GetDocumentsByAttributeValue ]

        /// <summary>
        /// Recupera dall'archivio i documenti aventi l'attributo del valore specificato.
        /// </summary>
        /// <param name="archiveName">Nome dell'archivio.</param>
        /// <param name="attributeName">Nome dell'attributo.</param>
        /// <param name="attributeValue">Valore esatto dell'attributo (case insensitive).</param>
        /// <returns></returns>
        [OperationContract]
        BindingList<Document> GetDocumentsByAttributeValue(string archiveName, string attributeName, string attributeValue);
        /// <summary>
        /// Recupera dall'archivio i documenti aventi l'attributo del valore specificato.
        /// </summary>
        /// <param name="archiveName">Nome dell'archivio.</param>
        /// <param name="attributeName">Nome dell'attributo.</param>
        /// <param name="attributeValue">Valore esatto dell'attributo (case insensitive).</param>
        /// <param name="skip">Indice di inizio pagina.</param>
        /// <param name="take">Estensione della pagina.</param>
        /// <returns></returns>
        [OperationContract]
        BindingList<Document> GetPagingDocumentsByAttributeValue(string archiveName, string attributeName, string attributeValue, int skip, int take);
        /// <summary>
        /// Recupera dall'archivio i documenti aventi l'attributo contenente nel valore la stringa specificata.
        /// </summary>
        /// <param name="archiveName">Nome dell'archivio.</param>
        /// <param name="attributeName">Nome dell'attributo.</param>
        /// <param name="attributeValue">Stringa che deve essere contenuta nel valore dell'attributo (case insensitive).</param>
        /// <returns></returns>
        [OperationContract]
        BindingList<Document> GetDocumentsByAttributeValueContains(string archiveName, string attributeName, string attributeValue);
        /// <summary>
        /// Recupera dall'archivio una pagina dei documenti aventi l'attributo contenente nel valore la stringa specificata.
        /// </summary>
        /// <param name="archiveName">Nome dell'archivio.</param>
        /// <param name="attributeName">Nome dell'attributo.</param>
        /// <param name="attributeValue">Stringa che deve essere contenuta nel valore dell'attributo (case insensitive).</param>
        /// <param name="skip">Indice di inizio della pagina.</param>
        /// <param name="take">Estensione della pagina.</param>
        /// <returns></returns>
        [OperationContract]
        BindingList<Document> GetPagingDocumentsByAttributeValueContains(string archiveName, string attributeName, string attributeValue, int skip, int take);

        #endregion
    }
}
