Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Logging

Partial Public Class uscFascInsertMiscellanea
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Private Const DELETE_DOCUMENT As String = "Delete_Document"
    Private Const RELOAD_CALLBACK As String = "uscFascInsertMiscellanea.refreshDocuments()"
    Private _fascMiscellaneaLocation As Location = Nothing
#End Region

#Region " Properties "
    Public ReadOnly Property FascMiscellaneaLocation() As Location
        Get
            If _fascMiscellaneaLocation Is Nothing Then
                _fascMiscellaneaLocation = Facade.LocationFacade.GetById(ProtocolEnv.FascicleMiscellaneaLocation)
            End If
            Return _fascMiscellaneaLocation
        End Get
    End Property

    Public ReadOnly Property ArchiveName() As String
        Get
            Return FascMiscellaneaLocation.ProtBiblosDSDB
        End Get
    End Property

    Protected ReadOnly Property IdFascicleFolder As Guid?
        Get
            Return GetKeyValueOrDefault(Of Guid?)("IdFascicleFolder", Nothing)
        End Get
    End Property

    Protected ReadOnly Property IdFascicle As Guid?
        Get
            Return GetKeyValueOrDefault(Of Guid?)("IdFascicle", Nothing)
        End Get
    End Property

    Public Property OnlySignEnabled As Boolean

    Public Property FilterByArchiveDocumentId As Guid?

#End Region

#Region " Events "
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        If Not IsPostBack Then
        End If

        If OnlySignEnabled Then
            radPaneButton.Visible = False
            uscMiscellanea.OnlySignEnabled = OnlySignEnabled
            uscMiscellanea.FilterByArchiveDocumentId = FilterByArchiveDocumentId
        End If
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf FascMiscellaneaAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(uscMiscellanea, uscMiscellanea)
    End Sub

    Public Overloads Function GetKeyValueOrDefault(Of T)(key As String, defaultValue As T) As T
        Return Context.Request.QueryString.GetValueOrDefault(key, defaultValue)
    End Function

    Protected Sub FascMiscellaneaAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, String.Concat("Errore interno di sistema durante l'eliminazione del documento di inserti: ", ex.Message), ex)
            Return
        End Try

        Dim idDocument As Guid

        If (ajaxModel IsNot Nothing AndAlso String.Equals(ajaxModel.ActionName, DELETE_DOCUMENT) AndAlso ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 1 _
            AndAlso Not String.IsNullOrEmpty(ajaxModel.Value(0)) AndAlso Guid.TryParse(ajaxModel.Value(0), idDocument)) Then
            If DeleteDocument(idDocument) Then
                AjaxManager.ResponseScripts.Add(RELOAD_CALLBACK)
            Else
                BasePage.AjaxAlert("E' avvenuta una anomalia durante la cancellazione dell'inserto. Contattare l'assistenza.")
            End If

        End If

    End Sub

    Private Function DeleteDocument(idDocument As Guid) As Boolean
        FileLogger.Debug(LoggerName, String.Format("uscFascInsertMiscellanea_AjaxRequest -> IdFascicle {0} - Delete document with Id: {1}", IdFascicle, idDocument))
        Try
            Service.DetachDocument(FascMiscellaneaLocation.ProtBiblosDSDB, idDocument)
            Return True
        Catch ex As Exception
            FileLogger.Warn(LoggerName, String.Concat("Errore in eliminazione documento inserti: ", ex.Message), ex)
            Return False
        End Try
    End Function
#End Region

End Class

