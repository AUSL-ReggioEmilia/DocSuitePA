Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Extensions
Imports Newtonsoft.Json
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports System.IO
Imports System.Web
Imports System.Collections.Specialized
Imports VecompSoftware.Helpers
Imports System.Text

Partial Public Class TbltTemplateDocumentRepositoryGes
    Inherits CommonBasePage

#Region " Fields "
    Private Const PAGE_TITLE As String = "Gestione Deposito Documentale"
    Public Const INSERT_ACTION As String = "Insert"
    Public Const EDIT_ACTION As String = "Edit"
    Private Const CONFIRM_CALLBACK As String = "tbltTemplateDocumentRepositoryGes.confirmCallback('{0}', {1})"
    Private Const ERROR_CALLBACK As String = "tbltTemplateDocumentRepositoryGes.errorCallback()"
    Private Const RESET_CONTROLS As String = "tbltTemplateDocumentRepositoryGes.resetControls()"
    Private _currentTemplateDocumentRepositoryLocation As Location
    Private _templateDocument As BiblosDocumentInfo

#End Region

#Region " Properties "

    Protected ReadOnly Property CurrentTemplateDocumentRepositoryLocation As Location
        Get
            If _currentTemplateDocumentRepositoryLocation Is Nothing Then
                _currentTemplateDocumentRepositoryLocation = Facade.LocationFacade.GetById(ProtocolEnv.TemplateDocumentRepositoryLocation)
            End If
            Return _currentTemplateDocumentRepositoryLocation
        End Get
    End Property

    Protected ReadOnly Property TemplateId As Guid?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Guid?)("TemplateId", Nothing)
        End Get
    End Property

    Protected ReadOnly Property TemplateIdArchiveChain As Guid?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Guid?)("TemplateIdArchiveChain", Nothing)
        End Get
    End Property

    Protected ReadOnly Property CurrentTemplateDocument As BiblosDocumentInfo
        Get
            If _templateDocument Is Nothing AndAlso TemplateIdArchiveChain.HasValue Then
                Dim document As BiblosDocumentInfo = BiblosDocumentInfo.GetDocumentsLatestVersion(CurrentTemplateDocumentRepositoryLocation.DocumentServer, TemplateIdArchiveChain.Value).FirstOrDefault()
                If document Is Nothing Then
                    Throw New DocSuiteException(String.Concat("Nessun documento trovato per idchain ", TemplateIdArchiveChain.Value))
                End If

                _templateDocument = document
            End If
            Return _templateDocument
        End Get
    End Property
#End Region

#Region " Events "
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()

        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub btnConfirm_OnClick(sender As Object, e As EventArgs) Handles btnConfirm.Click, btnPublish.Click
        Try
            Dim senderControl As RadButton = DirectCast(sender, RadButton)

            If uscUploadDocument.DocumentsCount = 0 Then
                AjaxAlert("Nessun documento presente. Inserire almeno un documento per proseguire")
                AjaxManager.ResponseScripts.Add(ERROR_CALLBACK)
                Exit Sub
            End If

            Dim chainId As Guid = If(Action.Eq(INSERT_ACTION), Guid.Empty, TemplateIdArchiveChain.Value)
            If uscUploadDocument.DocumentInfosAdded.Count > 0 Then
                Dim addedDocument As DocumentInfo = uscUploadDocument.DocumentInfosAdded.First()
                Dim biblosFunc As Func(Of DocumentInfo, BiblosDocumentInfo) = If(Action.Eq(INSERT_ACTION), (Function(d As DocumentInfo) SaveBiblosDocument(d)), (Function(d As DocumentInfo) UpdateBiblosDocument(d)))
                Dim savedTemplate As BiblosDocumentInfo = biblosFunc(addedDocument)
                chainId = savedTemplate.ChainId
            End If

            AjaxManager.ResponseScripts.Add(String.Format(CONFIRM_CALLBACK, chainId, JsonConvert.SerializeObject(senderControl.ID.Eq(btnPublish.ID))))
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore durante il salvataggio del documento su Biblos", ex)
            AjaxAlert("Errore durante il salvataggio del documento su Biblos")
            AjaxManager.ResponseScripts.Add(ERROR_CALLBACK)
        Finally
            ResetControls()
        End Try
    End Sub
#End Region

#Region " Methods "
    Private Sub Initialize()

        If CurrentTemplateDocumentRepositoryLocation Is Nothing Then
            FileLogger.Warn(LoggerName, "Nessuna Location definita per il deposito documentale.")
            Throw New DocSuiteException(String.Format("Nessuna location definita con ID {0}.", ProtocolEnv.TemplateDocumentRepositoryLocation))
            Exit Sub
        End If

        Page.Title = PAGE_TITLE

        If String.IsNullOrEmpty(Action) OrElse (Not Action.Eq(INSERT_ACTION) AndAlso Not Action.Eq(EDIT_ACTION)) Then
            Throw New DocSuiteException("Action type non corretto")
        End If

        If Action.Eq(EDIT_ACTION) Then
            uscUploadDocument.LoadDocumentInfo(CurrentTemplateDocument, False, True, False, False, False)
        End If
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirm, btnConfirm)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnPublish, btnPublish)
    End Sub

    Private Function UpdateBiblosDocument(document As DocumentInfo) As BiblosDocumentInfo
        Try
            Dim toUpdate As BiblosDocumentInfo = BiblosDocumentInfo.GetDocumentsLatestVersion(CurrentTemplateDocumentRepositoryLocation.DocumentServer, TemplateIdArchiveChain.Value).FirstOrDefault()
            toUpdate.Name = document.Name
            toUpdate.Stream = document.Stream
            toUpdate.ClearForCopy()
            Dim newDocument As BiblosDocumentInfo = Service.UpdateDocument(toUpdate, DocSuiteContext.Current.User.FullUserName)
            Return newDocument
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            Throw ex
        End Try
    End Function

    Private Function SaveBiblosDocument(document As DocumentInfo) As BiblosDocumentInfo
        If CurrentTemplateDocumentRepositoryLocation Is Nothing Then
            Throw New DocSuiteException(String.Format("Nessuna location definita con ID {0}.", ProtocolEnv.TemplateDocumentRepositoryLocation))
        End If

        document.Signature = String.Empty
        Dim storedBiblosDocumentInfo As BiblosDocumentInfo = document.ArchiveInBiblos(CurrentTemplateDocumentRepositoryLocation.DocumentServer, CurrentTemplateDocumentRepositoryLocation.ProtBiblosDSDB)
        Return storedBiblosDocumentInfo
    End Function

    Private Sub ResetControls()
        AjaxManager.ResponseScripts.Add(RESET_CONTROLS)
    End Sub
#End Region

End Class