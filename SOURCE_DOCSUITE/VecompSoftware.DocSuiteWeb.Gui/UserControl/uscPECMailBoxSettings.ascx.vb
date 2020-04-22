Imports System.Collections.Generic
Imports System.Web
Imports System.Web.Script.Services
Imports System.Web.Services
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods


Public Class uscPECMailBoxSettings
    Inherits DocSuite2008BaseControl

#Region "Fields"
    
#End Region

#Region " Properties "

    Public ReadOnly Property PageContent As RadWindow
        Get
            Return rwPECMailBoxSettings
        End Get
    End Property

    Public ReadOnly Property PECMailBoxId As HiddenField
        Get
            Return lblPECMailBoxId
        End Get
    End Property

    Public ReadOnly Property InvoiceTypes As String
        Get
            Dim InvoiceTypesDictionary As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Dim itemValues As Array = System.Enum.GetValues(GetType(InvoiceType))
            Dim itemNames As Array = System.Enum.GetNames(GetType(InvoiceType))
            For i As Integer = 0 To itemNames.Length - 1
                Dim item As New ListItem(DirectCast(i, InvoiceType).GetDescription(), itemValues(i))
                InvoiceTypesDictionary.Add(itemNames(i), item.Text)
            Next
            Return JsonConvert.SerializeObject(InvoiceTypesDictionary)
        End Get
    End Property

#End Region

#Region "Events"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub

#End Region

#Region "Methods"

#End Region

End Class