Imports System.Linq
Imports System.ComponentModel

<Serializable()> _
Public Class DSWMessage
    Inherits DomainObject(Of Int32)
    Implements IAuditable
    
    Enum MessageTypeEnum
        Email = 1
    End Enum

    Enum MessageStatusEnum
        <Description("Bozza")> _
        Draft = -100
        <Description("Invio in corso")> _
        Active = 0
        <Description("Inviato")> _
        Sent = 1
        <Description("Errore")> _
        [Error] = -10
    End Enum

#Region " Properties "

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser

    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate

    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser

    ''' <summary> Indica lo Status del Messaggio </summary>
    Public Overridable Property Status As MessageStatusEnum

    ''' <summary> Tipologia del messaggio </summary>
    Public Overridable Property MessageType As MessageTypeEnum

    ''' <summary> E-mail effettiva </summary>
    ''' <remarks> consente di accedere alle proprietà dell'invio effettivo </remarks>
    Public Overridable Property Emails As IList(Of MessageEmail)

    ''' <summary> Lista di tutti i contatti associati ad un messaggio </summary>
    Public Overridable Property MessageContacts As IList(Of MessageContact)

    ''' <summary> Lista di tutti gli allegati associati ad un messaggio e-mail </summary>
    Public Overridable Property Attachments As IList(Of MessageAttachment)

    ''' <summary> Location utilizzata per memorizzare il content dell'e-mail spedita </summary>
    Public Overridable Property Location As Location

    ''' Riferimento circolare a DLL VecompSoftware.DocSuiteWeb.Data.Entity
    ''' <summary> E-mail effettiva </summary>
    ''' <remarks> consente di accedere ai messaggi inviati da Tavolo </remarks>
    '' Public Overridable Property DeskMessages As ICollection(Of DeskMessages)

#End Region

#Region " Constructors "

    Public Sub New()

    End Sub

    Public Sub New(type As MessageTypeEnum)
        MessageType = type
    End Sub

#End Region

#Region " Methods "

    ''' <summary> Ritorna il mittente del messaggio </summary>
    Public Overridable Function GetSender() As MessageContactEmail
        Return (From messageContact In MessageContacts Where messageContact.ContactPosition = Data.MessageContact.ContactPositionEnum.Sender Select messageContact.ContactEmails.First()).FirstOrDefault()
    End Function

    ''' <summary> Ritorna i destinatari del campo "A" </summary>
    Public Overridable Function GetRecipients() As IList(Of MessageContactEmail)
        Return (From messageContact In MessageContacts Where messageContact.ContactPosition = Data.MessageContact.ContactPositionEnum.Recipient Select messageContact.ContactEmails.First()).ToList()
    End Function

    ''' <summary>
    '''  Ritorna i destinatari del campo "Ccn"
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Function GetRecipientBccs() As IList(Of MessageContactEmail)
        Return (From messageContact In MessageContacts Where messageContact.ContactPosition = Data.MessageContact.ContactPositionEnum.RecipientBcc Select messageContact.ContactEmails.First()).ToList()
    End Function

    ''' <summary>
    '''  Ritorna i destinatari del campo "Cc"
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Function GetRecipientCcs() As IList(Of MessageContactEmail)
        Return (From messageContact In MessageContacts Where messageContact.ContactPosition = Data.MessageContact.ContactPositionEnum.RecipientCc Select messageContact.ContactEmails.First()).ToList()
    End Function
#End Region

End Class
