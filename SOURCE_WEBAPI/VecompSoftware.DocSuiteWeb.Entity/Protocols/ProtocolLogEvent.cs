using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.Protocols
{
    public enum ProtocolLogEvent
    {
        /// <summary>
        /// Annullamento
        /// </summary>
        [Description("P - Annullamento")]
        PA,
        /// <summary>
        /// Inserimento
        /// </summary>
        [Description("P - Inserimento")]
        PI,
        /// <summary>
        /// Modifica
        /// </summary>
        [Description("P - Modifica")]
        PM,
        /// <summary>
        /// /// Autorizzazione
        /// /// </summary>
        [Description("P - Autorizzazione")]
        PZ,
        /// <summary>
        /// Accettazione/Rifiuto autorizzazione
        /// </summary>
        [Description("P - Accettazione/Rifiuto Autorizzazione")]
        AR,
        /// <summary>
        /// Visualizzazione Sommario
        /// </summary>
        [Description("P - Visualizzazione Sommario")]
        PS,
        /// <summary>
        /// Visualizzazione Sommario - Prima visualizzazione Protocollo
        /// </summary>

        [Description("P - Visualizzazione Sommario")]
        P1,
        /// <summary>
        /// Visualizzazione Documento
        /// </summary>
        [Description("P - Visualizzazione Documento")]
        PD,
        /// <summary>
        /// Elenco Documenti
        /// </summary>
        [Description("P - Elenco Documenti")]
        PE,
        /// <summary>
        /// Interoperabilità
        /// </summary>
        [Description("P - Interoperabilità")]
        PT,
        /// <summary>
        /// Invio Protocollo verso terzi
        /// </summary>
        [Description("P - Invio Protocollo verso terzi")]
        PO,
        /// <summary>
        /// Rigetto di protocollo
        /// </summary>
        [Description("P - Rigetto di protocollo")]
        PR,
        /// <summary>
        /// Errore
        /// </summary>
        [Description("Errore")]
        PX,
        /// <summary>
        /// Rispondi da PEC
        /// </summary>
        [Description("Rispondi da PEC")]
        PP,
        /// <summary>
        /// Correzione
        /// </summary>
        [Description("P - Correzione")]
        PC,
        /// <summary>
        /// Protocollo collegato ad atto
        /// </summary>
        [Description("P - Protocollo collegato ad atto")]
        PL,
        /// <summary>
        /// /Protocollo in evidenza
        /// </summary>
        [Description("P - Protocollo in evidenza")]
        PH,
        /// <summary>
        /// Protocollo completato e attivato
        /// </summary>
        [Description("P - Protocollo attivato")]
        PF,
        /// <summary>
        /// Protocollo inviato ai Settori
        /// </summary>
        [Description("P - Protocollo inviato ai Settori")]
        PW,
        /// <summary>
        /// Serializzazione del comando "Attestazione di conformità"
        /// </summary>
        [Description("P - Serializzazione comando Attestazione di Conformità")]
        SB,
        /// <summary>
        /// Inviato con successo il comando "Attestazione di conformità"
        /// </summary>
        [Description("P - Inviato con successo il comando Attestazione di Conformità")]
        SC,
        /// <summary>
        /// Terminata con successo attività "Attestazione di conformità"
        /// </summary>
        [Description("P - Terminata con successo attività Attestazione di conformità")]
        ST,
        /// <summary>
        /// Inviato con successo il comando di securizzazione documenti
        /// </summary>
        [Description("P - Inviato con successo il comando di securizzazione documenti")]
        LC,
        /// <summary>
        /// Terminata con successo attività di securizzazione documenti
        /// </summary>
        [Description("P - Terminata con successo attività di securizzazione documenti")]
        LT,
        /// <summary>
        /// Assegnato livello privacy al documento
        /// </summary>
        [Description("P - Assegnato livello privacy al documento")]
        LP,
        /// <summary>
        /// Serie Documentale
        /// </summary>
        [Description("S - Serie Documentale")]
        SD,
        /// <summary>
        /// Inviato con successo il comando di securizzazione documenti
        /// </summary>
        [Description("P - Inviato con successo il comando di applicazione del contrassegno a stampa dei documenti")]
        CS,
        /// <summary>
        /// Terminata con successo attività di securizzazione documenti
        /// </summary>
        [Description("P - Terminata con successo attività di applicazione del contrassegno a stampa dei documenti")]
        CT,
        /// <summary>
        /// Protocolli inviati/condivisi con integrazioni esterne
        /// </summary>
        [Description("P - Protocollo inviato e condiviso con integrazioni esterne")]
        SH,
    }
}
