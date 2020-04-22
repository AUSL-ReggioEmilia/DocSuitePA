Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class ReslElimina
    Inherits ReslBasePage

#Region " Properties "

    Public ReadOnly Property AjaxDefaultLoadingPanel() As RadAjaxLoadingPanel
        Get
            Return DefaultLoadingPanel
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, pnlMain, AjaxDefaultLoadingPanel)
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConferma.Click
        Dim returnAction As String
        Select Case Action
            Case "Ann", "Ado"
                returnAction = "CANCEL"

                CurrentResolution.Status = Facade.ResolutionStatusFacade.GetById(ResolutionStatusId.Annullato)
                CurrentResolution.LastChangedReason = rtbAnnulmentReason.Text

                Try
                    Facade.ResolutionFacade.Update(CurrentResolution)
                    Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RC, "L'atto è stato annullato.")
                Catch ex As Exception
                    Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RX, "ATTI.ANNULLAMENTO: Errore in Aggiornamento Stato")
                    AjaxAlert("Errore in Aggiornamento Stato")
                    Exit Sub
                End Try
            Case Else
                returnAction = "DELETE"
        End Select
        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", returnAction))
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        If CurrentResolution Is Nothing Then
            Exit Sub
        End If
        Select Case Action
            Case "Ann"
                Title = "Annulla Proposta"
                lblTitolo.Text = String.Format("Proposta: {0}", CurrentResolution.IdFull)
            Case "Ado"
                Dim id As String = ""
                Dim idFull As String = ""
                Dim s As String = Facade.ResolutionTypeFacade.GetDescription(CurrentResolution.Type)
                Title = "Annulla " & s
                Facade.ResolutionFacade.ReslFullNumber(CurrentResolution, CurrentResolution.Type.Id, id, idFull, False)
                lblTitolo.Text = s & ": " & idFull
            Case Else
                lblTitolo.Text = String.Format("Proposta: {0}", CurrentResolution.IdFull)
        End Select
    End Sub

#End Region

End Class
