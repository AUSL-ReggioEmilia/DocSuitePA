Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models

Namespace Viewers
    Public Class ResolutionViewer
        Inherits ReslBasePage
        Implements ISendMail

#Region " Fields "

        Private _resolutionKeys As List(Of Integer) = Nothing
        Private _multipleResolutions As Boolean?
        Private _resolutionRightsList As IList(Of ResolutionRights)
        Private _resolutions As List(Of Resolution) = Nothing

        Private _previousIncremental As String
        Private _incremental As Short?
        Private _documents As Boolean?
        Private _attachments As Boolean?
        Private _attachmentsfromstep As Boolean?
        Private _annexes As Boolean?
        Private _documentsomissis As Boolean?
        Private _documentsomissisfromstep As Boolean?
        Private _attachmentsomissis As Boolean?
        Private _attachmentsomissisfromstep As Boolean?
        Private _dematerialisation As Boolean?
        Private _metadata As Boolean?
#End Region

#Region " Properties "
        Private ReadOnly Property ResolutionKeys As List(Of Integer)
            Get
                If _resolutionKeys.IsNullOrEmpty() Then
                    If ViewState("keys") Is Nothing Then
                        _resolutionKeys = HttpContext.Current.Request.QueryString.GetValueOrDefault("keys", New List(Of Integer))
                        ViewState("keys") = _resolutionKeys
                    Else
                        _resolutionKeys = DirectCast(ViewState("keys"), List(Of Integer))
                    End If
                End If
                Return _resolutionKeys
            End Get
        End Property

        Private ReadOnly Property MultipleResolutions As Boolean
            Get
                If Not _multipleResolutions.HasValue Then
                    If ViewState("MultipleResolutions") Is Nothing Then
                        _multipleResolutions = HttpContext.Current.Request.QueryString.GetValueOrDefault("multiple", False)
                        ViewState("MultipleResolutions") = _multipleResolutions.Value
                    Else
                        _multipleResolutions = DirectCast(ViewState("MultipleResolutions"), Boolean)
                    End If
                End If
                Return _multipleResolutions.Value
            End Get
        End Property

        Private ReadOnly Property ResolutionList As List(Of Resolution)
            Get
                If Not ResolutionKeys.IsNullOrEmpty() AndAlso _resolutions Is Nothing Then
                    _resolutions = ResolutionKeys.Select(Function(k) FacadeFactory.Instance.ResolutionFacade.GetById(k)).ToList()
                End If
                If _resolutions Is Nothing Then
                    _resolutions = New List(Of Resolution)()
                End If
                Return _resolutions
            End Get
        End Property

        Private ReadOnly Property PreviousIncremental As String
            Get
                If String.IsNullOrEmpty(_previousIncremental) Then
                    If ViewState("previous") Is Nothing Then
                        _previousIncremental = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of String)("previous", "conditional")
                        ViewState("previous") = _previousIncremental
                    Else
                        _previousIncremental = ViewState("previous").ToString()
                    End If
                End If
                Return _previousIncremental
            End Get
        End Property

        Private ReadOnly Property Incremental As Short
            Get
                If Not _incremental.HasValue Then
                    If ViewState("incremental") Is Nothing Then
                        _incremental = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Short)("incremental", -1)
                        ViewState("incremental") = _incremental
                    Else
                        _incremental = CType(ViewState("incremental"), Short?)
                    End If
                End If
                Return _incremental.Value
            End Get
        End Property

        Private ReadOnly Property ShowDocuments As Boolean
            Get
                If Not _documents.HasValue Then
                    If ViewState("documents") Is Nothing Then
                        _documents = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Boolean)("documents", False)
                        ViewState("documents") = _documents.Value
                    Else
                        _documents = DirectCast(ViewState("documents"), Boolean)
                    End If
                End If
                Return _documents.Value
            End Get
        End Property

        Private ReadOnly Property ShowAttachments As Boolean
            Get
                If Not _attachments.HasValue Then
                    If ViewState("attachments") Is Nothing Then
                        _attachments = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Boolean)("attachments", False)
                        ViewState("attachments") = _attachments.Value
                    Else
                        _attachments = DirectCast(ViewState("attachments"), Boolean)
                    End If
                End If
                Return _attachments.Value
            End Get
        End Property

        Private ReadOnly Property ShowAttachmentsFromStep As Boolean
            Get
                If Not _attachmentsfromstep.HasValue Then
                    If ViewState("attachmentsfromstep") Is Nothing Then
                        _attachmentsfromstep = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Boolean)("attachmentsfromstep", False)
                        ViewState("attachmentsfromstep") = _attachmentsfromstep.Value
                    Else
                        _attachmentsfromstep = DirectCast(ViewState("attachmentsfromstep"), Boolean)
                    End If
                End If
                Return _attachmentsfromstep.Value
            End Get
        End Property

        Private ReadOnly Property ShowAnnexes As Boolean
            Get
                If Not _annexes.HasValue Then
                    If ViewState("annexes") Is Nothing Then
                        _annexes = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Boolean)("annexes", False)
                        ViewState("annexes") = _annexes.Value
                    Else
                        _annexes = DirectCast(ViewState("annexes"), Boolean)
                    End If
                End If
                Return _annexes.Value
            End Get
        End Property

        Private ReadOnly Property ShowDocumentsOmissis As Boolean
            Get
                If Not _documentsomissis.HasValue Then
                    If ViewState("documentsomissis") Is Nothing Then
                        _documentsomissis = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Boolean)("documentsomissis", False)
                        ViewState("documentsomissis") = _documentsomissis.Value
                    Else
                        _documentsomissis = DirectCast(ViewState("documentsomissis"), Boolean)
                    End If
                End If
                Return _documentsomissis.Value
            End Get
        End Property

        Private ReadOnly Property ShowDocumentsOmissisFromStep As Boolean
            Get
                If Not _documentsomissisfromstep.HasValue Then
                    If ViewState("documentsomissisfromstep") Is Nothing Then
                        _documentsomissisfromstep = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Boolean)("documentsomissisfromstep", False)
                        ViewState("documentsomissisfromstep") = _documentsomissisfromstep.Value
                    Else
                        _documentsomissisfromstep = DirectCast(ViewState("documentsomissisfromstep"), Boolean)
                    End If
                End If
                Return _documentsomissisfromstep.Value
            End Get
        End Property

        Private ReadOnly Property ShowAttachmentsOmissis As Boolean
            Get
                If Not _attachmentsomissis.HasValue Then
                    If ViewState("attachmentsomissis") Is Nothing Then
                        _attachmentsomissis = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Boolean)("attachmentsomissis", False)
                        ViewState("attachmentsomissis") = _attachmentsomissis.Value
                    Else
                        _attachmentsomissis = DirectCast(ViewState("attachmentsomissis"), Boolean)
                    End If
                End If
                Return _attachmentsomissis.Value
            End Get
        End Property

        Private ReadOnly Property ShowAttachmentsOmissisFromStep As Boolean
            Get
                If Not _attachmentsomissisfromstep.HasValue Then
                    If ViewState("attachmentsomissisfromstep") Is Nothing Then
                        _attachmentsomissisfromstep = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Boolean)("attachmentsomissisfromstep", False)
                        ViewState("attachmentsomissisfromstep") = _attachmentsomissisfromstep.Value
                    Else
                        _attachmentsomissisfromstep = DirectCast(ViewState("attachmentsomissisfromstep"), Boolean)
                    End If
                End If
                Return _attachmentsomissisfromstep.Value
            End Get
        End Property

        Private ReadOnly Property ShowDematerialisation As Boolean
            Get
                If Not _dematerialisation.HasValue Then
                    If ViewState("dematerialisation") Is Nothing Then
                        _dematerialisation = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Boolean)("dematerialisation", False)
                        ViewState("dematerialisation") = _dematerialisation.Value
                    Else
                        _dematerialisation = DirectCast(ViewState("dematerialisation"), Boolean)
                    End If
                End If
                Return _dematerialisation.Value
            End Get
        End Property

        Private ReadOnly Property ShowMetadata As Boolean
            Get
                If Not _metadata.HasValue Then
                    If ViewState("metadata") Is Nothing Then
                        _metadata = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Boolean)("metadata", False)
                        ViewState("metadata") = _metadata.Value
                    Else
                        _metadata = DirectCast(ViewState("metadata"), Boolean)
                    End If
                End If
                Return _metadata.Value
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
                Return New List(Of ContactDTO)()
            End Get
        End Property

        Public ReadOnly Property Documents() As IList(Of DocumentInfo) Implements ISendMail.Documents
            Get
                Dim list As IList(Of DocumentInfo) = ViewerLight.CheckedDocuments
                If list.IsNullOrEmpty() Then
                    list = GetFlatDocumentList()
                End If
                Return Facade.DocumentFacade.FilterAllowedDocuments(list, CurrentResolution.Container.Id, Nothing, Nothing, DSWEnvironment.Resolution, False)
            End Get
        End Property

        Public ReadOnly Property Subject() As String Implements ISendMail.Subject
            Get
                Return MailFacade.GetResolutionSubject(CurrentResolution)
            End Get
        End Property

        Public ReadOnly Property Body() As String Implements ISendMail.Body
            Get
                Return MailFacade.GetResolutionBody(CurrentResolution)
            End Get
        End Property

#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            MasterDocSuite.TitleVisible = False
            InitializeAjax()

            If HasIdResolution AndAlso CurrentResolution IsNot Nothing Then
                If Not CurrentResolution.AdoptionDate.HasValue OrElse CurrentResolution.EffectivenessDate.HasValue Then
                    ViewerLight.IdContainer = CurrentResolution.Container.Id
                End If

                If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso CurrentResolution.Container.PrivacyEnabled AndAlso CurrentResolutionRight.IsExecutive AndAlso CurrentResolution.AdoptionDate IsNot Nothing Then
                    ViewerLight.ModifyPrivacyEnabled = True
                    ViewerLight.CurrentDocumentUnitID = CurrentResolution.UniqueId
                    ViewerLight.CurrentLocationId = CurrentResolution.Location.Id
                End If

                If CurrentResolution.Number IsNot Nothing Then
                    ViewerLight.PrefixFileName = String.Concat("ATTI_", If(String.IsNullOrEmpty(CurrentResolution.InclusiveNumber), String.Concat(CurrentResolution.Year, "_", CurrentResolution.Number.Value.ToString("00000")), CurrentResolution.InclusiveNumber.Replace("/", "_")))
                End If
            End If

            If Not Page.IsPostBack Then
                btnSend.Visible = HasIdResolution AndAlso CurrentResolution IsNot Nothing
                btnSend.PostBackUrl = String.Format("{0}?Type=Resl&overridepreviouspageurl=true&FromViewer=true", btnSend.PostBackUrl)
                ViewerLight.DataSource = GetResolutionDocuments()
            End If
            ViewerLight.CheckViewableRight = True
            ViewerLight.AlwaysDocumentTreeOpen = ProtocolEnv.ViewLightAlwaysOpenPages.Contains("ResolutionViewer")
        End Sub

#End Region

#Region " Methods "
        Private Function GetResolutionDocuments() As List(Of DocumentInfo)
            Dim datasource As List(Of DocumentInfo)
            If MultipleResolutions Then
                datasource = ResolutionKeys.Select(Function(f) Facade.ResolutionFacade.GetResolutionDataSource(f, Incremental, ShowDocuments, ShowDocumentsOmissis, ShowDocumentsOmissisFromStep, ShowAttachments,
                                                                                                             ShowAttachmentsFromStep, ShowAttachmentsOmissis, ShowAttachmentsOmissisFromStep, ShowAnnexes,
                                                                                                             ShowDematerialisation, ShowMetadata, PreviousIncremental)).ToList()
            Else
                datasource = New List(Of DocumentInfo) From {Facade.ResolutionFacade.GetResolutionDataSource(IdResolution, Incremental, ShowDocuments, ShowDocumentsOmissis, ShowDocumentsOmissisFromStep, ShowAttachments,
                                                                                                             ShowAttachmentsFromStep, ShowAttachmentsOmissis, ShowAttachmentsOmissisFromStep, ShowAnnexes,
                                                                                                             ShowDematerialisation, ShowMetadata, PreviousIncremental)}
            End If
            Return datasource
        End Function

        Private Function GetFlatDocumentList() As IList(Of DocumentInfo)
            Dim incr As Short = Incremental
            If incr <= 0 Then
                incr = Facade.ResolutionWorkflowFacade.GetActiveIncremental(IdResolution, 1S)
            End If

            Dim list As New List(Of DocumentInfo)
            If ShowDocuments Then
                Dim folder As FolderInfo = Facade.ResolutionFacade.CreateDocumentFolder(CurrentResolution, incr, includeUniqueId:=True)
                If folder IsNot Nothing Then
                    list.AddRange(folder.Children)
                End If

                Dim prev As Short? = Facade.ResolutionFacade.GetPreviousIncremental(CurrentResolution, incr, PreviousIncremental)
                If prev.HasValue Then
                    folder = Facade.ResolutionFacade.CreateDocumentFolder(CurrentResolution, prev.Value, includeUniqueId:=True)
                    If folder IsNot Nothing Then
                        list.AddRange(folder.Children)
                    End If
                End If
            End If

            If ShowAttachments Then
                Dim folder As FolderInfo = Facade.ResolutionFacade.CreateAttachmentsFolder(CurrentResolution, incr, False, includeUniqueId:=True)
                If folder IsNot Nothing Then
                    list.AddRange(folder.Children)
                End If
            End If

            ' Annessi
            If ShowAnnexes Then
                Dim folder As FolderInfo = Facade.ResolutionFacade.CreateAnnexesFolder(CurrentResolution, incr, includeUniqueId:=True)
                If folder IsNot Nothing Then
                    list.AddRange(folder.Children)
                End If
            End If

            Return list
        End Function

        Protected Sub ManagerAjaxRequests(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
            Dim arguments As String() = Split(e.Argument, "|")

            ViewerLight.ReloadViewer(arguments, GetResolutionDocuments())
        End Sub

        Private Sub InitializeAjax()
            AddHandler MasterDocSuite.AjaxManager.AjaxRequest, AddressOf ManagerAjaxRequests
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        End Sub

#End Region


    End Class
End Namespace