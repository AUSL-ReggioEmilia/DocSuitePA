using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using VecompSoftware.DocSuiteWeb.Services.WSDocm.Dto;

namespace VecompSoftware.DocSuiteWeb.Services.WSDocm
{
    [ServiceContract(Namespace = "http://www.vecompsoftware.it/DocSuiteWS")]
    [XmlSerializerFormat]
    public interface IWSDocm
    {
        /// <summary>
        /// Metodo che restituisce i contenitori su cui l'utente ha diritti di lettura
        /// </summary>
        /// <param name="utente">Nome utente per l'autenticazione sul server ([dominio]/[nome_utente])</param>
        /// <param name="errore">Ritorno dell'eventuale errore/eccezione generata dal metodo (obsolete)</param>
        /// <returns>Lista di contenitori</returns>
        [OperationContract(Action = "http://www.vecompsoftware.it/DocSuiteWS/GetContenitori", ReplyAction = "http://www.vecompsoftware.it/DocSuiteWS/GetContenitori")]
        [Description("Metodo che restituisce i contenitori su cui l'utente ha diritti di lettura")]
        List<Contenitore> GetContenitori(string Utente, string StringaErrore);

        /// <summary>
        /// Metodo che restituisce la lista dei Metadati di una Pratica
        /// </summary>
        /// <param name="utente">Nome utente per l'autenticazione sul server ([dominio]/[nome_utente])</param>
        /// <param name="errore">Ritorno dell'eventuale errore/eccezione generata dal metodo  (obsolete)</param>
        /// <returns>Lista di metadati</returns>
        [OperationContract(Action = "http://www.vecompsoftware.it/DocSuiteWS/GetMetadati", ReplyAction = "http://www.vecompsoftware.it/DocSuiteWS/GetMetadati")]
        [Description("Metodo che restituisce la lista dei Metadati di una Pratica")]
        List<Metadato> GetMetadati(string Utente, string StringaErrore);

        /// <summary>
        /// Metodo che restituisce la lista dei Filtri per la ricerca di una Pratica
        /// </summary>
        /// <param name="utente">Nome utente per l'autenticazione sul server ([dominio]/[nome_utente])</param>
        /// <param name="errore">Ritorno dell'eventuale errore/eccezione generata dal metodo  (obsolete)</param>
        /// <returns></returns>
        [OperationContract(Action = "http://www.vecompsoftware.it/DocSuiteWS/GetFiltri", ReplyAction = "http://www.vecompsoftware.it/DocSuiteWS/GetFiltri")]
        [Description("Metodo che restituisce la lista dei Filtri per la ricerca di una Pratica")]
        List<Filtro> GetFiltri(string Utente, string StringaErrore);

        /// <summary>
        /// Metodo che restituisce la lista degli Stati di un Pratica
        /// </summary>
        /// <param name="utente">Nome utente per l'autenticazione sul server ([dominio]/[nome_utente])</param>
        /// <param name="errore">Ritorno dell'eventuale errore/eccezione generata dal metodo  (obsolete)</param>
        /// <returns></returns>
        [OperationContract(Action = "http://www.vecompsoftware.it/DocSuiteWS/GetStato", ReplyAction = "http://www.vecompsoftware.it/DocSuiteWS/GetStato")]
        [Description("Metodo che restituisce la lista degli Stati di un Pratica")]
        List<StatoPratica> GetStato(string Utente, string StringaErrore);

        /// <summary>
        /// Metodo che effettua una ricerca nel database delle Pratiche
        /// </summary>
        /// <param name="utente">Nome utente per l'autenticazione sul server ([dominio]/[nome_utente])</param>
        /// <param name="filtri">Array di oggetti Filtro</param>
        /// <param name="metadati">Array di String che indicano i campi da fillare</param>
        /// <param name="errore">Ritorno dell'eventuale errore/eccezione generata dal metodo  (obsolete)</param>
        /// <returns></returns>
        [OperationContract(Action = "http://www.vecompsoftware.it/DocSuiteWS/GetDati", Name = "GetDati", ReplyAction = "http://www.vecompsoftware.it/DocSuiteWS/GetDati")]
        [Description("Metodo che effettua una ricerca nel database delle Pratiche")]
        List<Pratica> GetDati(string Utente, Filtro[] Filtri, string[] Metadati, string StringaErrore);

        /// <summary>
        /// Metodo che effettua una ricerca nel database delle Pratiche per un dato numero di risultati
        /// </summary>
        /// <param name="utente">Nome utente per l'autenticazione sul server ([dominio]/[nome_utente])</param>
        /// <param name="filtri">Array di oggetti Filtro</param>
        /// <param name="metadati">Array di String che indicano i campi da fillare</param>
        /// <param name="maxResult">Numero massimo di risultati da restituire [minore o uguale a 0 per 'Tutti']</param>
        /// <param name="errore">Ritorno dell'eventuale errore/eccezione generata dal metodo  (obsolete)</param>
        /// <returns></returns>
        [OperationContract(Action = "http://www.vecompsoftware.it/DocSuiteWS/GetDatiMaxResult", Name = "GetDatiMaxResult", ReplyAction = "http://www.vecompsoftware.it/DocSuiteWS/GetDatiMaxResult")]
        [Description("Metodo che effettua una ricerca nel database delle Pratiche per un dato numero di risultati")]
        List<Pratica> GetDati(string Utente, Filtro[] Filtri, string[] Metadati, int MaxResult, string StringaErrore);

        /// <summary>
        /// Metodo che cambia lo stato di una Pratica
        /// </summary>
        /// <param name="utente">Nome utente per l'autenticazione sul server ([dominio]/[nome_utente])</param>
        /// <param name="anno">Anno della Pratica</param>
        /// <param name="numero">Numero della Pratica</param>
        /// <param name="nuovoStato">Nuovo Stato della Pratica (Es: AR, AP, PA, ... )</param>
        /// <param name="statoData">Data per l'operazione prevista (Es: in caso di chiusura Data Chiusura, riapertura Data Riapertura, archiviazione Data Archiviazione)</param>
        /// <param name="causale">Motivo cambio di stato della pratica</param>
        /// <param name="appendiCausale">True, appende la causale a quella esistente, False: sovrascrive la causale</param>
        /// <param name="errore">Ritorno dell'eventuale errore/eccezione generata dal metodo  (obsolete)</param>
        /// <returns></returns>
        [OperationContract(Action = "http://www.vecompsoftware.it/DocSuiteWS/CambiaStato", ReplyAction = "http://www.vecompsoftware.it/DocSuiteWS/CambiaStato")]
        [Description("Metodo che cambia lo stato di una Pratica")]
        bool CambiaStato(string Utente, short Anno, int Numero, string NuovoStato, DateTime StatoData, string Causale,
            bool AppendiCausale, string StringaErrore);
    }
}
