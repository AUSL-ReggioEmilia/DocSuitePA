Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging

<DataObject()>
Public Class ProtocolUserFacade
    Inherits BaseProtocolFacade(Of ProtocolUser, Guid, NHibernateProtocolUserDao)

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "
    Public Sub SetHighlightUser(protocol As Protocol, account As String, note As String)
        If protocol IsNot Nothing AndAlso Not String.IsNullOrEmpty(account) Then
            Dim protocolUser As ProtocolUser = protocol.Users.FirstOrDefault(Function(x) x.Account.Eq(account) AndAlso x.Type = ProtocolUserType.Highlight)
            If protocolUser IsNot Nothing AndAlso Not String.IsNullOrEmpty(note) Then
                protocolUser.Note = note
                UpdateOnly(protocolUser)
            Else
                Dim protocolHighlightUser As ProtocolUser = New ProtocolUser() With {
                    .Account = account,
                    .Protocol = protocol,
                    .Type = ProtocolUserType.Highlight,
                    .Note = note
                }
                Save(protocolHighlightUser)
            End If

            FacadeFactory.Instance.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PH, $"Protocollo {protocol.FullNumber} in evidenza a {account} {note}")
        End If
    End Sub

    Public Sub RemoveHighlightUser(protocol As Protocol, account As String)
        If protocol.Users.Any(Function(x) x.Account.Eq(DocSuiteContext.Current.User.FullUserName) AndAlso x.Type = ProtocolUserType.Highlight) Then
            Dim item As ProtocolUser = protocol.Users.Single(Function(x) x.Account.Eq(account) AndAlso x.Type = ProtocolUserType.Highlight)
            protocol.Users.Remove(item)
            Delete(item)
            FacadeFactory.Instance.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PH, $"Rimossa evidenza del protocollo {protocol.FullNumber} a {account}")

            If DocSuiteContext.Current.ProtocolEnv.SendEmailProtocolHighlightRemovedEnabled Then
                Dim emailContacts As IList(Of MessageContactEmail) = CreateEmailContacts(protocol)
                Dim emailSubject As String = MailFacade.GetProtocolSubject(protocol)
                Dim emailBody As String = $"{DocSuiteContext.Current.ProtocolEnv.SendEmailProtocolHighlightEmailContent} {MailFacade.GetProtocolBody(protocol)}"
                Dim messageEmail As MessageEmail = FacadeFactory.Instance.MessageEmailFacade.CreateEmailMessage(emailContacts, emailSubject, emailBody, False)

                Dim emailMsgId As Integer = FacadeFactory.Instance.MessageEmailFacade.SendEmailMessage(messageEmail)
                FileLogger.Info(LoggerName, $"MailSenderControl - SendEmail - Mail inserita in coda di invio [id {emailMsgId} ]")
            End If
        End If
    End Sub

    Public Function GetProtocolUsersByProtocol(protocolId As Guid, accountUser As String) As ProtocolUser
        Return _dao.GetProtocolUserByProtocol(protocolId, accountUser)
    End Function

    Private Function CreateEmailContacts(protocolToDelete As Protocol) As IList(Of MessageContactEmail)
        Dim emailContacts As IList(Of MessageContactEmail) = New List(Of MessageContactEmail)

        Dim senderDomain As String = DocSuiteContext.Current.User.Domain
        Dim senderName As String = DocSuiteContext.Current.User.UserName

        Dim senderUserLog As UserLog = FacadeFactory.Instance.UserLogFacade.GetByUser(senderName, senderDomain)
        Dim senderUserLogEnabled As Boolean = senderUserLog IsNot Nothing AndAlso Not String.IsNullOrEmpty(senderUserLog.UserMail)

        Dim senderContactPosition As MessageContact.ContactPositionEnum = MessageContact.ContactPositionEnum.Sender
        Dim senderDisplayName As String = CommonAD.GetDisplayName(senderName)
        Dim senderDescription As String = $"<strong>{senderDisplayName}</strong>"
        Dim senderEmail As String = FacadeFactory.Instance.UserLogFacade.EmailOfUser(senderName, senderDomain, senderUserLogEnabled)
        Dim senderContact As MessageContactEmail = FacadeFactory.Instance.MessageContactEmailFacade.CreateEmailContact(senderDescription, senderDisplayName, senderEmail, senderContactPosition)
        emailContacts.Add(senderContact)

        Dim recipientFullName As String() = protocolToDelete.RegistrationUser.Split("\"c)
        Dim recipientUserDomain As String = recipientFullName(0)
        Dim recipientUserName As String = recipientFullName(1)

        Dim recipientUserLog As UserLog = FacadeFactory.Instance.UserLogFacade.GetByUser(protocolToDelete.RegistrationUser)
        Dim recipientUserLogEnabled As Boolean = recipientUserLog IsNot Nothing AndAlso Not String.IsNullOrEmpty(recipientUserLog.UserMail)

        Dim recipientEmail As String = FacadeFactory.Instance.UserLogFacade.EmailOfUser(recipientUserName, recipientUserDomain, recipientUserLogEnabled)
        Dim recipientContactPosition As MessageContact.ContactPositionEnum = MessageContact.ContactPositionEnum.Recipient
        Dim recipientDisplayName As String = CommonAD.GetDisplayName(protocolToDelete.RegistrationUser)
        Dim recipientContact As MessageContactEmail = FacadeFactory.Instance.MessageContactEmailFacade.CreateEmailContact(String.Empty, recipientDisplayName, recipientEmail, recipientContactPosition)
        emailContacts.Add(recipientContact)

        Return emailContacts
    End Function
#End Region
End Class