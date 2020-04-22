
Namespace MailSenders
    Public Class MailErrorEventArgs
        Inherits EventArgs

        Public Message As String
        Public Ex As Exception

        Public Sub New()

        End Sub
        Public Sub New(exception As Exception)
            Message = exception.Message
            Ex = exception
        End Sub

    End Class
End NameSpace