Imports System.Collections.Generic
Imports Telerik.Web.UI

Partial Public Class uscAmmTraspMonitorLogGrid
    Inherits DocSuite2008BaseControl

    Public ReadOnly Property PageContentDiv As RadAjaxPanel
        Get
            Return pageContent
        End Get
    End Property

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ammTraspMonitorLogGrid.DataSource = New List(Of String)
        End If
    End Sub
End Class
