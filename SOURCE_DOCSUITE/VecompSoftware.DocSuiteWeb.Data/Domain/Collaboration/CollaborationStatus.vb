<Serializable()> _
Public Class CollaborationStatus
    Inherits DomainObject(Of String)

#Region " Fields "

#End Region

#Region " Properties "
    Public Overridable Property MailEnable As Boolean
    Public Overridable Property Description As String
    Public Overridable Property MailSender As String
    Public Overridable Property MailRecipientsTo As String
    Public Overridable Property MailRecipientsCc As String
    Public Overridable Property MailRecipientsCcn As String
    Public Overridable Property MailSubject As String
    Public Overridable Property MailSubjectVars As String
    Public Overridable Property MailStatus As String
    #End Region

#Region " Constructors "
    Public Sub New()
    End Sub
    #End Region

End Class
