Imports System.Collections.Generic

Public Class uscReportDesigner
    Inherits DocSuite2008BaseControl

#Region " Fields "

#End Region

#Region " Properties "
    Public ReadOnly Property PageContentId As String
        Get
            Return pnlContent.ClientID
        End Get
    End Property

    Public Property IsEditable As Boolean
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region " Methods "
    Private Sub Initialize()
        rgvCondition.DataSource = New List(Of Object)
        rgvProperties.DataSource = New List(Of Object)
        rgvSort.DataSource = New List(Of Object)
    End Sub
#End Region

End Class