using System.Collections.Generic;
using System.ServiceModel;

namespace VecompSoftware.DocSuiteWeb.Services.WSProt
{
    [ServiceContract]
    public interface IWSProt
    {
        /// <summary>
        /// Inserisce i metadati di un nuovo protocollo
        /// </summary>
        /// <param name="xmlProt">struttura xml del protocollo</param>
        /// <returns>Restituisce l'identificativo del nuovo protocollo creato nel formato [Anno]/[Numero]</returns>
        [OperationContract]
        string Insert(string xmlProt);

        /// <summary>
        /// Aggiunge un documento al protocollo Anno/Numero come stream base 64. Il flag isMain
        /// definisce se il documento corrisponde al documento principale piuttosto che un allegato.
        /// </summary>
        /// <param name="year">identificativo protocollo</param>
        /// <param name="number">identificativo protocollo</param>
        /// <param name="base64DocumentStream">stream del documento in formato base 64</param>
        /// <param name="documentName">nome del documento passato come stream</param>
        /// <param name="isMain">Stabilisce se il documento sia riconosciuto come documento principale piuttosto che come allegato</param>
        [OperationContract]
        void AddDocument(int year, int number, string base64DocumentStream, string documentName, bool isMain);

        /// <summary>
        /// Completa l'inserimento e attiva la visibilità del protocollo dall'interfaccia utente della DSW
        /// </summary>
        /// <param name="year">identificativo protocollo</param>
        /// <param name="number">identificativo protocollo</param>
        [OperationContract]
        void InsertCommit(int year, int number);

        /// <summary>
        /// Restituisce il link per visualizzare il sommario di protocollo
        /// Aggiungere il parametro &ApriDocumento=Si all'url che viene restituito dal metodo 
        /// 
        /// <param name="year">identificativo protocollo</param>
        /// <param name="number">identificativo protocollo</param>
        /// <returns></returns>
        [OperationContract]
        string GetProtocolLink(int year, int number);

        /// <summary>
        /// Restituisce il link per visualizzare i documenti legati al protocollo indicato tramite anno e numero
        /// </summary>
        /// <param name="year">identificativo protocollo</param>
        /// <param name="number">identificativo protocollo</param>
        /// <returns>link da utilizzare per la visualizzazione del documento</returns>
        [OperationContract]
        string GetDocumentsViewerLink(int year, int number);

        /// <summary>
        /// Restituisce l'elenco completo di tutte le PEC (in ingresso e in uscita) inerenti ad un certo protocollo
        /// (definito da anno e numero passati come parametri)
        /// L'output del metodo sarà un xml
        /// </summary>
        /// <param name="year">identificativo protocollo</param>
        /// <param name="number">identificativo protocollo</param>
        /// <returns>xml che identifica l'elenco completo delle PEC</returns>
        [OperationContract]
        string GetPecs(int year, int number);

        /// <summary>
        /// Permette di modificare lo stato di un protocollo esistente
        /// </summary>
        /// <param name="year">Anno del protocollo da modificare</param>
        /// <param name="number">Numero del protocollo da modificare</param>
        /// <param name="statusCode">StatusCode del ProtocolStatus da impostare</param>
        [OperationContract]
        void SetProtocolStatus(short year, int number, string statusCode);

        /// <summary>
        /// Recupera l'elenco dei ProtocolStatus attualmente presenti in DB
        /// </summary>
        [OperationContract]
        string GetProtocolStatuses();


        /// <summary>
        /// EX GetProtocolInfo
        /// 
        /// Restuisce un xml del protocollo con tutte le info
        /// </summary>
        /// <param name="year">identificativo anno di protocollo</param>
        /// <param name="number">identificativo numero di protocollo</param>
        /// <returns>xml con le info del protocollo</returns>
        [OperationContract]
        string GetProtocolInfo(int year, int number);

        /// <summary>
        /// Carica il contatto di rubrica a partire dal codice ricevuto
        /// </summary>
        /// <param name="contactMailOrCode">E-mail o Codice di Ricerca del contatto da ricercare</param>
        /// <returns></returns>
        [OperationContract]
        int GetContact(string contactMailOrCode);
    }
}
