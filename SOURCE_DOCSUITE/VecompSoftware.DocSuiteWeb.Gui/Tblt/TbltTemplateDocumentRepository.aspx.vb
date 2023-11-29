Imports Newtonsoft.Json
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Logging
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Partial Class TbltTemplateDocumentRepository
    Inherits CommonBasePage

#Region "Fields"
    Private Const PAGE_TITLE As String = "Gestione Deposito Documentale"
    Private Const HIDE_LOADING_FUNCTION As String = "tbltTemplateDocumentRepository.hideLoadingPanel('{0}')"
    Private Const RESET_BUTTONS_STATE_FUNCTION As String = "tbltTemplateDocumentRepository.resetButtonsState()"
    Private Const DELETE_CALLBACK_FUNCTION As String = "tbltTemplateDocumentRepository.deleteCallback()"
    Private Shared DELETE_OPTION As String = "delete"
#End Region

#Region "Properties"

#End Region

#Region "Events"
    Protected Sub DeleteTemplate(idArchiveChain As String)
        If Not String.IsNullOrEmpty(idArchiveChain) Then
            Try
                Dim idChain As Guid = Nothing
                If Not Guid.TryParse(idArchiveChain, idChain) Then
                    AjaxAlert(String.Format("Il valore '{0}' non è un identificativo del documento valido", idArchiveChain))
                End If

                Service.DetachDocument(idChain)
                AjaxManager.ResponseScripts.Add(DELETE_CALLBACK_FUNCTION)
            Catch ex As Exception
                FileLogger.Error(LoggerName, ex.Message, ex)
                AjaxAlert("Errore in fase Detach del Documento, impossibile eseguire.")
                uscTemplateDocumentRepository.FolderToolBar_Grid.FindItemByValue(DELETE_OPTION).Enabled = True
                AjaxManager.ResponseScripts.Add(String.Format(HIDE_LOADING_FUNCTION, splContent.ClientID))
            Finally
                AjaxManager.ResponseScripts.Add(RESET_BUTTONS_STATE_FUNCTION)
            End Try
        End If
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.UserConnectedBelongsTo(ProtocolEnv.TemplateDocumentGroups)) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        InitializeAjax()
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub DeleteTemplate_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        If ajaxModel Is Nothing Then
            Return
        End If
        Select Case ajaxModel.ActionName
            Case "DeleteTemplate"
                Dim idArchiveChain As String = ajaxModel.Value(0)
                DeleteTemplate(idArchiveChain)
                Exit Select
        End Select
    End Sub
#End Region

#Region " Methods "
    Private Sub Initialize()
        Page.Title = PAGE_TITLE
    End Sub

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf DeleteTemplate_AjaxRequest
    End Sub
#End Region

End Class

