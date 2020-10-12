Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Logging
Partial Public Class DossierMiscellanea
    Inherits DossierBasePage

#Region " Fields "
    Private Const DELETE_DOCUMENT As String = "Delete_Document"
    Private Const RELOAD_CALLBACK As String = "dossierMiscellanea.refreshDocuments()"
    Private _dossierMiscellaneaLocation As Location = Nothing
#End Region

#Region " Properties "
    Public ReadOnly Property DossierMiscellaneaLocation() As Location
        Get
            If _dossierMiscellaneaLocation Is Nothing Then
                _dossierMiscellaneaLocation = Facade.LocationFacade.GetById(ProtocolEnv.DossierMiscellaneaLocation)
            End If
            Return _dossierMiscellaneaLocation
        End Get
    End Property

    Public ReadOnly Property ArchiveName() As String
        Get
            Return DossierMiscellaneaLocation.ProtBiblosDSDB
        End Get
    End Property
#End Region

#Region " Events "
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        If Not IsPostBack Then

        End If
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf DossierMiscellaneaAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(uscMiscellanea, uscMiscellanea)
    End Sub

    Protected Sub DossierMiscellaneaAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore in eliminazione documento inserti: " & ex.Message, ex)
            Return
        End Try

        Dim idDocument As Guid

        If (ajaxModel IsNot Nothing AndAlso String.Equals(ajaxModel.ActionName, DELETE_DOCUMENT) AndAlso ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 1 _
            AndAlso Not String.IsNullOrEmpty(ajaxModel.Value(0)) AndAlso Guid.TryParse(ajaxModel.Value(0), idDocument)) Then
            DeleteDocument(idDocument)
            AjaxManager.ResponseScripts.Add(RELOAD_CALLBACK)
        End If

    End Sub

    Private Sub DeleteDocument(idDocument As Guid)
        FileLogger.Debug(LoggerName, String.Format("DossierMiscellanea_AjaxRequest -> IdDossier {0} - Delete document with Id: {1}", IdDossier, idDocument))
        Try
            Service.DetachDocument(idDocument)
        Catch ex As Exception
            AjaxAlert("Errore in eliminazione documento inserti")
            FileLogger.Warn(LoggerName, "Errore in eliminazione documento inserti: " & ex.Message, ex)
            Exit Sub
        End Try
    End Sub
#End Region

End Class

