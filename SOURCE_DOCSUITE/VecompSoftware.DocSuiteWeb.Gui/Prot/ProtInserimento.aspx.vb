Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights.PEC
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Workflows
Imports VecompSoftware.DocSuiteWeb.Data.Formatter
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.DTO.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Protocols
Imports VecompSoftware.DocSuiteWeb.Facade.Common.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Workflows
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Protocols
Imports VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses
Imports VecompSoftware.DocSuiteWeb.Model.Workflow
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.WebAPI
Imports VecompSoftware.Helpers.Workflow
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging
Imports UDS = VecompSoftware.Helpers.UDS
Imports WebAPIFacade = VecompSoftware.DocSuiteWeb.Facade.WebAPI

Partial Public Class ProtInserimento
    Inherits ProtBasePage

#Region " Fields "

    Private Const PrevSelectedIdContainerSessionName As String = "ProtInserimento_PrevSelectedIdContainer"
    Public Const UDS_ADDRESS_NAME As String = "API-UDSAddress"
    Private Const ODATA_EQUAL_UDSID As String = "$filter=UDSId eq {0}"

    Private _documentType As DocumentType
    Private _currentMail As PECMail
    Private _currentMailTemplate As TemplateProtocol
    Private _prevSelectedIdContainer As String
    Private _sessionQueryString As NameValueCollection
    Private _protocolBoxEnabled As Boolean?
    Private _collaborationUoia As Boolean?
    Private _currentIdCollaboration As Lazy(Of Integer?)
    Private _currentCollaboration As Lazy(Of Collaboration)

    Private _currentIdPECMail As Lazy(Of Integer?)
    Private _currentPECMail As Lazy(Of PECMail)

    Private _currentPECSourceProtocolYear As Lazy(Of Short?)
    Private _currentPECSourceProtocolNumber As Lazy(Of Integer?)
    Private _currentPECSourceProtocol As Lazy(Of Protocol)

    Private _hasPECSourceProtocol As Lazy(Of Boolean)
    Private _fromDesk As Boolean?
    Private _fromUDS As Boolean?
    Private _isPrivacy As Boolean?
    Private _currentDesk As Desk
    Private _currentUDSId As Guid?
    Private _currentUDSRepositoryId As Guid?
    Private _currentUDSRepositoryFacade As UDSRepositoryFacade
    Private _currentUDSFacade As UDSFacade
    Private _workflowPropertyFacade As WorkflowPropertyFacade
    Private _webAPIHelper As IWebAPIHelper
    Private _fascicleId As Guid?
    Private _currentUDSModel As Helpers.UDS.UDSModel
    Private _toSendProtocolType As SendDocumentUnitType?
    Private _protocolsKeys As IList(Of Guid)
    Private _currentWorkflowPropertyFinder As WorkflowPropertyFinder
    Private _currentWorkflowActivityLogFacade As VecompSoftware.DocSuiteWeb.Facade.WebAPI.Workflows.WorkflowActivityLogFacade

#End Region

#Region " Properties "

    ''' <summary> Query string in sessione </summary>
    ''' <remarks>Nothing se non è valida la richiesta o non è impostata <see>ProtocolEnv.QuerystringToSession</see></remarks>
    Private ReadOnly Property SessionQuerystring As NameValueCollection
        Get
            If _sessionQueryString Is Nothing AndAlso
                ProtocolEnv.QuerystringToSession AndAlso
                String.Compare(Request.QueryString.Item("seed"), PECView.SessionSeed, StringComparison.OrdinalIgnoreCase) = 0 Then
                ' Inizializzo la variabile
                _sessionQueryString = HttpUtility.ParseQueryString(Session(PECView.SessionSeed))
            End If
            Return _sessionQueryString
        End Get
    End Property

    Private ReadOnly Property CurrentMail As PECMail
        Get
            If ProtocolEnv.IsPECEnabled AndAlso _currentMail Is Nothing Then
                Dim id As Integer = Convert.ToInt32(GetFromSource("IdPECMail"))
                If id > 0 Then
                    _currentMail = FacadeFactory.Instance.PECMailFacade.GetById(id)
                End If
            End If
            Return _currentMail
        End Get
    End Property

    Private ReadOnly Property CurrentPecMailTemplate As TemplateProtocol
        Get
            If ProtocolEnv.TemplateProtocolEnable AndAlso _currentMailTemplate Is Nothing Then
                Dim param As String = Request.QueryString("IdTemplateProtocol")
                If Not String.IsNullOrEmpty(param) Then
                    Dim idTemplate As Integer = Integer.Parse(param)
                    If idTemplate > 0 Then
                        _currentMailTemplate = Facade.TemplateProtocolFacade.GetById(idTemplate)
                        Return _currentMailTemplate
                    End If
                End If
                Return Nothing
            End If
            Return _currentMailTemplate
        End Get
    End Property

    ''' <summary> share di rete per importazione documenti </summary>
    Private ReadOnly Property SharedFolder() As DirectoryInfo
        Get
            If Not ProtocolEnv.ImportSharedFolder.Eq("-") Then

                Return New DirectoryInfo(ProtocolEnv.ImportSharedFolder)
            End If
            Return Nothing
        End Get
    End Property

    ''' <summary> tipo documento che supportano gli scatoloni </summary>
    Private Property NeedPackage() As Boolean
        Get
            If Not (ViewState("needPackage") Is Nothing) Then
                Return Convert.ToBoolean(ViewState("needPackage"))
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("needPackage") = value
        End Set
    End Property

    Private ReadOnly Property SelectedIdDocumentType() As Nullable(Of Short)
        Get
            Dim idProtocolType As Nullable(Of Short)
            If rblTipoProtocollo.SelectedItem IsNot Nothing Then
                idProtocolType = Short.Parse(rblTipoProtocollo.SelectedItem.Value)
            End If
            Return idProtocolType
        End Get
    End Property

    Private Property SelectedDocumentType() As DocumentType
        Get
            If _documentType Is Nothing Then
                Dim vId As Integer
                If Not String.IsNullOrEmpty(cboIdDocType.SelectedValue) Then
                    If Integer.TryParse(cboIdDocType.SelectedValue, vId) Then
                        _documentType = Facade.DocumentTypeFacade.GetById(vId)
                    End If
                End If
            End If
            Return _documentType
        End Get
        Set(ByVal value As DocumentType)
            cboIdDocType.SelectedValue = value.Id.ToString
        End Set
    End Property

    ''' <summary>
    ''' Elenco dei protocolli collegati che han passato i documenti al protocollo in inserimento.
    ''' </summary>
    ''' <remarks>Stringa di ID separata da virgole.</remarks>
    Public Property SessionProtInserimentoLinks As String
        Get
            If Not Session.Item("ProtInserimento-Link") Is Nothing Then
                Return Session.Item("ProtInserimento-Link").ToString()
            Else
                Return String.Empty
            End If
        End Get
        Set(value As String)
            If String.IsNullOrEmpty(value) Then
                Session.Remove("ProtInserimento-Link")
            Else
                Session.Item("ProtInserimento-Link") = value
            End If
        End Set
    End Property

    Private Property PrevSelectedIdContainer As String
        Get
            If String.IsNullOrEmpty(_prevSelectedIdContainer) AndAlso Not String.IsNullOrEmpty(CType(Session(PrevSelectedIdContainerSessionName), String)) Then
                Dim idContainer As Integer
                If Integer.TryParse(CType(Session(PrevSelectedIdContainerSessionName), String), idContainer) Then
                    _prevSelectedIdContainer = idContainer.ToString
                End If
            End If
            Return _prevSelectedIdContainer
        End Get
        Set(value As String)
            _prevSelectedIdContainer = value
            Session(PrevSelectedIdContainerSessionName) = value
        End Set
    End Property

    Private Overloads ReadOnly Property CurrentContainerControl As ContainerControl
        Get
            Return New ContainerControl(cboIdContainer, rcbContainer)
        End Get
    End Property

    Public ReadOnly Property ProtocolBoxEnabled() As Boolean
        Get
            If _protocolBoxEnabled Is Nothing Then
                _protocolBoxEnabled = Request.QueryString.GetValueOrDefault(Of Boolean)("ProtocolBox", False)
            End If
            Return _protocolBoxEnabled.Value
        End Get
    End Property

    Public ReadOnly Property CurrentIdCollaboration As Integer?
        Get
            Return _currentIdCollaboration.Value
        End Get
    End Property

    Public ReadOnly Property CurrentCollaboration As Collaboration
        Get
            Return _currentCollaboration.Value
        End Get
    End Property

    Public ReadOnly Property CurrentPECMailId As Integer?
        Get
            Return _currentIdPECMail.Value
        End Get
    End Property

    Public ReadOnly Property CurrentPECMail As PECMail
        Get
            Return _currentPECMail.Value
        End Get
    End Property

    Public ReadOnly Property CurrentPECSourceProtocolYear As Short?
        Get
            Return _currentPECSourceProtocolYear.Value
        End Get
    End Property

    Public ReadOnly Property CurrentPECSourceProtocolNumber As Integer?
        Get
            Return _currentPECSourceProtocolNumber.Value
        End Get
    End Property

    Public ReadOnly Property CurrentPECSourceProtocol As Protocol
        Get
            Return _currentPECSourceProtocol.Value
        End Get
    End Property

    Public ReadOnly Property HasPECSourceProtocol As Boolean
        Get
            Return _hasPECSourceProtocol.Value
        End Get
    End Property

    Public ReadOnly Property CurrentUserDistributionRights() As Boolean
        Get
            If Not ProtocolEnv.IsDistributionEnabled OrElse String.IsNullOrEmpty(CurrentContainerControl.SelectedValue) Then
                Return False
            End If

            Dim containerId As Integer = CType(CurrentContainerControl.SelectedValue, Integer)
            Dim check As Boolean = Facade.ContainerFacade.CheckContainerRight(containerId, DSWEnvironment.Protocol, ProtocolContainerRightPositions.DocDistribution, True)
            Return check
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

    Private ReadOnly Property CurrentDeskFromInsert As Desk
        Get
            If _currentDesk Is Nothing Then
                Dim idDesk As Guid = HttpContext.Current.Request.QueryString.GetValueOrDefault("IdDesk", Guid.Empty)
                If Not idDesk.Equals(Guid.Empty) Then
                    _currentDesk = New DeskFacade(DocSuiteContext.Current.User.FullUserName).GetById(idDesk)
                    Return _currentDesk
                End If
                Return Nothing
            End If
            Return _currentDesk
        End Get
    End Property

    Private ReadOnly Property FromUDS As Boolean
        Get
            If Not _fromUDS.HasValue Then
                _fromUDS = Action.Eq("FromUDS")
            End If
            Return _fromUDS.Value
        End Get
    End Property

    Private ReadOnly Property CurrentUDSId As Guid?
        Get
            If _currentUDSId Is Nothing Then
                Dim idUDS As Guid? = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Guid?)("IdUDS", Nothing)
                _currentUDSId = idUDS
            End If
            Return _currentUDSId
        End Get
    End Property

    Private ReadOnly Property CurrentUDSRepositoryId As Guid?
        Get
            If _currentUDSRepositoryId Is Nothing Then
                Dim idUDSRepository As Guid? = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Guid?)("IdUDSRepository", Nothing)
                _currentUDSRepositoryId = idUDSRepository
            End If
            Return _currentUDSRepositoryId
        End Get
    End Property

    Public ReadOnly Property CurrentUDSFacade As UDSFacade
        Get
            If _currentUDSFacade Is Nothing Then
                _currentUDSFacade = New UDSFacade()
            End If
            Return _currentUDSFacade
        End Get
    End Property

    Public ReadOnly Property CurrentWorkflowPropertyFacade As WorkflowPropertyFacade
        Get
            If _workflowPropertyFacade Is Nothing Then
                _workflowPropertyFacade = New WorkflowPropertyFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _workflowPropertyFacade
        End Get
    End Property

    Public ReadOnly Property CurrentWorkflowPropertyFinder As WorkflowPropertyFinder
        Get
            If _currentWorkflowPropertyFinder Is Nothing Then
                _currentWorkflowPropertyFinder = New WorkflowPropertyFinder(DocSuiteContext.Current.Tenants)
            End If
            Return _currentWorkflowPropertyFinder
        End Get
    End Property

    Private ReadOnly Property ToSendProtocolType As SendDocumentUnitType?
        Get
            If Not _toSendProtocolType.HasValue Then
                _toSendProtocolType = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of SendDocumentUnitType?)("ToSendProtocolType", Nothing)
            End If
            Return _toSendProtocolType
        End Get
    End Property

    Private ReadOnly Property ProtocolsKeys As IList(Of Guid)
        Get
            If _protocolsKeys.IsNullOrEmpty() Then
                _protocolsKeys = HttpContext.Current.Request.QueryString.GetValue(Of List(Of Guid))("keys")
            End If
            Return _protocolsKeys
        End Get
    End Property

    Private ReadOnly Property IsPrivacy As Boolean
        Get
            If Not _isPrivacy.HasValue AndAlso Not String.IsNullOrEmpty(CurrentContainerControl.SelectedValue) Then
                Dim container As Container = Facade.ContainerFacade.GetById(CInt(CurrentContainerControl.SelectedValue))
                _isPrivacy = container.PrivacyEnabled
            End If
            Return _isPrivacy.Value
        End Get
    End Property


    Protected ReadOnly Property CurrentWorkflowActivityLogFacade As VecompSoftware.DocSuiteWeb.Facade.WebAPI.Workflows.WorkflowActivityLogFacade
        Get
            If _currentWorkflowActivityLogFacade Is Nothing Then
                _currentWorkflowActivityLogFacade = New VecompSoftware.DocSuiteWeb.Facade.WebAPI.Workflows.WorkflowActivityLogFacade(DocSuiteContext.Current.Tenants)
            End If
            Return _currentWorkflowActivityLogFacade
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not Action.Eq("Recovery") AndAlso Not Action.Eq("Recover") AndAlso Not Action.Eq("Redo") Then
            If Not Facade.ProtocolFacade.CheckInsertPreviousYear() Then
                Throw New DocSuiteException("Impossibile inserire nuovi protocolli.", String.Format("Non è ancora stato eseguito il cambio anno. {0}", ProtocolEnv.DefaultErrorMessage))
            End If
        End If

        If Action.Eq("Insert") Then
            uscMittenti.EnableFlagGroupChild = True
            uscDestinatari.EnableFlagGroupChild = True
            If ProtocolEnv.DematerialisationEnabled Then
                uscUploadDocumenti.CheckDematerialisationCompliance = True
                uscUploadAllegati.CheckDematerialisationCompliance = True
                uscUploadAnnexes.CheckDematerialisationCompliance = False
            End If
        End If

        InitializeAjax()

        InitQueryString()

        InitializeControls()

        AjaxManager.ClientEvents.OnRequestStart = "OnRequestStart"
        AjaxManager.ClientEvents.OnResponseEnd = "OnResponseEnd"

        'SEZIONE RECOVERY
        If Action.Eq("Recovery") Then
            Dim url As String = String.Format("Titolo=Selezione Protocollo da recuperare&NomeCampoID={0}&AddButton={1}&Action=Recovery", txtProtRecovery.ClientID, btnInserimento.ClientID)
            btnSelectProtocol.OnClientClick = "return " & ID & "_OpenWindow('windowSelProtocollo',600,500,'" & url & "');"
            RadWindowManagerProtocollo.Windows(0).OnClientClose = ID & "_CloseFunction"
            Title = "Protocollo - Recupero"
        Else
            RecoveryPanel.Visible = False
        End If

        'Abilitazione autorizzazioni
        If ProtocolEnv.IsAuthorizInsertEnabled Then
            pnlAutorizzazioni.Visible = True
            uscAutorizzazioni.Required = ProtocolEnv.IsAuthorizeInsertRequired
            uscAutorizzazioniCc.Required = False
            uscAutorizzazioni.SearchByUserEnabled = DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled AndAlso Not DocSuiteContext.Current.ProtocolEnv.DistributionHierarchicalEnabled
            If ProtocolEnv.MultiDomainEnabled AndAlso ProtocolEnv.TenantAuthorizationEnabled Then
                uscAutorizzazioni.TenantEnabled = True
            End If
            If ProtocolEnv.IsAuthorizInsertRolesEnabled Then
                AddHandler uscMittenti.ContactRemoved, AddressOf uscContact_RoleUpdate
                AddHandler uscMittenti.ContactAdded, AddressOf uscContact_RoleUpdate
                AddHandler uscDestinatari.ContactRemoved, AddressOf uscContact_RoleUpdate
                AddHandler uscDestinatari.ContactAdded, AddressOf uscContact_RoleUpdate
            End If
        End If

        'Importazione da share di rete
        uscUploadDocumenti.SharedFolder = SharedFolder

        If NeedPackage AndAlso CheckPackageError() Then
            EnableInsertButton(False)
        End If

        uscUploadDocumenti.ButtonPrivacyLevelVisible = DocSuiteContext.Current.PrivacyLevelsEnabled
        uscUploadAllegati.ButtonPrivacyLevelVisible = DocSuiteContext.Current.PrivacyLevelsEnabled
        uscUploadAnnexes.ButtonPrivacyLevelVisible = DocSuiteContext.Current.PrivacyLevelsEnabled
        If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso Not String.IsNullOrEmpty(CurrentContainerControl.SelectedValue) Then
            InitDocumentsPrivacyLevels(False)
        End If

        uscMittenti.ButtonContactSmartVisible = ProtocolEnv.ContactSmartEnabled
        uscDestinatari.ButtonContactSmartVisible = ProtocolEnv.ContactSmartEnabled
        If Not IsPostBack Then
            Initialize()
            InitializeDocumentControls()
        End If
    End Sub

    Private Sub InitializeDocumentControls()
        Dim uploaddocumentsLabels As New List(Of Tuple(Of uscDocumentUpload, Model.Entities.DocumentUnits.ChainType))
        uploaddocumentsLabels.Add(New Tuple(Of uscDocumentUpload, Model.Entities.DocumentUnits.ChainType)(uscUploadDocumenti, Model.Entities.DocumentUnits.ChainType.MainChain))
        uploaddocumentsLabels.Add(New Tuple(Of uscDocumentUpload, Model.Entities.DocumentUnits.ChainType)(uscUploadAllegati, Model.Entities.DocumentUnits.ChainType.AttachmentsChain))
        uploaddocumentsLabels.Add(New Tuple(Of uscDocumentUpload, Model.Entities.DocumentUnits.ChainType)(uscUploadAnnexes, Model.Entities.DocumentUnits.ChainType.AnnexedChain))
        InitializeDocumentLabels(uploaddocumentsLabels)
    End Sub

    Private Sub ProtInserimento_AjaxRequest(ByVal seneder As Object, ByVal e As AjaxRequestEventArgs)
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
            Dim arguments As String() = Split(e.Argument, "|", 2)
            Select Case arguments(0)
                Case "Recovery"
                    txtProtRecovery.Text = arguments(1)

                    Dim args As String() = arguments(1).Split("|"c)
                    If args.Length < 2 Then
                        AjaxAlert("Numero di protocollo non corretto")
                        Return
                    End If

                    Try
                        Dim p As Protocol = Facade.ProtocolFacade.GetById(Short.Parse(args(0)), Integer.Parse(args(1)))
                        If p.IdDocument.HasValue AndAlso p.IdDocument.Value > 0 Then
                            ' TODO: verificare a cosa serve salvare il file in temp
                            uscUploadDocumenti.SendSourceDocument = True
                            uscUploadDocumenti.LoadBiblosDocuments(p.Location.DocumentServer, p.Location.ProtBiblosDSDB, p.IdDocument, False, True)
                        End If
                        BindForm(p)

                    Catch ex As Exception
                        FileLogger.Warn(LoggerName, "Impossibile recuperare il  protocollo selezionato.", ex)
                        AjaxAlert("Impossibile recuperare il  protocollo selezionato")
                        Return
                    End Try

                    btnSelectProtocol.Enabled = False

                Case "UpdatePackage"
                    UpdatePackage()
                Case "UpdatePackageLot"
                    UpdatePackageLot()
                Case "SetPrivacy"
                    If arguments(1) IsNot Nothing Then
                        Dim models As IList(Of ReferenceModel) = JsonConvert.DeserializeObject(Of IList(Of ReferenceModel))(arguments(1))
                        Dim doc As DocumentInfo
                        For Each item As ReferenceModel In models
                            doc = uscUploadDocumenti.DocumentInfos.SingleOrDefault(Function(d) d.Attributes.Any(Function(a) a.Key.Eq(BiblosFacade.DOCUMENT_POSITION_ATTRIBUTE)) AndAlso d.Attributes(BiblosFacade.DOCUMENT_POSITION_ATTRIBUTE).Eq(item.EntityShortId.ToString()))
                            doc.AddAttribute(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE, item.Type)
                        Next
                    End If
            End Select
        End Try
    End Sub

    Protected Sub rblTipoProtocollo_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles rblTipoProtocollo.SelectedIndexChanged
        UpdateTipoProtocollo(False)
        pnlInvoice.Visible = False
        ContainerControlSelectionChanged()
        BindAutorizzazioniCc()
        ResetProtocolKind()
    End Sub

    Private Sub cboIdDocType_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboIdDocType.SelectedIndexChanged
        'dati archiviazione
        If ProtocolEnv.IsPackageEnabled AndAlso IsNumeric(cboIdDocType.SelectedValue) Then
            Dim docTypeList As IList(Of DocumentType) = Facade.TableDocTypeFacade.DocTypeSearch(Integer.Parse(cboIdDocType.SelectedValue), True, ProtocolEnv.IsPackageEnabled, "")
            If Not docTypeList.IsNullOrEmpty() Then
                NeedPackage = docTypeList(0).NeedPackage
                If Not String.IsNullOrEmpty(docTypeList(0).CommonUser) Then
                    ViewState("CommonUser") = docTypeList(0).CommonUser
                Else
                    ViewState("CommonUser") = DocSuiteContext.Current.User.UserName
                End If
            End If
            ViewState("needPackage") = NeedPackage
        End If
        RefreshPackage()

        If Not String.IsNullOrEmpty(DirectCast(sender, DropDownList).SelectedValue) Then
            DocTypeHiddenFields(DirectCast(sender, DropDownList).SelectedValue)
        End If
    End Sub

    Private Sub uscContact_RoleUpdate(ByVal sender As Object, ByVal e As EventArgs)
        If ProtocolEnv.ProtocolTypeSenderAuthorizationInsert.Any(Function(x) x.Equals(GetSelectedProtocolTypeId())) Then
            uscAutorizzazioni.LoadRoleContacts(uscMittenti.GetContacts(False), False)
        End If

        If Not ProtocolEnv.IsDistributionEnabled Then
            uscAutorizzazioni.LoadRoleContacts(uscDestinatari.GetContacts(True), False)
        Else
            uscAutorizzazioni.LoadRoleContacts(uscDestinatari.GetContacts(False), False)
        End If

        BindAutorizzazioniCc()
    End Sub

    Private Sub Container_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        ContainerControlSelectionChanged()
        RefreshPackage()
        BindProtocolKind()
        InitPanelAutorizzazioniCc()
    End Sub

    Private Sub rcbContainer_ItemsRequested(o As Object, e As RadComboBoxItemsRequestedEventArgs) Handles rcbContainer.ItemsRequested
        ' Contenitori legati all'utente
        Dim right As ProtocolContainerRightPositions = CType(If(Action.Eq("interop"), ProtocolContainerRightPositions.PECIn, ProtocolContainerRightPositions.Insert), ProtocolContainerRightPositions)
        Dim availableContainers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Protocol, right, True)

        ' Filtro per testo
        If Not String.IsNullOrEmpty(e.Text) Then
            availableContainers = Facade.ContainerFacade.FilterContainers(availableContainers, e.Text)
        End If

        ' Visualizzo il risultato della ricerca
        CurrentContainerControl.ClearItems()
        For Each container As Container In availableContainers
            CurrentContainerControl.AddItem(container.Name, container.Id.ToString())
        Next

        ' Se è disponibile un singolo contenitore lo seleziono di default.
        If availableContainers.Count = 1 Then
            CurrentContainerControl.SelectedValue = availableContainers(0).Id.ToString()
            ContainerControlSelectionChanged()
            UpdateTipoProtocollo()
        End If
    End Sub

    Private Sub BtnInserimentoClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnInserimento.Click
        If Not Page.IsValid Then
            AjaxAlert("Dati inseriti non validi, controllare i requisiti.")
            Exit Sub
        End If

        SaveProtocol()
    End Sub

    Public Sub SaveProtocol()
        'Memorizzazione Container in modo che al prossimo inserimento sia già selezionato
        PrevSelectedIdContainer = CurrentContainerControl.SelectedValue

        Dim vAccountingSectional As String = String.Empty
        Dim accountingYear As Short?
        Dim accountingNumber As Integer?
        Dim accountingSectionalNumber As Integer?
        Dim fascicleDocumentUnitFacade As WebAPIFacade.Fascicles.FascicleDocumentUnitFacade = New WebAPIFacade.Fascicles.FascicleDocumentUnitFacade(DocSuiteContext.Current.Tenants)
        Dim fascicleReference As Action(Of Guid, Guid) = AddressOf fascicleDocumentUnitFacade.LinkFascicleReference

        If ProtocolEnv.IsInvoiceEnabled Then
            Dim protocolType As ProtocolType = GetSelectedProtocolType()
            Dim container As Container = CurrentContainerControl.SelectedContainer("ProtDB")
            If (Not ValidateInvoiceFromPage(container.Id, protocolType.Id, vAccountingSectional, accountingYear, accountingNumber, accountingSectionalNumber)) Then
                Exit Sub
            End If
        End If

        ' Verifico se sono corretti e completi i dati, controllo anche lo Scatolone
        If Not CheckDataForInsert() OrElse Not CheckScatolone() Then
            Exit Sub
        End If

        ' Istanza di Protocollo su cui lavorare (Protocollo di recupero o ex-novo)
        Dim protocol As Protocol = GetWorkingProtocol()

        ' Recupero i dati dalla pagina
        BindProtocolFromPage(protocol)

        ' Inserimento Mittenti
        For Each contact As ContactDTO In uscMittenti.GetContacts(True)

            If Not contact.Contact.Address Is Nothing AndAlso ProtocolEnv.ValidationCityCodeEnabled Then
                If Not contact.Contact.Address.ValidateCityCode Then
                    AjaxAlert("Attenzione, la provincia dell'indirizzo del contatto {0} non è valida, correggerla per procedere", contact.Contact.Description)
                    Exit Sub
                End If
            End If

            Select Case contact.ContactPart
                Case ContactTypeEnum.AddressBook
                    protocol.AddSender(contact.Contact, contact.IsCopiaConoscenza)
                Case Else
                    protocol.AddSenderManual(contact.Contact, contact.IsCopiaConoscenza)
            End Select
        Next
        ' Inserimento Destinatari
        For Each contact As ContactDTO In uscDestinatari.GetContacts(True)

            If Not contact.Contact.Address Is Nothing AndAlso ProtocolEnv.ValidationCityCodeEnabled Then
                If Not contact.Contact.Address.ValidateCityCode Then
                    AjaxAlert("Attenzione, la provincia dell'indirizzo del contatto {0} non è valida, correggerla per procedere", contact.Contact.Description)
                    Exit Sub
                End If
            End If

            Select Case contact.ContactPart
                Case ContactTypeEnum.AddressBook
                    protocol.AddRecipient(contact.Contact, contact.IsCopiaConoscenza)
                Case Else
                    protocol.AddRecipientManual(contact.Contact, contact.IsCopiaConoscenza)
            End Select
        Next

        ' Recupero dati di Fattura, esegue anche verifiche sui dati di tipo fattura
        If ProtocolEnv.IsInvoiceEnabled Then
            Facade.ProtocolFacade.AddProtocolInvoice(protocol, txtInvoiceNumber.Text, dtpInvoiceDate.SelectedDate, vAccountingSectional, accountingYear, rdpAccountingDate.SelectedDate, accountingNumber, accountingSectionalNumber)
        End If

        If Action.Eq("FromCollaboration") And ProtocolEnv.SecureDocumentEnabled Then
            Facade.CollaborationFacade.FinalizeSecureDocument(CurrentCollaboration)
        End If

        'Salvataggio nuovo protocollo
        Try
            ' Nel caso di recupero da sospensione (Recovery) e di ripristino da Errore (Recover) il protocollo esiste ed è correttamente numerato, è solo da aggiornare.
            Select Case Action
                Case "Recovery", "Recover"
                    If ProtocolEnv.ProtParzialeRecoveryEnabled Then
                        protocol.IdStatus = ProtocolStatusId.Sospeso
                    End If
                    Facade.ProtocolFacade.UpdateOnly(protocol)
                    Facade.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PM, "Modificato protocollo")
                Case Else
                    Facade.ProtocolFacade.Save(protocol)
                    Facade.ProtocolLogFacade.Insert(protocol.Year, protocol.Number, ProtocolLogEvent.PI.ToString(), "Creato protocollo", DocSuiteContext.Current.User.FullUserName, False)
            End Select
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore in fase di salvataggio Protocollo.", ex)
            Facade.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PE, ex.Message)
            AjaxAlert("Errore in fase di salvataggio Protocollo")
            Exit Sub
        End Try

        BindRolesFromPage(protocol)

        Try
            If Not String.IsNullOrEmpty(SessionProtInserimentoLinks) Then
                Dim sLinks As String() = SessionProtInserimentoLinks.Split(","c)
                For Each sLink As String In sLinks
                    If Not String.IsNullOrEmpty(sLink.Trim()) Then
                        Dim sId As String() = sLink.Split("/"c)
                        Dim prot As Protocol = Facade.ProtocolFacade.GetById(Short.Parse(sId(0)), Integer.Parse(sId(1)), False)
                        Facade.ProtocolFacade.AddProtocolLink(protocol, prot, ProtocolFacade.ProtocolLinkType.Normale, fascicleReference)
                    End If
                Next
            End If
        Catch ex As Exception
            AjaxAlert($"Errore in inserimento Protocollo: {ex.Message}")
        Finally
            SessionProtInserimentoLinks = String.Empty
        End Try

        If ProtocolEnv.IsIssueEnabled Then
            For Each contact As ContactDTO In uscFascicle.GetContacts(False)
                Facade.ProtocolFacade.AddProtocolContactIssue(protocol, contact.Contact)
            Next
        End If

        'Aggiunta dati Package nel protocollo
        If ProtocolEnv.IsPackageEnabled AndAlso NeedPackage AndAlso rblTipoProtocollo.SelectedValue = "-1" Then
            Facade.ProtocolFacade.UpdateProtocolPackage(protocol, txtPackage.Text, txtLot.Text, txtIncremental.Text, txtOrigin.Text.ToCharArray().First())
        End If

        'Inserimento documento, allegato e dati fattura in biblos
        Try
            Dim info As New ProtocolSignatureInfo(uscUploadAllegati.DocumentInfosAdded.Count)
            Dim dematerialisationRequestModel As New DocumentManagementRequestModel()

            If Not uscUploadDocumenti.DocumentInfosAdded.IsNullOrEmpty() Then
                Dim attributes As Dictionary(Of String, String) = Facade.ProtocolFacade.GetDocumentAttributes(protocol, pnlInvoice.Visible)
                ' Add Documento principale
                If protocol.IdDocument.GetValueOrDefault(0) = 0 Then
                    Facade.ProtocolFacade.AddDocument(protocol, uscUploadDocumenti.DocumentInfosAdded(0), info, attributes)

                    Dim documentId As Guid
                    Dim chainId As Guid
                    If DocSuiteContext.Current.ProtocolEnv.DematerialisationEnabled AndAlso uscUploadDocumenti.DocumentInfosDematerialisationAdded IsNot Nothing AndAlso uscUploadDocumenti.DocumentInfosDematerialisationAdded.Count > 0 _
                        AndAlso Not uscUploadDocumenti.DocumentInfosDematerialisationAdded(0).Name.IsNullOrEmpty AndAlso uscUploadDocumenti.DocumentInfosAdded(0).Attributes.Keys.Contains("documentId") _
                        AndAlso Guid.TryParse(uscUploadDocumenti.DocumentInfosAdded(0).Attributes("documentId"), documentId) AndAlso Not documentId = Guid.Empty AndAlso uscUploadDocumenti.DocumentInfosAdded(0).Attributes.Keys.Contains("chainId") _
                        AndAlso Guid.TryParse(uscUploadDocumenti.DocumentInfosAdded(0).Attributes("chainId"), chainId) AndAlso Not chainId = Guid.Empty AndAlso uscUploadDocumenti.DocumentInfosAdded(0).Attributes.Keys.Contains("archiveName") AndAlso Not String.IsNullOrEmpty(uscUploadDocumenti.DocumentInfosAdded(0).Attributes("archiveName")) Then

                        Dim workflowReferenceBiblosModel As New WorkflowReferenceBiblosModel()
                        workflowReferenceBiblosModel.DocumentName = uscUploadDocumenti.DocumentInfosDematerialisationAdded(0).Name
                        workflowReferenceBiblosModel.ChainType = Model.Entities.DocumentUnits.ChainType.MainChain
                        workflowReferenceBiblosModel.ArchiveChainId = chainId
                        workflowReferenceBiblosModel.ArchiveDocumentId = documentId
                        workflowReferenceBiblosModel.ArchiveName = uscUploadDocumenti.DocumentInfosAdded(0).Attributes("archiveName")
                        dematerialisationRequestModel.Documents.Add(workflowReferenceBiblosModel)
                    End If
                    If ProtocolEnv.ProtParzialeRecoveryEnabled Then
                        protocol.IdStatus = ProtocolStatusId.Errato 'Messo per far partire il controllo di attivazione. Non deve essere persistito
                    End If
                End If
            Else
                If ProtocolEnv.ProtParzialeEnabled Then
                    protocol.IdStatus = ProtocolStatusId.Incompleto
                Else
                    If ProtocolEnv.ProtParzialeRecoveryEnabled Then
                        protocol.IdStatus = ProtocolStatusId.Sospeso
                    Else

                        ''NON DOVREBBE MAI ARRIVARE QUA PERCHE' C'E' IL VALIDATOR.. MA IN OGNI CASO
                        FileLogger.Warn(LoggerName, "Documento principale non inserito")
                        AjaxAlert("Documento principale non inserito")
                        Return
                    End If
                End If
                'Persisto lo status di protocollazione incompleta o sospesa
                Facade.ProtocolFacade.UpdateOnly(protocol)
            End If

            ' Add Allegati
            If uscUploadAllegati.DocumentsAddedCount > 0 Then
                Facade.ProtocolFacade.AddAttachments(protocol, uscUploadAllegati.DocumentInfosAdded, info)
                If DocSuiteContext.Current.ProtocolEnv.DematerialisationEnabled AndAlso uscUploadAllegati.DocumentInfosDematerialisationAdded IsNot Nothing AndAlso uscUploadAllegati.DocumentInfosDematerialisationAdded.Count > 0 Then
                    For Each attachment As DocumentInfo In uscUploadAllegati.DocumentInfosDematerialisationAdded
                        attachment = uscUploadAllegati.DocumentInfosAdded.Where(Function(x) x.Serialized = attachment.Serialized).SingleOrDefault()

                        Dim attachmentWorkflowReferenceBiblosModel As New WorkflowReferenceBiblosModel()
                        Dim attachmentId As Guid = Nothing
                        Dim attachmentChainId As Guid = Nothing

                        If attachment IsNot Nothing AndAlso Not String.IsNullOrEmpty(attachment.Name) AndAlso attachment.Attributes.Keys.Contains("documentId") AndAlso Guid.TryParse(attachment.Attributes("documentId"), attachmentId) AndAlso Not attachmentId = Guid.Empty _
                             AndAlso attachment.Attributes.Keys.Contains("chainId") AndAlso Guid.TryParse(attachment.Attributes("chainId"), attachmentChainId) AndAlso Not attachmentChainId = Guid.Empty _
                             AndAlso attachment.Attributes.Keys.Contains("archiveName") AndAlso Not String.IsNullOrEmpty(attachment.Attributes("archiveName")) Then
                            attachmentWorkflowReferenceBiblosModel.DocumentName = attachment.Name
                            attachmentWorkflowReferenceBiblosModel.ChainType = Model.Entities.DocumentUnits.ChainType.AttachmentsChain
                            attachmentWorkflowReferenceBiblosModel.ArchiveChainId = attachmentChainId
                            attachmentWorkflowReferenceBiblosModel.ArchiveDocumentId = attachmentId
                            attachmentWorkflowReferenceBiblosModel.ArchiveName = attachment.Attributes("archiveName")
                            dematerialisationRequestModel.Documents.Add(attachmentWorkflowReferenceBiblosModel)
                        End If
                    Next
                End If
            End If

            'Log di Serializzazione comando 'SB' per attestazione conformità
            If DocSuiteContext.Current.ProtocolEnv.DematerialisationEnabled AndAlso dematerialisationRequestModel.Documents IsNot Nothing AndAlso dematerialisationRequestModel.Documents.Count > 0 Then
                Dim documentUnit As New WorkflowReferenceModel()
                documentUnit.ReferenceId = protocol.UniqueId
                documentUnit.ReferenceType = DSWEnvironmentType.Protocol

                Dim refModel As New DocumentUnit()
                refModel.DocumentUnitName = "Protocollo"
                refModel.Subject = protocol.ProtocolObject
                refModel.Title = String.Concat(protocol.Year, "/", protocol.Number.ToString("0000000"))
                documentUnit.ReferenceModel = JsonConvert.SerializeObject(refModel)
                dematerialisationRequestModel.DocumentUnit = documentUnit
                dematerialisationRequestModel.RegistrationUser = DocSuiteContext.Current.User.FullUserName
                Facade.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.SB, JsonConvert.SerializeObject(dematerialisationRequestModel))
            End If

            ' Add Allegati non parte integrante (Annessi)
            If uscUploadAnnexes.DocumentsAddedCount > 0 Then
                Facade.ProtocolFacade.AddAnnexes(protocol, uscUploadAnnexes.DocumentInfosAdded, info)
            End If

        Catch ex As Exception
            Facade.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PE, ex.Message)

            Throw New DocSuiteException("Inserimento di protocollo", "Errore in inserimento del documento.", ex)
        End Try

        ' Prima attivazione del protocollo SSE non è parziale
        If protocol.IdStatus <> ProtocolStatusId.Incompleto AndAlso protocol.IdStatus <> ProtocolStatusId.Sospeso Then
            Facade.ProtocolFacade.Activation(protocol)
            Facade.ProtocolFacade.RaiseAfterInsert(protocol)
        End If

        If HasPECSourceProtocol Then
            FacadeFactory.Instance.ProtocolFacade.AddProtocolLink(protocol, CurrentPECSourceProtocol, ProtocolFacade.ProtocolLinkType.Normale, fascicleReference)
        End If

        If FromUDS Then
            Dim repository As UDSRepository = CurrentUDSRepositoryFacade.GetById(CurrentUDSRepositoryId.Value)
            Dim source As UDSDto = CurrentUDSFacade.GetUDSSource(repository, String.Format(ODATA_EQUAL_UDSID, CurrentUDSId.Value))
            If source IsNot Nothing Then
                Dim udsModel As UDS.UDSModel = source.UDSModel
                udsModel.FillProtocols(New List(Of UDS.ReferenceModel) From {New UDS.ReferenceModel() With {.UniqueId = protocol.UniqueId}})
                CurrentUDSRepositoryFacade.SendCommandUpdateData(CurrentUDSRepositoryId.Value, CurrentUDSId.Value, Guid.Empty, udsModel)
            End If
        End If

        'Workflow
        AddWorkflowProperties(protocol)

        'Collaborazione 
        Select Case True
            Case Action.Eq("Redo")
                Facade.ProtocolFacade.AddProtocolLink(protocol, CurrentProtocol, ProtocolFacade.ProtocolLinkType.Normale, fascicleReference) ' Aggiungo il collegamento fra 
                ' Annullo il protocollo precedente
                CurrentProtocol.IdStatus = ProtocolStatusId.Annullato
                Facade.ProtocolFacade.UpdateOnly(CurrentProtocol)

            Case Action.Eq("RispondiDaPEC")
                ' lo collego al protocollo originale
                Facade.ProtocolFacade.AddProtocolLink(protocol, CurrentProtocol, ProtocolFacade.ProtocolLinkType.RispondiDaPEC, fascicleReference)

            Case Action.Eq("FromCollaboration")
                Try
                    FinalizeCollaboration(protocol, CurrentCollaboration)
                Catch ex As Exception
                    Throw New DocSuiteException("Errore in aggiornamento Collaborazione.", String.Format("Impossibile collegare il protocollo alla Collaborazione numero [{0}].", CurrentIdCollaboration), ex)
                End Try

                If DocSuiteContext.Current.ProtocolEnv.CollaborationSourceProtocolEnabled _
                    AndAlso CurrentCollaboration IsNot Nothing _
                    AndAlso CurrentCollaboration.HasSourceProtocol Then

                    FacadeFactory.Instance.ProtocolFacade.AddProtocolLink(protocol, CurrentCollaboration.SourceProtocol, ProtocolFacade.ProtocolLinkType.Normale, fascicleReference)
                End If

            Case Action.Eq("interop") AndAlso CurrentMail IsNot Nothing
                Try
                    Facade.PECMailFacade.FinalizeToProtocol(CurrentMail.Id, protocol)
                    Facade.PECMailLogFacade.InsertLog(CurrentMail, String.Format("Pec protocollata con protocollo {0}", protocol.Id.ToString()), PECMailLogType.Linked)
                    Facade.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PM, String.Format("Collegata PEC n.{0} del {1:dd/MM/yyyy} con oggetto [{2}]", CurrentMail.Id, CurrentMail.RegistrationDate.ToLocalTime(), CurrentMail.MailSubject))
                    If "1".Equals(hfNeedClone.Value) Then
                        Facade.PECMailFacade.Duplicate(CurrentMail, True)
                    End If
                Catch ex As Exception
                    Throw New DocSuiteException("Errore in aggiornamento PEC.", String.Format("Impossibile collegare il protocolla alla PEC numero [{0}].", CurrentMail.Id), ex)
                End Try
            Case Action.Eq("IC")
                If Not ProtocolEnv.ProtocolTransfertEnabled Then
                    AjaxAlert("ProtocolTransfert non abilitato.")
                    Exit Sub
                End If
                Dim pt As New ProtocolTransfert()
                pt.Year = protocol.Year
                pt.Number = protocol.Number
                pt.Category = protocol.Category
                pt.Type = protocol.Type
                pt.Container = protocol.Container
                pt.ProtocolObject = StringHelper.Clean(protocol.ProtocolObject, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
                pt.Note = protocol.Note
                pt.Request = Request.QueryString.ToString()

                Facade.ProtocolTransfertFacade.Save(pt)
        End Select
        Dim toFascicle As Guid? = Nothing
        If Not String.IsNullOrEmpty(hf_workflowAction_toFascicle.Value) Then
            Dim tmp As Guid
            If Guid.TryParse(hf_workflowAction_toFascicle.Value, tmp) Then
                toFascicle = tmp
            End If
        End If
        'Invio un comando di inserimento protocollo alle web api
        Facade.ProtocolFacade.SendInsertProtocolCommand(protocol, CurrentCollaboration, toFascicle)


        Dim impersonator As Impersonator = CommonAD.ImpersonateSuperUser()
        Dim sharedFile As FileInfo = CommonShared.SelectedSharedFile
        If sharedFile IsNot Nothing AndAlso sharedFile.Exists Then
            sharedFile.Delete()
            CommonShared.SelectedSharedFile = Nothing
        End If
        impersonator.ImpersonationUndo()

        If Action.Eq("InsertMail") Then
            'Riferimento al protocollo all'interno di campo nascosti
            ' TODO: verificare se è veramente necessario mettere la signature in querystring, cercare di spostare e controllare che la generi correttamente
            Dim signature As String = Facade.ProtocolFacade.GenerateSignature(protocol, protocol.RegistrationDate.ToLocalTime().DateTime, New ProtocolSignatureInfo())
            Dim params As String = String.Concat("Year=", protocol.Year, "&Number=", protocol.Number, "&Segnature=", signature, "&ProtocolMail=true")
            Dim spage As String = String.Concat(DocSuiteContext.Current.CurrentTenant.DSWUrl, "/frameset.aspx?", params)
            ClientScript.RegisterClientScriptBlock(Me.GetType(), "redirect", "window.top.location='" & spage & "'", True)
        Else
            Dim vParams As String = String.Concat("Type=Prot&Year=", protocol.Year, "&Number=", protocol.Number)
            If IsWorkflowOperation.Equals(True) AndAlso CurrentIdWorkflowActivity.HasValue Then
                vParams &= String.Format("&IsWorkflowOperation=True&IdWorkflowActivity={0}", CurrentIdWorkflowActivity.Value)
                Dim wfActivity As WorkflowActivity = CurrentWorkflowActivityFacade.GetById(CurrentIdWorkflowActivity.Value)
                Dim modelWorkflowProperty As WorkflowProperty = CurrentWorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(CurrentIdWorkflowActivity.Value, WorkflowPropertyHelper.DSW_PROPERTY_MODEL)
                If modelWorkflowProperty IsNot Nothing AndAlso modelWorkflowProperty.PropertyType = WorkflowPropertyType.Json AndAlso Not String.IsNullOrEmpty(modelWorkflowProperty.ValueString) Then
                    Dim model As ProtocolModel = Nothing
                    Try
                        model = JsonConvert.DeserializeObject(Of ProtocolModel)(modelWorkflowProperty.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
                    Catch
                    End Try

                    If model IsNot Nothing Then
                        model.MainDocument.ContentStream = Nothing
                        For Each item As Model.Entities.Commons.DocumentModel In model.Annexes
                            item.ContentStream = Nothing
                        Next
                        For Each item As Model.Entities.Commons.DocumentModel In model.Attachments
                            item.ContentStream = Nothing
                        Next
                        modelWorkflowProperty.ValueString = JsonConvert.SerializeObject(model, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
                        CurrentWorkflowPropertyFacade.Update(modelWorkflowProperty)
                        modelWorkflowProperty = CurrentWorkflowPropertyFacade.GetWorkflowPropertyByInstanceAndName(wfActivity.WorkflowInstance.Id, WorkflowPropertyHelper.DSW_PROPERTY_MODEL)
                        If modelWorkflowProperty IsNot Nothing Then
                            modelWorkflowProperty.ValueString = JsonConvert.SerializeObject(model, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
                            CurrentWorkflowPropertyFacade.Update(modelWorkflowProperty)
                        End If
                    End If

                End If

            End If
            If FromUDS Then
                vParams &= "&Action=FromUDS"
            End If

            If CurrentIdWorkflowActivity.HasValue Then
                'WorkflowActivityLog protocollazione della attività generica
                Dim workflowActivityLog As New VecompSoftware.DocSuiteWeb.Entity.Workflows.WorkflowActivityLog
                workflowActivityLog.LogDescription = String.Format("Attività generica protocollo Year {0} Number {1}", protocol.Year, protocol.Number)
                workflowActivityLog.LogType = VecompSoftware.DocSuiteWeb.Entity.Workflows.WorkflowStatus.Progress
                workflowActivityLog.Entity = New Entity.Workflows.WorkflowActivity(CurrentIdWorkflowActivity.Value)
                workflowActivityLog.SystemComputer = DocSuiteContext.Current.UserComputer
                CurrentWorkflowActivityLogFacade.Save(workflowActivityLog)
            End If

            If Action.Eq("ToSendProtocol") Then
                For Each uniqueId As Guid In ProtocolsKeys
                    Dim childProtocol As Protocol = Facade.ProtocolFacade.GetByUniqueId(uniqueId)
                    Facade.ProtocolFacade.AddProtocolLink(protocol, childProtocol, ProtocolFacade.ProtocolLinkType.Normale, Nothing)
                Next
                If ToSendProtocolType.Equals(SendDocumentUnitType.ToMail) Then
                    Response.Redirect(String.Format("~/MailSenders/ProtocolMailSender.aspx?RedirectToProtocolSummary=true&recipients=false&Year={0}&Number={1}&Type=Prot", protocol.Year, protocol.Number))
                Else
                    Response.Redirect(String.Format("~/PEC/PECInsert.aspx?RedirectToProtocolSummary=true&Type=Pec&SimpleMode={0}&Year={1}&Number={2}", DocSuiteContext.Current.ProtocolEnv.PECSimpleMode.ToString(), protocol.Year, protocol.Number))
                End If
            Else
                Response.Redirect("ProtVisualizza.aspx?" & CommonShared.AppendSecurityCheck(vParams))
            End If
        End If

    End Sub

    ''' <summary>
    ''' Sub di inserimento del protocollo che chiude e collega tutte le collaborazioni figlie
    ''' </summary>
    ''' <param name="protocol">Oggetto corrente del protocollo</param>
    Private Sub FinalizeCollaboration(ByVal protocol As Protocol, ByVal collaboration As Collaboration)

        FacadeFactory.Instance.CollaborationFacade.FinalizeToProtocol(collaboration, protocol)
        Dim draft As CollaborationXmlData = FacadeFactory.Instance.ProtocolDraftFacade.GetDataFromCollaboration(collaboration)
        If draft IsNot Nothing AndAlso draft.GetType() = GetType(ResolutionXML) Then
            Dim resolutionXml As ResolutionXML = CType(draft, ResolutionXML)
            UpdateResolutions(resolutionXml, protocol)
        End If
        If collaboration IsNot Nothing AndAlso collaboration.CollaborationAggregates IsNot Nothing Then
            For Each aggregate As CollaborationAggregate In collaboration.CollaborationAggregates
                FinalizeCollaboration(protocol, aggregate.CollaborationChild)
            Next
        End If

    End Sub
    Private Sub uscDestinatari_OChartItemContactsAdded(sender As Object, e As OChartItemContactsEventArgs) Handles uscDestinatari.OChartItemContactsAdded
        BindOChartForRecipient(e.ItemFullCode)
    End Sub

    Private Sub uscAutorizzazioniCc_OnRoleUserViewModeChanged(ByVal sender As Object, ByVal e As EventArgs) Handles uscAutorizzazioniCc.OnRoleUserViewModeChanged
        If uscAutorizzazioniCc.CurrentRoleUserViewMode.Value = uscSettori.RoleUserViewMode.RoleUsers Then
            BindAutorizzazioniCc(uscSettori.RoleUserViewMode.Roles)
        Else
            BindAutorizzazioniCc(uscSettori.RoleUserViewMode.RoleUsers)
        End If
    End Sub

    Private Sub uscAutorizzazioniCc_OnRoleUserViewManagersChanged(sender As Object, e As EventArgs) Handles uscAutorizzazioniCc.OnRoleUserViewManagersChanged
        BindAutorizzazioniCc()
    End Sub

    Private Sub uscAutorizzazazioni_RoleEdited(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles uscAutorizzazioni.RolesAdded, uscAutorizzazioni.RoleRemoved
        If rblTipoProtocollo.SelectedValue = 1 Then
            BindAutorizzazioniCc(uscSettori.RoleUserViewMode.Roles)
        Else
            BindAutorizzazioniCc(uscSettori.RoleUserViewMode.RoleUsers)
        End If
    End Sub

    Private Sub ddlTemplateProtocol_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlTemplateProtocol.SelectedIndexChanged
        Try
            If String.IsNullOrEmpty(ddlTemplateProtocol.SelectedValue) Then
                Exit Sub
            End If

            Dim idTemplate As Integer = CType(ddlTemplateProtocol.SelectedValue, Integer)
            Dim template As TemplateProtocol = Facade.TemplateProtocolFacade.GetById(idTemplate)
            BindPageFromTemplate(template)

        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            Throw ex
        End Try
    End Sub

    Private Sub txtDocumentProtocol_TextChanged(sender As Object, e As EventArgs) Handles txtDocumentProtocol.TextChanged, txtDocumentDate.SelectedDateChanged
        Dim selectedProtocolTypeId As Integer? = GetSelectedProtocolTypeId()
        If DocSuiteContext.Current.ProtocolEnv.DetectProtocolInDuplicateMetadatas AndAlso selectedProtocolTypeId.HasValue AndAlso selectedProtocolTypeId.Value = -1 AndAlso
            hf_checkProtocolInMetadatas.Value.Eq("1") AndAlso Not String.IsNullOrEmpty(txtDocumentProtocol.Text) Then
            Dim foundedProtocol As Protocol = Facade.ProtocolFacade.GetLastProtocolInDuplicateMetadatas(txtDocumentProtocol.Text, txtDocumentDate.SelectedDate, String.Empty)
            If foundedProtocol IsNot Nothing Then
                Dim protocolSender As ContactDTO = foundedProtocol.GetSenders().First(Function(f) f.Contact IsNot Nothing)
                AjaxAlertConfirmAndDeny(String.Concat("Il protocollo mittente risulta già protocollato nel seguente protocollo : ", String.Format("{0:0000}/{1:000000}", foundedProtocol.Year, foundedProtocol.Number), " - ", foundedProtocol.ProtocolObject, " del mittente ", protocolSender.Contact.Description, ". Vuoi continuare?").Replace("'", ""), Nothing, "RedirectToPage('../Prot/ProtInserimento.aspx')", Nothing, Nothing)
            End If
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub BindContainerControl()
        Dim right As ProtocolContainerRightPositions = If(Action.Eq("interop"), ProtocolContainerRightPositions.PECIn, ProtocolContainerRightPositions.Insert)
        Dim availableContainers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Protocol, right, True)

        If availableContainers.IsNullOrEmpty() Then
            Throw New DocSuiteException("Inserimento Protocollo", "Utente senza diritti di Inserimento.")
        End If

        CurrentContainerControl.Enabled = True
        CurrentContainerControl.ClearItems()
        If availableContainers.Count > 1 AndAlso Not ProtocolEnv.AuthorizContainer <> 0 Then
            CurrentContainerControl.AddItem(String.Empty, String.Empty)
        End If
        For Each c As Container In availableContainers
            CurrentContainerControl.AddItem(c.Name, c.Id.ToString())
        Next

        If ProtocolEnv.AuthorizContainer <> 0 Then
            CurrentContainerControl.SelectedValue = ProtocolEnv.AuthorizContainer.ToString()
        End If

        If availableContainers.Count.Equals(1) Then
            CurrentContainerControl.SelectedValue = availableContainers.Item(0).Id.ToString()
            CurrentContainerControl.Enabled = False
            ContainerControlSelectionChanged()
            UpdateTipoProtocollo()
        End If
    End Sub

    Private Function GetCurrentCollaboration() As Collaboration
        If CurrentIdCollaboration.HasValue Then
            Return FacadeFactory.Instance.CollaborationFacade.GetById(Me.CurrentIdCollaboration.Value)
        End If
        Return Nothing
    End Function

    Private Function GetCurrentPECMail() As PECMail
        If CurrentPECMailId.HasValue Then
            Return FacadeFactory.Instance.PECMailFacade.GetById(Me.CurrentPECMailId.Value)
        End If
        Return Nothing
    End Function

    Private Sub InitQueryString()
        _currentIdCollaboration = New Lazy(Of Integer?)(Function() Request.QueryString.GetValueOrDefault(Of Integer?)("IdCollaboration", Nothing))
        _currentCollaboration = New Lazy(Of Collaboration)(Function() GetCurrentCollaboration())

        _currentIdPECMail = New Lazy(Of Integer?)(Function() Request.QueryString.GetValueOrDefault(Of Integer?)("IdPECMail", Nothing))
        _currentPECMail = New Lazy(Of PECMail)(Function() GetCurrentPECMail())

        _currentPECSourceProtocolYear = New Lazy(Of Short?)(Function() GetPECSourceProtocolYear())
        _currentPECSourceProtocolNumber = New Lazy(Of Integer?)(Function() GetPECSourceProtocolNumber())
        _currentPECSourceProtocol = New Lazy(Of Protocol)(Function() GetPECSourceProtocol())
        _hasPECSourceProtocol = New Lazy(Of Boolean)(Function() IsPECSourceProtocolAvailable() AndAlso CurrentPECSourceProtocol IsNot Nothing)
    End Sub

    Private Function IsPECSourceProtocolAvailable() As Boolean
        Return DocSuiteContext.Current.ProtocolEnv.PECSourceProtocolEnabled _
            AndAlso CurrentPECMail IsNot Nothing _
            AndAlso New PECMailRightsUtil(Me.CurrentPECMail, DocSuiteContext.Current.User.FullUserName).IsProtocollable _
            AndAlso Regex.IsMatch(Me.CurrentPECMail.MailSubject, DocSuiteContext.Current.ProtocolEnv.PECSourceProtocolPattern)
    End Function

    Private Function GetPECSourceProtocolYear() As Short?
        If IsPECSourceProtocolAvailable() Then
            Return CShort(Regex.Match(Me.CurrentPECMail.MailSubject, DocSuiteContext.Current.ProtocolEnv.PECSourceProtocolPattern).Value.Substring(1, 4))
        End If
        Return Nothing
    End Function

    Private Function GetPECSourceProtocolNumber() As Integer?
        If IsPECSourceProtocolAvailable() Then
            Return CInt(Regex.Match(Me.CurrentPECMail.MailSubject, DocSuiteContext.Current.ProtocolEnv.PECSourceProtocolPattern).Value.Substring(5, 7))
        End If
        Return Nothing
    End Function

    Private Function GetPECSourceProtocol() As Protocol
        If IsPECSourceProtocolAvailable() Then
            Return FacadeFactory.Instance.ProtocolFacade.GetById(Me.CurrentPECSourceProtocolYear.Value, CurrentPECSourceProtocolNumber.Value)
        End If
        Return Nothing
    End Function

    Private Sub InitializeWorkflowWizard()
        Dim insertProtocolStep As RadWizardStep = New RadWizardStep()
        Select Case Action
            Case "FromCollaboration"
                insertProtocolStep.ID = "InsertCollaborationProtocol"
                insertProtocolStep.Title = "Protocolla Collaborazione"
                insertProtocolStep.ToolTip = "Protocolla Collaborazione"
            Case "FromUDS"
                insertProtocolStep.ID = "InsertUDSProtocol"
                insertProtocolStep.Title = "Protocolla Archivio"
                insertProtocolStep.ToolTip = "Protocolla Archivio"
            Case Else
                insertProtocolStep.ID = "InsertProtocol"
                insertProtocolStep.Title = "Inserisci nuovo protocollo"
                insertProtocolStep.ToolTip = "Inserisci nuovo protocollo"
        End Select
        insertProtocolStep.Active = True
        MasterDocSuite.WorkflowWizardControl.WizardSteps.Add(insertProtocolStep)

        Dim sendCompleteStep As RadWizardStep = New RadWizardStep()
        sendCompleteStep.ID = "SendComplete"
        sendCompleteStep.Title = "Concludi attività"
        sendCompleteStep.ToolTip = "Concludi attività"
        sendCompleteStep.Enabled = False
        MasterDocSuite.WorkflowWizardControl.WizardSteps.Add(sendCompleteStep)
    End Sub

    Private Sub Initialize()
        If IsWorkflowOperation Then
            MasterDocSuite.WorkflowWizardRow.Visible = True
            InitializeWorkflowWizard()
        End If

        ' Lunghezza oggetto
        uscOggetto.MaxLength = ProtocolEnv.ObjectMaxLength
        'Nascondo campi nascosti per Gestione Recovery
        WebUtils.ObjAttDisplayNone(txtIdContact)
        WebUtils.ObjAttDisplayNone(txtIdContactNew)
        WebUtils.ObjAttDisplayNone(txtProtDate)
        WebUtils.ObjAttDisplayNone(btnInserimento)

        SessionProtInserimentoLinks = String.Empty

        'Abilitazione Tipologia Documento
        pnlIdDocType.Visible = ProtocolEnv.IsTableDocTypeEnabled

        'Inizializzo le causali di Protocollo
        ddlProtKindList.Items.Add(New ListItem(ProtocolKind.Standard.GetDescription(), CType(ProtocolKind.Standard, Short).ToString()))
        ddlProtKindList.DataBind()
        pnlProtocolKind.Visible = False

        ' PECMail
        If ProtocolEnv.IsPECDestinationEnabled AndAlso CurrentMail IsNot Nothing AndAlso CurrentMail.MailBox.IsDestinationEnabled Then

            pnlDestinationNote.Visible = True
            If CurrentMail.DestinationNote IsNot Nothing Then
                lblDestinationNote.Text = CurrentMail.DestinationNote.Replace(vbCrLf & vbCrLf, vbCrLf).Replace(vbCrLf, " - ")
            End If
        Else
            pnlDestinationNote.Visible = False
        End If

        If ProtocolEnv.IsProtocolRecoverEnabled AndAlso ProtocolEnv.CheckRecoverToProtocol Then

            If DocSuiteContext.Current.ProtocolEnv.IsProtocolRecoverEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.CheckRecoverToProtocol Then
                Dim protocolliErrati As Integer = Facade.ProtocolFacade.GetRecoveringProtocolsFinder.Count()
                If protocolliErrati > 0 Then
                    pnlProtocolAlertNote.Visible = True
                    Dim howManyProtocols As String = If(protocolliErrati = 1, "È presente 1 protocollo ", "Sono presenti {0} protocolli ")
                    ProtocolAlertNoteTitle.Text = "Attenzione!"
                    ProtocolAlertNoteMessage.Text = String.Format(String.Format("{0} in stato di errore ancora da gestire.<br/>Utilizzare la gestione ""Recupero Errori"".", howManyProtocols), protocolliErrati)
                End If
            End If
        End If

        'fattura
        pnlInvoice.Visible = False

        'imposta la pagina in base alla tipologia selezionata
        rblTipoProtocollo.DataSource = Facade.ProtocolTypeFacade().GetTypes()
        rblTipoProtocollo.DataBind()

        ContainerControlSelectionChanged()

        rblTipoProtocollo.SelectedValue = "-1"
        If DocSuiteContext.Current.ProtocolEnv.PECToProtocolFixedDirection AndAlso CurrentMail IsNot Nothing Then
            Select Case CurrentMail.Direction
                Case PECMailDirection.Ingoing
                    rblTipoProtocollo.SelectedValue = "-1" ' Ingresso
                    rblTipoProtocollo.Enabled = False
                Case PECMailDirection.Outgoing
                    rblTipoProtocollo.SelectedValue = "1" ' Uscita
                    rblTipoProtocollo.Enabled = False
            End Select
        End If

        UpdateTipoProtocollo()

        cbClaim.Visible = ProtocolEnv.IsClaimEnabled
        hfNeedClone.Value = Request.QueryString("needClone")
        txtProtDate.Text = String.Format("{0:dd/MM/yyyy}", DateTime.Now)

        Dim bIPA As Boolean = Not String.IsNullOrEmpty(ProtocolEnv.LdapIndicePa)
        uscMittenti.ButtonIPAVisible = bIPA
        uscDestinatari.ButtonIPAVisible = bIPA
        If DocSuiteContext.Current.IsResolutionEnabled AndAlso DocSuiteContext.Current.ResolutionEnv.ProposerContact.HasValue Then
            uscMittenti.AddExcludeContact(DocSuiteContext.Current.ResolutionEnv.ProposerContact.Value)
            uscDestinatari.AddExcludeContact(DocSuiteContext.Current.ResolutionEnv.ProposerContact.Value)
        End If
        'Sezione Fascicoli: TODO user control
        panelFascicolo.Visible = ProtocolEnv.IsIssueEnabled

        'Sezione Classificatore
        uscClassificatori.Multiple = False

        'Sezione Documenti: Abilitazione Firma
        uscUploadDocumenti.SignButtonEnabled = ProtocolEnv.IsFDQEnabled

        uscUploadAllegati.SignButtonEnabled = ProtocolEnv.IsFDQEnabled

        'Copia da atto
        If (DocSuiteContext.Current.IsResolutionEnabled) Then
            uscUploadAllegati.ButtonCopyResl.Visible = ResolutionEnv.CopyReslDocumentsEnabled
        End If

        'Copia da protocollo
        uscUploadAllegati.ButtonCopyProtocol.Visible = ProtocolEnv.CopyProtocolDocumentsEnabled

        uscUploadAllegati.ButtonCopySeries.Visible = ProtocolEnv.CopyFromSeries

        uscUploadAnnexes.SignButtonEnabled = ProtocolEnv.IsFDQEnabled

        'Copia da atto
        If (DocSuiteContext.Current.IsResolutionEnabled) Then
            uscUploadAnnexes.ButtonCopyResl.Visible = ResolutionEnv.CopyReslDocumentsEnabled
        End If

        'Copia da protocollo
        uscUploadAnnexes.ButtonCopyProtocol.Visible = ProtocolEnv.CopyProtocolDocumentsEnabled

        'Copia da protocollo
        uscUploadAnnexes.ButtonCopySeries.Visible = ProtocolEnv.CopyFromSeries

        uscDestinatari.ButtonImportVisible = ProtocolEnv.IsImportContactEnabled
        uscMittenti.ButtonImportVisible = ProtocolEnv.IsImportContactEnabled
        uscMittenti.ButtonManualMultiVisible = ProtocolEnv.EnableContactAndDistributionGroup
        uscDestinatari.ButtonManualMultiVisible = ProtocolEnv.EnableContactAndDistributionGroup
        'TODO: sostiutire con la nuova BL

        'Impostazione Titolo Pagina
        Select Case Action
            Case "Insert", "Duplicate", "RispondiDaPEC"
                Title = "Protocollo - Inserimento"
            Case "InsertMail"
                Title = "Protocollo - Inserimento da Mail"
            Case "IC"
                Title = "Protocollo - Inserimento con Parametri"
            Case "Interop"
                Title = "Protocollo -  Interoperabilità"
            Case "FromCollaboration"
                Title = "Protocollo - Inserimento da Collaborazione"
            Case "Recovery"
                Title = "Protocollo - Recupero"
                If Not Facade.ProtocolFacade.HasProtSuspended Then
                    Throw New DocSuiteException("Protocollo Recupero", "Nessun Protocollo da recuperare.")
                End If
            Case "Recover"

                If CurrentProtocol Is Nothing Then
                    AjaxAlert("Errore in Lettura Protocollo per il Recupero. Ripetere l\'operazione di Inserimento")
                    Exit Sub
                Else
                    tblProtocollo.Visible = True
                End If
            Case "PECError"
                Title = "Protocollo - PEC in errore"

        End Select

        rblTipoProtocollo.DataSource = Facade.ProtocolTypeFacade().GetTypes()
        rblTipoProtocollo.DataBind()
        BindContainerControl()

        'Inizializzo pannelli di autorizzazione per copia conoscenza
        If rblTipoProtocollo.SelectedValue = 1 Then
            BindAutorizzazioniCc(uscSettori.RoleUserViewMode.Roles)
        Else
            BindAutorizzazioniCc(uscSettori.RoleUserViewMode.RoleUsers)
        End If

        InitPanelAutorizzazioniCc()

        'Pulizia directory temporanea
        CommonInstance.UserDeleteTemp(TempType.I)

        pnlProtocolStatus.Visible = False
        If ProtocolEnv.IsStatusEnabled Then
            pnlProtocolStatus.Visible = True
            cboProtocolStatus.DataSource = Facade.ProtocolStatusFacade().GetByProtocolStatusExclusion("P")
            cboProtocolStatus.DataBind()
        End If

        pnlTemplateProtocol.Visible = False
        If ProtocolEnv.TemplateProtocolEnable Then
            Dim templates As IList(Of TemplateProtocol) = Facade.TemplateProtocolFacade.GetTemplates()
            If templates.Any() Then
                pnlTemplateProtocol.Visible = True
                ddlTemplateProtocol.Items.Add(New ListItem(String.Empty, String.Empty))
                For Each template As TemplateProtocol In templates.OrderBy(Function(o) o.TemplateName)
                    Dim item As ListItem = New ListItem(template.TemplateName, template.Id.ToString())
                    ddlTemplateProtocol.Items.Add(item)
                Next
                If CurrentPecMailTemplate IsNot Nothing Then
                    ddlTemplateProtocol.SelectedValue = CurrentPecMailTemplate.Id.ToString()
                    BindPageFromTemplate(CurrentPecMailTemplate)
                Else
                    Dim defaultTemplate As TemplateProtocol = Facade.TemplateProtocolFacade.GetDefaultTemplate()
                    If defaultTemplate IsNot Nothing Then
                        ddlTemplateProtocol.SelectedValue = defaultTemplate.Id.ToString()
                        BindPageFromTemplate(defaultTemplate)
                    End If
                End If
            End If
        End If

        If PreviousPage IsNot Nothing AndAlso TypeOf PreviousPage Is IProtocolInitializer Then
            ' Inizializzazioni da IProtocolInitializer
            Dim initializer As ProtocolInitializer = CType(PreviousPage, IProtocolInitializer).GetProtocolInitializer()
            UseProtocolInitializer(initializer)
            If Not Action.Eq("FromUDS") Then
                InitFromPECSourceProtocol()
            End If
        Else
            ' Inizializzazioni da QueryString
            Select Case Action
                Case "InsertMail"
                    InitializeInsertMail()
                Case "FromCollaboration"
                    InitializeFromCollaboration()
                Case "IC"
                    InitializeInserimentoCompleto()
                Case "FromUDS"
                    InitializeFromUDS()
                Case "FromWorkflow"
                    InitializeFromWorkflow()
                Case "FromWorkflowUI"
                    RadScriptManager.RegisterStartupScript(Page, Page.GetType(), "InitializePageFromWorkflowUI", "$(document).ready(function() {BindingFromWorkflowUI()});", True)

            End Select

            Dim documentDate As Date = Request.QueryString.GetValueOrDefault("DocumentDate", Date.MinValue)
            If documentDate > Date.MinValue Then
                txtDocumentDate.SelectedDate = documentDate
            End If

            ' Duplicazione Protocollo
            Select Case Action
                Case "Duplicate"
                    Dim hasDisabledElements As Boolean = DuplicateProtocol()
                    If hasDisabledElements Then
                        AjaxAlert("Sono stati rimossi in automatico elementi non più attivi.")
                    End If
                Case "Redo"
                    Try
                        Dim vProtocol As Protocol = Facade.ProtocolFacade.GetById(CurrentProtocolYear, CurrentProtocolNumber)
                        BindForm(vProtocol)
                    Catch ex As Exception
                        FileLogger.Warn(LoggerName, "Errore in Lettura Protocollo per il Reinserimento. Ripetere l'operazione di Inserimento.", ex)
                        AjaxAlert("Errore in Lettura Protocollo per il Reinserimento. Ripetere l\'operazione di Inserimento")
                        Exit Sub
                    End Try
                Case "RispondiDaPEC"
                    RispondiDaPEC()
                Case Else
                    If (Not String.IsNullOrEmpty(PrevSelectedIdContainer) AndAlso CurrentContainerControl.HasItemWithValue(PrevSelectedIdContainer)) AndAlso Not Action.Eq("FromUDS") Then
                        CurrentContainerControl.SelectedValue = PrevSelectedIdContainer
                        ContainerControlSelectionChanged()
                        UpdateTipoProtocollo()
                    End If
            End Select

        End If
    End Sub

    Private Sub UseProtocolInitializer(item As ProtocolInitializer)

        If Not String.IsNullOrEmpty(item.Subject) Then
            uscOggetto.Text = item.Subject
        End If

        If Not String.IsNullOrEmpty(item.Notes) Then
            txtNote.Text = item.Notes
        End If

        If item.ProtocolType.HasValue Then
            rblTipoProtocollo.SelectedValue = item.ProtocolType.ToString()
            UpdateTipoProtocollo(needSwap:=False)
        End If

        If Not item.Senders.IsNullOrEmpty() Then
            uscMittenti.DataSource = item.Senders
            uscMittenti.DataBind()
        End If

        If Not item.Recipients.IsNullOrEmpty() Then
            uscDestinatari.DataSource = item.Recipients
            uscDestinatari.DataBind()
        End If

        If item.MainDocument IsNot Nothing Then
            uscUploadDocumenti.LoadDocumentInfo(item.MainDocument, False, True, False, True)
            uscUploadDocumenti.InitializeNodesAsAdded(False)
        End If

        If item.Attachments IsNot Nothing Then
            uscUploadAllegati.LoadDocumentInfo(item.Attachments, False, True, False, True)
            uscUploadAllegati.InitializeNodesAsAdded(True)
        End If

        If item.Annexed IsNot Nothing Then
            uscUploadAnnexes.LoadDocumentInfo(item.Annexed, False, True, False, True)
            uscUploadAnnexes.InitializeNodesAsAdded(True)
        End If

        txtDocumentProtocol.Text = item.SenderProtocolNumber

        If item.SenderProtocolDate.HasValue Then
            txtDocumentDate.SelectedDate = item.SenderProtocolDate.Value
        End If

        If item.Containers IsNot Nothing AndAlso item.Containers.Count > 0 Then
            ' Contenitori legati all'utente
            Dim availableContainers As List(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Protocol, ProtocolContainerRightPositions.Insert, True).ToList()
            ' Restringo l'elenco ai soli contenitori definiti dall'organigramma
            Dim effectiveContainers As IList(Of Container) = availableContainers.SelectMany(Function(c) item.Containers.Where(Function(o) o.Id = c.Id)).ToList()

            If effectiveContainers IsNot Nothing AndAlso effectiveContainers.Count > 0 Then
                ' Visualizzo il risultato della ricerca
                CurrentContainerControl.ClearItems()
                For Each container As Container In effectiveContainers
                    CurrentContainerControl.AddItem(container.Name, container.Id.ToString())
                Next

                CurrentContainerControl.SelectedValue = effectiveContainers.FirstOrDefault().Id.ToString()
                ContainerControlSelectionChanged()
                UpdateTipoProtocollo(needSwap:=Not FromUDS)

                ''Una volta impostato il contenitore vengono caricati i tipi di documento
                If Not String.IsNullOrEmpty(item.DocumentTypeLabel) AndAlso ProtocolEnv.IsTableDocTypeEnabled Then
                    Dim documentTypeItem As ListItem = cboIdDocType.Items.FindByText(item.DocumentTypeLabel)
                    If documentTypeItem IsNot Nothing Then
                        cboIdDocType.ClearSelection()
                        documentTypeItem.Selected = True
                    End If
                End If
            End If
        End If

        If item.Category IsNot Nothing AndAlso Facade.CategoryFacade.IsCategoryActive(item.Category) Then
            uscClassificatori.DataSource.Add(item.Category)
            uscClassificatori.DataBind()
        End If

        If item.Roles IsNot Nothing Then
            For Each role As Role In item.Roles
                uscAutorizzazioni.SourceRoles.Add(role)
            Next
            uscAutorizzazioni.DataBind()
        End If
    End Sub

    Private Overloads Sub BindDocType(idContainer As Integer?)
        Dim availableDocTypes As IList(Of DocumentType)
        If idContainer.HasValue Then
            availableDocTypes = Facade.ContainerDocTypeFacade.ContainerDocTypeSearch(idContainer.Value, True)
        Else
            availableDocTypes = Facade.DocumentTypeFacade.DocTypeSearch(0, True, ProtocolEnv.IsPackageEnabled, "")
        End If
        cboIdDocType.Items.Clear()
        For Each dt As DocumentType In availableDocTypes
            Dim currentItem As New ListItem(dt.Description, dt.Id)
            If ProtocolEnv.IsPackageEnabled AndAlso dt.NeedPackage Then
                If String.IsNullOrEmpty(dt.CommonUser) Then
                    currentItem.Text &= " (*)"
                Else
                    currentItem.Text &= " (#)"
                End If
            End If
            cboIdDocType.Items.Add(currentItem)
        Next
        If cboIdDocType.Items.Count > 1 Then
            cboIdDocType.Items.Insert(0, New ListItem(String.Empty, String.Empty))
            cboIdDocType.SelectedIndex = 0
        End If
    End Sub

    Private Function RecursiveFindControl(ByVal p_parent As Control, ByVal p_idToFind As String) As Control
        Dim retval As Control = Nothing
        If p_parent IsNot Nothing Then
            If Not String.IsNullOrEmpty(p_parent.ID) AndAlso p_parent.ID.Equals(p_idToFind) Then
                retval = p_parent
            Else
                For Each c As Control In p_parent.Controls
                    If Not String.IsNullOrEmpty(c.ID) AndAlso c.ID.Equals(p_idToFind) Then
                        retval = c
                    Else
                        retval = RecursiveFindControl(c, p_idToFind)
                    End If
                    If retval IsNot Nothing Then Exit For
                Next
            End If
        End If
        Return retval
    End Function

    Private Sub ResetHiddenFields()
        divLblNote.Style.Remove("display")
        divTxtNote.Style.Remove("display")
        divLblServiceCategory.Style.Remove("display")
        divSelServiceCategory.Style.Remove("display")
    End Sub

    Private Sub DocTypeHiddenFields(ByVal p_docTypeId As Integer)
        ResetHiddenFields()
        Dim currentDocType As DocumentType = Facade.DocumentTypeFacade.GetById(p_docTypeId, False, "ProtDB")
        Dim fieldsToHide(-1) As String
        If Not String.IsNullOrEmpty(currentDocType.HiddenFields) Then
            fieldsToHide = currentDocType.HiddenFields.Split("|"c)
        End If
        For Each f As String In fieldsToHide
            Try
                DirectCast(RecursiveFindControl(Page, f), HtmlControl).Style.Add("display", "none")
            Catch ex As Exception
                FileLogger.Error(LoggerName, "ProtInserimento.aspx.vb, DocTypeHiddenFields: errore stile.", ex)
            End Try
        Next
    End Sub

    Private Sub InitializeAjax()
        AddHandler MasterDocSuite.AjaxManager.AjaxRequest, AddressOf ProtInserimento_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, lblAssegnatario)
        AjaxManager.AjaxSettings.AddAjaxSetting(cboIdContainer, uscUploadDocumenti)
        AjaxManager.AjaxSettings.AddAjaxSetting(cboIdContainer, uscUploadAllegati)
        AjaxManager.AjaxSettings.AddAjaxSetting(cboIdContainer, uscUploadAnnexes)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbContainer, uscUploadDocumenti)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbContainer, uscUploadAllegati)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbContainer, uscUploadAnnexes)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, uscMittenti, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, uscDestinatari, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, UpdatePanelProtocollo, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, pnlContenitore)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, pnlIdDocType)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, pnlInvoice)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInserimento, uscOggetto)

        If ProtocolEnv.IsAuthorizInsertEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, uscAutorizzazioni, MasterDocSuite.AjaxDefaultLoadingPanel)
        End If
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInserimento, uscMittenti, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInserimento, uscDestinatari, MasterDocSuite.AjaxDefaultLoadingPanel)

        'Template di Protocollo
        If ProtocolEnv.TemplateProtocolEnable Then
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, CurrentContainerControl.ActiveControl)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, uscOggetto)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, divLblNote)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, divTxtNote)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, divLblServiceCategory)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, divSelServiceCategory)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, uscMittenti)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, uscDestinatari)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, pnlInvoice)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, pnlAccounting)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, pnlSectionalType)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, rblTipoProtocollo)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, lblProtocolloMittente)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, PanelProtocollo)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, uscUploadDocumenti)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, uscUploadAllegati)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, uscUploadAnnexes)
            If ProtocolEnv.IsStatusEnabled Then
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, cboProtocolStatus)
            End If
            If ProtocolEnv.IsTableDocTypeEnabled Then
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, pnlIdDocType)
            End If
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, uscClassificatori)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, lblAssegnatario)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, uscContactAssegnatario)
            If ProtocolEnv.IsAuthorizInsertEnabled Then
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, uscAutorizzazioni)
            End If
            If ProtocolEnv.ProtocolKindEnabled Then
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, pnlProtocolKind)
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, ddlProtKindList)
            End If

            If ProtocolEnv.IsPackageEnabled Then
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, pnlPackage)
            End If
        End If

        ' inizializzo il container control
        CurrentContainerControl.AutoPostBack = False
        Dim ajaxified As New List(Of Control)
        ajaxified.AddRange({divLblNote, divTxtNote, divLblServiceCategory, divSelServiceCategory})

        If ProtocolEnv.ContainerBehaviourEnabled Then
            CurrentContainerControl.AutoPostBack = True
        End If

        If ProtocolEnv.IsInvoiceEnabled Then
            CurrentContainerControl.AutoPostBack = True
            ajaxified.AddRange({pnlInvoice, pnlAccounting, pnlSectionalType, lblAssegnatario, uscMittenti.TreeViewControl, uscDestinatari.TreeViewControl, uscMittenti.Header, uscDestinatari.Header, rblTipoProtocollo, lblProtocolloMittente, PanelProtocollo})
        End If

        If ProtocolEnv.IsTableDocTypeEnabled Then
            CurrentContainerControl.AutoPostBack = True
            ajaxified.Add(pnlIdDocType)
        End If

        If ProtocolEnv.OChartEnabled Then
            CurrentContainerControl.AutoPostBack = True
            ajaxified.Add(uscMittenti)
            ajaxified.Add(uscDestinatari)
            If ProtocolEnv.IsAuthorizInsertEnabled Then
                ajaxified.Add(uscAutorizzazioni)
            End If
        End If

        If RecoveryPanel.Visible Then
            ajaxified.Add(CurrentContainerControl.ActiveControl)
        End If

        'Inizializzo il modello di Protocollo
        If ProtocolEnv.ProtocolKindEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, pnlProtocolKind)
            AjaxManager.AjaxSettings.AddAjaxSetting(CurrentContainerControl.ActiveControl, pnlProtocolKind)
        End If

        'Behaviour da scelta contenitore --> nel caso di implementazione più generica espandere questa modalità
        ajaxified.Add(uscClassificatori)

        CurrentContainerControl.AddAjaxSettings(AjaxManager, ajaxified)
        If CurrentContainerControl.AutoPostBack Then
            AddHandler cboIdContainer.SelectedIndexChanged, AddressOf Container_SelectedIndexChanged
            AddHandler rcbContainer.SelectedIndexChanged, AddressOf Container_SelectedIndexChanged
        End If

        ' inizializzo altro
        AjaxManager.AjaxSettings.AddAjaxSetting(cboIdDocType, divLblNote)
        AjaxManager.AjaxSettings.AddAjaxSetting(cboIdDocType, divTxtNote)
        AjaxManager.AjaxSettings.AddAjaxSetting(cboIdDocType, divLblServiceCategory)
        AjaxManager.AjaxSettings.AddAjaxSetting(cboIdDocType, divSelServiceCategory)

        'Chiamate Ajax pannelli package
        If ProtocolEnv.IsPackageEnabled Then
            cboIdDocType.AutoPostBack = True
            AjaxManager.AjaxSettings.AddAjaxSetting(cboIdDocType, pnlPackage)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlPackage)
            AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, pnlPackage)
        End If
        'Chiamate Ajax pannelli autorizzazioni
        If ProtocolEnv.IsAuthorizInsertEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(uscMittenti, uscAutorizzazioni)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscDestinatari, uscAutorizzazioni)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscAutorizzazioni, uscAutorizzazioni)
        End If

        If ProtocolEnv.IsDistributionEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, uscAutorizzazioniCc)
            AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, uscAutorizzazioniCc, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cboIdContainer, uscAutorizzazioniCc, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(rcbContainer, uscAutorizzazioniCc, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscAutorizzazioni, uscAutorizzazioniCc)
        End If

        If DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso ProtocolEnv.IsAuthorizInsertEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, uscAutorizzazioni)
            AjaxManager.AjaxSettings.AddAjaxSetting(cboIdContainer, uscAutorizzazioni)
            AjaxManager.AjaxSettings.AddAjaxSetting(rcbContainer, uscAutorizzazioni)
        End If

        If RecoveryPanel.Visible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RecoveryPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, PanelProtocollo)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscOggetto.TextBoxControl)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscClassificatori)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rblTipoProtocollo)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlIdDocType)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlProtocolKind)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlInvoice)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlContenitore)
        End If
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, panelHiddenFileds)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, divTxtNote)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadDocumenti, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadAllegati, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadAnnexes, MasterDocSuite.AjaxDefaultLoadingPanel)

        If PanelProtocollo.Visible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(txtDocumentProtocol, txtDocumentProtocol)
        End If
    End Sub

    Private Sub InitializeControls()
        Select Case Action
            Case "Recovery"
                uscUploadDocumenti.IsDocumentRequired = Not ProtocolEnv.ProtParzialeRecoveryEnabled
            Case "Insert"
                uscUploadDocumenti.IsDocumentRequired = Not ProtocolEnv.ProtParzialeEnabled
            Case Else
                uscUploadDocumenti.IsDocumentRequired = True
        End Select

        rfvContainer.ControlToValidate = CurrentContainerControl.ActiveControl.ID.ToString()

    End Sub

    Private Sub RispondiDaPEC()
        Dim hasDisabledElements As Boolean = False
        Dim replyProtocol As Protocol = Facade.ProtocolFacade.Duplicate(CurrentProtocolYear, CurrentProtocolNumber, hasDisabledElements, True, True, True, True, True, False, False, True)

        uscOggetto.Text = StringHelper.Clean(replyProtocol.ProtocolObject, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
        If replyProtocol.Container IsNot Nothing AndAlso CurrentContainerControl.HasItemWithValue(replyProtocol.Container.Id.ToString()) Then
            CurrentContainerControl.SelectedValue = replyProtocol.Container.Id.ToString()
            ContainerControlSelectionChanged()
        End If

        'Tipologia protocollo
        rblTipoProtocollo.SelectedValue = "1" ' in uscita
        UpdateTipoProtocollo()

        For Each vProtRole As ProtocolRole In replyProtocol.Roles
            uscAutorizzazioni.SourceRoles.Add(vProtRole.Role)
        Next
        uscAutorizzazioni.DataBind()

        Dim categoryToDuplicate As Category = replyProtocol.Category
        If Facade.CategoryFacade.IsCategoryActive(categoryToDuplicate) Then
            uscClassificatori.DataSource.Add(categoryToDuplicate)
            uscClassificatori.DataBind()
        End If

        For Each vContact As ProtocolContact In replyProtocol.Contacts
            Dim vContactDTO As New ContactDTO
            vContactDTO.Contact = vContact.Contact
            vContactDTO.IsCopiaConoscenza = vContact.Type = "CC"
            vContactDTO.Type = ContactDTO.ContactType.Address
            If vContact.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                uscDestinatari.DataSource.Add(vContactDTO)
            End If
        Next
        For Each vContact As ProtocolContactManual In replyProtocol.ManualContacts
            Dim vContactDTO As New ContactDTO
            vContactDTO.Contact = vContact.Contact
            vContactDTO.IsCopiaConoscenza = vContact.Type = "CC"
            vContactDTO.Type = ContactDTO.ContactType.Manual
            If vContact.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                uscDestinatari.DataSource.Add(vContactDTO)
            End If
        Next

        uscMittenti.DataBind()
        uscDestinatari.DataBind()
    End Sub

    ''' <summary>
    ''' Metodo per la lettura della querystring o della sessione del parametro
    ''' </summary>
    ''' <param name="parameterName">Nome del parametro in querystring o in sessione</param>
    ''' <returns>Valore decodificato in formato stringa</returns>
    ''' <remarks>Creata per l'inserimento interop, può essere usata per qualunque form se in avvio viene impostata la sessione</remarks>

    Private Function GetFromSource(ByVal parameterName As String) As String
        Dim parameterValue As String
        If SessionQuerystring IsNot Nothing Then
            parameterValue = SessionQuerystring(parameterName)
        Else
            parameterValue = Request.QueryString(parameterName)
        End If
        Return Server.UrlDecode(parameterValue)
    End Function

    Private Sub InitializeInserimentoCompleto()
        Dim container As String = Server.UrlDecode(Request.QueryString("Ct"))
        Dim direction As String = Server.UrlDecode(Request.QueryString("D"))
        Dim [object] As String = Server.UrlDecode(Request.QueryString("Obj"))
        Dim sender As String = Server.UrlDecode(Request.QueryString("Mt"))
        Dim senderCode As String = Server.UrlDecode(Request.QueryString("CdMt"))
        Dim recipient As String = Server.UrlDecode(Request.QueryString("Dt"))
        Dim recipientCode As String = Server.UrlDecode(Request.QueryString("CdDt"))
        Dim category As String = Server.UrlDecode(Request.QueryString("Clt"))
        Dim note As String = Server.UrlDecode(Request.QueryString("Nt"))
        Dim sectionalNumber As String = Server.UrlDecode(Request.QueryString("NbSz"))
        Dim vatRegistrationNumber As String = Server.UrlDecode(Request.QueryString("INbRg"))
        Dim vatRegistrationDate As String = Server.UrlDecode(Request.QueryString("IDtRg"))
        Dim invoiceNumber As String = Server.UrlDecode(Request.QueryString("NbFt"))
        Dim invoiceDate As String = Server.UrlDecode(Request.QueryString("DFt"))
        Dim piva As String = Server.UrlDecode(Request.QueryString("PIF"))


        Dim documuentName As String = Server.UrlDecode(Request.QueryString("DcN"))
        Dim documentPath As String = Server.UrlDecode(Request.QueryString("DcP"))
        Dim attachName As String = Server.UrlDecode(Request.QueryString("AtcN"))
        Dim attachPath As String = Server.UrlDecode(Request.QueryString("AtcP"))


        Dim alertMessage As New StringBuilder
        ' Avverto che questo parametro non è più usato
        If Not String.IsNullOrEmpty(attachPath) Then
            alertMessage.AppendFormat("Attach Path non supportato: {0} {1}", attachPath, Environment.NewLine)
        End If

        If Not String.IsNullOrEmpty(container) Then
            Dim availableContainers As IList(Of Container) = Facade.ContainerFacade.GetContainerByName(container, True)
            If availableContainers.Count > 0 Then
                If CurrentContainerControl.HasItemWithValue(availableContainers.Item(0).Id.ToString()) Then
                    CurrentContainerControl.SelectedValue = availableContainers.Item(0).Id.ToString()
                    ContainerControlSelectionChanged()
                Else
                    alertMessage.AppendFormat("Contenitore errato o diritti insufficienti per l'utente: {0} {1}", container, Environment.NewLine)
                End If
            Else
                alertMessage.AppendFormat("Nessun Contenitore trovato con nome: {0} {1}", container, Environment.NewLine)
            End If
        End If

        If Not String.IsNullOrEmpty(direction) Then
            Select Case direction
                Case "I"
                    rblTipoProtocollo.SelectedValue = "-1"
                    UpdateTipoProtocollo()
                Case "U"
                    rblTipoProtocollo.SelectedValue = "1"
                    UpdateTipoProtocollo()
                Case Else
                    alertMessage.AppendFormat("Campo Direzione non valido. I valori accettati sono I o U. Parametro attuale: {0} {1}", direction, Environment.NewLine)
            End Select
        End If
        uscOggetto.Text = [object]
        txtNote.Text = note
        txtInvoiceNumber.Text = invoiceNumber
        txtAccountingNumber.Text = vatRegistrationNumber

        If Not String.IsNullOrEmpty(sectionalNumber) Then
            Dim founded As ListItem = ddlAccountingSectional.Items.FindByText(sectionalNumber)
            If founded IsNot Nothing Then
                ddlAccountingSectional.SelectedValue = sectionalNumber
            End If
        End If

        If Not String.IsNullOrEmpty(invoiceDate) Then
            dtpInvoiceDate.SelectedDate = Convert.ToDateTime(invoiceDate)
        End If

        If Not String.IsNullOrEmpty(vatRegistrationDate) Then
            rdpAccountingDate.SelectedDate = Convert.ToDateTime(vatRegistrationDate)
        End If

        If Not String.IsNullOrEmpty(documuentName) Then
            If Not String.IsNullOrEmpty(documuentName) Then
                Dim doc As New TempFileDocumentInfo(New FileInfo(Path.Combine(CommonInstance.AppTempPath, documentPath))) With {.Name = documuentName}
                uscUploadDocumenti.LoadDocumentInfo(doc)
            End If
        End If

        If Not String.IsNullOrEmpty(attachName) Then
            Dim aAttachsPath() As String = attachName.Split(","c)
            Dim aAttachsName() As String = attachName.Split(","c)
            For index As Integer = 0 To aAttachsName.Length - 1
                Dim doc As New TempFileDocumentInfo(New FileInfo(Path.Combine(CommonInstance.AppTempPath, aAttachsPath(index)))) With {.Name = aAttachsName(index)}
                uscUploadAllegati.LoadDocumentInfo(doc)
            Next
        End If

        If Not String.IsNullOrEmpty(sender) Then
            Dim emails() As String = sender.Split(","c)
            For Each email As String In emails
                Dim contacts As IList(Of Contact) = Facade.ContactFacade.GetContactWithEmail(New String() {email})
                If contacts.Count > 0 Then
                    uscMittenti.DataSource.Add(New ContactDTO(contacts(0), ContactDTO.ContactType.Address))
                Else
                    ' inserisco come contatto manuale
                    Dim manualContact As Contact = New Contact()
                    manualContact.IsActive = 1
                    manualContact.Parent = Nothing
                    manualContact.ContactType = New Data.ContactType(Data.ContactType.Person)
                    manualContact.Description = String.Format("{0}|", email)
                    manualContact.EmailAddress = String.Empty
                    manualContact.FiscalCode = piva
                    manualContact.Role = Nothing

                    uscMittenti.DataSource.Add(New ContactDTO(manualContact, ContactDTO.ContactType.Manual))
                End If
            Next
            uscMittenti.DataBind()
        End If

        If Not String.IsNullOrEmpty(recipient) Then
            Dim emails() As String = recipient.Split(","c)
            For Each email As String In emails
                Dim contacts As IList(Of Contact) = Facade.ContactFacade.GetContactWithEmail(New String() {email})
                If contacts.Count > 0 Then
                    uscDestinatari.DataSource.Add(New ContactDTO(contacts(0), ContactDTO.ContactType.Address))
                Else ' inserisco come contatto manuale
                    Dim manualContact As Contact = New Contact()
                    manualContact.IsActive = 1
                    manualContact.Parent = Nothing
                    manualContact.ContactType = New Data.ContactType(Data.ContactType.Person)
                    manualContact.Description = String.Format("{0}|", email)
                    manualContact.EmailAddress = String.Empty
                    manualContact.FiscalCode = piva
                    manualContact.Role = Nothing

                    uscDestinatari.DataSource.Add(New ContactDTO(manualContact, ContactDTO.ContactType.Manual))
                End If
            Next
            uscDestinatari.DataBind()
        End If


        If Not String.IsNullOrEmpty(senderCode) Then
            Dim vCodSenders As String() = senderCode.Split(","c)
            For Each senderCode In vCodSenders
                Dim contacts As IList(Of Contact) = Facade.ContactFacade.GetContactBySearchCode(senderCode, -1)
                If contacts.Count > 0 Then
                    If contacts.Count = 1 Then
                        uscMittenti.DataSource.Add(New ContactDTO(contacts(0), ContactDTO.ContactType.Address))
                        uscMittenti.DataBind()
                    Else
                        alertMessage.AppendFormat("Codice Mittente non univoco: {0} {1}", senderCode, Environment.NewLine)
                    End If
                Else
                    alertMessage.AppendFormat("Codice Mittente non trovato: {0} {1}", senderCode, Environment.NewLine)
                End If
            Next
        End If

        If Not String.IsNullOrEmpty(recipientCode) Then
            Dim vCodRecipients As String() = recipientCode.Split(","c)
            For Each recipientCode In vCodRecipients
                Dim contacts As IList(Of Contact) = Facade.ContactFacade.GetContactBySearchCode(recipientCode, -1)
                If contacts.Count > 0 Then
                    If contacts.Count = 1 Then
                        uscDestinatari.DataSource.Add(New ContactDTO(contacts(0), ContactDTO.ContactType.Address))
                        uscDestinatari.DataBind()
                    Else
                        alertMessage.AppendFormat("Codice Destinatario non univoco: {0} {1}", recipientCode, Environment.NewLine)
                    End If
                Else
                    alertMessage.AppendFormat("Codice Destinatario non trovato: {0} {1}", recipientCode, Environment.NewLine)
                End If
            Next
        End If


        If Not String.IsNullOrEmpty(category) Then
            Dim vCodes() As String = category.Split("."c)

            Dim fullCode As String = ""
            For Each vCode As String In vCodes
                fullCode &= vCode.PadLeft(4, "0"c) & "|"
            Next

            fullCode = fullCode.Substring(0, fullCode.Length - 1)

            Dim cList As IList(Of Category) = Facade.CategoryFacade.GetCategoryByFullCode(fullCode, -1)
            If cList.Count > 0 Then
                uscClassificatori.DataSource.Add(cList(0))
                uscClassificatori.DataBind()
            Else
                alertMessage.AppendFormat("Nessuna classificatore trovato con Id: {0} {1}", category, Environment.NewLine)
            End If

        End If

        If alertMessage.Length <> 0 Then
            Throw New DocSuiteException(alertMessage.ToString())
            'AjaxAlert()
        End If

    End Sub

    Private Sub InitializeInsertMail()
        'informazioni mail
        Dim document As String = Request.QueryString("Document")
        Dim attachments As String = Request.QueryString("Attachment")
        Dim senderUser As String = Request.QueryString("Sender")
        Dim recipients As String = Request.QueryString("Recipient")
        Dim subject As String = Request.QueryString("Object")
        Dim direction As String = Request.QueryString("Direction")
        Dim path As String = Request.QueryString("Path")
        'Share

        Dim impersonator As Impersonator = CommonAD.ImpersonateSuperUser()

        Dim shareDocFolder As New DirectoryInfo(path)
        If Not shareDocFolder.Exists Then
            Exit Sub
        End If

        'documento
        If Not String.IsNullOrEmpty(document) Then
            Dim File As FileInfo() = shareDocFolder.GetFiles(document)
            If File.Length = 1 Then
                Dim Filename As String
                Dim FileN As String = Right(File(0).Name, Len(File(0).Name) - InStrRev(File(0).Name, "\"))
                Dim Valore As String = CommonShared.UserDocumentName & "-Insert-" & String.Format("{0:HHmmss}", Now()) & "-" & FileN
                'Copia il documento dalla share alla cartella temporanea
                File(0).CopyTo(CommonUtil.GetInstance().AppTempPath & Valore)
                Filename = File(0).Name.Substring(InStr(document, "-"))
                If Filename.EndsWith("body" & File(0).Extension) Then
                    Filename = "Corpo della mail" + File(0).Extension
                End If
                Dim doc As New TempFileDocumentInfo(New FileInfo(CommonUtil.GetInstance().AppTempPath & Valore)) With {.Name = Filename}
                uscUploadDocumenti.LoadDocumentInfo(doc, True)
            End If
        End If
        'allegati
        If Not String.IsNullOrEmpty(attachments) Then
            For Each allegato As String In Split(attachments, ",")
                Dim file As FileInfo() = shareDocFolder.GetFiles(allegato)
                If file.Length = 1 Then
                    Dim filename As String
                    Dim fileN As String = Right(file(0).Name, Len(file(0).Name) - InStrRev(file(0).Name, "\"))
                    Dim valore As String = CommonShared.UserDocumentName & "-Insert-" & String.Format("{0:HHmmss}", Now()) & "-" & fileN
                    'Copia il documento dalla share alla cartella temporanea
                    file(0).CopyTo(CommonUtil.GetInstance().AppTempPath & valore)
                    filename = file(0).Name.Substring(InStr(allegato, "-"))
                    If filename.EndsWith("body" & file(0).Extension) Then
                        filename = "Corpo della mail" + file(0).Extension
                    End If
                    Dim doc As New TempFileDocumentInfo(New FileInfo(CommonUtil.GetInstance().AppTempPath & valore)) With {.Name = filename}
                    uscUploadDocumenti.LoadDocumentInfo(doc, True)
                End If
            Next allegato
        End If

        impersonator.ImpersonationUndo()

        'Oggetto
        If Not IsNothing(subject) And subject <> "" Then
            uscOggetto.Text = subject
        End If
        'Destinatari
        If Not IsNothing(recipients) And recipients <> "" Then
            Dim contacts As IList(Of Contact)
            Dim destinatari As String() = Split(recipients, ",")
            Dim contactsId As String = String.Empty
            Dim contactError As String = String.Empty
            For Each destinatario As String In destinatari
                contacts = Facade.ContactFacade.GetContactWithEmail(New String() {destinatario})
                Select Case contacts.Count
                    Case 1
                        uscDestinatari.DataSource.Add(New ContactDTO(contacts(0), ContactDTO.ContactType.Address))
                    Case 0
                        contactError &= "\n" & destinatario
                    Case Is > 1
                        For Each contact As Contact In contacts
                            contactsId &= contact.Id & "|"
                        Next
                End Select
            Next destinatario
            uscDestinatari.DataBind()
            If contactError <> "" Then
                AjaxAlert("Nessun destinatario trovato con i seguenti indirizzi email: " & contactError)
            End If
            If contactsId <> "" Then
                uscDestinatari.OpenDoubleWindows(contactsId)
            End If

        End If
        'Mittenti
        If Not IsNothing(senderUser) And senderUser <> "" Then
            Dim contacts As IList(Of Contact)
            Dim Mittenti As String() = Split(senderUser, ",")
            Dim contactError As String = String.Empty
            Dim contactsId As String = String.Empty
            For Each Mittente As String In Mittenti
                contacts = Facade.ContactFacade.GetContactWithEmail(New String() {Mittente})
                Select Case contacts.Count
                    Case 1
                        uscMittenti.DataSource.Add(New ContactDTO(contacts(0), ContactDTO.ContactType.Address))
                    Case 0
                        contactError &= "\n" & Mittente
                    Case Is > 1
                        For Each contact As Contact In contacts
                            contactsId &= contact.Id & "|"
                        Next
                End Select
            Next Mittente
            uscMittenti.DataBind()
            If contactError <> "" Then
                AjaxAlert("Nessun mittente trovato con i seguenti indirizzi email: " & contactError)
            End If
            If contactsId <> "" Then
                uscMittenti.OpenDoubleWindows(contactsId)
            End If
        End If
        'Direzione
        If Not String.IsNullOrEmpty(direction) Then
            Select Case direction
                Case "IN"
                    rblTipoProtocollo.SelectedValue = "-1"
                    UpdateTipoProtocollo()
                Case "OUT"
                    rblTipoProtocollo.SelectedValue = "1"
                    UpdateTipoProtocollo()
            End Select
        End If
    End Sub

    Private Sub InitializeFromCollaboration()
        Dim idCollaboration As String = Request.QueryString("IdCollaboration")
        Dim collaboration As Collaboration = FacadeFactory.Instance.CollaborationFacade.GetById(Integer.Parse(idCollaboration))
        If collaboration Is Nothing Then
            AjaxAlert("Errore in ricerca registrazione")
            Exit Sub
        End If

        rblTipoProtocollo.SelectedValue = "1" ' Uscita

        uscOggetto.Text = collaboration.CollaborationObject
        txtNote.Text = collaboration.Note
        uscContactAssegnatario.DataSource = collaboration.RegistrationName

        InitializeFromCollaborationSourceProtocol()

        UpdateTipoProtocollo()

        ' Dizionario di appoggio per l'estrazione versioni dei documenti
        Dim dictionary As IDictionary(Of Guid, BiblosDocumentInfo)

        'Area Documentale: Documenti
        dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(collaboration, VersioningDocumentGroup.MainDocument)
        If dictionary IsNot Nothing AndAlso dictionary.Values.Count = 1 Then
            Try
                uscUploadDocumenti.LoadDocumentInfo(New List(Of DocumentInfo)(dictionary.Values))
                uscUploadDocumenti.InitializeNodesAsAdded(True)
            Catch ex As Exception
                FileLogger.Warn(LoggerName, "Errore estrazione Documento: " & ex.Message, ex)
                AjaxAlert("Errore estrazione Documento: " & ex.Message)
            End Try
        End If

        'Area Documentale: Allegati
        dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(collaboration, VersioningDocumentGroup.Attachment)
        If dictionary IsNot Nothing AndAlso dictionary.Values.Count > 0 Then
            Try
                uscUploadAllegati.LoadDocumentInfo(New List(Of DocumentInfo)(dictionary.Values))
                uscUploadAllegati.InitializeNodesAsAdded(True)
            Catch ex As Exception
                FileLogger.Warn(LoggerName, "Errore estrazione Allegati: " & ex.Message, ex)
                AjaxAlert("Errore estrazione Allegati: " & ex.Message)
            End Try
        End If

        'Area Documentale: Annessi
        dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(collaboration, VersioningDocumentGroup.Annexed)
        If dictionary IsNot Nothing AndAlso dictionary.Values.Count > 0 Then

            Dim annexed As New List(Of DocumentInfo)
            For Each item As DocumentInfo In dictionary.Values
                If FileHelper.MatchExtension(item.Extension, FileHelper.XLS) Then
                    Try
                        uscMittenti.ImportFromExcel(item, True)
                        Continue For
                    Catch ex As Exception
                        ' Se il documento passato è valido per l'importazione dei contatti
                        FileLogger.Debug(LoggerName, "File XLS non valido per i contatti.", ex)
                    End Try
                End If
                annexed.Add(item)
            Next

            Try
                If annexed IsNot Nothing AndAlso annexed.Count > 0 Then
                    uscUploadAnnexes.LoadDocumentInfo(annexed)
                    uscUploadAnnexes.InitializeNodesAsAdded(True)
                End If
            Catch ex As Exception
                FileLogger.Warn(LoggerName, "Errore estrazione Annessi.", ex)
                AjaxAlert("Errore estrazione Annessi: " & ex.Message)
            End Try
        End If

        ' Inserimento da DRAFT
        Dim draft As CollaborationXmlData = Facade.ProtocolDraftFacade.GetDataFromCollaboration(collaboration)
        If draft IsNot Nothing Then
            ' Inizializzo il protocollo da bozza
            BindDraft(draft)
        End If

        InitializeFromWorkflow(collaboration)
    End Sub

    Private Sub InitializeFromUDS()
        Dim repository As UDSRepository = CurrentUDSRepositoryFacade.GetById(CurrentUDSRepositoryId.Value)
        Dim source As UDSDto = CurrentUDSFacade.GetUDSSource(repository, String.Format(ODATA_EQUAL_UDSID, CurrentUDSId.Value))
        Dim protocolInitializer As ProtocolInitializer = CurrentUDSRepositoryFacade.GetProtocolInitializer(source)
        UseProtocolInitializer(protocolInitializer)
    End Sub

    Private Sub InitializeFromWorkflow(Optional collaboration As Collaboration = Nothing)
        If CurrentIdWorkflowActivity.HasValue Then
            Dim dsw_p_Model As WorkflowProperty = CurrentWorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(CurrentIdWorkflowActivity.Value, WorkflowPropertyHelper.DSW_PROPERTY_MODEL)
            If dsw_p_Model IsNot Nothing AndAlso dsw_p_Model.PropertyType = WorkflowPropertyType.Json AndAlso Not String.IsNullOrEmpty(dsw_p_Model.ValueString) Then
                Dim protocolModel As ProtocolModel = Nothing
                Try
                    protocolModel = JsonConvert.DeserializeObject(Of ProtocolModel)(dsw_p_Model.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
                Catch ex As Exception
                End Try
                If protocolModel IsNot Nothing Then
                    Dim dsw_a_CollaborationSignSummaryTemplateId As WorkflowProperty = CurrentWorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(CurrentIdWorkflowActivity.Value, "_dsw_a_CollaborationSignSummaryTemplateId")
                    Dim dsw_a_Collaboration_GenerateSignSummary As WorkflowProperty = CurrentWorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(CurrentIdWorkflowActivity.Value, "_dsw_a_Collaboration_GenerateSignSummary")
                    Dim dsw_e_UDSId As WorkflowProperty = CurrentWorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(CurrentIdWorkflowActivity.Value, WorkflowPropertyHelper.DSW_FIELD_UDS_ID)
                    Dim dsw_e_UDSRepositoryId As WorkflowProperty = CurrentWorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(CurrentIdWorkflowActivity.Value, WorkflowPropertyHelper.DSW_FIELD_UDS_REPOSITORY_ID)
                    Dim dsw_p_ProposerRole As WorkflowProperty = CurrentWorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(CurrentIdWorkflowActivity.Value, WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE)
                    Dim udsDto As UDSDto = Nothing
                    If dsw_e_UDSId IsNot Nothing AndAlso dsw_e_UDSRepositoryId IsNot Nothing AndAlso dsw_e_UDSId.ValueGuid.HasValue AndAlso dsw_e_UDSRepositoryId.ValueGuid.HasValue Then
                        Dim repository As UDSRepository = CurrentUDSRepositoryFacade.GetById(dsw_e_UDSRepositoryId.ValueGuid.Value)
                        udsDto = CurrentUDSFacade.GetUDSSource(repository, String.Format(ODATA_EQUAL_UDSID, dsw_e_UDSId.ValueGuid.Value))
                    End If

                    Dim protocolInitializer As ProtocolInitializer = New ProtocolModelInitializer().GetProtocolInitializer(DocSuiteContext.Current.Tenants, protocolModel, collaboration,
                                dsw_a_CollaborationSignSummaryTemplateId, dsw_a_Collaboration_GenerateSignSummary, dsw_p_ProposerRole, udsDto)
                    UseProtocolInitializer(protocolInitializer)
                    End If
                End If
        End If
    End Sub

    Private Sub InitializePageFromWorkflowUI(workflowReferenceModel As WorkflowReferenceModel, workflowStartModel As WorkflowStart)
        If workflowReferenceModel Is Nothing OrElse workflowStartModel Is Nothing Then
            Exit Sub
        End If
        Dim workflowArgument As WorkflowArgument = Nothing
        workflowArgument = workflowStartModel.Arguments.Values.SingleOrDefault(Function(f) f.Name = WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT)
        If workflowArgument IsNot Nothing AndAlso String.IsNullOrEmpty(uscOggetto.Text) Then
            uscOggetto.Text = workflowArgument.ValueString
        End If
        If Not String.IsNullOrEmpty(workflowReferenceModel.ReferenceModel) AndAlso workflowReferenceModel.ReferenceType = Model.Entities.Commons.DSWEnvironmentType.Fascicle Then
            Dim fascicle As Entity.Fascicles.Fascicle = JsonConvert.DeserializeObject(Of Entity.Fascicles.Fascicle)(workflowReferenceModel.ReferenceModel)
            txtNote.Text = $"Protocollo originato dal fascicolo {fascicle.Title} - {fascicle.FascicleObject}"
            hf_workflowAction_toFascicle.Value = fascicle.UniqueId.ToString()
            If fascicle.Container IsNot Nothing Then
                CurrentContainerControl.SelectedValue = fascicle.Container.EntityShortId.ToString()
                ContainerControlSelectionChanged()
            End If
        End If
        If workflowReferenceModel.Documents IsNot Nothing Then
            Dim workflowReferenceBiblosModel As WorkflowReferenceBiblosModel = workflowReferenceModel.Documents.SingleOrDefault(Function(f) f.ChainType = Model.Entities.DocumentUnits.ChainType.MainChain AndAlso f.ArchiveChainId.HasValue AndAlso f.ArchiveDocumentId.HasValue)
            Dim temppath As New DirectoryInfo(CommonUtil.GetInstance().AppTempPath)
            Dim doc As BiblosDocumentInfo = Nothing
            Dim tempDoc As TempFileDocumentInfo = Nothing
            Dim results As List(Of DocumentInfo) = Nothing

            If workflowReferenceBiblosModel IsNot Nothing Then
                doc = New BiblosDocumentInfo(String.Empty, workflowReferenceBiblosModel.ArchiveDocumentId.Value, workflowReferenceBiblosModel.ArchiveChainId.Value)
                tempDoc = New TempFileDocumentInfo(workflowReferenceBiblosModel.DocumentName, BiblosFacade.SaveUniqueToTemp(doc))
                uscUploadDocumenti.LoadDocumentInfo(tempDoc, False, True, False, True)
                uscUploadDocumenti.InitializeNodesAsAdded(False)
            End If

            results = New List(Of DocumentInfo)()
            For Each workflowReferenceBiblosModel In workflowReferenceModel.Documents.Where(Function(f) f.ChainType = Model.Entities.DocumentUnits.ChainType.AttachmentsChain AndAlso f.ArchiveChainId.HasValue AndAlso f.ArchiveDocumentId.HasValue)
                doc = New BiblosDocumentInfo(String.Empty, workflowReferenceBiblosModel.ArchiveDocumentId.Value, workflowReferenceBiblosModel.ArchiveChainId.Value)
                results.Add(New TempFileDocumentInfo(workflowReferenceBiblosModel.DocumentName, BiblosFacade.SaveUniqueToTemp(doc)))
            Next
            If results.Any() Then
                uscUploadAllegati.LoadDocumentInfo(results, False, True, False, True)
                uscUploadAllegati.InitializeNodesAsAdded(True)
            End If

            results = New List(Of DocumentInfo)()
            For Each workflowReferenceBiblosModel In workflowReferenceModel.Documents.Where(Function(f) f.ChainType = Model.Entities.DocumentUnits.ChainType.AnnexedChain AndAlso f.ArchiveChainId.HasValue AndAlso f.ArchiveDocumentId.HasValue)
                doc = New BiblosDocumentInfo(String.Empty, workflowReferenceBiblosModel.ArchiveDocumentId.Value, workflowReferenceBiblosModel.ArchiveChainId.Value)
                results.Add(New TempFileDocumentInfo(workflowReferenceBiblosModel.DocumentName, BiblosFacade.SaveUniqueToTemp(doc)))
            Next
            If results.Any() Then
                uscUploadAnnexes.LoadDocumentInfo(results, False, True, False, True)
                uscUploadAnnexes.InitializeNodesAsAdded(True)
            End If
        End If

    End Sub

    Private Sub InitializeFromCollaborationSourceProtocol()
        ' Verifico che sia attiva la funzionalità, 
        ' che esista una Collaborazione di origine
        ' e che esista il relativo Protocollo di origine della Collaborazione.
        If Not DocSuiteContext.Current.ProtocolEnv.CollaborationSourceProtocolEnabled _
            OrElse CurrentCollaboration Is Nothing _
            OrElse Not CurrentCollaboration.HasSourceProtocol Then
            Return
        End If

        CurrentContainerControl.SelectedValue = CurrentCollaboration.SourceProtocol.Container.Id
        ContainerControlSelectionChanged()

        rblTipoProtocollo.SelectedValue = "1" ' Uscita
        rblTipoProtocollo.Enabled = False

        uscClassificatori.DataSource.Add(Me.CurrentCollaboration.SourceProtocol.Category)
        uscClassificatori.DataBind()

        uscDestinatari.DataSource = FacadeFactory.Instance.ProtocolFacade.GetSenders(Me.CurrentCollaboration.SourceProtocol)
        uscDestinatari.DataBind()

        If DocSuiteContext.Current.ProtocolEnv.IsIssueEnabled Then
            Dim contacts As List(Of ContactDTO) = CurrentCollaboration.SourceProtocol.ContactIssues _
                .Select(Function(c) FacadeFactory.Instance.ContactFacade.CreateDTO(c)) _
                .Where(Function(d) d IsNot Nothing) _
                .ToList()

            uscFascicle.DataSource = contacts
            uscFascicle.DataBind()
        End If
    End Sub

    Private Sub InitFromPECSourceProtocol()
        If Not HasPECSourceProtocol Then
            Return
        End If

        If Not DocSuiteContext.Current.ProtocolEnv.PECSourceProtocolIdContainer.Equals(0) Then
            CurrentContainerControl.SelectedValue = DocSuiteContext.Current.ProtocolEnv.PECSourceProtocolIdContainer.ToString()
        End If

        If DocSuiteContext.Current.ProtocolEnv.IsTableDocTypeEnabled Then
            Dim documentType As DocumentType = FacadeFactory.Instance.TableDocTypeFacade.GetByCode("PEC")
            If documentType IsNot Nothing Then
                cboIdDocType.SelectedValue = documentType.Id.ToString()
            End If
        End If

        uscOggetto.Text = CurrentPECMail.MailSubject

        If Not String.IsNullOrWhiteSpace(DocSuiteContext.Current.ProtocolEnv.PECSourceProtocolIdRoles) Then
            Dim idRoles As List(Of Integer) = DocSuiteContext.Current.ProtocolEnv.PECSourceProtocolIdRoles _
                                              .Split({"|"c}, StringSplitOptions.RemoveEmptyEntries) _
                                              .Distinct().Select(Function(i) Integer.Parse(i)).ToList()
            Dim roles As IList(Of Role) = FacadeFactory.Instance.RoleFacade.GetByIds(idRoles)
            uscAutorizzazioni.SourceRoles.AddRange(roles)
            uscAutorizzazioni.DataBind()
        End If

        uscClassificatori.DataSource.Add(Me.CurrentPECSourceProtocol.Category)
        uscClassificatori.DataBind()

        Dim fascicles As List(Of ContactDTO) = CurrentPECSourceProtocol.ContactIssues _
                                              .Select(Function(c) FacadeFactory.Instance.ContactFacade.CreateDTO(c)) _
                                              .Where(Function(d) d IsNot Nothing) _
                                              .ToList()

        uscFascicle.DataSource = fascicles
        uscFascicle.DataBind()
    End Sub

    Private Sub ContainerControlSelectionChanged()
        If ProtocolEnv.IsTableDocTypeEnabled Then
            'Carico l'ultima impostazione del tipo documento fatta
            Dim prevSelectedDocType As String = cboIdDocType.SelectedValue
            If (String.IsNullOrEmpty(prevSelectedDocType)) Then
                'Se non c'è già un valore impostato manualmente e se c'è una PEC collegata (ovvero sto protocollando la PEC) e se esiste un tipo documento di default impostato
                If CurrentMail IsNot Nothing Then
                    'Casistica ProtocolBox
                    If ProtocolBoxEnabled AndAlso Not String.IsNullOrEmpty(ProtocolEnv.ProtocolBoxProtocolDocTypeDefault) Then
                        'Imposto il tipo documento al valore di default
                        prevSelectedDocType = ProtocolEnv.ProtocolBoxProtocolDocTypeDefault
                    ElseIf Not String.IsNullOrEmpty(ProtocolEnv.PecProtocolDocTypeDefault) Then
                        'Imposto il tipo documento al valore di default
                        prevSelectedDocType = ProtocolEnv.PecProtocolDocTypeDefault
                    End If
                End If
            End If
            Dim intValue As Integer = 0
            If Not String.IsNullOrEmpty(CurrentContainerControl.SelectedValue) AndAlso Integer.TryParse(CurrentContainerControl.SelectedValue, intValue) Then
                BindDocType(intValue)
            Else
                BindDocType(Nothing)
            End If

            If Not String.IsNullOrEmpty(prevSelectedDocType) Then
                If cboIdDocType.Items.FindByValue(prevSelectedDocType) IsNot Nothing Then
                    cboIdDocType.ClearSelection()
                    cboIdDocType.Items.FindByValue(prevSelectedDocType).Selected = True
                End If
            End If
            intValue = 0
            If Not String.IsNullOrEmpty(cboIdDocType.SelectedValue) AndAlso Integer.TryParse(cboIdDocType.SelectedValue, intValue) Then
                DocTypeHiddenFields(intValue)
            End If
        End If

        If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso Not String.IsNullOrEmpty(CurrentContainerControl.SelectedValue) Then
            InitDocumentsPrivacyLevels(True)
        End If

        If Not String.IsNullOrEmpty(CurrentContainerControl.SelectedValue) AndAlso DocSuiteContext.Current.SimplifiedPrivacyEnabled Then
            Dim hasPrivacyRight As Boolean = Facade.ContainerGroupFacade.HasContainerRight(Convert.ToInt32(CurrentContainerControl.SelectedValue), DocSuiteContext.Current.User.Domain, DocSuiteContext.Current.User.UserName, ProtocolContainerRightPositions.Privacy, DSWEnvironment.Protocol)
            uscAutorizzazioni.UserAuthorizationEnabled = hasPrivacyRight
            uscAutorizzazioni.PrivacyAuthorizationButtonVisible = hasPrivacyRight
            uscAutorizzazioni.InitializePrivacyAuthorization()
            uscAutorizzazioni.InitializeUserAuthorization()
        End If

        ' Qualora sia abilitata la gestione degli organigrammi precompilo l'inserimento con le risorse condivise.
        If Not (Action = "Duplicate") Then
            BindOChartResources()
        End If

        ''Check behaviours
        If ProtocolEnv.ContainerBehaviourEnabled AndAlso Not String.IsNullOrEmpty(CurrentContainerControl.SelectedValue) Then
            ApplyContainerBehaviours(CurrentContainerControl.SelectedContainer("ProtDB"))
        End If

        If Not ProtocolEnv.IsInvoiceEnabled OrElse String.IsNullOrEmpty(CurrentContainerControl.SelectedValue) Then
            Exit Sub
        End If

        Dim containerExtFacade As New ContainerExtensionFacade("ProtDB")
        rblTipoProtocollo.Enabled = True
        Dim containerExtensions As IList(Of ContainerExtension) = containerExtFacade.GetByContainerAndKey(CurrentContainerControl.SelectedValue, ContainerExtensionType.FT)
        If containerExtensions.Count > 0 AndAlso containerExtensions(0).KeyValue = "1" Then
            Dim containerExtensionsSearch As IList = containerExtFacade.GetByContainerAndKey(CurrentContainerControl.SelectedValue, ContainerExtensionType.SC)
            If Not containerExtensionsSearch Is Nothing Then
                ddlAccountingSectional.Items.Clear()
                For Each cntExt As ContainerExtension In containerExtensionsSearch
                    ddlAccountingSectional.Items.Add("" & cntExt.KeyValue)
                Next
                pnlSectionalType.Visible = True
            Else
                pnlSectionalType.Visible = False
            End If
            Dim containerExtensionsTest As IList = containerExtFacade.GetByContainerAndKey(CurrentContainerControl.SelectedValue, ContainerExtensionType.TP)
            Select Case containerExtensionsTest(0).KeyValue
                Case "A"
                    pnlAccounting.Visible = False
                    rblTipoProtocollo.SelectedValue = "1"
                    rblTipoProtocollo.Enabled = False
                    UpdateTipoProtocollo()
                Case "P"
                    pnlAccounting.Visible = True
                    rblTipoProtocollo.SelectedValue = "-1"
                    rblTipoProtocollo.Enabled = False
                    UpdateTipoProtocollo()
                Case Else
                    AjaxAlert("Tipo Contenitore non gestito, Tipologie valide A/P")
            End Select

            ' Se il tipo di numeratore è alfanumerico, adeguo coerentemente la validazione del campo numero fattura.
            If Not String.IsNullOrEmpty(containerExtensions(0).NumeratorType) AndAlso containerExtensions(0).NumeratorType.Eq("A") Then
                cvInvoiceNumber.Type = ValidationDataType.String
                cvInvoiceNumber.Operator = ValidationCompareOperator.NotEqual
                cvInvoiceNumber.ValueToCompare = String.Empty
            Else
                cvInvoiceNumber.Type = ValidationDataType.Integer
                cvInvoiceNumber.Operator = ValidationCompareOperator.GreaterThan
                cvInvoiceNumber.ValueToCompare = "0"
            End If

            pnlInvoice.Visible = True
            uscDestinatari.IsFiscalCodeRequired = True
            uscMittenti.IsFiscalCodeRequired = True
        Else
            pnlInvoice.Visible = False
            uscDestinatari.IsFiscalCodeRequired = False
            uscMittenti.IsFiscalCodeRequired = False
            If String.IsNullOrWhiteSpace(rblTipoProtocollo.SelectedValue) Then
                rblTipoProtocollo.SelectedValue = "-1"
                UpdateTipoProtocollo()
            End If
        End If
    End Sub

    Private Sub ApplyContainerBehaviours(ByVal container As Container)
        Dim behaviours As IList(Of ContainerBehaviour) = Facade.ContainerBehaviourFacade.GetBehaviours(container, ContainerBehaviourAction.Insert, String.Empty)
        Try
            For Each behaviour As ContainerBehaviour In behaviours
                ' Cerco il Target
                Dim target As Control = Nothing

                Select Case True
                    Case behaviour.AttributeName.StartsWith("#") ' ID controllo
                        target = Page.FindControlRecursive(behaviour.AttributeName.Substring(1))
                    Case behaviour.AttributeName.StartsWith("§") ' Controllo, Nome dell'attributo
                        ' TODO
                    Case behaviour.AttributeName.StartsWith("@") ' Attributo
                        ' TODO
                    Case behaviour.AttributeName.StartsWith("$") ' Item, via Reflection
                        ' TODO
                End Select

                If target Is Nothing Then
                    FileLogger.Warn(LoggerName, String.Format("Target '{0}' non trovato.", behaviour.AttributeName))
                    Continue For
                End If

                ' Verifico se il target è valorizzato già
                Dim myValue As Object = GetControlValue(target)
                If myValue IsNot Nothing AndAlso behaviour.KeepValue Then
                    Continue For ' Campo già valorizzato, passo al successivo
                End If

                ' Imposto il valore
                Dim setValue As String = behaviour.AttributeValue
                SetControlValue(target, setValue, "{0}")
            Next
        Catch ex As Exception
            AjaxAlert("Errore in esecuzione comando.")
            FileLogger.Error(LoggerName, "Errore in esecuzione comando.", ex)
        End Try
    End Sub

    Private Shared Function GetControlValue(source As Control) As Object
        Dim radTextBox As RadTextBox = TryCast(source, RadTextBox)
        If (radTextBox IsNot Nothing) AndAlso Not String.IsNullOrEmpty(radTextBox.Text) Then
            Return radTextBox.Text
        End If

        Dim radNumericTextBox As RadNumericTextBox = TryCast(source, RadNumericTextBox)
        If (radNumericTextBox IsNot Nothing) AndAlso radNumericTextBox.Value.HasValue Then
            Return radNumericTextBox.Value
        End If

        Dim radDatePicker As RadDatePicker = TryCast(source, RadDatePicker)
        If (radDatePicker IsNot Nothing) AndAlso radDatePicker.SelectedDate.HasValue Then
            Return radDatePicker.SelectedDate.Value
        End If

        Dim label As WebControls.Label = TryCast(source, WebControls.Label)
        If (label IsNot Nothing) AndAlso Not String.IsNullOrEmpty(label.Text) Then
            Return label.Text
        End If

        Dim classificatore As uscClassificatore = TryCast(source, uscClassificatore)
        If (classificatore IsNot Nothing) AndAlso Not classificatore.SelectedCategories.IsNullOrEmpty() Then
            Return classificatore.SelectedCategories.Item(0)
        End If

        FileLogger.Warn(LoggerName, String.Format("Controllo [{0}] in ProtInserimento non gestito correttamente.", source.ID))
        Return Nothing

    End Function

    Private Sub SetControlValue(control As Control, value As Object, format As String)
        FileLogger.Warn(LoggerName, String.Format("Controllo [{0}] in ProtInserimento impostato a null", control.ID))

        If String.IsNullOrEmpty(format) Then
            format = "{0}"
        End If

        Dim radTextBox As RadTextBox = TryCast(control, RadTextBox)
        If (radTextBox IsNot Nothing) Then
            If value Is Nothing Then
                radTextBox.Text = String.Empty
            Else
                radTextBox.Text = DirectCast(value, String)
            End If
            Return
        End If

        Dim radDatePicker As RadDatePicker = TryCast(control, RadDatePicker)
        If (radDatePicker IsNot Nothing) Then
            If value Is Nothing Then
                radDatePicker.SelectedDate = Nothing
            Else
                radDatePicker.SelectedDate = DirectCast(value, Date)
            End If
            Return
        End If

        Dim radNumericTextBox As RadNumericTextBox = TryCast(control, RadNumericTextBox)
        If (radNumericTextBox IsNot Nothing) Then

            If value Is Nothing Then
                radNumericTextBox.Value = Nothing
            Else
                Dim o As String = TryCast(value, String)
                If (o IsNot Nothing) Then
                    value = Double.Parse(o)
                End If
                radNumericTextBox.Value = DirectCast(value, Double)
            End If
            Return
        End If

        Dim label As Label = TryCast(control, Label)
        If (label IsNot Nothing) Then
            If value Is Nothing Then
                label.Text = String.Empty
                ViewState(label.ID) = String.Empty
            Else
                label.Text = String.Format(format, value)
                ViewState(label.ID) = label.Text
            End If
            Return
        End If

        Dim usc As uscClassificatore = TryCast(control, uscClassificatore)
        If (usc IsNot Nothing) Then
            Try
                If value Is Nothing Then
                    usc.DataSource.Clear()
                    usc.DataBind()
                Else
                    Dim cat As Category = (New CategoryFacade).GetById(CType(value, Integer))
                    If cat IsNot Nothing Then
                        usc.DataSource.Add(cat)
                        usc.DataBind()
                    End If
                End If
            Catch ex As Exception
                FileLogger.Warn(LoggerName, String.Format("Errore in caricamento valore di default per {0}: {1}", control.ID, value))
            End Try
        End If
    End Sub

    Private Function CheckPackageError() As Boolean
        If Not ViewState("PackageError") Is Nothing AndAlso Convert.ToBoolean(ViewState("PackageError")) Then
            Return True
        End If
        Return False
    End Function

    Private Sub EnableInsertButton(ByVal enable As Boolean)
        Dim disable As Boolean = (Not enable)
        ClientScript.RegisterStartupScript(Me.GetType(), "AbilitaConferma" & enable.ToString(), "<SCRIPT LANGUAGE=""javascript"">document.getElementById('btnInserimentoV').disabled = " & disable.ToString().ToLower() & ";</SCRIPT>")
        ViewState("PackageError") = disable
    End Sub

    Private Sub BindForm(ByRef protocol As Protocol)
        If (Not protocol.ProtocolObject.Eq("DUMMY")) Then
            uscOggetto.Text = StringHelper.Clean(protocol.ProtocolObject, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
        End If

        If protocol.Container IsNot Nothing AndAlso String.IsNullOrEmpty(CurrentContainerControl.SelectedValue) Then
            CurrentContainerControl.SelectedValue = protocol.Container.Id.ToString()
            ContainerControlSelectionChanged()
        End If

        'Tipologia protocollo
        rblTipoProtocollo.SelectedValue = protocol.Type.Id.ToString()
        UpdateTipoProtocollo(needSwap:=False)

        If Not String.IsNullOrEmpty(protocol.Note) AndAlso String.IsNullOrEmpty(txtNote.Text) Then
            txtNote.Text = protocol.Note
        End If

        If ProtocolEnv.IsInvoiceEnabled AndAlso Not ProtocolEnv.InvoiceSDIEnabled Then
            If Not String.IsNullOrEmpty(protocol.InvoiceNumber) AndAlso String.IsNullOrEmpty(txtInvoiceNumber.Text) Then
                txtInvoiceNumber.Text = protocol.InvoiceNumber
            End If

            If protocol.AccountingNumber.HasValue AndAlso String.IsNullOrEmpty(txtAccountingNumber.Text) Then
                txtAccountingNumber.Text = protocol.AccountingNumber.Value.ToString()
            End If

            If protocol.InvoiceDate.HasValue AndAlso Not dtpInvoiceDate.SelectedDate.HasValue Then
                dtpInvoiceDate.SelectedDate = protocol.InvoiceDate.Value
            End If

            If protocol.AccountingDate.HasValue AndAlso Not rdpAccountingDate.SelectedDate.HasValue Then
                rdpAccountingDate.SelectedDate = protocol.AccountingDate.Value
            End If


            If Not String.IsNullOrEmpty(protocol.AccountingSectional) AndAlso String.IsNullOrEmpty(ddlAccountingSectional.Text) Then
                ddlAccountingSectional.SelectedValue = protocol.AccountingSectional
            End If

        End If


        'Assegnatario
        If String.IsNullOrEmpty(uscContactAssegnatario.DataSource) Then
            uscContactAssegnatario.DataSource = protocol.Subject
        End If

        If String.IsNullOrEmpty(SelServiceCategory.Text) Then
            SelServiceCategory.CategoryText = protocol.ServiceCategory
        End If

        If uscAutorizzazioni.GetRoles().Count = 0 Then
            For Each vProtRole As ProtocolRole In protocol.Roles
                uscAutorizzazioni.SourceRoles.Add(vProtRole.Role)
            Next
            uscAutorizzazioni.DataBind()
        End If

        If uscClassificatori.SelectedCategories.Count = 0 Then
            Dim categoryToDuplicate As Category = protocol.Category
            If Action.Eq("Duplicate") Then
                If Not Facade.CategoryFacade.IsCategoryActive(categoryToDuplicate) Then
                    categoryToDuplicate = Nothing
                End If
            End If
            uscClassificatori.DataSource.Add(categoryToDuplicate)
            uscClassificatori.DataBind()
        End If

        Dim mittentiAlreadySource As Boolean = uscMittenti.GetContacts(False).Count > 0
        Dim destinatariAlreadySource As Boolean = uscDestinatari.GetContacts(False).Count > 0
        For Each vContact As ProtocolContact In protocol.Contacts
            Dim vContactDTO As New ContactDTO
            vContactDTO.Contact = vContact.Contact
            vContactDTO.IsCopiaConoscenza = vContact.Type.Eq("CC")
            vContactDTO.Type = ContactDTO.ContactType.Address
            If vContact.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                If Not mittentiAlreadySource Then
                    uscMittenti.DataSource.Add(vContactDTO)
                End If
            Else
                If Not destinatariAlreadySource Then
                    uscDestinatari.DataSource.Add(vContactDTO)
                End If
            End If
        Next

        For Each vContact As ProtocolContactManual In protocol.ManualContacts
            Dim vContactDTO As New ContactDTO
            vContactDTO.Contact = vContact.Contact
            vContactDTO.IsCopiaConoscenza = vContact.Type.Eq("CC")
            vContactDTO.Type = ContactDTO.ContactType.Manual
            If vContact.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                If Not mittentiAlreadySource Then
                    uscMittenti.DataSource.Add(vContactDTO)
                End If
            Else
                If Not destinatariAlreadySource Then
                    uscDestinatari.DataSource.Add(vContactDTO)
                End If
            End If
        Next

        If Not mittentiAlreadySource Then
            uscMittenti.DataBind()
        End If

        If Not destinatariAlreadySource Then
            uscDestinatari.DataBind()
        End If

        If protocol.DocumentType IsNot Nothing AndAlso String.IsNullOrEmpty(cboIdDocType.SelectedValue) Then
            cboIdDocType.SelectedValue = protocol.DocumentType.Id.ToString()
            cboIdDocType_SelectedIndexChanged(cboIdDocType, New EventArgs())
        End If
    End Sub

    ''' <summary> Popola gli user control con gli attributes di protocollazione, inseriti dal wscoll. </summary>

    Private Sub BindDraft(draft As CollaborationXmlData)
        Try
            Select Case draft.GetType()
                Case GetType(ProtocolXML)
                    Dim protocolDraft As ProtocolXML = CType(draft, ProtocolXML)
                    uscOggetto.Text = protocolDraft.Object
                    ' Note
                    txtNote.Text = protocolDraft.Notes
                    ' Container
                    CurrentContainerControl.SelectedValue = protocolDraft.Container.ToString()
                    ContainerControlSelectionChanged()

                    ' Assegnatario
                    uscContactAssegnatario.DataSource = protocolDraft.Assignee
                    ' Categoria di Servizio
                    SelServiceCategory.CategoryText = protocolDraft.ServiceCode
                    ' Classificazione
                    Dim category As Category = Facade.CategoryFacade.GetById(protocolDraft.Category)
                    If category IsNot Nothing Then
                        uscClassificatori.DataSource.Add(category)
                    End If
                    uscClassificatori.DataBind()

                    ' Settori
                    For Each role As Role In From item In protocolDraft.Authorizations Select role1 = Facade.RoleFacade.GetById(item) Where role1 IsNot Nothing
                        uscAutorizzazioni.SourceRoles.Add(role)
                    Next
                    uscAutorizzazioni.DataBind()

                    ' Tipo Protocollo
                    rblTipoProtocollo.SelectedValue = protocolDraft.Type.ToString()

                    UpdateTipoProtocollo()

                    ' Inserimento dei contatti dei Mittenti
                    BindContactsFromContactBag(protocolDraft.Senders, True)
                    uscMittenti.DataBind()

                    ' Inserimento dei contatti dei Destinatari
                    BindContactsFromContactBag(protocolDraft.Recipients, False)
                    uscDestinatari.DataBind()

                    cboIdDocType.SelectedValue = protocolDraft.DocumentType.ToString()
                Case GetType(ResolutionXML)
                    'Dim resolutionDraft As ResolutionXML = CType(draft, ResolutionXML)
                    'If resolutionDraft.ResolutionsToUpdate IsNot Nothing Then
                    'UpdateResolutions(resolutionDraft)
                    'End If
            End Select

        Catch ex As Exception
            Throw New DocSuiteException("Errore", "Problema in fase di caricamento bozza di protocollo.", ex)
        End Try
    End Sub

    Private Sub BindContactsFromContactBag(ByVal listContactBag As List(Of ContactBag), ByVal isSender As Boolean)
        Dim contactTypeFacade As New ContactTypeFacade()

        For Each contactBag As ContactBag In listContactBag
            For Each contactXml As ContactXML In contactBag.Contacts
                Select Case contactBag.SourceType
                    Case ContactTypeEnum.AddressBook, ContactTypeEnum.AD
                        'Case ContactTypeEnum.IPA
                        Dim contact As Contact = Facade.ContactFacade.GetById(contactXml.Id)
                        If contact Is Nothing Then
                            Throw New ArgumentException(String.Format("Attenzione contatto non presente in rubrica id: {0}", contactXml.Id), "id")
                        End If

                        Dim vContactDTO As New ContactDTO
                        vContactDTO.Contact = contact
                        vContactDTO.IsCopiaConoscenza = contactXml.Cc
                        vContactDTO.Type = ContactDTO.ContactType.Address
                        If isSender Then  ' Mittenti
                            uscMittenti.DataSource.Add(vContactDTO)
                        Else              ' Destinatari
                            uscDestinatari.DataSource.Add(vContactDTO)
                        End If
                    Case Else
                        Dim contact As New Contact()
                        ' Tipo di Contatto
                        Dim contactType As Data.ContactType = contactTypeFacade.GetById(contactXml.Type.First())
                        If contactType Is Nothing Then
                            Throw New ArgumentException(String.Format("Attenzione tipo di contatto non presente type: {0}", contactXml.Type), "Type")
                        End If

                        If contactXml.Description IsNot Nothing Then
                            contact.Description = contactXml.Description
                        Else
                            contact.Description = String.Format("{0}|{1}", contactXml.Surname, contactXml.Name)
                        End If

                        contact.ContactType = contactType
                        contact.EmailAddress = contactXml.StandardMail
                        contact.CertifiedMail = contactXml.CertifiedMail
                        contact.FiscalCode = contactXml.FiscalCode

                        ' Indirizzo contatto
                        Dim address As New Address
                        If contactXml.Address IsNot Nothing Then
                            address.Address = contactXml.Address.Name
                            address.City = contactXml.Address.City
                            address.ZipCode = contactXml.Address.Cap
                            address.CivicNumber = contactXml.Address.Number
                            address.CityCode = contactXml.Address.Prov
                            If Not String.IsNullOrEmpty(contactXml.Address.Type) Then
                                Dim lstPlaceName As IList(Of ContactPlaceName) = Facade.ContactPlaceNameFacade.GetByDescription(contactXml.Address.Type)
                                If Not lstPlaceName.IsNullOrEmpty Then
                                    address.PlaceName = lstPlaceName(0)
                                End If
                            End If
                        End If

                        contact.Address = address
                        contact.TelephoneNumber = contactXml.Telephone
                        contact.FaxNumber = contactXml.Fax
                        contact.Note = contactXml.Notes
                        contact.BirthPlace = contactXml.BirthPlace
                        ' Data
                        Dim data As Date
                        If DateTime.TryParseExact(contactXml.BirthDate, "d/M/yyyy", Nothing, DateTimeStyles.None, data) Then
                            contact.BirthDate = data
                        Else
                            contact.BirthDate = Nothing
                        End If

                        Dim vContactDTO As New ContactDTO
                        vContactDTO.Contact = contact
                        vContactDTO.IsCopiaConoscenza = contactXml.Cc
                        vContactDTO.Type = ContactDTO.ContactType.Manual
                        If isSender Then ' Mittenti
                            uscMittenti.DataSource.Add(vContactDTO)
                        Else             ' Destinatari
                            uscDestinatari.DataSource.Add(vContactDTO)
                        End If
                End Select
            Next
        Next
    End Sub

    ''' <summary>
    ''' Verifica quali campi del protocollo sono stati scelti dall'utente per essere duplicati
    ''' </summary>
    ''' <param name="field">Campo da verificare</param>
    ''' <param name="right">Valore</param>
    ''' <returns>True se il campo è da duplicare, false altrimenti</returns>
    Private Function GetCheck(ByVal field As String, ByVal right As Integer) As Boolean
        If Mid$(field, right, 1) = "1" Then
            Return True
        End If
        Return False
    End Function

    Private Function DuplicateProtocol() As Boolean
        Dim hasDisabledElements As Boolean = False
        Dim Year As Short = Request.QueryString("Year")
        Dim Number As Integer = Request.QueryString("Number")
        Dim Check As String = Request.QueryString("Check")
        Dim vProtocol As Protocol = Facade.ProtocolFacade.Duplicate(Year, Number, hasDisabledElements,
            GetCheck(Check, ProtocolDuplicaOption.Container),
            GetCheck(Check, ProtocolDuplicaOption.Senders),
            GetCheck(Check, ProtocolDuplicaOption.Recipients),
            GetCheck(Check, ProtocolDuplicaOption.Object),
            GetCheck(Check, ProtocolDuplicaOption.Category),
            GetCheck(Check, ProtocolDuplicaOption.Other),
            GetCheck(Check, ProtocolDuplicaOption.DocType),
            GetCheck(Check, ProtocolDuplicaOption.Roles))
        vProtocol.ProtocolObject = StringHelper.Clean(vProtocol.ProtocolObject, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
        BindForm(vProtocol)
        Return hasDisabledElements
    End Function

    Private Sub RefreshPackage()
        If Not NeedPackage OrElse rblTipoProtocollo.SelectedValue <> -1 Then
            pnlPackage.Visible = False
            ViewState("totIncr") = Nothing
            ViewState("maxDocs") = Nothing
            EnableInsertButton(True)
            Exit Sub
        End If

        Dim packageList As IList(Of Package) = Facade.PackageFacade.GetPackageDocumentByUser(ViewState("CommonUser"))
        If packageList.IsNullOrEmpty() Then
            AjaxAlert("Errore{0}Non sono presenti scatoloni aperti per l'utente connesso.", Environment.NewLine)
            btnNewPackage.Enabled = False
            btnNewLot.Enabled = False
            EnableInsertButton(False)
            Exit Sub
        ElseIf packageList.Count > 1 Then
            AjaxAlert("Errore{0}E' presente più di uno scatolone aperto per l'utente connesso.", Environment.NewLine)
            btnNewPackage.Enabled = False
            btnNewLot.Enabled = False
            EnableInsertButton(False)
            Exit Sub
        End If

        pnlPackage.Visible = True
        btnNewLot.Attributes.Add("onclick", "return Confirm('Conferma cambio Lotto?','UpdatePackageLot');")
        btnNewPackage.Attributes.Add("onclick", "return Confirm('Conferma cambio Scatolone?','UpdatePackage');")

        Dim package As Package = packageList(0)

        ' originariamente i due incrementi erano all'interno della query
        Dim incremental As Integer = package.Incremental + 1
        Dim totalIncremental As Integer = package.TotalIncremental + 1
        Dim maxDocs As Integer = package.MaxDocuments

        If totalIncremental > maxDocs Then
            AjaxAlert("E' stato superato il numero massimo consentito per questo scatolone, è necessario aprire un altro scatolone.")
            btnNewLot.Enabled = False
            EnableInsertButton(False)
        End If

        btnNewPackage.Enabled = Facade.PackageFacade.VerifyPackage(package.Package, ViewState("CommonUser")) <> 0

        txtOrigin.Text = package.Origin
        txtIncremental.Text = incremental.ToString("00000")
        txtPackage.Text = String.Format("{0:00000}", package.Package)
        txtLot.Text = String.Format("{0:000}", package.Lot)

        ViewState("totIncr") = totalIncremental
        ViewState("maxDocs") = maxDocs
        EnableInsertButton(True)
    End Sub

    Private Sub UpdatePackageLot()
        Facade.PackageFacade.ChangeLot(Integer.Parse(txtPackage.Text), txtOrigin.Text)
        RefreshPackage()
    End Sub

    Private Sub UpdatePackage()
        Dim actualPackage As Integer = Integer.Parse(txtPackage.Text)
        Dim nextPackage As Integer = Facade.PackageFacade.VerifyPackage(actualPackage, DocSuiteContext.Current.User.UserName)

        Select Case nextPackage
            Case -1 'non esiste lo scatolone attuale
                AjaxAlert("Errore.{0}Non esiste più lo scatolone attuale.", Environment.NewLine)
                btnNewPackage.Enabled = False
                btnNewLot.Enabled = False
                EnableInsertButton(False)
                Exit Sub
            Case 0  'non è disponibile il prossimo scatolone
                AjaxAlert("Errore.{0}Non ci sono scatoloni diponibili per l'utente corrente.", Environment.NewLine)
                btnNewPackage.Enabled = False
                btnNewLot.Enabled = False
                EnableInsertButton(False)
                Exit Sub
            Case Else
                Facade.PackageFacade.ChangePackage(actualPackage, nextPackage, txtOrigin.Text(0))
                EnableInsertButton(True)
        End Select

        RefreshPackage()
    End Sub

    Private Function CheckDataForInsert() As Boolean

        If Action.Eq("Recovery") And txtProtRecovery.Text.Length = 0 Then
            ' Se manca il protocollo di recupero faccio aprire la finestra
            AjaxManager.ResponseScripts.Add("setTimeout('btnSelectProtocolV()', 500);")
            Return False
        End If

        'verifica collaborazione se già protocollato
        If Action.Eq("FromCollaboration") Then
            Dim coll As Collaboration = Facade.CollaborationFacade.GetById(Request.QueryString("IdCollaboration"))
            If coll IsNot Nothing Then
                'forzatura per valori a 0
                If (coll.Year.GetValueOrDefault(0) <> 0) And (coll.Number.GetValueOrDefault(0) <> 0) Then
                    AjaxAlert("Il documento della Collaborazione è già stato Protocollato. Numero di Protocollo " & coll.ProtocolString)
                    Return False
                End If
            End If
        End If

        ' Verifica data protocollo mittente
        If GetSelectedProtocolTypeId().HasThisValue(-1) Then
            If txtDocumentDate.SelectedDate.GetValueOrDefault(DateTime.Today) > DateTime.Today Then
                AjaxAlert("Impossibile salvare. La data del protocollo mittente deve essere antecedente a quella odierna.")
                Return False
            End If
        End If

        'Verifica oggetto
        If uscOggetto.Text.Length > uscOggetto.MaxLength Then
            AjaxAlert("Impossibile salvare. Il campo Oggetto ha superato i caratteri disponibili. (Caratteri {0} Disponibili {1})", uscOggetto.Text.Length, uscOggetto.MaxLength)
            Return False
        End If

        If ProtocolEnv.ObjectMinLength <> 0 AndAlso uscOggetto.Text.Length < ProtocolEnv.ObjectMinLength Then
            AjaxAlert("Impossibile salvare. Il campo Oggetto non ha raggiunto i caratteri minimi ({0} caratteri).", ProtocolEnv.EnvObjectMinLength)
            Return False
        End If

        Dim PartialObject As String = StringHelper.Clean(uscOggetto.Text, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
        If Not (String.Equals(uscOggetto.Text, PartialObject)) Then
            uscOggetto.Text = PartialObject
            AjaxAlert("L'oggetto è stato ripulito in automatico dai caratteri speciali. Verificare che l'oggetto risultante sia valido")
            Return False
        End If

        If CurrentContainerControl.SelectedContainer("ProtDB") Is Nothing Then
            AjaxAlert("Impossibile salvare. Nessun Contenitore Selezionato.")
            Return False
        End If

        If CurrentContainerControl.SelectedContainer("ProtDB").ProtLocation Is Nothing Then
            AjaxAlert("Impossibile salvare.{0}La LOCATION del protocollo per il Contenitore {1} non è stata definita.",
                      Environment.NewLine,
                      CurrentContainerControl.SelectedContainer("ProtDB").Name)
            Return False
        End If

        If ProtocolEnv.EnvTableDocTypeRequired AndAlso (SelectedDocumentType Is Nothing) Then
            AjaxAlert("Impossibile salvare. Il Campo Tipologia spedizione non è valorizzato o non è numerico. Valore Attuale: " & cboIdDocType.SelectedValue)
            Return False
        End If

        ' 03/10/2011 Rocca
        If Not ProtocolEnv.IsInteropEnabled AndAlso Action.Eq("Interop") Then
            AjaxAlert("Impossibile salvare. Interoperabilità non abilitata")
            Return False
        End If

        If GetSelectedProtocolType() Is Nothing Then
            AjaxAlert("Impossibile salvare. Il Campo TIPO PROTOCOLLO non è valorizzato o non è numerico. Valore Attuale: " & rblTipoProtocollo.SelectedValue)
            Return False
        End If


        If ProtocolEnv.AuthorizContainer <> 0 Then

            If CurrentContainerControl.SelectedValue.Equals(ProtocolEnv.AuthorizContainer.ToString()) Then
                ' il contenitore è Autorizzazioni
                If Not cboProtocolStatus.SelectedValue.Eq("A") Then
                    ' errore se diverso da Assegnato
                    AjaxAlert("Impossibile salvare. Modificare il contenitore o lo stato del protocollo ad Assegnato")
                    Return False
                End If
            Else
                ' il contenitore è diverso da Autorizzazioni
                If cboProtocolStatus.SelectedValue.Eq("A") Then
                    ' errore se Assegnato
                    AjaxAlert("Impossibile salvare. Modificare il contenitore o lo stato del protocollo ad un valore diverso da Assegnato")
                    Return False
                End If
            End If
        End If

        Return True

    End Function

    Private Function CheckScatolone() As Boolean
        'Verifica incrementale scatolone
        If Not ProtocolEnv.IsPackageEnabled Or Not NeedPackage Then
            Return True
        End If

        If Not pnlPackage.Visible Then
            Return True
        End If

        Dim maxDocs As Integer = Convert.ToInt32(ViewState("maxDocs"))
        Dim totalIncremental As Integer = Convert.ToInt32(ViewState("totIncr"))
        If totalIncremental > maxDocs Then
            AjaxAlert("È stato superato il numero massimo consentito per questo scatolone, è necessario aprire un altro scatolone.")
            btnNewLot.Enabled = False
            EnableInsertButton(False)
            Return False
        End If
        'controllo se l'utente ha già inserito il protocollo nello scatolone da un' altra postazione (il campo package identifica univocamente uno scatolone, un utente può avere un solo scatolone aperto per volta)
        If Facade.PackageFacade.CheckPackageExistence(Me.txtOrigin.Text, Int32.Parse(txtPackage.Text),
                                                      Int32.Parse(txtLot.Text),
                                                      Int32.Parse(txtIncremental.Text)) Then
            AjaxAlert("Questo progressivo dello scatolone è già stato utilizzato .Riprovare la protocollazione.")
            txtIncremental.Text = (Convert.ToInt32(txtIncremental.Text.TrimStart("0"c)) + 1).ToString("00000")
            ViewState("totIncr") = totalIncremental + 1
            Return False
        End If

        Return True
    End Function

    Private Function GetWorkingProtocol() As Protocol
        Dim protocol As Protocol
        Select Case Action
            Case "Recovery"
                Dim vRecovery As IList(Of String) = StringHelper.ConvertStringToList(Of String)(txtProtRecovery.Text, "|"c)
                If vRecovery.Count < 2 Then
                    Throw New DocSuiteException("Recupero protocollo", String.Format("Errore in Lettura Protocollo per il Recupero: [{0}].", txtProtRecovery.Text))
                End If
                protocol = Facade.ProtocolFacade.GetById(Short.Parse(vRecovery(0)), Integer.Parse(vRecovery(1)))
            Case "Recover"

                Dim year As Short = Request.QueryString.GetValue(Of Short)("Year")
                Dim number As Integer = Request.QueryString.GetValue(Of Integer)("Number")
                protocol = Facade.ProtocolFacade.RecoverProtocol(New YearNumberCompositeKey(year, number))
            Case Else
                If Action = "InsertMail" OrElse Action = "Interop" Then
                    If Not CurrentMail Is Nothing AndAlso CurrentMail.HasDocumentUnit() Then
                        Throw New DocSuiteException("Creazione nuovo protocollo", String.Format("La PEC con oggetto [{0}] è già stata archiviata con numero [{1}/{2:0000000}].", CurrentMail.MailSubject, CurrentMail.Year, CurrentMail.Number))
                    End If
                End If
                Try
                    protocol = Facade.ProtocolFacade.CreateProtocol()
                Catch ex As Exception
                    Throw New DocSuiteException("Creazione nuovo protocollo", "Il Server non ha assegnato correttamente il numero di Protocollo progressivo.", ex)
                End Try
        End Select

        protocol.IdStatus = ProtocolStatusId.Errato
        If Not Action.Eq("Recovery") Then
            ' In caso di recupero da sospensione non devo azzerare le catene
            protocol.IdDocument = 0
            protocol.IdAttachments = 0
        End If

        Return protocol
    End Function

    Private Sub BindProtocolFromPage(protocol As Protocol)
        If ProtocolEnv.IsClaimEnabled Then
            protocol.IsClaim = cbClaim.Checked
        End If

        'contenitore
        protocol.Container = CurrentContainerControl.SelectedContainer("ProtDB")
        protocol.Location = protocol.Container.ProtLocation
        'TODO: Verificare il caso di SelectedContainer.ProtAttachLocation non valorizzato
        protocol.AttachLocation = protocol.Container.ProtAttachLocation

        'classificatore
        If Not uscClassificatori.HasSelectedCategories Then
            Throw New DocSuiteException("Errore caricamento", "Nessun classificatore trovato.")
        End If

        protocol.Category = uscClassificatori.SelectedCategories.First()

        'tipo documento
        If ProtocolEnv.IsTableDocTypeEnabled Then
            protocol.DocumentType = SelectedDocumentType
        End If

        protocol.Type = GetSelectedProtocolType()

        'modello di Protocollo
        If ProtocolEnv.ProtocolKindEnabled Then
            protocol.IdProtocolKind = GetSelectedProtocolKindId()
        End If

        If ProtocolEnv.IsStatusEnabled AndAlso Not String.IsNullOrEmpty(cboProtocolStatus.SelectedValue) Then
            protocol.Status = Facade.ProtocolStatusFacade.GetById(cboProtocolStatus.SelectedValue)
        End If

        protocol.ProtocolObject = StringHelper.Clean(uscOggetto.Text, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
        protocol.DocumentDate = txtDocumentDate.SelectedDate
        protocol.DocumentProtocol = txtDocumentProtocol.Text
        If uscUploadDocumenti.DocumentInfosAdded.Count > 0 Then
            protocol.DocumentCode = uscUploadDocumenti.DocumentInfosAdded.First().Name
        End If

        'Note
        protocol.Note = If(String.IsNullOrEmpty(txtNote.Text), Nothing, StringHelper.Clean(txtNote.Text, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString))
        'Assegnatari/Proponente
        protocol.Subject = If(String.IsNullOrEmpty(uscContactAssegnatario.GetContactText()), Nothing, uscContactAssegnatario.GetContactText())
        'Categoria di servizio    
        protocol.ServiceCategory = If(String.IsNullOrEmpty(SelServiceCategory.CategoryText), Nothing, SelServiceCategory.CategoryText)
    End Sub

    Private Function ValidateInvoiceFromPage(containerId As Integer, protocolTypeId As Integer, ByRef vAccountingSectional As String, ByRef accountingYear As Short?, ByRef accountingNumber As Integer?, ByRef accountingSectionalNumber As Integer?) As Boolean
        If Not ProtocolEnv.IsInvoiceEnabled Then
            Return True
        End If

        Dim containerExtFacade As ContainerExtensionFacade = New ContainerExtensionFacade("ProtDB")
        If Not pnlInvoice.Visible Then
            Return True
        End If

        vAccountingSectional = String.Empty
        If ddlAccountingSectional.Visible Then
            vAccountingSectional = ddlAccountingSectional.SelectedItem.Text
        End If

        accountingYear = Nothing
        Dim containerExtensionsTest As IList(Of ContainerExtension) = containerExtFacade.GetByContainerAndKey(containerId, ContainerExtensionType.TP)
        Select Case containerExtensionsTest(0).KeyValue
            Case "A"
                accountingYear = If(dtpInvoiceDate.SelectedDate.HasValue, Convert.ToInt16(dtpInvoiceDate.SelectedDate.Value.Year), Nothing)
                txtAccountingNumber.Text = txtInvoiceNumber.Text
            Case "P"
                accountingYear = If(rdpAccountingDate.SelectedDate.HasValue, Convert.ToInt16(rdpAccountingDate.SelectedDate.Value.Year), Nothing)
        End Select

        Dim invoices As IList(Of Protocol.AdvancedProtocol) = Facade.ProtocolFacade.SerachInvoiceAccountingDouble(containerId, vAccountingSectional, accountingYear.Value, Integer.Parse(txtAccountingNumber.Text))
        If invoices.Count > 0 Then
            AjaxAlert(String.Concat("Documento già Inserito ", ProtocolFacade.ProtocolFullNumber(invoices(0).Year, invoices(0).Number)))
            Return False
        End If

        Dim bTest As Boolean = False
        Dim sTest As String = String.Empty
        Dim contactList As IList(Of ContactDTO) = Nothing

        Select Case protocolTypeId
            Case -1
                sTest = "Mittente"
                contactList = uscMittenti.GetContacts(False)
                If (contactList.Count > 0) Then
                    bTest = True
                End If
            Case 1
                sTest = "Destinatario"
                contactList = uscDestinatari.GetContacts(False)
                If (contactList.Count > 0) Then
                    bTest = True
                End If
        End Select

        If Not bTest Then
            FileLogger.Warn(LoggerName, String.Concat("Specificare un ", sTest, " valido"))
            AjaxAlert(String.Concat("Specificare un ", sTest, " valido"))
            Return False
        End If

        If contactList.Count > 1 Then
            FileLogger.Warn(LoggerName, String.Concat("Specificare un unico ", sTest))
            AjaxAlert(String.Concat("Specificare un unico ", sTest))
            Return False
        End If

        'verifica l'esistenza della partita IVA
        If String.IsNullOrEmpty(contactList(0).Contact.FiscalCode) Then
            FileLogger.Warn(LoggerName, String.Concat("Il ", sTest, " inserito non possiede Codice Fiscale/Partita IVA"))
            AjaxAlert(String.Format("Il {0} inserito non possiede Codice Fiscale/Partita IVA", sTest))
            Return False
        End If

        accountingNumber = Nothing
        If Not String.IsNullOrEmpty(txtAccountingNumber.Text) Then
            accountingNumber = Integer.Parse(txtAccountingNumber.Text)
        End If

        accountingSectionalNumber = Nothing
        Dim vAccountingSectionalVal As String = vAccountingSectional
        If Not String.IsNullOrEmpty(vAccountingSectional) Then
            Dim containersExtension As IList(Of ContainerExtension) = containerExtFacade.GetByContainerAndKey(containerId, ContainerExtensionType.SC)
            If containersExtension.Any() Then
                accountingSectionalNumber = containersExtension.Where(Function(w) w.KeyValue.Eq(vAccountingSectionalVal)).Select(Function(s) s.AccountingSectionalNumber).Single()
            End If
        End If
        Return True
    End Function

    Private Sub UpdateTipoProtocollo(Optional needBindContainer As Boolean = False, Optional needSwap As Boolean = True)
        Select Case rblTipoProtocollo.SelectedValue
            Case -1 ' Ingresso
                lblAssegnatario.Text = "Assegnatario:"
                lblProtocolloMittente.Text = "Protocollo del Mittente"
                PanelProtocollo.Visible = True
                rblTipoProtocollo.Width = Unit.Pixel(5)

                uscMittenti.IsRequired = True
                uscMittenti.APIDefaultProvider = False
                uscMittenti.Enable()

                uscDestinatari.IsRequired = False
                uscDestinatari.APIDefaultProvider = True
                uscDestinatari.Disable()

                If ProtocolEnv.InnerContactRoot.HasValue Then
                    uscMittenti.AddExcludeContact(ProtocolEnv.InnerContactRoot.Value)
                    uscMittenti.ContactRoot = Nothing
                    uscDestinatari.RemoveExcludeContact(ProtocolEnv.InnerContactRoot.Value)
                    uscDestinatari.ContactRoot = ProtocolEnv.InnerContactRoot.Value
                End If

                ' Swap dei contatti da destinatari a mittenti
                If needSwap AndAlso uscDestinatari.Count > 0 Then
                    'TODO: uscDestinatari.GetManualContacts() -> uscDestinatari.GetContacts()
                    uscMittenti.DataSource = uscDestinatari.GetManualContacts()
                    uscMittenti.DataBind()
                    uscDestinatari.DataSource.Clear()
                    uscDestinatari.DataBind()
                End If

            Case 1 ' Uscita
                lblAssegnatario.Text = "Proponente:"
                lblProtocolloMittente.Text = ""
                PanelProtocollo.Visible = False

                uscMittenti.IsRequired = ProtocolEnv.IsProtSenderRequired
                uscMittenti.APIDefaultProvider = True
                uscMittenti.Disable()

                uscDestinatari.IsRequired = True
                uscDestinatari.APIDefaultProvider = False
                uscDestinatari.Enable()

                If ProtocolEnv.InnerContactRoot.HasValue Then
                    uscMittenti.RemoveExcludeContact(ProtocolEnv.InnerContactRoot.Value)
                    uscMittenti.ContactRoot = ProtocolEnv.InnerContactRoot.Value
                    uscDestinatari.ContactRoot = Nothing
                    If Not ProtocolEnv.IncludeInnerContactRootToRecipients Then
                        uscDestinatari.AddExcludeContact(ProtocolEnv.InnerContactRoot.Value)
                    End If
                End If

                txtDocumentProtocol.Text = ""
                txtDocumentDate.SelectedDate = Nothing

                ' Swap dei contatti dei mittenti a destinatari
                If needSwap AndAlso uscMittenti.Count > 0 Then
                    uscDestinatari.DataSource = uscMittenti.GetManualContacts()
                    uscDestinatari.DataBind()
                    uscMittenti.DataSource.Clear()
                    uscMittenti.DataBind()
                End If

            Case 0 ' Tra uffici
                lblAssegnatario.Text = "Interno:"
                lblProtocolloMittente.Text = ""
                PanelProtocollo.Visible = False

                uscMittenti.IsRequired = True
                uscMittenti.APIDefaultProvider = True
                uscMittenti.Enable()

                uscDestinatari.IsRequired = True
                uscDestinatari.APIDefaultProvider = True
                uscDestinatari.Enable()

                If ProtocolEnv.InnerContactRoot.HasValue Then
                    uscMittenti.RemoveExcludeContact(ProtocolEnv.InnerContactRoot.Value)
                    uscMittenti.ContactRoot = ProtocolEnv.InnerContactRoot
                    uscDestinatari.RemoveExcludeContact(ProtocolEnv.InnerContactRoot.Value)
                    uscDestinatari.ContactRoot = ProtocolEnv.InnerContactRoot
                End If

                txtDocumentProtocol.Text = ""
                txtDocumentDate.SelectedDate = Nothing
                uscMittenti.Enable()
                uscDestinatari.Enable()

        End Select
        RefreshPackage()

        If (needBindContainer) Then
            BindContainerControl()
        End If
        uscMittenti.UpdateButtons()
        uscDestinatari.UpdateButtons()
    End Sub


    ''' <summary> Imposto le autorizzazioni ai settori </summary>
    ''' <remarks> Deve essere eseguita DOPO aver settato i contatti </remarks>

    Private Sub BindRolesFromPage(ByRef protocol As Protocol)
        If Not ProtocolEnv.IsAuthorizInsertEnabled Then
            Exit Sub
        End If

        Dim roles As IList(Of Role) = uscAutorizzazioni.GetRoles()
        If roles.IsNullOrEmpty() AndAlso (Not DocSuiteContext.Current.SimplifiedPrivacyEnabled OrElse uscAutorizzazioni.GetUsers().IsNullOrEmpty()) Then
            Exit Sub
        End If

        'Aggiungo autorizzazioni utente
        If DocSuiteContext.Current.SimplifiedPrivacyEnabled Then
            Dim users As IDictionary(Of String, String) = uscAutorizzazioni.GetUsers()
            For Each user As KeyValuePair(Of String, String) In users
                protocol.AddUser(user)
            Next

            Facade.ProtocolFacade.Update(protocol)
            For Each user As KeyValuePair(Of String, String) In users
                Facade.ProtocolLogFacade.AddUserAuthorization(protocol, user.Key)
            Next
        End If

        For Each item As Role In roles
            protocol.AddRole(item, DocSuiteContext.Current.User.FullUserName, DateTimeOffset.UtcNow, If(DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled, ProtocolDistributionType.Explicit, Nothing))
            Facade.ProtocolLogFacade.AddRoleAuthorization(protocol, item)
        Next

        'Aggiorno le autorizzazioni privacy
        If DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso protocol.Roles IsNot Nothing AndAlso protocol.Roles.Count > 0 Then
            Dim privacyRoles As IList(Of String) = uscAutorizzazioni.GetPrivacyRoles()
            Dim proles As IList(Of ProtocolRole) = protocol.Roles.Where(Function(x) privacyRoles.Any(Function(p) p.Eq(x.Id.Id.ToString()))).ToList()
            For Each item As ProtocolRole In proles
                item.Type = ProtocolRoleTypes.Privacy
            Next
        End If

        If Not DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled OrElse protocol.Roles.IsNullOrEmpty() Then
            Exit Sub
        End If

        ' Aggiungo le autorizzazioni implicite.
        Dim oldValues As IEnumerable(Of Integer) = uscAutorizzazioni.GetOldValues().Select(Function(value) value.TryConvert(Of Integer)()) ' FG20131115: Mah...
        Facade.ProtocolFacade.SetImplicitProtocolRoles(protocol, oldValues)

        ' Aggiorno i CC
        If CurrentUserDistributionRights Then
            Facade.ProtocolFacade.UpdateRoleAuthorization(protocol, uscAutorizzazioniCc.GetFullIncrementalPathAttribute(True, uscSettori.NodeTypeAttributeValue.Role), True)
        End If

        If CurrentUserDistributionRights AndAlso ProtocolEnv.ProtocolDistributionTypologies.Contains(protocol.Type.Id) Then
            Facade.ProtocolFacade.AddRoleUserAuthorizations(protocol, uscAutorizzazioniCc.GetRoleValues(True, uscSettori.NodeTypeAttributeValue.RoleUser))
        End If


        If DocSuiteContext.Current.ProtocolEnv.IsAuthorizInsertRolesEnabled Then
            ' Propago lo stato di copia conoscenza ai relativi settori abbinati.
            Dim ccRoles As IEnumerable(Of Integer) = protocol.Contacts.Where(Function(c) c.Type.Eq("CC") AndAlso c.Contact.Role IsNot Nothing).Select(Function(c) c.Contact.Role.Id)
            Facade.ProtocolFacade.SetProtocolRoleDistributionType(protocol, ccRoles) ' Recuperare da contatti
        End If

    End Sub


    ''' <summary> Appena dopo la protocollazione si occupa di caricare i dati da inserire negli atti </summary>

    Private Sub UpdateResolutions(ByVal resolutionXml As ResolutionXML, ByVal prot As Protocol)
        If resolutionXml.ResolutionsToUpdate IsNot Nothing Then
            resolutionXml.Load(prot)
            Facade.ResolutionFacade.UpdateFromXml(resolutionXml, prot)
        End If
    End Sub

    Private Function GetSelectedProtocolTypeId() As Integer?
        Dim parsed As Integer
        If Integer.TryParse(rblTipoProtocollo.SelectedValue, parsed) Then
            Return parsed
        End If
        Return Nothing
    End Function

    Private Function GetSelectedProtocolType() As ProtocolType
        Dim selectedId As Integer? = GetSelectedProtocolTypeId()
        If selectedId.HasValue Then
            Return FacadeFactory.Instance.ProtocolTypeFacade.GetById(selectedId.Value)
        End If
        Return Nothing
    End Function

    Private Sub BindOChartForProtocolIn(selectedContainer As Container, Optional needAuthorization As Boolean = True)
        ' Recupero i CONDIVISORI della risorsa contenitore.
        Dim sharers As IEnumerable(Of OChartItem) = CommonShared.EffectiveOChart.Items.FindResourceSharers(selectedContainer)
        If sharers.IsNullOrEmpty() Then
            Return
        End If

        uscDestinatari.DataSource.Clear()
        uscDestinatari.DataBind()
        uscAutorizzazioni.SourceRoles.Clear()
        uscAutorizzazioni.DataBind()

        ' Precompilo i destinatari con i contatti recuperati dai CONDIVISORI della risorsa contenitore.
        Dim itemContacts As IEnumerable(Of OChartItemContact) = sharers.SelectMany(Function(s) s.Contacts)
        If Not itemContacts.IsNullOrEmpty() Then
            Dim contacts As IEnumerable(Of Integer) = itemContacts.Select(Function(c) c.Contact.Id)
            Dim renewedContacts As IEnumerable(Of Contact) = contacts.Select(Function(c) FacadeFactory.Instance.ContactFacade.GetById(c))
            Dim dtos As IEnumerable(Of ContactDTO) = renewedContacts.Select(Function(c) New ContactDTO(c, ContactDTO.ContactType.Address))
            uscDestinatari.DataSource = dtos.ToList()
            uscDestinatari.DataBind()
        End If

        ' Precompilo le autorizzazioni ai settori con i settori recuperati dai CONDIVISORI della risorsa contenitore.
        Dim itemRoles As IEnumerable(Of OChartItemRole) = sharers.SelectMany(Function(r) r.Roles)
        If needAuthorization AndAlso Not itemRoles.IsNullOrEmpty() Then
            Dim roles As ICollection(Of Integer) = itemRoles.Select(Function(r) r.Role.Id).ToList()
            Dim renewedRoles As IEnumerable(Of Role) = FacadeFactory.Instance.RoleFacade.GetByIds(roles)
            uscAutorizzazioni.SourceRoles = renewedRoles.ToList()
            uscAutorizzazioni.DataBind()
        End If

    End Sub

    Private Sub BindOChartForProtocolOut(selectedContainer As Container, Optional needAuthorization As Boolean = True)
        ' Recupero il PROPRIETARIO della risorsa contenitore.
        Dim master As OChartItem = CommonShared.EffectiveOChart.Items.FindResourceMaster(selectedContainer)
        If master Is Nothing Then
            Return
        End If

        uscMittenti.DataSource.Clear()
        uscMittenti.DataBind()

        ' Precompilo i mittenti con i contatti recuperati dal PROPRIETARIO della risorsa contenitore.
        If Not master.Contacts.IsNullOrEmpty() Then
            Dim contacts As IEnumerable(Of Integer) = master.Contacts.Select(Function(c) c.Contact.Id)
            Dim renewedContacts As IEnumerable(Of Contact) = contacts.Select(Function(c) FacadeFactory.Instance.ContactFacade.GetById(c))
            Dim dtos As IEnumerable(Of ContactDTO) = renewedContacts.Select(Function(c) New ContactDTO(c, ContactDTO.ContactType.Address))
            uscMittenti.DataSource = dtos.ToList()
            uscMittenti.DataBind()
        End If
    End Sub

    Private Sub BindOChartForRecipient(fullCode As String)
        If Not DocSuiteContext.Current.ProtocolEnv.OChartProtocolPreloadingEnabled Then
            Return
        End If

        If String.IsNullOrWhiteSpace(fullCode) Then
            Return
        End If

        Dim selectedProtocolTypeId As Integer? = GetSelectedProtocolTypeId()
        If Not selectedProtocolTypeId.HasValue Then
            Return
        End If

        Dim validProtocolTypes As New List(Of Integer) From {-1, 0}
        If Not validProtocolTypes.Contains(selectedProtocolTypeId.Value) Then
            Return
        End If

        Dim item As OChartItem = CommonShared.EffectiveOChart.Items.FindByFullCode(fullCode)
        If item Is Nothing OrElse Not item.HasRoles Then
            Return
        End If

        ' Recupero una distinta dei settori derivati dagli item dei contatti selezionati.
        Dim roleIds As IEnumerable(Of Integer) = item.Roles.Select(Function(r) r.Role.Id).Distinct()
        Dim renewed As IEnumerable(Of Role) = roleIds.Select(Function(i) FacadeFactory.Instance.RoleFacade.GetById(i))

        ' Scremo quelli già autorizzati.
        Dim missing As IEnumerable(Of Role) = renewed.Where(Function(r) Not uscAutorizzazioni.SourceRoles.Any(Function(a) a.Id.Equals(r.Id)))
        If missing.IsNullOrEmpty() Then
            Return
        End If

        uscAutorizzazioni.SourceRoles.AddRange(missing)
        uscAutorizzazioni.DataBind()
    End Sub

    Private Sub BindOChartResources()
        If Not DocSuiteContext.Current.ProtocolEnv.OChartProtocolPreloadingEnabled Then
            Return
        End If

        Dim selectedProtocolTypeId As Integer? = GetSelectedProtocolTypeId()
        If Not selectedProtocolTypeId.HasValue Then
            Return
        End If

        Dim selectedContainer As Container = CurrentContainerControl.SelectedContainer("ProtDB")
        If selectedContainer Is Nothing Then
            Return
        End If

        Select Case selectedProtocolTypeId
            Case -1 ' Ingresso
                BindOChartForProtocolIn(selectedContainer)
            Case 0 ' Ingresso e uscita (tra uffici)
                BindOChartForProtocolIn(selectedContainer, needAuthorization:=False)
                BindOChartForProtocolOut(selectedContainer, needAuthorization:=False)
            Case 1 ' Uscita
                BindOChartForProtocolOut(selectedContainer)
            Case Else
                Return
        End Select
    End Sub

    Private Overloads Sub BindAutorizzazioniCc(ByVal roleUserViewMode As uscSettori.RoleUserViewMode)
        BindAutorizzazioniCc(uscAutorizzazioni.GetRoles(), roleUserViewMode)
    End Sub

    Private Overloads Sub BindAutorizzazioniCc()
        BindAutorizzazioniCc(uscAutorizzazioni.GetRoles(), CType(uscAutorizzazioniCc.CurrentRoleUserViewMode, uscSettori.RoleUserViewMode?))
    End Sub

    Private Overloads Sub BindAutorizzazioniCc(ByVal roles As IEnumerable(Of Role), ByVal roleUserViewMode As uscSettori.RoleUserViewMode?)
        If Not ProtocolEnv.IsDistributionEnabled Then
            Exit Sub
        End If

        uscAutorizzazioniCc.Checkable = True
        uscAutorizzazioniCc.TreeViewControl.CheckBoxes = True
        uscAutorizzazioniCc.Required = False
        uscAutorizzazioniCc.CopiaConoscenzaEnabled = True
        uscAutorizzazioniCc.ViewDistributableManager = CurrentUserDistributionRights AndAlso ProtocolEnv.ProtocolDistributionTypologies.Contains(CType(rblTipoProtocollo.SelectedValue, Integer))
        uscAutorizzazioniCc.CurrentRoleUserViewMode = roleUserViewMode

        uscAutorizzazioniCc.SourceRoles = CType(roles, List(Of Role))
        uscAutorizzazioniCc.DataBindForRoleUser(CType(rblTipoProtocollo.SelectedValue, Integer), CurrentUserDistributionRights)
    End Sub

    Private Sub InitPanelAutorizzazioniCc()
        If ProtocolEnv.IsDistributionEnabled AndAlso CurrentUserDistributionRights Then
            uscAutorizzazioniCc.TableContentControl.SetDisplay(True)
            BindAutorizzazioniCc()
        Else
            uscAutorizzazioniCc.TableContentControl.SetDisplay(False)
        End If
    End Sub

    Private Sub InitPanelProtocolKind()
        If ProtocolEnv.ProtocolKindEnabled Then
            If ddlProtKindList.Items.Count > 1 Then
                pnlProtocolKind.Visible = True
            Else
                pnlProtocolKind.Visible = False
            End If
        Else
            pnlProtocolKind.Visible = False
        End If
    End Sub

    Private Function GetSelectedProtocolKindId() As Short
        Dim kindId As Short
        Short.TryParse(ddlProtKindList.SelectedValue, kindId)
        Return kindId
    End Function

    Private Sub ResetProtocolKind()
        ddlProtKindList.SelectedValue = "0"
        ddlProtKindList.DataBind()
        pnlProtocolKind.Visible = False
    End Sub

    ''' <summary>
    ''' Eseguo il Bind della nuova gestione di Modello Protocollo (Generico per tutti i futuri modelli)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub BindProtocolKind()
        If Not ProtocolEnv.ProtocolKindEnabled Then
            Exit Sub
        End If

        If String.IsNullOrEmpty(CurrentContainerControl.SelectedValue) Then
            ResetProtocolKind()
            Exit Sub
        End If

        Dim idContainerSelected As Integer = CType(CurrentContainerControl.SelectedValue, Integer)
        Dim availableKinds As IList(Of ProtocolKind) = Facade.ProtocolFacade.AvailableProtocolKinds(idContainerSelected)

        ddlProtKindList.Items.Clear()
        'Abilitazione Tipologia di Protocollo
        Dim kinds As Array = [Enum].GetValues(GetType(ProtocolKind))
        If availableKinds IsNot Nothing Then
            For Each kind As ProtocolKind In kinds

                Dim index As Integer = kind
                If Not availableKinds.Any(Function(x) x.Equals(kind)) Then
                    Continue For
                End If

                Dim item As ListItem = New ListItem(kind.GetDescription(), index.ToString())
                ddlProtKindList.Items.Add(item)
            Next
        End If
        ddlProtKindList.DataBind()

        InitPanelProtocolKind()
    End Sub

    Private Sub BindPageFromTemplate(ByVal template As TemplateProtocol)
        If template Is Nothing Then
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(template.ProtocolObject) Then
            uscOggetto.Text = StringHelper.Clean(template.ProtocolObject, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
        End If

        'Note
        If template.TemplateAdvancedProtocol IsNot Nothing AndAlso Not String.IsNullOrEmpty(template.TemplateAdvancedProtocol.Note) Then
            txtNote.Text = template.TemplateAdvancedProtocol.Note
        End If

        'Assegnatari/Proponente
        If template.TemplateAdvancedProtocol IsNot Nothing AndAlso Not String.IsNullOrEmpty(template.TemplateAdvancedProtocol.Subject) Then
            uscContactAssegnatario.DataSource = template.TemplateAdvancedProtocol.Subject
        End If

        'Categoria di servizio
        If template.TemplateAdvancedProtocol IsNot Nothing AndAlso Not String.IsNullOrEmpty(template.TemplateAdvancedProtocol.ServiceCategory) Then
            SelServiceCategory.CategoryText = template.TemplateAdvancedProtocol.ServiceCategory
        End If

        'Tipologia protocollo
        If template.Type IsNot Nothing Then
            rblTipoProtocollo.SelectedValue = template.Type.Id.ToString()
            UpdateTipoProtocollo()
        End If

        'Contenitore
        BindContainerControl()
        If template.Container IsNot Nothing Then
            Dim exist As Boolean = CurrentContainerControl.HasItemWithValue(template.Container.Id.ToString())
            If exist Then
                CurrentContainerControl.SelectedValue = template.Container.Id.ToString()
                ContainerControlSelectionChanged()

                'Accounting sectional
                If template.TemplateAdvancedProtocol IsNot Nothing AndAlso Not String.IsNullOrEmpty(template.TemplateAdvancedProtocol.AccountingSectional) Then
                    ddlAccountingSectional.SelectedValue = template.TemplateAdvancedProtocol.AccountingSectional
                End If

                'ProtocolKind
                ResetProtocolKind()
                If template.IdProtocolKind.HasValue Then
                    Dim match As ListItem = ddlProtKindList.Items.FindByValue(template.IdProtocolKind.ToString())
                    If match IsNot Nothing Then
                        ddlProtKindList.SelectedValue = template.IdProtocolKind.ToString()
                        BindProtocolKind()
                    End If
                End If
            End If
        End If

        'Settori
        If ProtocolEnv.IsAuthorizInsertEnabled Then
            If Not template.Roles.IsNullOrEmpty() Then
                uscAutorizzazioni.SourceRoles.Clear()
                For Each templateProtocolRole As TemplateProtocolRole In template.Roles
                    uscAutorizzazioni.SourceRoles.Add(templateProtocolRole.Role)
                Next
                uscAutorizzazioni.DataBind()
            End If
        End If

        'Classificatore
        If Not template.TemplateAdvancedProtocol IsNot Nothing OrElse template.Category IsNot Nothing Then
            uscClassificatori.Clear()
            Dim categoryToBind As Category = If(template.TemplateAdvancedProtocol IsNot Nothing AndAlso template.TemplateAdvancedProtocol.SubCategory IsNot Nothing, template.TemplateAdvancedProtocol.SubCategory, template.Category)
            If Facade.CategoryFacade.IsCategoryActive(categoryToBind) Then
                uscClassificatori.DataSource.Add(categoryToBind)
            End If
            uscClassificatori.DataBind()
        End If

        If (template.HasContacts) Then
            'Contatti
            Dim dsMittenti As IList(Of ContactDTO) = New List(Of ContactDTO)()
            Dim dsDestinatari As IList(Of ContactDTO) = New List(Of ContactDTO)()

            For Each templateProtocolContact As TemplateProtocolContact In template.Contacts
                Dim dto As New ContactDTO
                dto.Contact = templateProtocolContact.Contact
                dto.IsCopiaConoscenza = templateProtocolContact.Type.Eq("CC")
                dto.Type = ContactDTO.ContactType.Address
                If templateProtocolContact.Id.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                    dsMittenti.Add(dto)
                Else
                    dsDestinatari.Add(dto)
                End If
            Next

            'Contatti manuali
            For Each templateProtocolContactManual As TemplateProtocolContactManual In template.ContactsManual
                Dim dto As New ContactDTO
                dto.Contact = templateProtocolContactManual.Contact
                dto.IsCopiaConoscenza = templateProtocolContactManual.Type.Eq("CC")
                dto.Type = ContactDTO.ContactType.Manual
                If templateProtocolContactManual.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                    dsMittenti.Add(dto)
                Else
                    dsDestinatari.Add(dto)
                End If
            Next

            If (Not dsMittenti.IsNullOrEmpty()) Then
                uscMittenti.DataSource.Clear()
                uscMittenti.DataSource = dsMittenti
                uscMittenti.DataBind()
            End If
            If (Not dsDestinatari.IsNullOrEmpty()) Then
                uscDestinatari.DataSource.Clear()
                uscDestinatari.DataSource = dsDestinatari
                uscDestinatari.DataBind()
            End If

        End If

        'Status
        If ProtocolEnv.IsStatusEnabled Then
            If template.TemplateAdvancedProtocol IsNot Nothing AndAlso template.TemplateAdvancedProtocol.Status IsNot Nothing Then
                cboProtocolStatus.SelectedValue = template.TemplateAdvancedProtocol.Status.Id.ToString()
            End If
        End If

        If ProtocolEnv.IsTableDocTypeEnabled Then
            If template.DocType IsNot Nothing Then
                cboIdDocType.SelectedValue = template.DocType.Id.ToString()
            End If
        End If
    End Sub

    'Aggiunta proprietà workflow
    'todo: da rivedere la logica per una gestione astratta dei salvataggi
    Private Sub AddWorkflowProperties(protocol As Protocol)
        If IsWorkflowOperation AndAlso CurrentIdWorkflowActivity.HasValue Then
            Dim workflowActivityFacade As WorkflowActivityFacade = New WorkflowActivityFacade(DocSuiteContext.Current.User.FullUserName)
            Dim activity As WorkflowActivity = workflowActivityFacade.GetById(CurrentIdWorkflowActivity.Value)
            Dim documentChainInfo As BiblosDocumentInfo = ProtocolFacade.GetDocument(protocol)

            Dim propertyYear As WorkflowProperty = New WorkflowProperty(DocSuiteContext.Current.User.FullUserName) With {
                .Name = WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_YEAR,
                .PropertyType = WorkflowPropertyType.PropertyInt,
                .WorkflowType = WorkflowType.Activity,
                .ValueInt = protocol.Year
            }

            Dim propertyNumber As WorkflowProperty = New WorkflowProperty(DocSuiteContext.Current.User.FullUserName) With {
                .Name = WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_NUMBER,
                .PropertyType = WorkflowPropertyType.PropertyInt,
                .WorkflowType = WorkflowType.Activity,
                .ValueInt = protocol.Number
            }

            Dim propertyDocument As WorkflowProperty = New WorkflowProperty(DocSuiteContext.Current.User.FullUserName) With {
                .Name = WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_CHAINID_MAIN,
                .PropertyType = WorkflowPropertyType.PropertyGuid,
                .WorkflowType = WorkflowType.Activity,
                .ValueGuid = documentChainInfo.ChainId
            }

            If Not String.IsNullOrEmpty(ProtocolEnv.ExternalViewerProtocolLink) Then
                Dim propertyExternalViewer As WorkflowProperty = New WorkflowProperty(DocSuiteContext.Current.User.FullUserName) With {
                    .Name = WorkflowPropertyHelper.DSW_FIELD_EXTERNALVIEWER_URL,
                    .PropertyType = WorkflowPropertyType.PropertyString,
                    .WorkflowType = WorkflowType.Activity,
                    .ValueString = String.Format(ProtocolEnv.ExternalViewerProtocolLink, protocol.Year, protocol.Number)
                }
                activity.WorkflowProperties.Add(propertyExternalViewer)
            End If

            activity.WorkflowProperties.Add(propertyYear)
            activity.WorkflowProperties.Add(propertyNumber)
            activity.WorkflowProperties.Add(propertyDocument)
            activity.Status = WorkflowStatus.Progress
            workflowActivityFacade.Save(activity)
        End If
    End Sub

    Private Sub InitDocumentsPrivacyLevels(showAlert As Boolean)
        Dim minLevel As Integer = 0
        Dim forceValue As Integer? = Nothing
        If Not String.IsNullOrEmpty(CurrentContainerControl.SelectedValue) AndAlso DocSuiteContext.Current.PrivacyLevelsEnabled Then
            Dim container As Container = Facade.ContainerFacade.GetById(CInt(CurrentContainerControl.SelectedValue))
            If (container IsNot Nothing) Then
                uscUploadDocumenti.ButtonPrivacyLevelVisible = container.PrivacyEnabled
                uscUploadAllegati.ButtonPrivacyLevelVisible = container.PrivacyEnabled
                uscUploadAnnexes.ButtonPrivacyLevelVisible = container.PrivacyEnabled
                forceValue = container.PrivacyLevel
                If container.PrivacyEnabled Then
                    forceValue = Nothing
                    Dim docs As List(Of DocumentInfo) = New List(Of DocumentInfo)(uscUploadDocumenti.DocumentInfosAdded)
                    docs.AddRange(uscUploadAllegati.DocumentInfosAdded)
                    docs.AddRange(uscUploadAnnexes.DocumentInfosAdded)

                    If Facade.DocumentFacade.CheckPrivacyLevel(docs, container) Then
                        forceValue = container.PrivacyLevel
                        If showAlert Then
                            AjaxAlert(String.Concat("Attenzione! Il livello di ", PRIVACY_LABEL, " del contenitore scelto è maggiore dei livelli attribuiti ai documenti. Ai documenti con livello di ", PRIVACY_LABEL, " minore, è stato attribuito il livello del contenitore."))
                        End If
                    End If

                    If container IsNot Nothing AndAlso container.PrivacyEnabled Then
                        minLevel = container.PrivacyLevel
                    End If
                End If
            End If
        End If

        uscUploadDocumenti.MinPrivacyLevel = minLevel
        uscUploadAllegati.MinPrivacyLevel = minLevel
        uscUploadAnnexes.MinPrivacyLevel = minLevel
        uscUploadDocumenti.RefreshPrivacyLevelAttributes(minLevel, forceValue)
        uscUploadAllegati.RefreshPrivacyLevelAttributes(minLevel, forceValue)
        uscUploadAnnexes.RefreshPrivacyLevelAttributes(minLevel, forceValue)
    End Sub
#End Region
End Class