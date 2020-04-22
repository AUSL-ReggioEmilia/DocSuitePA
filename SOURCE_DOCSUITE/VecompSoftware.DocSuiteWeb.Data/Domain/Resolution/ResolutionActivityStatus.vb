Imports System.ComponentModel

Public Enum ResolutionActivityStatus
    <Description("Da elaborare")>
    ToBeProcessed = 0
    <Description("Completato con successo")>
    Processed = 1
    <Description("In errore")>
    ProcessedWithErrors = 2
End Enum
