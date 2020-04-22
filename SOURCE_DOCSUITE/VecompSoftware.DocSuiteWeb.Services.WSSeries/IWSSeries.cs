using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace VecompSoftware.DocSuiteWeb.Services.WSSeries
{
    [ServiceContract]
    public interface IWSSeries
    {
        [OperationContract]
        bool IsAlive();

        /// <summary>
        /// Inserisce i metadati di una nuova DocumentSeriesItem, dove il documento principale è obbligatorio.
        /// </summary>
        /// <param name="xmlSeries">struttura xml del DocumentSeriesItem</param>
        /// <returns>Identificativo univoco DocumentSeriesItem</returns>
        [OperationContract]
        string Insert(string xmlSeries);

        /// <summary>
        /// Permette la consultazione della DocumentSeriesItem indicata
        /// </summary>
        /// <param name="id">Identificativo univoco della DocumentSeriesItem</param>
        /// <param name="withDocument">True per ottenere lo stream del Documento, False altrimenti</param>
        /// <param name="withAnnexed">True per ottenere lo stream degli Annessi, False altrimenti</param>
        /// <param name="pdf">True per ottenere lo stream dei documenti in pdf, False altrimenti</param>
        /// <returns></returns>
        [OperationContract(Name = "Consultation")]
        string ConsultationItem(int id, bool withDocument, bool withAnnexed, bool withUnPublished, bool pdf);

        /// <summary>
        /// Permette la consultazione della DocumentSeriesItem indicata
        /// </summary>
        /// <param name="id">Identificativo univoco della DocumentSeriesItem</param>
        /// <param name="withDocument">True per ottenere lo stream del Documento, False altrimenti</param>
        /// <param name="withAnnexed">True per ottenere lo stream degli Annessi, False altrimenti</param>
        /// <param name="pdf">True per ottenere lo stream dei documenti in pdf, False altrimenti</param>
        /// <param name="onlyPublished">True includere solo elementi pubblicati, False altrimenti</param>
        /// <returns></returns>
        [OperationContract(Name = "ConsultationNew")]
        string ConsultationItem(int id, bool withDocument, bool withAnnexed, bool withUnPublished, bool pdf, bool onlyPublished);

        /// <summary>
        /// Permette la consultazione della DocumentSeriesItem indicata
        /// </summary>
        /// <param name="id">Identificativo dell'Item</param>
        /// <param name="withDocument">Indica se si desidera ricevere anche l'elenco dei documenti</param>
        /// <param name="withAnnexed">Indica se si desidera ricevere anche l'elenco dei documenti annessi</param>
        /// <param name="includeStream">Indica se si desidera ricevere anche lo stream dei documenti</param>
        /// <param name="pdf">Indica se l'eventuale stream richiesto deve essere o meno convertito in PDF</param>
        /// <returns></returns>
        [OperationContract(Name = "GetDocumentSeriesItem")]
        string GetSeriesItem(int id, bool withDocument, bool withAnnexed, bool withUnPublished, bool includeStream, bool pdf);

        /// <summary>
        /// Permette la consultazione della DocumentSeriesItem indicata
        /// </summary>
        /// <param name="id">Identificativo dell'Item</param>
        /// <param name="withDocument">Indica se si desidera ricevere anche l'elenco dei documenti</param>
        /// <param name="withAnnexed">Indica se si desidera ricevere anche l'elenco dei documenti annessi</param>
        /// <param name="includeStream">Indica se si desidera ricevere anche lo stream dei documenti</param>
        /// <param name="pdf">Indica se l'eventuale stream richiesto deve essere o meno convertito in PDF</param>
        /// <param name="onlyPublished">True includere solo elementi pubblicati, False altrimenti</param>
        /// <returns></returns>
        [OperationContract(Name = "GetDocumentSeriesItemNew")]
        string GetSeriesItem(int idDocumentSeriesItem, bool includeDocuments, bool includeAnnexedDocuments, bool includeUnPublishedAnnexedDocuments, bool includeStream, bool pdf, bool onlyPublished);

        /// <summary>
        /// Permette di modificare un DocumentSeriesItem, ma non i relativi documenti
        /// </summary>
        /// <param name="xmlSeries">struttura xml del DocumentSeriesItem</param>
        /// <returns></returns>
        [OperationContract]
        void Update(string xmlSeries);

        /// <summary>
        /// Permette di Inserire o aggiungere annessi alla catena associata al DocumentSeriesItem
        /// </summary>
        /// <param name="id">Identificativo univoco della DocumentSeriesItem</param>
        /// <param name="nameDocument">Nome file del annesso da caricare</param>
        /// <param name="base64DocumentStream">steam annesso serializzaro in base64</param>
        /// <returns>Identificativo della catena degli annessi</returns>
        [OperationContract]
        void AddAnnexed(int id, string nameDocument, string base64DocumentStream);

        /// <summary>
        /// Dato in input xml del finder, consente di effettuare la ricerca dei DocumentSeriesItem 
        /// </summary>
        /// <param name="xmlFinder">xml del finder</param>
        /// <param name="pdf">True per richiedere i documenti convertiti in pdf, False per ottenere quelli originali</param>
        /// <returns>Lista DocumentSeriesItem serializzata</returns>
        [OperationContract]
        string Search(string xmlFinder, bool pdf);

        /// <summary>
        /// Dato in input xml del finder, consente di effettuare la ricerca dei DocumentSeriesItem di tipo specifico "Ritirato"
        /// </summary>
        /// <param name="xmlFinder">xml del finder</param>
        /// <param name="pdf">True per richiedere i documenti convertiti in pdf, False per ottenere quelli originali</param>
        /// <returns>Lista DocumentSeriesItem serializzata</returns>
        [OperationContract]
        string SearchRetired(string xmlFinder, bool pdf);

        /// <summary>
        /// Restituisce l'elenco degli archivi disponibili
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string GetPublicationEnabledContainers();

        /// <summary>
        /// Restituisce l'elenco delle Family di DocumentSeries
        /// </summary>
        /// <param name="onlyPublicationEnabled">Indica se si vuole ottenere l'elenco delle sole DocumentSeries abilitate alla pubblicazione</param>
        /// <param name="includeSubsections">Indica se il risultato deve contenere anche l'elenco delle eventuali sotto-sezioni di DocumentSeries</param>
        /// <param name="includeEmptyFamilies"> </param>
        /// <returns></returns>
        [OperationContract]
        string GetFamilies(bool onlyPublicationEnabled, bool includeSubsections, bool includeEmptyFamilies, int? idArchive);

        /// <summary>
        /// Restituire una lista di ArchiveAttribute data la rispettiva DocumentSeries
        /// </summary>
        /// <param name="idDocumentSeries">identificatore della documentSeries</param>
        /// <returns></returns>
        [OperationContract]
        string GetDynamicData(int idDocumentSeries);

        /// <summary>
        /// Dato il rispettivo idDocumentSeriesItem restituisce il documento
        /// </summary>
        /// <param name="idDocumentSeriesItem"></param>
        /// <param name="idDoc"> </param>
        /// <param name="pdf"></param>
        [OperationContract]
        string GetMainDocument(int idDocumentSeriesItem, Guid idDoc, bool pdf);

        /// <summary>
        /// Dato il rispettivo idDocumentSeriesItem restituisce il documento con signature
        /// </summary>
        /// <param name="idDocumentSeriesItem"></param>
        /// <param name="idDoc"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        [OperationContract]
        string GetMainDocumentWithSignature(int idDocumentSeriesItem, Guid idDoc, string signature);

        /// <summary>
        /// Dato il rispettivo idDocumentSeriesItem e il rispettivo Guid ritorna il rispettivo annesso
        /// </summary>
        /// <param name="idDocumentSeriesItem"></param>
        /// <param name="idAnnexed"></param>
        /// <param name="pdf"></param>
        [OperationContract]
        string GetAnnexed(int idDocumentSeriesItem, Guid idAnnexed, bool pdf);

        /// <summary>
        /// Dato il rispettivo idDocumentSeriesItem e il rispettivo Guid ritorna il rispettivo annesso con signature
        /// </summary>
        /// <param name="idDocumentSeriesItem"></param>
        /// <param name="idAnnexed"></param>
        /// <param name="signature"></param>
        [OperationContract]
        string GetAnnexedWithSignature(int idDocumentSeriesItem, Guid idAnnexed, string signature);

        /// <summary>
        /// Permette di ottenere le documentSeriesItem associate ad una Resolution
        /// </summary>
        /// <param name="idResolution">identificativo univoco della resolution</param>
        /// <param name="pdf"> </param>
        /// <returns>lista di DocumentSeriesItem serializzati in xml</returns>
        [OperationContract]
        string GetDocumentSeriesItemsByResolution(int idResolution, bool pdf);

        /// <summary>
        /// Restituisce la Family richiesta
        /// </summary>
        /// <param name="idFamily">Identificativo della Family</param>
        /// <param name="onlyPublicationEnabled">Indica se si desiderano solo Serie in pubblicazione</param>
        /// <param name="includeSubsections">Include le Sezioni</param>
        /// <returns></returns>
        [OperationContract]
        string GetFamily(int idFamily, bool onlyPublicationEnabled, bool includeSubsections, int? idArchive);

        /// <summary>
        /// dato il rispettivo idDocumentSeriesItem il rispettivo Guid ritorna il rispettivo annesso non pubblicato
        /// </summary>
        /// <param name="idDocumentSeriesItem"></param>
        /// <param name="idUnpublished"></param>
        /// <param name="pdf"></param>
        /// <returns></returns>
        [OperationContract]
        string GetUnPublished(int idDocumentSeriesItem, Guid idUnpublished, bool pdf);

        /// <summary>
        /// dato il rispettivo idDocumentSeriesItem il rispettivo Guid ritorna il rispettivo annesso non pubblicato con signature
        /// </summary>
        /// <param name="idDocumentSeriesItem"></param>
        /// <param name="idUnPublished"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        [OperationContract]
        string GetUnPublishedWithSignature(int idDocumentSeriesItem, Guid idUnPublished, string signature);

        /// <summary>
        /// Recupero le ultime seriesitem pubblicate nell'archivio desiderato
        /// </summary>
        /// <param name="idArchive"></param>
        /// <param name="topResults"></param>
        /// <returns></returns>
        [OperationContract]
        string GetLatestDocumentSeriesItemByArchive(int idArchive, int topResults);

        /// <summary>
        /// Dato un l'id di un documento recupero la serie associata
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        [OperationContract]
        string GetDocumentSeriesByDocumentId(Guid idDocument);
        /// <summary>
        /// Dato l'id della ContainerArchive recupero la lista dei nomi dei rispettivi archivi
        /// </summary>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        [OperationContract]
        string GetArchivesNameByContainerArchiveId(int idArchive);

        [OperationContract]
        string GetDocumentSeries(int idSeries, bool includeSubsections, int? idArchive);

        [OperationContract]
        int SearchCount(string xmlFinder);

        [OperationContract]
        int SearchCountRetired(string xmlFinder);

        [OperationContract]
        string GetIndex(int idSeries, string impersonatingUser, string urlFile, string titolo, string @abstract, string entePubblicatore, string licenza, string urlMask, int? year = null, bool checkPublished = true);

        [OperationContract]
        string SearchConstraints(string xmlFinder);

        [OperationContract]
        string SearchConstraintsRetired(string xmlFinder);
    }
}
