Imports System.Collections.Generic

Public Class uscResolutionKindSeries
    Inherits DocSuite2008BaseControl

#Region " Fields "

#End Region

#Region " Properties "
    Public ReadOnly Property PageContent As Panel
        Get
            Return pnlPageContent
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region " Methods "
    Public Sub Initialize()
        grdDocumentSeries.DataSource = New List(Of String)
    End Sub
#End Region

End Class