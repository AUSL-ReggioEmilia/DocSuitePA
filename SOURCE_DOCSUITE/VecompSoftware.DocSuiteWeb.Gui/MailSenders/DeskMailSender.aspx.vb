Imports System.Linq
Imports NHibernate.Util
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data

Public Class DeskMailSender
    Inherits DeskBasePage

#Region "Fields"
    Private Const CURRENT_PAGE_NAME As String = "DeskMailSender"
#End Region

#Region "Properties"


    Private ReadOnly Property OverridePreviousPageUrl As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("overridepreviouspageurl", False)
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            Initialize()
        End If
    End Sub

    Private Sub MailSenderControl_MailSent(sender As Object, e As MailSenders.MailSentEventArgs) Handles MailSenderControl.MailSent
        '' Collego la mail spedita al tavolo
        If CurrentDesk IsNot Nothing Then
            Dim deskMessage As DeskMessage = New DeskMessage() With {.Desk = CurrentDesk, .Message = e.Mail}
            CurrentDeskMessageFacade.Save(deskMessage)

            '' Aggiorno il log del tavolo per ogni destinatario            
            Facade.MessageContactFacade.GetByMessage(e.Mail.Message, MessageContact.ContactPositionEnum.Recipient) _
                .Select(Function(s) Facade.MessageContactEmailFacade.GetByContact(s)) _
                .ForEach(Sub(act) CurrentDeskLogFacade.InsertLog(DeskLogType.Modify, _
                                                                      String.Format("Spedito a {0}({1}).", act.Description, act.Email), CurrentDesk, SeverityLog.Info))
        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub Initialize()
        Dim previous As ISendMail = DirectCast(PreviousPage, ISendMail)
        If previous Is Nothing Then
            Throw New DocSuiteException("Errore pagina di invio mail", "Impossibile inizializzare la mail di invio")
        End If
        MasterDocSuite.TitleVisible = False

        MailSenderControl.SenderDescriptionValue = previous.SenderDescription
        MailSenderControl.SenderEmailValue = previous.SenderEmail
        MailSenderControl.SubjectValue = previous.Subject
        MailSenderControl.BodyValue = previous.Body
        MailSenderControl.Documents = previous.Documents

        If ProtocolEnv.DeleteMultipleMailRecipientPages.Contains(CURRENT_PAGE_NAME) Then MailSenderControl.EnableCheckBoxRecipients = True

        ' Imposto il ritorno saltando 1 livello (se richiesto dalla request)
        If OverridePreviousPageUrl Then
            Dim previousBasePage As CommonBasePage = DirectCast(PreviousPage, CommonBasePage)
            If previousBasePage IsNot Nothing AndAlso Not String.IsNullOrEmpty(previousBasePage.PreviousPageUrl) Then
                PreviousPageUrl = previousBasePage.PreviousPageUrl
            End If
        End If

        MailSenderControl.DataBind()
    End Sub

    Private Sub MailSenderControlEvent(sender As Object, e As EventArgs) Handles MailSenderControl.CancelByUser, MailSenderControl.ConfirmByUser
        Response.Redirect(PreviousPageUrl)
    End Sub
#End Region

End Class