Imports System.Net.Mail
Imports VecompSoftware.DocSuiteWeb.Data

<Serializable()>
Public Class MessageEmail
    Inherits AuditableDomainObject(Of Int32)

#Region " Properties "
    Private _subject As String = String.Empty


    Public Overridable Property Message As DSWMessage

    Public Overridable Property SentDate As DateTime?

    Public Overridable Property Subject As String
        Get
            Return _subject
        End Get
        Set(value As String)
            _subject = If(String.IsNullOrEmpty(value), value, value.Replace(vbCr, String.Empty).Replace(vbLf, String.Empty).Replace("\r", String.Empty).Replace("\n", String.Empty).Replace(Environment.NewLine, String.Empty))
        End Set
    End Property

    Public Overridable Property Body As String

    Public Overridable Property Priority As MailPriority

    Public Overridable Property EmlDocumentId As Guid?

    Public Overridable Property IsDispositionNotification As Boolean
#End Region

#Region " Constructors "

    Public Sub New()

    End Sub

    Public Sub New(message As DSWMessage, subject As String, body As String, IsDispositionNotification As Boolean)
        Me.Message = message
        Me.Subject = subject
        Me.Body = body
        Me.IsDispositionNotification = IsDispositionNotification
    End Sub

#End Region

#Region " Methods "

    Public Overridable Function GetSender() As MessageContactEmail
        Return Message.GetSender()
    End Function

    Public Overridable Function GetRecipients() As IList(Of MessageContactEmail)
        Return Message.GetRecipients()
    End Function
    Public Overridable Function GetRecipientBccs() As IList(Of MessageContactEmail)
        Return Message.GetRecipientBccs()
    End Function

    Public Overridable Function GetRecipientCcs() As IList(Of MessageContactEmail)
        Return Message.GetRecipientCcs()
    End Function
#End Region

End Class
