Imports VecompSoftware.DocSuiteWeb.Data

Namespace MailSenders
    Public Class MailSentEventArgs
        Inherits EventArgs

        Public Mail As MessageEmail
        Public ShowConfirm As Boolean

        Public Sub New(mailSent As MessageEmail)
            Mail = mailSent
            ShowConfirm = DocSuiteContext.Current.ProtocolEnv.ShowConfirmMessageOnSendMail
        End Sub

    End Class
End NameSpace