Imports System.Collections.Generic

Public Class FascicleLink
    Inherits FascBasePage

#Region "Fields"

#End Region

#Region "Properties"

#End Region

#Region "Events"
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            rgvLinkedFascicles.DataSource = New List(Of String)()
        End If
    End Sub
#End Region

End Class