Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Extensions
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class uscMassimarioScarto
    Inherits DocSuite2008BaseControl

#Region "Fields"
#End Region

#Region "Properties"
    Public ReadOnly Property TreeMassimarioScarto As RadTreeView
        Get
            Return rtvMassimario
        End Get
    End Property

    Public Property HideCanceledFilter As Boolean

    Protected ReadOnly Property HideCanceledFilterJson As String
        Get
            Return JsonConvert.SerializeObject(HideCanceledFilter)
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
        InitializeAjax()
        If Not IsPostBack Then

        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()

    End Sub
#End Region

End Class