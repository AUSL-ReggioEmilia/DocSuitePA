Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Model.ExternalModels
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class ProtocolMailSender
    Inherits ProtBasePage

#Region " Fields "

    Private protocol As Protocol = Nothing
    Private Const CURRENT_PAGE_NAME As String = "ProtocolMailSender"
    Private Const IP4D_SUBJECT As String = "IP4D - Protocollo {0} del {1:dd/MM/yyyy}"
    Private Const IP4D_ACTION As String = "IP4D"
#End Region

#Region " Properties "

    ''' <summary> Protocollo dal quale sto mandando la mail </summary>
    ''' <remarks> Nothing se provengo da visualizzazione multipla </remarks>
    Public Shadows ReadOnly Property CurrentProtocol As Protocol
        Get
            If protocol Is Nothing Then
                Try
                    protocol = MyBase.CurrentProtocol
                Catch
                    ' TODO: togliere questo accrocchio fatto per rilasciare e usare lista di protocolli o altro
                End Try
            End If
            Return protocol
        End Get
    End Property

    Private ReadOnly Property LoadRecipients As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("recipients", True)
        End Get
    End Property

    Private ReadOnly Property OverridePreviousPageUrl As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("overridepreviouspageurl", False)
        End Get
    End Property

    Private ReadOnly Property IsIP4DAction As Boolean
        Get
            Return Action.Eq(IP4D_ACTION)
        End Get
    End Property

    Private ReadOnly Property RedirectToProtocolSummary As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("RedirectToProtocolSummary", False)
        End Get
    End Property

    Private ReadOnly Property SendToRoles As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("SendToRoles", False)
        End Get
    End Property

    Private ReadOnly Property FromViewer As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("FromViewer", False)
        End Get
    End Property

    Private ReadOnly Property SendToUsers As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("SendToUsers", False)
        End Get
    End Property
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        'SetResponseNoCache()
        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            MasterDocSuite.TitleVisible = False
            Initialize()
        End If
    End Sub

    Private Sub MailSenderControl_MailSent(sender As Object, e As MailSenders.MailSentEventArgs) Handles MailSenderControl.MailSent
        '' Collego la mail spedita al protocollo
        If CurrentProtocol IsNot Nothing Then
            Dim newProtocolMessage As New ProtocolMessage(CurrentProtocol, e.Mail.Message)
            Facade.ProtocolMessageFacade.Save(newProtocolMessage)
            Dim destinatari As IList(Of MessageContact) = Facade.MessageContactFacade.GetByMessage(e.Mail.Message, MessageContact.ContactPositionEnum.Recipient)
            '' Aggiorno il log del protocol

            For Each contact As MessageContact In destinatari
                Dim item As MessageContactEmail = Facade.MessageContactEmailFacade.GetByContact(contact)
                If contact.ContactType.Equals(MessageContact.ContactTypeEnum.Role) Then

                    Facade.ProtocolLogFacade.Log(CurrentProtocol, ProtocolLogEvent.PW, String.Format("Spedito al settore {0}({1}).", item.Description, item.Email))

                Else
                    Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PO, String.Format("Spedito a {0}({1}).", item.Description, item.Email))
                End If
            Next
        End If

    End Sub
    Private Sub MailSenderControl_IP4DSent(sender As Object, e As IP4DSentEventArgs) Handles MailSenderControl.IP4DSent
        If e.IP4DModel Is Nothing Then
            Exit Sub
        End If

        For Each recipient As ExternalViewerContactModel In e.IP4DModel.Recipients
            Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PO, String.Format("Spedito a IP4D {0}({1}-{2}).", recipient.Name, recipient.Email, recipient.Account))
        Next
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        If SendToUsers Then
            MailSenderControl.CurrentContactRecipientPosition = MessageContact.ContactPositionEnum.RecipientBcc
        End If
        Dim previous As ISendMail = DirectCast(PreviousPage, ISendMail)
        If previous IsNot Nothing Then
            ' Se arriva da pagina con implementazione
            MailSenderControl.SenderDescriptionValue = previous.SenderDescription
            MailSenderControl.SenderEmailValue = previous.SenderEmail
            If ProtocolEnv.MailUploadProtRecipientsEnabled AndAlso CurrentProtocol IsNot Nothing AndAlso CurrentProtocol.GetRecipients().Count > 0 _
                AndAlso Not OverridePreviousPageUrl AndAlso Not LoadRecipients Then
                Dim mailRecipients As IList(Of ContactDTO) = New List(Of ContactDTO)
                For Each contact As ContactDTO In CurrentProtocol.GetRecipients()
                    Dim contactMail As String = RegexHelper.MatchEmail(contact.Contact.EmailAddress)
                    If Not String.IsNullOrEmpty(contactMail) AndAlso RegexHelper.IsValidEmail(contact.Contact.EmailAddress) Then
                        mailRecipients.Add(contact)
                    End If
                Next
                MailSenderControl.Recipients = mailRecipients
            End If
            If Not IsIP4DAction Then
                MailSenderControl.SubjectValue = previous.Subject
                MailSenderControl.BodyValue = previous.Body
                MailSenderControl.Documents = previous.Documents
            End If

            ' Imposto il ritorno saltando 1 livello (se richiesto dalla request)
            If OverridePreviousPageUrl Then
                Dim previousBasePage As CommonBasePage = DirectCast(PreviousPage, CommonBasePage)
                If previousBasePage IsNot Nothing AndAlso Not String.IsNullOrEmpty(previousBasePage.PreviousPageUrl) Then

                    PreviousPageUrl = previousBasePage.PreviousPageUrl
                End If
            End If


            If DocSuiteContext.Current.PrivacyEnabled AndAlso FromViewer AndAlso previous.Documents.Count = 0 Then
                Dim fullMessage As String = String.Concat("Attenzione: solo i documenti con un livello di ", PRIVACY_LABEL, " adeguato vengono allegati alla mail.\r\nL'utente non risulta avere un livello di ", PRIVACY_LABEL, " coerente con alcun documento.")
                If DocSuiteContext.Current.SimplifiedPrivacyEnabled Then
                    fullMessage = String.Concat("Attenzione: solo i documenti al quale l'utente è autorizzato vengono allegati alla mail.\r\nL'utente non risulta essere autorizzato al trattamento privacy per alcun documento.")
                End If
                AjaxManager.Alert(fullMessage)
            End If

        ElseIf CurrentProtocol IsNot Nothing Then
            Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PO, "Aperta pagina di spedizione.")
            ' Se arriva da una pagina che non implementa ISendMail
            MailSenderControl.SenderDescriptionValue = CommonInstance.UserDescription
            MailSenderControl.SenderEmailValue = CommonInstance.UserMail
            If Not IsIP4DAction Then
                MailSenderControl.SubjectValue = MailFacade.GetProtocolSubject(CurrentProtocol)
                MailSenderControl.BodyValue = MailFacade.GetProtocolBody(CurrentProtocol)
                MailSenderControl.Documents = ProtocolFacade.GetAllDocuments(CurrentProtocol)
            End If
        Else
            Throw New DocSuiteException("Errore pagina di invio mail", "Impossibile inizializzare la mail di invio")
        End If

        If IsIP4DAction Then
            MailSenderControl.IsIP4DEnabled = IsIP4DAction
            MailSenderControl.SubjectValue = String.Format(IP4D_SUBJECT, CurrentProtocol.FullNumber, CurrentProtocol.RegistrationDate)
            MailSenderControl.AuthorizationsVisibility = False
            MailSenderControl.UDYear = CurrentProtocol.Year
            MailSenderControl.UDNumber = CurrentProtocol.Number
            MailSenderControl.UDId = CurrentProtocol.UniqueId
            MailSenderControl.UDRegistrationUser = CurrentProtocol.RegistrationUser
            MailSenderControl.UDRegistrationDate = CurrentProtocol.RegistrationDate
        End If

        If ProtocolEnv.DeleteMultipleMailRecipientPages.Contains(CURRENT_PAGE_NAME) Then
            MailSenderControl.EnableCheckBoxRecipients = True
        End If

        Dim recipients As IList(Of ContactDTO) = New List(Of ContactDTO)

        If LoadRecipients AndAlso CurrentProtocol IsNot Nothing AndAlso CurrentProtocol.Roles IsNot Nothing AndAlso CurrentProtocol.Roles.Count > 0 AndAlso Not IsIP4DAction AndAlso Not SendToUsers Then
            If ProtocolEnv.MailRecipientsSelectionEnabled Then
                Dim roleIds As String = String.Join("|", CurrentProtocol.Roles.Select(Function(r) r.Role.Id))
                Dim parameters As String = String.Concat("&Roles=", roleIds)
                If Facade.ProtocolLogFacade.CountMailRolesLogs(CurrentProtocol.UniqueId) > 0 Then
                    'preparo gli id dei settori ai quali è già stata inviata un'e-mail
                    Dim logs As IList(Of ProtocolLog) = Facade.ProtocolLogFacade.GetMailRolesLogs(CurrentProtocol.UniqueId)
                    Dim sendingMailRoles As IList(Of Role) = CurrentProtocol.Roles.Where(Function(r) logs.Any(Function(l) l.LogDescription.Contains(String.Concat("Spedito al settore ", r.Role.Name)))).Select(Function(t) t.Role).ToList()
                    Dim sentRoleIDs As String = String.Join("|", sendingMailRoles.Select(Function(r) r.Id))
                    parameters = String.Concat(parameters, "&Sent=", sentRoleIDs)
                End If
                Dim url As String = String.Concat("../UserControl/CommonSelMailRecipients.aspx?Type=", Type, parameters)
                AjaxManager.ResponseScripts.Add(String.Concat("return OpenWindowMailSettori('", url, "');"))
            Else
                For Each contact As ContactDTO In RoleFacade.GetValidContacts(CurrentProtocol.GetRoles())
                    recipients.Add(contact)
                Next

            End If
        End If

        If DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso CurrentProtocol IsNot Nothing AndAlso CurrentProtocol.Roles IsNot Nothing AndAlso SendToRoles Then
            Dim privacyRoleRecipients As IList(Of Role) = Facade.RoleFacade.GetByIds(CurrentProtocol.Roles.Where(Function(pr) Not pr.Type.IsNullOrEmpty() AndAlso pr.Type.Equals(ProtocolRoleTypes.Privacy)).Select(Function(r) r.Role.Id).ToArray())
            If privacyRoleRecipients.Any() Then
                Dim contact As Contact
                For Each role As Role In privacyRoleRecipients
                    If role.RoleUsers IsNot Nothing Then
                        For Each privacyManager As RoleUser In role.RoleUsers.Where(Function(ru) ru.DSWEnvironment = DSWEnvironment.Protocol AndAlso ru.Type = RoleUserType.MP.ToString())
                            contact = New Contact()
                            contact.IsActive = 1S
                            contact.Description = privacyManager.Description
                            contact.EmailAddress = privacyManager.Email
                            contact.ContactType = New ContactType(ContactType.Person)
                            recipients.Add(New ContactDTO(contact, Data.ContactDTO.ContactType.Manual))
                        Next
                    End If
                Next
            End If
        End If

        If SendToUsers AndAlso DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso CurrentProtocol IsNot Nothing AndAlso CurrentProtocol.Users IsNot Nothing Then
            Dim privacyUserRecipients As IList(Of ProtocolUser) = CurrentProtocol.Users
            If privacyUserRecipients.Any() Then
                Dim contact As Contact
                Dim emailAddress As String
                For Each user As ProtocolUser In privacyUserRecipients
                    emailAddress = FacadeFactory.Instance.UserLogFacade.EmailOfUser(user.Account, True)
                    If Not String.IsNullOrEmpty(emailAddress) Then
                        contact = New Contact()
                        contact.IsActive = 1S
                        contact.Description = emailAddress
                        contact.EmailAddress = emailAddress
                        contact.ContactType = New ContactType(ContactType.Person)
                        recipients.Add(New ContactDTO(contact, Data.ContactDTO.ContactType.Manual))
                    End If
                Next
            End If
            MailSenderControl.SubjectValue = String.Format(ProtocolEnv.ProtocolMailAuthExternalViewerSubject, protocol.FullNumber)
            MailSenderControl.BodyValue = String.Format(ProtocolEnv.ProtocolMailAuthExternalViewerBody, protocol.Container.Name, DocSuiteContext.Current.User.FullUserName, protocol.FullNumber, String.Format(DocSuiteContext.Current.ProtocolEnv.ExternalViewerMyDocuments, CurrentProtocol.UniqueId))
        End If

        MailSenderControl.Recipients = recipients

        If CurrentProtocol IsNot Nothing AndAlso (CurrentProtocolRights.IsRoleAuthorized OrElse CurrentProtocolRights.IsEditable) Then
            MailSenderControl.MailWithPasswordBody = String.Concat("La password per visualizzare i documenti del protocollo ", CurrentProtocol.FullNumber, " è: ")
        End If

        MailSenderControl.EnableAttachment = DocSuiteContext.Current.GetEnableAttachmentByPage("ProtocolMailSender") AndAlso Not IsIP4DAction
        MailSenderControl.DataBind()
    End Sub

    Private Sub MailSenderControlEvent(sender As Object, e As EventArgs) Handles MailSenderControl.CancelByUser, MailSenderControl.ConfirmByUser
        If RedirectToProtocolSummary Then
            Response.Redirect("~/Prot/ProtVisualizza.aspx?" & CommonShared.AppendSecurityCheck(String.Concat("Type=Prot&Year=", CurrentProtocol.Year, "&Number=", CurrentProtocol.Number)))
        Else
            Response.Redirect(PreviousPageUrl)
        End If
    End Sub

#End Region

End Class