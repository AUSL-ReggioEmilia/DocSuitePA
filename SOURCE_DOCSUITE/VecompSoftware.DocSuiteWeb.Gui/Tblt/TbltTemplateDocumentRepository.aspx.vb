Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Extensions
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Logging

Partial Class TbltTemplateDocumentRepository
    Inherits CommonBasePage

#Region "Fields"
    Private Const PAGE_TITLE As String = "Gestione Deposito Documentale"
    Private Const HIDE_LOADING_FUNCTION As String = "tbltTemplateDocumentRepository.hideLoadingPanel('{0}')"
    Private Const RESET_BUTTONS_STATE_FUNCTION As String = "tbltTemplateDocumentRepository.resetButtonsState()"
    Private Const DELETE_CALLBACK_FUNCTION As String = "tbltTemplateDocumentRepository.deleteCallback()"
    Private _argument As String = String.Empty
#End Region

#Region "Properties"

#End Region

#Region "Events"
    Protected Sub btnElimina_Click(sender As Object, e As EventArgs) Handles btnElimina.Click
        If Not String.IsNullOrEmpty(_argument) Then
            Try
                Dim idChain As Guid = Nothing
                If Not Guid.TryParse(_argument, idChain) Then
                    AjaxAlert(String.Format("Il valore '{0}' non è un identificativo del documento valido", _argument))
                End If

                Service.DetachDocument(String.Empty, idChain)
                AjaxManager.ResponseScripts.Add(DELETE_CALLBACK_FUNCTION)
            Catch ex As Exception
                FileLogger.Error(LoggerName, ex.Message, ex)
                AjaxAlert("Errore in fase Detach del Documento, impossibile eseguire.")
                btnElimina.Enabled = True
                AjaxManager.ResponseScripts.Add(String.Format(HIDE_LOADING_FUNCTION, splContent.ClientID))
            Finally
                AjaxManager.ResponseScripts.Add(RESET_BUTTONS_STATE_FUNCTION)
            End Try
        End If
    End Sub

    Protected Overrides Sub RaisePostBackEvent(sourceControl As IPostBackEventHandler, eventArgument As String)
        If (sourceControl.Equals(btnElimina)) Then
            _argument = eventArgument
        End If

        MyBase.RaisePostBackEvent(sourceControl, eventArgument)
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region " Methods "
    Private Sub Initialize()
        Page.Title = PAGE_TITLE
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnElimina, btnElimina)
    End Sub
#End Region

End Class

