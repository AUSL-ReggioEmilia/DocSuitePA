Imports Microsoft.Reporting.WebForms

Public Interface IPrintRpt

    ReadOnly Property TablePrint() As ReportViewer
    Property TitlePrint() As String
    Property RdlcPrint() As String

    Sub DoPrint()

End Interface