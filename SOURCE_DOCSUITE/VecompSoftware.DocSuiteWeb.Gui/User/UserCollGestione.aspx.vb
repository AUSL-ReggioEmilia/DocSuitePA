Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Templates
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.DTO.Workflows
Imports VecompSoftware.DocSuiteWeb.Entity.Templates
Imports VecompSoftware.DocSuiteWeb.Entity.Workflows
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Collaborations
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Collaborations
Imports VecompSoftware.DocSuiteWeb.Facade.Common.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks
Imports VecompSoftware.DocSuiteWeb.Facade.Report
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI.Workflows
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports VecompSoftware.DocSuiteWeb.Model.Workflow
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Compress
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.WebAPI
Imports VecompSoftware.Helpers.Workflow
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging
Imports WorkflowActivityAction = VecompSoftware.DocSuiteWeb.Model.Workflow.WorkflowActivityAction
Imports WorkflowActivityArea = VecompSoftware.DocSuiteWeb.Model.Workflow.WorkflowActivityArea

Partial Public Class UserCollGestione
    Inherits UserBasePage
    Implements ISignMultipleDocuments
    Implements ISendMail

    Private Delegate Sub GenericSubDelegate()

    Public Enum CheckOutMode
        Download = 0
        OpenLocalFile = 1
    End Enum

#Region " Fields "

    Private Const VersioningKeyAttributeName As String = "VersioningKey"
    Private Const CheckedOutZipName As String = "CheckedOutDocuments.zip"

    Private _title2 As String
    Private _action2 As String
    Private _roleContact As Dictionary(Of Role, List(Of Contact))

    Private _sourceProtocolYear As Lazy(Of Short?)
    Private _sourceProtocolNumber As Lazy(Of Integer?)
    Private _sourceProtocol As Lazy(Of Protocol)
    Private _hasSourceProtocol As Lazy(Of Boolean)
    Private _fromDesk As Boolean?
    Private _fromWorkflow As Boolean?
    Private _fromWorkflowUI As Boolean?
    Private _collaborationUoia As Boolean?
    Private _currentDesk As Desk
    Private _hasDeskSource As Boolean?
    Private _deskSource As Desk
    Private _deskLocation As Location
    Private _collaborationUoiaSelected As List(Of Collaboration)
    Private _currentDeskCollaborationFacade As DeskCollaborationFacade
    Private Const DESK_RESUME_URL As String = "../Desks/DeskSummary.aspx?Type=Desk&Action=Modify&DeskId={0}"
    Private Const DESK_NEW_URL As String = "../Desks/DeskInsert.aspx?Type=Desk&CollaborationId={0}&ContainerId={1}"
    Private _atLeastOneDocumentSigned As Boolean? = Nothing
    Private _workflowActivity As WorkflowActivity

    Private _workflowInstance As WorkflowInstance
    Private _currentSigner As CollaborationSign
    Private _currentUDSFacade As UDSFacade
    Private Const ODATA_EQUAL_UDSID As String = "$filter=UDSId eq {0}"
    Private _currentUDS As UDSDto
    Private _currentUDSRepository As UDSRepository
    Private _currentTemplateCollaborationFinder As TemplateCollaborationFinder
    Private _currentTemplateName As String = String.Empty
    Private _documentEditEnabled As Boolean?
    Private _checkModifyRight As Boolean?
    Private _signersEditEnabled As Boolean?
    Private _documentUnitDraftEditorEnabled As Boolean?
    Private _collaborationEnv As CollaborationParameter
    Private _currentWorkflowActivityLogFacade As VecompSoftware.DocSuiteWeb.Facade.WebAPI.Workflows.WorkflowActivityLogFacade
    Private _isFromResolution As Boolean?
    Private _protocolXml As ProtocolXML
    Private _popUpDocumentNotSignedAlertEnabled As Boolean?
    Private _btnCheckoutEnabled As Boolean?
    Private _hasViewableRights As Boolean?
#Region " Messages "
    Private Const NO_DOCUMENT_TYPE_SELECTED_TOOLTIP As String = "E' necessario selezionare una tipologia di documento"
    Private Const NO_SPECIFIC_DOCUMENT_TYPE_SELECTED_TOOLTIP As String = "E' necessario selezionare una tipologia specifica di documento"
#End Region
#End Region

#Region " Properties "

    Private ReadOnly Property CurrentTemplateName As String
        Get
            If String.IsNullOrEmpty(_currentTemplateName) Then
                If ddlDocumentType.SelectedItem IsNot Nothing AndAlso Not ddlDocumentType.SelectedItem.Text.IsNullOrEmpty() Then
                    _currentTemplateName = ddlDocumentType.SelectedItem.Text
                    If ddlSpecificDocumentType.SelectedItem IsNot Nothing AndAlso Not ddlSpecificDocumentType.SelectedItem.Text.IsNullOrEmpty() Then
                        _currentTemplateName = ddlSpecificDocumentType.SelectedItem.Text
                    End If
                End If
            End If
            Return _currentTemplateName
        End Get
    End Property

    ''' <summary>
    ''' Proprietà che verifica se la collaborazione è uoia oppure no
    ''' Check da query string
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property FromCollaboratinoUoia As Boolean
        Get
            If Not _collaborationUoia.HasValue Then
                _collaborationUoia = HttpContext.Current.Request.QueryString.GetValueOrDefault("Uoia", False)
            End If
            Return _collaborationUoia.Value
        End Get
    End Property
    Private ReadOnly Property DefaultTemplateId As String
        Get
            Return HttpContext.Current.Request.QueryString.GetValueOrDefault("DefaultTemplateId", String.Empty)
        End Get
    End Property
    ''' <summary>
    ''' Proprieta  lista delle Collaborazioni selezionate per la uoia
    ''' </summary>
    ''' <returns></returns>
    Public Property CollaborationUoiaSelected As List(Of Collaboration)
        Get
            _collaborationUoiaSelected = DirectCast(Session("SessionCollaborationUoia"), List(Of Collaboration))
            Return _collaborationUoiaSelected
        End Get
        Set(value As List(Of Collaboration))
            _collaborationUoiaSelected = value
        End Set
    End Property

    Private ReadOnly Property Title2 As String
        Get
            If String.IsNullOrEmpty(_title2) Then
                If ViewState("Title2") Is Nothing Then
                    _title2 = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of String)("Title2", "")
                    ViewState("Title2") = _title2
                Else
                    _title2 = ViewState("Title2").ToString()
                End If
            End If
            Return _title2
        End Get
    End Property

    Private ReadOnly Property Action2 As String
        Get
            If String.IsNullOrEmpty(_action2) Then
                If ViewState("Action2") Is Nothing Then
                    _action2 = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of String)("Action2", "")
                    ViewState("Action2") = _action2
                Else
                    _action2 = ViewState("Action2").ToString()
                End If
            End If
            Return _action2
        End Get
    End Property

    Public ReadOnly Property DocumentsToSign() As IList(Of MultiSignDocumentInfo) Implements ISignMultipleDocuments.DocumentsToSign
        Get
            Dim list As New List(Of MultiSignDocumentInfo)
            Dim effectiveSigner As String = String.Empty
            Dim dictionary As IDictionary(Of Guid, BiblosDocumentInfo) =
                Facade.CollaborationVersioningFacade.GetLastVersionDocuments(CurrentCollaboration, VersioningDocumentGroup.MainDocument)

            Dim collaborationSigns As ICollection(Of String) = Facade.CollaborationSignsFacade.GetEffectiveSigners(CurrentCollaboration.Id).Select(Function(s) s.SignUser).ToList()
            Dim collaborationSign As CollaborationSign = CurrentCollaboration.CollaborationSigns.Where(Function(x) x.IsActive = 1S).FirstOrDefault()
            Dim listDelegations As List(Of String) = Facade.UserLogFacade.GetDelegationsSign()

            If listDelegations.Any(Function(x) x.Eq(collaborationSign.SignUser)) Then
                effectiveSigner = collaborationSign.SignUser
            End If
            If Not dictionary.IsNullOrEmpty() Then
                For Each key As Guid In dictionary.Keys
                    Dim msdi As New MultiSignDocumentInfo(dictionary(key))
                    msdi.GroupCode = CurrentCollaboration.Id.ToString()
                    msdi.Mandatory = True
                    msdi.DocType = "Doc. Principale"
                    msdi.Description = CurrentCollaboration.CollaborationObject
                    msdi.IdOwner = key.ToString().Replace("/"c, "§"c)
                    msdi.Signers = collaborationSigns
                    msdi.EffectiveSigner = effectiveSigner
                    list.Add(msdi)
                Next
            End If

            Dim loadAlsoOmissis As Boolean = CurrentCollaboration IsNot Nothing AndAlso Not String.IsNullOrEmpty(CurrentCollaboration.DocumentType) AndAlso (DocSuiteContext.Current.IsResolutionEnabled AndAlso (CurrentCollaboration.DocumentType.Eq(CollaborationDocumentType.D.ToString()) OrElse CurrentCollaboration.DocumentType.Eq(CollaborationDocumentType.A.ToString())))

            If loadAlsoOmissis AndAlso ResolutionEnv.MainDocumentOmissisEnable Then
                dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(CurrentCollaboration, VersioningDocumentGroup.MainDocumentOmissis)

                If Not dictionary.IsNullOrEmpty() Then
                    For Each key As Guid In dictionary.Keys

                        Dim msdi As New MultiSignDocumentInfo(dictionary(key))
                        msdi.GroupCode = CurrentCollaboration.Id.ToString()
                        msdi.Mandatory = ProtocolEnv.CollaborationMultiSignDocumentsDefaultFlagged
                        msdi.MandatorySelectable = True
                        msdi.Description = CurrentCollaboration.CollaborationObject
                        msdi.DocType = "Doc. Omissis"
                        msdi.IdOwner = key.ToString().Replace("/"c, "§"c)
                        msdi.Signers = collaborationSigns
                        msdi.EffectiveSigner = effectiveSigner
                        list.Add(msdi)
                    Next
                End If
            End If

            dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(CurrentCollaboration, VersioningDocumentGroup.Attachment)

            If Not dictionary.IsNullOrEmpty() Then
                For Each key As Guid In dictionary.Keys

                    Dim msdi As New MultiSignDocumentInfo(dictionary(key))
                    msdi.GroupCode = CurrentCollaboration.Id.ToString()
                    msdi.Mandatory = ProtocolEnv.CollaborationMultiSignDocumentsDefaultFlagged
                    msdi.MandatorySelectable = True
                    msdi.Description = CurrentCollaboration.CollaborationObject
                    msdi.DocType = "Allegato"
                    msdi.IdOwner = key.ToString().Replace("/"c, "§"c)
                    msdi.Signers = collaborationSigns
                    msdi.EffectiveSigner = effectiveSigner
                    list.Add(msdi)
                Next
            End If

            If loadAlsoOmissis AndAlso ResolutionEnv.AttachmentOmissisEnable Then
                dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(CurrentCollaboration, VersioningDocumentGroup.AttachmentOmissis)

                If Not dictionary.IsNullOrEmpty() Then
                    For Each key As Guid In dictionary.Keys

                        Dim msdi As New MultiSignDocumentInfo(dictionary(key))
                        msdi.GroupCode = CurrentCollaboration.Id.ToString()
                        msdi.Mandatory = ProtocolEnv.CollaborationMultiSignDocumentsDefaultFlagged
                        msdi.MandatorySelectable = True
                        msdi.Description = CurrentCollaboration.CollaborationObject
                        msdi.DocType = "Allegato Omissis"
                        msdi.IdOwner = key.ToString().Replace("/"c, "§"c)
                        msdi.Signers = collaborationSigns
                        msdi.EffectiveSigner = effectiveSigner
                        list.Add(msdi)
                    Next
                End If
            End If

            dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(CurrentCollaboration, VersioningDocumentGroup.Annexed)

            If Not dictionary.IsNullOrEmpty() Then
                For Each key As Guid In dictionary.Keys

                    Dim msdi As New MultiSignDocumentInfo(dictionary(key))
                    msdi.GroupCode = CurrentCollaboration.Id.ToString()
                    msdi.Mandatory = ProtocolEnv.CollaborationMultiSignDocumentsDefaultFlagged
                    msdi.MandatorySelectable = True
                    msdi.Description = CurrentCollaboration.CollaborationObject
                    msdi.DocType = "Annesso"
                    msdi.IdOwner = key.ToString().Replace("/"c, "§"c)
                    msdi.Signers = collaborationSigns
                    msdi.EffectiveSigner = effectiveSigner
                    list.Add(msdi)
                Next
            End If

            Return list
        End Get
    End Property

    Public ReadOnly Property ReturnUrl() As String Implements ISignMultipleDocuments.ReturnUrl
        Get
            Dim url As String = "~/User/UserCollGestione.aspx?Type={0}&Titolo=Inserimento&Action={1}&idCollaboration={2}&Action2={3}&Title2={4}"
            If IsWorkflowOperation Then
                url = String.Concat(url, String.Format("&IsWorkflowOperation=True&IdWorkflowActivity={0}", CurrentIdWorkflowActivity))
            End If
            Return String.Format(url, Type, Action, CurrentCollaboration.Id, Action2, Title2)
        End Get
    End Property
    Public ReadOnly Property SignAction As String Implements ISignMultipleDocuments.SignAction
        Get
            Return String.Format(Action)
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
            Dim list As New List(Of ContactDTO)()
            If CurrentCollaborationSign IsNot Nothing Then
                list.Add(MailFacade.CreateManualContact(CurrentCollaborationSign.SignName, CurrentCollaborationSign.SignEMail, ContactType.Aoo, True, True))
            End If
            Return list
        End Get
    End Property

    Public ReadOnly Property Documents() As IList(Of DocumentInfo) Implements ISendMail.Documents
        Get
            Return New List(Of DocumentInfo)()
        End Get
    End Property

    Public ReadOnly Property Subject() As String Implements ISendMail.Subject
        Get
            Return String.Format("{0} Richiesta Informazioni Collaborazione n. {1} del {2} - {3}",
                                 DocSuiteContext.ProductName,
                                 CurrentCollaboration.Id,
                                 CurrentCollaboration.RegistrationDate.DefaultString(),
                                 CurrentCollaboration.CollaborationObject)
        End Get
    End Property

    Public ReadOnly Property Body() As String Implements ISendMail.Body
        Get
            Dim trailingMessage As String = ""
            If Not Action.Eq(CollaborationMainAction.CancellazioneDocumento) Then
                Dim signature As String = Facade.CollaborationFacade.GenerateSignature(CurrentCollaboration, CurrentCollaboration.RegistrationDate.DateTime, CurrentCollaboration.DocumentType)
                Dim mainAction, subAction, titleStep As String
                If CurrentCollaborationSign.IdStatus.Eq(CollaborationStatusType.DP.ToString()) Then
                    mainAction = CollaborationMainAction.AlProtocolloSegreteria
                    subAction = CollaborationSubAction.AlProtocolloSegreteria
                    titleStep = "Al Protocollo/Segreteria"
                Else
                    mainAction = CollaborationMainAction.DaVisionareFirmare
                    subAction = CollaborationSubAction.DaVisionareFirmare
                    titleStep = "Da Visionare/Firmare"
                End If
                trailingMessage = String.Format("<BR><BR><a href={0}?Tipo=Coll&Azione=Apri&Identificativo={1}&Stato={2}&CollType={3}&SubAction={4}&TitleStep={5}>{6}</a>",
                                                DocSuiteContext.Current.CurrentTenant.DSWUrl, CurrentCollaboration.Id, mainAction, CollaborationFacade.GetPageTypeFromDocumentType(CurrentCollaboration.DocumentType), subAction, HttpUtility.UrlEncode(titleStep), signature)
            End If
            Return String.Format("Oggetto: <B>{0}</B><BR>Note: <B>{1}</B>{2}",
                                 StringHelper.ReplaceCrLf(HttpUtility.HtmlEncode(CurrentCollaboration.CollaborationObject)),
                                 StringHelper.ReplaceCrLf(HttpUtility.HtmlEncode(CurrentCollaboration.Note)), trailingMessage)
        End Get
    End Property

    ''' <summary> Settori caricati con i contatti che hanno portato alla loro inclusione. </summary>
    ''' <remarks> Di fatto in parte replica il controllo dei settori di restituzione </remarks>
    Private Property RoleContacts As Dictionary(Of Role, List(Of Contact))
        Get
            If _roleContact Is Nothing AndAlso ViewState("roleContacts") Is Nothing Then
                _roleContact = New Dictionary(Of Role, List(Of Contact))
                ViewState("roleContacts") = _roleContact
            ElseIf _roleContact Is Nothing AndAlso ViewState("roleContacts") IsNot Nothing Then
                _roleContact = DirectCast(ViewState("roleContacts"), Dictionary(Of Role, List(Of Contact)))
            End If
            Return _roleContact
        End Get
        Set(value As Dictionary(Of Role, List(Of Contact)))
            _roleContact = value
            ViewState("roleContacts") = _roleContact
        End Set
    End Property

    Private ReadOnly Property SourceProtocolYear As Short?
        Get
            Return _sourceProtocolYear.Value
        End Get
    End Property

    Private ReadOnly Property SourceProtocolNumber As Integer?
        Get
            Return _sourceProtocolNumber.Value
        End Get
    End Property

    Private ReadOnly Property SourceProtocol As Protocol
        Get
            Return _sourceProtocol.Value
        End Get
    End Property

    Private ReadOnly Property HasSourceProtocol As Boolean
        Get
            Return _hasSourceProtocol.Value
        End Get
    End Property

    Private ReadOnly Property InsertFromDesk As Boolean
        Get
            If Not _fromDesk.HasValue Then
                _fromDesk = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Boolean)("FromDesk", False)
            End If
            Return _fromDesk.Value
        End Get
    End Property
    Private ReadOnly Property FromWorkflowUI As Boolean
        Get
            If Not _fromWorkflowUI.HasValue Then
                _fromWorkflowUI = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Boolean)("FromWorkflowUI", False)
            End If
            Return _fromWorkflowUI.Value
        End Get
    End Property

    Private ReadOnly Property InsertFromWorkflow As Boolean
        Get
            If Not _fromWorkflow.HasValue Then
                _fromWorkflow = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Boolean)("FromWorkflow", False)
            End If
            Return _fromWorkflow.Value
        End Get
    End Property

    Private ReadOnly Property CurrentDeskFromInsert As Desk
        Get
            If _currentDesk Is Nothing Then
                Dim idDesk As Guid = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Guid)("IdDesk", Guid.Empty)
                If Not idDesk.Equals(Guid.Empty) Then
                    _currentDesk = New DeskFacade(DocSuiteContext.Current.User.FullUserName).GetById(idDesk)
                    Return _currentDesk
                End If
                Return Nothing
            End If
            Return _currentDesk
        End Get
    End Property

    Public Property CollaborationInitializerSource As CollaborationInitializer
        Get
            Return TryCast(Session("collaborationInitializerSource"), CollaborationInitializer)
        End Get
        Set(value As CollaborationInitializer)
            If value Is Nothing Then
                Session.Remove("collaborationInitializerSource")
            Else
                Session("collaborationInitializerSource") = value
            End If
        End Set
    End Property

    Public Property ProtocolXMLSource As ProtocolXML
        Get
            Return TryCast(Session("ProtocolXMLSource"), ProtocolXML)
        End Get
        Set(value As ProtocolXML)
            If value Is Nothing Then
                Session.Remove("ProtocolXMLSource")
            Else
                Session("ProtocolXMLSource") = value
            End If
        End Set
    End Property

    Public ReadOnly Property HasDeskSource As Boolean
        Get
            If Not _hasDeskSource.HasValue Then
                _hasDeskSource = False
                If DeskSource IsNot Nothing Then
                    _hasDeskSource = True
                End If
            End If
            Return _hasDeskSource.Value
        End Get
    End Property

    Public ReadOnly Property DeskSource As Desk
        Get
            If _deskSource Is Nothing AndAlso CollaborationId.HasValue Then
                _deskSource = New DeskFacade(DocSuiteContext.Current.User.FullUserName).GetByIdCollaboration(CollaborationId.Value)
            End If
            Return _deskSource
        End Get
    End Property

    Public ReadOnly Property CurrentDeskCollaborationFacade As DeskCollaborationFacade
        Get
            If _currentDeskCollaborationFacade Is Nothing Then
                _currentDeskCollaborationFacade = New DeskCollaborationFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _currentDeskCollaborationFacade
        End Get
    End Property

    Public ReadOnly Property DeskLocation As Location
        Get
            If _deskLocation IsNot Nothing Then
                _deskLocation = Facade.ContainerFacade.GetById(DeskSource.Container.Id, False, "ProtDB").DeskLocation
            End If
            Return _deskLocation
        End Get
    End Property

    Public ReadOnly Property IsSecretaryEnable As Boolean
        Get
            Dim temp As IList(Of String) = Facade.RoleUserFacade.GetAccounts(DocSuiteContext.Current.User.FullUserName)

            If temp.Any(Function(x) x.Eq(CurrentCollaboration.GetFirstCollaborationSignActive().SignUser)) Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property IsProposer As Boolean
        Get
            If CurrentCollaboration.RegistrationUser.Eq(DocSuiteContext.Current.User.FullUserName) Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property IsFirstActiveSigner As Boolean
        Get
            If CurrentCollaboration.GetFirstCollaborationSignActive().SignUser.Eq(DocSuiteContext.Current.User.FullUserName) Then
                Return True
            End If
            Return False
        End Get
    End Property

    Private ReadOnly Property NeedSetAtLeastOneDocumentSigned As Boolean
        Get
            Return Not _atLeastOneDocumentSigned.HasValue
        End Get
    End Property

    Private Property AtLeastOneDocumentSigned As Boolean
        Get
            If Not _atLeastOneDocumentSigned.HasValue Then
                Return False
            End If
            Return _atLeastOneDocumentSigned.Value
        End Get
        Set(value As Boolean)
            If value AndAlso Not _atLeastOneDocumentSigned.HasValue Then
                _atLeastOneDocumentSigned = value
            End If
        End Set
    End Property

    Protected ReadOnly Property CurrentWorkflowActivity As WorkflowActivity
        Get
            If _workflowActivity Is Nothing Then
                Dim result As WebAPIDto(Of WorkflowActivity) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentWorkflowActivityFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        If CurrentIdWorkflowActivity.IsEmpty() AndAlso CurrentCollaboration Is Nothing Then
                            Return Nothing
                        End If

                        If Not CurrentIdWorkflowActivity.IsEmpty() Then
                            finder.UniqueId = CurrentIdWorkflowActivity
                        Else
                            finder.WorkflowInstanceId = CurrentWorkflowInstance.UniqueId
                            finder.Statuses = New List(Of WorkflowStatus) From {WorkflowStatus.Todo, WorkflowStatus.Progress}
                            finder.ActivityType = Entity.Workflows.WorkflowActivityType.CollaborationSign
                        End If
                        Return finder.DoSearch().FirstOrDefault()
                    End Function)

                If result IsNot Nothing Then
                    _workflowActivity = result.Entity
                End If
            End If
            Return _workflowActivity
        End Get
    End Property

    Protected ReadOnly Property CurrentWorkflowInstance As WorkflowInstance
        Get
            If _workflowInstance Is Nothing AndAlso CurrentCollaboration.IdWorkflowInstance.HasValue Then
                Dim result As WebAPIDto(Of WorkflowInstance) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentWorkflowInstanceFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.UniqueId = CurrentCollaboration.IdWorkflowInstance.Value
                        finder.ExpandRepository = True
                        finder.ExpandProperties = False
                        Return finder.DoSearch().FirstOrDefault()
                    End Function)

                If result IsNot Nothing Then
                    _workflowInstance = result.Entity
                End If
            End If
            Return _workflowInstance
        End Get
    End Property

    Protected ReadOnly Property CurrentSigner As CollaborationSign
        Get
            If _currentSigner Is Nothing Then
                _currentSigner = Facade.CollaborationSignsFacade.SearchFull(CurrentCollaboration.Id, True).FirstOrDefault()
            End If
            Return _currentSigner
        End Get
    End Property

    Private ReadOnly Property CurrentUDSRepository As UDSRepository
        Get
            If _currentUDSRepository Is Nothing Then
                If CurrentWorkflowActivity Is Nothing Then
                    Return Nothing
                End If

                Dim udsRepositoryIdProperty As WebAPIDto(Of WorkflowProperty) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentWorkflowPropertyFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.WorkflowActivityId = CurrentWorkflowActivity.UniqueId
                        finder.Name = WorkflowPropertyHelper.DSW_FIELD_UDS_REPOSITORY_ID
                        Return finder.DoSearch().FirstOrDefault()
                    End Function)

                If udsRepositoryIdProperty IsNot Nothing AndAlso udsRepositoryIdProperty.Entity IsNot Nothing Then
                    _currentUDSRepository = CurrentUDSRepositoryFacade.GetById(udsRepositoryIdProperty.Entity.ValueGuid.Value)
                End If
            End If
            Return _currentUDSRepository
        End Get
    End Property

    Public ReadOnly Property CurrentUDS As UDSDto
        Get
            If _currentUDS Is Nothing Then
                If CurrentWorkflowActivity Is Nothing Then
                    Return Nothing
                End If

                Dim udsIdProperty As WebAPIDto(Of WorkflowProperty) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentWorkflowPropertyFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.WorkflowActivityId = CurrentWorkflowActivity.UniqueId
                        finder.Name = WorkflowPropertyHelper.DSW_FIELD_UDS_ID
                        Return finder.DoSearch().FirstOrDefault()
                    End Function)

                If udsIdProperty IsNot Nothing AndAlso udsIdProperty.Entity IsNot Nothing AndAlso CurrentUDSRepository IsNot Nothing Then
                    _currentUDS = CurrentUDSFacade.GetUDSSource(CurrentUDSRepository, String.Format(ODATA_EQUAL_UDSID, udsIdProperty.Entity.ValueGuid))
                End If
            End If
            Return _currentUDS
        End Get
    End Property

    Private ReadOnly Property CurrentUDSFacade As UDSFacade
        Get
            If _currentUDSFacade Is Nothing Then
                _currentUDSFacade = New UDSFacade()
            End If
            Return _currentUDSFacade
        End Get
    End Property

    Private ReadOnly Property CurrentTemplateCollaborationFinder As TemplateCollaborationFinder
        Get
            If _currentTemplateCollaborationFinder Is Nothing Then
                _currentTemplateCollaborationFinder = New TemplateCollaborationFinder(DocSuiteContext.Current.Tenants)
                _currentTemplateCollaborationFinder.ResetDecoration()
                _currentTemplateCollaborationFinder.EnablePaging = False
            End If
            Return _currentTemplateCollaborationFinder
        End Get
    End Property

    Private ReadOnly Property CollaborationEnv As CollaborationParameter
        Get
            If _collaborationEnv Is Nothing AndAlso CurrentCollaboration IsNot Nothing Then
                _collaborationEnv = New CollaborationParameter(CurrentCollaboration.TemplateName)
            End If
            Return _collaborationEnv
        End Get
    End Property

    Private ReadOnly Property DocumentEditEnabled As Boolean
        Get
            If Not _documentEditEnabled.HasValue Then
                _documentEditEnabled = CurrentCollaboration.CollaborationSigns.Where(Function(s) s.Incremental = 1S AndAlso s.IsActive = 1S).Count = 1 AndAlso (IsProposer OrElse IsSecretaryEnable)
            End If
            Return _documentEditEnabled.Value
        End Get
    End Property

    Private ReadOnly Property CheckModifyRight As Boolean
        Get
            If Not _checkModifyRight.HasValue Then
                _checkModifyRight = (CurrentCollaboration.CollaborationSigns.Where(Function(s) s.Incremental = 1S AndAlso s.IsActive = 1S AndAlso Not s.SignDate.HasValue AndAlso Action2.Eq(CollaborationMainAction.AllaVisioneFirma)).Count = 1 AndAlso IsProposer) OrElse
                    (CurrentCollaboration.IdStatus.Eq(CollaborationStatusType.DL.ToString()) AndAlso IsProposer)
            End If
            Return _checkModifyRight.Value
        End Get
    End Property

    Private ReadOnly Property SignersEditEnabled As Boolean
        Get
            If Not _signersEditEnabled.HasValue Then
                _signersEditEnabled = CollaborationEnv IsNot Nothing AndAlso CollaborationEnv.SignersEditEnabled AndAlso DocumentEditEnabled
            End If
            Return _signersEditEnabled.Value
        End Get
    End Property
    Private ReadOnly Property DocumentUnitDraftEditorEnabled As Boolean
        Get
            If Not _documentUnitDraftEditorEnabled.HasValue Then
                _documentUnitDraftEditorEnabled = CollaborationEnv IsNot Nothing AndAlso CollaborationEnv.DocumentUnitDraftEditorEnabled
            End If
            Return _documentUnitDraftEditorEnabled.Value
        End Get
    End Property

    Protected ReadOnly Property CurrentWorkflowActivityLogFacade As WorkflowActivityLogFacade
        Get
            If _currentWorkflowActivityLogFacade Is Nothing Then
                _currentWorkflowActivityLogFacade = New WorkflowActivityLogFacade(DocSuiteContext.Current.Tenants, CurrentTenant)
            End If
            Return _currentWorkflowActivityLogFacade
        End Get
    End Property

    Private ReadOnly Property IsFromResolution As Boolean
        Get
            If Not _isFromResolution.HasValue Then
                _isFromResolution = CheckIsFromResolution()
            End If
            Return _isFromResolution.Value
        End Get
    End Property

    Private ReadOnly Property PopUpDocumentNotSignedAlertEnabled As Boolean
        Get
            If Not _popUpDocumentNotSignedAlertEnabled.HasValue Then
                _popUpDocumentNotSignedAlertEnabled = CollaborationEnv IsNot Nothing AndAlso CollaborationEnv.PopUpDocumentNotSignedAlertEnabled
            End If
            Return _popUpDocumentNotSignedAlertEnabled.Value
        End Get
    End Property
    Private ReadOnly Property BtnCheckoutEnabled As Boolean
        Get
            If Not _btnCheckoutEnabled.HasValue Then
                _btnCheckoutEnabled = CollaborationEnv IsNot Nothing AndAlso CollaborationEnv.BtnCheckoutEnabled
            End If
            Return _btnCheckoutEnabled.Value
        End Get
    End Property
    Private ReadOnly Property HasViewableRights As Boolean
        Get
            If Not _hasViewableRights.HasValue Then
                _hasViewableRights = CurrentCollaboration IsNot Nothing AndAlso CurrentODataFacade.HasCollaborationViewableRight(CurrentCollaboration.Id)
            End If
            Return _hasViewableRights.Value
        End Get
    End Property
#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not FromCollaboratinoUoia Then
            Session("SessionCollaborationUoia") = Nothing
        End If


        Dim location As Location = Facade.LocationFacade.GetById(ProtocolEnv.CollaborationLocation)
        If location Is Nothing Then
            Throw New DocSuiteException("Errore di configurazione", String.Format("Location [{0}] mancante nel database.", ProtocolEnv.CollaborationLocation))
        ElseIf String.IsNullOrEmpty(location.ProtBiblosDSDB) Then
            Throw New DocSuiteException("Errore di configurazione", String.Format("Archivio non configuration per la Location [{0}].", ProtocolEnv.CollaborationLocation))
        End If

        'per operazioni contemporanee
        VerificaOperazioniConcorrenza()

        _sourceProtocolYear = New Lazy(Of Short?)(Function() GetSourceProtocolYear())
        _sourceProtocolNumber = New Lazy(Of Integer?)(Function() GetSourceProtocolNumber())
        _sourceProtocol = New Lazy(Of Protocol)(Function() GetSourceProtocol())
        _hasSourceProtocol = New Lazy(Of Boolean)(Function() SourceProtocol IsNot Nothing)

        Title = CType(Session("lblTitle"), String)

        InitializeAjax()

        uscVisioneFirma.TemplateName = CurrentTemplateName
        If Not IsPostBack Then

            InitializeDocumentControls()
            Dim localType As String = "Prot"
            If Not String.IsNullOrEmpty(Type()) Then
                localType = Type()
            End If
            uscVisioneFirma.CollaborationType = localType
            uscVisioneFirma.EnvironmentType = localType

            btnMail.PostBackUrl = String.Format("{0}?Type={1}", btnMail.PostBackUrl, localType)

            'Verifica se provengo da Tavoli
            InitializePageFromDesk()
            'verifico se provengo da attività
            InitializePageFromWorkflow()
            Dim sourcePage As MultipleSignBasePage = TryCast(PreviousPage, MultipleSignBasePage)
            If sourcePage IsNot Nothing Then
                Facade.CollaborationVersioningFacade.CheckSignedDocuments(sourcePage.SignedDocuments)
                If CurrentCollaboration.IdWorkflowInstance.HasValue Then
                    For Each signedDoc As MultiSignDocumentInfo In sourcePage.SignedDocuments
                        Dim currentVersioning As CollaborationVersioning = Facade.CollaborationVersioningFacade.GetById(Guid.Parse(signedDoc.IdOwner))
                        UpdateWorkflowStep(currentVersioning)
                    Next
                End If
                Response.Redirect(ReturnUrl, True)
            End If

            txtDestinatarioOK.Visible = False
            txtRestituzioniOK.Visible = False
            txtInoltroOK.Visible = False
            txtLastChanged.Visible = False

            Initialize()
            InitializeWorkflowWizard()
            InitializeSourceProtocol()

            'Tavoli
            If HasDeskSource Then
                trDeskSource.Visible = True
                deskLink.Text = DeskSource.Name
                deskLink.NavigateUrl = String.Format("~/Desks/DeskSummary.aspx?Type=Desk&Action=View&DeskId={0}", DeskSource.Id)
            End If
        End If

        AddHandler btnInoltra.Click, AddressOf BtnInoltraClick
        AddHandler btnRifiuta.Click, AddressOf BtnRifiutaClick
        If IsWorkflowOperation Then
            AddHandler MasterDocSuite.OnWorkflowConfirmed, AddressOf WorkflowConfirmed
            If Not CurrentSigner.IsRequired.GetValueOrDefault(False) Then
                RemoveHandler btnInoltra.Click, AddressOf BtnInoltraClick
                RemoveHandler btnRifiuta.Click, AddressOf BtnRifiutaClick
                AddHandler btnRifiuta.Click, AddressOf BtnRifiutaWorkflowClick
                AddHandler btnInoltra.Click, AddressOf BtnApprovaWorkflowClick
            Else
                RemoveHandler btnCancella.Click, AddressOf BtnCancellaClick
                AddHandler btnCancella.Click, AddressOf BtnRifiutaWorkflowClick
            End If
        End If

        If tEditCollaborationData.Visible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdSave, btnEditCollaborationData)
        End If

        lblDestinatari.Text = DocumentTypeCaption(ddlDocumentType.SelectedValue)

        Session("lblTitle") = Title
    End Sub

    Protected Sub UserCollGestione_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
            Select Case ajaxModel.ActionName
                Case "BindingFromWorkflowUI"
                    If ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count = 2 Then
                        Dim workflowReferenceModel As WorkflowReferenceModel = JsonConvert.DeserializeObject(Of WorkflowReferenceModel)(ajaxModel.Value(0))
                        Dim workflowStartModel As WorkflowStart = JsonConvert.DeserializeObject(Of WorkflowStart)(ajaxModel.Value(1))
                        InitializePageFromWorkflowUI(workflowReferenceModel, workflowStartModel)
                    End If
            End Select
            AjaxManager.ResponseScripts.Add("HideLoadingPanel();")
        Catch
            Dim arguments As String() = Split(e.Argument, "|")
            Select Case arguments(0)
                Case "FORWARD"
                    ExecuteForward()
                Case "VISIONEPROTOCOLLA"
                    ExecuteVisioneProtocolla()
                Case "RESTITUTION"
                    ExecuteRestitution()
                Case "ABSENTMANAGERS"
                    If (arguments(1) IsNot Nothing) Then
                        Dim deserialized As AbsentManager() = JsonConvert.DeserializeObject(Of AbsentManager())(arguments(1))
                        SetManagersAbsence(deserialized)
                    End If
                Case "DELETECOLLABORATION"
                    ForceDeleteCollaboration()
                Case "DOCUMENTUNITDRAFT"
                    SessionProtocolTemplate(arguments(1))
                    AjaxManager.ResponseScripts.Add("HideLoadingPanel();")
            End Select
        End Try
    End Sub

    Private Sub SessionProtocolTemplate(protocolXML As String)
        If (protocolXML IsNot Nothing) Then
            ProtocolXMLSource = JsonConvert.DeserializeObject(Of ProtocolXML)(protocolXML)
        End If
    End Sub

    Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
        If Not ValidateObject() OrElse Not ValidateAlertDate() Then
            Exit Sub
        End If

        If DocSuiteContext.Current.ProtocolEnv.CollaborationManagementExpired AndAlso Not CurrentCollaboration.MemorandumDate.Eq(alertDate.SelectedDate) Then
            Facade.CollaborationLogFacade.Insert(CurrentCollaboration, String.Format("Data avviso: da [{0}] a [{1}].", CurrentCollaboration.AlertDate, alertDate.SelectedDate))
            CurrentCollaboration.AlertDate = alertDate.SelectedDate
        End If

        If Not CurrentCollaboration.MemorandumDate.Eq(txtDate.SelectedDate) Then
            Facade.CollaborationLogFacade.Insert(CurrentCollaboration, String.Format("Data scadenza: da [{0}] a [{1}].", CurrentCollaboration.MemorandumDate, txtDate.SelectedDate))
            CurrentCollaboration.MemorandumDate = txtDate.SelectedDate
        End If

        If Not String.IsNullOrWhiteSpace(txtObject.Text) Then
            Facade.CollaborationLogFacade.Insert(CurrentCollaboration, String.Format("Oggetto: da [{0}] a [{1}].", CurrentCollaboration.CollaborationObject, txtObject.Text))
            CurrentCollaboration.CollaborationObject = txtObject.Text
        End If

        If Not String.IsNullOrWhiteSpace(txtNote.Text) Then
            Facade.CollaborationLogFacade.Insert(CurrentCollaboration, String.Format("Note: da [{0}] a [{1}].", CurrentCollaboration.Note, txtNote.Text))
            CurrentCollaboration.Note = txtNote.Text
        End If

        Facade.CollaborationFacade.Update(CurrentCollaboration)

        If ProtocolEnv.CollaborationVisionSignatureDataEditable AndAlso Action.Eq(CollaborationSubAction.AllaVisioneFirma) Then
            If tAlertData.Visible Then
                txtDate.DateInput.ReadOnly = True
                txtDate.DatePopupButton.Visible = False
            End If
            alertDate.DateInput.ReadOnly = True
            alertDate.DatePopupButton.Visible = False
            txtObject.Enabled = False
            txtNote.Enabled = False
            btnEditCollaborationData.Enabled = True
            cmdSave.Visible = False
        End If
    End Sub

    ''' <summary> Edit con passaggio a step successivo. </summary>
    Private Sub btnConferma_Click(sender As Object, e As EventArgs) Handles btnConferma.Click
        If HasCheckOut() OrElse Not ValidateObject() OrElse Not ValidateAlertDate() Then
            Exit Sub
        End If

        Try
            Dim nextAction As String = Action2
            Dim success As Boolean = False
            Select Case Action
                Case CollaborationSubAction.InserimentoAllaVisioneFirma, CollaborationSubAction.InserimentoAlProtocolloSegreteria
                    success = InsertNewCollaboration()
                    nextAction = CollaborationMainAction.AllaVisioneFirma
                Case CollaborationSubAction.AllaVisioneFirma, CollaborationSubAction.AllaVisioneFirma, CollaborationSubAction.AttivitaInCorso
                    EditCollaboration()
                    success = True
            End Select

            ' Se attiva la uoia allora eseguo l'inserimento nella tabella aggregate
            If FromCollaboratinoUoia Then
                InsertNewCollaborationAggregate()
                For Each collUoia As Collaboration In CollaborationUoiaSelected
                    Dim collaborationXmlDraft As CollaborationXmlData = Facade.ProtocolDraftFacade.GetDataFromCollaboration(collUoia)
                    If collaborationXmlDraft IsNot Nothing Then
                        Facade.ProtocolDraftFacade.AddProtocolDraft(CurrentCollaboration, "Bozza di Protocollo da Collaborazione", collaborationXmlDraft)
                        Exit For
                    End If
                Next

                'Ora svuoto la Sessione
                Session("SessionCollaborationUoia") = Nothing
            End If

            If InsertFromWorkflow Then
                Dim workflowActivityLog As WorkflowActivityLog = New WorkflowActivityLog With {
                    .LogDescription = String.Format("Attività generica Collaborazione {0}", CurrentCollaboration.Id),
                    .LogType = WorkflowStatus.Progress,
                    .Entity = New WorkflowActivity(CurrentIdWorkflowActivity),
                    .SystemComputer = DocSuiteContext.Current.UserComputer
                }
                CurrentWorkflowActivityLogFacade.Save(workflowActivityLog)
            End If

            If success Then
                ' Vado allo step successivo
                Response.Redirect(String.Format("UserCollRisultati.aspx?Title={0}&Action={1}", Title2, nextAction), False)
            End If

        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore in fase di conferma", ex)
            AjaxAlert(ex.Message)
        Finally
            If CurrentCollaboration IsNot Nothing AndAlso String.IsNullOrEmpty(CurrentCollaboration.GetFirstDocumentVersioningName()) Then
                AjaxAlert("Errore documento principale{0} {1}", Environment.NewLine, ProtocolEnv.DefaultErrorMessage)
            End If
        End Try

    End Sub

    Private Sub BtnCancellaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancella.Click
        If CanDeleteCollaboration() Then
            DeleteCollaboration()
        End If
    End Sub

    Private Function CanDeleteCollaboration() As Boolean
        Dim deleteCollaboration As Boolean = False
        If ProtocolEnv.ForceDeleteCollaborationEnabled Then
            Dim collaborationVersioning As CollaborationVersioning = Facade.CollaborationVersioningFacade.GetLastCheckout(CurrentCollaboration)
            deleteCollaboration = True
            If Not IsNothing(collaborationVersioning) Then
                Dim confirmMessage As String = String.Format("Attenzione! Ci sono dei file estratti in modifica nella collaborazione {0}. Estratti da {1}. Annullare la collaborazione?", CurrentCollaboration.Id, collaborationVersioning.CheckOutUser)
                AjaxAlertConfirm(confirmMessage, "ExecuteAjaxRequest('DELETECOLLABORATION');", Nothing)
                deleteCollaboration = False
            End If
        Else
            deleteCollaboration = Not HasCheckOut()
        End If
        Return deleteCollaboration
    End Function


    Private Sub ForceDeleteCollaboration()
        Dim collaborationVersioningList As IList(Of CollaborationVersioning) = Facade.CollaborationVersioningFacade.GetDocumentInCheckout(CurrentCollaboration)
        For Each versioning As CollaborationVersioning In collaborationVersioningList
            Facade.CollaborationVersioningFacade.UndoCheckOut(versioning, DocSuiteContext.Current.User.FullUserName, True)
        Next
        DeleteCollaboration()
    End Sub

    Private Sub DeleteCollaboration()
        Facade.CollaborationFacade.SendMail(CurrentCollaboration, CollaborationMainAction.CancellazioneDocumento)
        Try
            ' Aggiornamento resolution
            If DocSuiteContext.Current.IsResolutionEnabled Then
                Dim resolutions As ICollection(Of Resolution) = Facade.ResolutionFacade.GetBySupervisoryBoardCollaborationId(CurrentCollaboration.Id)
                If Not resolutions.IsNullOrEmpty() Then
                    For Each resolution As Resolution In resolutions
                        resolution.SupervisoryBoardProtocolCollaboration = Nothing
                        Facade.ResolutionFacade.UpdateOnly(resolution)
                    Next
                End If
                'aggiorna atto da stato affarigenerali -> attiva 
                If ProtocolEnv.CheckResolutionCollaborationOriginEnabled Then
                    Dim draft As CollaborationXmlData = FacadeFactory.Instance.ProtocolDraftFacade.GetDataFromCollaboration(CurrentCollaboration)
                    If draft IsNot Nothing AndAlso draft.GetType() = GetType(ResolutionXML) Then
                        Dim resolutionXML As ResolutionXML = CType(draft, ResolutionXML)
                        If (resolutionXML IsNot Nothing) Then
                            Dim idResolution As Integer = resolutionXML.ResolutionsToUpdate()(0)
                            Dim resolution As Resolution = Facade.ResolutionFacade.GetById(idResolution)
                            Dim idAnnexes As Guid = Guid.Empty
                            Dim signature As String = "*"
                            resolution.Status = Facade.ResolutionStatusFacade.GetById(ResolutionStatusId.Attivo)
                            'annessi
                            Dim documentsDictionary As IDictionary(Of Guid, BiblosDocumentInfo)
                            documentsDictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(CurrentCollaboration, VersioningDocumentGroup.Annexed)
                            If documentsDictionary IsNot Nothing AndAlso documentsDictionary.Count > 0 Then
                                Facade.ResolutionFacade.SaveBiblosDocuments(resolution, New List(Of DocumentInfo)(documentsDictionary.Values), signature, idAnnexes)
                                Facade.ResolutionFacade.SqlResolutionDocumentUpdate(idResolution, idAnnexes, ResolutionFacade.DocType.Annessi)
                            End If
                            Facade.ResolutionFacade.UpdateOnly(resolution)
                            Facade.ResolutionLogFacade.Log(resolution, ResolutionLogType.AC, "ANNULLA COLLABORAZIONE GENERATA DA AFFARI GENERALI")
                        End If
                    End If
                End If
            End If
            Facade.CollaborationLogFacade.Insert(CurrentCollaboration, Nothing, Nothing, Nothing, CollaborationLogType.CA, String.Format("Annullamento Collaborazione {0}", CurrentCollaboration.Id))
            FacadeFactory.Instance.TableLogFacade.Insert("Collaboration", LogEvent.DL, $"Annullata collaborazione {CurrentCollaboration.CollaborationObject} del {CurrentCollaboration.RegistrationDate} ({CurrentCollaboration.Id} - da {DocSuiteContext.Current.User.FullUserName})", CurrentCollaboration.UniqueId)
            Facade.ProtocolDraftFacade.DeleteFromCollaboration(CurrentCollaboration)
            Facade.CollaborationFacade.Delete(CurrentCollaboration)
            Response.Redirect(String.Format("UserCollRisultati.aspx?Title={0}&Action={1}", Title2, Action2), False)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            AjaxAlert("Errore in Annullamento della Registrazione")
        End Try
    End Sub

    Private Sub BtnRestituzioneClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnRestituzione.Click
        If HasCheckOut() OrElse Not ValidateAlertDate() Then
            Exit Sub
        End If

        ' Se non esiste almeno un destinatario di firma/inoltro selezionato
        If uscSettoriSegreterie.GetCheckedRoles().IsNullOrEmpty() AndAlso Not uscRestituzioni.TreeViewControl.Nodes(0).Nodes.Cast(Of RadTreeNode)().Any(Function(node) node.Checked) Then
            AjaxAlert("Selezionare almeno un destinatario.")
            Exit Sub
        End If

        ExistDocumentToSign("RESTITUTION", New GenericSubDelegate(AddressOf ExecuteRestitution))
    End Sub

    Private Sub BtnRifiutaClick(ByVal sender As Object, ByVal e As EventArgs)
        If HasCheckOut() OrElse Not ValidateAlertDate() Then
            Exit Sub
        End If

        Try
            Facade.CollaborationFacade.Update(CurrentCollaboration, rblPriority.SelectedValue, txtDate.SelectedDate, txtObject.Text, txtNote.Text, CollaborationStatusType.DL, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, "", 0, False)
            Facade.CollaborationFacade.SendMail(CurrentCollaboration, CollaborationMainAction.DaLeggere)
            Facade.CollaborationLogFacade.Insert(CurrentCollaboration, $"Collaborazione rifiutata e restituita al proponente con motivazione '{txtNote.Text}'.")
            Response.Redirect(String.Format("UserCollRisultati.aspx?Title={0}&Action={1}", Title2, Action2))
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            AjaxAlert("Errore in Aggiornamento Dati: " & ex.Message)
        End Try
    End Sub

    Private Sub BtnVersioningClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnVersioning.Click
        If HasCheckOut() Then
            Exit Sub
        End If

        Response.Redirect("../User/UserCollVersioning.aspx?" & CommonShared.AppendSecurityCheck("Type=Prot&IdCollaboration=" & CurrentCollaboration.Id.ToString()))
    End Sub

    Private Sub BtnInoltraClick(ByVal sender As Object, ByVal e As EventArgs)

        If Not ValidateAlertDate() Then
            Exit Sub
        End If

        If rblTipoOperazione.SelectedValue.Eq("IS") Then
            ExistDocumentToSign("FORWARD", New GenericSubDelegate(AddressOf ExecuteForward))
            Exit Sub
        End If

        If HasCheckOut() Then
            Exit Sub
        End If

        ExistDocumentToSign("FORWARD", New GenericSubDelegate(AddressOf ExecuteForward))

    End Sub

    Private Sub BtnApprovaWorkflowClick(sender As Object, e As EventArgs)
        Try
            ApproveRefuseWorkflowCollaboration(True)
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            AjaxAlert("Aggiornamento attività non riuscita.")
        End Try
    End Sub

    Private Sub BtnRifiutaWorkflowClick(sender As Object, e As EventArgs)
        Try
            ApproveRefuseWorkflowCollaboration(False)
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            AjaxAlert("Aggiornamento attività non riuscita.")
        End Try

    End Sub

    Private Sub BtnProtocollaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnProtocolla.Click
        If HasCheckOut() Then
            Exit Sub
        End If

        If ProtocolEnv.CheckCollaborationSigns AndAlso Not CheckRequiredSign() Then
            Exit Sub
        End If

        If DocSuiteContext.Current.ProtocolEnv.ForceCollaborationSignDateEnabled Then
            Dim activeSigner As CollaborationSign = CurrentCollaboration.GetFirstCollaborationSignActive()
            If Not activeSigner.SignDate.HasValue Then
                activeSigner.SignDate = Date.Now
                Facade.CollaborationSignsFacade.UpdateOnly(activeSigner)
            End If
            Facade.CollaborationFacade.UpdateBiblosSignsModel(CurrentCollaboration)
        End If

        Dim params As String = String.Format("Action=FromCollaboration&IdCollaboration={0}", CurrentCollaboration.Id)
        Dim url As String = String.Empty
        Select Case CurrentCollaboration.DocumentType
            Case CollaborationDocumentType.P.ToString()
                url = $"../Prot/ProtInserimento.aspx?{CommonShared.AppendSecurityCheck(params)}"
            Case CollaborationDocumentType.D.ToString(), CollaborationDocumentType.A.ToString()
                url = $"../Resl/ReslInserimento.aspx?{CommonShared.AppendSecurityCheck($"{params}&Type=Resl")}"
            Case CollaborationDocumentType.S.ToString()
                url = $"../Series/CollaborationToSeries.aspx?{CommonShared.AppendSecurityCheck($"{params}&Type=Series")}"
            Case CollaborationDocumentType.UDS.ToString()
                url = $"../UDS/CollaborationToUDS.aspx?{params}&Type=UDS"
            Case CollaborationDocumentType.W.ToString()
                'verifica che ci sia format corretto
                url = $"{String.Format(CommonInstance.GetWorkflowActionPage(WorkflowActivityArea.Collaboration, WorkflowActivityAction.ToProtocol), CurrentCollaboration.Id)}&FromWorkFlow=True&IdWorkflowActivity={CurrentWorkflowActivity.UniqueId}"
        End Select

        If Integer.TryParse(CurrentCollaboration.DocumentType, 0) Then
            url = $"../UDS/CollaborationToUDS.aspx?{params}&Type=UDS&UDSEnvironment={CurrentCollaboration.DocumentType}"
        End If

        If ProtocolEnv.CollaborationOpenOnMain Then
            url = url.Replace("~", "..")
            AjaxManager.ResponseScripts.Add($"location.href='{url}'")
        Else
            Response.Redirect(url)
        End If

    End Sub

    Private Sub BtnVisioneProtocollaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnVisioneProtocolla.Click
        If HasCheckOut() OrElse Not ValidateAlertDate() Then
            Exit Sub
        End If

        If ProtocolEnv.CheckCollaborationSigns AndAlso Not CheckRequiredSign() Then
            Exit Sub
        End If

        ExistDocumentToSign("VISIONEPROTOCOLLA", New GenericSubDelegate(AddressOf ExecuteVisioneProtocolla))
    End Sub

    Private Sub BtnRichiamoClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnRichiamo.Click
        If CurrentCollaboration Is Nothing Then
            Exit Sub
        End If

        If HasCheckOut() Then
            Exit Sub
        End If

        Dim collsign As CollaborationSign = Facade.CollaborationSignsFacade.SearchFull(CurrentCollaboration.Id, True).Single()

        Try
            Facade.CollaborationFacade.Richiamo(CurrentCollaboration, collsign)
            Facade.CollaborationFacade.SendMail(CurrentCollaboration, CollaborationMainAction.Richiamo)
            Response.Redirect(String.Format("UserCollRisultati.aspx?Title={0}&Action={1}", Title2, Action2))

        Catch ex As DocSuiteException
            FileLogger.Warn(LoggerName, ex.Message, ex)
            AjaxAlert("Errore in aggiornamento registrazione" & Environment.NewLine & ex.Message)
        End Try
    End Sub

    Private Sub RblTipoOperazioneSelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles rblTipoOperazione.SelectedIndexChanged
        SetVisioneFirma()
    End Sub

    Private Sub UscInoltroContactAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscInoltro.RoleUserContactAdded, uscInoltro.ManualContactAdded
        Dim contact As Contact = JsonConvert.DeserializeObject(Of Contact)(uscInoltro.JsonContactAdded)
        ' Elimino le collaborazioni che ho già firmato
        If (ProtocolEnv.CollaborationNoMultipleSign() AndAlso
            Facade.CollaborationSignsFacade.IsSigned(CurrentCollaboration.Id, contact.Code)) Then
            uscInoltro.DataSource = Nothing
            uscInoltro.DataBind()
            AjaxAlert("La collaborazione è già stata firmata dall'utente '{0}'.", contact.Description)
            Exit Sub
        End If

        InitializeSegreterie(contact)
    End Sub

    Private Sub UscVisioneFirmaContactAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscVisioneFirma.RoleUserContactAdded, uscVisioneFirma.ManualContactAdded
        Dim contact As Contact = JsonConvert.DeserializeObject(Of Contact)(uscVisioneFirma.JsonContactAdded)
        InitializeSegreterie(contact)

        ' Imposta il check per l'ultimo destinatario di firma aggiungo coerentemente al parametro VisioneFirmaChecked.
        If DocSuiteContext.Current.ProtocolEnv.VisioneFirmaChecked Then
            Dim lastAddedNode As RadTreeNode = uscVisioneFirma.TreeViewControl.Nodes(0).Nodes.FindNodeByText(contact.FullDescription(uscVisioneFirma.SimpleMode))
            If lastAddedNode IsNot Nothing Then
                lastAddedNode.Checked = True
            End If
        End If
    End Sub

    Private Sub UscVisioneFirmaContactRemoved(ByVal sender As Object, ByVal e As EventArgs) Handles uscVisioneFirma.ContactRemoved
        Dim tnRemoved As RadTreeNode = uscVisioneFirma.LastRemovedNode
        If tnRemoved Is Nothing Then
            RoleContacts.Clear()
            uscVisioneFirma.TreeViewControl.Nodes(0).Nodes.Clear()
            uscSettoriSegreterie.SourceRoles.Clear()
            uscSettoriSegreterie.DataBind()
            Exit Sub
        End If

        Dim serializedContact As String = tnRemoved.Attributes(uscContattiSel.ManualContactAttribute)
        If String.IsNullOrEmpty(serializedContact) Then
            Exit Sub
        End If

        Dim contact As Contact = JsonConvert.DeserializeObject(Of Contact)(serializedContact)
        ' Controllo se la rimozione del contatto porta alla rimozione di ruoli
        Dim roles As List(Of Role) = uscSettoriSegreterie.GetRoles().ToList()
        For Each role As Role In roles
            If Not RoleContacts.ContainsKey(role) OrElse Not RoleContacts(role).Contains(contact) Then
                Continue For
            End If
            ' Questo orrore, incipit di futuri bachi, è dovuto al fatto che i contatti non sono consistenti ne facilmente verificabili
            RoleContacts(role).RemoveAll(Function(c) c.Code.Eq(contact.Code) AndAlso c.Description.Eq(contact.Description))
            If RoleContacts(role).IsNullOrEmpty() Then
                RoleContacts.Remove(role)
                uscSettoriSegreterie.RemoveRole(role)
            End If
        Next
    End Sub

    Private Sub DdlDocumentTypeSelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlDocumentType.SelectedIndexChanged
        If String.IsNullOrEmpty(ddlDocumentType.SelectedValue) Then
            pnlMainPanel.SetDisplay(False)
            btnConferma.Enabled = False
            btnConferma.ToolTip = NO_DOCUMENT_TYPE_SELECTED_TOOLTIP
            ddlSpecificDocumentType.Items.Clear()
            ddlSpecificDocumentType.ClearSelection()
            Exit Sub
        End If

        pnlMainPanel.SetDisplay(Not ProtocolEnv.CollaborationTemplateRequired)
        btnConferma.Enabled = Not ProtocolEnv.CollaborationTemplateRequired
        btnConferma.ToolTip = Nothing
        If ProtocolEnv.CollaborationTemplateRequired Then
            btnConferma.ToolTip = NO_SPECIFIC_DOCUMENT_TYPE_SELECTED_TOOLTIP
        End If

        Try
            ' Imposto lo stile della pagina sul tipo selezionato
            Dim currentTemplate As WebAPIDto(Of TemplateCollaboration) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentTemplateCollaborationFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.UniqueId = Guid.Parse(ddlDocumentType.SelectedItem.Attributes("TemplateId"))
                        finder.ExpandProperties = True
                        Return finder.DoSearch().FirstOrDefault()
                    End Function)

            If currentTemplate Is Nothing OrElse currentTemplate.Entity Is Nothing Then
                Throw New DocSuiteException(String.Format("Errore in caricamento Template, nessun Template di collaborazione trovato con ID {0}", ddlDocumentType.SelectedValue))
            End If

            Dim pageType As String = CollaborationFacade.GetPageTypeFromDocumentType(currentTemplate.Entity.DocumentType)
            AjaxManager.ResponseScripts.Add(String.Format("changeBodyClass('{0}')", pageType.ToLower()))
            uscVisioneFirma.CollaborationType = ddlDocumentType.SelectedValue
            uscVisioneFirma.EnvironmentType = pageType
            uscVisioneFirma.UpdateRoleUserType()

            trSpecificDocumentType.Visible = ProtocolEnv.CollaborationTemplateRequired
            ddlSpecificDocumentType.Items.Clear()
            ddlSpecificDocumentType.ClearSelection()
            Dim results As ICollection(Of TemplateCollaboration) = GetCollaborationTemplates(False, True, currentTemplate.Entity.DocumentType)
            If results IsNot Nothing AndAlso results.Count > 0 Then
                trSpecificDocumentType.Visible = True
                ddlSpecificDocumentType.Items.Add(New DropDownListItem(String.Empty, String.Empty) With {.Selected = True})
                Dim item As DropDownListItem
                For Each template As TemplateCollaboration In results
                    item = New DropDownListItem(template.Name, template.DocumentType)
                    item.Attributes.Add("TemplateId", template.UniqueId.ToString())
                    ddlSpecificDocumentType.Items.Add(item)
                Next
            End If

            If Not ProtocolEnv.CollaborationTemplateRequired Then
                FillPageFromEntity(currentTemplate.Entity)
            End If
            InitializeDocumentControls()
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            AjaxAlert("Errore in aggiornamento dati per la tipologia di collaborazione selezionata")
        End Try
    End Sub


    Private Sub ddlSpecificDocumentType_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlSpecificDocumentType.SelectedIndexChanged
        If String.IsNullOrEmpty(ddlSpecificDocumentType.SelectedValue) Then
            btnConferma.ToolTip = Nothing
            If ProtocolEnv.CollaborationTemplateRequired Then
                pnlMainPanel.SetDisplay(False)
                btnConferma.Enabled = False
                btnConferma.ToolTip = NO_SPECIFIC_DOCUMENT_TYPE_SELECTED_TOOLTIP
            End If
            Exit Sub
        End If

        If ProtocolEnv.CollaborationTemplateRequired Then
            pnlMainPanel.SetDisplay(True)
            btnConferma.Enabled = True
        End If
        btnConferma.ToolTip = Nothing

        Try
            ' Imposto lo stile della pagina sul tipo selezionato
            Dim currentTemplate As WebAPIDto(Of TemplateCollaboration) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentTemplateCollaborationFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.UniqueId = Guid.Parse(ddlSpecificDocumentType.SelectedItem.Attributes("TemplateId"))
                        finder.ExpandProperties = True
                        Return finder.DoSearch().FirstOrDefault()
                    End Function)

            If currentTemplate Is Nothing OrElse currentTemplate.Entity Is Nothing Then
                Throw New DocSuiteException(String.Format("Errore in caricamento Template, nessun Template di collaborazione trovato con ID {0}", ddlSpecificDocumentType.SelectedValue))
            End If
            FillPageFromEntity(currentTemplate.Entity)
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            AjaxAlert("Errore in aggiornamento dati per la tipologia di collaborazione selezionata")
        End Try
    End Sub

    Private Sub UscDocumentUploadDocumentBeforeSign(ByVal sender As Object, ByVal e As DocumentBeforeSignEventArgs) Handles uscDocumento.DocumentBeforeSign, uscDocumentoOmissis.DocumentBeforeSign, uscAllegati.DocumentBeforeSign, uscAllegatiOmissis.DocumentBeforeSign, uscAnnexed.DocumentBeforeSign
        If Action.Eq(CollaborationSubAction.InserimentoAllaVisioneFirma) OrElse Action.Eq(CollaborationSubAction.InserimentoAlProtocolloSegreteria) Then
            Exit Sub
        End If

        ' TODO: gestire qui tutte le attività preventive alla firma.
        ' Impostare e.Cancel = true in caso si voglia bloccare l'apertura della maschera di firma

        Dim versioning As CollaborationVersioning = CollaborationVersioningFacade.GetVersioningByDocumentInfo(LastVersionings, e.SourceDocument)
        If versioning.CheckedOut Then
            Dim message As String = String.Format("Impossibile firmare il file [{0}] perchè già estratto da {1} dal {2:dd/MM/yyyy}", versioning.DocumentName, CommonAD.GetDisplayName(versioning.CheckOutUser), versioning.CheckOutDate)
            e.Cancel = True
            AjaxAlert(message)
        End If
    End Sub

    Private Sub UscDocumentUploadDocumentSigned(ByVal sender As Object, ByVal e As DocumentSignedEventArgs) Handles uscDocumento.DocumentSigned, uscDocumentoOmissis.DocumentSigned, uscAllegati.DocumentSigned, uscAllegatiOmissis.DocumentSigned, uscAnnexed.DocumentSigned
        If Action.Eq(CollaborationSubAction.InserimentoAllaVisioneFirma) OrElse Action.Eq(CollaborationSubAction.InserimentoAlProtocolloSegreteria) Then
            Return
        End If

        Try
            Dim versioning As CollaborationVersioning = CollaborationVersioningFacade.GetVersioningByDocumentInfo(LastVersionings, e.SourceDocument)
            If CollaborationVersioningFacade.IsMainDocumentVersioning(versioning) Then
                Facade.CollaborationVersioningFacade.CheckOut(versioning, DocSuiteContext.Current.User.FullUserName)
                Facade.CollaborationVersioningFacade.CheckIn(versioning, DocSuiteContext.Current.User.FullUserName, e.DestinationDocument)
                If e.DestinationDocument.IsSigned Then
                    Facade.CollaborationFacade.SetSignedByUser(CurrentCollaboration, DocSuiteContext.Current.User.FullUserName)
                End If
            Else
                Facade.CollaborationVersioningFacade.CheckOut(versioning, DocSuiteContext.Current.User.FullUserName)
                Facade.CollaborationVersioningFacade.CheckIn(versioning, DocSuiteContext.Current.User.FullUserName, e.DestinationDocument)
            End If
            Facade.CollaborationLogFacade.Insert(CurrentCollaboration, versioning.CollaborationIncremental, versioning.Incremental, versioning.IdDocument, CollaborationLogType.MF, String.Format("Firma Documento [{0}].", e.SourceDocument.Name))

            LastVersionings = Nothing
            UpdateWorkflowStep(versioning)
            BindMainDocuments()
            BindMainDocumentsOmissis()
            BindAttachments()
            BindAttachmentsOmissis()
            BindAnnexed()
        Catch ex As Exception
            Dim message As String = String.Format("Si è verificato un errore in fase di firma: {0}.", ex.Message)
            FileLogger.Error(LoggerName, message, ex)
            ' TODO: controllare come mai non si usa l'alert normale
            Dim alert As String = String.Format("alert('{0}'); var returnFalse = function() {{ return false; }};", message)
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "uscDocumentUpload_ButtonPreviewClick_Alert", alert, True)
        End Try
    End Sub

    Private Sub UscDocumentoDocumentUploaded(ByVal sender As Object, ByVal e As DocumentEventArgs) Handles uscDocumento.DocumentUploaded
        If Not Action.Eq(CollaborationSubAction.AllaVisioneFirma) AndAlso Not Action.Eq(CollaborationSubAction.AttivitaInCorso) Then
            Return
        End If

        If CheckModifyRight Then
            Dim versioning As CollaborationVersioning = LastVersionings.Where(Function(v) v.DocumentGroup.Eq(VersioningDocumentGroup.MainDocument)).FirstOrDefault()
            If versioning IsNot Nothing Then
                Facade.CollaborationVersioningFacade.CheckOut(versioning, DocSuiteContext.Current.User.FullUserName)
                Facade.CollaborationVersioningFacade.CheckIn(versioning, DocSuiteContext.Current.User.FullUserName, e.Document)
                Facade.CollaborationLogFacade.Insert(CurrentCollaboration, "Modificato documento principale.")
            End If
            BindMainDocuments()
        End If

    End Sub

    Private Sub UscDocumentoOmissisDocumentUploaded(ByVal sender As Object, ByVal e As DocumentEventArgs) Handles uscDocumentoOmissis.DocumentUploaded
        If Action.Eq(CollaborationSubAction.InserimentoAllaVisioneFirma) OrElse Action.Eq(CollaborationSubAction.InserimentoAlProtocolloSegreteria) Then
            Return
        End If

        AddDocumentsToVersioning(New List(Of DocumentInfo) From {e.Document}, VersioningDocumentGroup.MainDocumentOmissis)
        BindMainDocumentsOmissis()
    End Sub

    Private Sub UscAllegatiDocumentUploaded(ByVal sender As Object, ByVal e As DocumentEventArgs) Handles uscAllegati.DocumentUploaded
        If Action.Eq(CollaborationSubAction.InserimentoAllaVisioneFirma) OrElse Action.Eq(CollaborationSubAction.InserimentoAlProtocolloSegreteria) Then
            Return
        End If

        AddDocumentsToVersioning(New List(Of DocumentInfo) From {e.Document}, VersioningDocumentGroup.Attachment)
        BindAttachments()
    End Sub

    Private Sub UscAllegatiOmissisDocumentUploaded(ByVal sender As Object, ByVal e As DocumentEventArgs) Handles uscAllegatiOmissis.DocumentUploaded
        If Action.Eq(CollaborationSubAction.InserimentoAllaVisioneFirma) OrElse Action.Eq(CollaborationSubAction.InserimentoAlProtocolloSegreteria) Then
            Return
        End If

        AddDocumentsToVersioning(New List(Of DocumentInfo) From {e.Document}, VersioningDocumentGroup.AttachmentOmissis)
        BindAttachmentsOmissis()
    End Sub

    Private Sub UscAnnexedDocumentUploaded(sender As Object, e As DocumentEventArgs) Handles uscAnnexed.DocumentUploaded
        If Action.Eq(CollaborationSubAction.InserimentoAllaVisioneFirma) OrElse Action.Eq(CollaborationSubAction.InserimentoAlProtocolloSegreteria) Then
            Return
        End If

        AddDocumentsToVersioning(New List(Of DocumentInfo) From {e.Document}, VersioningDocumentGroup.Annexed)
        BindAnnexed()
    End Sub

    Private Function CanCheckOut(documentUpload As uscDocumentUpload) As Boolean
        Dim doc As DocumentInfo
        Dim versioning As CollaborationVersioning = New CollaborationVersioning()
        For Each node As RadTreeNode In documentUpload.TreeViewControl.Nodes(0).Nodes
            doc = uscDocumentUpload.GetDocumentInfoByNode(node)
            If doc IsNot Nothing Then
                versioning = CollaborationVersioningFacade.GetVersioningByDocumentInfo(LastVersionings, doc)
            End If
            If versioning IsNot Nothing Then
                If CheckModifyRight AndAlso versioning.CheckOutDate Is Nothing Then
                    Return True
                End If
            End If
        Next

        If DocSuiteContext.Current.ProtocolEnv.CheckOutCollaborationOnlyForSigners AndAlso Not Action.Eq(CollaborationSubAction.DaVisionareFirmare) Then
            Return False
        End If

        If HasDeskSource Then
            Return False
        End If

        Dim retval As Boolean = documentUpload.SelectedDocumentInfo IsNot Nothing

        If retval AndAlso DocSuiteContext.Current.ProtocolEnv.CollaborationWhiteListEnabled Then
            Dim whiteList As String() = DocSuiteContext.Current.ProtocolEnv.CollaborationWhiteList.Split("|"c)
            retval = whiteList.Any(Function(e) FileHelper.MatchExtension(documentUpload.SelectedDocumentInfo.Name, e))
        End If

        If DocSuiteContext.Current.ProtocolEnv.CheckOutCollaborationOnlyForSigners AndAlso retval Then
            If Not IsSignatory() Then
                retval = False
            End If
        End If

        Return retval
    End Function

    Private Sub UscDocumentUpload_DocumentRemoved(sender As Object, e As DocumentEventArgs) Handles uscDocumentoOmissis.DocumentRemoved, uscAllegati.DocumentRemoved, uscAllegatiOmissis.DocumentRemoved, uscAnnexed.DocumentRemoved
        Dim versioning As CollaborationVersioning = CollaborationVersioningFacade.GetVersioningByDocumentInfo(LastVersionings, e.Document)
        If versioning IsNot Nothing Then
            Facade.CollaborationVersioningFacade.DiscardVersioning(versioning)
        End If

        Dim documentUpload As uscDocumentUpload = DirectCast(sender, uscDocumentUpload)
        GetCheckOutCommand(documentUpload).Enabled = CanCheckOut(documentUpload)
    End Sub

    Private Sub cmdVersioningManagement_Click(sender As Object, e As EventArgs) Handles cmdVersioningManagement.Click
        Dim backurl As New Uri(ConvertRelativeUrlToAbsoluteUrl(Page.Request.Url.OriginalString), UriKind.Absolute)
        Dim url As String = String.Format("CollaborationVersioningManagement.aspx?collaborationid={0}&backurl={1}", CurrentCollaboration.Id, Server.UrlEncode(backurl.PathAndQuery))
        Response.Redirect(url)
    End Sub

    Private Sub cmdCheckOut_Click(sender As Object, e As EventArgs) Handles cmdDocumentCheckOut.Click, cmdDocumentOmissisCheckOut.Click, cmdAttachmentsCheckOut.Click, cmdAttachmentsOmissisCheckOut.Click, cmdAnnexedCheckOut.Click
        Dim cmdSender As Button = DirectCast(sender, Button)
        Dim uscUpload As uscDocumentUpload = GetUscUploadByCommand(cmdSender)

        Dim selected As IList(Of DocumentInfo)
        If uscUpload.SelectedDocumentInfo Is Nothing Then
            AjaxAlert("Nessun documento selezionato")
            Exit Sub
        End If
        selected = New List(Of DocumentInfo) From {uscUpload.SelectedDocumentInfo}
        CheckOutDocuments(selected)

        BindUscUploadByCommand(cmdSender)
        BindUploadButtons(uscUpload, uscUpload.SelectedDocumentInfo)
    End Sub

    Private Sub cmdUndoCheckOut_Click(sender As Object, e As EventArgs) Handles cmdDocumentUndoCheckOut.Click, cmdDocumentOmissisUndoCheckOut.Click, cmdAttachmentsUndoCheckOut.Click, cmdAttachmentsOmissisUndoCheckOut.Click, cmdAnnexedUndoCheckOut.Click
        Dim cmdSender As Button = DirectCast(sender, Button)
        Dim uscUpload As uscDocumentUpload = GetUscUploadByCommand(cmdSender)

        Dim selected As IList(Of DocumentInfo) = Nothing
        If uscUpload.SelectedDocumentInfo IsNot Nothing Then
            selected = New List(Of DocumentInfo) From {uscUpload.SelectedDocumentInfo}
        End If
        UndoCheckOutDocuments(selected)

        BindUscUploadByCommand(cmdSender)
        BindUploadButtons(uscUpload, uscUpload.SelectedDocumentInfo)
    End Sub

    Private Sub cmdCheckIn_Click(sender As Object, e As EventArgs) Handles cmdDocumentCheckIn.Click, cmdDocumentOmissisCheckIn.Click, cmdAttachmentsCheckIn.Click, cmdAttachmentsOmissisCheckIn.Click, cmdAnnexedCheckIn.Click
        Dim cmdSender As Button = DirectCast(sender, Button)
        Dim uscUpload As uscDocumentUpload = GetUscUploadByCommand(cmdSender)

        Dim selected As IList(Of DocumentInfo) = Nothing
        If uscUpload.SelectedDocumentInfo IsNot Nothing Then
            selected = New List(Of DocumentInfo) From {uscUpload.SelectedDocumentInfo}
        End If
        CheckInDocuments(selected)

        If CurrentCollaboration Is Nothing Then
            Exit Sub
        End If
        BindUscUploadByCommand(cmdSender)
        BindUploadButtons(uscUpload, uscUpload.SelectedDocumentInfo)
    End Sub

    Private Sub uscDocumento_DocumentSelected(sender As Object, e As DocumentEventArgs) Handles uscDocumento.DocumentSelected, uscDocumentoOmissis.DocumentSelected, uscAllegati.DocumentSelected, uscAllegatiOmissis.DocumentSelected, uscAnnexed.DocumentSelected
        Dim doc As DocumentInfo = e.Document
        BindUploadButtons(DirectCast(sender, Control), doc)
    End Sub

    Private Sub btnNewDesk_Click(sender As Object, e As EventArgs) Handles btnNewDesk.Click
        CreateNewDeskFromCollaboration()
    End Sub

    Private Sub btnResumeDesk_Click(sender As Object, e As EventArgs) Handles btnResumeDesk.Click
        ResumeDeskFromCollaboration()
    End Sub

    Protected Sub btnEditCollaborationData_Click(sender As Object, e As EventArgs) Handles btnEditCollaborationData.Click
        If tAlertData.Visible Then
            txtDate.DateInput.ReadOnly = False
            txtDate.DatePopupButton.Visible = True
        End If
        alertDate.DateInput.ReadOnly = False
        alertDate.DatePopupButton.Visible = True
        txtObject.Enabled = True
        txtNote.Enabled = True
        btnEditCollaborationData.Enabled = False
        cmdSave.Visible = True
    End Sub

    Protected Sub btnExpandUDS_Click(sender As Object, e As EventArgs) Handles btnExpandUDS.Click
        UDSSummaryExpander()
    End Sub

    Private Sub btnAbsence_Click(sender As Object, e As EventArgs) Handles btnAbsence.Click
        If ProtocolEnv.CollaborationRightsEnabled AndAlso Not CurrentCollaboration.DocumentType = CollaborationDocumentType.D.ToString() Then
            AjaxAlert("Non è possibile gestire le assenze per la tipologia di collaborazione selezionata.")
            Exit Sub
        End If

        Dim url As String = String.Concat("../User/UserAbsentManagers.aspx?Type=Resl&IdCollaboration=", CollaborationId.ToString())
        AjaxManager.ResponseScripts.Add(String.Concat("OpenWindow('", url, "','rwAbsentManager',400,300);"))
    End Sub

    Private Sub btnDocumentUnitDraftEditor_Click(sender As Object, e As EventArgs) Handles btnDocumentUnitDraftEditor.Click
        Dim url As String = String.Empty

        If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.P.ToString()) Then
            url = String.Format("{0}?{1}", "../Prot/TemplateProtocolInsert.aspx", CommonShared.AppendSecurityCheck("Type=Prot&Action=precompiler"))
            AjaxManager.ResponseScripts.Add(String.Concat("OpenWindow('", url, "','rwPrecompilerProtocol',800,600);"))
        End If
        If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.UDS.ToString()) Then
            'to do
        End If
    End Sub
#End Region

#Region " Methods "
    ''' <summary>
    ''' Sub che esegue le operazioni di aggregazioni UOIA visuali. Inserisce i documenti in usercontrol di Main document e Attachments
    ''' </summary>
    Private Sub GenerateTemplateUoia()

        ddlDocumentType.SelectedValue = CollaborationDocumentType.P.ToString()
        txtObject.Text = GenerateSubjectFromUoia(CollaborationUoiaSelected, String.Format("Trasmissione Verbali SSIA {0}", DocSuiteContext.Current.ProtocolEnv.CorporateName))

        'Inserimento Utenti principali in destinatario
        'TODO vedifica della firma degli allegati
        Dim destinatario As IList(Of RoleUser) = Facade.RoleUserFacade.GetByRoleIdAndType(DocSuiteContext.Current.ProtocolEnv.CollaborationRoleUoia, RoleUserType.D, True, True)
        For Each dest As RoleUser In destinatario
            uscVisioneFirma.AddCollaborationContact(ContactType.AdAmPerson, dest.Description, dest.Email, dest.Account, String.Empty, dest.Id.ToString, True, True, True)
        Next

        'Inserimento segreterie
        'TODO vedifica della firma degli allegati
        Dim segretaries As IList(Of RoleUser) = Facade.RoleUserFacade.GetByRoleIdAndType(DocSuiteContext.Current.ProtocolEnv.CollaborationRoleUoia, RoleUserType.S, True, True)
        For Each collaborationRole As RoleUser In segretaries
            Dim role As Role = Facade.RoleFacade.GetById(collaborationRole.Role.Id)
            uscSettoriSegreterie.AddRole(role, True, False, False, True)
        Next

        'Inserimento documento principale contenente elenco delle uoia aggregate
        'TODO vedifica della firma degli allegati
        uscDocumento.LoadDocumentInfo(CreateMainDocumentUoia(CollaborationUoiaSelected, destinatario, UoiaParametersReport()), False, True, False, True)
        uscDocumento.InitializeNodesAsAdded(True)
        uscDocumento.TreeViewControl.Nodes(0).Nodes(0).Selected = True

        ''Inserimento documenti delle uoia aggregate in allegati
        ''TODO vedifica della firma degli allegati
        uscAllegati.LoadDocumentInfo(UoiaCollaboration(), False, True, False, False)
        uscAllegati.InitializeNodesAsAdded(True)
        uscAllegati.TreeViewControl.Nodes(0).Nodes(0).Selected = True

    End Sub

    ''' <summary>
    ''' Metodo  nel quale i main document delle collaborazioni aggregate vengono importati nella collaborazione principale come allegati
    ''' </summary>
    Private Function UoiaCollaboration() As List(Of DocumentInfo)
        '' TODO: i documenti devono essere firmati
        Dim docsOfCollaborations As New List(Of DocumentInfo)
        For Each collUoia As Collaboration In CollaborationUoiaSelected
            Dim lastAttachments As IDictionary(Of Guid, BiblosDocumentInfo)
            lastAttachments = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(collUoia, VersioningDocumentGroup.MainDocument)
            For Each d As KeyValuePair(Of Guid, BiblosDocumentInfo) In lastAttachments
                docsOfCollaborations.Add(d.Value)
            Next
        Next
        Return docsOfCollaborations

    End Function

    Private Function UoiaParametersReport() As Dictionary(Of String, String)
        Dim parametersForReport As Dictionary(Of String, String) = New Dictionary(Of String, String)

        'Recupero la descrizione associata al una qualsiasi collaborazione da aggregare. Ogni collaborazione ha lo stesso file XML.
        Dim spettabile As String = Facade.ProtocolDraftFacade.GetContactDescriptionFromCollaborationUoia(CollaborationUoiaSelected.FirstOrDefault())
        parametersForReport.Add("Spettabile", spettabile)

        Return parametersForReport
    End Function


    Private Sub InitializeVersioningCommands(showCommands As Boolean)
        cmdVersioningManagement.Visible = showCommands AndAlso Not DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled AndAlso BtnCheckoutEnabled

        If DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled Then
            cmdDocumentCheckOut.Text = "Estrai"
            cmdDocumentUndoCheckOut.Text = "Annulla"
            cmdDocumentCheckIn.Text = "Archivia"

            cmdDocumentOmissisCheckOut.Text = "Estrai"
            cmdDocumentOmissisUndoCheckOut.Text = "Annulla"
            cmdDocumentOmissisCheckIn.Text = "Archivia"

            cmdAttachmentsCheckOut.Text = "Estrai"
            cmdAttachmentsUndoCheckOut.Text = "Annulla"
            cmdAttachmentsCheckIn.Text = "Archivia"

            cmdAttachmentsOmissisCheckOut.Text = "Estrai"
            cmdAttachmentsOmissisUndoCheckOut.Text = "Annulla"
            cmdAttachmentsOmissisCheckIn.Text = "Archivia"

            cmdAnnexedCheckOut.Text = "Estrai"
            cmdAnnexedUndoCheckOut.Text = "Annulla"
            cmdAnnexedCheckIn.Text = "Archivia"
        End If

        cmdDocumentCheckOut.Visible = showCommands AndAlso BtnCheckoutEnabled
        cmdDocumentUndoCheckOut.Visible = showCommands AndAlso DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled AndAlso BtnCheckoutEnabled
        cmdDocumentCheckIn.Visible = showCommands AndAlso DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled AndAlso BtnCheckoutEnabled

        cmdDocumentOmissisCheckOut.Visible = showCommands AndAlso BtnCheckoutEnabled
        cmdDocumentOmissisUndoCheckOut.Visible = showCommands AndAlso DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled AndAlso BtnCheckoutEnabled
        cmdDocumentOmissisCheckIn.Visible = showCommands AndAlso DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled AndAlso BtnCheckoutEnabled

        cmdAttachmentsCheckOut.Visible = showCommands AndAlso BtnCheckoutEnabled
        cmdAttachmentsUndoCheckOut.Visible = showCommands AndAlso DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled AndAlso BtnCheckoutEnabled
        cmdAttachmentsCheckIn.Visible = showCommands AndAlso DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled AndAlso BtnCheckoutEnabled

        cmdAttachmentsOmissisCheckOut.Visible = showCommands AndAlso BtnCheckoutEnabled
        cmdAttachmentsOmissisUndoCheckOut.Visible = showCommands AndAlso DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled AndAlso BtnCheckoutEnabled
        cmdAttachmentsOmissisCheckIn.Visible = showCommands AndAlso DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled AndAlso BtnCheckoutEnabled

        cmdAnnexedCheckOut.Visible = showCommands AndAlso BtnCheckoutEnabled
        cmdAnnexedUndoCheckOut.Visible = showCommands AndAlso DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled AndAlso BtnCheckoutEnabled
        cmdAnnexedCheckIn.Visible = showCommands AndAlso DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled AndAlso BtnCheckoutEnabled
    End Sub

    Private Sub InitializeDocumentsState()
        If uscDocumento.SelectedDocumentInfo IsNot Nothing Then
            BindUploadButtons(uscDocumento, uscDocumento.SelectedDocumentInfo)
        End If

        If uscDocumentoOmissis.SelectedDocumentInfo IsNot Nothing Then
            BindUploadButtons(uscDocumentoOmissis, uscDocumentoOmissis.SelectedDocumentInfo)
        End If

        If uscAllegati.SelectedDocumentInfo IsNot Nothing Then
            BindUploadButtons(uscAllegati, uscAllegati.SelectedDocumentInfo)
        End If

        If uscAllegatiOmissis.SelectedDocumentInfo IsNot Nothing Then
            BindUploadButtons(uscAllegatiOmissis, uscAllegatiOmissis.SelectedDocumentInfo)
        End If

        If uscAnnexed.SelectedDocumentInfo IsNot Nothing Then
            BindUploadButtons(uscAnnexed, uscAnnexed.SelectedDocumentInfo)
        End If
    End Sub

    Private Function GetPreviewOnClientClick() As String
        If CurrentCollaboration Is Nothing Then
            Return Nothing
        End If

        Dim url As String = String.Format("{0}/Viewers/CollaborationViewer.aspx?{1}",
                                           DocSuiteContext.Current.CurrentTenant.DSWUrl,
                                          CommonShared.AppendSecurityCheck("DataSourceType=coll&id=" & CurrentCollaboration.Id))
        Return String.Format("window.location.href='{0}'; var returnFalse = function() {{ return false; }};", url)
    End Function
    Private Sub VerificaOperazioniConcorrenza()
        If CurrentCollaboration Is Nothing OrElse CurrentCollaboration.LastChangedDate Is Nothing OrElse CurrentCollaboration.LastChangedUser Is Nothing Then
            Exit Sub
        End If

        Dim lastOperation As String = String.Format("{0} {1}", CurrentCollaboration.LastChangedUser, CurrentCollaboration.LastChangedDate.Value)
        If Not String.IsNullOrEmpty(txtLastChanged.Text) AndAlso Not txtLastChanged.Text.Eq(lastOperation) Then
            AjaxAlert("La Registrazione è stata modificata dall'utente: [{1}].{0}Aggiornare l'elenco.", Environment.NewLine, lastOperation)
            Exit Sub
        End If

        txtLastChanged.Text = lastOperation
    End Sub

    Private Function GetListItem(template As TemplateCollaboration) As DropDownListItem
        Dim listItem As New DropDownListItem(template.Name.ToString(), template.DocumentType)
        listItem.Attributes.Add("TemplateId", template.UniqueId.ToString())

        If CurrentWorkflowActivity IsNot Nothing Then
            Dim workflowLabelProperty As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_LABEL, CurrentWorkflowActivity.UniqueId)
            If workflowLabelProperty IsNot Nothing Then
                listItem.Text = workflowLabelProperty.ValueString
            End If
        End If

        Return listItem
    End Function

    Private Sub BindDdlDocumentType()
        Dim listItems As DropDownListItem() = Array.Empty(Of DropDownListItem)
        Dim templates As ICollection(Of TemplateCollaboration) = New List(Of TemplateCollaboration)

        If CurrentCollaboration IsNot Nothing Then
            listItems = {New DropDownListItem(CurrentCollaboration.TemplateName, CurrentCollaboration.DocumentType)}
        Else
            Try
                templates = GetCollaborationTemplates(True, True)
            Catch ex As Exception
                FileLogger.Error(LoggerName, ex.Message, ex)
                AjaxAlert("Errore in recupero definizioni di collaborazione")
                Exit Sub
            End Try

            If templates IsNot Nothing AndAlso templates.Count > 0 Then
                listItems = templates.Select(Function(t) GetListItem(t)).ToArray()
            End If
        End If

        ddlDocumentType.Items.Clear()
        ddlDocumentType.Items.Add(String.Empty)
        For Each item As DropDownListItem In listItems
            ddlDocumentType.Items.Add(item)
        Next
    End Sub

    Private Sub InitializeDocumentControls()
        btnMultiSign.PostBackUrl = MultipleSignBasePage.GetMultipleSignUrl()


        lblDocumentCaption.Text = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.MainChain)
        uscDocumento.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.MainChain)


        uscDocumento.SignButtonEnabled = ProtocolEnv.IsFDQEnabled
        uscDocumento.ButtonRemoveEnabled = True
        uscDocumento.ButtonFileEnabled = True
        uscDocumento.ButtonLibrarySharepointEnabled = ProtocolEnv.UploadSharepointDocumentLibrary
        uscDocumento.IsDocumentRequired = True
        uscDocumento.CheckSelectedNode = True
        uscDocumento.HeaderVisible = False

        uscDocumento.MultipleDocuments = False
        'Da gestire l'inserimento di più documenti principali in collaborazione
        'If Not ddlDocumentType.SelectedValue = CollaborationDocumentType.P.ToString() AndAlso ProtocolEnv.CollaborationMultiDocument Then
        '    uscDocumento.MultipleDocuments = True
        'End If

        lblAttachmentsCaption.Text = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AttachmentsChain)
        uscAllegati.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AttachmentsChain)
        uscAllegati.SignButtonEnabled = ProtocolEnv.IsFDQEnabled
        uscAllegati.ButtonRemoveEnabled = True
        uscAllegati.ButtonFileEnabled = True
        uscAllegati.ButtonLibrarySharepointEnabled = ProtocolEnv.UploadSharepointDocumentLibrary
        uscAllegati.IsDocumentRequired = False
        uscAllegati.IsAttachment = True
        uscAllegati.MultipleDocuments = True
        uscAllegati.CheckSelectedNode = True
        uscAllegati.HeaderVisible = False
        If DocSuiteContext.Current.IsResolutionEnabled Then
            uscAllegati.ButtonCopyResl.Visible = DocSuiteContext.Current.ResolutionEnv.CopyReslDocumentsEnabled
        End If
        uscAllegati.ButtonCopyProtocol.Visible = ProtocolEnv.CopyProtocolDocumentsEnabled
        uscAllegati.ButtonCopyUDS.Visible = ProtocolEnv.UDSEnabled

        lblAnnexedCaption.Text = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AnnexedChain)
        uscAnnexed.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AnnexedChain)
        uscAnnexed.SignButtonEnabled = ProtocolEnv.IsFDQEnabled
        uscAnnexed.ButtonRemoveEnabled = True
        uscAnnexed.ButtonFileEnabled = True
        uscAnnexed.ButtonLibrarySharepointEnabled = ProtocolEnv.UploadSharepointDocumentLibrary
        uscAnnexed.IsDocumentRequired = False
        uscAnnexed.IsAttachment = True
        uscAnnexed.MultipleDocuments = True
        uscAnnexed.CheckSelectedNode = True
        uscAnnexed.HeaderVisible = False
        If DocSuiteContext.Current.IsResolutionEnabled Then
            uscAnnexed.ButtonCopyResl.Visible = DocSuiteContext.Current.ResolutionEnv.CopyReslDocumentsEnabled
        End If
        uscAnnexed.ButtonCopyProtocol.Visible = ProtocolEnv.CopyProtocolDocumentsEnabled
        uscAnnexed.ButtonCopyUDS.Visible = ProtocolEnv.UDSEnabled

        lblDocumentoOmissisCaption.Text = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.MainOmissisChain)
        uscDocumentoOmissis.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.MainOmissisChain)

        lblAttachmentOmissisCaption.Text = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AttachmentOmissisChain)
        uscAllegatiOmissis.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AttachmentOmissisChain)

        uscDocumento.ButtonSelectTemplateEnabled = GetControlTemplateDocumentVisibility(Entity.DocumentUnits.ChainType.MainChain)
        uscDocumentoOmissis.ButtonSelectTemplateEnabled = GetControlTemplateDocumentVisibility(Entity.DocumentUnits.ChainType.MainOmissisChain)
        uscAllegati.ButtonSelectTemplateEnabled = GetControlTemplateDocumentVisibility(Entity.DocumentUnits.ChainType.AttachmentsChain)
        uscAllegatiOmissis.ButtonSelectTemplateEnabled = GetControlTemplateDocumentVisibility(Entity.DocumentUnits.ChainType.AttachmentOmissisChain)
        uscAnnexed.ButtonSelectTemplateEnabled = GetControlTemplateDocumentVisibility(Entity.DocumentUnits.ChainType.AnnexedChain)

        ''Definisco se attivare i documenti aggiuntivi (disponibili solo per gli atti)
        Dim ddlType As String = ddlDocumentType.SelectedValue
        If String.IsNullOrEmpty(ddlType) AndAlso CurrentCollaboration IsNot Nothing Then ddlType = CurrentCollaboration.DocumentType

        If DocSuiteContext.Current.IsResolutionEnabled AndAlso (ddlType.Eq(CollaborationDocumentType.D.ToString()) OrElse ddlType.Eq(CollaborationDocumentType.A.ToString())) Then

            If ResolutionEnv.MainDocumentOmissisEnable Then
                With uscDocumentoOmissis
                    .TreeViewCaption = "Documento Omissis"
                    .SignButtonEnabled = ProtocolEnv.IsFDQEnabled
                    .ButtonCopyResl.Visible = DocSuiteContext.Current.ResolutionEnv.CopyReslDocumentsEnabled
                    .ButtonCopyProtocol.Visible = ProtocolEnv.CopyProtocolDocumentsEnabled
                    .ButtonCopyUDS.Visible = ProtocolEnv.UDSEnabled
                End With
                tblDocumentoOmissis.Style.Remove("display")
            End If

            If ResolutionEnv.AttachmentOmissisEnable Then
                With uscAllegatiOmissis
                    .TreeViewCaption = "Allegati Omissis (parte integrante)"
                    .SignButtonEnabled = ProtocolEnv.IsFDQEnabled
                    .ButtonCopyResl.Visible = DocSuiteContext.Current.ResolutionEnv.CopyReslDocumentsEnabled
                    .ButtonCopyProtocol.Visible = ProtocolEnv.CopyProtocolDocumentsEnabled
                    .ButtonCopyUDS.Visible = ProtocolEnv.UDSEnabled
                End With
                tblAllegatiOmissis.Style.Remove("display")
            End If
        Else
            If tblDocumentoOmissis.Style("display") Is Nothing Then
                tblDocumentoOmissis.Style.Add("display", "none")
            End If
            If tblAllegatiOmissis.Style("display") Is Nothing Then
                tblAllegatiOmissis.Style.Add("display", "none")
            End If
        End If
    End Sub

    Private Sub InitializeAjax()

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscVisioneFirma)

        'si previene l'uso dell'header della pagina
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdSave, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdPreviewDocuments, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnVersioning, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCancella, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentType, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlSpecificDocumentType, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnNewDesk, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnResumeDesk, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRestituzione, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRifiuta, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnProtocolla, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnVisioneProtocolla, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInoltra, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRichiamo, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdVersioningManagement, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRefresh, pnlFilterPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        'si previene l'utilizzo della pagina
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdSave, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdPreviewDocuments, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnVersioning, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCancella, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentType, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlSpecificDocumentType, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentType, pnlDocumentUnitDraftEditor, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlSpecificDocumentType, pnlDocumentUnitDraftEditor, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentType, buttonPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlSpecificDocumentType, buttonPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnNewDesk, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnResumeDesk, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRestituzione, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRifiuta, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnProtocolla, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnVisioneProtocolla, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInoltra, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRichiamo, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdVersioningManagement, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRefresh, pnlMainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        'si previene l'utilizzo della pulsantiera
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCancella, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdSave, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdPreviewDocuments, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnVersioning, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnNewDesk, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnResumeDesk, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRestituzione, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRifiuta, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnProtocolla, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnVisioneProtocolla, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInoltra, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRichiamo, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdVersioningManagement, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRefresh, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)

        'AjaxManager.AjaxSettings.AddAjaxSetting(btnRefresh, uscDocumento.TreeViewControl)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscVisioneFirma, pnlRestituzioni)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscSettoriSegreterie, uscSettoriSegreterie)

        ' documento
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, lblDocumento)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscDocumento)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, cmdDocumentCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentCheckOut, lblDocumento)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentCheckOut, uscDocumento)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDocumento, cmdDocumentCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentUndoCheckOut, lblDocumento)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentUndoCheckOut, uscDocumento)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDocumento, cmdDocumentUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentCheckIn, lblDocumento)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentCheckIn, uscDocumento)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDocumento, cmdDocumentCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentCheckIn, cmdDocumentCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentCheckIn, cmdDocumentUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentCheckOut, cmdDocumentCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentCheckOut, cmdDocumentUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentUndoCheckOut, cmdDocumentCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentUndoCheckOut, cmdDocumentCheckOut)

        ' omissis
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscDocumentoOmissis)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, cmdDocumentOmissisCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentOmissisCheckOut, uscDocumentoOmissis)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDocumentoOmissis, cmdDocumentOmissisCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentOmissisUndoCheckOut, uscDocumentoOmissis)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDocumentoOmissis, cmdDocumentOmissisUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentOmissisCheckIn, uscDocumentoOmissis)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDocumentoOmissis, cmdDocumentOmissisCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentOmissisCheckIn, cmdDocumentOmissisCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentOmissisCheckIn, cmdDocumentOmissisUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentOmissisCheckOut, cmdDocumentOmissisCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentOmissisCheckOut, cmdDocumentOmissisUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentOmissisUndoCheckOut, cmdDocumentOmissisCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentOmissisUndoCheckOut, cmdDocumentOmissisCheckOut)

        ' allegati
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscAllegati)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, cmdAttachmentsCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsCheckOut, uscAllegati)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAllegati, cmdAttachmentsCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsUndoCheckOut, uscAllegati)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAllegati, cmdAttachmentsUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsCheckIn, uscAllegati)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAllegati, cmdAttachmentsCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsCheckIn, cmdAttachmentsCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsCheckIn, cmdAttachmentsUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsCheckOut, cmdAttachmentsCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsCheckOut, cmdAttachmentsUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsUndoCheckOut, cmdAttachmentsCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsUndoCheckOut, cmdAttachmentsCheckOut)

        ' allegati omissis
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscAllegatiOmissis)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, cmdAttachmentsOmissisCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsOmissisCheckOut, uscAllegatiOmissis)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAllegatiOmissis, cmdAttachmentsOmissisCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsOmissisUndoCheckOut, uscAllegatiOmissis)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAllegatiOmissis, cmdAttachmentsOmissisUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsOmissisCheckIn, uscAllegatiOmissis)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAllegatiOmissis, cmdAttachmentsOmissisCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsOmissisCheckIn, cmdAttachmentsOmissisCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsOmissisCheckIn, cmdAttachmentsOmissisUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsOmissisCheckOut, cmdAttachmentsOmissisCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsOmissisCheckOut, cmdAttachmentsOmissisUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsOmissisUndoCheckOut, cmdAttachmentsOmissisCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAttachmentsOmissisUndoCheckOut, cmdAttachmentsOmissisCheckOut)

        ' annessi
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscAnnexed)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, cmdAnnexedCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAnnexedCheckOut, uscAnnexed)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAnnexed, cmdAnnexedCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAnnexedUndoCheckOut, uscAnnexed)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAnnexed, cmdAnnexedUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAnnexedCheckIn, uscAnnexed)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAnnexed, cmdAnnexedCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAnnexedCheckIn, cmdAnnexedCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAnnexedCheckIn, cmdAnnexedUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAnnexedCheckOut, cmdAnnexedCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAnnexedCheckOut, cmdAnnexedUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAnnexedUndoCheckOut, cmdAnnexedCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAnnexedUndoCheckOut, cmdAnnexedCheckOut)

        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoOperazione, btnProtocolla)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoOperazione, btnRestituzione)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoOperazione, btnRifiuta)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoOperazione, btnVisioneProtocolla)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoOperazione, btnInoltra)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoOperazione, pnlRestituzioni)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoOperazione, pnlInoltro)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoOperazione, rblTipoOperazione)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnEditCollaborationData, txtDate)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnEditCollaborationData, alertDate)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnEditCollaborationData, txtObject)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnEditCollaborationData, txtNote)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnEditCollaborationData, btnEditCollaborationData)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnEditCollaborationData, cmdSave)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdSave, txtDate)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdSave, txtObject)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdSave, txtNote)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdSave, cmdSave)

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, txtObject)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, txtNote)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlInoltro)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rblPriority)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, txtDate)

        'AjaxManager.AjaxSettings.AddAjaxSetting(pnlInoltro, AjaxManager)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlRestituzioni)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscInoltro, pnlRestituzioni)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, cmdPreviewDocuments)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnRifiuta, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInoltra, buttonPanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, btnInoltra)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnExpandUDS, pnlUDSTitle, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnExpandUDS, uscUDS)

        If tEditCollaborationData.Visible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdSave, btnEditCollaborationData)
        End If

        If tAlertData.Visible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdSave, alertDate)
        End If

        AddHandler AjaxManager.AjaxRequest, AddressOf UserCollGestione_AjaxRequest
    End Sub

    Private Sub InitializeSignButtons()
        uscDocumento.SignButtonEnabled = ProtocolEnv.IsFDQEnabled
        uscAllegati.SignButtonEnabled = ProtocolEnv.IsFDQEnabled
        uscAnnexed.SignButtonEnabled = ProtocolEnv.IsFDQEnabled
    End Sub

    Private Sub InitializeInsVisioneFirma()
        Title &= Title2 & " - Inserimento"
        pnlVisioneFirma.Visible = True
        btnConferma.Visible = True
        btnConferma.Enabled = False
        btnMultiSign.Visible = False

        pnlRestituzioni.Visible = True
        uscSettoriSegreterie.ReadOnly = False
        txtRestituzioniOK.Text = "OK"
        uscRestituzioni.DataSource.Clear()
        uscRestituzioni.DataBind()

        txtObject.Focus()
        btnMail.Visible = False
        btnRefresh.Visible = False
        pnlMainPanel.SetDisplay(False)
    End Sub

    Private Sub InitializeInsProtocolloSegreteria()
        Title &= Title2 & " - Inserimento"
        btnMultiSign.Visible = False
        pnlRestituzioni.Visible = True
        uscSettoriSegreterie.ReadOnly = False

        pnlVisioneFirma.Visible = True
        uscVisioneFirma.ReadOnly = True
        txtDestinatarioOK.Text = "oo"
        uscVisioneFirma.AddCollaborationContact(ContactType.AdAmPerson, CommonInstance.UserDescription, CommonInstance.UserMail, DocSuiteContext.Current.User.FullUserName, "G", String.Empty, False)
        InitializeSegreterie(New Contact() With {.Code = DocSuiteContext.Current.User.FullUserName, .RoleUserIdRole = ""})

        btnConferma.Visible = True
        btnConferma.Enabled = False
        btnMail.Visible = False
        txtObject.Focus()
        pnlMainPanel.SetDisplay(False)
    End Sub

    Private Sub InitializeVisioneFirma()
        Title &= Title2 & " - Modifica"

        btnConferma.Visible = DocSuiteContext.Current.ProtocolEnv.CollaborationConfirmButtonEnabled
        btnCancella.Visible = IsProposer

        InitializeMainDocument(False, True)
        InitializeMainDocumentOmissis(True, True)
        InitializeAttachments(True, True)
        InitializeAttachmentsOmissis(True, True)
        InitializeAnnexed(True, True)
        btnMultiSign.Visible = False



        If Action.Eq(CollaborationSubAction.AllaVisioneFirma) AndAlso ProtocolEnv.CollaborationAttachmentEditable AndAlso
           ProtocolEnv.CollDocSignedNotEditable AndAlso AtLeastOneDocumentSigned Then
            InitializeEditButtonsDocumentUpload()
        End If

        InitDati(True)

        If Facade.CollaborationSignsFacade.HasNext(CurrentCollaboration.Id) Then
            pnlInoltro.Visible = False
            InitVisioneFirma(True, True, False, SignersEditEnabled)
        Else
            pnlInoltro.Visible = True
            InitInoltro()
            InitVisioneFirma(False, True, False, SignersEditEnabled)
        End If

        InitRestituzioniTabella(True)
        uscInoltro.ButtonSelectVisible = False
        uscInoltro.ButtonSelectDomainVisible = False
        uscInoltro.ButtonSelectOChartVisible = False
        uscInoltro.ButtonRoleVisible = False

        If CurrentCollaboration.IdStatus.Eq(CollaborationStatusType.IN.ToString()) Then
            If Action2.Eq(CollaborationMainAction.AllaVisioneFirma) Then
                InitializeMainDocument(True, True)
                If CheckModifyRight Then
                    InitializeMainDocument(True, True)
                    InitializeMainDocumentOmissis(True, False)
                    InitializeAttachments(True, False)
                    InitializeAttachmentsOmissis(True, False)
                    InitializeAnnexed(True, False)
                End If
                If CurrentCollaboration.CollaborationSigns.Count > 1 Then
                    btnConferma.Visible = CheckModifyRight

                    ' Inibisco la modifica
                    InitDati(False)

                    pnlInoltro.Visible = False
                    uscVisioneFirma.ReadOnly = Not CheckModifyRight
                    uscVisioneFirma.ButtonSelectVisible = False
                    uscVisioneFirma.ButtonSelectDomainVisible = SignersEditEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.AbilitazioneRubricaDomain
                    uscVisioneFirma.ButtonSelectOChartVisible = False
                    uscVisioneFirma.ButtonDeleteVisible = CheckModifyRight
                    uscVisioneFirma.ButtonRoleVisible = CheckModifyRight
                    uscVisioneFirma.EnableCheck = SignersEditEnabled OrElse (IsFromResolution AndAlso CheckModifyRight)

                    uscRestituzioni.ButtonSelectVisible = False
                    uscRestituzioni.ButtonDeleteVisible = False
                    uscRestituzioni.ReadOnly = True

                    uscSettoriSegreterie.ReadOnly = Not SignersEditEnabled
                    uscSettoriSegreterie.EditableCheck = SignersEditEnabled

                Else
                    btnMultiSign.Visible = ProtocolEnv.EnableMultiSign
                    uscVisioneFirma.ReadOnly = Not SignersEditEnabled
                    uscVisioneFirma.ButtonSelectDomainVisible = SignersEditEnabled AndAlso ProtocolEnv.CollManualContactEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.AbilitazioneRubricaDomain
                    uscVisioneFirma.ButtonSelectOChartVisible = CurrentCollaboration.SignCount.GetValueOrDefault(0) > 1 AndAlso ProtocolEnv.CollManualContactEnabled

                    uscVisioneFirma.ButtonDeleteVisible = CheckModifyRight OrElse SignersEditEnabled
                    uscVisioneFirma.ButtonRoleVisible = CheckModifyRight OrElse SignersEditEnabled
                    uscVisioneFirma.EnableCheck = SignersEditEnabled

                End If
            End If

        ElseIf CurrentCollaboration.IdStatus.Eq(CollaborationStatusType.DL.ToString()) Then
            ' Se la collaborazione è partita da tavoli si aprono 2 strade.
            ' 1) Apertuna di un nuovo tavolo con i SOLI documenti NON firmati
            ' 2) Riapertura del tavolo precedente
            If DeskSource IsNot Nothing Then
                btnNewDesk.Visible = True
                btnResumeDesk.Visible = True
                btnConferma.Visible = False
            Else
                btnConferma.Text = "Conferma e Reinvia"
            End If
            pnlRifiuto.Visible = True
            InitializeMainDocument(True, True)
        End If
        'Collaborazione per cui l'utente corrente è il proponente e ma la collaborazione è in stato di bozza



        If uscAllegati.TreeViewControl.Nodes(0).Nodes IsNot Nothing AndAlso uscAllegati.TreeViewControl.Nodes(0).Nodes.Count > 1 Then
            For Each node As RadTreeNode In uscAllegati.TreeViewControl.Nodes(0).Nodes
                node.Selected = False
            Next
        End If

        ValidateExistRecipients()
    End Sub

    Private Sub InitializeVisionareFirmare()

        btnCancella.Visible = IsFirstActiveSigner

        btnMail.Visible = False
        Title &= Title2
        pnlTipoOperazione.Visible = True

        InitializeMainDocument(True, True)
        InitializeMainDocumentOmissis(True)
        InitializeAttachments(True)
        InitializeAttachmentsOmissis(True)
        InitializeAnnexed(True)

        If Action.Eq(CollaborationSubAction.DaVisionareFirmare) AndAlso ProtocolEnv.CollaborationAttachmentEditable _
            AndAlso ProtocolEnv.CollDocSignedNotEditable AndAlso AtLeastOneDocumentSigned Then
            InitializeEditButtonsDocumentUpload()
        End If

        InitVisioneFirma(False, True)
        InitRestituzioniTabella(True)

        InitDati(True)

        rblTipoOperazione.SelectedValue = "A"

        If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.U.ToString()) Then
            btnRestituzione.Visible = False
        End If

        If Facade.CollaborationSignsFacade.HasNext(CurrentCollaboration.Id) Then
            rblTipoOperazione.Items.Remove(rblTipoOperazione.Items.FindByValue("P"))
            rblTipoOperazione.Items.Remove(rblTipoOperazione.Items.FindByValue("A"))

            btnRestituzione.Visible = False
            btnVisioneProtocolla.Visible = False
            rblTipoOperazione.SelectedValue = "I" ' Inoltro

            Dim nextSignUsers As IList(Of CollaborationSign) = Facade.CollaborationSignsFacade.GetNext(CurrentCollaboration.Id)
            Dim nextUser As CollaborationSign = nextSignUsers.OrderBy(Function(o) o.Incremental).First()
            Dim signName As String = Facade.CollaborationSignsFacade.GetCollaborationSignDescription(nextUser.SignName, nextUser.IsAbsent)
            uscInoltro.AddCollaborationContact(ContactType.AdAmPerson, signName, nextUser.SignEMail, nextUser.SignUser, String.Empty, String.Empty, False)

            uscInoltro.ButtonSelectVisible = False
            uscInoltro.ButtonSelectDomainVisible = False
            uscInoltro.ButtonSelectOChartVisible = False
            uscInoltro.ButtonRoleVisible = False
        Else
            If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.U.ToString()) Then
                rblTipoOperazione.Items.Remove(rblTipoOperazione.Items.FindByValue("P"))
                rblTipoOperazione.Items.Remove(rblTipoOperazione.Items.FindByValue("A"))
                rblTipoOperazione.SelectedValue = "I" ' Inoltro
            Else
                rblTipoOperazione.Items.FindByValue("A").Text = CollaborationFacade.GetModuleName(ddlDocumentType.SelectedValue, "2")
                If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.S.ToString()) Then
                    rblTipoOperazione.Items.FindByValue("A").Text = ProtocolEnv.DocumentSeriesName
                End If

                If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.UDS.ToString()) OrElse Integer.TryParse(ddlDocumentType.SelectedValue, 0) Then
                    rblTipoOperazione.Items.FindByValue("A").Text = "Agli Archivi"
                End If

                If Facade.ContainerFacade.HasInsertOrProposalRights() Then
                    rblTipoOperazione.Items.FindByValue("P").Text = CollaborationFacade.GetModuleName(ddlDocumentType.SelectedValue, "3")
                    If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.S.ToString()) Then
                        rblTipoOperazione.Items.FindByValue("P").Text = ProtocolEnv.DocumentSeriesName
                    End If

                    If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.UDS.ToString()) OrElse Integer.TryParse(ddlDocumentType.SelectedValue, 0) Then
                        rblTipoOperazione.Items.FindByValue("P").Text = "Archivia"
                    End If
                Else
                    rblTipoOperazione.Items.Remove(rblTipoOperazione.Items.FindByValue("P"))
                End If

                btnRestituzione.Text = CollaborationFacade.GetModuleName(ddlDocumentType.SelectedValue, "2")
                If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.S.ToString()) Then
                    btnRestituzione.Text = ProtocolEnv.DocumentSeriesName
                End If

                If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.UDS.ToString()) OrElse Integer.TryParse(ddlDocumentType.SelectedValue, 0) Then
                    btnRestituzione.Text = "Agli Archivi"
                End If

                btnVisioneProtocolla.Text = CollaborationFacade.GetModuleName(ddlDocumentType.SelectedValue, "3")
                If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.S.ToString()) Then
                    btnVisioneProtocolla.Text = ProtocolEnv.DocumentSeriesName
                End If

                If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.UDS.ToString()) OrElse Integer.TryParse(ddlDocumentType.SelectedValue, 0) Then
                    btnVisioneProtocolla.Text = "Archivia"
                End If
            End If
        End If

        If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.D.ToString()) Then
            btnRestituzione.Enabled = btnRestituzione.Enabled AndAlso Not IsFromResolution
            pnlTipoOperazione.Visible = btnRestituzione.Enabled
            If IsFromResolution Then
                InitializeAnnexed(True)
            End If
        End If
        SetVisioneFirma()
        ValidateExistRecipients()
    End Sub
    Private Sub InitializeDelegaVisionareFirmare()
        btnCancella.Visible = IsFirstActiveSigner
        btnMail.Visible = True
        Title &= Title2
        pnlTipoOperazione.Visible = True

        InitializeMainDocument(True, True)
        InitializeMainDocumentOmissis(True)
        InitializeAttachments(True)
        InitializeAttachmentsOmissis(True)
        InitializeAnnexed(True)

        If Action.Eq(CollaborationSubAction.DaFirmareInDelega) AndAlso ProtocolEnv.CollaborationAttachmentEditable _
            AndAlso ProtocolEnv.CollDocSignedNotEditable AndAlso AtLeastOneDocumentSigned Then
            InitializeEditButtonsDocumentUpload()
        End If

        InitVisioneFirma(False, True)
        InitRestituzioniTabella(True)

        InitDati(True)

        rblTipoOperazione.SelectedValue = "A"

        If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.U.ToString()) Then
            btnRestituzione.Visible = False
        End If

        If Facade.CollaborationSignsFacade.HasNext(CurrentCollaboration.Id) Then
            rblTipoOperazione.Items.Remove(rblTipoOperazione.Items.FindByValue("P"))
            rblTipoOperazione.Items.Remove(rblTipoOperazione.Items.FindByValue("A"))

            btnRestituzione.Visible = False
            btnVisioneProtocolla.Visible = False
            rblTipoOperazione.SelectedValue = "I" ' Inoltro

            Dim nextSignUsers As IList(Of CollaborationSign) = Facade.CollaborationSignsFacade.GetNext(CurrentCollaboration.Id)
            Dim nextUser As CollaborationSign = nextSignUsers.OrderBy(Function(o) o.Incremental).First()
            Dim signName As String = Facade.CollaborationSignsFacade.GetCollaborationSignDescription(nextUser.SignName, nextUser.IsAbsent)
            uscInoltro.AddCollaborationContact(ContactType.AdAmPerson, signName, nextUser.SignEMail, nextUser.SignUser, String.Empty, String.Empty, False)

            uscInoltro.ButtonSelectVisible = False
            uscInoltro.ButtonSelectDomainVisible = False
            uscInoltro.ButtonSelectOChartVisible = False
            uscInoltro.ButtonRoleVisible = False
        Else
            If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.U.ToString()) Then
                rblTipoOperazione.Items.Remove(rblTipoOperazione.Items.FindByValue("P"))
                rblTipoOperazione.Items.Remove(rblTipoOperazione.Items.FindByValue("A"))
                rblTipoOperazione.SelectedValue = "I" ' Inoltro
            Else
                rblTipoOperazione.Items.FindByValue("A").Text = CollaborationFacade.GetModuleName(ddlDocumentType.SelectedValue, "2")
                If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.S.ToString()) Then
                    rblTipoOperazione.Items.FindByValue("A").Text = ProtocolEnv.DocumentSeriesName
                End If

                If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.UDS.ToString()) OrElse Integer.TryParse(ddlDocumentType.SelectedValue, 0) Then
                    rblTipoOperazione.Items.FindByValue("A").Text = "Agli Archivi"
                End If

                If Facade.ContainerFacade.HasInsertOrProposalRights() Then
                    rblTipoOperazione.Items.FindByValue("P").Text = CollaborationFacade.GetModuleName(ddlDocumentType.SelectedValue, "3")
                    If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.S.ToString()) Then
                        rblTipoOperazione.Items.FindByValue("P").Text = ProtocolEnv.DocumentSeriesName
                    End If

                    If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.UDS.ToString()) OrElse Integer.TryParse(ddlDocumentType.SelectedValue, 0) Then
                        rblTipoOperazione.Items.FindByValue("P").Text = "Archivia"
                    End If
                Else
                    rblTipoOperazione.Items.Remove(rblTipoOperazione.Items.FindByValue("P"))
                End If

                btnRestituzione.Text = CollaborationFacade.GetModuleName(ddlDocumentType.SelectedValue, "2")
                If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.S.ToString()) Then
                    btnRestituzione.Text = ProtocolEnv.DocumentSeriesName
                End If
                If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.UDS.ToString()) OrElse Integer.TryParse(ddlDocumentType.SelectedValue, 0) Then
                    btnRestituzione.Text = "Agli Archivi"
                End If

                btnVisioneProtocolla.Text = CollaborationFacade.GetModuleName(ddlDocumentType.SelectedValue, "3")
                If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.S.ToString()) Then
                    btnVisioneProtocolla.Text = ProtocolEnv.DocumentSeriesName
                End If

                If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.UDS.ToString()) OrElse Integer.TryParse(ddlDocumentType.SelectedValue, 0) Then
                    btnVisioneProtocolla.Text = "Archivia"
                End If
            End If
        End If

        If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.D.ToString()) Then
            btnRestituzione.Enabled = btnRestituzione.Enabled AndAlso Not IsFromResolution
            pnlTipoOperazione.Visible = btnRestituzione.Enabled
            If IsFromResolution Then
                InitializeAnnexed(True)
            End If
        End If
        SetVisioneFirma()
        ValidateExistRecipients()
    End Sub
    Private Sub InitializeCollaborationTasksManager()

        Title &= Title2

        btnMultiSign.Visible = False
        InitializeMainDocument(False, True)
        InitializeAttachments(False)
        InitializeAnnexed(False)
        InitializeMainDocumentOmissis(False)
        InitializeAttachmentsOmissis(False)

        If (Action.Eq(CollaborationSubAction.AlProtocolloSegreteria) OrElse Action.Eq(CollaborationSubAction.AttivitaInCorso)) AndAlso ProtocolEnv.CollaborationAttachmentEditable _
            AndAlso ProtocolEnv.CollDocSignedNotEditable AndAlso AtLeastOneDocumentSigned Then
            InitializeEditButtonsDocumentUpload()
        End If

        InitVisioneFirma(False, True, False, If(Action.Eq(CollaborationSubAction.AttivitaInCorso), SignersEditEnabled, False))
        InitRestituzioniTabella(False)
        InitDati(False)

        Dim temp As IList(Of String) = Facade.RoleUserFacade.GetAccounts(DocSuiteContext.Current.User.FullUserName)

        If IsProposer OrElse IsFirstActiveSigner Then
            btnCancella.Visible = True
        End If

        Select Case Action
            Case CollaborationSubAction.AlProtocolloSegreteria
                btnMail.Visible = False


            Case CollaborationSubAction.AttivitaInCorso
                If IsSecretaryEnable Then
                    btnCancella.Visible = True
                End If

                For Each node As RadTreeNode In uscAllegati.TreeViewControl.Nodes(0).Nodes
                    node.Selected = False
                Next

                If DocumentEditEnabled Then
                    InitializeMainDocument(True, True)
                    InitializeAttachments(True)
                    InitializeAnnexed(True)
                    InitializeMainDocumentOmissis(True)
                    InitializeAttachmentsOmissis(False)
                End If

                If SignersEditEnabled Then
                    btnConferma.Visible = True
                    uscSettoriSegreterie.ReadOnly = False
                    uscSettoriSegreterie.EditableCheck = True
                End If

                ' Abilito il button "Al protocollo"/"Inoltra" se il dirigente ha firmato, ma si è dimenticato di mandare avanti la collaborazione
                Dim hasNext As Boolean = Facade.CollaborationSignsFacade.HasNext(CurrentCollaboration.Id)
                If hasNext Then
                    btnInoltra.Visible = True
                    btnInoltra.Enabled = False
                Else
                    btnRestituzione.Visible = True
                    btnRestituzione.Enabled = False
                End If

                btnRestituzione.Text = CollaborationFacade.GetModuleName(ddlDocumentType.SelectedValue, "2")
                If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.S.ToString()) Then
                    btnRestituzione.Text = ProtocolEnv.DocumentSeriesName
                End If
                If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.UDS.ToString()) OrElse Integer.TryParse(ddlDocumentType.SelectedValue, 0) Then
                    btnRestituzione.Text = "Agli Archivi"
                End If

                If CurrentCollaboration.DocumentType.Eq(CollaborationDocumentType.U.ToString()) Then
                    btnRestituzione.Visible = False
                End If

                If Facade.CollaborationSignsFacade.IsCollaborationSignedByActiveSigner(CurrentCollaboration.Id) Then
                    If hasNext Then
                        btnInoltra.Enabled = True
                    Else
                        btnRestituzione.Enabled = True
                    End If
                End If

                If DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates IsNot Nothing AndAlso DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates.Managers IsNot Nothing AndAlso CurrentCollaboration.DocumentType.Eq(CollaborationDocumentType.D.ToString()) Then
                    btnAbsence.Visible = Facade.CollaborationUsersFacade.IsCollaborationDirectorSecretary(CurrentCollaboration)
                    btnAbsence.Enabled = Facade.CollaborationSignsFacade.GetManagerSigners(CurrentCollaboration).Any(Function(s) (Not s.IsAbsent.HasValue OrElse Not s.IsAbsent.Value) AndAlso s.Incremental >= CurrentCollaborationSign.Incremental)
                End If
        End Select

        ValidateExistRecipients()
    End Sub

    Private Sub InitializeProtocollareGestire()
        btnCancella.Visible = True

        Title &= Title2
        btnProtocolla.Visible = True
        If CurrentCollaboration.DocumentType.Eq(CollaborationDocumentType.U.ToString()) Then
            btnProtocolla.Visible = False
        End If

        btnMail.Visible = False
        btnMultiSign.Visible = False

        InitializeMainDocument(False, False)
        InitializeMainDocumentOmissis(True)
        InitializeAttachments(True)
        InitializeAttachmentsOmissis(True)
        InitializeAnnexed(True)

        If Action.Eq(CollaborationSubAction.DaProtocollareGestire) AndAlso ProtocolEnv.CollaborationAttachmentEditable _
            AndAlso ProtocolEnv.CollDocSignedNotEditable AndAlso AtLeastOneDocumentSigned Then
            InitializeEditButtonsDocumentUpload()
        End If

        InitVisioneFirma(False, True)
        InitRestituzioniTabella(False)
        InitDati(False)

        Dim collvers As CollaborationVersioning = CurrentCollaboration.GetDocumentVersioning()(0)
        Dim signed As BiblosDocumentInfo = Facade.CollaborationVersioningFacade.GetBiblosDocument(collvers)
        If PopUpDocumentNotSignedAlertEnabled AndAlso Not signed.IsSigned Then
            AjaxAlert("ATTENZIONE, il Documento non è Firmato Digitalmente")
        End If

        btnProtocolla.Text = CollaborationFacade.GetModuleName(ddlDocumentType.SelectedValue, "3")
        If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.S.ToString()) Then
            btnProtocolla.Text = ProtocolEnv.DocumentSeriesName
        End If

        If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.UDS.ToString()) OrElse Integer.TryParse(ddlDocumentType.SelectedValue, 0) Then
            btnProtocolla.Text = "Archivia"
        End If

        'Visualizzazione tip. operazione per inoltro da parte della segreteria
        pnlTipoOperazione.Visible = False
        rblTipoOperazione.Items.Remove(rblTipoOperazione.Items.FindByValue("P"))
        rblTipoOperazione.Items.Remove(rblTipoOperazione.Items.FindByValue("R"))
        rblTipoOperazione.Items.FindByValue("A").Text = "Segreteria"
        rblTipoOperazione.Items.FindByValue("A").Value = "S" ' Segreteria
        rblTipoOperazione.SelectedValue = "S"
        rblTipoOperazione.Items.FindByValue("I").Value = "IS" ' Procedura di inoltro da segreteria

        ValidateExistRecipients()
    End Sub

    Private Sub InitializeProtocollatiGestiti()
        Title &= Title2
        btnMail.Visible = False
        btnMultiSign.Visible = False

        InitializeMainDocument(False, True)
        InitializeMainDocumentOmissis(False)
        InitializeAttachments(False)
        InitializeAttachmentsOmissis(False)
        InitializeAnnexed(False)

        If Action.Eq(CollaborationSubAction.ProtocollatiGestiti) AndAlso ProtocolEnv.CollaborationAttachmentEditable _
            AndAlso ProtocolEnv.CollDocSignedNotEditable AndAlso AtLeastOneDocumentSigned Then
            InitializeEditButtonsDocumentUpload()
        End If

        InitVisioneFirma(False, True)
        InitDati(False)
        btnConferma.Visible = False

        btnCancella.Visible = True

        ValidateExistRecipients()
    End Sub

    Private Sub InitializeActions()
        btnConferma.Visible = False
        btnRefresh.Visible = DocSuiteContext.Current.ProtocolEnv.CollaborationRefreshButtonVisible
        btnRefresh.OnClientClick = $"window.location.href='{Request.Url.ToString()}';"
        Select Case Action
            Case CollaborationSubAction.InserimentoAllaVisioneFirma
                InitializeInsVisioneFirma()

            Case CollaborationSubAction.InserimentoAlProtocolloSegreteria
                InitializeInsProtocolloSegreteria()

            Case CollaborationSubAction.AllaVisioneFirma
                If CurrentCollaboration Is Nothing Then
                    Throw New DocSuiteException("Errore in Ricerca Registrazione", String.Format("Impossibile trovare la registrazione [{0}].", CollaborationId))
                End If
                InitializeVisioneFirma()

            Case CollaborationSubAction.DaVisionareFirmare
                If CurrentCollaboration Is Nothing Then
                    Throw New DocSuiteException("Errore in Ricerca Registrazione", String.Format("Impossibile trovare la registrazione [{0}].", CollaborationId))
                End If

                If CurrentCollaboration.Resolution IsNot Nothing AndAlso CurrentCollaboration.Resolution.Id <> Nothing Then
                    Throw New DocSuiteException("Collaborazione n. " & CurrentCollaboration.Id, "Collaborazione (Atto) già protocollata")

                ElseIf CurrentCollaboration.DocumentType = "P" AndAlso CurrentCollaboration.Number IsNot Nothing AndAlso CurrentCollaboration.Year IsNot Nothing Then
                    Throw New DocSuiteException("Collaborazione n. " & CurrentCollaboration.Id, "Collaborazione (Protocollo) già protocollata")

                ElseIf (Not IsSignatory()) Then 'se nn sono il firmatario lancio un messaggio di errore
                    Throw New DocSuiteException("Collaborazione n. " & CurrentCollaboration.Id, "Non hai i permessi per visionare questa collaborazione, potresti non essere il firmatario")

                ElseIf CurrentCollaboration.IdStatus.Eq(CollaborationStatusType.DP.ToString()) Then
                    'Se sono nello stato Da Protocollare non posso ritornare allo stato Da Visionare/Firmare
                    Throw New DocSuiteException("Collaborazione n. " & CurrentCollaboration.Id, "Collaborazione è in stato da protocollare")

                End If
                SetDocumentSignPdfManage()
                InitializeVisionareFirmare()

            Case CollaborationSubAction.DaFirmareInDelega
                If CurrentCollaboration Is Nothing Then
                    Throw New DocSuiteException("Errore in Ricerca Registrazione", String.Format("Impossibile trovare la registrazione [{0}].", CollaborationId))
                End If

                If CurrentCollaboration.Resolution IsNot Nothing AndAlso CurrentCollaboration.Resolution.Id <> Nothing Then
                    Throw New DocSuiteException("Collaborazione n. " & CurrentCollaboration.Id, "Collaborazione (Atto) già protocollata")

                ElseIf CurrentCollaboration.Number IsNot Nothing AndAlso CurrentCollaboration.Year IsNot Nothing Then
                    Throw New DocSuiteException("Collaborazione n. " & CurrentCollaboration.Id, "Collaborazione (Protocollo) già protocollata")

                ElseIf (Not IsDelegateSignatory()) Then 'se nn sono il firmatario lancio un messaggio di errore
                    Throw New DocSuiteException("Collaborazione n. " & CurrentCollaboration.Id, "Non hai la delega per visionare questa collaborazione, potresti non essere il firmatario")

                ElseIf CurrentCollaboration.IdStatus.Eq(CollaborationStatusType.DP.ToString()) Then
                    'Se sono nello stato Da Protocollare non posso ritornare allo stato Da Visionare/Firmare
                    Throw New DocSuiteException("Collaborazione n. " & CurrentCollaboration.Id, "Collaborazione è in stato da protocollare")

                End If
                SetDocumentSignPdfManage()
                InitializeDelegaVisionareFirmare()

            Case CollaborationSubAction.AlProtocolloSegreteria, CollaborationSubAction.AttivitaInCorso
                If CurrentCollaboration Is Nothing Then
                    Throw New DocSuiteException("Errore in Ricerca Registrazione", String.Format("Impossibile trovare la registrazione [{0}].", CollaborationId))
                End If
                InitializeCollaborationTasksManager()

            Case CollaborationSubAction.DaProtocollareGestire ' Da Protocollare/Gestire   
                If CurrentCollaboration Is Nothing Then
                    Throw New DocSuiteException("Errore in Ricerca Registrazione", String.Format("Impossibile trovare la registrazione [{0}].", CollaborationId))
                End If

                If CurrentCollaboration.Resolution IsNot Nothing AndAlso CurrentCollaboration.Resolution IsNot Nothing AndAlso CurrentCollaboration.Resolution.Id > 0 Then
                    Dim message As String = String.Format("Collaborazione n. {0} Collaborazione (Atto) già protocollata", CurrentCollaboration.Id)
                    Throw New DocSuiteException(message)
                End If

                If CurrentCollaboration.DocumentType = "P" AndAlso CurrentCollaboration.Number IsNot Nothing AndAlso CurrentCollaboration.Year IsNot Nothing Then
                    Dim message As String = String.Format("Collaborazione n. {0} Collaborazione (Protocollo) già protocollata", CurrentCollaboration.Id)
                    Throw New DocSuiteException(message)
                End If
                InitializeProtocollareGestire()

            Case CollaborationSubAction.ProtocollatiGestiti
                If CurrentCollaboration Is Nothing Then
                    Throw New DocSuiteException("Errore in Ricerca Registrazione", String.Format("Impossibile trovare la registrazione [{0}].", CollaborationId))
                End If
                InitializeProtocollatiGestiti()
        End Select
    End Sub


    Private Sub UpdateWorkflowStep(collaborationVersioning As CollaborationVersioning)
        If CurrentWorkflowInstance IsNot Nothing AndAlso CurrentWorkflowActivity IsNot Nothing Then

            Dim document As BiblosDocumentInfo = Facade.CollaborationVersioningFacade.GetLastVersionDocumentByIncremental(CurrentCollaboration.Id, collaborationVersioning.CollaborationIncremental)
            Dim currentUserSigned As Boolean = Facade.CollaborationSignsFacade.IsCollaborationSignedByActiveSigner(CurrentCollaboration.Id)
            Dim workflowNotify As WorkflowNotify = New WorkflowNotify(CurrentWorkflowActivity.UniqueId) With {
                    .WorkflowName = CurrentWorkflowInstance.WorkflowRepository.Name}

            Dim dsw_p_SignerModel As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL, CurrentWorkflowActivity.UniqueId)
            Dim dsw_p_SignerPosition As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_POSITION, CurrentWorkflowActivity.UniqueId)

            If dsw_p_SignerModel IsNot Nothing AndAlso dsw_p_SignerPosition IsNot Nothing Then
                Dim collaborationSignerModels As List(Of CollaborationSignerWorkflowModel) = JsonConvert.DeserializeObject(Of List(Of CollaborationSignerWorkflowModel))(dsw_p_SignerModel.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)

                Dim currentSignPosition As Integer = CurrentCollaboration.CollaborationSigns.Where(Function(x) Convert.ToBoolean(x.IsActive)).Single().Incremental

                If dsw_p_SignerPosition.ValueInt.Value + 1 <> currentSignPosition Then
                    Throw New Exception($"Errore critico nelle strutture interne del workflow rispetto alla collaborazione [dsw_p_SignerPosition = {dsw_p_SignerPosition.ValueInt.Value}, CollaborationSignsIncremental = {currentSignPosition}]")
                End If

                If document.IsSigned Then
                    Dim documentModel As CollaborationDocumentWorkflowModel = New CollaborationDocumentWorkflowModel() With {
                            .DocumentName = document.Name,
                            .IdDocument = document.DocumentId,
                            .SignDate = DateTimeOffset.UtcNow}
                    collaborationSignerModels = WorkflowBuildApprovedModel(currentSignPosition, True, collaborationSignerModels)
                    Dim currentModel As CollaborationSignerWorkflowModel = collaborationSignerModels(CType(dsw_p_SignerPosition.ValueInt.Value, Integer))
                    If Not currentModel.SignedDocuments.Any(Function(x) x.IdDocument.Equals(document.DocumentId)) Then
                        currentModel.SignedDocuments.Add(documentModel)
                    End If
                End If

                dsw_p_SignerModel.ValueString = JsonConvert.SerializeObject(collaborationSignerModels, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
                workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL, New WorkflowArgument() With {
                                                       .Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL,
                                                       .PropertyType = ArgumentType.Json,
                                                       .ValueString = dsw_p_SignerModel.ValueString})
                If currentUserSigned AndAlso CurrentSigner.IsRequired Then
                    workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_SIGNED, New WorkflowArgument() With {
                                                      .Name = WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_SIGNED,
                                                      .PropertyType = ArgumentType.PropertyBoolean,
                                                      .ValueBoolean = True})
                    MasterDocSuite.WizardActionColumn.SetDisplay(True)
                    AjaxManager.ResponseScripts.Add("toCompleteStep();")
                End If
                Dim webApiHelper As WebAPIHelper = New WebAPIHelper()
                If Not WebAPIImpersonatorFacade.ImpersonateSendRequest(webApiHelper, workflowNotify, DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, DocSuiteContext.Current.CurrentTenant.OriginalConfiguration) Then
                    Throw New Exception("UpdateWorkflowStep is not correctly evaluated from WebAPI. See specific error in WebAPI logger")
                End If
            End If
        End If
    End Sub

    Private Sub InitializeWorkflowWizard()
        If CurrentCollaboration IsNot Nothing AndAlso CurrentCollaboration.IdWorkflowInstance.HasValue AndAlso Not ProtocolEnv.WorkflowManagerEnabled Then
            Throw New DocSuiteException("Non è possibile gestire una collaborazione di tipo workflow se il parametro WorkflowManagerEnabled è disabilitato")
        End If

        If IsWorkflowOperation AndAlso CurrentCollaboration IsNot Nothing AndAlso CurrentCollaboration.IdWorkflowInstance.HasValue AndAlso Action.Eq(CollaborationSubAction.DaVisionareFirmare) Then

            MasterDocSuite.WorkflowWizardRow.Visible = True

            Dim collaborationApprovedOrRefused As Boolean = IsCollaborationApprovedOrRefused()
            Dim collaborationSignStep As RadWizardStep = New RadWizardStep()
            collaborationSignStep.ID = "CollaborationSignDocuments"
            collaborationSignStep.Title = If(CurrentSigner.IsRequired, Entity.Workflows.WorkflowActivityType.CollaborationSign.GetDescription(), "Approva collaborazione")
            collaborationSignStep.ToolTip = If(CurrentSigner.IsRequired, Entity.Workflows.WorkflowActivityType.CollaborationSign.GetDescription(), "Approva collaborazione")
            collaborationSignStep.Active = Not collaborationApprovedOrRefused
            collaborationSignStep.Enabled = Not collaborationApprovedOrRefused
            MasterDocSuite.WorkflowWizardControl.WizardSteps.Add(collaborationSignStep)

            Dim sendCompleteStep As RadWizardStep = New RadWizardStep()
            sendCompleteStep.ID = "SendComplete"
            sendCompleteStep.Title = "Concludi attività"
            sendCompleteStep.ToolTip = "Concludi attività"
            sendCompleteStep.Active = collaborationApprovedOrRefused
            sendCompleteStep.Enabled = collaborationApprovedOrRefused
            MasterDocSuite.WorkflowWizardControl.WizardSteps.Add(sendCompleteStep)
            MasterDocSuite.WizardActionColumn.Visible = True
            MasterDocSuite.WizardActionColumn.SetDisplay(collaborationApprovedOrRefused)
        End If
    End Sub

    ''' <summary> Inizializzazione </summary>
    ''' <remarks> Gestisce la logica delle view in base allo stato del workflow </remarks>
    Private Sub Initialize()
        ' Queste inizializzazioni sono controllate nel setter anche dalla ProtocolEnv.
        uscVisioneFirma.ButtonSelectDomainVisible = ProtocolEnv.CollManualContactEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.AbilitazioneRubricaDomain
        uscVisioneFirma.ButtonSelectVisible = False
        uscRestituzioni.ButtonSelectVisible = False

        If ProtocolEnv.HideManualRecipient Then
            lblOtherRecipients.Visible = False
            uscRestituzioni.Visible = False
        End If

        InitializeSignButtons()
        Title = String.Format("Collaborazione {0}- ", If(CurrentCollaboration IsNot Nothing, String.Format("n. {0} ", CurrentCollaboration.Id), ""))

        trSpecificDocumentType.Visible = ProtocolEnv.CollaborationTemplateRequired
        rfvSpecificDocumentType.Enabled = ProtocolEnv.CollaborationTemplateRequired
        BindDdlDocumentType()

        If String.IsNullOrEmpty(uscVisioneFirma.TemplateName) AndAlso Not String.IsNullOrEmpty(DefaultTemplateId) Then
            Dim selectedItem As DropDownListItem = ddlDocumentType.Items.SingleOrDefault(Function(f) f.Attributes("TemplateId") = DefaultTemplateId)
            If selectedItem IsNot Nothing Then
                ddlDocumentType.SelectedValue = selectedItem.Value
                selectedItem.Selected = True
                uscVisioneFirma.TemplateName = selectedItem.Text
            End If
        End If

        Dim docType As String = Request.QueryString("docType")
        If Not String.IsNullOrEmpty(docType) Then
            ddlDocumentType.SelectedValue = docType
        End If

        Dim doc As String = Request.QueryString("Document")
        If Not String.IsNullOrEmpty(doc) Then
            ddlDocumentType.SelectedValue = doc
        End If

        '' Set documento di default da parametro. Viene utilizzato in Inserimento di Collaborazione o in pagine in cui non è forzato via queryString (docType o Document).
        If String.IsNullOrEmpty(ddlDocumentType.SelectedValue) AndAlso Not String.IsNullOrEmpty(ProtocolEnv.CollaborationTipologiaDefault) Then
            '' Controllo formale del parametro inserito in fase di configurazione
            For Each val As String In [Enum].GetNames(GetType(CollaborationDocumentType))
                If String.Compare(val, ProtocolEnv.CollaborationTipologiaDefault, True) = 0 Then
                    ddlDocumentType.SelectedValue = val
                    Exit For
                End If
            Next
        End If

        'Se uoia 
        If FromCollaboratinoUoia Then
            GenerateTemplateUoia()
        End If

        cmdSave.Visible = False
        btnMultiSign.Visible = ProtocolEnv.EnableMultiSign
        btnRichiamo.Visible = False

        tAlertData.Visible = DocSuiteContext.Current.ProtocolEnv.CollaborationManagementExpired
        lblMemorandumDate.Text = If(DocSuiteContext.Current.ProtocolEnv.CollaborationManagementExpired, "Data scadenza: ", "Data promemoria: ")

        pnlUDS.SetDisplay(False)

        tEditCollaborationData.Visible = False

        If Action.Eq(CollaborationSubAction.InserimentoAllaVisioneFirma) OrElse
            Action.Eq(CollaborationSubAction.InserimentoAlProtocolloSegreteria) OrElse
            Action.Eq(CollaborationSubAction.AttivitaInCorso) OrElse
            Action.Eq(CollaborationSubAction.DaProtocollareGestire) Then

            InitializeVersioningCommands(False)
            cmdPreviewDocuments.Visible = False
        Else
            InitializeVersioningCommands(True)
            cmdPreviewDocuments.Visible = True
            cmdPreviewDocuments.OnClientClick = GetPreviewOnClientClick()
        End If

        If CurrentCollaboration IsNot Nothing Then
            btnVersioning.Visible = True
        End If

        InitializeActions()
        InitializeDocumentsState()
        InitializeWorkflowCollaboration()

        If (Action.Eq(CollaborationSubAction.InserimentoAllaVisioneFirma) OrElse
                Action.Eq(CollaborationSubAction.InserimentoAlProtocolloSegreteria)) AndAlso Not FromCollaboratinoUoia Then
            DdlDocumentTypeSelectedIndexChanged(ddlDocumentType, New EventArgs())
        End If

        If FromCollaboratinoUoia OrElse InsertFromWorkflow Then
            pnlMainPanel.SetDisplay(True)
            btnConferma.Enabled = True
        End If

    End Sub

    Private Sub InitializeWorkflowCollaboration()
        If CurrentCollaboration Is Nothing OrElse Not CurrentCollaboration.IdWorkflowInstance.HasValue Then
            Exit Sub
        End If

        Dim workflowCollaborationApprovedStatus As WorkflowCollaborationSignerStatus = GetWorkflowCollaborationSignerStatus()
        uscSettoriSegreterie.ReadOnly = True
        uscSettoriSegreterie.Checkable = False
        uscRestituzioni.ReadOnly = True
        btnConferma.Visible = False
        pnlTipoOperazione.Visible = False
        btnNewDesk.Visible = False
        btnResumeDesk.Visible = False
        btnRestituzione.Visible = False
        btnRifiuta.Visible = Action.Eq(CollaborationSubAction.DaVisionareFirmare) AndAlso Not CurrentSigner.IsRequired.GetValueOrDefault(False)
        btnRifiuta.Enabled = workflowCollaborationApprovedStatus.Equals(WorkflowCollaborationSignerStatus.None) OrElse workflowCollaborationApprovedStatus.Equals(WorkflowCollaborationSignerStatus.Approved)
        If Not CurrentSigner.IsRequired.GetValueOrDefault(False) Then
            btnRifiuta.Text = "Rifiuta"
        End If
        btnProtocolla.Visible = Action.Eq(CollaborationSubAction.DaProtocollareGestire)
        btnVisioneProtocolla.Visible = False
        btnInoltra.Visible = Action.Eq(CollaborationSubAction.DaVisionareFirmare) AndAlso Not CurrentSigner.IsRequired.GetValueOrDefault(False)
        btnInoltra.Enabled = workflowCollaborationApprovedStatus.Equals(WorkflowCollaborationSignerStatus.None) OrElse workflowCollaborationApprovedStatus.Equals(WorkflowCollaborationSignerStatus.Refused)
        If Not CurrentSigner.IsRequired.GetValueOrDefault(False) Then
            btnInoltra.Text = "Approva"
        End If
        tNote.Visible = False
        btnRichiamo.Visible = False
        btnMail.Visible = False
        cmdSave.Visible = False
        btnCancella.Visible = Action.Eq(CollaborationSubAction.DaVisionareFirmare) AndAlso CurrentSigner.IsRequired.GetValueOrDefault(False)
        If CurrentSigner.IsRequired.GetValueOrDefault(False) Then
            btnCancella.Text = "Annulla"
        End If
        btnCancella.Enabled = workflowCollaborationApprovedStatus.Equals(WorkflowCollaborationSignerStatus.None) OrElse workflowCollaborationApprovedStatus.Equals(WorkflowCollaborationSignerStatus.Approved)

        If CurrentUDSRepository IsNot Nothing Then
            pnlUDS.SetDisplay(True)
            lblPnlUDS.Text = $"Archivio {CurrentUDSRepository.Name}"
            uscUDS.ActionType = uscUDS.ACTION_TYPE_VIEW
            If Action2.Eq(CollaborationMainAction.DaVisionareFirmare) Then
                btnEditUDS.Visible = True
                btnEditUDS.PostBackUrl = String.Format(UDSView.EDIT_PAGE_URL, CurrentUDS.Id, CurrentUDSRepository.Id, Server.UrlEncode(Request.Url.ToString()))
                btnEditUDS.Text = $"Modifica {CurrentUDSRepository.Name}"
            End If
            UDSSummaryExpander()
        End If
    End Sub

    Private Sub BindMainDocuments()
        Dim lastMainDocuments As IDictionary(Of Guid, BiblosDocumentInfo)
        lastMainDocuments = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(CurrentCollaboration, VersioningDocumentGroup.MainDocument)
        If lastMainDocuments IsNot Nothing AndAlso lastMainDocuments.Count > 0 Then
            uscDocumento.LoadDocumentInfo(New List(Of DocumentInfo)(lastMainDocuments.Values), False, True, False, False, False, previewable:=HasViewableRights)
            uscDocumento.TreeViewControl.Nodes(0).Nodes(0).Selected = True
        End If

        ' Etichetta versioning
        Dim versioning As CollaborationVersioning = LastVersionings.FirstOrDefault(Function(v) CollaborationVersioningFacade.IsMainDocumentVersioning(v))
        If versioning Is Nothing Then
            lblDocumento.Text = String.Empty
            Exit Sub
        End If

        If NeedSetAtLeastOneDocumentSigned Then
            AtLeastOneDocumentSigned = lastMainDocuments.First().Value.IsSigned
        End If


        Dim label As String = String.Format("(v.{0} di {1} del {2:dd/MM/yyyy})", versioning.Incremental, CommonAD.GetDisplayName(versioning.RegistrationUser), versioning.RegistrationDate)
        If versioning.CheckedOut Then
            label = String.Format("{0} estratto da {1} dal {2:dd/MM/yyyy}", label, CommonAD.GetDisplayName(versioning.CheckOutUser), versioning.CheckOutDate)
        End If
        lblDocumento.Text = label
        uscDocumento.TreeViewControl.SelectedNode.Attributes(VersioningKeyAttributeName) = JsonConvert.SerializeObject(versioning.Id)

        cmdDocumentCheckOut.Enabled = CanCheckOut(uscDocumento)
    End Sub

    Private Sub BindMainDocumentsOmissis()
        Dim lastMainDocumentsOmissis As IDictionary(Of Guid, BiblosDocumentInfo)
        lastMainDocumentsOmissis = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(CurrentCollaboration, VersioningDocumentGroup.MainDocumentOmissis)
        If lastMainDocumentsOmissis IsNot Nothing AndAlso lastMainDocumentsOmissis.Count > 0 Then
            uscDocumentoOmissis.LoadDocumentInfo(New List(Of DocumentInfo)(lastMainDocumentsOmissis.Values), False, True, False, False, False, previewable:=HasViewableRights)
            uscDocumentoOmissis.TreeViewControl.Nodes(0).Nodes(0).Selected = True
        End If

        For Each node As RadTreeNode In uscDocumentoOmissis.TreeViewControl.Nodes(0).Nodes
            Dim doc As DocumentInfo = uscDocumentUpload.GetDocumentInfoByNode(node)
            If doc Is Nothing Then
                Continue For
            End If
            If NeedSetAtLeastOneDocumentSigned Then
                AtLeastOneDocumentSigned = doc.IsSigned
            End If
            Dim label As String = doc.Name
            Dim versioning As CollaborationVersioning = CollaborationVersioningFacade.GetVersioningByDocumentInfo(LastVersionings, doc)
            If versioning Is Nothing Then
                label = String.Format("{0} (non ancora versionato)", label)
            Else
                label = String.Format("{0} (v.{1} del {2:dd/MM/yyyy})", label, versioning.Incremental, versioning.RegistrationDate)
                If versioning.CheckedOut Then
                    label = String.Format("{0} *{1} dal {2:dd/MM/yyyy}", label, CommonAD.GetDisplayName(versioning.CheckOutUser), versioning.CheckOutDate)
                End If
                node.Attributes(VersioningKeyAttributeName) = JsonConvert.SerializeObject(versioning.Id)
            End If
            node.Text = label
        Next

        cmdDocumentOmissisCheckOut.Enabled = CanCheckOut(uscDocumentoOmissis)
    End Sub

    Private Sub BindAttachments()
        Dim lastAttachments As IDictionary(Of Guid, BiblosDocumentInfo)
        lastAttachments = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(CurrentCollaboration, VersioningDocumentGroup.Attachment)
        If lastAttachments IsNot Nothing AndAlso lastAttachments.Count > 0 Then
            uscAllegati.LoadDocumentInfo(New List(Of DocumentInfo)(lastAttachments.Values), False, True, False, False, False, previewable:=HasViewableRights)
            uscAllegati.InitializeNodesAsAdded(True)
            uscAllegati.TreeViewControl.Nodes(0).Nodes(0).Selected = True
        End If

        For Each node As RadTreeNode In uscAllegati.TreeViewControl.Nodes(0).Nodes
            Dim doc As DocumentInfo = uscDocumentUpload.GetDocumentInfoByNode(node)
            If doc Is Nothing Then
                Continue For
            End If
            If NeedSetAtLeastOneDocumentSigned Then
                AtLeastOneDocumentSigned = doc.IsSigned
            End If
            Dim label As String = doc.Name
            Dim versioning As CollaborationVersioning = CollaborationVersioningFacade.GetVersioningByDocumentInfo(LastVersionings, doc)
            If versioning Is Nothing Then
                label = String.Format("{0} (non ancora versionato)", label)
            Else
                label = String.Format("{0} (v.{1} del {2:dd/MM/yyyy})", label, versioning.Incremental, versioning.RegistrationDate)
                If versioning.CheckedOut Then
                    label = String.Format("{0} *{1} dal {2:dd/MM/yyyy}", label, CommonAD.GetDisplayName(versioning.CheckOutUser), versioning.CheckOutDate)
                End If
                node.Attributes(VersioningKeyAttributeName) = JsonConvert.SerializeObject(versioning.Id)
            End If
            node.Text = label
        Next

        cmdAttachmentsCheckOut.Enabled = CanCheckOut(uscAllegati)
    End Sub

    Private Sub BindAttachmentsOmissis()
        Dim lastAttachmentsOmissis As IDictionary(Of Guid, BiblosDocumentInfo)
        lastAttachmentsOmissis = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(CurrentCollaboration, VersioningDocumentGroup.AttachmentOmissis)
        If lastAttachmentsOmissis IsNot Nothing AndAlso lastAttachmentsOmissis.Count > 0 Then
            uscAllegatiOmissis.LoadDocumentInfo(New List(Of DocumentInfo)(lastAttachmentsOmissis.Values), False, True, False, False, False, previewable:=HasViewableRights)
            uscAllegatiOmissis.InitializeNodesAsAdded(True)
            uscAllegatiOmissis.TreeViewControl.Nodes(0).Nodes(0).Selected = True
        End If

        For Each node As RadTreeNode In uscAllegatiOmissis.TreeViewControl.Nodes(0).Nodes
            Dim doc As DocumentInfo = uscDocumentUpload.GetDocumentInfoByNode(node)
            If doc Is Nothing Then
                Continue For
            End If

            If NeedSetAtLeastOneDocumentSigned Then
                AtLeastOneDocumentSigned = doc.IsSigned
            End If

            Dim label As String = doc.Name
            Dim versioning As CollaborationVersioning = CollaborationVersioningFacade.GetVersioningByDocumentInfo(LastVersionings, doc)
            If versioning Is Nothing Then
                label = String.Format("{0} (non ancora versionato)", label)
            Else
                label = String.Format("{0} (v.{1} del {2:dd/MM/yyyy})", label, versioning.Incremental, versioning.RegistrationDate)
                If versioning.CheckedOut Then
                    label = String.Format("{0} *{1} dal {2:dd/MM/yyyy}", label, CommonAD.GetDisplayName(versioning.CheckOutUser), versioning.CheckOutDate)
                End If
                node.Attributes(VersioningKeyAttributeName) = JsonConvert.SerializeObject(versioning.Id)
            End If
            node.Text = label
        Next

        cmdAttachmentsOmissisCheckOut.Enabled = CanCheckOut(uscAllegatiOmissis)
    End Sub

    Private Sub BindAnnexed()
        Dim lastAnnexed As IDictionary(Of Guid, BiblosDocumentInfo)
        lastAnnexed = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(CurrentCollaboration, VersioningDocumentGroup.Annexed)
        If lastAnnexed IsNot Nothing AndAlso lastAnnexed.Count > 0 Then
            uscAnnexed.LoadDocumentInfo(New List(Of DocumentInfo)(lastAnnexed.Values), False, True, False, False, False, previewable:=HasViewableRights)
            uscAnnexed.InitializeNodesAsAdded(True)
            uscAnnexed.TreeViewControl.Nodes(0).Nodes(0).Selected = True
        End If

        For Each node As RadTreeNode In uscAnnexed.TreeViewControl.Nodes(0).Nodes
            Dim doc As DocumentInfo = uscDocumentUpload.GetDocumentInfoByNode(node)
            If doc Is Nothing Then
                Continue For
            End If

            If NeedSetAtLeastOneDocumentSigned Then
                AtLeastOneDocumentSigned = doc.IsSigned
            End If

            Dim label As String = doc.Name
            Dim versioning As CollaborationVersioning = CollaborationVersioningFacade.GetVersioningByDocumentInfo(LastVersionings, doc)
            If versioning Is Nothing Then
                label = String.Format("{0} (non ancora versionato)", label)
            Else
                label = String.Format("{0} (v.{1} del {2:dd/MM/yyyy})", label, versioning.Incremental, versioning.RegistrationDate)
                If versioning.CheckedOut Then
                    label = String.Format("{0} *{1} dal {2:dd/MM/yyyy}", label, CommonAD.GetDisplayName(versioning.CheckOutUser), versioning.CheckOutDate)
                End If
                node.Attributes(VersioningKeyAttributeName) = JsonConvert.SerializeObject(versioning.Id)
            End If
            node.Text = label
        Next

        cmdAnnexedCheckOut.Enabled = CanCheckOut(uscAnnexed)
    End Sub

    Private Sub InitializeMainDocument(ByVal enabled As Boolean, versioningDisabled As Boolean, ByVal daNonProtocollare As Boolean)
        If Not ProtocolEnv.CollaborationDocumentEditable Then
            enabled = enabled AndAlso (Action.Eq(CollaborationSubAction.DaVisionareFirmare) OrElse Action.Eq(CollaborationSubAction.DaFirmareInDelega))
        End If

        'Mostro i pulsanti di upload documento se se la collaborazione è ancora al primo firmatario, se la pagina è Ins.visione/firma o Attività in corso 
        'e se:  1)l'utente è il proponente o 2)se l'utente è segreteria corrente
        If Not enabled AndAlso DocumentEditEnabled Then
            enabled = Action.Eq(CollaborationSubAction.AllaVisioneFirma) OrElse Action.Eq(CollaborationSubAction.AttivitaInCorso) OrElse Action.Eq(CollaborationSubAction.DaFirmareInDelega)
        End If

        If daNonProtocollare = True Then
            uscDocumento.IsDocumentRequired = enabled
        Else
            uscDocumento.IsDocumentRequired = Not enabled
        End If

        uscDocumento.SignButtonEnabled = enabled AndAlso ProtocolEnv.IsFDQEnabled
        uscDocumento.ButtonScannerEnabled = enabled
        uscDocumento.ButtonFileEnabled = enabled
        uscDocumento.ButtonLibrarySharepointEnabled = enabled AndAlso ProtocolEnv.UploadSharepointDocumentLibrary
        uscDocumento.ButtonRemoveEnabled = False
        uscDocumento.ButtonSelectTemplateEnabled = enabled AndAlso GetControlTemplateDocumentVisibility(Entity.DocumentUnits.ChainType.MainChain)
        uscDocumento.ButtonPreviewEnabled = enabled OrElse (Not enabled AndAlso HasViewableRights)

        If Action.Eq(CollaborationSubAction.DaVisionareFirmare) OrElse Action.Eq(CollaborationSubAction.DaFirmareInDelega) Then
            uscDocumento.ButtonScannerEnabled = False
            uscDocumento.ButtonFileEnabled = False
            uscDocumento.ButtonLibrarySharepointEnabled = False
            uscDocumento.ButtonSelectTemplateEnabled = False
        End If

        If Action.Eq(CollaborationSubAction.DaFirmareInDelega) Then
            uscDocumento.DelegatedUser = CurrentSigner.SignUser
        End If

        BindMainDocuments()

        If versioningDisabled Then
            cmdDocumentCheckOut.Enabled = False
            cmdDocumentUndoCheckOut.Enabled = False
            cmdDocumentCheckIn.Enabled = False
        End If
    End Sub

    Private Sub InitializeMainDocument(enabled As Boolean, daNonProtocollare As Boolean)
        InitializeMainDocument(enabled, Not enabled, daNonProtocollare)
    End Sub

    Private Sub InitializeMainDocumentOmissis(ByVal enabled As Boolean, versioningDisabled As Boolean)
        If Not ProtocolEnv.CollaborationAttachmentEditable Then
            enabled = enabled AndAlso (Action.Eq(CollaborationSubAction.DaVisionareFirmare) OrElse Action.Eq(CollaborationSubAction.DaFirmareInDelega))
        End If

        uscDocumentoOmissis.IsDocumentRequired = False
        uscDocumentoOmissis.SignButtonEnabled = enabled AndAlso ProtocolEnv.IsFDQEnabled
        uscDocumentoOmissis.ButtonScannerEnabled = enabled
        uscDocumentoOmissis.ButtonFileEnabled = enabled
        uscDocumentoOmissis.ButtonLibrarySharepointEnabled = enabled AndAlso ProtocolEnv.UploadSharepointDocumentLibrary
        uscDocumentoOmissis.ButtonRemoveEnabled = enabled
        uscDocumentoOmissis.ButtonSelectTemplateEnabled = enabled AndAlso GetControlTemplateDocumentVisibility(Entity.DocumentUnits.ChainType.MainOmissisChain)
        uscDocumentoOmissis.ButtonPreviewEnabled = enabled OrElse (Not enabled AndAlso HasViewableRights)

        If Action.Eq(CollaborationSubAction.DaFirmareInDelega) Then
            uscDocumentoOmissis.DelegatedUser = CurrentSigner.SignUser
        End If

        BindMainDocumentsOmissis()

        If versioningDisabled Then
            cmdDocumentOmissisCheckOut.Enabled = False
            cmdDocumentOmissisUndoCheckOut.Enabled = False
            cmdDocumentOmissisCheckOut.Enabled = False
        End If
    End Sub

    Private Sub InitializeMainDocumentOmissis(enabled As Boolean)
        InitializeMainDocumentOmissis(enabled, Not enabled)
    End Sub
    Private Sub InitializeEditButtonsDocumentUpload()
        InizializeAddButtonDocumentUpload(False, uscAllegati)
        InizializeAddButtonDocumentUpload(False, uscAllegatiOmissis)
        InizializeAddButtonDocumentUpload(False, uscAnnexed)
        InizializeRemoveButtonDocumentUpload(False, uscAllegati)
        InizializeRemoveButtonDocumentUpload(False, uscAllegatiOmissis)
        InizializeRemoveButtonDocumentUpload(False, uscAnnexed)
    End Sub
    Private Sub InizializeButtonDocumentUpload(ByVal enabled As Boolean, ByRef usc As uscDocumentUpload)
        InizializeAddButtonDocumentUpload(enabled, usc)
        InizializeRemoveButtonDocumentUpload(enabled, usc)
        InizializeSignButtonDocumentUpload(enabled, usc)
    End Sub

    Private Sub InizializeAddButtonDocumentUpload(ByVal enabled As Boolean, ByRef usc As uscDocumentUpload)
        usc.IsDocumentRequired = False
        usc.ButtonScannerEnabled = enabled
        usc.ButtonFileEnabled = enabled
        usc.ButtonLibrarySharepointEnabled = enabled AndAlso ProtocolEnv.UploadSharepointDocumentLibrary
        usc.ButtonCopyProtocol.Visible = enabled
        usc.ButtonCopyResl.Visible = enabled
        usc.ButtonCopyUDS.Visible = enabled
        usc.EnableImportContactManual = False
        usc.ButtonPreviewEnabled = enabled OrElse (Not enabled AndAlso HasViewableRights)

        Dim currentChainType As Entity.DocumentUnits.ChainType = Entity.DocumentUnits.ChainType.MainChain
        Select Case usc.ID
            Case uscDocumentoOmissis.ID
                currentChainType = Entity.DocumentUnits.ChainType.MainOmissisChain
            Case uscAllegati.ID
                currentChainType = Entity.DocumentUnits.ChainType.AttachmentsChain
            Case uscAllegatiOmissis.ID
                currentChainType = Entity.DocumentUnits.ChainType.AttachmentOmissisChain
            Case uscAnnexed.ID
                currentChainType = Entity.DocumentUnits.ChainType.AnnexedChain
        End Select
        usc.ButtonSelectTemplateEnabled = enabled AndAlso GetControlTemplateDocumentVisibility(currentChainType)
    End Sub
    Private Sub InizializeRemoveButtonDocumentUpload(ByVal enabled As Boolean, ByRef usc As uscDocumentUpload)
        usc.ButtonRemoveEnabled = enabled
    End Sub

    Private Sub InizializeSignButtonDocumentUpload(ByVal enabled As Boolean, ByRef usc As uscDocumentUpload)
        usc.SignButtonEnabled = enabled AndAlso ProtocolEnv.IsFDQEnabled
    End Sub

    Private Sub InitializeAttachments(ByVal enabled As Boolean, versioningDisabled As Boolean)
        If Not ProtocolEnv.CollaborationAttachmentEditable Then
            enabled = enabled AndAlso (Action.Eq(CollaborationSubAction.DaVisionareFirmare) OrElse Action.Eq(CollaborationSubAction.DaFirmareInDelega))
        End If

        InizializeButtonDocumentUpload(enabled, uscAllegati)

        If Action.Eq(CollaborationSubAction.DaFirmareInDelega) Then
            uscAllegati.DelegatedUser = CurrentSigner.SignUser
        End If

        BindAttachments()

        If versioningDisabled Then
            cmdAttachmentsCheckOut.Enabled = False
            cmdAttachmentsUndoCheckOut.Enabled = False
            cmdAttachmentsCheckIn.Enabled = False
        End If
    End Sub

    Private Sub InitializeAttachments(enabled As Boolean)
        InitializeAttachments(enabled, Not enabled)
    End Sub

    Private Sub InitializeAttachmentsOmissis(ByVal enabled As Boolean, versioningDisabled As Boolean)
        If Not ProtocolEnv.CollaborationAttachmentEditable Then
            enabled = enabled AndAlso (Action.Eq(CollaborationSubAction.DaVisionareFirmare) OrElse Action.Eq(CollaborationSubAction.DaFirmareInDelega))
        End If

        InizializeButtonDocumentUpload(enabled, uscAllegatiOmissis)

        If Action.Eq(CollaborationSubAction.DaFirmareInDelega) Then
            uscAllegatiOmissis.DelegatedUser = CurrentSigner.SignUser
        End If

        BindAttachmentsOmissis()

        If versioningDisabled Then
            cmdAttachmentsOmissisCheckOut.Enabled = False
            cmdAttachmentsOmissisUndoCheckOut.Enabled = False
            cmdAttachmentsOmissisCheckIn.Enabled = False
        End If
    End Sub

    Private Sub InitializeAttachmentsOmissis(enabled As Boolean)
        InitializeAttachmentsOmissis(enabled, Not enabled)
    End Sub

    Private Sub InitializeAnnexed(enabled As Boolean, versioningDisabled As Boolean)
        If Not ProtocolEnv.CollaborationAttachmentEditable Then
            enabled = enabled AndAlso (Action.Eq(CollaborationSubAction.DaVisionareFirmare) OrElse Action.Eq(CollaborationSubAction.DaFirmareInDelega))
        End If

        InizializeButtonDocumentUpload(enabled, uscAnnexed)

        If Action.Eq(CollaborationSubAction.DaFirmareInDelega) Then
            uscAnnexed.DelegatedUser = CurrentSigner.SignUser
        End If

        BindAnnexed()

        If versioningDisabled Then
            cmdAnnexedCheckOut.Enabled = False
            cmdAnnexedUndoCheckOut.Enabled = False
            cmdAnnexedCheckIn.Enabled = False
        End If
    End Sub

    Private Sub InitializeAnnexed(enabled As Boolean)
        InitializeAnnexed(enabled, Not enabled)
    End Sub

    ''' <summary> Verifica che l'utente corrente sia il firmatario </summary>
    ''' <returns>true se l'utente corrente corrisponde al firmatario, false in caso contrario</returns>
    Private Function IsSignatory() As Boolean
        ' Verifico l'utente corrente se è il firmatario
        Dim signs As IList(Of CollaborationSign) = Facade.CollaborationSignsFacade.SearchFull(CurrentCollaboration.Id)
        If signs IsNot Nothing Then
            Return signs.Any(Function(sign) DocSuiteContext.Current.User.FullUserName.Eq(sign.SignUser) AndAlso sign.IsActive)
        End If
        Return False
    End Function
    Private Function IsDelegateSignatory() As Boolean
        ' Verifico l'utente corrente se è il firmatario delegato
        Dim signs As IList(Of CollaborationSign) = Facade.CollaborationSignsFacade.SearchFull(CurrentCollaboration.Id)
        If signs IsNot Nothing Then
            Dim listDelegations As List(Of String) = Facade.UserLogFacade.GetDelegationsSign()
            Return signs.Any(Function(sign) listDelegations.Any(Function(x) x.Eq(sign.SignUser)) AndAlso sign.IsActive)
        End If
        Return False
    End Function

    ''' <summary> Inizializza proponente e campi base. </summary>
    ''' <param name="enabled"> Indica se abilitare la modifica. </param>
    Private Sub InitDati(ByVal enabled As Boolean)
        pnlProponente.Visible = True
        Dim proposerDisplayName As String = CommonAD.GetDisplayName(CurrentCollaboration.RegistrationUser)
        Dim registrationUserNode As New RadTreeNode(proposerDisplayName)
        registrationUserNode.Style.Add("font-weight", "bold")
        uscProponente.TreeViewControl.Nodes(0).Nodes.Clear()
        uscProponente.TreeViewControl.Nodes(0).Nodes.Add(registrationUserNode)

        rblPriority.SelectedValue = CurrentCollaboration.IdPriority
        rblPriority.Enabled = enabled

        ddlDocumentType.SelectedValue = CurrentCollaboration.DocumentType
        ddlDocumentType.Enabled = False
        trSpecificDocumentType.Visible = False
        rfvSpecificDocumentType.Enabled = False

        txtDate.SelectedDate = CurrentCollaboration.MemorandumDate
        txtDate.DateInput.ReadOnly = Not enabled
        txtDate.DatePopupButton.Visible = enabled

        If DocSuiteContext.Current.ProtocolEnv.CollaborationManagementExpired Then
            alertDate.SelectedDate = CurrentCollaboration.AlertDate
            alertDate.DateInput.ReadOnly = Not enabled
            alertDate.DatePopupButton.Visible = enabled
        End If

        txtObject.Text = CurrentCollaboration.CollaborationObject
        txtObject.Enabled = enabled

        txtNote.Text = CurrentCollaboration.Note
        txtNote.Enabled = enabled

        tEditCollaborationData.Visible = Not enabled AndAlso ProtocolEnv.CollaborationVisionSignatureDataEditable AndAlso Action.Eq(CollaborationSubAction.AllaVisioneFirma)

        cmdSave.Visible = enabled
    End Sub

    Private Sub InitVisioneFirma(ByVal enabled As Boolean, ByVal highlightActive As Boolean, Optional onlySigners As Boolean = False, Optional editSigners As Boolean = False)
        pnlVisioneFirma.Visible = True
        uscVisioneFirma.ReadOnly = Not enabled AndAlso Not editSigners
        Dim signs As IList(Of CollaborationSign) = Facade.CollaborationSignsFacade.SearchFull(CurrentCollaboration.Id)
        If signs IsNot Nothing Then
            For Each sign As CollaborationSign In signs
                Dim bold As Boolean = sign.IsActive AndAlso highlightActive
                Dim signName As String = Facade.CollaborationSignsFacade.GetCollaborationSignDescription(sign.SignName, sign.IsAbsent)
                uscVisioneFirma.AddCollaborationContact(ContactType.AdAmPerson, signName, sign.SignEMail, sign.SignUser, "", "", If(editSigners, True, sign.IsRequired.Value), sign.IsRequired.Value, bold:=bold)
            Next
        End If

        If Not onlySigners Then
            Dim dummy As Contact = New Contact() With {.Code = DocSuiteContext.Current.User.FullUserName, .RoleUserIdRole = String.Empty}
            Dim segretaries As IList(Of CollaborationUser) = Facade.CollaborationUsersFacade.GetByCollaboration(CurrentCollaboration.Id, Nothing, DestinatonType.S)

            For Each collaborationRole As CollaborationUser In segretaries
                Dim role As Role = Facade.RoleFacade.GetById(collaborationRole.IdRole.Value)
                RoleContacts.Add(role, New List(Of Contact)({dummy}))

                uscSettoriSegreterie.AddRole(role, True, False, False, collaborationRole.DestinationFirst.GetValueOrDefault(False))
            Next

            txtDestinatarioOK.Text = If(enabled, "OK", "oo")
        End If

    End Sub

    Private Sub InitInoltro()
        Dim signs As IList(Of CollaborationSign) = Facade.CollaborationSignsFacade.SearchFull(CurrentCollaboration.Id, True)
        If signs IsNot Nothing Then
            For Each sign As CollaborationSign In signs
                Dim signName As String = Facade.CollaborationSignsFacade.GetCollaborationSignDescription(sign.SignName, sign.IsAbsent)
                uscInoltro.AddCollaborationContact(ContactType.AdAmPerson, signName, sign.SignEMail, sign.SignUser, String.Empty, String.Empty, False)
            Next
        End If
        txtInoltroOK.Text = "OK"
    End Sub

    Private Sub InitRestituzioniTabella(ByVal enabled As Boolean)
        pnlRestituzioni.Visible = True
        uscRestituzioni.ReadOnly = Not enabled
        uscSettoriSegreterie.ReadOnly = Not enabled
        uscSettoriSegreterie.EditableCheck = enabled
        uscRestituzioni.TreeViewControl.Enabled = enabled
        txtRestituzioniOK.Text = "oo"

        Dim users As IList(Of CollaborationUser) = Facade.CollaborationUsersFacade.GetByCollaboration(CurrentCollaboration.Id, Nothing, Nothing)
        If users Is Nothing Then
            Exit Sub
        End If

        For Each collUser As CollaborationUser In users
            If collUser.DestinationType.Eq(DestinatonType.S.ToString()) Then
                ' Se sto guardando un settore (segreteria)
                ' Imposto il check a tutti i settori già legati alla collaborazione
                For Each role As Role In RoleContacts.Keys
                    For Each contact As Contact In RoleContacts(role)
                        If contact.RoleUserIdRole.Eq(collUser.IdRole) Then
                            uscSettoriSegreterie.CheckRole(role)
                        End If
                    Next
                Next
            Else
                ' Se sono su un utente
                uscRestituzioni.AddCollaborationContact(ContactType.AdAmPerson, collUser.DestinationName, collUser.DestinationEMail, collUser.Account, "G", "", True, collUser.DestinationFirst.Value)
            End If
        Next
    End Sub

    ''' <summary>Controlla che esistano destinatari di firma/inoltro</summary>
    ''' <remarks>Al seguito della correzione del ticket questo metodo potrebbe essere inutile</remarks>
    Private Sub ValidateExistRecipients()
        If Not uscSettoriSegreterie.GetCheckedRoles().IsNullOrEmpty() OrElse uscRestituzioni.TreeViewControl.Nodes(0).Nodes.Count <> 0 Then
            Exit Sub
        End If
        If Not ProtocolEnv.CheckExistRecipientsAndRepair Then
            Throw New DocSuiteException("Collaborazione non valida", String.Format("Collaborazione [{0}] senza destinatari, contattare l'assistenza.", CurrentCollaboration.Id))
        End If
        For Each collaborationUser As CollaborationUser In CurrentCollaboration.CollaborationUsers
            collaborationUser.DestinationFirst = True
        Next
        Facade.CollaborationFacade.Save(CurrentCollaboration)
        uscSettoriSegreterie.CheckAll()
    End Sub

    Private Sub SetVisioneFirma()
        btnRestituzione.Visible = False
        btnRifiuta.Visible = False
        btnVisioneProtocolla.Visible = False
        btnInoltra.Visible = False
        Dim localType As String = "Prot"
        If Not String.IsNullOrEmpty(Type()) Then
            localType = Type()
        End If

        Select Case rblTipoOperazione.SelectedValue
            Case "A" ' Al protocollo / alla delibera
                btnRestituzione.Visible = True
                pnlRestituzioni.Visible = True
                pnlInoltro.Visible = False
            Case "R" ' Restituzione
                btnRifiuta.Visible = True
                pnlRestituzioni.Visible = False
                pnlInoltro.Visible = False
            Case "I" ' Inoltro
                btnInoltra.Visible = True
                pnlRestituzioni.Visible = True
                pnlInoltro.Visible = True
                uscInoltro.IsRequired = True
                uscInoltro.CollaborationType = ddlDocumentType.SelectedValue
                uscInoltro.EnvironmentType = localType
                uscInoltro.UpdateRoleUserType()
            Case "P" ' Protocollazione
                btnVisioneProtocolla.Visible = True
                pnlRestituzioni.Visible = False
                pnlInoltro.Visible = False
            Case "S" ' Segreteria
                btnProtocolla.Visible = True
                pnlInoltro.Visible = False

                uscSettoriSegreterie.EditableCheck = False
                uscRestituzioni.ReadOnly = True
                uscRestituzioni.TreeViewControl.Enabled = False
            Case "IS" ' Procedura di inoltro da segreteria
                btnInoltra.Visible = True
                pnlRestituzioni.Visible = True
                pnlInoltro.Visible = True
                btnProtocolla.Visible = False

                uscSettoriSegreterie.EditableCheck = False
                uscRestituzioni.ReadOnly = False
                uscRestituzioni.TreeViewControl.Enabled = True
        End Select
    End Sub

    ''' <summary> Inserisce una lista piatta di settori legati al contatto passato </summary>
    Private Sub InitializeSegreterie(ByVal contact As Contact)
        Dim roles As New List(Of Role)
        If Not String.IsNullOrEmpty(contact.RoleUserIdRole) Then
            roles.Add(Facade.RoleFacade.GetById(Integer.Parse(contact.RoleUserIdRole)))
        Else
            roles.AddRange(Facade.RoleUserFacade.GetSecretaryRoles(contact.Code, True))
        End If

        If roles.IsNullOrEmpty() Then
            Exit Sub
        End If

        For Each role As Role In roles
            If RoleContacts.ContainsKey(role) Then
                RoleContacts(role).Add(contact)
            Else
                RoleContacts.Add(role, New List(Of Contact)({contact}))
                Dim checked As Boolean = True
                If Not String.IsNullOrEmpty(CurrentTemplateName) AndAlso TemplateModels IsNot Nothing AndAlso TemplateModels.Any(Function(f) f.TemplateName.Eq(CurrentTemplateName)) Then
                    checked = False
                End If

                uscSettoriSegreterie.AddRole(role, True, False, False, checked)
            End If
        Next
    End Sub

    Private Sub InsertNewCollaborationAggregate()
        Try

            For Each collaboration As Collaboration In CollaborationUoiaSelected
                Dim Aggregate As New CollaborationAggregate
                Aggregate.CollaborationDocumentType = CollaborationDocumentType.P.ToString
                Aggregate.CollaborationFather = CurrentCollaboration
                Aggregate.CollaborationChild = collaboration
                Facade.CollaborationAggregateFacade.Save(Aggregate)
            Next

        Catch ex As Exception

            FileLogger.Warn(LoggerName, "Errore in fase di inserimento aggregate", ex)
            AjaxAlert(ex.Message)

        End Try

    End Sub


    Private Function InsertNewCollaboration() As Boolean
        'VERIFICA DOCUMENTI aggiunti se presenti in FileSystem
        If Not CommonInstance.BiblosExistFile(uscDocumento.DocumentInfosAdded) Then
            uscDocumento.TreeViewControl.Nodes(0).Nodes.Clear()
            Throw New DocSuiteException("Errore inserimento", String.Format("Il Documento sul Server non è valido.{0}Reinserire il Documento", Environment.NewLine))
        End If

        'Recupero la location delle collaborazioni (ID = 99 di default da parametro) per il database della tipologia indicata
        Dim loc As Location = Facade.LocationFacade.GetById(ProtocolEnv.CollaborationLocation)
        If loc.ProtBiblosDSDB Is Nothing Then
            Throw New DocSuiteException("Errore inserimento", String.Format("Inserire il valore ProtBiblosDSDB nella tabella Location per il DB: {0}.", Type))
        End If

        If Not Action.Eq(CollaborationSubAction.InserimentoAllaVisioneFirma) AndAlso Not Action.Eq(CollaborationSubAction.InserimentoAlProtocolloSegreteria) Then
            Throw New DocSuiteException("Errore inserimento", String.Format("Tipo inserimento [{0}] non previsto", Action))
        End If

        ' Settori ai quali avviene la distribuzione
        Dim segreterie As List(Of CollaborationUser) = (From destinationRole In uscSettoriSegreterie.GetCheckedRoles() Select New CollaborationUser(destinationRole) With {.DestinationFirst = True}).ToList()
        ' Altri settori
        segreterie.AddRange(From role In uscSettoriSegreterie.GetUncheckedRoles() Select New CollaborationUser(role) With {.DestinationFirst = False})

        ' Utenti al quale distribuire
        Dim altriDestinatariProtocollazione As List(Of CollaborationContact) = GetManualContacts(uscRestituzioni.TreeViewControl.Nodes(0))

        If Not segreterie.Any(Function(cc) cc.DestinationFirst.GetValueOrDefault(False)) AndAlso Not altriDestinatariProtocollazione.Any(Function(cc) cc.DestinationFirst) Then
            Throw New DocSuiteException("Errore inserimento", String.Format("Nessun Destinatario di {0} selezionato.", DocumentTypeCaption(ddlDocumentType.SelectedValue)))
        End If

        ' Seleziono lo stato corretto per la nuova collaborazione
        Dim idStatus As CollaborationStatusType
        Dim mailStatus As String = String.Empty
        Select Case Action
            Case CollaborationSubAction.InserimentoAllaVisioneFirma
                idStatus = CollaborationStatusType.IN
                mailStatus = CollaborationMainAction.InserimentoPerVisioneFirma

            Case CollaborationSubAction.InserimentoAlProtocolloSegreteria
                idStatus = CollaborationStatusType.DP
                mailStatus = CollaborationMainAction.DaProtocollareGestire
        End Select

        ' Utenti firmatari
        Dim destinatariFirma As List(Of CollaborationContact) = GetManualContacts(uscVisioneFirma.TreeViewControl.Nodes(0))
        If (ProtocolEnv.CollaborationProposerAsSignerEnabled) Then
            Dim proposerAsSigner As CollaborationContact = New CollaborationContact()
            proposerAsSigner.Account = DocSuiteContext.Current.User.FullUserName
            proposerAsSigner.DestinationName = CommonInstance.UserDescription
            proposerAsSigner.DestinationEMail = Facade.UserLogFacade.EmailOfUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain, ProtocolEnv.EnableUserProfile)
            destinatariFirma.Insert(0, proposerAsSigner)
        End If

        Dim template As TemplateCollaboration = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentTemplateCollaborationFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        If trSpecificDocumentType.Visible AndAlso Not String.IsNullOrEmpty(ddlSpecificDocumentType.SelectedValue) Then
                            finder.UniqueId = Guid.Parse(ddlSpecificDocumentType.SelectedItem.Attributes("TemplateId"))
                        Else
                            finder.UniqueId = Guid.Parse(ddlDocumentType.SelectedItem.Attributes("TemplateId"))
                        End If
                        Return finder.DoSearch().First().Entity
                    End Function)

        Dim collaboration As Collaboration = FacadeFactory.Instance.CollaborationFacade.CreateCollaboration()
        collaboration.DocumentType = template.DocumentType
        collaboration.IdPriority = rblPriority.SelectedValue
        collaboration.IdStatus = idStatus.ToString()
        collaboration.MemorandumDate = txtDate.SelectedDate
        collaboration.TemplateName = template.Name
        If FromCollaboratinoUoia Then
            collaboration.TemplateName = "Lettera UOIA"
        End If
        If DocSuiteContext.Current.ProtocolEnv.CollaborationManagementExpired Then
            collaboration.AlertDate = alertDate.SelectedDate
        End If
        collaboration.CollaborationObject = txtObject.Text
        collaboration.Note = txtNote.Text
        If DocSuiteContext.Current.ProtocolEnv.CollaborationSourceProtocolEnabled Then
            collaboration.SetSourceProtocol(SourceProtocol)
        End If

        FacadeFactory.Instance.CollaborationFacade.Insert(collaboration, destinatariFirma, altriDestinatariProtocollazione, segreterie)

        ' FG20140730: mantenuto per retrocompatibilità.
        CollaborationId = collaboration.Id

        ' Inserisco il documento
        If Not AddDocumentsToVersioning(uscDocumento.DocumentInfosAdded, VersioningDocumentGroup.MainDocument) Then
            Facade.CollaborationFacade.Delete(CollaborationId.Value)
            Throw New DocSuiteException("Errore inserimento", String.Format("Si è verificato un errore nell'inserimento del Documento.{0}Annullare la registrazione", Environment.NewLine))
        End If

        ' Inserisco il documento privacy
        If Not AddDocumentsToVersioning(uscDocumentoOmissis.DocumentInfosAdded, VersioningDocumentGroup.MainDocumentOmissis) Then
            Facade.CollaborationFacade.Delete(CollaborationId.Value)
            Throw New DocSuiteException("Errore inserimento", String.Format("Si è verificato un errore nell'inserimento del Documento Omissis.{0}Annullare la registrazione", Environment.NewLine))
        End If

        ' todo: da verificare l'idcollaboration in ingresso, prima era newcollaborationid. - FG
        ' Inserisco gli allegati
        If Not AddDocumentsToVersioning(uscAllegati.DocumentInfosAdded, VersioningDocumentGroup.Attachment) Then
            Throw New DocSuiteException("Errore inserimento", String.Format("Si è verificato un errore nell'inserimento degli Allegati.{0}Annullare la registrazione", Environment.NewLine))
        End If

        ' Inserisco gli allegati
        If Not AddDocumentsToVersioning(uscAllegatiOmissis.DocumentInfosAdded, VersioningDocumentGroup.AttachmentOmissis) Then
            Throw New DocSuiteException("Errore inserimento", String.Format("Si è verificato un errore nell'inserimento degli Allegati Omissis.{0}Annullare la registrazione", Environment.NewLine))
        End If

        ' Inserisco gli "Annexed"
        If Not AddDocumentsToVersioning(uscAnnexed.DocumentInfosAdded, VersioningDocumentGroup.Annexed) Then
            Throw New DocSuiteException("Errore inserimento", String.Format("Si è verificato un errore nell'inserimento degli Annessi (non parte integrante).{0}Annullare la registrazione", Environment.NewLine))
        End If

        If (ProtocolEnv.CollaborationProposerAsSignerEnabled) AndAlso Action.Eq(CollaborationSubAction.InserimentoAllaVisioneFirma) Then
            Facade.CollaborationSignsFacade.UpdateForward(collaboration)
            Facade.CollaborationLogFacade.Insert(collaboration, "Avanzamento automatico al destinatario successivo")
            Facade.CollaborationFacade.UpdateBiblosSignsModel(collaboration)
        End If

        'Setto il tavolo di riferimento
        If DocSuiteContext.Current.ProtocolEnv.DeskEnable AndAlso InsertFromDesk Then
            CurrentDeskCollaborationFacade.AddDeskCollaboration(CurrentDeskFromInsert, collaboration)
        End If

        If pnlDocumentUnitDraftEditor.Visible Then
            Dim protocolXML As ProtocolXML = ProtocolXMLSource
            If Not protocolXML Is Nothing Then
                Facade.ProtocolDraftFacade.AddProtocolDraft(CurrentCollaboration, "Protocollo Precompilato da Collaborazione", protocolXML)
            End If
            Session.Remove("TemplateProtocolPrecopiler")
        End If

        ' Spedisco l'email
        Facade.CollaborationFacade.SendMail(collaboration, mailStatus)
        If (ProtocolEnv.CollaborationProposerAsSignerEnabled) AndAlso Action.Eq(CollaborationSubAction.InserimentoAllaVisioneFirma) Then
            Facade.CollaborationFacade.SendMail(collaboration, CollaborationMainAction.DaVisionareFirmare)
        End If

        Return True
    End Function


    Private Function ValidateObject() As Boolean
        Dim maxLength As Integer
        If ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.D.ToString()) OrElse ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.A.ToString()) Then
            maxLength = DocSuiteContext.Current.ResolutionEnv.ObjectMaxLength
        ElseIf ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.P.ToString()) Then
            maxLength = DocSuiteContext.Current.ProtocolEnv.ObjectMaxLength
        ElseIf ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.U.ToString()) Then
            maxLength = DocSuiteContext.Current.ProtocolEnv.ObjectMaxLength
        ElseIf ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.S.ToString()) OrElse ddlDocumentType.SelectedValue.Eq(CollaborationDocumentType.UDS.ToString()) OrElse Integer.TryParse(ddlDocumentType.SelectedValue, 0) Then
            maxLength = 500
        End If

        If txtObject.Text.Length > maxLength Then
            AjaxAlert("Impossibile salvare.{0}Il campo Oggetto ha superato i caratteri disponibili.{0}(Caratteri {1} Disponibili {2})", Environment.NewLine, txtObject.Text.Length, maxLength)
            Return False
        End If

        Return True
    End Function

    Private Function ValidateAlertDate() As Boolean
        If Not DocSuiteContext.Current.ProtocolEnv.CollaborationManagementExpired Then
            Return True
        End If

        If Not txtDate.SelectedDate.HasValue AndAlso alertDate.SelectedDate.HasValue Then
            AjaxAlert("Impossibile proseguire.{0}Il campo Data scadenza è obbligatorio se è valorizzato il campo Data Avviso", Environment.NewLine)
            Return False
        ElseIf alertDate.SelectedDate >= txtDate.SelectedDate Then
            AjaxAlert("Impossibile proseguire.{0}Il campo Data Avviso deve essere una data inferiore a quella di Data scadenza", Environment.NewLine)
            Return False
        End If
        Return True
    End Function

    ''' <summary> Permette di creare una lista Contatti selezionati </summary>
    Private Function GetManualContacts(ByVal treeNode As RadTreeNode) As List(Of CollaborationContact)
        Dim result As New List(Of CollaborationContact)
        If Not treeNode.Nodes.Count.Equals(0) Then
            For Each item As RadTreeNode In treeNode.Nodes
                result.AddRange(GetManualContacts(item))
            Next
            Return result
        End If

        Dim serialized As String = treeNode.Attributes(uscContattiSel.ManualContactAttribute)
        If String.IsNullOrEmpty(serialized) Then
            Return result
        End If

        Dim deserialized As Contact = JsonConvert.DeserializeObject(Of Contact)(serialized)
        Dim contact As New CollaborationContact()
        If deserialized.ContactType IsNot Nothing AndAlso deserialized.ContactType.Id = ContactType.Mistery Then
            contact.IdRole = deserialized.RoleUserIdRole
        Else
            contact.Account = deserialized.Code
        End If

        contact.DestinationName = deserialized.Description
        If contact.DestinationName.Contains("(") Then
            contact.DestinationName = contact.DestinationName.Substring(0, contact.DestinationName.IndexOf("(")).Trim()
        End If

        contact.DestinationEMail = deserialized.EmailAddress
        contact.DestinationFirst = treeNode.Checkable AndAlso treeNode.Checked
        result.Add(contact)

        Return result
    End Function

    ''' <summary> Aggiunge nella collaborazione la prima versione dei documenti. </summary>
    ''' <param name="documentList">Nuovi documenti da inserire nel versioning.</param>
    ''' <param name="documentGroup">DocumentGroup di riferimento.</param>
    Private Function AddDocumentsToVersioning(documentList As IList(Of DocumentInfo), documentGroup As String) As Boolean
        Dim warnings As New StringBuilder

        For Each doc As DocumentInfo In documentList
            Try
                doc.Signature = Facade.CollaborationFacade.GetSignature(CurrentCollaboration.Id)
                Facade.CollaborationVersioningFacade.AddDocumentToVersioning(CurrentCollaboration, doc, documentGroup)
            Catch ex As Exception
                Dim message As String = String.Format("Errore in inserimento documento [{0}] di tipo [{1}]: {2}", doc.Name, documentGroup, ex.Message)
                FileLogger.Warn(Facade.CollaborationFacade.LoggerName, message, ex)
                warnings.AppendLine(message)
            End Try
        Next
        If warnings.Length > 0 Then
            AjaxAlert(warnings.ToString())
            Return False
        End If
        Return True
    End Function

    Private Sub EditCollaboration()
        If CurrentCollaboration Is Nothing Then
            Throw New DocSuiteException("Errore modifica", "Impossibile trovare la collaborazione.")
        End If

        If DocSuiteContext.Current.ProtocolEnv.CollaborationManagementExpired Then
            CurrentCollaboration.AlertDate = alertDate.SelectedDate
        End If

        ' Controllo la presenza di destinatari di protocollazione
        If uscRestituzioni.TreeViewControl.Nodes(0).Nodes.Count = 0 AndAlso uscSettoriSegreterie.GetCheckedRoles().IsNullOrEmpty() Then
            Throw New DocSuiteException("Errore modifica", String.Format("Nessun Destinatario di {0} selezionato.", DocumentTypeCaption(ddlDocumentType.SelectedValue)))
        End If

        ' controllo la presenza di contatti da distribuire
        Dim distributionContacts As List(Of CollaborationContact) = Nothing
        If SignersEditEnabled OrElse CheckModifyRight Then
            distributionContacts = GetManualContacts(uscVisioneFirma.TreeViewControl.Nodes(0))
            If (ProtocolEnv.CollaborationProposerAsSignerEnabled AndAlso
                Not distributionContacts.First().Account.Eq(CurrentCollaboration.RegistrationUser)) Then
                Dim proposerAsSigner As CollaborationContact = New CollaborationContact()
                proposerAsSigner.Account = CurrentCollaboration.RegistrationUser
                proposerAsSigner.DestinationName = CommonAD.GetDisplayName(CurrentCollaboration.RegistrationUser)
                proposerAsSigner.DestinationEMail = Facade.UserLogFacade.EmailOfUser(CurrentCollaboration.RegistrationUser, ProtocolEnv.EnableUserProfile)
                distributionContacts.Insert(0, proposerAsSigner)
            End If
            Facade.CollaborationFacade.DeleteSigns(CurrentCollaboration)
            Facade.CollaborationFacade.InsertDistribution(CollaborationId.Value, distributionContacts)
            FileLogger.Info(LoggerName, String.Concat("Modificati firmatari collaborazione n.", CurrentCollaboration.Id))
        End If

        Dim tContactR As List(Of CollaborationContact) = GetManualContacts(uscRestituzioni.TreeViewControl.Nodes(0))

        ' Settori ai quali avviene la distribuzione
        Dim destinationRoles As IList(Of Role) = uscSettoriSegreterie.GetCheckedRoles()
        If destinationRoles.IsNullOrEmpty() AndAlso tContactR.IsNullOrEmpty() Then
            Throw New DocSuiteException("Errore modifica", String.Format("Nessun Destinatario di {0} selezionato.", DocumentTypeCaption(ddlDocumentType.SelectedValue)))
        End If
        Dim restitutionRoles As List(Of CollaborationUser) = destinationRoles.Select(Function(destinationRole) New CollaborationUser(destinationRole) With {.DestinationFirst = True}).ToList()
        ' Altri settori
        restitutionRoles.AddRange(From role In uscSettoriSegreterie.GetUncheckedRoles() Select New CollaborationUser(role) With {.DestinationFirst = False})

        Facade.CollaborationFacade.DeleteUsers(CollaborationId.Value)
        Facade.CollaborationFacade.Update(CurrentCollaboration, rblPriority.SelectedValue, txtDate.SelectedDate, txtObject.Text, txtNote.Text, CollaborationStatusType.IN, Nothing, tContactR, restitutionRoles, Nothing, Nothing, Nothing, Nothing, "", 0, False)
        FileLogger.Info(LoggerName, String.Concat("Modificate segreterie collaborazione n.", CurrentCollaboration.Id))
        Facade.CollaborationLogFacade.Insert(CurrentCollaboration, "Modificata collaborazione")

        Facade.CollaborationFacade.SendMail(CurrentCollaboration, CollaborationMainAction.Modifica)
        If (ProtocolEnv.CollaborationProposerAsSignerEnabled AndAlso (SignersEditEnabled OrElse CheckModifyRight)) AndAlso
            distributionContacts.First().Account.Eq(CurrentCollaboration.RegistrationUser) Then
            Facade.CollaborationSignsFacade.UpdateForward(CurrentCollaboration)
            Facade.CollaborationLogFacade.Insert(CurrentCollaboration, "Avanzamento automatico al destinatario successivo")
            Facade.CollaborationFacade.UpdateBiblosSignsModel(CurrentCollaboration)
            Facade.CollaborationFacade.SendMail(CurrentCollaboration, CollaborationMainAction.DaVisionareFirmare)
        End If
    End Sub

    Public Function CheckRequiredSign() As Boolean

        Dim lst As IList(Of CollaborationSign) = CurrentCollaboration.GetRequiredSigns()
        If lst.Count <= 0 Then
            Return True
        End If

        Dim sMsg As New StringBuilder
        For Each cs As CollaborationSign In lst
            sMsg.AppendFormat("{0},", cs.SignName)
        Next

        If sMsg.Length <> 0 Then
            sMsg.Remove(sMsg.Length - 1, 1)
        End If

        AjaxAlert("Il documento deve essere firmato da {0}", sMsg.ToString())
        Return False
    End Function

    Public Sub ExistDocumentToSign(ByVal ajaxOperation As String, ByVal e As [Delegate])
        If Not Facade.CollaborationVersioningFacade.CheckUserDocumentsSign(New Integer() {CollaborationId.Value}) AndAlso
            (PopUpDocumentNotSignedAlertEnabled OrElse (Not PopUpDocumentNotSignedAlertEnabled AndAlso CurrentSigner.IsRequired)) Then
            Dim confirmMessage As String = "Documento non firmato. Si desidera proseguire comunque?"
            If DocSuiteContext.Current.ProtocolEnv.ForceProsecutable Then
                confirmMessage &= "\nQuesta operazione rimuoverà eventuali obbligatorietà di firma."
            End If

            AjaxAlertConfirm(confirmMessage, "ExecuteAjaxRequest('" & ajaxOperation & "');", Nothing)
        Else
            e.DynamicInvoke()
        End If
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    ''' <summary> Procedura di inoltro da segreteria </summary>
    Private Sub ExecuteForward(Optional absentManagers As Boolean = False)
        If Not CurrentCollaboration.Prosecutable Then
            If DocSuiteContext.Current.ProtocolEnv.ForceProsecutable Then
                ' Permetto all'utente di disabilitare l'obbligatorietà di firma e proseguire comunque.
                FacadeFactory.Instance.CollaborationSignsFacade.SkipRequiredSigns(CurrentCollaboration)
            Else
                AjaxAlert("La firma del documento è obbligatoria, impossibile proseguire.")
                Exit Sub
            End If
        End If

        If DocSuiteContext.Current.ProtocolEnv.CollaborationManagementExpired Then
            CurrentCollaboration.AlertDate = alertDate.SelectedDate
        End If

        Dim forwardContacts As List(Of CollaborationContact) = GetManualContacts(uscInoltro.TreeViewControl.Nodes(0))

        Dim flgInoltro As Boolean = False
        If (CurrentCollaboration IsNot Nothing) AndAlso Facade.CollaborationSignsFacade.HasNext(CollaborationId.Value) Then
            forwardContacts = Nothing
            flgInoltro = True
        End If

        Dim restitutionContacts As List(Of CollaborationContact) = Nothing
        Dim restitutionRoles As New List(Of CollaborationUser)
        If pnlRestituzioni.Visible Then
            Facade.CollaborationFacade.DeleteUsers(CollaborationId.Value)

            restitutionContacts = GetManualContacts(uscRestituzioni.TreeViewControl.Nodes(0))

            ' Settori ai quali avviene la distribuzione
            Dim destinationRoles As IList(Of Role) = uscSettoriSegreterie.GetCheckedRoles()
            If destinationRoles.IsNullOrEmpty() AndAlso (restitutionContacts.IsNullOrEmpty() OrElse Not restitutionContacts.Any(Function(rc) rc.DestinationFirst)) Then
                Throw New DocSuiteException("Errore inoltro", String.Format("Nessun Destinatario di {0} selezionato.", DocumentTypeCaption(ddlDocumentType.SelectedValue)))
            End If
            For Each destinationRole As Role In destinationRoles
                restitutionRoles.Add(New CollaborationUser(destinationRole) With {.DestinationFirst = True})
            Next
            ' Altri settori
            For Each role As Role In uscSettoriSegreterie.GetUncheckedRoles()
                restitutionRoles.Add(New CollaborationUser(role) With {.DestinationFirst = False})
            Next
        End If

        Dim idDocument As Integer = 0
        Dim documentName As String = String.Empty
        Try
            If rblTipoOperazione.SelectedValue.Eq("IS") Then
                Facade.CollaborationFacade.Update(CurrentCollaboration, rblPriority.SelectedValue, txtDate.SelectedDate, txtObject.Text, txtNote.Text, Nothing, Nothing, restitutionContacts, restitutionRoles, Nothing, Nothing, Nothing, Nothing, "", 0, False)

                Facade.CollaborationSignsFacade.UpdateForwardSecretary(CurrentCollaboration, "IN", forwardContacts)
                FacadeFactory.Instance.CollaborationLogFacade.Insert(CurrentCollaboration, "Collaborazione inoltrata alle segreterie")
            Else
                If flgInoltro Then
                    Facade.CollaborationSignsFacade.UpdateForward(CurrentCollaboration)
                End If
                If absentManagers AndAlso Not CurrentCollaboration.CollaborationSigns.Any(Function(s) s.Incremental > CurrentCollaborationSign.Incremental AndAlso (Not s.IsAbsent.HasValue OrElse Not s.IsAbsent.Value)) Then
                    Facade.CollaborationFacade.Update(CurrentCollaboration, rblPriority.SelectedValue, txtDate.SelectedDate, txtObject.Text, txtNote.Text, CollaborationStatusType.DP, Nothing, restitutionContacts, restitutionRoles, forwardContacts, Nothing, Nothing, Nothing, "", 0, False)
                    Facade.CollaborationFacade.SendMail(CurrentCollaboration, CollaborationMainAction.DaProtocollareGestire)
                    FacadeFactory.Instance.CollaborationLogFacade.Insert(CurrentCollaboration, "Avanzamento al protocollo/segreteria")
                Else
                    Facade.CollaborationFacade.Update(CurrentCollaboration, rblPriority.SelectedValue, txtDate.SelectedDate, txtObject.Text, txtNote.Text, Nothing, Nothing, restitutionContacts, restitutionRoles, forwardContacts, Nothing, Nothing, Nothing, "", 0, False)
                    Facade.CollaborationFacade.SendMail(CurrentCollaboration, CollaborationMainAction.DaVisionareFirmare)
                    FacadeFactory.Instance.CollaborationLogFacade.Insert(CurrentCollaboration, "Prosegui")
                End If
                If idDocument <> 0 Then
                    Facade.CollaborationVersioningFacade.InsertDocument(CollaborationId.Value, idDocument, documentName)
                End If
            End If

            If DocSuiteContext.Current.ProtocolEnv.ForceCollaborationSignDateEnabled Then
                Facade.CollaborationFacade.UpdateBiblosSignsModel(CurrentCollaboration)
            End If

            ' Notifiche 

            ' SSE eseguo il comando "Inoltra" in Attivita in corso, notifico l'operazione
            If Action.Eq(CollaborationSubAction.AttivitaInCorso) Then
                Facade.CollaborationFacade.SendMail(CurrentCollaboration, CollaborationMainAction.NotificaStep)
            End If
            If Not absentManagers Then
                Response.Redirect(String.Format("UserCollRisultati.aspx?Title={0}&Action={1}", Title2, Action2))
            End If

        Catch exception As DocSuiteException
            Throw
        Catch ex As Exception
            FileLogger.Error(Facade.CollaborationFacade.LoggerName, String.Format("Errore inoltro per collaborazione [{0}].", CurrentCollaboration.Id), ex)
            AjaxAlert("Errore inoltro. Contattare l'assistenza.")
            btnInoltra.Enabled = True
        End Try
    End Sub

    ''' <summary>Visione/Protocolla Operation</summary>
    Private Sub ExecuteVisioneProtocolla()
        ' TODO: verificare pezzo di codice perso per strada (in errore documento idDocumuent è sempre = 0)
        Dim idDocument As Integer = 0
        Dim DocumentName As String = ""

        If DocSuiteContext.Current.ProtocolEnv.CollaborationManagementExpired Then
            CurrentCollaboration.AlertDate = alertDate.SelectedDate
        End If

        Try
            Dim tContactD As New List(Of CollaborationContact)
            Facade.CollaborationFacade.Update(CurrentCollaboration, rblPriority.SelectedValue, txtDate.SelectedDate, txtObject.Text, txtNote.Text, Nothing, Nothing, tContactD, Nothing, Nothing, Nothing, Nothing, Nothing, "", 0, False)
        Catch ex As Exception
            If idDocument <> 0 Then
                Facade.CollaborationVersioningFacade.InsertDocument(CollaborationId.Value, idDocument, DocumentName)
            End If
            FileLogger.Warn(Facade.CollaborationFacade.LoggerName, ex.Message, ex)
            AjaxAlert("Errore in Aggiornamento Dati:" & ex.Message)
            Exit Sub
        End Try

        'protocolla
        Dim params As String = "Action=FromCollaboration&IdCollaboration=" & CollaborationId
        Dim script As String = String.Empty
        Select Case CurrentCollaboration.DocumentType
            Case CollaborationDocumentType.P.ToString()
                script = "../Prot/ProtInserimento.aspx?" & CommonShared.AppendSecurityCheck(params)
            Case CollaborationDocumentType.D.ToString(), CollaborationDocumentType.A.ToString()
                script = "../Resl/ReslInserimento.aspx?" & CommonShared.AppendSecurityCheck(params & "&Type=Resl")
            Case CollaborationDocumentType.S.ToString()
                script = "../Series/CollaborationToSeries.aspx?" & CommonShared.AppendSecurityCheck(params & "&Type=Series")
            Case CollaborationDocumentType.UDS.ToString()
                script = String.Concat("../UDS/CollaborationToUDS.aspx?", params, "&Type=UDS")
        End Select

        If Integer.TryParse(CurrentCollaboration.DocumentType, 0) Then
            script = String.Concat("../UDS/CollaborationToUDS.aspx?", params, "&Type=UDS&UDSEnvironment=", CurrentCollaboration.DocumentType)
        End If

        Response.Redirect(script)
    End Sub

    ''' <summary> Restituzione Operation </summary>
    Private Sub ExecuteRestitution()

        ' Se sono l'ultimo firmatario oppure l'unico firmatario e ho il capo isRequired valorizzato 1, impedisco all'utente di proseguire senza firma. 
        If Not CurrentCollaboration.Prosecutable Then
            If DocSuiteContext.Current.ProtocolEnv.ForceProsecutable Then
                ' Permetto all'utente di disabilitare l'obbligatorietà di firma e proseguire comunque.
                FacadeFactory.Instance.CollaborationSignsFacade.SkipRequiredSigns(CurrentCollaboration)
            Else
                AjaxAlert("La firma del documento è obbligatoria, impossibile proseguire.")
                Exit Sub
            End If
        End If

        If DocSuiteContext.Current.ProtocolEnv.CollaborationManagementExpired Then
            CurrentCollaboration.AlertDate = alertDate.SelectedDate
        End If

        Dim restitutionContacts As List(Of CollaborationContact) = Nothing
        Dim restitutionRoles As New List(Of CollaborationUser)
        If pnlRestituzioni.Visible Then
            restitutionContacts = GetManualContacts(uscRestituzioni.TreeViewControl.Nodes(0))

            ' Settori ai quali avviene la distribuzione
            Dim destinationRoles As IList(Of Role) = uscSettoriSegreterie.GetCheckedRoles()
            For Each destinationRole As Role In destinationRoles
                restitutionRoles.Add(New CollaborationUser(destinationRole) With {.DestinationFirst = True})
            Next
            ' Altri settori
            For Each role As Role In uscSettoriSegreterie.GetUncheckedRoles()
                restitutionRoles.Add(New CollaborationUser(role) With {.DestinationFirst = False})
            Next

            ' Controllo che ci sia almeno un destinatario selezionato
            If destinationRoles.IsNullOrEmpty() AndAlso (restitutionContacts.IsNullOrEmpty() OrElse Not restitutionContacts.Any(Function(rc) rc.DestinationFirst)) Then
                Throw New DocSuiteException("Errore restituzione", String.Format("Nessun Destinatario di {0} selezionato.", DocumentTypeCaption(ddlDocumentType.SelectedValue)))
            End If
            ' Se la validazione dei nuovi "restitutari" va a buon fine cancello i vecchi
            Try
                ' TODO: rifattorizzare e mettere questi metodi in transazione
                Facade.CollaborationFacade.DeleteUsers(CollaborationId.Value)
            Catch ex As Exception
                FileLogger.Error(Facade.CollaborationFacade.LoggerName, String.Format("Errore cancellazione utenti per restituzione collaborazione [{0}].", CurrentCollaboration.Id), ex)
                AjaxAlert("Errore cancellazione utenti per restituzione. Contattare l'assistenza.")
                Exit Sub
            End Try
        End If

        Dim idDocument As Integer = 0
        Dim documentName As String = ""
        Try
            If DocSuiteContext.Current.ProtocolEnv.ForceCollaborationSignDateEnabled Then
                Dim activeSigner As CollaborationSign = CurrentCollaboration.GetFirstCollaborationSignActive()
                If Not activeSigner.SignDate.HasValue Then
                    activeSigner.SignDate = Date.Now
                    Facade.CollaborationSignsFacade.UpdateOnly(activeSigner)
                End If
                Facade.CollaborationFacade.UpdateBiblosSignsModel(CurrentCollaboration)
            End If

            Facade.CollaborationFacade.Update(CurrentCollaboration, rblPriority.SelectedValue, txtDate.SelectedDate, txtObject.Text, txtNote.Text, CollaborationStatusType.DP, Nothing, restitutionContacts, restitutionRoles, Nothing, Nothing, Nothing, Nothing, "", 0, False)
            FacadeFactory.Instance.CollaborationLogFacade.Insert(CurrentCollaboration, "Avanzamento al protocollo/segreteria")
            If idDocument <> 0 Then
                Facade.CollaborationVersioningFacade.InsertDocument(CollaborationId.Value, idDocument, documentName)
            End If

            Facade.CollaborationFacade.SendMail(CurrentCollaboration, CollaborationMainAction.DaProtocollareGestire)

            ' SSE eseguo il comando "al protocollo" in Attivita in corso, notifico l'operazione
            If Action.Eq(CollaborationSubAction.AttivitaInCorso) Then
                Facade.CollaborationFacade.SendMail(CurrentCollaboration, CollaborationMainAction.NotificaStep)
            End If

            Response.Redirect(String.Format("UserCollRisultati.aspx?Title={0}&Action={1}", Title2, Action2))
        Catch exception As DocSuiteException
            Throw
        Catch ex As Exception
            FileLogger.Error(Facade.CollaborationFacade.LoggerName, String.Format("Errore restituzione per la collaborazione [{0}].", CurrentCollaboration.Id), ex)
            AjaxAlert("Errore restituzione. Contattare l'assistenza.")
        End Try
    End Sub

    Private Function GetCheckedOutContent(files As List(Of DocumentInfo)) As DocumentInfo
        If files.Count = 1 Then
            Return files.First()
        End If

        Dim tempFileName As String = FileHelper.UniqueFileNameFormat(CheckedOutZipName, DocSuiteContext.Current.User.UserName)
        Dim destination As String = Path.Combine(CommonInstance.AppTempPath, tempFileName)
        Dim content As List(Of KeyValuePair(Of String, Byte())) = files.Select(Function(d) New KeyValuePair(Of String, Byte())(d.Name, d.Stream)).ToList()
        Dim compressManager As ICompress = New ZipCompress()
        compressManager.Compress(content, destination)
        Dim zipped As New FileInfo(destination)
        Dim tempInfo As New TempFileDocumentInfo(zipped) With {.Name = CheckedOutZipName}
        FileLogger.Info(LoggerName, String.Format("{0} inclusi in {1}", files.Count, tempInfo.Name))
        Return tempInfo
    End Function

    Private Shared Function GetHandlerUrl(items As NameValueCollection, name As String) As String
        Return String.Format("{0}/Viewers/Handlers/DocumentInfoHandler.ashx/{1}?{2}",
                              DocSuiteContext.Current.CurrentTenant.DSWUrl,
                             FileHelper.FileNameToUrl(name),
                             CommonShared.AppendSecurityCheck(items.AsEncodedQueryString()))
    End Function

    Private Sub StartDownload(content As DocumentInfo)
        Dim itemCollection As NameValueCollection = content.ToQueryString()
        itemCollection.Add("Download", "True")
        itemCollection.Add("Original", "True")
        AjaxManager.ResponseScripts.Add(String.Format("StartDownload('{0}');", GetHandlerUrl(itemCollection, content.Name)))
    End Sub

    Private Const excel_ext As String = "|.xlsx|.xlsm|.xlsb|.xltx|.xltm|.xlt|.xls|.xlam|.xla|.xlw|.xlr|.csv|.dif|.slk|"
    Private Const word_ext As String = "|.doc|.docm|.docx|.docx|.dot|.dotm|.dotx|.htm|.html|.mht|.mhtml|.odt|.pdf|.rtf|.txt|.wps|.xml|.xps"

    ''' <summary>
    ''' Downlaod file checkout in share di rete
    ''' </summary>
    ''' <param name="content"></param>
    ''' <remarks></remarks>
    Private Sub OpenLocalFile(content As DocumentInfo)
        Dim destination As String = Path.Combine(DocSuiteContext.Current.ProtocolEnv.VersioningShare, content.Name)
        If Not File.Exists(destination) Then
            File.WriteAllBytes(destination, content.Stream)
        End If
        Dim fileInfo As FileInfo = New FileInfo(destination)

        Dim functionName As String = "OpenAlert"
        If excel_ext.Contains(String.Concat("|", fileInfo.Extension, "|")) Then
            functionName = "OpenExcel"
        End If
        If word_ext.Contains(String.Concat("|", fileInfo.Extension, "|")) Then
            functionName = "OpenWord"
        End If

        If functionName.Eq("OpenAlert") Then
            destination = fileInfo.Extension
        End If

        Dim escaped As String = HttpUtility.JavaScriptStringEncode(destination)
        Dim jsOpenLocalFile As String = String.Format("{0}('{1}');", functionName, escaped)
        AjaxManager.ResponseScripts.Add(jsOpenLocalFile)
    End Sub

    Private Sub DeleteLocalFile(content As FileDocumentInfo)
        Dim destination As String = Path.Combine(DocSuiteContext.Current.ProtocolEnv.VersioningShare, content.FileInfo.FullName)
        If Not File.Exists(destination) Then
            Return
        End If

        Try
            File.Delete(destination)
        Catch ex As Exception
            Dim message As String = String.Format("Non è stato possibile cancellare il file dal percorso: {0}", destination)
            FileLogger.Warn(LoggerName, message, ex)
        End Try
    End Sub

    Private Sub CheckOutDocuments(selected As IList(Of DocumentInfo))
        If selected Is Nothing OrElse selected.Count = 0 Then
            AjaxAlert("Nessun documento selezionato.")
            Return
        End If
        If DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled AndAlso Not selected.HasSingle() Then
            AjaxAlert("Non è possibile estrarre più di un documento per volta.")
            Return
        End If

        Dim hasError As Boolean = False
        Dim sessions As New List(Of String)
        Dim checkedOut As New List(Of DocumentInfo)
        For Each item As DocumentInfo In selected
            Dim versioning As CollaborationVersioning = CollaborationVersioningFacade.GetVersioningByDocumentInfo(LastVersionings, item)
            If versioning Is Nothing Then
                Dim message As String = String.Format("CollaborationVersioning per [{0}] mancante o non valido.", item.Name)
                AjaxAlert(message)
                FileLogger.Warn(LoggerName, message)
                hasError = True
                Continue For
            End If
            Try
                sessions.Add(Facade.CollaborationVersioningFacade.CheckOut(versioning, DocSuiteContext.Current.User.FullUserName))

                item.Name = CollaborationVersioningFacade.CheckedOutFileNameFormat(versioning)
                checkedOut.Add(item)

            Catch ex As InvalidOperationException
                ' E' stato richiesto di ingentilire l'errore
                AjaxAlert("Il file [{0}] risulta già estratto.", versioning.DocumentName)
                FileLogger.Warn(LoggerName, "File già estratto", ex)
                hasError = True

            Catch ex As Exception
                Dim message As String = String.Format("Check Out {0} ({1}): {2}", item.Name, versioning.CheckOutSessionId, ex.Message)
                AjaxAlert(message)
                FileLogger.Error(LoggerName, message, ex)
                hasError = True

            End Try
        Next
        LastVersionings = Nothing
        FileLogger.Info(LoggerName, "Check Out completato:" & String.Join(", ", sessions))

        Dim content As DocumentInfo = GetCheckedOutContent(checkedOut)
        If DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled Then
            OpenLocalFile(content)
        Else
            StartDownload(content)
        End If

        LastVersionings = Nothing
        If hasError Then
            AjaxAlert("Non è stato possibile eseguire l'operazione.")
        End If
    End Sub

    Private Sub UndoCheckOutDocuments(selected As IList(Of DocumentInfo))
        If selected Is Nothing OrElse selected.Count = 0 Then
            AjaxAlert("Nessun documento selezionato.")
            Return
        End If

        Dim skipped As New List(Of DocumentInfo)
        For Each item As DocumentInfo In selected
            Dim versioning As CollaborationVersioning = CollaborationVersioningFacade.GetVersioningByDocumentInfo(LastVersionings, item)
            If versioning Is Nothing Then
                Dim message As String = String.Format("CollaborationVersioning per {0} mancante o non valido.", item.Name)
                AjaxAlert(message)
                FileLogger.Warn(LoggerName, message)
                skipped.Add(item)
                Continue For
            End If
            Try
                Dim checkedOut As FileDocumentInfo = FacadeFactory.Instance.CollaborationVersioningFacade.GetLocalCheckedOutDocument(versioning)
                FacadeFactory.Instance.CollaborationVersioningFacade.UndoCheckOut(versioning, DocSuiteContext.Current.User.FullUserName)
                DeleteLocalFile(checkedOut)
            Catch ex As Exception
                Dim message As String = String.Format("Check Out {0} ({1}): {2}", item.Name, versioning.CheckOutSessionId, ex.Message)
                AjaxAlert(message)
                FileLogger.Error(LoggerName, message, ex)
                skipped.Add(item)
                Continue For
            End Try
        Next
        LastVersionings = Nothing
    End Sub
    Private Sub CheckInDocuments(selected As IList(Of DocumentInfo))
        If selected Is Nothing OrElse selected.Count = 0 Then
            AjaxAlert("Nessun documento selezionato.")
            Return
        End If
        If DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled AndAlso Not selected.HasSingle() Then
            AjaxAlert("Non è possibile estrarre più di un documento per volta.")
            Return
        End If

        Dim skipped As New List(Of DocumentInfo)
        For Each item As DocumentInfo In selected
            Dim versioning As CollaborationVersioning = CollaborationVersioningFacade.GetVersioningByDocumentInfo(LastVersionings, item)
            If versioning Is Nothing Then
                Dim message As String = String.Format("CollaborationVersioning per {0} mancante o non valido.", item.Name)
                AjaxAlert(message)
                FileLogger.Warn(LoggerName, message)
                skipped.Add(item)
                Continue For
            End If
            Try
                Dim checkedOut As FileDocumentInfo = FacadeFactory.Instance.CollaborationVersioningFacade.GetLocalCheckedOutDocument(versioning, True)
                FacadeFactory.Instance.CollaborationVersioningFacade.CheckIn(versioning, DocSuiteContext.Current.User.FullUserName, checkedOut)
                DeleteLocalFile(checkedOut)
            Catch ex As Exception
                Dim message As String = String.Format("Check In {0} ({1}): {2}", item.Name, versioning.CheckOutSessionId, ex.Message)
                AjaxAlert(message)
                FileLogger.Error(LoggerName, message, ex)
                skipped.Add(item)
                Continue For
            End Try
        Next
        LastVersionings = Nothing
        FileLogger.Info(LoggerName, "Check In completato.")

        If skipped.Count > 0 Then
            AjaxAlert("Non è stato possibile eseguire l'operazione.")
        End If
    End Sub

    Private Function DocumentTypeCaption(ByVal documentType As String) As String
        If Integer.TryParse(documentType, 0) Then
            Return "l'archiviazione"
        End If

        Select Case documentType
            Case CollaborationDocumentType.D.ToString()
                Return Facade.ResolutionTypeFacade.DeliberaCaption

            Case CollaborationDocumentType.A.ToString()
                Return Facade.ResolutionTypeFacade.DeterminaCaption

            Case CollaborationDocumentType.S.ToString()
                Return ProtocolEnv.DocumentSeriesName

            Case CollaborationDocumentType.UDS.ToString()
                Return "l'archiviazione"

            Case Else
                Return "protocollazione"
        End Select

    End Function

    Private Function GetUscUploadByCommand(command As Button) As uscDocumentUpload
        Select Case command.ID
            Case cmdDocumentCheckOut.ID, cmdDocumentUndoCheckOut.ID, cmdDocumentCheckIn.ID
                Return uscDocumento
            Case cmdDocumentOmissisCheckOut.ID, cmdDocumentOmissisUndoCheckOut.ID, cmdDocumentOmissisCheckIn.ID
                Return uscDocumentoOmissis
            Case cmdAttachmentsCheckOut.ID, cmdAttachmentsUndoCheckOut.ID, cmdAttachmentsCheckIn.ID
                Return uscAllegati
            Case cmdAttachmentsOmissisCheckOut.ID, cmdAttachmentsOmissisUndoCheckOut.ID, cmdAttachmentsOmissisCheckIn.ID
                Return uscAllegatiOmissis
            Case cmdAnnexedCheckOut.ID, cmdAnnexedUndoCheckOut.ID, cmdAnnexedCheckIn.ID
                Return uscAnnexed
            Case Else
                Throw New NotImplementedException("Caso non previsto.")
        End Select
    End Function

    Private Function GetCheckOutCommand(documentUpload As uscDocumentUpload) As Button
        Select Case documentUpload.ID
            Case uscDocumento.ID
                Return cmdDocumentCheckOut
            Case uscDocumentoOmissis.ID
                Return cmdDocumentOmissisCheckOut
            Case uscAllegati.ID
                Return cmdAttachmentsCheckOut
            Case uscAllegatiOmissis.ID
                Return cmdAttachmentsOmissisCheckOut
            Case uscAnnexed.ID
                Return cmdAnnexedCheckOut
            Case Else
                Throw New NotImplementedException("Caso non previsto.")
        End Select
    End Function

    Private Sub BindUscUploadByCommand(command As Button)
        Select Case command.ID
            Case cmdDocumentCheckOut.ID, cmdDocumentUndoCheckOut.ID, cmdDocumentCheckIn.ID
                BindMainDocuments()
            Case cmdDocumentOmissisCheckOut.ID, cmdDocumentOmissisUndoCheckOut.ID, cmdDocumentOmissisCheckIn.ID
                BindMainDocumentsOmissis()
            Case cmdAttachmentsCheckOut.ID, cmdAttachmentsUndoCheckOut.ID, cmdAttachmentsCheckIn.ID
                BindAttachments()
            Case cmdAttachmentsOmissisCheckOut.ID, cmdAttachmentsOmissisUndoCheckOut.ID, cmdAttachmentsOmissisCheckIn.ID
                BindAttachmentsOmissis()
            Case cmdAnnexedCheckOut.ID, cmdAnnexedUndoCheckOut.ID, cmdAnnexedCheckIn.ID
                BindAnnexed()
            Case Else
                Throw New NotImplementedException("Caso non previsto.")
        End Select
    End Sub

    Private Sub BindUploadButtons(sender As Control, doc As DocumentInfo)
        If (LastVersionings IsNot Nothing) Then
            Dim versioning As CollaborationVersioning = CollaborationVersioningFacade.GetVersioningByDocumentInfo(LastVersionings, doc)
            If versioning IsNot Nothing Then
                Dim isCheckedOut As Boolean = versioning.CheckedOut.GetValueOrDefault()
                Dim isMyCheckOut As Boolean = versioning.CheckOutUser.Eq(DocSuiteContext.Current.User.FullUserName)
                Dim label As String = If(isCheckedOut, "Modifica", "Estrai")
                Select Case sender.ID
                    Case uscDocumento.ID
                        cmdDocumentCheckOut.Text = label
                        cmdDocumentUndoCheckOut.Enabled = CheckModifyRight OrElse (isCheckedOut AndAlso isMyCheckOut)
                        cmdDocumentCheckIn.Enabled = (CheckModifyRight AndAlso Action.Eq(CollaborationSubAction.AllaVisioneFirma)) OrElse (isCheckedOut AndAlso isMyCheckOut)

                    Case uscDocumentoOmissis.ID
                        cmdDocumentOmissisCheckOut.Text = label
                        cmdDocumentOmissisUndoCheckOut.Enabled = CheckModifyRight OrElse (isCheckedOut AndAlso isMyCheckOut)
                        cmdDocumentOmissisCheckIn.Enabled = CheckModifyRight OrElse (isCheckedOut AndAlso isMyCheckOut)


                    Case uscAllegati.ID
                        cmdAttachmentsCheckOut.Text = label
                        cmdAttachmentsUndoCheckOut.Enabled = CheckModifyRight OrElse (isCheckedOut AndAlso isMyCheckOut)

                        cmdAttachmentsCheckIn.Enabled = CheckModifyRight OrElse (isCheckedOut AndAlso isMyCheckOut)


                    Case uscAllegatiOmissis.ID
                        cmdAttachmentsOmissisCheckOut.Text = label
                        cmdAttachmentsOmissisUndoCheckOut.Enabled = CheckModifyRight OrElse (isCheckedOut AndAlso isMyCheckOut)

                        cmdAttachmentsOmissisCheckIn.Enabled = CheckModifyRight OrElse (isCheckedOut AndAlso isMyCheckOut)


                    Case uscAnnexed.ID
                        cmdAnnexedCheckOut.Text = label
                        cmdAnnexedUndoCheckOut.Enabled = CheckModifyRight OrElse (isCheckedOut AndAlso isMyCheckOut)

                        cmdAnnexedCheckIn.Enabled = CheckModifyRight OrElse (isCheckedOut AndAlso isMyCheckOut)
                    Case Else
                        Throw New NotImplementedException("Caso non previsto.")
                End Select

                Dim documentUpload As uscDocumentUpload = DirectCast(sender, uscDocumentUpload)
                GetCheckOutCommand(documentUpload).Enabled = CanCheckOut(documentUpload)
            End If
        End If
    End Sub


    Private Function GetSourceProtocolYear() As Short?
        If CurrentCollaboration IsNot Nothing Then
            Return CurrentCollaboration.SourceProtocolYear
        End If

        If SourceProtocol IsNot Nothing Then
            Return SourceProtocol.Year
        End If

        Return Nothing
    End Function

    Private Function GetSourceProtocolNumber() As Integer?
        If CurrentCollaboration IsNot Nothing Then
            Return CurrentCollaboration.SourceProtocolNumber
        End If

        If SourceProtocol IsNot Nothing Then
            Return SourceProtocol.Number
        End If

        Return Nothing
    End Function

    Private Function GetSourceProtocol() As Protocol
        If CurrentCollaboration IsNot Nothing Then
            Return CurrentCollaboration.SourceProtocol
        End If

        If Request.QueryString.AllKeys.Contains("SourceUniqueIdProtocol") Then
            Dim uniqueIdProtocol As Guid = Request.QueryString.GetValue(Of Guid)("SourceUniqueIdProtocol")
            Return Facade.ProtocolFacade.GetById(uniqueIdProtocol)
        End If

        Return Nothing
    End Function

    Private Sub InitializeSourceProtocol()
        If Not DocSuiteContext.Current.ProtocolEnv.CollaborationSourceProtocolEnabled OrElse Not HasSourceProtocol Then
            Return
        End If

        trSourceProtocol.Visible = True
        cmdSourceProtocol.Icon.PrimaryIconUrl = "../Comm/Images/DocSuite/Protocollo16.gif"
        cmdSourceProtocol.Icon.PrimaryIconHeight = Unit.Pixel(16)
        cmdSourceProtocol.Icon.PrimaryIconWidth = Unit.Pixel(16)
        cmdSourceProtocol.Text = FacadeFactory.Instance.ProtocolFacade.GetProtocolNumber(SourceProtocolYear.Value, SourceProtocolNumber.Value)

        cmdSourceProtocol.NavigateUrl = $"~/Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={SourceProtocol.Id}&Type=Prot")}"

        ddlDocumentType.Enabled = False
    End Sub
    Private Sub InitializePageFromWorkflow()
        If InsertFromWorkflow Then
            If PreviousPage IsNot Nothing AndAlso TypeOf PreviousPage Is ICollaborationInitializer Then
                Dim initializer As CollaborationInitializer = CType(PreviousPage, ICollaborationInitializer).GetCollaborationInitializer()
                If initializer.MainDocument IsNot Nothing Then
                    uscDocumento.LoadDocumentInfo(initializer.MainDocument, False, True, False, True)
                    uscDocumento.InitializeNodesAsAdded(False)
                End If
                If initializer.Attachments IsNot Nothing Then
                    uscAllegati.LoadDocumentInfo(initializer.Attachments, False, True, False, True)
                    uscAllegati.InitializeNodesAsAdded(True)
                End If
            End If
        End If
        If FromWorkflowUI Then
            RadScriptManager.RegisterStartupScript(Page, Page.GetType(), "InitializePageFromWorkflowUI", "$(document).ready(function() {BindingFromWorkflowUI()});", True)
        End If
    End Sub

    Private Sub InitializePageFromWorkflowUI(workflowReferenceModel As WorkflowReferenceModel, workflowStartModel As WorkflowStart)
        If workflowReferenceModel Is Nothing OrElse workflowStartModel Is Nothing Then
            Exit Sub
        End If
        Dim workflowArgument As WorkflowArgument = Nothing
        workflowArgument = workflowStartModel.Arguments.Values.SingleOrDefault(Function(f) f.Name = WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT)
        If workflowArgument IsNot Nothing AndAlso String.IsNullOrEmpty(txtObject.Text) Then
            txtObject.Text = workflowArgument.ValueString
        End If
        workflowArgument = workflowStartModel.Arguments.Values.SingleOrDefault(Function(f) f.Name = WorkflowPropertyHelper.DSW_PROPERTY_DUEDATE)
        If workflowArgument IsNot Nothing AndAlso Not txtDate.SelectedDate.HasValue Then
            txtDate.SelectedDate = workflowArgument.ValueDate
        End If
        workflowArgument = workflowStartModel.Arguments.Values.SingleOrDefault(Function(f) f.Name = WorkflowPropertyHelper.DSW_PROPERTY_PRIORITY)
        If workflowArgument IsNot Nothing AndAlso workflowArgument.ValueInt.HasValue Then
            rblPriority.SelectedValue = If(workflowArgument.ValueInt.Value = 0, "N", If(workflowArgument.ValueInt.Value = 1, "B", "A"))
        End If
        If Not String.IsNullOrEmpty(workflowReferenceModel.ReferenceModel) AndAlso workflowReferenceModel.ReferenceType = Model.Entities.Commons.DSWEnvironmentType.Fascicle Then
            Dim fascicle As Entity.Fascicles.Fascicle = JsonConvert.DeserializeObject(Of Entity.Fascicles.Fascicle)(workflowReferenceModel.ReferenceModel)
            txtNote.Text = $"Collaborazione avviata dal fascicolo {fascicle.Title} - {fascicle.FascicleObject}"
        End If

        If workflowReferenceModel.Documents IsNot Nothing Then
            Dim workflowReferenceBiblosModel As WorkflowReferenceBiblosModel = workflowReferenceModel.Documents.SingleOrDefault(Function(f) f.ChainType = Model.Entities.DocumentUnits.ChainType.MainChain AndAlso f.ArchiveChainId.HasValue AndAlso f.ArchiveDocumentId.HasValue)
            Dim temppath As New DirectoryInfo(CommonUtil.GetInstance().AppTempPath)
            Dim doc As BiblosDocumentInfo = Nothing
            Dim tempDoc As TempFileDocumentInfo = Nothing
            Dim results As List(Of DocumentInfo) = Nothing

            If workflowReferenceBiblosModel IsNot Nothing Then
                doc = New BiblosDocumentInfo(workflowReferenceBiblosModel.ArchiveDocumentId.Value, workflowReferenceBiblosModel.ArchiveChainId.Value)
                tempDoc = New TempFileDocumentInfo(workflowReferenceBiblosModel.DocumentName, BiblosFacade.SaveUniqueToTemp(doc))
                uscDocumento.LoadDocumentInfo(tempDoc, False, True, False, True)
                uscDocumento.InitializeNodesAsAdded(False)
            End If

            results = New List(Of DocumentInfo)()
            For Each workflowReferenceBiblosModel In workflowReferenceModel.Documents.Where(Function(f) f.ChainType = Model.Entities.DocumentUnits.ChainType.AttachmentsChain AndAlso f.ArchiveChainId.HasValue AndAlso f.ArchiveDocumentId.HasValue)
                doc = New BiblosDocumentInfo(workflowReferenceBiblosModel.ArchiveDocumentId.Value, workflowReferenceBiblosModel.ArchiveChainId.Value)
                results.Add(New TempFileDocumentInfo(workflowReferenceBiblosModel.DocumentName, BiblosFacade.SaveUniqueToTemp(doc)))
            Next
            If results.Any() Then
                uscAllegati.LoadDocumentInfo(results, False, True, False, True)
                uscAllegati.InitializeNodesAsAdded(True)
            End If

            results = New List(Of DocumentInfo)()
            For Each workflowReferenceBiblosModel In workflowReferenceModel.Documents.Where(Function(f) f.ChainType = Model.Entities.DocumentUnits.ChainType.AnnexedChain AndAlso f.ArchiveChainId.HasValue AndAlso f.ArchiveDocumentId.HasValue)
                doc = New BiblosDocumentInfo(workflowReferenceBiblosModel.ArchiveDocumentId.Value, workflowReferenceBiblosModel.ArchiveChainId.Value)
                results.Add(New TempFileDocumentInfo(workflowReferenceBiblosModel.DocumentName, BiblosFacade.SaveUniqueToTemp(doc)))
            Next
            If results.Any() Then
                uscAnnexed.LoadDocumentInfo(results, False, True, False, True)
                uscAnnexed.InitializeNodesAsAdded(True)
            End If
        End If

    End Sub

    Private Sub InitializePageFromDesk()
        If InsertFromDesk Then
            If CollaborationInitializerSource Is Nothing Then
                Exit Sub
            End If
            txtObject.Text = CollaborationInitializerSource.Subject
            If CollaborationInitializerSource.MainDocument IsNot Nothing Then
                uscDocumento.LoadDocumentInfo(CollaborationInitializerSource.MainDocument, False, True, False, True)
                uscDocumento.InitializeNodesAsAdded(False)
            End If

            If CollaborationInitializerSource.Attachments IsNot Nothing Then
                uscAllegati.LoadDocumentInfo(CollaborationInitializerSource.Attachments, False, True, False, True)
                uscAllegati.InitializeNodesAsAdded(True)
            End If

            If CollaborationInitializerSource.Annexed IsNot Nothing Then
                uscAnnexed.LoadDocumentInfo(CollaborationInitializerSource.Annexed, False, True, False, True)
                uscAnnexed.InitializeNodesAsAdded(True)
            End If
        End If

        CollaborationInitializerSource = Nothing
    End Sub

    Private Sub CreateNewDeskFromCollaboration()
        Dim newDesk As Desk = New Desk(DocSuiteContext.Current.User.FullUserName)
        ' Redirigo verso la pagina di modifica del tavolo
        Response.Redirect(String.Format(DESK_NEW_URL, CurrentCollaboration.Id, DeskSource.Container.Id))
    End Sub

    Private Sub ResumeDeskFromCollaboration()
        Dim deskFacade As DeskFacade = New DeskFacade(DocSuiteContext.Current.User.FullUserName)
        ' Carico i documenti di collaborazione. Tutti i documenti non firmati finiranno sul tavolo. Metto il tavolo in stato attivo
        Dim desk As Desk = DeskSource
        desk = deskFacade.InsertDocumentNotSignedFromCollaboration(Facade.CollaborationVersioningFacade.GetLastVersionDocuments(CurrentCollaboration), desk, DeskLocation)
        desk.Status = DeskState.Open
        deskFacade.Update(desk)

        ' Aggiorno lo stato della collaborazione corrente
        CurrentCollaboration.IdStatus = CollaborationMainAction.AnnullataRitornoAlTavolo
        Facade.CollaborationFacade.UpdateOnly(CurrentCollaboration)
        ' Redirigo verso la pagina di modifica del tavolo
        Response.Redirect(String.Format(DESK_RESUME_URL, desk.Id))
    End Sub

    Public Function GenerateSubjectFromUoia(UoiaCollaborations As List(Of Collaboration), currentSubject As String) As String
        If (UoiaCollaborations.IsNullOrEmpty()) Then
            Return currentSubject
        End If

        Dim collaboration As Collaboration = UoiaCollaborations.First()
        Dim draft As CollaborationXmlData = FacadeFactory.Instance.ProtocolDraftFacade.GetDataFromCollaboration(collaboration)
        If draft IsNot Nothing AndAlso draft.GetType() = GetType(ProtocolXML) Then
            Dim protocolXML As ProtocolXML = CType(draft, ProtocolXML)
            If (protocolXML IsNot Nothing) Then
                Dim contactBag As ContactBag = protocolXML.Recipients.FirstOrDefault(Function(f) f.SourceType = 0)
                If contactBag IsNot Nothing AndAlso contactBag.Contacts IsNot Nothing AndAlso contactBag.Contacts.Any() Then
                    Dim contactXML As ContactXML = contactBag.Contacts.FirstOrDefault()
                    If (contactXML IsNot Nothing) Then
                        Return String.Concat(currentSubject, " - Ditta '", contactXML.Description, "'")
                    End If
                End If
            End If
        End If

        Return currentSubject
    End Function

    Private Function CheckIsFromResolution() As Boolean
        Dim isFromResolution As Boolean = False
        If DocSuiteContext.Current.IsResolutionEnabled AndAlso ProtocolEnv.CheckResolutionCollaborationOriginEnabled Then
            Dim draft As CollaborationXmlData = FacadeFactory.Instance.ProtocolDraftFacade.GetDataFromCollaboration(CurrentCollaboration)
            If draft IsNot Nothing AndAlso draft.GetType() = GetType(ResolutionXML) Then
                Dim resolutionXML As ResolutionXML = CType(draft, ResolutionXML)
                If (resolutionXML IsNot Nothing) Then
                    isFromResolution = True
                End If
            End If
        End If
        Return isFromResolution
    End Function
    ''' <summary>
    ''' Creazione del pdf contente l'elenco delle collaborazioni di tipo uoia aggregate
    ''' </summary>
    ''' <param name="UoiaCollaborations"></param>
    ''' <returns>Il file Info del pdf generato</returns>
    Public Function CreateMainDocumentUoia(ByVal UoiaCollaborations As List(Of Collaboration), destinatario As IList(Of RoleUser), parametri As Dictionary(Of String, String)) As DocumentInfo

        Dim path As String = HttpContext.Current.Server.MapPath("../Report/Collaboration/CollaborationUoia.rdlc")


        ' Salvataggio su file temporaneo
        Dim title As String = String.Format("Verbali di verifica periodica impianti {0}.pdf", DocSuiteContext.Current.ProtocolEnv.CorporateName)
        'Dim fileName As String = String.Format("{0}{1}.pdf", CommonUtil.GetInstance().AppTempPath, Title)
        Dim temppath As New DirectoryInfo(CommonUtil.GetInstance().AppTempPath)
        Dim doc As DocumentInfo
        Dim tempDoc As TempFileDocumentInfo
        Try
            doc = ReportFacade.GenerateReport(Of Collaboration)(path, parametri, UoiaCollaborations).DoPrint(title)
            tempDoc = New TempFileDocumentInfo(title, BiblosFacade.SaveUniqueToTemp(doc))
        Catch ex As Exception
            Throw New DocSuiteException("Creazione Stampa UOIA", String.Format("Impossibile creare il main document della collaborazione Uoia, verificare i dati. {0}", ex))

        End Try

        Return tempDoc

    End Function

    ''' <summary>
    ''' Approvo o rifiuto una collaborazione di tipo Workflow
    ''' </summary>
    ''' <param name="approved"></param>
    Private Sub ApproveRefuseWorkflowCollaboration(approved As Boolean)
        If approved AndAlso Not CheckAttachmentRequired() Then
            AjaxAlert("Attenzione! deve essere inserito obbligatoriamente un nuovo allegato prima di approvare o rifiutare la collaborazione")
            Return
        End If

        Dim dsw_p_SignerModel As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL, CurrentWorkflowActivity.UniqueId)

        Dim collaborationSignerModels As List(Of CollaborationSignerWorkflowModel) = New List(Of CollaborationSignerWorkflowModel)
        If dsw_p_SignerModel IsNot Nothing AndAlso Not String.IsNullOrEmpty(dsw_p_SignerModel.ValueString) Then
            collaborationSignerModels = JsonConvert.DeserializeObject(Of List(Of CollaborationSignerWorkflowModel))(dsw_p_SignerModel.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
        End If

        Dim currentSignPosition As Short = CurrentCollaboration.CollaborationSigns.Where(Function(x) Convert.ToBoolean(x.IsActive)).Single().Incremental
        Dim dsw_p_SignerPosition As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_POSITION, CurrentWorkflowActivity.UniqueId)
        If dsw_p_SignerPosition.ValueInt.Value + 1 <> currentSignPosition Then
            Throw New Exception($"Errore critico nelle strutture interne del workflow rispetto alla collaborazione [dsw_p_SignerPosition = {dsw_p_SignerPosition.ValueInt.Value}, CollaborationSignsIncremental = {currentSignPosition}]")
        End If

        collaborationSignerModels = WorkflowBuildApprovedModel(currentSignPosition, approved, collaborationSignerModels)
        dsw_p_SignerModel.ValueString = JsonConvert.SerializeObject(collaborationSignerModels, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)

        Dim workflowNotify As WorkflowNotify = New WorkflowNotify(CurrentWorkflowActivity.UniqueId) With {
                    .WorkflowName = CurrentWorkflowInstance.WorkflowRepository.Name}
        workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL, New WorkflowArgument() With {
                                                   .Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL,
                                                   .PropertyType = ArgumentType.Json,
                                                   .ValueString = dsw_p_SignerModel.ValueString})
        Dim webApiHelper As WebAPIHelper = New WebAPIHelper()
        If Not WebAPIImpersonatorFacade.ImpersonateSendRequest(webApiHelper, workflowNotify, DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, DocSuiteContext.Current.CurrentTenant.OriginalConfiguration) Then
            Throw New Exception("ApproveRefuseWorkflowCollaboration is not correctly evaluated from WebAPI. See specific error in WebAPI logger")
        End If

        'Ad aggiornamento avvenuto disabilito i pulsanti
        btnInoltra.Enabled = Not approved
        btnRifiuta.Enabled = approved
        btnCancella.Enabled = approved

        MasterDocSuite.WizardActionColumn.SetDisplay(True)
        AjaxManager.ResponseScripts.Add("toCompleteStep();")
    End Sub

    Protected Sub WorkflowConfirmed(sender As Object, e As EventArgs)
        If CurrentCollaboration.IdWorkflowInstance.HasValue AndAlso CurrentSigner.IsRequired.GetValueOrDefault(False) Then
            If Not CheckAttachmentRequired() Then
                AjaxAlert("Attenzione! deve essere inserito obbligatoriamente un nuovo allegato prima di completare l'attività")
                Return
            End If
        End If

        Try
            'Update collaboration model property
            Dim mapper As MapperCollaborationModel = New MapperCollaborationModel()
            Dim collaborationModel As CollaborationModel = mapper.MappingDTO(CurrentCollaboration)
            Dim serializedCollaborationModel As String = JsonConvert.SerializeObject(collaborationModel, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)

            If CurrentWorkflowActivity IsNot Nothing AndAlso (CurrentWorkflowActivity.Status = WorkflowStatus.Todo OrElse CurrentWorkflowActivity.Status = WorkflowStatus.Progress) Then
                Dim dsw_p_SignerModel As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL, CurrentWorkflowActivity.UniqueId)
                Dim dsw_p_SignerPosition As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_POSITION, CurrentWorkflowActivity.UniqueId)
                Dim workflowNotify As WorkflowNotify = New WorkflowNotify(CurrentWorkflowActivity.UniqueId) With {
                    .WorkflowName = CurrentWorkflowInstance.WorkflowRepository.Name
                }
                workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, New WorkflowArgument() With {
                                                   .Name = WorkflowPropertyHelper.DSW_PROPERTY_MODEL,
                                                   .PropertyType = ArgumentType.Json,
                                                   .ValueString = serializedCollaborationModel})
                workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL, New WorkflowArgument() With {
                                                   .Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL,
                                                   .PropertyType = ArgumentType.Json,
                                                   .ValueString = dsw_p_SignerModel.ValueString})
                workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE, New WorkflowArgument() With {
                                                   .Name = WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE,
                                                   .PropertyType = ArgumentType.PropertyBoolean,
                                                   .ValueBoolean = True})
                workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_POSITION, New WorkflowArgument() With {
                                                   .Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_POSITION,
                                                   .PropertyType = ArgumentType.PropertyInt,
                                                   .ValueInt = dsw_p_SignerPosition.ValueInt.Value + 1})
                Dim webApiHelper As WebAPIHelper = New WebAPIHelper()
                If Not WebAPIImpersonatorFacade.ImpersonateSendRequest(webApiHelper, workflowNotify, DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, DocSuiteContext.Current.CurrentTenant.OriginalConfiguration) Then
                    Throw New Exception("CollaborationWorkflowConfirmed is not correctly evaluated from WebAPI. See specific error in WebAPI logger")
                End If
                Facade.CollaborationFacade.Evict(CurrentCollaboration)
                Dim reloadedCollaboration As Collaboration = Facade.CollaborationFacade.GetById(CurrentCollaboration.Id)
                If reloadedCollaboration.IdStatus.Eq("WM") Then
                    Facade.CollaborationFacade.UpdateBiblosSignsModel(CurrentCollaboration)
                    Response.Redirect("~/User/UserWorkflow.aspx?Type=Comm")
                End If

                Dim signerStatus As WorkflowCollaborationSignerStatus = GetWorkflowCollaborationSignerStatus()
                If signerStatus.Equals(WorkflowCollaborationSignerStatus.Refused) Then
                    Facade.ProtocolDraftFacade.DeleteFromCollaboration(reloadedCollaboration)
                    Facade.CollaborationFacade.Delete(reloadedCollaboration)
                Else
                    Dim currentSigner As CollaborationSign = reloadedCollaboration.CollaborationSigns.SingleOrDefault(Function(f) f.IsActive = 1S AndAlso f.SignUser.Eq(DocSuiteContext.Current.User.FullUserName) AndAlso Not f.SignDate.HasValue)
                    If currentSigner IsNot Nothing Then
                        currentSigner.SignDate = Date.UtcNow
                        Facade.CollaborationSignsFacade.Update(currentSigner)
                    End If
                    Facade.CollaborationFacade.NextStep(New List(Of Integer) From {reloadedCollaboration.Id})
                End If
            End If
            Response.Redirect("~/User/UserWorkflow.aspx?Type=Comm")
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            AjaxAlert("Completamento attività di workflow non riuscita.")
        End Try
    End Sub

    Private Sub UDSSummaryExpander()
        If uscUDS.Visible Then
            btnExpandUDS.ImageUrl = ImagePath.SmallExpand
            btnExpandUDS.ToolTip = "Espandi"
            uscUDS.Visible = False
        Else
            btnExpandUDS.ImageUrl = ImagePath.SmallShrink
            btnExpandUDS.ToolTip = "Comprimi"
            uscUDS.Visible = True
            If uscUDS.UDSItemSource Is Nothing Then
                uscUDS.CurrentUDSRepositoryId = CurrentUDSRepository.Id
                uscUDS.UDSItemSource = CurrentUDS.UDSModel
                uscUDS.UDSYear = CurrentUDS.Year
                uscUDS.UDSNumber = CurrentUDS.Number
                uscUDS.UDSRegistrationDate = CurrentUDS.RegistrationDate
                uscUDS.UDSRegistrationUser = CurrentUDS.RegistrationUser
                uscUDS.UDSLastChangedDate = CurrentUDS.LastChangedDate
                uscUDS.UDSLastChangedUser = CurrentUDS.LastChangedUser
                uscUDS.UDSCategory = CurrentUDS.Category
                uscUDS.UDSSubject = CurrentUDS.Subject
                uscUDS.UDSAuthorizations = CurrentUDS.Authorizations
                uscUDS.UDSId = CurrentUDS.Id
                uscUDS.RepositoryBind()
                uscUDS.RefreshDynamicControls()
            End If
        End If
    End Sub

    Private Function CheckAttachmentRequired() As Boolean
        'Recupero, se presente, la WF Property relativa al'obbligatorietà di inserimento allegato
        Dim workflowAddAttachmentRequiredProperty As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_ADD_ATTACHMENT_REQUIRED, CurrentWorkflowActivity.UniqueId)
        If workflowAddAttachmentRequiredProperty IsNot Nothing AndAlso workflowAddAttachmentRequiredProperty.ValueBoolean.HasValue AndAlso workflowAddAttachmentRequiredProperty.ValueBoolean.Value Then
            Dim workflowCollaborationModelProperty As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, CurrentWorkflowActivity.UniqueId)
            If workflowCollaborationModelProperty IsNot Nothing Then
                Dim collaborationModel As CollaborationModel = JsonConvert.DeserializeObject(Of CollaborationModel)(workflowCollaborationModelProperty.ValueString, DocSuiteContext.DefaultJsonSerializerSettings)
                Dim modelAttachmentCount As Integer = collaborationModel.CollaborationVersionings.Where(Function(x) x.DocumentGroup.Eq(VersioningDocumentGroup.Attachment)).Count()
                Dim versioningAttachmentCount As Integer = CurrentCollaboration.CollaborationVersioning.Where(Function(x) x.DocumentGroup.Eq(VersioningDocumentGroup.Attachment)).Count()
                If versioningAttachmentCount <= modelAttachmentCount Then
                    Return False
                End If
            Else
                If uscAllegati.DocumentsAddedCount = 0 Then
                    Return False
                End If
            End If
        End If
        Return True
    End Function

    Private Function IsCollaborationApprovedOrRefused() As Boolean
        If CurrentWorkflowActivity Is Nothing Then
            Return False
        End If
        Dim result As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL, CurrentWorkflowActivity.UniqueId)
        If result IsNot Nothing Then
            Dim signerModels As List(Of CollaborationSignerWorkflowModel) = JsonConvert.DeserializeObject(Of List(Of CollaborationSignerWorkflowModel))(result.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
            If signerModels.Count > 0 Then
                Return signerModels.Last().UserName.Eq(DocSuiteContext.Current.User.FullUserName)
            End If
            Return False
        End If
    End Function

    Private Function GetWorkflowCollaborationSignerStatus() As WorkflowCollaborationSignerStatus
        If CurrentWorkflowActivity Is Nothing Then
            Return WorkflowCollaborationSignerStatus.None
        End If
        Dim result As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL, CurrentWorkflowActivity.UniqueId)
        If result IsNot Nothing Then
            Dim signerModels As List(Of CollaborationSignerWorkflowModel) = JsonConvert.DeserializeObject(Of List(Of CollaborationSignerWorkflowModel))(result.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
            If signerModels.Count > 0 Then
                Dim lastSigner As CollaborationSignerWorkflowModel = signerModels.Last()
                If Not lastSigner.UserName.Eq(DocSuiteContext.Current.User.FullUserName) Then
                    Return WorkflowCollaborationSignerStatus.None
                End If

                If lastSigner.HasApproved Then
                    Return WorkflowCollaborationSignerStatus.Approved
                Else
                    Return WorkflowCollaborationSignerStatus.Refused
                End If
            End If
            Return WorkflowCollaborationSignerStatus.None
        End If
    End Function

    Private Sub SetManagersAbsence(managersAccounts As AbsentManager())

        If CurrentCollaboration.CollaborationSigns.Where(Function(s) managersAccounts.Any(Function(m) Not m.Equals(s.SignUser) AndAlso (Not s.IsAbsent.HasValue OrElse Not s.IsAbsent.Value))).Count = 0 Then
            AjaxAlert("Attenzione: tutti i direttori sono assenti, la collaborazione non ha il documento firmato.")
        End If

        If Facade.CollaborationSignsFacade.SetAbsentManagers(CurrentCollaboration, managersAccounts) Then
            If CurrentCollaborationSign.IsAbsent.HasValue AndAlso CurrentCollaborationSign.IsAbsent.Value Then
                ExecuteForward(True)
            End If
            uscVisioneFirma.TreeViewControl.Nodes(0).Nodes.Clear()
            InitVisioneFirma(False, True, True)
        Else
            AjaxAlert("Errore in fase di aggiornamento di Direttori assenti.")
        End If

        btnAbsence.Enabled = Facade.CollaborationSignsFacade.GetManagerSigners(CurrentCollaboration).Any(Function(s) (Not s.IsAbsent.HasValue OrElse Not s.IsAbsent.Value) AndAlso s.Incremental >= CurrentCollaborationSign.Incremental)
        AjaxManager.ResponseScripts.Add("HideLoadingPanel();")
    End Sub

    Private Sub FillPageFromEntity(template As TemplateCollaboration)
        rblPriority.SelectedValue = template.IdPriority
        If (String.IsNullOrEmpty(txtObject.Text) OrElse txtObject.Text.Equals(hdfLastTemplateObject.Value)) Then
            txtObject.Text = template.Object
        End If
        hdfLastTemplateObject.Value = template.Object

        If (String.IsNullOrEmpty(txtNote.Text) OrElse txtNote.Text.Equals(hdfLastTemplateNote.Value)) Then
            txtNote.Text = template.Note
        End If
        hdfLastTemplateNote.Value = template.Note

        uscSettoriSegreterie.RemoveAllRoles()
        For Each secretary As TemplateCollaborationUser In template.TemplateCollaborationUsers.Where(Function(x) x.UserType.Equals(TemplateCollaborationUserType.Secretary) AndAlso x.Role IsNot Nothing).OrderBy(Function(o) o.Incremental)
            Dim nhRole As Role = Facade.RoleFacade.GetById(secretary.Role.EntityShortId)
            If nhRole IsNot Nothing Then
                uscSettoriSegreterie.AddRole(nhRole, True, False, False, secretary.IsRequired)
            End If
        Next

        uscVisioneFirma.DataSource = Nothing
        uscVisioneFirma.DataBind()
        For Each signer As TemplateCollaborationUser In template.TemplateCollaborationUsers.Where(Function(x) x.UserType.Equals(TemplateCollaborationUserType.Signer) AndAlso Not String.IsNullOrEmpty(x.Account)).OrderBy(Function(o) o.Incremental)
            Dim user As AccountModel = CommonAD.GetAccount(signer.Account)
            Dim roleUserId As String = String.Empty
            Dim description As String = String.Empty
            Dim email As String = String.Empty
            If user IsNot Nothing Then
                description = user.DisplayName
                email = user.Email
            End If

            If signer.Role IsNot Nothing Then
                roleUserId = signer.Role.EntityShortId.ToString()
                Dim roleUser As RoleUser = Facade.RoleUserFacade.GetByRoleIdAndAccount(signer.Role.EntityShortId, signer.Account, True).FirstOrDefault()
                If roleUser IsNot Nothing Then
                    description = Facade.CollaborationFacade.GetSignerDescription(roleUser.Description, roleUser.Account, template.DocumentType)
                    If Not String.IsNullOrEmpty(roleUser.Email) Then
                        email = roleUser.Email
                    End If
                End If
            End If

            If String.IsNullOrEmpty(description) AndAlso String.IsNullOrEmpty(email) Then
                FileLogger.Warn(LoggerName, String.Concat("Non sono stati trovati riferimenti per l'account ", signer.Account))
                description = signer.Account
            End If
            uscVisioneFirma.AddCollaborationContact(ContactType.AdAmPerson, description, email, signer.Account, String.Empty, roleUserId, True, signer.IsRequired, fromTemplate:=True)
        Next

        uscRestituzioni.DataSource = Nothing
        uscRestituzioni.DataBind()
        For Each contactRestitution As TemplateCollaborationUser In template.TemplateCollaborationUsers.Where(Function(x) x.UserType.Equals(TemplateCollaborationUserType.Person) AndAlso Not String.IsNullOrEmpty(x.Account)).OrderBy(Function(o) o.Incremental)
            Dim user As AccountModel = CommonAD.GetAccount(contactRestitution.Account)
            Dim description As String = String.Empty
            Dim email As String = String.Empty
            If user IsNot Nothing Then
                description = user.DisplayName
                email = user.Email
            End If

            If String.IsNullOrEmpty(description) AndAlso String.IsNullOrEmpty(email) Then
                FileLogger.Warn(LoggerName, String.Concat("Non sono stati trovati riferimenti per l'account ", contactRestitution.Account))
                description = contactRestitution.Account
            End If
            uscRestituzioni.AddCollaborationContact(ContactType.AdAmPerson, description, email, contactRestitution.Account, "G", String.Empty, True, contactRestitution.IsRequired, fromTemplate:=True)
        Next
        pnlDocumentUnitDraftEditor.Visible = False
        If Not template.JsonParameters Is Nothing Then
            Dim collaborationParameter As CollaborationParameter = New CollaborationParameter(template.Name)
            pnlDocumentUnitDraftEditor.Visible = template.DocumentType.Eq(CollaborationDocumentType.P.ToString()) AndAlso collaborationParameter.DocumentUnitDraftEditorEnabled
        End If
    End Sub

    Private Function GetCollaborationTemplates(isLocked As Boolean, onlyAuthorized As Boolean, Optional documentType As String = "") As ICollection(Of TemplateCollaboration)
        Dim results As ICollection(Of WebAPIDto(Of TemplateCollaboration)) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentTemplateCollaborationFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.OnlyAuthorized = onlyAuthorized
                        finder.Locked = isLocked
                        finder.Status = TemplateCollaborationStatus.Active
                        finder.UserName = DocSuiteContext.Current.User.UserName
                        finder.Domain = DocSuiteContext.Current.User.Domain
                        finder.SortExpressions.AddSafe("Entity.Name", "ASC")
                        If Not String.IsNullOrEmpty(documentType) Then
                            finder.DocumentType = documentType
                        End If
                        Return finder.DoSearch()
                    End Function)

        If results IsNot Nothing Then
            Return results.Select(Function(s) s.Entity).ToList()
        End If
        Return New List(Of TemplateCollaboration)
    End Function

    Private Sub SetDocumentSignPdfManage()
        uscDocumento.CloseAfterSignPdfConversion = True
        uscDocumentoOmissis.CloseAfterSignPdfConversion = True
        uscAllegati.CloseAfterSignPdfConversion = True
        uscAllegatiOmissis.CloseAfterSignPdfConversion = True
        uscAnnexed.CloseAfterSignPdfConversion = True
    End Sub

    Private Function GetControlTemplateDocumentVisibility(chainType As Entity.DocumentUnits.ChainType) As Boolean
        If Not ProtocolEnv.TemplateDocumentVisibilities.Any(Function(x) x.Name.Eq(NameOf(Collaboration))) Then
            Return False
        End If

        Dim modelVisibility As TemplateDocumentVisibilityConfiguration = ProtocolEnv.TemplateDocumentVisibilities.First(Function(x) x.Name.Eq(NameOf(Collaboration)))
        Return modelVisibility.VisibilityChains.ContainsKey(CType(chainType, Model.Entities.DocumentUnits.ChainType)) AndAlso modelVisibility.VisibilityChains(CType(chainType, Model.Entities.DocumentUnits.ChainType))
    End Function

#End Region

End Class