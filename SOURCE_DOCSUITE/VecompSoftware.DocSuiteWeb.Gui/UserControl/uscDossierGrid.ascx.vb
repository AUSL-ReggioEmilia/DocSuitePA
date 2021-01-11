Imports System.Collections.Generic
Imports Telerik.Web.UI

Public Class uscDossierGrid
    Inherits DocSuite2008BaseControl

    Public ReadOnly Property PageContentDiv As RadAjaxPanel
        Get
            Return pageContent
        End Get
    End Property
    Public Property IsWindowPopupEnable As Boolean = False

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            dossierGrid.DataSource = New List(Of String)
        End If

    End Sub

End Class