Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Extensions
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class uscTemplateDocumentRepository
    Inherits DocSuite2008BaseControl

#Region "Fields"
#End Region

#Region "Properties"
    Public ReadOnly Property TreeTemplateDocumentRepository As RadTreeView
        Get
            Return rtvTemplateDocument
        End Get
    End Property

    Public Property OnlyPublishedTemplate As Boolean

    Protected ReadOnly Property OnlyPublishedTemplateSerializedValue As String
        Get
            Return JsonConvert.SerializeObject(OnlyPublishedTemplate)
        End Get
    End Property
    Public ReadOnly Property FolderToolBar_Grid As RadToolBar
        Get
            Return FolderToolBar
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub Initialize()

    End Sub
#End Region

End Class