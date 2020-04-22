Imports System.ComponentModel

<Flags()>
Public Enum LogKindEnum As Short
    <Description("Collaborazione")>
    Collaboration = 1
    <Description("Serie Documentali")>
    DocumentSeries = 2
    <Description("Email")>
    Message = 3
    <Description("PEC")>
    PEC = 4
    <Description("Protocollo")>
    Protocol = 5
    <Description("Atti/Delibere")>
    Resolution = 6
    <Description("Pratiche")>
    Document = 7
    <Description("Firma documenti")>
    Sign = 8
End Enum
