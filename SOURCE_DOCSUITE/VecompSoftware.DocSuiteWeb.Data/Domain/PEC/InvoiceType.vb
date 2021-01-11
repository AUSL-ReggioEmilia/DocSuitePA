Imports System.ComponentModel

Public Enum InvoiceType
    <Description("Nessuna")>
    None
    <Description("Fattura verso PA")>
    InvoicePA
    <Description("Fattura verso privati")>
    InvoicePR
End Enum