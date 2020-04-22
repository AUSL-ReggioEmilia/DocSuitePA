Imports System.ComponentModel
Public Enum PECMailReceiptType
    <Description("accettazione")>
    Accettazione = 0
    <Description("non-accettazione")>
    NonAccettazione = 1
    <Description("avvenuta-consegna")>
    AvvenutaConsegna = 2
    <Description("preavviso-errore-consegna")>
    PreavvisoErroreConsegna = 3
    <Description("errore-consegna")>
    ErroreConsegna = 4
End Enum