Imports System.ComponentModel

Public Enum ResolutionLogType
    ''' <summary> Inserimento </summary>
    <Description("R - Inserimento")>
    RI
    ''' <summary> Modifica </summary>
    <Description("R - Modifica Atto")>
    RM
    ''' <summary> Annullamento </summary>
    <Description("R - Annullamento Atto")>
    RC
    ''' <summary> Autorizzazione </summary>
    <Description("D - Autorizzazione")>
    RZ
    ''' <summary> Visualizzazione Sommario </summary>
    <Description("R - Visualizzazione Sommario")>
    RS
    ''' <summary> Visualizzazione Documento </summary>
    <Description("R - Visualizzazione Documento")>
    RD
    ''' <summary> Retrocessione </summary>
    <Description("R - Retrocessione")>
    RU
    ''' <summary> Pubblicazione web del documento </summary>
    <Description("R - Pubblicazione web del documento")>
    WP
    ''' <summary> Ritiro pubblicazione web </summary>
    <Description("R - Ritiro pubblicazione web")>
    WR
    ''' <summary> Errore generico </summary>
    <Description("R - Errore generico")>
    RX
    ''' <summary> Creazione gestione stampe </summary>
    <Description("R - Composizione Documenti Atto")>
    RP
    ''' <summary> Inserimento documento </summary>
    <Description("R- Inserimento documento")>
    RDI
    ''' <summary> Visualizzazione Log </summary>
    <Description("R - Visualizzazione LOG")>
    RL
    ''' <summary> Errore </summary>
    <Description("R - Errore")>
    RE
    ''' <summary> Gestione flusso </summary>
    <Description("R - Gestione flusso")>
    RF
    ''' <summary> Flusso generico? </summary>
    <Description("R - Flusso")>
    RW
    ''' <summary> Invio documento verso terzi </summary>
    <Description("R - Invio documento verso terzi")>
    RO
    ''' <summary> Serie Documentale </summary>
    <Description("S - Serie Documentale")>
    SD
    ''' <summary> Creazione attività per JeepService </summary>
    <Description("RA - ResolutionActivity")>
    RA
    ''' <summary> Serializzazione del comando "Attestazione di conformità" </summary>
    <Description("R - Serializzazione comando Attestazione di Conformità")>
    SB
    ''' <summary> Inviato con successo il comando "Attestazione di conformità" </summary>
    <Description("P - Inviato con successo il comando Attestazione di Conformità")>
    SC
    ''' <summary> Terminata con successo attività "Attestazione di conformità" </summary>
    <Description("R - Terminata con successo attività Attestazione di conformità")>
    ST
    ''' <summary> Assegnato livello privacy al documento </summary>
    <Description("R - Assegnato livello privacy al documento")>
    LP
    ''' <summary> Assegnato livello privacy al documento </summary>
    <Description("R - Creazione collaborazione ")>
    CC
    ''' <summary> Annulla collaborazione speciale </summary>
    <Description("AC - Annulla collaborazione speciale")>
    AC
    ''' <summary>  Conferma la visione della Resolution </summary>
    <Description("CV - Conferma visione della Resolution")>
    CV
End Enum
