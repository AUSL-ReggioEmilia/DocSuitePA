Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ProtPecSend
    Inherits ProtBasePage

#Region " Fields "

    Private _selectedMails As String()

#End Region

#Region " Properties "

    Private ReadOnly Property SimpleMode As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("SimpleMode", False)
        End Get
    End Property

    ''' <summary> Elenco degli id delle PEC selezionate. </summary>
    Private ReadOnly Property SelectedMails As String()
        Get
            If _selectedMails Is Nothing AndAlso Not String.IsNullOrEmpty(Request.QueryString.Item("selectedMails")) Then
                _selectedMails = Request.QueryString.Item("selectedMails").Split("|"c)
            End If
            Return _selectedMails
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False

        If Not IsPostBack Then
            If SelectedMails Is Nothing OrElse SelectedMails.Length = 0 Then
                AjaxAlert("Nessuna mail selezionata.")
                Exit Sub
            End If
            For Each pec As String In SelectedMails
                Dim currentPecMail As PECMail = Facade.PECMailFacade.GetById(Integer.Parse(pec))
                blsMailAttachments.Items.Add(New ListItem(String.Concat(currentPecMail.MailSubject, FileHelper.EML), currentPecMail.Id.ToString()))
            Next
        End If
        uscDestinatari.SimpleMode = SimpleMode
        MasterDocSuite.TitleVisible = False
    End Sub

    Private Sub cmdSend_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSend.Click
        Try
            SendMail()
            If DocSuiteContext.Current.ProtocolEnv.PECForwardSuccessfulEnabled Then
                AjaxAlert("Inoltro avvenuto con successo.")
            End If
            AjaxManager.ResponseScripts.Add("GetRadWindow().close();")
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore in fase spedizione", ex)
            AjaxAlert("Errore in fase spedizione, contattare l'assistenza.")
        End Try
    End Sub

#End Region

#Region " Methods "

    ''' <summary> Invia la mail con allegate le PEC selezionate. </summary>
    Private Sub SendMail()

        Dim mm As New Net.Mail.MailMessage()

        ' Mittente
        If String.IsNullOrEmpty(ProtocolEnv.ProtPecSendSender) Then
            Throw New Exception("Impossibile inviare mail: Parametro ProtPecSendSender non configurato.")
        End If
        mm.From = New Net.Mail.MailAddress(ProtocolEnv.ProtPecSendSender)

        ' Destinatari e Copia Conoscenza
        For Each c As ContactDTO In uscDestinatari.GetContacts(False)
            Dim mail As String
            If Not String.IsNullOrEmpty(c.Contact.EmailAddress) Then
                mail = c.Contact.EmailAddress
            ElseIf Not String.IsNullOrEmpty(c.Contact.CertifiedMail) Then
                mail = c.Contact.CertifiedMail
            Else
                Throw New Exception(String.Format("{0} non ha un indirizzo email valido.", c.Contact.Description))
            End If

            If c.IsCopiaConoscenza Then
                mm.CC.Add(mail)
            Else
                mm.To.Add(mail)
            End If
        Next

        ' Oggetto e Corpo
        mm.Subject = txtMailSubject.Text
        mm.Body = txtMailBody.Text

        ' Allegati
        For Each pec As ListItem In blsMailAttachments.Items
            If Not String.IsNullOrEmpty(pec.Value) Then
                Dim currentPecMail As PECMail = Facade.PECMailFacade.GetById(pec.Value)
                Dim content As String
                If currentPecMail.Direction = PECMailDirection.Outgoing Then
                    content = currentPecMail.MailContent
                    If (content = Nothing) Then content = String.Empty
                Else
                    Dim mailContent As PECMailContent = Facade.PECMailContentFacade.GetByMail(currentPecMail)
                    If mailContent IsNot Nothing Then
                        content = mailContent.MailContent
                    End If
                End If
                Dim mailAttachment As Net.Mail.Attachment = Net.Mail.Attachment.CreateAttachmentFromString(content, String.Concat(currentPecMail.MailSubject, FileHelper.Eml))
                mm.Attachments.Add(mailAttachment)
                ' Log inoltra (Forward)
                'Facade.PECMailLogFacade.Forward(currentPecMail)
            End If
        Next

        ' Invio Mail
        Dim client As New Net.Mail.SmtpClient(DocSuiteContext.Current.ProtocolEnv.MailSmtpServer)
        client.UseDefaultCredentials = True
        client.Send(mm)
    End Sub

#End Region

End Class