''' <summary> Classe ponte per indicare i contatti della collaborazione, sia utenti che settori. </summary>
<Serializable()> _
Public Class CollaborationContact

#Region " Properties "

    Public Property Account As String
    Public Property IdRole As Short
    Public Property DestinationName As String
    Public Property DestinationEMail As String
    Public Property DestinationFirst As Boolean

#End Region

#Region " Constructors "

    Public Sub New()
    End Sub

    Public Sub New(ByVal account As String, idRole As Short?, ByVal destinationName As String, ByVal destinationMail As String, ByVal destinationFirst As Boolean)
        If Not String.IsNullOrEmpty(account) Then
            Me.Account = account
        End If
        If idRole.HasValue Then
            Me.IdRole = idRole.Value
        End If

        Me.DestinationName = destinationName
        DestinationEMail = destinationMail
        Me.DestinationFirst = destinationFirst
    End Sub

    Public Sub New(contact As Contact, destinationFirst As Boolean)

        If contact.ContactType IsNot Nothing AndAlso contact.ContactType.Id = ContactType.Mistery Then
            Account = contact.RoleUserIdRole
        Else
            Account = contact.Code
        End If

        DestinationName = contact.Description
        DestinationEMail = contact.EmailAddress
        Me.DestinationFirst = destinationFirst

    End Sub

#End Region

End Class