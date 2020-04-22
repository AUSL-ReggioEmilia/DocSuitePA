Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class GenericMailSender
    Inherits CommBasePage

#Region "Fields"
    Private Const CURRENT_PAGE_NAME As String = "GenericMailSender"
#End Region

#Region " Property "

    Private ReadOnly Property LoadRecipients As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("recipients", True)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then

            Title = String.Format("{0} - Invio", PECBasePage.EmailLabel)

            Dim previous As ISendMail = DirectCast(PreviousPage, ISendMail)
            If previous IsNot Nothing Then
                MailSenderControl.SenderDescriptionValue = previous.SenderDescription

                MailSenderControl.SenderEmailValue = previous.SenderEmail
                MailSenderControl.SubjectValue = previous.Subject
                MailSenderControl.BodyValue = previous.Body

                MailSenderControl.Documents = previous.Documents
                If LoadRecipients Then
                    MailSenderControl.Recipients = previous.Recipients
                End If

                If ProtocolEnv.DeleteMultipleMailRecipientPages.Contains(CURRENT_PAGE_NAME) Then
                    MailSenderControl.EnableCheckBoxRecipients = True
                End If
                MailSenderControl.EnableAttachment = DocSuiteContext.Current.GetEnableAttachmentByPage("GenericMailSender")
                MailSenderControl.DataBind()

            Else
                MailSenderControl.SenderDescriptionValue = CommonInstance.UserDescription
                MailSenderControl.SenderEmailValue = CommonInstance.UserMail
            End If

        End If
    End Sub

    Private Sub MailSenderControl_CancelByUser(sender As Object, e As EventArgs) Handles MailSenderControl.CancelByUser, MailSenderControl.ConfirmByUser
        Response.Redirect(PreviousPageUrl)
    End Sub

#End Region

End Class