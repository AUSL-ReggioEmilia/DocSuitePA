Imports VecompSoftware.DocSuiteWeb.Data

<Serializable()>
Public Class MessageContact
    Inherits AuditableDomainObject(Of Int32)
    Enum ContactTypeEnum
        User = 1
        Role = 2
        AdGroup = 3
    End Enum

    Enum ContactPositionEnum
        Sender = 1
        Recipient = 2
        RecipientCc = 3
        RecipientBcc = 4
    End Enum

#Region " Properties "

    Public Overridable Property Message As DSWMessage

    Public Overridable Property ContactType As ContactTypeEnum

    Public Overridable Property ContactPosition As ContactPositionEnum

    Public Overridable Property Description As String

    Public Overridable Property ContactEmails As IList(Of MessageContactEmail)

#End Region

#Region " Constructors "

    Public Sub New()

    End Sub

    Public Sub New(contactType As ContactTypeEnum, description As String, contactPosition As ContactPositionEnum)
        Me.ContactType = contactType
        Me.Description = description
        Me.ContactPosition = contactPosition
    End Sub

#End Region

End Class
