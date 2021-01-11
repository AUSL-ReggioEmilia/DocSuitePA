Imports System.ComponentModel

<Flags()>
Public Enum ProtocolStatusId
    <Description("Incompleto")>
    Incompleto = -10
    <Description("Rigettato")>
    Rejected = -20
    <Description("Temporaneo")>
    Temporaneo = -9
    <Description("Errato")>
    Errato = -5
    <Description("Prenotato")>
    Prenotato = -4
    <Description("Sospeso")>
    Sospeso = -3
    <Description("Annullato")>
    Annullato = -2
    <Description("Errato")>
    ErratoE = -1
    <Description("Attivo")>
    Attivo = 0
    <Description("Contenitore")>
    Container = 1
    <Description("Mittenti")>
    Senders = 2
    <Description("Destinatari")>
    Recipients = 3
    <Description("Recuperato")>
    Recuperato = 4
    <Description("Utilizzato")>
    Utilizzato = 5

    <Description("Fattura PA inviata")>
    PAInvoiceSent = 6
    <Description("Fattura PA notificata")>
    PAInvoiceNotified = 7
    'Fattura PA Notificata da SDI, non ancora da Destinatario
    <Description("Fattura PA accettata")>
    PAInvoiceAccepted = 8
    <Description("Fattura PA rifiutata da SDI")>
    PAInvoiceSdiRefused = 9
    <Description("Fattura rifiutata")>
    PAInvoiceRefused = 10

End Enum