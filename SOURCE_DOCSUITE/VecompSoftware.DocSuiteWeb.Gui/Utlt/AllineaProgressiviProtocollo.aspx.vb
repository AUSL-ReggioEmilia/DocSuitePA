Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public Class AllineaProgressiviProtocollo
    Inherits CommonBasePage

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnConferma_Click(sender As Object, e As System.EventArgs) Handles btnConferma.Click
        Dim parameter As Parameter = Facade.ParameterFacade.GetCurrentAndRefresh()
        Facade.ParameterFacade.UpdateProtocolLastUsedNumber(parameter.LastUsedYear)

        AjaxAlert("Allineamento eseguito con successo.")

        Initialize()
    End Sub

#End Region

    Private Sub Initialize()
        Dim parameter As Parameter = Facade.ParameterFacade.GetCurrentAndRefresh()
        Dim max As Integer = Facade.ProtocolFacade.GetMaxProtocolNumber(parameter.LastUsedYear)
        txtYearNumber.Text = ProtocolFacade.ProtocolFullNumber(parameter.LastUsedYear, max)
        txtParameterNumber.Text = ProtocolFacade.ProtocolFullNumber(parameter.LastUsedYear, parameter.LastUsedNumber)

        If max + 1 <> parameter.LastUsedNumber Then
            btnConferma.Enabled = True
        Else
            btnConferma.Enabled = False
        End If
    End Sub

End Class