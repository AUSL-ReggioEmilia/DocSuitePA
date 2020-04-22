Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos.Models

Public Class ProtocolInitializer
    Public Sub New()
        Me.Roles = New List(Of Role)
        Me.Containers = New List(Of Container)
        Me.Senders = New List(Of ContactDTO)
        Me.Recipients = New List(Of ContactDTO)
        Me.Attachments = New List(Of DocumentInfo)
        Me.Annexed = New List(Of DocumentInfo)
    End Sub

    Public Property Subject As String
    Public Property Notes As String
    Public Property ProtocolType As Integer?
    Public Property Senders As List(Of ContactDTO)
    Public Property Recipients As List(Of ContactDTO)
    Public Property MainDocument As DocumentInfo
    Public Property Attachments() As List(Of DocumentInfo)
    Public Property Annexed() As List(Of DocumentInfo)
    Public Property SenderProtocolNumber As String
    Public Property SenderProtocolDate As DateTime?
    Public Property Containers As IList(Of Container)
    Public Property Category As Category
    Public Property Roles As IList(Of Role)
    Public Property DocumentTypeLabel As String
End Class
