Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Logging

Partial Public Class ProtModificaOggettiImport
    Inherits ProtBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjaxSettings()
    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscObjectFinder, lblCounter)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscObjectFinder, btnConferma)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, lblCounter)
    End Sub

    Private Sub LoadProtocols(ByVal finder As NHibernateProtocolObjectFinder)
        finder.LoadOnlyProtocolObjectToImport = True
        Dim count As Integer = finder.Count()
        lblCounter.Text = "Trovati " & count & " protocolli da importare."
        btnConferma.Visible = count > 0
    End Sub

    Private Sub uscObjectFinder_DoSearch(ByVal sender As Object, ByVal e As ProtocolObjectFinderEventArgs) Handles uscObjectFinder.DoSearch
        LoadProtocols(e.Finder)
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConferma.Click
        Dim finder As NHibernateProtocolObjectFinder = uscObjectFinder.Finder
        Try
            'Facade.ProtocolFacade.ImportProtocolObject(finder.RegistrationDateFrom, finder.RegistrationDateTo, finder.IdContainer) ' Un attimo qua... - FG
            LoadProtocols(finder)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore in importazione protocolli a causa di: " & ex.Message, ex)
            AjaxAlert("Errore in importazione protocolli a causa di: " & ex.Message)
        End Try
    End Sub
End Class