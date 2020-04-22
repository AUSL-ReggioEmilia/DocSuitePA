Imports System.ComponentModel

Public Enum DocumentSeriesItemLogType

    <Description("Inserimento")>
    Insert
    <Description("Visualizzazione")>
    View
    <Description("Modifica")>
    Edit
    <Description("Pubblicazione")>
    Publish
    <Description("Ritiro pubblicazione")>
    Retire
    <Description("Annullamento")>
    Cancel
    <Description("Mail")>
    Mail
    <Description("Richiesta Dematerializzazione")>
    SB
    <Description("Inviato con successo il comando Attestazione di Conformit�")>
    SC
    <Description("Terminata con successo attivit� Attestazione di conformit�")>
    ST
    <Description("Assegnato livello privacy al documento")>
    LP
    <Description("Visualizzazione Documento")>
    SD
    <Description("Impostato obbligo di trasparenza")>
    SO
    <Description("Rimosso obbligo di trasparenza")>
    RO
End Enum