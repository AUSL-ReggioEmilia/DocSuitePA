Imports VecompSoftware.Services.Biblos.Models

Public Class ResolutionInitializer

#Region " Fields "

#End Region

#Region " Properties "
    Public Property Subject As String
    Public Property Notes As String
    Public Property ResolutionType As String
    Public Property MainDocument As DocumentInfo
    Public Property MainDocumentOmissis As DocumentInfo
    Public Property Attachments() As List(Of DocumentInfo)
    Public Property AttachmentsOmissis() As List(Of DocumentInfo)
    Public Property Annexed() As List(Of DocumentInfo)
#End Region

#Region " Constructor "
    Public Sub New()
        Me.Attachments = New List(Of DocumentInfo)
        Me.AttachmentsOmissis = New List(Of DocumentInfo)
        Me.Annexed = New List(Of DocumentInfo)
    End Sub
#End Region

#Region " Methods "

#End Region

End Class
