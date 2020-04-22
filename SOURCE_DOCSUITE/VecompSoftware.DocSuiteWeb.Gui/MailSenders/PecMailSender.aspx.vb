Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Gui.MailSenders
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class PecMailSender
    Inherits CommonBasePage

#Region " Fields "

    Private _currentItem As PECMail
    Private Const CURRENT_PAGE_NAME As String = "PecMailSender"
#End Region

#Region " Properties "

    Private ReadOnly Property IdItem As Integer
        Get
            Return Request.QueryString.GetValue(Of Integer)("PECId")
        End Get
    End Property

    Private ReadOnly Property LoadRecipients As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("recipients", True)
        End Get
    End Property

    Private ReadOnly Property CurrentItem As PECMail
        Get
            If _currentItem Is Nothing Then
                _currentItem = Facade.PECMailFacade.GetById(IdItem)
            End If
            Return _currentItem
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            MasterDocSuite.TitleVisible = False
            Initialize()
        End If
    End Sub

    Private Sub MailSenderControl_MailSent(sender As Object, e As MailSentEventArgs) Handles MailSenderControl.MailSent

        If CurrentItem IsNot Nothing AndAlso e.Mail IsNot Nothing Then
            Dim destinatari As IList(Of MessageContact) = Facade.MessageContactFacade.GetByMessage(e.Mail.Message, MessageContact.ContactPositionEnum.Recipient)
            Dim contactsMail As IList(Of MessageContactEmail) = New List(Of MessageContactEmail)(destinatari.Count)
            For Each contact As MessageContact In destinatari
                contactsMail.Add(Facade.MessageContactEmailFacade.GetByContact(contact))
            Next
            e.Mail.Message.MessageContacts = destinatari
            Facade.PECMailLogFacade.Sended(CurrentItem, e.Mail, contactsMail)
        End If

    End Sub
#End Region

#Region " Methods "

    Private Sub Initialize()
        Dim previous As ISendMail = DirectCast(PreviousPage, ISendMail)
        If previous IsNot Nothing Then
            ' Se arriva da pagina con implementazione
            MailSenderControl.SenderDescriptionValue = previous.SenderDescription
            MailSenderControl.SenderEmailValue = previous.SenderEmail
            MailSenderControl.SubjectValue = previous.Subject
            MailSenderControl.BodyValue = previous.Body
            MailSenderControl.Documents = previous.Documents
        Else
            Throw New DocSuiteException("Errore pagina di invio mail", "Impossibile inizializzare la mail di invio")
        End If

        If ProtocolEnv.DeleteMultipleMailRecipientPages.Contains(CURRENT_PAGE_NAME) Then MailSenderControl.EnableCheckBoxRecipients = True

        If LoadRecipients AndAlso CurrentItem IsNot Nothing Then
            MailSenderControl.Recipients = previous.Recipients
        End If

        MailSenderControl.DataBind()
    End Sub

    Private Sub MailSenderControlEvent(sender As Object, e As EventArgs) Handles MailSenderControl.CancelByUser, MailSenderControl.ConfirmByUser
        Response.Redirect(PreviousPageUrl)
    End Sub

#End Region

End Class