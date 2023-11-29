Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Logging

Public Class ReslPecOcSummary
    Inherits ReslBasePage

#Region " Fields "
    Dim _currentPecOc As PECOC
    Dim _currentPecMail As PECMail
#End Region

#Region " Properties "

    Private ReadOnly Property CurrentPecOc As PECOC
        Get
            If _currentPecOc IsNot Nothing Then
                Return _currentPecOc
            End If

            Dim idPecOc As Integer
            If Not Integer.TryParse(Request("PECOC"), idPecOc) Then
                Throw New DocSuiteException("Errore PEC Collegio Sindacale", String.Format("Errore in recupero richiesta [{0}].", Request("PECOC")))
            End If

            _currentPecOc = Facade.PECOCFacade.GetById(idPecOc)
            Return _currentPecOc

        End Get
    End Property

    Protected ReadOnly Property CurrentPecMail As PECMail
        Get
            If _currentPecMail Is Nothing AndAlso CurrentPecOc.IdMail.HasValue Then
                _currentPecMail = Facade.PECMailFacade.GetById(CurrentPecOc.IdMail.Value)
            End If
            Return _currentPecMail
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub DeleteClick(sender As Object, e As EventArgs) Handles Delete.Click
        DeletePecOc()
    End Sub

    Protected Sub SendClick(sender As Object, e As EventArgs) Handles Send.Click
        MailBody.Text = CurrentPecMail.MailBody
        editForm.Visible = True
    End Sub

    Protected Sub ConfermaClick(sender As Object, e As EventArgs) Handles Confirm.Click
        Try
            ' Sposto la mail dalla mailbox delle bozze a quella destinata all'invio.
            Dim sourceMailBox As PECMailBox = CurrentPecMail.MailBox
            CurrentPecMail.MailBox = ResolutionEnv.MailBoxCollegioSindacale

            ' Aggiorno il corpo della mail con le modifiche fatte in anteprima.
            CurrentPecMail.MailBody = MailBody.Text

            ' Attivo la mail.
            CurrentPecMail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active)

            Facade.PECMailFacade.Update(CurrentPecMail)
            Facade.PECMailLogFacade.Moved(CurrentPecMail, sourceMailBox, CurrentPecMail.MailBox, "Invio automatico al collegio sindacale.")

            Initialize()
            AjaxAlert("...Inserito correttamente nella coda di invio.")
        Catch ex As Exception
            Dim message As String = String.Format("Errore durante l'inserimento nella coda di invio del messaggio: {0}.", ex.Message)
            AjaxAlert(message)
        End Try
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(Send, editForm, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(Confirm, editForm, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(Confirm, Send)
    End Sub

    Private Sub Initialize()
        ReslType.Text = CurrentPecOc.ResolutionType.Description
        ExtractAttachment.Checked = CurrentPecOc.ExtractAttachments
        If CurrentPecOc.ToDate.HasValue Then
            Data.Text = String.Format("Dal {0} al {1}", CurrentPecOc.FromDate.ToString("dd/MM/yyyy"), CurrentPecOc.ToDate.Value.ToString("dd/MM/yyyy"))
        Else
            Data.Text = CurrentPecOc.FromDate.ToString("dd/MM/yyyy")
        End If
        Status.Text = CurrentPecOc.Status.ToString("G")
        If CurrentPecMail IsNot Nothing AndAlso CurrentPecMail.MailBox.Id = ResolutionEnv.MailBoxCollegioSindacale.Id Then
            Status.Text &= " (in coda di invio)"
        End If
        Send.Visible = CurrentPecOc.Status = PECOCStatus.Completo AndAlso CurrentPecMail.MailBox.Id <> ResolutionEnv.MailBoxCollegioSindacale.Id
        editForm.Visible = False
    End Sub

    Private Sub DeletePecOc()
        Try
            CurrentPecOc.IsActive = False
            CurrentPecOc.Status = PECOCStatus.Cancellato
            Facade.PECOCFacade.Update(CurrentPecOc)
            Facade.PECOCLogFacade.InsertLog(CurrentPecOc)
            AjaxManager.ResponseScripts.Add(String.Format("location.href = '{0}/Resl/ReslPecOc.aspx?Type=Resl'", DocSuiteContext.Current.CurrentTenant.DSWUrl))
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            AjaxAlert("Cancellazione non riuscita.")
        End Try
    End Sub
#End Region

End Class