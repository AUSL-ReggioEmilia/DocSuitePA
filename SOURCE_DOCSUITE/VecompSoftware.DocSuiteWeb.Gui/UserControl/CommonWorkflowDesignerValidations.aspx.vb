Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class CommonWorkflowDesignerValidations
    Inherits CommonBasePage
#Region "Properties"

    Public ReadOnly Property SelectedKeys As String
        Get
            Return HttpContext.Current.Request.QueryString("SelectedKeys")
        End Get
    End Property
    Public ReadOnly Property PageContent As String
        Get
            Return HttpContext.Current.Request.QueryString("PageContentId")
        End Get
    End Property
#End Region
#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            MasterDocSuite.TitleVisible = False
        End If
    End Sub
#End Region
End Class