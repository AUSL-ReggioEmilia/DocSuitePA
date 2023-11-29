Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging
Imports ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents

Public Class UscMiscellanea
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Private Const LOAD_DOCUMENTS As String = "LoadDocuments"
    Private Const INITIALIZE_SIGN_DOCUMENT As String = "InitializeSignDocument"
    Private Const SIGNED_DOCUMENT As String = "Signed"
    Private Const BIND_MISCELLANEA As String = "uscMiscellanea.bindMiscellanea({0});"
    Private Const OPEN_SIGN_WINDOW As String = "uscMiscellanea.openSignWindow('{0}');"
    Private Const INITIALIZE_CALLBACK As String = "uscMiscellanea.initializeCallback();"
#End Region

#Region " Properties "

    Public Property OnlySignEnabled As Boolean

    Public Property Environment As DSWEnvironment

    Public Property DocumentUnitId As Guid?

    Public Property FilterByArchiveDocumentId As Guid?

    Public ReadOnly Property PageContentDiv As Control
        Get
            Return pageContent
        End Get
    End Property

    Public ReadOnly Property HasDgrooveSigner As Boolean
        Get
            Return DocSuiteContext.Current.HasDgrooveSigner
        End Get
    End Property
#End Region

#Region " Events"

    Protected Sub uscMiscellanea_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            If OnlySignEnabled Then

            End If
            miscellaneaGrid.DataSource = New List(Of String)
            miscellaneaGrid.DataBind()
        End If
    End Sub

    Protected Sub UscMiscellaneaAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore in caricamento miscellanea: " & ex.Message, ex)
            Return
        End Try

        Dim documentInfoList As IList(Of BiblosDocumentInfo) = New List(Of BiblosDocumentInfo)
        Dim location As Integer
        Dim idArchiveChain As Guid

        If ajaxModel Is Nothing OrElse ajaxModel.Value Is Nothing Then
            AjaxManager.ResponseScripts.Add(INITIALIZE_CALLBACK)
            Return
        End If

        Select Case ajaxModel.ActionName
            Case LOAD_DOCUMENTS
                If Not Guid.TryParse(ajaxModel.Value(0), idArchiveChain) OrElse Not Integer.TryParse(ajaxModel.Value(1), location) Then
                    AjaxManager.ResponseScripts.Add(INITIALIZE_CALLBACK)
                    Return
                End If
                LoadDocuments(location, idArchiveChain)
            Case INITIALIZE_SIGN_DOCUMENT
                Dim documentId As Guid = Guid.Empty
                If String.IsNullOrEmpty(ajaxModel.Value(0)) OrElse Not Guid.TryParse(ajaxModel.Value(0), documentId) Then
                    AjaxManager.ResponseScripts.Add(INITIALIZE_CALLBACK)
                    Return
                End If
                Dim documents As ICollection(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocumentInfo(documentId, Nothing, True)
                Dim items As NameValueCollection = documents.FirstOrDefault().ToQueryString()
                AjaxManager.ResponseScripts.Add(String.Format(OPEN_SIGN_WINDOW, items.AsEncodedQueryString()))

            Case SIGNED_DOCUMENT
                If String.IsNullOrEmpty(ajaxModel.Value(0)) OrElse String.IsNullOrEmpty(ajaxModel.Value(1)) Then
                    AjaxManager.ResponseScripts.Add(INITIALIZE_CALLBACK)
                    Return
                End If

                Dim signedFile As TempFileDocumentInfo = CType(DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(ajaxModel.Value(0))), TempFileDocumentInfo)
                Dim originalDocumentSerialized As NameValueCollection = HttpUtility.ParseQueryString(ajaxModel.Value(1))
                UpdateSignedDocument(location, originalDocumentSerialized, signedFile)
        End Select

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf UscMiscellaneaAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(miscellaneaGrid, miscellaneaGrid, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub LoadDocuments(locationId As Integer, idArchiveChain As Guid)
        Dim documents As ICollection(Of BiblosDocumentInfo) = New List(Of BiblosDocumentInfo)()
        Dim location As Location = Facade.LocationFacade.GetById(locationId)
        If location IsNot Nothing Then
            If FilterByArchiveDocumentId.HasValue Then
                documents = BiblosDocumentInfo.GetDocumentInfo(FilterByArchiveDocumentId.Value, Nothing, True)
            Else
                documents = BiblosDocumentInfo.GetDocuments(idArchiveChain)
            End If

            Dim noteAttribute As KeyValuePair(Of String, String)
            Dim documentModels As List(Of MiscellaneaDocumentModel) = New List(Of MiscellaneaDocumentModel)
            Dim mappedDoc As MiscellaneaDocumentModel
            Dim registrationUserAttribute As KeyValuePair(Of String, String)
            Dim user As AccountModel = Nothing
            For Each doc As BiblosDocumentInfo In documents.OrderByDescending(Function(f) f.DateCreated.Value)
                mappedDoc = New MiscellaneaDocumentModel With {
                    .IdChain = doc.DocumentParentId.Value,
                    .IdDocument = doc.DocumentId,
                    .Name = doc.Name,
                    .LocationId = locationId,
                    .Version = Convert.ToInt32(doc.Version),
                    .Status = MiscellaneaDocumentStatus.Undefined,
                    .IsSigned = False,
                    .EditEnabled = Not OnlySignEnabled,
                    .Note = String.Empty
                }
                noteAttribute = doc.Attributes.SingleOrDefault(Function(f) f.Key.Equals(BiblosFacade.NOTE_ATTRIBUTE))
                If Not String.IsNullOrEmpty(noteAttribute.Value) Then
                    mappedDoc.Note = noteAttribute.Value
                End If

                mappedDoc.RegistrationUser = String.Empty
                registrationUserAttribute = doc.Attributes.SingleOrDefault(Function(f) f.Key.Equals(BiblosFacade.REGISTRATION_USER_ATTRIBUTE))
                If Not String.IsNullOrEmpty(registrationUserAttribute.Value) Then
                    mappedDoc.RegistrationUser = registrationUserAttribute.Value
                    user = CommonAD.GetAccount(registrationUserAttribute.Value)
                    If user IsNot Nothing Then
                        mappedDoc.RegistrationUser = user.DisplayName
                    End If
                End If

                If doc.DateCreated.HasValue Then
                    mappedDoc.RegistrationDate = doc.DateCreated.Value
                End If
                documentModels.Add(mappedDoc)
            Next
            AjaxManager.ResponseScripts.Add(String.Format(BIND_MISCELLANEA, JsonConvert.SerializeObject(documentModels)))
        End If
    End Sub

    Private Sub UpdateSignedDocument(locationId As Integer, originalDocumentSerialized As NameValueCollection, signedFile As TempFileDocumentInfo)
        If Not signedFile.FileInfo.Exists Then
            BasePage.AjaxAlert(String.Format("Documento firmato non è valido, reinserire il documento. {0}", ProtocolEnv.DefaultErrorMessage), False)
            Exit Sub
        End If

        Dim originalDocument As BiblosDocumentInfo = DirectCast(DocumentInfoFactory.BuildDocumentInfo(originalDocumentSerialized), BiblosDocumentInfo)
        originalDocument.Stream = signedFile.Stream
        originalDocument.IsSigned = True

        If FileHelper.MatchExtension(signedFile.FileInfo.Name, FileHelper.P7M) Then
            originalDocument.Name = $"{originalDocument.Name}{FileHelper.P7M}"
        End If

        FileLogger.Info(LoggerName, $"[SIGN] fileName: {originalDocument.Name} idDocument: {originalDocument.DocumentId}")
        originalDocument.Update(DocSuiteContext.Current.User.FullUserName)
        LoadDocuments(locationId, originalDocument.DocumentParentId.Value)
    End Sub
#End Region

End Class