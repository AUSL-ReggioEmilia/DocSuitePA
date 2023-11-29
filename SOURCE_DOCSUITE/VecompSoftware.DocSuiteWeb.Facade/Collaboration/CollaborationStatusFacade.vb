Imports System
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

<ComponentModel.DataObject()> _
Public Class CollaborationStatusFacade
    Inherits BaseProtocolFacade(Of CollaborationStatus, String, NHibernateCollaborationStatusDao)

    ''' <summary>
    ''' Ricava il mittente da utilizzare in uno specifico status di collaborazione
    ''' </summary>
    ''' <param name="collaborationStatus">Status della collaborazione contenente le specifiche</param>
    ''' <param name="idCollaboration">Collaborazione di riferimento nella quale effettuare le ricerche</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSender(collaborationStatus As CollaborationStatus, idCollaboration As Integer, idTenantAOO As Guid) As MessageContactEmail
        Dim contacts As IList(Of Contact) = Factory.CollaborationStatusRecipientFacade.ResolveAddresses(collaborationStatus.MailSender, idCollaboration, True, idTenantAOO)
        If contacts.Count > 0 Then
            Return Factory.MessageContactEmailFacade.CreateEmailContact(contacts.FirstOrDefault().Description, DocSuiteContext.Current.User.FullUserName, contacts.FirstOrDefault().EmailAddress, MessageContact.ContactPositionEnum.Sender)
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Ricava i destinatari da utilizzare in uno specifico status di collaborazione
    ''' Si attende di trovare in DB un elenco separato da pipe |
    ''' </summary>
    ''' <param name="collaborationStatus">Status della collaborazione contenente le specifiche</param>
    ''' <param name="idCollaboration">Collaborazione di riferimento nella quale effettuare le ricerche</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRecipients(collaborationStatus As CollaborationStatus, idCollaboration As Integer, idTenantAOO As Guid) As IList(Of MessageContactEmail)
        Dim contacts As New List(Of MessageContactEmail)
        ''Carico i destinatari TO
        contacts.AddRange(LoadContactsFromDefinition(collaborationStatus.MailRecipientsTo, idCollaboration, MessageContact.ContactPositionEnum.Recipient, idTenantAOO))

        ''Carico i destinatari CC
        contacts.AddRange(LoadContactsFromDefinition(collaborationStatus.MailRecipientsCc, idCollaboration, MessageContact.ContactPositionEnum.RecipientCc, idTenantAOO))

        ''Carico i destinatari CCN
        contacts.AddRange(LoadContactsFromDefinition(collaborationStatus.MailRecipientsCcn, idCollaboration, MessageContact.ContactPositionEnum.RecipientBcc, idTenantAOO))

        '' Corrisponde ad un distinct su tipologia di contatto e e-mail
        Return contacts.GroupBy(Function(mce) New With {Key mce.MessageContact.ContactPosition, Key mce.Email}).Select(Function(c) c.First()).ToList()
    End Function

    Public Function LoadContactsFromDefinition(mailRecipientDefinition As String, idCollaboration As Integer, messageContactPosition As MessageContact.ContactPositionEnum, idTenantAOO As Guid) As IList(Of MessageContactEmail)
        If String.IsNullOrEmpty(mailRecipientDefinition) Then
            Return New List(Of MessageContactEmail)
        End If

        Dim definitions As String() = mailRecipientDefinition.Split("|"c)
        If definitions.IsNullOrEmpty() Then
            Return New List(Of MessageContactEmail)
        End If

        Dim contacts As New List(Of MessageContactEmail)
        For Each mailRecipient As String In definitions
            For Each mr As String In mailRecipient.Split("_"c)
                Dim statusRecipients As IList(Of Contact) = FacadeFactory.Instance.CollaborationStatusRecipientFacade.ResolveAddresses(mr, idCollaboration, True, idTenantAOO)
                Dim available As IEnumerable(Of Contact) = statusRecipients.Where(Function(r) Not r.Description.Eq(DocSuiteContext.Current.User.UserName) _
                                                                                      AndAlso Not r.EmailAddress.Eq(CommonShared.DsUserMail))

                If available.IsNullOrEmpty() Then
                    Exit For
                End If

                Dim mailContacts As IEnumerable(Of MessageContactEmail) = available.Select(Function(r) FacadeFactory.Instance.MessageContactEmailFacade.CreateEmailContact(r.Description, DocSuiteContext.Current.User.FullUserName, r.EmailAddress, messageContactPosition))
                contacts.AddRange(mailContacts)
            Next
        Next

        Return contacts
    End Function

    Public Sub New()
        MyBase.New()
    End Sub
End Class
