Imports System.ComponentModel

Public Enum InvoiceStatus
    None = 0
    <Description("Fattura PA inviata")>
    PAInvoiceSent = 1
    <Description("Fattura PA notificata")>
    PAInvoiceNotified = 2
    <Description("Fattura PA accettata")>
    PAInvoiceAccepted = 3
    <Description("Fattura PA scartata dallo SDI")>
    PAInvoiceSdiRefused = 4
    <Description("Fattura rifiutata")>
    PAInvoiceRefused = 5
    <Description("Flusso di lavoro avviato")>
    InvoiceWorkflowStarted = 6
    <Description("Flusso di lavoro concluso")>
    InvoiceWorkflowCompleted = 7
    <Description("Flusso di lavoro in errore")>
    InvoiceWorkflowError = 8
    <Description("Fatture da firmare")>
    InvoiceSignRequired = 9
    <Description("In attesa registrazione contabile")>
    InvoiceLookingMetadata = 10
    <Description("Contabilizzata")>
    InvoiceAccounted = 11
End Enum
