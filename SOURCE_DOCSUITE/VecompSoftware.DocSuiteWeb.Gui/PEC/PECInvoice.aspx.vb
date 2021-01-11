Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class PECInvoice
    Inherits CommonBasePage

#Region " Fields "

#End Region

#Region " Properties "
    Public ReadOnly Property InvoiceDirection As String
        Get
            Return Request.QueryString.GetValueOrDefault("Direction", String.Empty)
        End Get
    End Property

#End Region

#Region "Events"
#End Region

#Region "Methods"
#End Region

End Class
