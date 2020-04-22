
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Partial Public Class ReslUltimaPagina
    Inherits ReslBasePage

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        If uscDocumentUpload.DocumentInfosAdded.Count = 0 Then
            AjaxAlert("Nessun documento presente. Selezionare un documento.")
        End If

        Try
            Dim lastPageDocument As DocumentInfo = uscDocumentUpload.DocumentInfosAdded.First()
            lastPageDocument.Name = "UltimaPagina.pdf"
            lastPageDocument.Signature = Facade.ResolutionFacade.SqlResolutionGetNumber(idResolution:=CurrentResolution.Id, complete:=True)
            Dim idChain As Integer = lastPageDocument.ArchiveInBiblos(CurrentResolution.Location.DocumentServer, CurrentResolution.Location.ReslBiblosDSDB).BiblosChainId
            Facade.ResolutionFacade.SqlResolutionDocumentUpdate(CurrentResolution.Id, idChain, ResolutionFacade.DocType.UltimaPagina)
            Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RP, "Caricata ultima pagina")
            AjaxManager.ResponseScripts.Add("CloseWindow();")
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore durante il salvataggio del documento su Biblos", ex)
            AjaxAlert("Errore durante il salvataggio del documento su Biblos")
        End Try
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        Title = String.Concat(Facade.TabMasterFacade.TreeViewCaption, " - Ultima pagina")
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, btnSave)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, pageContent, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

#End Region

End Class