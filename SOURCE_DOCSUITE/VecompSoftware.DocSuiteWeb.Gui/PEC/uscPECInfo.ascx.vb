Imports VecompSoftware.DocSuiteWeb.Data

Public Class uscPECInfo
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Private _mail As PECMail
    Private _sessionMail As PECMail
    Private _pecLabel As String = "PEC"

#End Region

#Region " Properties  "

    Public Property SessionPECMail As PECMail
        Get
            Return _sessionMail
        End Get
        Set(value As PECMail)
            _sessionMail = value
        End Set
    End Property

    Public Property PECMailId As Integer?

    Public Property PECMail As PECMail
        Get
            If _mail Is Nothing AndAlso ViewState("pecId") IsNot Nothing Then
                _mail = Facade.PECMailFacade.GetById(DirectCast(ViewState("pecId"), Integer))
            End If
            If _mail Is Nothing AndAlso PECMailId.HasValue Then
                _mail = Facade.PECMailFacade.GetById(PECMailId.Value)
            End If
            Return _mail
        End Get
        Set(value As PECMail)
            _mail = value
            ViewState("pecId") = If(value IsNot Nothing, value.Id, Nothing)
        End Set
    End Property

    Public Property PecLabel As String
        Get
            If ViewState("pecLabel") IsNot Nothing Then
                _pecLabel = ViewState("pecLabel").ToString()
            Else
                ViewState("pecLabel") = _pecLabel
            End If
            Return _pecLabel
        End Get
        Set(value As String)
            ViewState("pecLabel") = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack AndAlso Visible Then
            If PECMail IsNot Nothing Then
                BindData(PECMail)
            ElseIf SessionPECMail IsNot Nothing Then
                BindData(SessionPECMail)
            End If
        End If
       
    End Sub

#End Region

#Region " Methods "

    Public Sub BindData(mail As PECMail)
        lblDetail.Text = String.Format("Dettaglio {0} [{1}]", PecLabel, mail.Id)
        lblFrom.Text = Server.HtmlEncode(mail.MailSenders)
        lblTo.Text = Server.HtmlEncode(mail.MailRecipients)
        lblMailBox.Text = Facade.PECMailboxFacade.MailBoxRecipientLabel(mail.MailBox)
        lblDate.Text = If(mail.MailDate.HasValue, mail.MailDate.Value.ToString("dd/MM/yyyy HH:mm"), "NON ANCORA SPEDITA")
        lblSubject.Text = mail.MailSubject
    End Sub

#End Region

End Class