Imports System.Net.Mail
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Partial Class UtltSmtpMail
    Inherits SuperAdminPage

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not Me.IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnInvia_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnInvia.Click
        FncSmtpLogSendMail()
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        'Put user code to initialize the page here
        txtMailSmtpServer.Text = DocSuiteContext.Current.ProtocolEnv.MailSmtpServer
        txtUserErrorMailFrom.Text = DocSuiteContext.Current.ProtocolEnv.UserErrorMailFrom
        txtUserErrorMailTo.Text = DocSuiteContext.Current.ProtocolEnv.UserErrorMailTo
    End Sub

    Public Function FncSmtpLogSendMail() As Boolean
        Dim mm As New MailMessage
        mm.IsBodyHtml = True
        mm.From = New MailAddress(txtUserErrorMailFrom.Text)
        mm.To.Add(New MailAddress(txtUserErrorMailTo.Text))
        mm.Subject = DocSuiteContext.ProductName & " Prova"
        mm.Body = "Testo di Prova"

        Dim server As New SmtpClient(txtMailSmtpServer.Text)
        server.UseDefaultCredentials = True
        server.Send(mm)
        FncSmtpLogSendMail = True
    End Function

#End Region

End Class
