Imports System.Collections.Generic
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Public Class FascMoveItems
    Inherits FascBasePage

#Region " Fields "
    Private _miscellaneaLocation As Location
    Public Const MOVE_MISCELLANEA_DOCUMENT As String = "MoveMiscellaneaDocument"
    Private Const DEFAULT_ERROR_MESSAGE As String = "E' avvenuta una anomalia, contattare l'assistenza"
    Private Const MOVE_MISCELLANEA_CALLBACK_FORMAT As String = "fascMoveItems.moveMiscellaneaDocumentsCallback('{0}','{1}',{2});"
#End Region

#Region " Properties "
    Public ReadOnly Property ItemsType As String
        Get
            Return GetKeyValueOrDefault(Of String)("ItemsType", String.Empty)
        End Get
    End Property

    Private ReadOnly Property MiscellaneaLocation As Location
        Get
            If _miscellaneaLocation Is Nothing Then
                _miscellaneaLocation = Facade.LocationFacade.GetById(ProtocolEnv.FascicleMiscellaneaLocation)
            End If
            Return _miscellaneaLocation
        End Get
    End Property
    Protected ReadOnly Property DestinationFascicleId As Guid?
        Get
            Return GetKeyValueOrDefault(Of Guid?)("DestinationFascicleId", Nothing)
        End Get
    End Property
    Protected ReadOnly Property MoveToFascicle As Boolean?
        Get
            Return GetKeyValueOrDefault(Of Boolean?)("MoveToFascicle", Nothing)
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub FascMoveItemsAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Dim errorMessage As String = String.Empty
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, String.Concat("E' avvenuta una anomalia, contattare l'assistenza: ", ex.Message), ex)
            AjaxManager.ResponseScripts.Add(String.Format(MOVE_MISCELLANEA_CALLBACK_FORMAT, HttpUtility.JavaScriptStringEncode(DEFAULT_ERROR_MESSAGE), String.Empty, False.ToString().ToLower()))
            Return
        End Try

        Dim idDocuments As ICollection(Of Guid)
        Dim idFascicleFolderChain As Guid?
        Dim toCreateNewChain As Boolean = False
        Try
            If (ajaxModel IsNot Nothing AndAlso String.Equals(ajaxModel.ActionName, MOVE_MISCELLANEA_DOCUMENT) AndAlso ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 1 _
            AndAlso Not String.IsNullOrEmpty(ajaxModel.Value(0))) Then
                idDocuments = JsonConvert.DeserializeObject(Of List(Of Guid))(ajaxModel.Value(0))
                If Not String.IsNullOrEmpty(ajaxModel.Value(1)) Then
                    idFascicleFolderChain = Guid.Parse(ajaxModel.Value(1))
                End If
                toCreateNewChain = Not idFascicleFolderChain.HasValue OrElse idFascicleFolderChain.Value.Equals(Guid.Empty)

                For Each idDocument As Guid In idDocuments
                    idFascicleFolderChain = MoveMiscellaneaDocument(idDocument, idFascicleFolderChain)
                Next
            End If
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            errorMessage = DEFAULT_ERROR_MESSAGE
        End Try
        AjaxManager.ResponseScripts.Add(String.Format(MOVE_MISCELLANEA_CALLBACK_FORMAT, HttpUtility.JavaScriptStringEncode(errorMessage), idFascicleFolderChain, toCreateNewChain.ToString().ToLower()))
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        MasterDocSuite.TitleVisible = False
        uscFascicleFolders.FoldersToDisabled = New List(Of Guid) From {IdFascicleFolder.Value}
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf FascMoveItemsAjaxRequest
    End Sub

    Private Function MoveMiscellaneaDocument(idDocument As Guid, idFascicleFolderChain As Guid?) As Guid
        Dim document As BiblosDocumentInfo = New BiblosDocumentInfo(idDocument)
        idFascicleFolderChain = AddMiscellaneaDocumentToFolder(document, idFascicleFolderChain)
        If Not MoveToFascicle.HasValue OrElse Not MoveToFascicle.Value Then
            RemoveMiscellaneaDocumentFromFolder(idDocument)
        End If
        Return idFascicleFolderChain.Value
    End Function

    Private Function AddMiscellaneaDocumentToFolder(document As DocumentInfo, idFascicleFolderChain As Guid?) As Guid
        If MiscellaneaLocation Is Nothing Then
            Throw New Exception("Nessun location presente per gli inserti di Fascicolo. Errore di configurazione, contattatare Assistenza.")
        End If

        Dim storedBiblosDocumentInfo As BiblosDocumentInfo = document.ArchiveInBiblos(MiscellaneaLocation.ProtBiblosDSDB, idFascicleFolderChain)
        Return storedBiblosDocumentInfo.ChainId
    End Function

    Private Sub RemoveMiscellaneaDocumentFromFolder(idDocument As Guid)
        Service.DetachDocument(idDocument)
    End Sub
#End Region

End Class