using System.ServiceModel;

namespace VecompSoftware.DocSuiteWeb.Services.WSColl
{
    [ServiceContract]
    public interface IWSColl
    {        
        /// <summary>
        /// Inserisce i metadati di una nuova collaborazione ed eventualmente dati aggiuntivi per
        /// la successiva gestione
        /// </summary>
        /// <param name="xmlColl"></param>
        /// <returns>Restituisce il numero della collaborazione come intero</returns>
        [OperationContract]
        int Insert(string xmlColl);

        /// <summary>
        /// Permette di aggiungere un documento ad una collaborazione 
        /// </summary>
        /// <param name="collNumber">Identificativo della collaborazione</param>
        /// <param name="base64DocumentStream">Stream in base64 del documento</param>
        /// <param name="documentName">Nome con cui salvare il documento</param>
        /// <param name="isMain">Definisce il fatto che il documento sia riconosciuto come documento principale piuttosto che come allegato</param>
        [OperationContract]
        void AddDocument(int collNumber, string base64DocumentStream, string documentName, bool isMain);

        /// <summary>
        /// Permette di aggiungere un documento ad una collaborazione 
        /// </summary>
        /// <param name="collNumber">Identificativo della collaborazione</param>
        /// <param name="documentStream">Stream in byte del documento</param>
        /// <param name="documentName">Nome con cui salvare il documento</param>
        /// <param name="isMain">Stabilisce se il documento sia riconosciuto come documento principale piuttosto che come allegato</param>
        [OperationContract(Name = "AddDocumentByte")]
        void AddDocument(int collNumber, byte[] documentStream, string documentName, bool isMain);

        /// <summary>
        /// Attiva la collaborazione indicata (presuppone quindi che tutti i documenti necessari siano stati caricati)
        /// Prima di chiamare questo metodo la collaborazione risulterà non attiva e quindi non visibile ad alcun utente.
        /// </summary>
        /// <param name="collNumber">Identificativo della collaborazione</param>
        [OperationContract]
        void Start(int collNumber);

        /// <summary>
        /// Restituisce lo status della collaborazione numero collNumber.
        /// </summary>
        /// <param name="collNumber">Identificativo della collaborazione</param>
        /// <returns>Restituisce xml dello status</returns>
        [OperationContract]
        string GetStatus(int collNumber);
    }
}
