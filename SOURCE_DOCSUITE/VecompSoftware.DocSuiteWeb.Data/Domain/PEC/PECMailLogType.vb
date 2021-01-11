Imports System.ComponentModel

Public Enum PECMailLogType
    <Description("Creazione")> _
    Create
    <Description("Lettura")> _
    Read
    <Description("Modifica")> _
    Modified
    <Description("Spostamento")> _
    Move
    <Description("Risposta")> _
    Replied
    <Description("Inoltro")> _
    Forwarded
    <Description("Collegamento")> _
    Linked
    <Description("Cancellazione")> _
    Delete
    <Description("Ripristino")> _
    Restore
    <Description("Bozza")> _
    Draft
    <Description("Notifica")> _
    MoveNotify
    <Description("Rimozione Collegamento")> _
    Unlinked
    <Description("Avviso")> _
    Warning
    <Description("Errore")> _
    [Error]
    <Description("Altro")> _
    Undefined
    <Description("Riprocessata")> _
    Reprocessed
    <Description("Inviata")> _
    Sended
    <Description("Reinvio")> _
    Resend
    <Description("Correzione")> _
    Fixed
    <Description("RulsetActivated")> _
    RulsetActivated
End Enum