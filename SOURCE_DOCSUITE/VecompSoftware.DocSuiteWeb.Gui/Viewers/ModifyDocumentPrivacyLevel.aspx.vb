Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Public Class ModifyDocumentPrivacyLevel
    Inherits CommonBasePage

#Region " Fields "
    Private _documentsModel As String
    Private _currentLocationId As Integer?
    Private _currentUserPrivacyLevel As Integer?
    Private _documentUnitFinder As DocumentUnitFinder
    Private _currentDocumentUnit As Entity.DocumentUnits.DocumentUnit
    Private _currentDocumentUnitId As Guid?
    Private _currentLocation As Location
    Private _uSCSDocuments As IList(Of uscDocumentUpload)
    Private _isCurrentUserPrivacyRoleManager As Boolean?
    Private Const PRIVACY_MODIFY_MESSAGE As String = "Associato livello {0} {1} al documento {2} [{3}]"
#End Region

#Region " Properties "

    Private ReadOnly Property DocumentsModel As String
        Get
            If String.IsNullOrEmpty(_documentsModel) Then
                _documentsModel = HttpContext.Current.Request.QueryString.GetValueOrDefault("DocumentsModel", String.Empty)
            End If
            Return _documentsModel
        End Get
    End Property


    Private ReadOnly Property CurrentDocumentUnitId As Guid
        Get
            If _currentDocumentUnitId Is Nothing OrElse _currentDocumentUnitId.Equals(Guid.Empty) Then
                _currentDocumentUnitId = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Guid)("UDId", Guid.Empty)
            End If
            Return _currentDocumentUnitId.Value
        End Get
    End Property


    Private ReadOnly Property CurrentLocationId As Integer
        Get
            If _currentLocationId Is Nothing Then
                _currentLocationId = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Integer)("IdLocation", -1)
            End If
            Return _currentLocationId.Value
        End Get
    End Property

    Private ReadOnly Property CurrentLocation As Location
        Get
            If _currentLocation Is Nothing Then
                _currentLocation = Facade.LocationFacade.GetById(CurrentLocationId)
            End If
            Return _currentLocation
        End Get
    End Property


    Private ReadOnly Property CurrentDocumentUnit As Entity.DocumentUnits.DocumentUnit
        Get
            If _currentDocumentUnit Is Nothing Then
                _currentDocumentUnit = WebAPIImpersonatorFacade.ImpersonateFinder(DocumentUnitFinder,
                            Function(impersonationType, finder)
                                finder.ResetDecoration()
                                finder.IdDocumentUnit = CurrentDocumentUnitId
                                finder.EnablePaging = False
                                finder.ExpandContainer = True
                                finder.ExpandChains = True
                                finder.ExpandRoles = True
                                Return finder.DoSearch().Select(Function(f) f.Entity).SingleOrDefault()
                            End Function)
            End If
            Return _currentDocumentUnit
        End Get
    End Property

    Private ReadOnly Property DocumentUnitFinder As DocumentUnitFinder
        Get
            If _documentUnitFinder Is Nothing Then
                _documentUnitFinder = New DocumentUnitFinder(DocSuiteContext.Current.CurrentTenant)
            End If
            Return _documentUnitFinder
        End Get
    End Property

    Private ReadOnly Property USCSDocuments As IList(Of uscDocumentUpload)
        Get
            If _uSCSDocuments Is Nothing Then
                _uSCSDocuments = New List(Of uscDocumentUpload)
                _uSCSDocuments.Add(uscDocument)
                _uSCSDocuments.Add(uscAttachments)
                _uSCSDocuments.Add(uscAnnexed)
                _uSCSDocuments.Add(uscAdopted)
                _uSCSDocuments.Add(uscProposed)
                _uSCSDocuments.Add(uscSupervisoryBoard)
                _uSCSDocuments.Add(uscUnpublishedAnnexed)
            End If
            Return _uSCSDocuments
        End Get
    End Property

    Private ReadOnly Property CurrentUserPrivacyLevel As Integer
        Get
            If _currentUserPrivacyLevel Is Nothing Then
                _currentUserPrivacyLevel = Facade.ContainerGroupFacade.GetMaxPrivacyLevel(CurrentDocumentUnit.Container.EntityShortId, DocSuiteContext.Current.User.Domain, DocSuiteContext.Current.User.UserName)
            End If
            Return _currentUserPrivacyLevel.Value
        End Get
    End Property

    Private Property NotAllowedDocuments As Integer

    Private ReadOnly Property IsCurrentUserPrivacyRoleManager As Boolean
        Get
            If Not _isCurrentUserPrivacyRoleManager.HasValue Then
                Dim roleIds As Guid() = CurrentDocumentUnit.DocumentUnitRoles.Where(Function(d) d.AuthorizationRoleType = RoleAuthorizationType.Responsible).Select(Function(r) r.UniqueIdRole).ToArray()
                _isCurrentUserPrivacyRoleManager = Facade.RoleUserFacade.IsCurrentUserPrivacyManager(roleIds)
            End If
            Return _isCurrentUserPrivacyRoleManager.Value
        End Get
    End Property

#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        InitializeAjax()

        If Not IsPostBack() Then

            If CurrentDocumentUnit Is Nothing Then
                AjaxAlert("Si è verificata una anomalia temporanea. Contattare assistenza per riallineare le strutture documentali.")
                Exit Sub
            End If

            Initialize()
            InitializeDocuments()
        End If
    End Sub

    Protected Sub btnConfirm_OnClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirm.Click

        For Each userControlDocumet As uscDocumentUpload In USCSDocuments
            For Each document As DocumentInfo In userControlDocumet.DocumentInfos
                UpdateDocument(document)
            Next
        Next

        AjaxManager.ResponseScripts.Add("CloseWindow('true');")
    End Sub
#End Region

#Region " Methods "

    Protected Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirm, pageContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirm, btnConfirm, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub

    Protected Sub Initialize()
        Title = String.Concat("Modifica livelli di ", PRIVACY_LABEL)

        Dim minLevel As Integer = 0
        Dim maxLevel As Integer? = Nothing

        If CurrentDocumentUnit.Container IsNot Nothing Then
            minLevel = CurrentDocumentUnit.Container.PrivacyLevel
            If Not ProtocolEnv.PrivacyRoleManagerCanEditEnabled OrElse Not IsCurrentUserPrivacyRoleManager Then
                maxLevel = CurrentUserPrivacyLevel
            End If
        End If

        If (Not ProtocolEnv.PrivacyRoleManagerCanEditEnabled OrElse Not IsCurrentUserPrivacyRoleManager) AndAlso maxLevel <= minLevel Then
            maxLevel = minLevel
        End If

        For Each userControlDocumet As uscDocumentUpload In USCSDocuments
            userControlDocumet.ModifiyPrivacyLevelEnabled = True
            userControlDocumet.MinPrivacyLevel = minLevel
            userControlDocumet.MaxPrivacyLevel = maxLevel
        Next
    End Sub

    Protected Sub InitializeDocuments()

        Dim Documents As List(Of ReferenceModel) = JsonConvert.DeserializeObject(Of List(Of ReferenceModel))(DocumentsModel)
        Dim mainDocuments As List(Of BiblosDocumentInfo) = New List(Of BiblosDocumentInfo)
        Dim adoptedDocuments As List(Of BiblosDocumentInfo) = New List(Of BiblosDocumentInfo)
        Dim attachments As List(Of BiblosDocumentInfo) = New List(Of BiblosDocumentInfo)
        Dim annexed As List(Of BiblosDocumentInfo) = New List(Of BiblosDocumentInfo)
        Dim unPublishedAnnexed As List(Of BiblosDocumentInfo) = New List(Of BiblosDocumentInfo)
        Dim proposedDocuments As List(Of BiblosDocumentInfo) = New List(Of BiblosDocumentInfo)
        Dim supervisoryBoardDocuments As List(Of BiblosDocumentInfo) = New List(Of BiblosDocumentInfo)

        For Each documentModel As ReferenceModel In Documents
            If BelongToChain(documentModel.IdArchiveChain, Entity.DocumentUnits.ChainType.MainChain) Then
                mainDocuments.Add(New BiblosDocumentInfo(Guid.Parse(documentModel.UniqueId)))
            End If

            If BelongToChain(documentModel.IdArchiveChain, Entity.DocumentUnits.ChainType.AttachmentsChain) Then
                attachments.Add(New BiblosDocumentInfo(Guid.Parse(documentModel.UniqueId)))
            End If

            If BelongToChain(documentModel.IdArchiveChain, Entity.DocumentUnits.ChainType.AnnexedChain) Then
                annexed.Add(New BiblosDocumentInfo(Guid.Parse(documentModel.UniqueId)))
            End If

            If BelongToChain(documentModel.IdArchiveChain, Entity.DocumentUnits.ChainType.AssumedProposalChain) Then
                adoptedDocuments.Add(New BiblosDocumentInfo(Guid.Parse(documentModel.UniqueId)))
            End If

            If BelongToChain(documentModel.IdArchiveChain, Entity.DocumentUnits.ChainType.UnpublishedAnnexedChain) Then
                unPublishedAnnexed.Add(New BiblosDocumentInfo(Guid.Parse(documentModel.UniqueId)))
            End If

            If BelongToChain(documentModel.IdArchiveChain, Entity.DocumentUnits.ChainType.ProposalChain) Then
                proposedDocuments.Add(New BiblosDocumentInfo(Guid.Parse(documentModel.UniqueId)))
            End If

            If BelongToChain(documentModel.IdArchiveChain, Entity.DocumentUnits.ChainType.SupervisoryBoardChain) Then
                supervisoryBoardDocuments.Add(New BiblosDocumentInfo(Guid.Parse(documentModel.UniqueId)))
            End If
        Next

        NotAllowedDocuments = 0
        LoadDocuments(uscDocument, mainDocuments, mainDocumentDiv)
        LoadDocuments(uscAttachments, attachments, attachmentsDiv)
        LoadDocuments(uscAnnexed, annexed, annexedDiv)
        LoadDocuments(uscUnpublishedAnnexed, unPublishedAnnexed, unpublishedAnnexedDiv)
        LoadDocuments(uscAdopted, adoptedDocuments, adoptedDocumentDiv)
        LoadDocuments(uscProposed, proposedDocuments, proposedDocumentDiv)
        LoadDocuments(uscSupervisoryBoard, supervisoryBoardDocuments, supervisoryBoardDiv)

        If NotAllowedDocuments > 0 Then
            AjaxAlert(String.Concat("Sono stati nascosti alcuni documenti poiché possiedono un livello di ", PRIVACY_LABEL, " troppo alto per l'utente"))
        End If
    End Sub

    Private Sub UpdateDocument(doc As DocumentInfo)
        Try
            Dim chain As BiblosChainInfo = New BiblosChainInfo(DirectCast(doc, BiblosDocumentInfo).ChainId)
            If Not doc.Attributes.SequenceEqual(chain.Attributes) Then
                If chain.Attributes.Count > 0 Then
                    Dim toUpdateAttributes As Dictionary(Of String, String) = chain.Attributes.Where(Function(x) Not x.Key.Eq(BiblosFacade.FILENAME_ATTRIBUTE) _
                                                                                                                 OrElse Not x.Key.Eq(BiblosFacade.SIGNATURE_ATTRIBUTE) _
                                                                                                                 OrElse Not x.Key.Eq(BiblosFacade.DOCUMENT_POSITION_ATTRIBUTE)) _
                                                                                                                 .ToDictionary(Function(x) x.Key, Function(x) x.Value)

                    doc.AddAttributes(toUpdateAttributes)
                End If
            End If
            Service.UpdateDocument(doc, DocSuiteContext.Current.User.FullUserName)
            LogUpdate(doc)
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            AjaxAlert("Si sono verificati degli errori nel salvataggio dei documenti in Biblos, contattare assistenza.")
        End Try
    End Sub

    Private Function BelongToChain(idChain As String, type As Entity.DocumentUnits.ChainType) As Boolean
        Return Guid.Parse(idChain) = CurrentDocumentUnit.DocumentUnitChains.Where(Function(y) y.ChainType = type).Select(Function(x) x.IdArchiveChain).FirstOrDefault()
    End Function

    Private Sub LoadDocuments(uscDoument As uscDocumentUpload, documents As List(Of BiblosDocumentInfo), wrapper As HtmlGenericControl)
        If documents.Count() > 0 Then

            Dim loadedDocuments As Integer = 0
            For Each doc As BiblosDocumentInfo In documents
                If doc.Attributes.Any(Function(x) x.Key = BiblosFacade.PRIVACYLEVEL_ATTRIBUTE) Then
                    Dim privacyLevel As Integer = Integer.Parse(doc.Attributes.Where(Function(y) y.Key = BiblosFacade.PRIVACYLEVEL_ATTRIBUTE).Select(Function(x) x.Value).First())
                    If CurrentUserPrivacyLevel >= privacyLevel OrElse (ProtocolEnv.PrivacyRoleManagerCanEditEnabled AndAlso IsCurrentUserPrivacyRoleManager) Then
                        uscDoument.LoadDocumentInfo(doc, False, True, True, False, False)
                        loadedDocuments += 1
                    End If
                Else
                    uscDoument.LoadDocumentInfo(doc, False, True, True, False, False)
                    loadedDocuments += 1
                End If
            Next
            If loadedDocuments > 0 Then
                wrapper.Visible = True
            End If
            If loadedDocuments < documents.Count() Then
                NotAllowedDocuments += 1
            End If
        End If
    End Sub

    Private Sub LogUpdate(doc As BiblosDocumentInfo)
        Select Case CurrentDocumentUnit.Environment
            Case DSWEnvironment.Protocol
                Facade.ProtocolLogFacade.Insert(CurrentDocumentUnit.Year, CurrentDocumentUnit.Number, ProtocolLogEvent.LP.ToString(), String.Format(PRIVACY_MODIFY_MESSAGE, PRIVACY_LABEL, doc.Attributes.Item("PrivacyLevel"), doc.Name, doc.DocumentId))
            Case DSWEnvironment.Resolution
                Facade.ResolutionLogFacade.Insert(CurrentDocumentUnit.EntityId, ResolutionLogType.LP, String.Format(PRIVACY_MODIFY_MESSAGE, PRIVACY_LABEL, doc.Attributes.Item("PrivacyLevel"), doc.Name, doc.DocumentId))
            Case DSWEnvironment.DocumentSeries
                Facade.DocumentSeriesItemLogFacade.AddLog(CurrentDocumentUnit.EntityId, DocumentSeriesItemLogType.LP, String.Format(PRIVACY_MODIFY_MESSAGE, PRIVACY_LABEL, doc.Attributes.Item("PrivacyLevel"), doc.Name, doc.DocumentId))
        End Select
    End Sub
#End Region

End Class