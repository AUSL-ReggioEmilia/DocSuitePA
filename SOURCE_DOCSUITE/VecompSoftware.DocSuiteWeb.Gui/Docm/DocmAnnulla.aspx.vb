Imports VecompSoftware.Services.Logging

Partial Public Class DocmAnnulla
    Inherits DocmBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirm, btnConfirm)
        uscDocumentData.CurrentDocument = CurrentDocument
        If Not Me.IsPostBack Then
            Initialize()
        End If
    End Sub

#Region "Initialize"
    Private Sub Initialize()
        uscDocumentData.Show()

        uscDocumentData.VisibleGenerale = True
        uscDocumentData.VisiblePratica = True
        uscDocumentData.VisibleAltri = False
        uscDocumentData.VisibleClassificatoreModifica = False
        uscDocumentData.VisibleClassificazione = False
        uscDocumentData.VisibleContatti = False
        uscDocumentData.VisibleDate = False
        uscDocumentData.VisibleDateModifica = False
        uscDocumentData.VisibleDateSovrapposte = False
        uscDocumentData.VisibleDati = False
        uscDocumentData.VisibleDatiModifica = False
    End Sub
#End Region

#Region "Confirm Button Handler"
    Private Sub btnConfirm_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirm.Click
        If CurrentDocument IsNot Nothing Then
            Try
                Facade.DocumentFacade.Cancel(CurrentDocument, txtCancelDescription.Text)
            Catch ex As Exception
                FileLogger.Warn(LoggerName, "Errore in Annullamento della Pratica a causa di: " & ex.Message, ex)
                AjaxAlert("Errore in Annullamento della Pratica a causa di: " & ex.Message)
            End Try
            Me.RegisterFolderRefreshFullScript()
        End If
    End Sub
#End Region
    
End Class