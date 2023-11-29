Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Entity.Fascicles
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Partial Public Class FascVisualizza
    Inherits FascBasePage
    Implements ISendMail

#Region " Fields "

    Private Const FASCICLE_INITIALIZE_CALLBACK As String = "fascVisualizza.initializeCallback({0});"
    Private Const FASCICLE_REFRESH_UD_REQUEST As String = "fascVisualizza.sendRefreshUDRequest(null,null,true);"
    Private Const FASCICLE_REFRESH_UD_REQUEST_NO_UPDATE As String = "fascVisualizza.sendRefreshUDRequest(null,null,false);"
    Private Const FASCICLE_OPEN_SIGN_WINDOW As String = "fascVisualizza.openSignWindow('{0}');"
    Private Const SIGNED_DOCUMENT As String = "Signed"
    Private Const INITIALIZE_SIGN_DOCUMENT As String = "InitializeSignDocument"
    Private Const DELETE_MISCELLANEA_DOCUMENT As String = "Delete_Miscellanea_Document"
    Private Const DELETE_FASCICLEDOCUMENT As String = "Eliminazione inserto {0} n. {1} da fascicolo n. {2}"

    Private _fascMiscellaneaLocation As Location = Nothing
    Private _fascicleLogFacade As WebAPI.Fascicles.FascicleLogFacade


#End Region

#Region " Properties "

    Protected ReadOnly Property SignalRAddress As String
        Get
            Return DocSuiteContext.Current.CurrentTenant.SignalRAddress
        End Get
    End Property

    Protected ReadOnly Property IdWorkflowActivity As String
        Get
            Dim _idWorkflowActivity As String = Request.QueryString.GetValueOrDefault("IdWorkflowActivity", String.Empty)
            Return _idWorkflowActivity
        End Get
    End Property

    Protected ReadOnly Property WorkflowEnabled As Boolean
        Get
            Return DocSuiteContext.Current.ProtocolEnv.WorkflowManagerEnabled
        End Get
    End Property

    Protected ReadOnly Property ButtonLogVisible As Boolean
        Get
            Return CommonShared.HasGroupAdministratorRight() OrElse If(String.IsNullOrEmpty(ProtocolEnv.EnvGroupLogView), False, CommonShared.HasGroupLogViewRight())
        End Get
    End Property

    Public ReadOnly Property FascMiscellaneaLocation() As Location
        Get
            If _fascMiscellaneaLocation Is Nothing Then
                _fascMiscellaneaLocation = Facade.LocationFacade.GetById(ProtocolEnv.FascicleMiscellaneaLocation)
            End If
            Return _fascMiscellaneaLocation
        End Get
    End Property
    Public ReadOnly Property SenderDescription() As String Implements ISendMail.SenderDescription
        Get
            Return CommonInstance.UserDescription
        End Get
    End Property

    Public ReadOnly Property SenderEmail() As String Implements ISendMail.SenderEmail
        Get
            Return CommonInstance.UserMail
        End Get
    End Property

    Public ReadOnly Property Recipients() As IList(Of ContactDTO) Implements ISendMail.Recipients
        Get
            Return RoleFacade.CopyGetValidContacts(CurrentFascicleWebAPI.FascicleRoles.Select(Function(x) x.Role).ToList())
        End Get
    End Property

    Public ReadOnly Property Documents() As IList(Of DocumentInfo) Implements ISendMail.Documents
        Get
            Return Nothing
        End Get
    End Property
    Public ReadOnly Property Subject() As String Implements ISendMail.Subject
        Get
            Return MailFacade.GetFascicleSubject(CurrentFascicleWebAPI)
        End Get
    End Property

    Public ReadOnly Property Body() As String Implements ISendMail.Body
        Get
            Return MailFacade.GetFascicleBody(CurrentFascicleWebAPI)
        End Get
    End Property
    Public ReadOnly Property FascicleLogFacade As WebAPI.Fascicles.FascicleLogFacade
        Get
            If _fascicleLogFacade Is Nothing Then
                _fascicleLogFacade = New WebAPI.Fascicles.FascicleLogFacade(DocSuiteContext.Current.Tenants, Nothing)
            End If
            Return _fascicleLogFacade
        End Get
    End Property

    Public ReadOnly Property HasDgrooveSigner As Boolean
        Get
            Return DocSuiteContext.Current.HasDgrooveSigner
        End Get
    End Property

#End Region

#Region " Events "
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjaxSettings()
        uscFascicolo.UscDocumentReference.ReferenceUniqueId = IdFascicle.ToString()
        If btnSendToRoles.Visible Then
            btnSendToRoles.PostBackUrl = String.Format("~/MailSenders/FascicleMailSender.aspx?Type=Fasc&IdFascicle={0}&SendToRoles=true", IdFascicle)
        End If
        If Not IsPostBack Then
            If Not IdFascicle = Guid.Empty Then
                uscFascicolo.CurrentFascicleId = IdFascicle
            End If
            If Not String.IsNullOrEmpty(IdWorkflowActivity) Then
                uscFascicolo.CurrentWorkflowActivityId = IdWorkflowActivity
            End If
        End If
    End Sub

    Protected Sub FascVisualizzaAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore pagina sommario di fascicolo: " & ex.Message, ex)
            Return
        End Try

        If ajaxModel Is Nothing Then
            FileLogger.Warn(LoggerName, "Errore pagina sommario di fascicolo")
            Return
        End If

        Select Case ajaxModel.ActionName
            Case DELETE_MISCELLANEA_DOCUMENT
                If ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 Then
                    'Key - value pair where the Key is the id of the document and the Value is the file name
                    Dim documentsToDelete As IDictionary(Of Guid, String) = JsonConvert.DeserializeObject(Of IDictionary(Of Guid, String))(ajaxModel.Value(0))
                    DeleteDocuments(documentsToDelete)
                End If
                AjaxManager.ResponseScripts.Add(FASCICLE_REFRESH_UD_REQUEST_NO_UPDATE)
            Case INITIALIZE_SIGN_DOCUMENT
                Dim documentId As Guid = Guid.Empty
                If String.IsNullOrEmpty(ajaxModel.Value(0)) OrElse Not Guid.TryParse(ajaxModel.Value(0), documentId) Then
                    AjaxManager.ResponseScripts.Add(FASCICLE_REFRESH_UD_REQUEST_NO_UPDATE)
                    Return
                End If
                Dim documents As ICollection(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocumentInfo(documentId, Nothing, True)
                Dim items As NameValueCollection = documents.FirstOrDefault().ToQueryString()
                AjaxManager.ResponseScripts.Add(String.Format(FASCICLE_OPEN_SIGN_WINDOW, items.AsEncodedQueryString()))
            Case SIGNED_DOCUMENT
                If String.IsNullOrEmpty(ajaxModel.Value(0)) OrElse String.IsNullOrEmpty(ajaxModel.Value(1)) Then
                    AjaxManager.ResponseScripts.Add(FASCICLE_REFRESH_UD_REQUEST_NO_UPDATE)
                    Return
                End If

                Dim signedFile As TempFileDocumentInfo = CType(DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(ajaxModel.Value(0))), TempFileDocumentInfo)
                Dim originalDocumentSerialized As NameValueCollection = HttpUtility.ParseQueryString(ajaxModel.Value(1))
                UpdateSignedDocument(originalDocumentSerialized, signedFile)
                AjaxManager.ResponseScripts.Add(FASCICLE_REFRESH_UD_REQUEST)
        End Select
    End Sub
#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AddHandler AjaxManager.AjaxRequest, AddressOf FascVisualizzaAjaxRequest
    End Sub

    Private Sub DeleteDocuments(documentsToDelete As IDictionary(Of Guid, String))
        Dim errorCounter As Integer = 0
        For Each documentInfoPair As KeyValuePair(Of Guid, String) In documentsToDelete
            Dim idDocument As Guid = documentInfoPair.Key
            Dim documentName As String = documentInfoPair.Value

            FileLogger.Debug(LoggerName, String.Format("FascVisualizza_AjaxRequest -> IdFascicle {0} - Delete document with Id: {1}", IdFascicle, idDocument))
            Try
                FascicleLogFacade.InsertFascicleDocumentLog(IdFascicle, FascicleLogType.DocumentDelete, String.Format(DELETE_FASCICLEDOCUMENT, documentName, idDocument, CurrentFascicle.Title))
                Service.DetachDocument(idDocument)
            Catch ex As Exception
                FileLogger.Warn(LoggerName, "Errore in eliminazione documento inserti: " & ex.Message, ex)
                errorCounter += 1
            End Try
        Next

        If (errorCounter > 0) Then
            AjaxAlert(String.Format("Errore in eliminazione {0} documenti inserti", errorCounter))
        End If
    End Sub

    Private Sub UpdateSignedDocument(originalDocumentSerialized As NameValueCollection, signedFile As TempFileDocumentInfo)
        If Not signedFile.FileInfo.Exists Then
            AjaxAlert($"Documento firmato non è valido, reinserire il documento. {ProtocolEnv.DefaultErrorMessage}")
            Exit Sub
        End If

        Dim originalDocument As BiblosDocumentInfo = DirectCast(DocumentInfoFactory.BuildDocumentInfo(originalDocumentSerialized), BiblosDocumentInfo)
        Dim cloned As MemoryDocumentInfo = New MemoryDocumentInfo(originalDocument.Stream, originalDocument.Name)
        Dim storedBiblosDocumentInfo As BiblosDocumentInfo = cloned.ArchiveInBiblos(originalDocument.ArchiveName, originalDocument.ChainId)

        originalDocument.Stream = signedFile.Stream
        originalDocument.IsSigned = True

        If FileHelper.MatchExtension(signedFile.FileInfo.Name, FileHelper.P7M) Then
            originalDocument.Name = $"{originalDocument.Name}{FileHelper.P7M}"
        End If

        FileLogger.Info(LoggerName, $"[SIGN] fileName: {originalDocument.Name} idDocument: {originalDocument.DocumentId}")
        originalDocument.Update(DocSuiteContext.Current.User.FullUserName)
    End Sub
#End Region

End Class

