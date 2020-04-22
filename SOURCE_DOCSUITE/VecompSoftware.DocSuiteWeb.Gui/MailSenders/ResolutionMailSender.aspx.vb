Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Logging

Public Class ResolutionMailSender
    Inherits ReslBasePage

#Region "Fields"
    Private Const CURRENT_PAGE_NAME As String = "ResolutionMailSender"
#End Region

#Region " Properties "

    ''' <summary> Indica di preselezionare ruoli destinatari. </summary>
    Private ReadOnly Property EnableRolesSelection As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("selectRoles", False)
        End Get
    End Property

    Private ReadOnly Property OverridePreviousPageUrl As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("overridepreviouspageurl", False)
        End Get
    End Property

    Private ReadOnly Property FromViewer As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("FromViewer", False)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RO, "Aperta pagina di spedizione.")
            MasterDocSuite.TitleVisible = False
            Initialize()
        End If
    End Sub

    Private Sub MailSenderControl_MailSent(sender As Object, e As MailSenders.MailSentEventArgs) Handles MailSenderControl.MailSent
        If CurrentResolution IsNot Nothing Then
            Dim message As New ResolutionMessage(CurrentResolution, e.Mail.Message)
            Facade.ResolutionMessageFacade.Save(message)
        End If
        Dim destinatari As IList(Of MessageContact) = Facade.MessageContactFacade.GetByMessage(e.Mail.Message, MessageContact.ContactPositionEnum.Recipient)

        For Each contact As MessageContact In destinatari
            Dim item As MessageContactEmail = Facade.MessageContactEmailFacade.GetByContact(contact)
            Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RO, String.Format("Spedito a {0}({1}).", item.Description, item.Email))
        Next

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, MailSenderControl, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        Dim previous As ISendMail = DirectCast(PreviousPage, ISendMail)
        If previous IsNot Nothing Then
            ' Se arriva da pagina con implementazione
            MailSenderControl.SenderDescriptionValue = previous.SenderDescription
            MailSenderControl.SenderEmailValue = previous.SenderEmail
            MailSenderControl.SubjectValue = previous.Subject
            MailSenderControl.BodyValue = previous.Body
            MailSenderControl.Documents = previous.Documents
            ' Imposto il ritorno saltando 1 livello (se richiesto dalla request)
            If OverridePreviousPageUrl Then
                Dim previousBasePage As CommonBasePage = DirectCast(PreviousPage, CommonBasePage)
                If previousBasePage IsNot Nothing AndAlso Not String.IsNullOrEmpty(previousBasePage.PreviousPageUrl) Then
                    PreviousPageUrl = previousBasePage.PreviousPageUrl
                End If
            End If

            'TODO: rivedere con privacy di tipo 4 quando verrà applicata agli atti
            If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso FromViewer AndAlso previous.Documents.Count = 0 Then
                AjaxManager.Alert(String.Concat("Attenzione: solo i documenti con un livello di ", PRIVACY_LABEL, " adeguato vengono allegati alla mail.\r\n L'utente non risulta avere un livello di ", PRIVACY_LABEL, " coerente con alcun documento."))
            End If

        Else
            ' Se arriva da una pagina di resolution che non implementa ISendMail
            Facade.ResolutionLogFacade.Insert(CurrentResolution, ResolutionLogType.RO, "Aperta pagina di spedizione.")
            MailSenderControl.SenderDescriptionValue = CommonInstance.UserDescription
            MailSenderControl.SenderEmailValue = CommonInstance.UserMail
            MailSenderControl.SubjectValue = MailFacade.GetResolutionSubject(CurrentResolution)
            MailSenderControl.BodyValue = MailFacade.GetResolutionBody(CurrentResolution)
        End If

        If ProtocolEnv.DeleteMultipleMailRecipientPages.Contains(CURRENT_PAGE_NAME) Then MailSenderControl.EnableCheckBoxRecipients = True

        If EnableRolesSelection AndAlso CurrentResolution.ResolutionRoles IsNot Nothing AndAlso CurrentResolution.ResolutionRoles.Count > 0 Then
            If ProtocolEnv.MailRecipientsSelectionEnabled Then
                Dim roleIds As String = String.Join("|", CurrentResolution.ResolutionRoles.Select(Function(r) r.Role.Id))
                Dim url As String = String.Concat("../UserControl/CommonSelMailRecipients.aspx?Type=", Type, "&Roles=", roleIds)
                AjaxManager.ResponseScripts.Add(String.Concat("return OpenWindowMailSettori('", url, "');"))
            Else
                Dim roles As List(Of Role) = CurrentResolution.ResolutionRoles.Select(Function(rr) rr.Role).ToList()
                MailSenderControl.Recipients = RoleFacade.GetValidContacts(roles)
            End If
        End If
        MailSenderControl.EnableAttachment = DocSuiteContext.Current.GetEnableAttachmentByPage("ResolutionMailSender")
        MailSenderControl.DataBind()

    End Sub

    Private Sub MailSenderControlEvent(sender As Object, e As EventArgs) Handles MailSenderControl.CancelByUser, MailSenderControl.ConfirmByUser
        Response.Redirect(PreviousPageUrl)
    End Sub

#End Region

End Class