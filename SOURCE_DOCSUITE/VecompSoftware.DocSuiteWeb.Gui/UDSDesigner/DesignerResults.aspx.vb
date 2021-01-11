Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.RadGrid

Public Class DesignerResults
    Inherits CommonBasePage

#Region "Fields"
    Private _radGridFilterHelper As RadGridFilterHelper
#End Region

#Region "Properties"
    Public ReadOnly Property RadGridFilterHelper As RadGridFilterHelper
        Get
            If _radGridFilterHelper Is Nothing Then
                _radGridFilterHelper = New RadGridFilterHelper(DocSuiteContext.RadGridLocalizeConfiguration)
            End If
            Return _radGridFilterHelper
        End Get
    End Property
#End Region

#Region "Events"

#End Region

#Region "Constructor"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        'todo: lasciato per future implementazioni
    End Sub

    Private Sub Initialize()
        InitializeFilterMenu()
        dgvRepositories.CurrentPageIndex = 0
        dgvRepositories.PageSize = 20
    End Sub

    Private Sub InitializeFilterMenu()
        Dim Menu As GridFilterMenu = dgvRepositories.FilterMenu
        For Each item As RadMenuItem In Menu.Items
            item.Text = RadGridFilterHelper.GetLocalizeFilterName(item.Text)
        Next
    End Sub
#End Region

End Class