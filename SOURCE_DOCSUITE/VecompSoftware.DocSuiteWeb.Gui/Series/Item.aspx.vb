Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.AVCP.Entities
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentSeries
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Resolutions
Imports VecompSoftware.DocSuiteWeb.DTO.DocumentSeries
Imports VecompSoftware.DocSuiteWeb.DTO.Resolutions
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade.Formattables
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Resolutions
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI.Fascicles
Imports VecompSoftware.DocSuiteWeb.Gui.Resl
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses
Imports VecompSoftware.DocSuiteWeb.Model.Workflow
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging
Imports DocumentArchives = VecompSoftware.DocSuiteWeb.Entity.DocumentArchives
Imports FascicleDocumentUnitFacade = VecompSoftware.DocSuiteWeb.Facade.WebAPI.Fascicles.FascicleDocumentUnitFacade
Imports VecompSoftware.Commons.Interfaces.CQRS.Events

Namespace Series
    Public Class Item
        Inherits CommonBasePage

#Region " Fields "

        Private Const OwnerRolesInSessionName As String = "Series.OwnerRoles"
        Private Const categoryTag As String = "#uscClassificatori"
        Private _selectedDocumentSeriesSubsection As DocumentSeriesSubsection
        Private _currentSeriesItem As DocumentSeriesItem
        Private _currentOwnerRole As Role
        Private _previousPage As String
        Private _action As DocumentSeriesAction
        Private _currentDocumentSeriesItemRights As DocumentSeriesItemRights
        Private _myRoles As IList(Of Role)
        Private _availableContainers As IEnumerable(Of Container)
        Private _hasCollaborationSource As Boolean?
        Private Const COLLABORATION_PATH_FORMAT As String = "~/User/UserCollGestione.aspx?Type={0}&Titolo=Visualizzazione&Action={1}&idCollaboration={2}&Action2={3}&Title2=Visualizzazione"
        Private _currentAVCPFacade As AVCP.AVCPFacade
        Private _isBandiDiGaraSeries As Boolean?
        Private _isAVCPSeries As Boolean?
        Private _resolutionKindFacade As ResolutionKindFacade
        Private _hasRelationAVCPSeries As Boolean?
        Private _currentDocumentSeriesAVCPItem As DocumentSeriesItem
        Private _currentResolutionId As Integer?
        Private _resolutionKindDocumentSeries As ResolutionKindDocumentSeriesFacade
        Private _currentFascicleDocumentUnitFacade As FascicleDocumentUnitFacade
        Private _currentProtocol As Protocol
        Private _selectedDocumentSeriesId As Integer?
        Private _documentUnitToDelete As IList(Of DocumentUnitModel)
        Private _currentDocumentSeriesConstraintFinder As DocumentSeriesConstraintFinder
        Private _currentResolutionKindDocumentSeriesFinder As ResolutionKindDocumentSeriesFinder
#End Region

#Region " Properties "
        Private ReadOnly Property CurrentResolutionKindFacade As ResolutionKindFacade
            Get
                If _resolutionKindFacade Is Nothing Then
                    _resolutionKindFacade = New ResolutionKindFacade(DocSuiteContext.Current.User.FullUserName)
                End If
                Return _resolutionKindFacade
            End Get
        End Property

        Private ReadOnly Property CurrentPreviousPage As String
            Get
                If _previousPage Is Nothing Then
                    _previousPage = Request.QueryString.GetValueOrDefault(Of String)("PreviousPage", String.Empty)
                End If
                Return _previousPage
            End Get
        End Property

        Private Property OwnerRolesInSession As List(Of Integer)
            Get
                Return CType(Session(OwnerRolesInSessionName), List(Of Integer))
            End Get
            Set(value As List(Of Integer))
                Session(OwnerRolesInSessionName) = value
            End Set
        End Property

        Public Property DocumentSeriesYearNumber As Integer
            Get
                If Not Session.Item("DocumentSeriesYearNumber") Is Nothing Then
                    Return CType(Session.Item("DocumentSeriesYearNumber"), Integer)
                Else
                    Return Nothing
                End If
            End Get
            Set(value As Integer)
                Session.Item("DocumentSeriesYearNumber") = value
            End Set
        End Property

        Public Property IdDocumentSeries As Integer
            Get
                If Not Session.Item("IdDocumentSeries") Is Nothing Then
                    Return CType(Session.Item("IdDocumentSeries"), Integer)
                Else
                    Return Nothing
                End If
            End Get
            Set(value As Integer)
                Session.Item("IdDocumentSeries") = value
            End Set
        End Property

        Private ReadOnly Property ActionNewItem() As Boolean
            Get
                Select Case Action
                    Case DocumentSeriesAction.Insert,
                         DocumentSeriesAction.FromResolution,
                         DocumentSeriesAction.Duplicate,
                         DocumentSeriesAction.FromCollaboration,
                         DocumentSeriesAction.FromResolutionKind,
                         DocumentSeriesAction.FromResolutionKindUpdate,
                         DocumentSeriesAction.FromResolutionView,
                         DocumentSeriesAction.FromProtocol
                        Return True

                    Case Else
                        Return False

                End Select
            End Get
        End Property

        Private ReadOnly Property Action() As DocumentSeriesAction
            Get
                Dim temp As String = Request.QueryString.GetValueOrDefault(Of String)("Action", DocumentSeriesAction.Insert.ToString())
                _action = CType([Enum].Parse(GetType(DocumentSeriesAction), temp), DocumentSeriesAction)

                If _action = DocumentSeriesAction.FromResolution AndAlso Not DocSuiteContext.Current.IsResolutionEnabled Then
                    Throw New InvalidOperationException("Modulo Resolution non abilitato.")
                End If
                Return _action
            End Get
        End Property

        Private ReadOnly Property Editable As Boolean
            Get
                Return ActionNewItem OrElse Action = DocumentSeriesAction.Edit
            End Get
        End Property

        Private ReadOnly Property SelectedDocumentSeriesSubsection As DocumentSeriesSubsection
            Get
                If _selectedDocumentSeriesSubsection Is Nothing AndAlso Not String.IsNullOrEmpty(ddlSubsection.SelectedValue) Then
                    _selectedDocumentSeriesSubsection = Facade.DocumentSeriesSubsectionFacade.GetById(CType(ddlSubsection.SelectedValue, Integer))
                End If
                Return _selectedDocumentSeriesSubsection
            End Get
        End Property

        Private ReadOnly Property SelectedArchiveInfo As ArchiveInfo
            Get
                If CurrentDocumentSeriesItem.DocumentSeries IsNot Nothing Then
                    Return DocumentSeriesFacade.GetArchiveInfo(CurrentDocumentSeriesItem.DocumentSeries)
                End If
                Return Nothing
            End Get
        End Property

        Protected ReadOnly Property CurrentDocumentSeriesItem As DocumentSeriesItem
            Get
                If ActionNewItem Then
                    If _currentSeriesItem Is Nothing Then
                        _currentSeriesItem = New DocumentSeriesItem()
                    End If
                Else
                    Dim idDocumentSeriesItem As Integer = Request.QueryString.GetValueOrDefault(Of Integer)("IdDocumentSeriesItem", 0)
                    Dim uniqueId As Guid? = Request.QueryString.GetValueOrDefault(Of Guid?)("UniqueId", Nothing)
                    If _currentSeriesItem Is Nothing Then
                        If idDocumentSeriesItem > 0 Then
                            _currentSeriesItem = Facade.DocumentSeriesItemFacade.GetById(idDocumentSeriesItem)

                        ElseIf uniqueId.HasValue Then
                            _currentSeriesItem = Facade.DocumentSeriesItemFacade.GetByUniqueId(uniqueId.Value)
                        End If
                    End If
                End If
                Return _currentSeriesItem
            End Get
        End Property

        Protected ReadOnly Property CurrentOwnerRole As Role
            Get
                If _currentOwnerRole Is Nothing AndAlso CurrentDocumentSeriesItem IsNot Nothing AndAlso CurrentDocumentSeriesItem.DocumentSeriesItemRoles.Where(Function(f) f.LinkType = DocumentSeriesItemRoleLinkType.Owner).Count() = 1 Then
                    _currentOwnerRole = CurrentDocumentSeriesItem.DocumentSeriesItemRoles.FirstOrDefault(Function(f) f.LinkType = DocumentSeriesItemRoleLinkType.Owner).Role
                End If
                Return _currentOwnerRole
            End Get
        End Property

        Public ReadOnly Property DocSeriesLocation As String
            Get
                If CurrentDocumentSeriesItem.Location Is Nothing Then
                    Return String.Empty
                End If
                Return CurrentDocumentSeriesItem.Location.ProtBiblosDSDB
            End Get

        End Property

        Public Overloads ReadOnly Property CurrentDocumentSeriesItemRights As DocumentSeriesItemRights
            Get
                If _currentDocumentSeriesItemRights Is Nothing Then
                    _currentDocumentSeriesItemRights = New DocumentSeriesItemRights(CurrentDocumentSeriesItem)
                End If
                Return _currentDocumentSeriesItemRights
            End Get
        End Property

        Private ReadOnly Property CurrentResolution() As Resolution
            Get
                If ViewState("currentResolution") Is Nothing Then
                    If (Action = DocumentSeriesAction.FromResolution OrElse Action = DocumentSeriesAction.FromResolutionKindUpdate) AndAlso Page.PreviousPage IsNot Nothing AndAlso (TypeOf Page.PreviousPage Is ToSeries) Then

                        Dim prevPage As ToSeries = DirectCast(Page.PreviousPage, ToSeries)
                        ViewState("currentResolution") = prevPage.CurrentResolution
                    Else
                        If CurrentResolutionId.HasValue Then
                            ViewState("currentResolution") = Facade.ResolutionFacade.GetById(CurrentResolutionId.Value)
                        End If
                    End If
                End If
                Return DirectCast(ViewState("currentResolution"), Resolution)
            End Get
        End Property
        Private Property CurrentCollaboration As Collaboration
            Get
                If ViewState("currentCollaboration") Is Nothing Then
                    If Action = DocumentSeriesAction.FromCollaboration AndAlso Page.PreviousPage IsNot Nothing AndAlso (TypeOf Page.PreviousPage Is CollaborationToSeries) Then
                        Dim prevPage As CollaborationToSeries = DirectCast(Page.PreviousPage, CollaborationToSeries)
                        ViewState("currentCollaboration") = prevPage.CurrentCollaboration
                    End If
                End If
                Return DirectCast(ViewState("currentCollaboration"), Collaboration)
            End Get
            Set(value As Collaboration)
                ViewState("currentCollaboration") = value
            End Set
        End Property

        Private ReadOnly Property MyRoles As IList(Of Role)
            Get
                If _myRoles Is Nothing Then
                    _myRoles = Facade.RoleFacade.GetUserRoles(DSWEnvironment.DocumentSeries, 1, Nothing, CurrentTenant.TenantAOO.UniqueId)
                End If
                Return _myRoles
            End Get
        End Property

        ''' <summary> Contenitori su cui l'operatore ha diritti di inserimento </summary>
        ''' <remarks> Solo in caso di Insert, FromResolution, FromCollaboration e FromResolutionKind </remarks>
        Private ReadOnly Property AvailableContainer() As IEnumerable(Of Container)
            Get
                If _availableContainers Is Nothing Then
                    _availableContainers = Facade.ContainerFacade.GetContainers(DSWEnvironment.DocumentSeries, New List(Of Integer)({DocumentSeriesContainerRightPositions.Insert, DocumentSeriesContainerRightPositions.Draft}), True)
                End If
                Return _availableContainers
            End Get
        End Property
        Private ReadOnly Property HasCollaborationSource As Boolean
            Get
                If Not _hasCollaborationSource.HasValue Then
                    Dim coll As Collaboration = Facade.CollaborationFacade.GetByIdDocumentSeriesItem(CurrentDocumentSeriesItem.Id)
                    _hasCollaborationSource = False
                    If coll IsNot Nothing Then
                        _hasCollaborationSource = True
                    End If
                End If
                Return _hasCollaborationSource.Value
            End Get
        End Property

        Private Property CurrentResolutionModel As ResolutionInsertModel
            Get
                If Session("CurrentResolutionModel") IsNot Nothing Then
                    Return DirectCast(Session("CurrentResolutionModel"), ResolutionInsertModel)
                End If
                Return Nothing
            End Get
            Set(value As ResolutionInsertModel)
                If value Is Nothing Then
                    Session.Remove("CurrentResolutionModel")
                Else
                    Session("CurrentResolutionModel") = value
                End If
            End Set
        End Property

        'Contiene la serie documentale associata alla proposta di Atto che si sta inserendo.
        'Utilizzato con parametro ResolutionKindEnabled
        'Dizionario contenente ID della serie documentale e ID della Bozza creata
        Public Property DraftSeriesItemAdded As IList(Of ResolutionSeriesDraftInsert)
            Get
                If Session("DraftSeriesItemAdded") IsNot Nothing Then
                    Return DirectCast(Session("DraftSeriesItemAdded"), IList(Of ResolutionSeriesDraftInsert))
                End If
                Return Nothing
            End Get
            Set(value As IList(Of ResolutionSeriesDraftInsert))
                If value Is Nothing Then
                    Session.Remove("DraftSeriesItemAdded")
                Else
                    Session("DraftSeriesItemAdded") = value
                End If
            End Set
        End Property

        Private Property CurrentPubblicazione As AVCP.pubblicazione
            Get
                If Session("CurrentAVCP") IsNot Nothing Then
                    Return DirectCast(Session("CurrentAVCP"), AVCP.pubblicazione)
                End If
                Return Nothing
            End Get
            Set(value As AVCP.pubblicazione)
                If value Is Nothing Then
                    Session.Remove("CurrentAVCP")
                Else
                    Session("CurrentAVCP") = value
                End If
            End Set
        End Property

        Private ReadOnly Property CurrentAVCPFacade As AVCP.AVCPFacade
            Get
                If _currentAVCPFacade Is Nothing Then
                    _currentAVCPFacade = New AVCP.AVCPFacade()
                End If
                Return _currentAVCPFacade
            End Get
        End Property

        Private Property CurrentDocumentSeriesModel As DocumentSeriesInsertModel
            Get
                If Session("CurrentDocumentSeriesModel") IsNot Nothing Then
                    Return DirectCast(Session("CurrentDocumentSeriesModel"), DocumentSeriesInsertModel)
                End If
                Return Nothing
            End Get
            Set(value As DocumentSeriesInsertModel)
                If value Is Nothing Then
                    Session.Remove("CurrentDocumentSeriesModel")
                Else
                    Session("CurrentDocumentSeriesModel") = value
                End If
            End Set
        End Property

        Private ReadOnly Property IsBandiDiGaraSeries As Boolean
            Get
                If _isBandiDiGaraSeries Is Nothing Then
                    _isBandiDiGaraSeries = False
                    If CurrentDocumentSeriesItem.DocumentSeries IsNot Nothing Then
                        If DocSuiteContext.Current.ProtocolEnv.BandiGaraDocumentSeriesId.HasValue Then
                            If CurrentDocumentSeriesItem.DocumentSeries.Id = DocSuiteContext.Current.ProtocolEnv.BandiGaraDocumentSeriesId Then
                                _isBandiDiGaraSeries = True
                            End If
                        End If
                    End If
                End If
                Return _isBandiDiGaraSeries.Value
            End Get
        End Property

        Private ReadOnly Property HasRelationAVCPSeries As Boolean
            Get
                If (_hasRelationAVCPSeries.HasValue) Then
                    Return _hasRelationAVCPSeries.Value
                End If

                _hasRelationAVCPSeries = False
                If Session("CurrentResolutionModel") IsNot Nothing Then
                    Dim resolution As ResolutionInsertModel = DirectCast(Session("CurrentResolutionModel"), ResolutionInsertModel)
                    If (resolution.ResolutionKind.HasValue) Then
                        Dim resolutionKind As ResolutionKind = CurrentResolutionKindFacade.GetById(resolution.ResolutionKind.Value)
                        If (resolutionKind IsNot Nothing AndAlso DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId IsNot Nothing) Then
                            _hasRelationAVCPSeries = resolutionKind.ResolutionKindDocumentSeries.Any(Function(f) f.DocumentSeries.Id = DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId)
                        End If
                    End If
                End If
                Return _hasRelationAVCPSeries
            End Get
        End Property


        Private ReadOnly Property IsAVCPSeries As Boolean
            Get
                If _isAVCPSeries Is Nothing Then
                    _isAVCPSeries = False
                    If CurrentDocumentSeriesItem.DocumentSeries IsNot Nothing Then
                        If DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId.HasValue Then
                            If CurrentDocumentSeriesItem.DocumentSeries.Id = DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId Then
                                _isAVCPSeries = True
                            End If
                        End If
                    End If
                End If
                Return _isAVCPSeries.Value
            End Get
        End Property

        Private ReadOnly Property CurrentDocumentSeriesAVCPItem As DocumentSeriesItem
            Get
                If _currentDocumentSeriesAVCPItem Is Nothing Then
                    Dim temp As Integer = Request.QueryString.GetValueOrDefault(Of Integer)("DocumentSeriesAVCPId", -1)
                    If _currentDocumentSeriesAVCPItem Is Nothing AndAlso temp > 0 Then
                        _currentDocumentSeriesAVCPItem = Facade.DocumentSeriesItemFacade.GetById(temp)
                    End If
                End If

                Return _currentDocumentSeriesAVCPItem
            End Get
        End Property

        Private ReadOnly Property CurrentResolutionId As Integer?
            Get
                If Not _currentResolutionId.HasValue Then
                    _currentResolutionId = Request.QueryString.GetValueOrDefault(Of Integer)("IdResolution", -1)
                End If
                Return _currentResolutionId
            End Get
        End Property

        Private ReadOnly Property HasAvcpDraftSeries As Boolean
            Get
                If DraftSeriesItemAdded IsNot Nothing Then
                    Return DraftSeriesItemAdded.Where(Function(x) x.IdSeries = DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId.Value).Any()
                Else
                    Return False
                End If
            End Get
        End Property

        Protected Overridable ReadOnly Property ReslKindDocumentSeriesFacade As ResolutionKindDocumentSeriesFacade
            Get
                If _resolutionKindDocumentSeries Is Nothing Then
                    _resolutionKindDocumentSeries = New ResolutionKindDocumentSeriesFacade(DocSuiteContext.Current.User.FullUserName)
                End If
                Return _resolutionKindDocumentSeries
            End Get
        End Property

        Private ReadOnly Property CurrentFascicleDocumentUnitFacade As FascicleDocumentUnitFacade
            Get
                If _currentFascicleDocumentUnitFacade Is Nothing Then
                    _currentFascicleDocumentUnitFacade = New FascicleDocumentUnitFacade(DocSuiteContext.Current.Tenants, CurrentTenant)
                End If
                Return _currentFascicleDocumentUnitFacade
            End Get
        End Property

        Public ReadOnly Property IdDocumentChain() As Guid?
            Get
                If CurrentDocumentSeriesItem.IdMain.Equals(Guid.Empty) Then
                    Return Nothing
                End If
                Return CurrentDocumentSeriesItem.IdMain
            End Get

        End Property

        Public ReadOnly Property IdAttachmentsChain() As Guid?
            Get
                If CurrentDocumentSeriesItem.IdAnnexed.Equals(Guid.Empty) Then
                    Return Nothing
                End If
                Return CurrentDocumentSeriesItem.IdAnnexed
            End Get
        End Property

        Private ReadOnly Property CurrentProtocol() As Protocol
            Get
                If _currentProtocol Is Nothing AndAlso (Action = DocumentSeriesAction.FromProtocol) Then
                    Dim uniqueIdProtocol As Guid? = Request.QueryString.GetValueOrDefault(Of Guid?)("UniqueIdProtocol", Nothing)
                    If uniqueIdProtocol.HasValue Then
                        _currentProtocol = Facade.ProtocolFacade.GetById(uniqueIdProtocol.Value, False)
                    End If
                End If
                Return _currentProtocol
            End Get
        End Property

        Private ReadOnly Property SelectedDocumentSeriesId() As Integer?
            Get
                If _selectedDocumentSeriesId Is Nothing Then
                    Dim id As String = Request.QueryString.GetValueOrDefault(Of String)("DocumentSeriesId", String.Empty)
                    Dim documentSeriesId As Integer = Nothing
                    If Integer.TryParse(id, documentSeriesId) Then
                        _selectedDocumentSeriesId = documentSeriesId
                    End If
                End If
                Return _selectedDocumentSeriesId
            End Get
        End Property

        Private Property DocumentUnitsToDelete As IList(Of DocumentUnitModel)
            Get
                If Session("DocumentUnitsToDelete") Is Nothing Then
                    Return New List(Of DocumentUnitModel)()
                End If
                Return DirectCast(Session("DocumentUnitsToDelete"), List(Of DocumentUnitModel))
            End Get
            Set(value As IList(Of DocumentUnitModel))
                Session("DocumentUnitsToDelete") = value
            End Set
        End Property

        Private ReadOnly Property CurrentDocumentSeriesConstraintFinder As DocumentSeriesConstraintFinder
            Get
                If _currentDocumentSeriesConstraintFinder Is Nothing Then
                    _currentDocumentSeriesConstraintFinder = New DocumentSeriesConstraintFinder(DocSuiteContext.Current.Tenants)
                End If
                Return _currentDocumentSeriesConstraintFinder
            End Get
        End Property

        Private ReadOnly Property CurrentResolutionKindDocumentSeriesFinder As ResolutionKindDocumentSeriesFinder
            Get
                If _currentResolutionKindDocumentSeriesFinder Is Nothing Then
                    _currentResolutionKindDocumentSeriesFinder = New ResolutionKindDocumentSeriesFinder(DocSuiteContext.Current.Tenants)
                    _currentResolutionKindDocumentSeriesFinder.EnablePaging = False
                End If
                Return _currentResolutionKindDocumentSeriesFinder
            End Get
        End Property
#End Region

#Region "Enums"
        Private Enum TypeSeries
            AVCP
            BandiDiGara
        End Enum
#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            ' Inizializzo le componenti Ajax
            InitializeAjax()

            uscDocumentUnitReferences.Visible = False
            If Action.Equals(DocumentSeriesAction.View) Then
                uscDocumentUnitReferences.Visible = True
                uscDocumentUnitReferences.ReferenceUniqueId = CurrentDocumentSeriesItem.UniqueId.ToString()
            End If

            If Not Page.IsPostBack Then
                DocumentUnitsToDelete = New List(Of DocumentUnitModel)()

                DocumentSeriesSelection.Text = ProtocolEnv.DocumentSeriesName
                lblDocumentSeries.Text = ProtocolEnv.DocumentSeriesName & ":"
                DocumentSeriesTitle.Text = ProtocolEnv.DocumentSeriesName
                lblSeriesLabel.Text = ProtocolEnv.DocumentSeriesName

                trCollaboration.Visible = False
                If ProtocolEnv.IsCollaborationEnabled AndAlso HasCollaborationSource Then
                    trCollaboration.Visible = True
                    Dim coll As Collaboration = Facade.CollaborationFacade.GetByIdDocumentSeriesItem(CurrentDocumentSeriesItem.Id)
                    collaborationLink.Text = coll.CollaborationObject
                    collaborationLink.NavigateUrl = String.Format(COLLABORATION_PATH_FORMAT, CollaborationFacade.GetPageTypeFromDocumentType(coll.DocumentType), CollaborationSubAction.ProtocollatiGestiti, coll.Id, CollaborationMainAction.ProtocollatiGestiti)
                End If

                ValidatorSummary.Enabled = ProtocolEnv.EnableValidatorSummary
                If ActionNewItem Then
                    ' Tipo Archivi
                    Dim results As IEnumerable(Of ContainerArchive) = AvailableContainer.Where(Function(c) c.Archive IsNot Nothing).Select(Function(c) c.Archive).Distinct()
                    ddlContainerArchive.DataValueField = "Id"
                    ddlContainerArchive.DataTextField = "Name"
                    ddlContainerArchive.DataSource = results
                    ddlContainerArchive.DataBind()
                    If Not results.IsNullOrEmpty() AndAlso results.Count = 1 Then
                        ddlContainerArchive.SelectedIndex = 0
                        ddlContainerArchive_SelectedIndexChanged(sender, Nothing)
                    Else
                        ddlContainerArchive.Items.Insert(0, String.Empty)
                    End If

                    ' Serie Documentali
                    LoadAvailableDocumentSeries()

                    ' Caricamento della serie d'appartenenza non legata ad un documentseriesitem
                    ' TODO: forse qui converrebbe preinizializzare un oggetto DocumentSeriesItem per intero
                    Dim selectedContainer As Container = Nothing
                    Select Case Action
                        Case DocumentSeriesAction.Insert
                            ' Se è disponibile un singolo contenitore lo seleziono di default
                            If AvailableContainer.Count = 1 Then
                                selectedContainer = AvailableContainer(0)
                            End If
                        Case DocumentSeriesAction.Duplicate
                            ' brutto, ma prima era semplicemente nascosto
                            selectedContainer = DirectCast(Page.PreviousPage, Duplicate).CurrentDocumentSeriesItem.DocumentSeries.Container
                        Case DocumentSeriesAction.FromResolution
                            ' brutto, ma prima era semplicemente nascosto
                            Dim toSieries As ToSeries = DirectCast(Page.PreviousPage, ToSeries)
                            selectedContainer = Facade.ContainerFacade.GetById(Integer.Parse(toSieries.SelectedDocumentSeriesId))
                        Case DocumentSeriesAction.FromCollaboration
                            selectedContainer = DirectCast(Page.PreviousPage, CollaborationToSeries).CurrentDocumentSeries.Container
                        Case DocumentSeriesAction.FromResolutionKind, DocumentSeriesAction.FromResolutionKindUpdate, DocumentSeriesAction.FromResolutionView
                            Dim series As DocumentSeries = Facade.DocumentSeriesFacade.GetById(GetKeyValue(Of Integer, Item)("IdSeries"))
                            selectedContainer = series.Container
                        Case DocumentSeriesAction.FromProtocol
                            If SelectedDocumentSeriesId IsNot Nothing Then
                                selectedContainer = Facade.ContainerFacade.GetById(SelectedDocumentSeriesId)
                            End If
                    End Select

                    If selectedContainer IsNot Nothing Then
                        ddlDocumentSeries.SelectedValue = selectedContainer.Id.ToString()
                        If selectedContainer.Archive IsNot Nothing Then
                            ddlContainerArchive.SelectedValue = selectedContainer.Archive.Id.ToString()
                        End If
                        CurrentDocumentSeriesItem.DocumentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(selectedContainer)
                    End If
                End If
            Else
                If ActionNewItem Then
                    If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                        InitDocumentsPrivacyLevels(False)
                    End If
                    ' brutto, ma prima era semplicemente nascosto
                    If String.IsNullOrEmpty(ddlDocumentSeries.SelectedValue) Then
                        CurrentDocumentSeriesItem.DocumentSeries = Nothing
                    Else
                        CurrentDocumentSeriesItem.DocumentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(Integer.Parse(ddlDocumentSeries.SelectedValue))
                    End If
                End If
            End If

            ' Ad ogni Ciclo di vita devo ricaricare in Pagina tutti i controlli Dinamici
            If CurrentDocumentSeriesItem.DocumentSeries IsNot Nothing Then
                LoadAttributesControls(SelectedArchiveInfo)
                InitializeBehaviours()
                ' Visualizzo il resto della pagina
                ContentWrapper.SetDisplay(True)
            End If

            Dim dragAndDropEnabled As Boolean = ProtocolEnv.DocumentSeriesReorderDocumentEnabled AndAlso Not Action.Equals(DocumentSeriesAction.View)
            Dim renameEnabled As Boolean = ProtocolEnv.DocumentSeriesRenameDocumentEnabled AndAlso Not Action.Equals(DocumentSeriesAction.View)
            uscUploadDocument.FilenameAutomaticRenameEnabled = ProtocolEnv.IsFilenameAutomaticRenameEnabled
            uscUploadDocument.DocumentsDragAndDropEnabled = dragAndDropEnabled
            uscUploadDocument.DocumentsRenameEnabled = renameEnabled
            uscUploadAnnexed.FilenameAutomaticRenameEnabled = ProtocolEnv.IsFilenameAutomaticRenameEnabled
            uscUploadAnnexed.DocumentsDragAndDropEnabled = dragAndDropEnabled
            uscUploadAnnexed.DocumentsRenameEnabled = renameEnabled
            uscUnpublishedAnnexed.FilenameAutomaticRenameEnabled = ProtocolEnv.IsFilenameAutomaticRenameEnabled
            uscUnpublishedAnnexed.DocumentsDragAndDropEnabled = dragAndDropEnabled
            uscUnpublishedAnnexed.DocumentsRenameEnabled = renameEnabled

            ' Se la serie documentale è di tipo AVCP o Bandi di gara, setto la possibilità di selezionare UN SOLO settore di appartenenza
            If IsBandiDiGaraSeries OrElse IsAVCPSeries Then
                uscRoleOwner.MultiSelect = False
            End If

            uscMulticlassificationRest.IdDocumentUnit = CurrentDocumentSeriesItem.UniqueId.ToString()
            uscMulticlassificationRest.Visible = ProtocolEnv.MulticlassificationEnabled

            If Not Page.IsPostBack Then

                ' Imposto il titolo
                Title = String.Format("{1} - {0}", Action.GetDescription(), ProtocolEnv.DocumentSeriesName)
                InitializeDocumentControls()

                ' Caricamento dei dati
                Select Case Action
                    Case DocumentSeriesAction.Insert
                        FileLogger.Debug(LoggerName, "Inserimento nuova registrazione in Serie Documentale.")
                        InitializeForInsert()
                    Case DocumentSeriesAction.Duplicate
                        InitializeActionDuplicate()
                        InitializeForInsert()
                    Case DocumentSeriesAction.FromResolution
                        InitializeActionFromResolution()
                        InitializeForInsert()
                    Case DocumentSeriesAction.FromCollaboration
                        InitializeActionFromCollaboration()
                        InitializeForInsert()
                    Case DocumentSeriesAction.View
                        InitializeActionView()
                    Case DocumentSeriesAction.Edit
                        InitializeActionEdit()
                    Case DocumentSeriesAction.FromResolutionKind
                        'todo: da qui gestire le serie documentali speciali per i comportamenti corrispondenti
                        InitializeActionFromResolutionKind()
                        InitializeForInsert()
                        InitializeKindConstraint()
                        If Not CurrentDocumentSeriesModel Is Nothing Then
                            BindPageFromModel()
                        End If
                        'Inizializzo lo stile per l'azione specifica
                        DocumentsPanel.Visible = Not CurrentDocumentSeriesItem.DocumentSeries.Id.Equals(ProtocolEnv.AvcpDocumentSeriesId)
                        ddlContainerArchive.Enabled = False
                        ddlDocumentSeries.Enabled = False
                        tblPubblication.Visible = False
                        cmdOk.Visible = False

                        ' Salto alla pagina di AVCPEditor se non trovo almeno una pubblicazione AVCP presente
                        If CurrentPubblicazione Is Nothing AndAlso IsBandiDiGaraSeries AndAlso HasRelationAVCPSeries AndAlso Not HasAvcpDraftSeries Then
                            ' Skip alla pagina successiva
                            SkipAvcpEditorPage()
                        Else
                            ' Pulisco la sessione di AVCP
                            If Session("CurrentAVCP") IsNot Nothing Then
                                Session.Remove("CurrentAVCP")
                            End If
                        End If
                    Case DocumentSeriesAction.FromResolutionKindUpdate, DocumentSeriesAction.FromResolutionView
                        'todo: da qui gestire le serie documentali speciali per i comportamenti corrispondenti
                        InitializeActionFromResolutionKind()
                        InitializeForInsert()
                        InitializeKindConstraint()
                        If Not CurrentDocumentSeriesModel Is Nothing Then
                            BindPageFromModel()
                        End If
                        'Inizializzo lo stile per l'azione specifica
                        DocumentsPanel.Visible = False
                        ddlContainerArchive.Enabled = False
                        ddlDocumentSeries.Enabled = False
                        tblPubblication.Visible = False
                        cmdOk.Visible = False

                        ' Salto alla pagina di AVCPEditor se non trovo almeno una pubblicazione AVCP presente
                        If CurrentPubblicazione Is Nothing AndAlso IsBandiDiGaraSeries AndAlso HasRelationAVCPSeries Then
                            ' Skip alla pagina successiva
                            SkipAvcpEditorPage()
                        Else
                            ' Pulisco la sessione di AVCP
                            If Session("CurrentAVCP") IsNot Nothing Then
                                Session.Remove("CurrentAVCP")
                            End If
                        End If
                    Case DocumentSeriesAction.FromProtocol
                        InitializeActionFromProtocol()
                        InitializeForInsert()
                End Select
            End If

            If Action.Equals(DocumentSeriesAction.Edit) Then
                LoadAssociatedDocumentUnits()
            End If

            If ProtocolEnv.TransparentMonitoringEnabled AndAlso Action.Equals(DocumentSeriesAction.View) AndAlso CurrentOwnerRole IsNot Nothing Then
                uscAmmTraspMonitorLog.OwnerRoleId = CurrentOwnerRole.Id
            End If

            If ProtocolEnv.DocumentSeriesPublicationDateConstraintEnabled AndAlso Action = DocumentSeriesAction.Insert Then
                ItemPublishingDate.MinDate = Date.Today.Date
            End If

            btnNuovoMonitoraggio.OnClientClick = "showWindow()"
        End Sub

        Private Sub btnYearConfirmClick(sender As Object, e As EventArgs) Handles btnYearConfirm.Click

            Dim year As Integer
            If dropYears.SelectedValue.IsNullOrEmpty Then
                year = DateTime.Today.Year
            Else
                year = Convert.ToInt32(dropYears.SelectedValue)
            End If
            DocumentSeriesYearNumber = year


            ScriptManager.RegisterStartupScript(Me, Page.GetType, "closeScript", "clientCloseYearSelectWindow('');", True)

            Dim attributes As AttributeCollection = btnYearConfirm.Attributes()
            If attributes.Item("Insert") IsNot Nothing Then
                sender = cmdOk
            ElseIf attributes.Item("Draft") IsNot Nothing Then
                sender = cmdSaveDraft
            End If

            Call CmdOkClick(sender, e)
        End Sub

        Private Sub showWindow()

            InitializeDropdownYear(IdDocumentSeries)
            ScriptManager.RegisterStartupScript(Me, Page.GetType(), "closeScript", "openYearSelectWindow();", True)

        End Sub

        Private Sub CmdOkClick(sender As Object, e As EventArgs) Handles cmdOk.Click, cmdSaveDraft.Click

            Dim btn As Button = CType(sender, Button)
            If ProtocolEnv.ChangeYearDocumentSeriesEnabled Then
                If DocumentSeriesYearNumber.Equals(0) Then
                    IdDocumentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(Integer.Parse(ddlDocumentSeries.SelectedValue)).Id
                    Dim years As Integer = Facade.DocumentSeriesIncrementalFacade.CountOpenDocumentIncrementalSeries(IdDocumentSeries)
                    If years > 1 And DocumentSeriesYearNumber.Equals(0) Then
                        If btn.CommandArgument.Eq("ACTIVE") Then
                            btnYearConfirm.AddAttribute("Insert", "Insert")
                        ElseIf btn.CommandArgument.Eq("DRAFT") Then
                            btnYearConfirm.AddAttribute("Draft", "Draft")
                        End If
                        showWindow()
                        Exit Sub
                    End If
                End If
            End If

            ' Istanzio la catena documentale con i documenti aggiunti in Catena Principale
            Dim chain As New BiblosChainInfo(uscUploadDocument.DocumentInfosAdded.ToList())
            FillDataFromPage(chain, CurrentDocumentSeriesItem)

            If tblConstraints.Visible AndAlso Not String.IsNullOrEmpty(ddlConstraints.SelectedValue) Then
                CurrentDocumentSeriesItem.ConstraintValue = ddlConstraints.SelectedItem.Text
            End If

            ' Recupero e salvo i dati di classificazione
            Dim selectedCategory As Category = ItemCategory.SelectedCategories.First()
            Dim root As Category = selectedCategory.Root
            If selectedCategory.Equals(root) Then
                CurrentDocumentSeriesItem.Category = selectedCategory
            Else
                CurrentDocumentSeriesItem.Category = root
                CurrentDocumentSeriesItem.SubCategory = selectedCategory
            End If

            ' SUBSECTION
            If CurrentDocumentSeriesItem.DocumentSeries.SubsectionEnabled.GetValueOrDefault(False) Then
                CurrentDocumentSeriesItem.DocumentSeriesSubsection = SelectedDocumentSeriesSubsection
            End If

            ' Recupero lo STATUS dell'Item che deve essere salvato (DRAFT o ACTIVE a seconda del pulsante chiamante)

            Dim status As DocumentSeriesItemStatus = DocumentSeriesItemStatus.Active
            If btn.CommandArgument.Eq("DRAFT") Then
                status = DocumentSeriesItemStatus.Draft
            End If

            CurrentDocumentSeriesItem.Priority = chkPriority.Checked

            ' Salvo l'Item in DB
            Facade.DocumentSeriesItemFacade.SaveDocumentSeriesItem(CurrentDocumentSeriesItem, DocumentSeriesYearNumber, chain, uscUploadAnnexed.DocumentInfosAdded, uscUnpublishedAnnexed.DocumentInfosAdded, status, String.Empty, CurrentCollaboration)

            ' Se l'Item è proveniente da una Protocol ne salvo il collegamento e registro su LOG Applicativo
            If Action = DocumentSeriesAction.FromProtocol Then
                Facade.ProtocolDocumentSeriesItemFacade.LinkProtocolToDocumentSeriesItem(CurrentProtocol, CurrentDocumentSeriesItem)

                Dim message As String = String.Format("Inserimento in {3} {0}: {1}/{2:000000}", CurrentDocumentSeriesItem.DocumentSeries.Container.Name, CurrentDocumentSeriesItem.Year, CurrentDocumentSeriesItem.Number, ProtocolEnv.DocumentSeriesName)
                If CurrentDocumentSeriesItem.Status.Equals(DocumentSeriesItemStatus.Draft) Then
                    message = String.Format("Inserimento in {2} {0}: Bozza N: {1}", CurrentDocumentSeriesItem.DocumentSeries.Container.Name, CurrentDocumentSeriesItem.Id, ProtocolEnv.DocumentSeriesName)
                End If
                ' Aggiungere log in Protocol
                Facade.ProtocolLogFacade.Log(CurrentProtocol, ProtocolLogEvent.SD, message)
            End If

            ' Se l'Item è proveniente da una Resolution ne salvo il collegamento e registro su LOG Applicativo
            If Action = DocumentSeriesAction.FromResolution Then
                ' aggiungere collegamento alla resolution
                Facade.DocumentSeriesItemResolutionLinkFacade.LinkResolutionToDocumentSeriesItem(Facade.ResolutionFacade.GetById(CurrentResolution.Id), CurrentDocumentSeriesItem)

                Facade.ResolutionDocumentSeriesItemFacade.LinkResolutionToDocumentSeriesItem(CurrentResolution, CurrentDocumentSeriesItem)
                ' Aggiungere log in Resolution
                Facade.ResolutionLogFacade.Log(
                    CurrentResolution, ResolutionLogType.SD, String.Format("Inserimento in {3} {0}: {1}/{2:000000}", CurrentDocumentSeriesItem.DocumentSeries.Container.Name, CurrentDocumentSeriesItem.Year, CurrentDocumentSeriesItem.Number, ProtocolEnv.DocumentSeriesName))
            End If

            If Action = DocumentSeriesAction.FromCollaboration Then
                CurrentCollaboration.DocumentSeriesItem = CurrentDocumentSeriesItem

                Facade.CollaborationFacade.Update(CurrentCollaboration, String.Empty, Nothing, String.Empty, Nothing, CollaborationStatusType.PT, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, String.Empty, 0, False)
                Facade.CollaborationFacade.SendMail(CurrentCollaboration, CollaborationMainAction.ProtocollatiGestiti, CurrentTenant.TenantAOO.UniqueId)
            End If

            If Action = DocumentSeriesAction.FromResolutionKind OrElse Action = DocumentSeriesAction.FromResolutionKindUpdate OrElse Action = DocumentSeriesAction.FromResolutionView Then
                If DraftSeriesItemAdded Is Nothing Then
                    DraftSeriesItemAdded = New List(Of ResolutionSeriesDraftInsert)
                End If

                If Not DraftSeriesItemAdded.Where(Function(x) x.IdSeries = CurrentDocumentSeriesItem.DocumentSeries.Id AndAlso x.IdSeriesItem = CurrentDocumentSeriesItem.Id).Any() Then
                    DraftSeriesItemAdded.Add(New ResolutionSeriesDraftInsert() With {
                                         .IdSeries = CurrentDocumentSeriesItem.DocumentSeries.Id,
                                         .IdSeriesItem = CurrentDocumentSeriesItem.Id})
                End If
            End If

            ' ROLES. Da Aggiungere DOPO il salvataggio dell'Item
            If CurrentDocumentSeriesItem.DocumentSeries.RoleEnabled.GetValueOrDefault(False) Then
                ' Owner ADD
                If uscRoleOwner.RoleListAdded IsNot Nothing AndAlso uscRoleOwner.RoleListAdded.Count > 0 Then
                    Dim roles As IList(Of Role) = Facade.RoleFacade.GetByIds(uscRoleOwner.RoleListAdded)
                    For Each role As Role In roles
                        Facade.DocumentSeriesItemRoleFacade.AddOwnerRole(CurrentDocumentSeriesItem, role)
                        If IsBandiDiGaraSeries And HasRelationAVCPSeries Then
                            ' Salvataggio dei Settori di appartenenza nella serie documentale di AVCP collegata alla serie di Bandi di gara
                            Facade.DocumentSeriesItemRoleFacade.AddOwnerRole(CurrentDocumentSeriesAVCPItem, role)
                        End If
                    Next
                    SaveOwnerRolesInSession(roles)
                End If

                ' Authorized ADD
                If uscRoleAuthorization.RoleListAdded IsNot Nothing AndAlso uscRoleAuthorization.RoleListAdded.Count > 0 Then
                    Dim roles As IList(Of Role) = Facade.RoleFacade.GetByIds(uscRoleAuthorization.RoleListAdded)
                    For Each role As Role In roles
                        Facade.DocumentSeriesItemRoleFacade.AddAuthorizedRole(CurrentDocumentSeriesItem, role)

                        If IsBandiDiGaraSeries And HasRelationAVCPSeries Then
                            ' Salvataggio dei Settori di appartenenza nella serie documentale di AVCP collegata alla serie di Bandi di gara
                            Facade.DocumentSeriesItemRoleFacade.AddAuthorizedRole(CurrentDocumentSeriesAVCPItem, role)
                        End If
                    Next
                End If
            End If

            If status = DocumentSeriesItemStatus.Active AndAlso CurrentDocumentSeriesItem.DocumentSeriesItemRoles IsNot Nothing _
                AndAlso CurrentDocumentSeriesItem.DocumentSeriesItemRoles.Count > 0 Then
                Facade.DocumentSeriesItemFacade.SendUpdateDocumentSeriesItemCommand(CurrentDocumentSeriesItem)
            End If

            If Not chkSaveSession.Checked Then
                DocumentSeriesYearNumber = 0
            End If

            Select Case Action
                Case DocumentSeriesAction.FromResolutionKind
                    GoToResolution()
                Case DocumentSeriesAction.FromResolutionKindUpdate
                    GoToResolutionUpdate()
                Case DocumentSeriesAction.FromResolutionView
                    GoToResolutionView()
                Case Else
                    GoToView(CurrentDocumentSeriesItem)
            End Select
        End Sub

        Private Sub SaveOwnerRolesInSession(roles As IList(Of Role))
            If ProtocolEnv.OwnerRolesInSession AndAlso MyRoles IsNot Nothing AndAlso MyRoles.Count > 1 Then
                OwnerRolesInSession = roles.Select(Function(x) x.Id).ToList()
            End If
        End Sub

        Private Sub CmdOkEditClick(sender As Object, e As EventArgs) Handles cmdOkEdit.Click, cmdAssignNumber.Click

            If Action = DocumentSeriesAction.Edit Then

                Dim chain As BiblosChainInfo = Facade.DocumentSeriesItemFacade.GetMainChainInfo(CurrentDocumentSeriesItem)
                chain.AddDocuments(uscUploadDocument.DocumentInfosAdded.ToList())

                Dim previousPublishingDate As DateTimeOffset? = CurrentDocumentSeriesItem.PublishingDate
                FillDataFromPage(chain, CurrentDocumentSeriesItem)

                Dim isConstraintChanged As Boolean = False
                If (tblConstraints.Visible AndAlso Not CurrentDocumentSeriesItem.ConstraintValue.Eq(ddlConstraints.SelectedItem.Text)) Then
                    CurrentDocumentSeriesItem.ConstraintValue = If(String.IsNullOrEmpty(ddlConstraints.SelectedItem.Text), Nothing, ddlConstraints.SelectedItem.Text)
                    isConstraintChanged = True
                End If

                If ((CurrentDocumentSeriesItem.SubCategory IsNot Nothing AndAlso Not ItemSubCategory.HasSelectedCategories) _
                        OrElse (CurrentDocumentSeriesItem.SubCategory Is Nothing AndAlso ItemSubCategory.HasSelectedCategories) _
                        OrElse (CurrentDocumentSeriesItem.SubCategory IsNot Nothing AndAlso ItemSubCategory.HasSelectedCategories AndAlso
                                    Not CurrentDocumentSeriesItem.SubCategory.Id.Equals(ItemSubCategory.SelectedCategories.First().Id))) Then
                    If CurrentFascicleDocumentUnitFacade.GetFascicolatedIdDocumentUnit(CurrentDocumentSeriesItem.UniqueId) IsNot Nothing Then
                        AjaxAlert("Non è possibile modificare il classificatore del documento in quanto già Fascicolato.")
                        Exit Sub
                    End If
                End If


                If Not ItemSubCategory.SelectedCategories.IsNullOrEmpty() Then
                    CurrentDocumentSeriesItem.SubCategory = ItemSubCategory.SelectedCategories.First()
                Else
                    CurrentDocumentSeriesItem.SubCategory = Nothing
                End If

                If CurrentDocumentSeriesItem.DocumentSeries.SubsectionEnabled Then
                    ' Salvo anche la subsection
                    CurrentDocumentSeriesItem.DocumentSeriesSubsection = SelectedDocumentSeriesSubsection
                End If

                ' ROLES. Da Aggiungere DOPO il salvataggio dell'Item
                If CurrentDocumentSeriesItem.DocumentSeries.RoleEnabled.GetValueOrDefault(False) Then
                    ' Owner ADD
                    If uscRoleOwner.RoleListAdded IsNot Nothing AndAlso uscRoleOwner.RoleListAdded.Count > 0 Then
                        Dim roles As IList(Of Role) = Facade.RoleFacade.GetByIds(uscRoleOwner.RoleListAdded)
                        For Each role As Role In roles
                            Facade.DocumentSeriesItemRoleFacade.AddOwnerRole(CurrentDocumentSeriesItem, role)
                        Next
                    End If
                    ' Owner REMOVE
                    If uscRoleOwner.RoleListRemoved IsNot Nothing AndAlso uscRoleOwner.RoleListRemoved.Count > 0 Then
                        Dim roles As IList(Of Role) = Facade.RoleFacade.GetByIds(uscRoleOwner.RoleListRemoved)
                        For Each role As Role In roles
                            Facade.DocumentSeriesItemRoleFacade.RemoveOwnerRole(CurrentDocumentSeriesItem, role)
                        Next
                    End If

                    ' Authorized ADD
                    If uscRoleAuthorization.RoleListAdded IsNot Nothing AndAlso uscRoleAuthorization.RoleListAdded.Count > 0 Then
                        Dim roles As IList(Of Role) = Facade.RoleFacade.GetByIds(uscRoleAuthorization.RoleListAdded)
                        For Each role As Role In roles
                            Facade.DocumentSeriesItemRoleFacade.AddAuthorizedRole(CurrentDocumentSeriesItem, role)
                        Next
                    End If
                    ' Authorized REMOVE
                    If uscRoleAuthorization.RoleListRemoved IsNot Nothing AndAlso uscRoleAuthorization.RoleListRemoved.Count > 0 Then
                        Dim roles As IList(Of Role) = Facade.RoleFacade.GetByIds(uscRoleAuthorization.RoleListRemoved)
                        For Each role As Role In roles
                            Facade.DocumentSeriesItemRoleFacade.RemoveAuthorizationRole(CurrentDocumentSeriesItem, role)
                        Next
                    End If
                End If
                ' Priorità
                CurrentDocumentSeriesItem.Priority = chkPriority.Checked

                If uscUploadDocument.DocumentsToDelete.Count > 0 Then
                    DeleteDocuments(uscUploadDocument.DocumentsToDelete, CurrentDocumentSeriesItem.Location)
                End If

                If uscUploadAnnexed.DocumentsToDelete.Count > 0 Then
                    DeleteDocuments(uscUploadAnnexed.DocumentsToDelete, CurrentDocumentSeriesItem.LocationAnnexed)
                End If

                If uscUnpublishedAnnexed.DocumentsToDelete.Count > 0 Then
                    DeleteDocuments(uscUnpublishedAnnexed.DocumentsToDelete, CurrentDocumentSeriesItem.LocationUnpublishedAnnexed)
                End If

                ' Gestisco la possibilità che l'utente, con i vari detach singoli, rimuova tutti i documenti presenti
                If uscUploadAnnexed.DocumentsCount = 0 AndAlso Not CurrentDocumentSeriesItem.IdAnnexed.Equals(Guid.Empty) Then
                    CurrentDocumentSeriesItem.IdAnnexed = Guid.Empty
                End If

                If uscUnpublishedAnnexed.DocumentsCount = 0 AndAlso Not CurrentDocumentSeriesItem.IdUnpublishedAnnexed.Equals(Guid.Empty) Then
                    CurrentDocumentSeriesItem.IdUnpublishedAnnexed = Guid.Empty
                End If

                Facade.DocumentSeriesItemFacade.UpdateDocumentSeriesItem(CurrentDocumentSeriesItem, chain, uscUploadAnnexed.DocumentInfosAdded, uscUnpublishedAnnexed.DocumentInfosAdded, $"Modificata registrazione {CurrentDocumentSeriesItem.DocumentSeries.Name} {CurrentDocumentSeriesItem.Year:0000}/{CurrentDocumentSeriesItem.Number:0000000}")

                If Not previousPublishingDate.HasValue AndAlso CurrentDocumentSeriesItem.PublishingDate.HasValue Then
                    Facade.DocumentSeriesItemLogFacade.AddLog(CurrentDocumentSeriesItem, DocumentSeriesItemLogType.Edit, $"Pubblicata registrazione in data {CurrentDocumentSeriesItem.PublishingDate:dd/MM/yyyy}")
                End If

                If previousPublishingDate.HasValue Then
                    If Not CurrentDocumentSeriesItem.PublishingDate.HasValue Then
                        Facade.DocumentSeriesItemLogFacade.AddLog(CurrentDocumentSeriesItem, DocumentSeriesItemLogType.Edit, "Rimossa data di pubblicazione")
                    ElseIf (CurrentDocumentSeriesItem.PublishingDate.Value <> previousPublishingDate.Value) Then
                        Facade.DocumentSeriesItemLogFacade.AddLog(CurrentDocumentSeriesItem, DocumentSeriesItemLogType.Edit, $"Modificata data di pubblicazione da {previousPublishingDate:dd/MM/yyyy} a {CurrentDocumentSeriesItem.PublishingDate:dd/MM/yyyy}")
                    End If
                End If

                If isConstraintChanged Then
                    If String.IsNullOrEmpty(CurrentDocumentSeriesItem.ConstraintValue) Then
                        Facade.DocumentSeriesItemLogFacade.AddLog(CurrentDocumentSeriesItem, DocumentSeriesItemLogType.RO, String.Concat("Rimosso obbligo trasparenza"))
                    Else
                        Facade.DocumentSeriesItemLogFacade.AddLog(CurrentDocumentSeriesItem, DocumentSeriesItemLogType.SO, String.Concat("Impostato obbligo trasparenza '", CurrentDocumentSeriesItem.ConstraintValue, "'"))
                    End If
                End If

                If ProtocolEnv.DocumentSeriesReorderDocumentEnabled OrElse ProtocolEnv.DocumentSeriesRenameDocumentEnabled Then
                    Dim toUpdateMainDocuments As List(Of BiblosDocumentInfo) = uscUploadDocument.DocumentInfos.OfType(Of BiblosDocumentInfo).Where(Function(x) Not uscUploadDocument.DocumentInfosAdded.Any(Function(xx) xx.Serialized = x.Serialized)).ToList()
                    Dim toUpdateAnnexed As List(Of BiblosDocumentInfo) = uscUploadAnnexed.DocumentInfos.OfType(Of BiblosDocumentInfo).Where(Function(x) Not uscUploadAnnexed.DocumentInfosAdded.Any(Function(xx) xx.Serialized = x.Serialized)).ToList()
                    Dim toUpdateUnpublishedAnnexed As List(Of BiblosDocumentInfo) = uscUnpublishedAnnexed.DocumentInfos.OfType(Of BiblosDocumentInfo).Where(Function(x) Not uscUnpublishedAnnexed.DocumentInfosAdded.Any(Function(xx) xx.Serialized = x.Serialized)).ToList()

                    Facade.DocumentSeriesItemFacade.ChangeDocumentsAttributes(CurrentDocumentSeriesItem, CurrentDocumentSeriesItem.Location, toUpdateMainDocuments, chain.Attributes)
                    Facade.DocumentSeriesItemFacade.ChangeDocumentsAttributes(CurrentDocumentSeriesItem, CurrentDocumentSeriesItem.LocationAnnexed, toUpdateAnnexed)
                    Facade.DocumentSeriesItemFacade.ChangeDocumentsAttributes(CurrentDocumentSeriesItem, CurrentDocumentSeriesItem.LocationUnpublishedAnnexed, toUpdateUnpublishedAnnexed)
                End If

                If DocumentUnitsToDelete IsNot Nothing Then
                    Dim resl As Resolution
                    For Each doc As DocumentUnitModel In DocumentUnitsToDelete
                        Select Case doc.Environment
                            Case DSWEnvironment.Protocol
                                Facade.ProtocolDocumentSeriesItemFacade.RemoveLinkProtocolToDocumentSeriesItem(doc.UniqueId, CurrentDocumentSeriesItem)

                                Dim message As String = String.Format("Disassociato in {3} {0}: {1}/{2:000000}", CurrentDocumentSeriesItem.DocumentSeries.Container.Name, CurrentDocumentSeriesItem.Year, CurrentDocumentSeriesItem.Number, ProtocolEnv.DocumentSeriesName)
                                If CurrentDocumentSeriesItem.Status.Equals(DocumentSeriesItemStatus.Draft) Then
                                    message = String.Format("Disassociato in {2} {0}: Bozza N: {1}", CurrentDocumentSeriesItem.DocumentSeries.Container.Name, CurrentDocumentSeriesItem.Id, ProtocolEnv.DocumentSeriesName)
                                End If
                                ' Aggiungere log in Protocol
                                Facade.ProtocolLogFacade.Insert(doc.Year, doc.Number, ProtocolLogEvent.SD, message)

                                Exit Select
                            Case DSWEnvironment.Resolution
                                resl = Facade.ResolutionFacade.GetById(doc.EntityId)
                                Facade.ResolutionDocumentSeriesItemFacade.RemoveLinkResolutionToDocumentSeriesItem(resl, CurrentDocumentSeriesItem)
                                ' Aggiungere log in Resolution
                                Facade.ResolutionLogFacade.Insert(CurrentDocumentSeriesItem.Id, ResolutionLogType.SD, String.Format("Disassociato in {3} {0}: {1}/{2:000000}", CurrentDocumentSeriesItem.DocumentSeries.Container.Name, CurrentDocumentSeriesItem.Year, CurrentDocumentSeriesItem.Number, ProtocolEnv.DocumentSeriesName))
                                Exit Select
                        End Select
                    Next
                End If
            End If

            Dim btn As Button = CType(sender, Button)
            If btn.CommandArgument.Eq("ASSIGN") Then

                ' Assegno anno e numero incrementali
                Facade.DocumentSeriesItemFacade.AssignNumber(CurrentDocumentSeriesItem)
            End If

            If CurrentDocumentSeriesItem.Status = DocumentSeriesItemStatus.Active Then
                If btn.CommandArgument.Eq("ASSIGN") Then
                    Facade.DocumentSeriesItemFacade.SendInsertDocumentSeriesItemCommand(CurrentDocumentSeriesItem, New List(Of IWorkflowAction))
                Else
                    Facade.DocumentSeriesItemFacade.SendUpdateDocumentSeriesItemCommand(CurrentDocumentSeriesItem)
                End If
            End If

            GoToView()
        End Sub

        Private Sub CmdEditClick(sender As Object, e As EventArgs) Handles cmdEdit.Click
            GoToEdit()
        End Sub

        Private Sub CmdPublishClick(sender As Object, e As EventArgs) Handles cmdPublish.Click
            Facade.DocumentSeriesItemFacade.Publish(CurrentDocumentSeriesItem)
            DataBindPublicationFields()
            DataBindLastEditFields()
            VisibilityPublicationButtons()
            VisibilityFlushButtons()
        End Sub

        Private Sub CmdRetireClick(sender As Object, e As EventArgs) Handles cmdRetire.Click
            Facade.DocumentSeriesItemFacade.Retire(CurrentDocumentSeriesItem)
            DataBindPublicationFields()
            DataBindLastEditFields()
            VisibilityPublicationButtons()
            VisibilityFlushButtons()
        End Sub

        Private Sub CmdCancelOkClick(sender As Object, e As EventArgs) Handles cmdCancelOk.Click
            Facade.DocumentSeriesItemFacade.Cancel(CurrentDocumentSeriesItem, txtCancelMotivation.Text)
            Facade.DocumentSeriesItemFacade.SendUpdateDocumentSeriesItemCommand(CurrentDocumentSeriesItem)
            GeToSearch()
        End Sub

        Private Sub CmdFlushClick(sender As Object, e As EventArgs) Handles cmdFlushDocs.Click, cmdFlushAnnexed.Click, cmdFlushUnpublishedAnnexed.Click
            Dim cmdSender As Control = CType(sender, Control)
            Dim message As String = String.Empty
            Select Case cmdSender.ID
                Case cmdFlushDocs.ID
                    Dim attributes As Dictionary(Of String, String) = Facade.DocumentSeriesItemFacade.GetAttributes(CurrentDocumentSeriesItem)
                    Dim chain As New BiblosChainInfo()
                    ' Riversare gli attributi da attuale catena
                    chain.AddAttributes(attributes)
                    Service.DetachDocument(CurrentDocumentSeriesItem.IdMain)
                    CurrentDocumentSeriesItem.IdMain = chain.ArchiveInBiblos(CurrentDocumentSeriesItem.Location.ProtBiblosDSDB)
                    CurrentDocumentSeriesItem.HasMainDocument = False
                    uscUploadDocument.LoadDocumentInfo(New List(Of DocumentInfo))
                    message = "Catena documenti svuotata."
                Case cmdFlushAnnexed.ID
                    Service.DetachDocument(CurrentDocumentSeriesItem.IdAnnexed)
                    CurrentDocumentSeriesItem.IdAnnexed = Guid.Empty
                    uscUploadAnnexed.LoadDocumentInfo(New List(Of DocumentInfo))
                    message = "Catena annessi svuotata."
                Case cmdFlushUnpublishedAnnexed.ID
                    Service.DetachDocument(CurrentDocumentSeriesItem.IdUnpublishedAnnexed)
                    CurrentDocumentSeriesItem.IdUnpublishedAnnexed = Guid.Empty
                    uscUnpublishedAnnexed.LoadDocumentInfo(New List(Of DocumentInfo))
                    message = "Catena annessi non pubblicati svuotata."
            End Select

            ' Salvo le modifiche
            Facade.DocumentSeriesItemFacade.Update(CurrentDocumentSeriesItem)
            Facade.DocumentSeriesItemLogFacade.AddLog(CurrentDocumentSeriesItem, DocumentSeriesItemLogType.Edit, message)
            Facade.DocumentSeriesItemFacade.SendUpdateDocumentSeriesItemCommand(CurrentDocumentSeriesItem)
            GoToView()
        End Sub

        Private Sub RadAjaxManagerAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)

            Dim arguments As String() = Split(e.Argument, "|")

            If arguments(0).Eq(uscUploadDocument.ClientID) OrElse arguments(0).Eq(uscUploadAnnexed.ClientID) OrElse arguments(0).Eq(uscUnpublishedAnnexed.ClientID) Then
                Exit Sub
            End If

            Select Case arguments(0)
                Case "removeDraftLink"
                    Dim sources As IList(Of DocumentUnitModel) = Nothing
                    Dim newSource As IList(Of DocumentUnitModel)
                    Try
                        sources = DirectCast(dgvLinkedDocumentUnit.DataSource, IList(Of DocumentUnitModel))
                        Dim guidDocUnitModel As Guid
                        If Guid.TryParse(arguments(1), guidDocUnitModel) Then
                            Dim docToDelete As DocumentUnitModel = sources.Where(Function(f) f.UniqueId.Equals(guidDocUnitModel)).FirstOrDefault()
                            If docToDelete IsNot Nothing Then
                                AddDocumentUnitToDelete(docToDelete)
                                sources.Remove(docToDelete)
                                newSource = sources
                            End If
                            dgvLinkedDocumentUnit.DataSource = newSource
                            dgvLinkedDocumentUnit.Rebind()
                        End If

                        Exit Sub
                    Catch ex As Exception
                        FileLogger.Error(LoggerName, String.Format("Errore in fase di rimozione dell'unità documentaria: {0}", arguments(0)), ex)
                        AjaxManager.Alert("Errore in fase di  rimozione dell'unità documentaria.")
                    End Try
            End Select

            Try

                Dim behaviours As IList(Of DocumentSeriesAttributeBehaviour) = Facade.DocumentSeriesAttributeBehaviourFacade.GetAttributeBehaviours(CurrentDocumentSeriesItem.DocumentSeries, Action, e.Argument)
                ApplyBehaviours(behaviours)
                AjaxManager.ResponseScripts.Add("item.initialize();")

            Catch ex As Exception
                FileLogger.Error(LoggerName, String.Format("Errore in fase di esecuzione Behaviour Command: {0}", e.Argument), ex)
                AjaxManager.Alert("Errore in fase di esecuzione commando.")
            End Try
        End Sub

        Private Sub ddlContainerArchive_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlContainerArchive.SelectedIndexChanged
            ContentWrapper.SetDisplay(False)
            LoadAvailableDocumentSeries()
        End Sub

        Private Sub ddlDocumentSeries_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlDocumentSeries.SelectedIndexChanged
            If CurrentDocumentSeriesItem.DocumentSeries Is Nothing Then
                ContentWrapper.SetDisplay(False)
            Else
                LoadDocumentSeries(True)
                ContentWrapper.SetDisplay(True)
            End If
        End Sub

        Private Sub CmdFlushOwnerClick(sender As Object, e As EventArgs) Handles cmdFlushOwner.Click
            OwnerRolesInSession = Nothing
            InitializeRoles()
            cmdFlushOwner.Visible = False
        End Sub

        Private Sub cmdViewDocuments_Click(sender As Object, e As EventArgs) Handles cmdViewDocuments.Click
            Response.Redirect("~/Viewers/DocumentSeriesItemViewer.aspx?" & CommonShared.AppendSecurityCheck(String.Format("id={0}", CurrentDocumentSeriesItem.Id)))
        End Sub

        Private Sub cmdAVCPEditor_Click(sender As Object, e As EventArgs) Handles cmdAVCPEditor.Click
            Dim ulr As String = Page.Request.Url.AbsoluteUri
            If Not String.IsNullOrEmpty(CurrentPreviousPage) AndAlso (CurrentPreviousPage.Contains("ReslInserimento.aspx") OrElse CurrentPreviousPage.Contains("ReslVisualizza.aspx")) Then
                ulr = CurrentPreviousPage
            End If
            Response.Redirect(String.Concat("~/Series/AVCPEditor.aspx?",
                                            CommonShared.AppendSecurityCheck(String.Format("Type=Series&IdDocumentSeriesItem={0}&PreviousPage={1}", CurrentDocumentSeriesItem.Id, HttpUtility.UrlEncode(ulr))), True))
        End Sub

        Private Sub cmdAVCPAutocomplete_Click(sender As Object, e As EventArgs) Handles cmdAVCPAutocomplete.Click
            Dim TypeSeries As TypeSeries
            Select Case CurrentDocumentSeriesItem.DocumentSeries.Id
                Case DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId
                    TypeSeries = TypeSeries.AVCP
                Case DocSuiteContext.Current.ProtocolEnv.BandiGaraDocumentSeriesId
                    TypeSeries = TypeSeries.BandiDiGara
                    'Case DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId
                    '    TypeSeries = "ProvDir"
            End Select

            ' Recupero e salvo i dati di classificazione
            Dim selectedCategory As Category = ItemCategory.SelectedCategories.First()
            Dim root As Category = selectedCategory.Root
            Dim categoryId As Integer
            Dim subCategoryId As Integer
            If selectedCategory.Equals(root) Then
                categoryId = selectedCategory.Id
            Else
                categoryId = root.Id
                subCategoryId = selectedCategory.Id
            End If
            BindModelFromPage()

            Response.Redirect(String.Format("~/Series/AVCPEditor.aspx?Type=Series&TypeSeries={0}&Category={1}&SubCategory={2}&IdResolution={3}&Action={4}&PreviousPage={5}" _
                                            , TypeSeries.GetDescription(), categoryId, subCategoryId, CurrentResolutionId, Action, HttpUtility.UrlEncode(Page.Request.Url.AbsoluteUri)))
        End Sub

        Private Sub uscRoleAuthorization_RolesAddes(sender As Object, e As RoleEventArgs) Handles uscRoleAuthorization.RoleAdded
            Dim ownerRoles As IList(Of Role) = uscRoleOwner.GetRoles()
            If ownerRoles.Any(Function(x) x.Id.Equals(e.Role.Id)) Then
                AjaxAlert(String.Format("il settore {0} è già presente come settore di appartenenza e quindi non può essere inserito.", e.Role.Name))
                uscRoleAuthorization.RemoveRoleNode(e.Role)
            End If
        End Sub

        Protected Sub ItemCategory_CategoryAdding(sender As Object, e As EventArgs) Handles ItemCategory.CategoryAdding
            If Action = DocumentSeriesAction.Edit Then
                ItemCategory.FromDate = CurrentDocumentSeriesItem.RegistrationDate.Date
            End If
        End Sub

        Protected Sub ItemSubCategory_CategoryAdding(sender As Object, e As EventArgs) Handles ItemSubCategory.CategoryAdding
            If Action = DocumentSeriesAction.Edit Then
                ItemSubCategory.FromDate = CurrentDocumentSeriesItem.RegistrationDate.Date
            End If
        End Sub

        Protected Sub DgvLinkedDocumentUnit_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles dgvLinkedDocumentUnit.ItemDataBound
            If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
                Exit Sub
            End If

            Dim dto As DocumentUnitModel = DirectCast(e.Item.DataItem, DocumentUnitModel)
            Dim btnRemoveLink As ImageButton = DirectCast(e.Item.FindControl("btnRemoveLink"), ImageButton)
            Dim seriesLink As HyperLink = DirectCast(e.Item.FindControl("documentUnitLink"), HyperLink)

            seriesLink.Visible = True
            btnRemoveLink.Visible = True
            seriesLink.Text = dto.Title

            Select Case dto.Environment
                Case DSWEnvironment.Protocol
                    seriesLink.NavigateUrl = $"~/Prot/ProtVisualizza.aspx?UniqueId={dto.UniqueId}&Type=Prot"
                    Exit Select
                Case DSWEnvironment.Resolution
                    seriesLink.NavigateUrl = String.Format("../Resl/ReslVisualizza.aspx?IdResolution={0}&Type=Resl", dto.EntityId)
            End Select

            btnRemoveLink.OnClientClick = String.Format("return RemoveDraftLink('{0}');", dto.UniqueId)
        End Sub

        Private Sub ItemPublishingDate_SelectedDateChanged(sender As Object, e As EventArgs) Handles ItemPublishingDate.SelectedDateChanged
            SetFiltersState()
            If ProtocolEnv.DocumentSeriesPublicationDateConstraintEnabled Then
                ItemPublishingDate.MinDate = Date.Today.Date

                If ItemPublishingDate.SelectedDate.HasValue AndAlso ItemPublishingDate.SelectedDate.Value < Date.Today.Date Then
                    SetFiltersState(True)
                    Return
                End If
            End If
        End Sub
#End Region

#Region " Methods "

        Private Sub InitializeAjax()
            AddHandler AjaxManager.AjaxRequest, AddressOf RadAjaxManagerAjaxRequest

            AjaxManager.AjaxSettings.AddAjaxSetting(ddlContainerArchive, ddlContainerArchive, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlContainerArchive, ddlDocumentSeries, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlContainerArchive, ContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlContainerArchive, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)

            AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentSeries, ddlDocumentSeries, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentSeries, ddlContainerArchive, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentSeries, ContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentSeries, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)

            AjaxManager.AjaxSettings.AddAjaxSetting(cmdOk, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdSaveDraft, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdOkEdit, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdEdit, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdAssignNumber, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)

            AjaxManager.AjaxSettings.AddAjaxSetting(cmdOk, ContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdSaveDraft, ContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdOkEdit, ContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdEdit, ContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdAssignNumber, ContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)

            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, ContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdCancelOk, txtCancelMotivation, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, DynamicControls, MasterDocSuite.AjaxDefaultLoadingPanel)

            AjaxManager.AjaxSettings.AddAjaxSetting(cmdPublish, ContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdRetire, ContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnYearConfirm, UpdatePanelSelectYear, MasterDocSuite.AjaxDefaultLoadingPanel)

            AjaxManager.AjaxSettings.AddAjaxSetting(cmdPublish, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdRetire, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)

            AjaxManager.AjaxSettings.AddAjaxSetting(cmdFlushDocs, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdFlushDocs, ContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdFlushAnnexed, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdFlushAnnexed, ContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdFlushUnpublishedAnnexed, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdFlushUnpublishedAnnexed, ContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)

            AjaxManager.AjaxSettings.AddAjaxSetting(cmdFlushOwner, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdFlushOwner, ContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdOk, dropYears)

            AjaxManager.AjaxSettings.AddAjaxSetting(cmdAVCPEditor, ContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdAVCPEditor, DynamicControls, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdAVCPEditor, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)

            AjaxManager.AjaxSettings.AddAjaxSetting(cmdAVCPAutocomplete, ContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdAVCPAutocomplete, DynamicControls, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdAVCPAutocomplete, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)

            AjaxManager.AjaxSettings.AddAjaxSetting(uscRoleOwner, uscRoleOwner)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscRoleAuthorization, uscRoleAuthorization)
            AjaxManager.AjaxSettings.AddAjaxSetting(ItemPublishingDate, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)

            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                AjaxManager.AjaxSettings.AddAjaxSetting(uscUploadDocument, uscUploadDocument, MasterDocSuite.AjaxDefaultLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(uscUploadAnnexed, uscUploadAnnexed, MasterDocSuite.AjaxDefaultLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(uscUnpublishedAnnexed, uscUnpublishedAnnexed, MasterDocSuite.AjaxDefaultLoadingPanel)
            End If
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, dgvLinkedDocumentUnit, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(dgvLinkedDocumentUnit, dgvLinkedDocumentUnit)
        End Sub

        Public Sub InitializeActionDuplicate()
            If (Page.PreviousPage Is Nothing) OrElse Not (TypeOf Page.PreviousPage Is Duplicate) Then
                Throw New DocSuiteException("Errore duplicazione registrazione", "Impossibile inizializzare dalla pagina di provenienza.")
            End If

            FileLogger.Debug(LoggerName, "Inserimento nuova registrazione in Serie Documentale da Duplication.")

            Dim prevPage As Duplicate = DirectCast(Page.PreviousPage, Duplicate)

            Dim toDuplicateItems As IList(Of ItemToDuplicate) = prevPage.ItemsToDuplicate
            Dim source As DocumentSeriesItem = prevPage.CurrentDocumentSeriesItem

            If toDuplicateItems.Contains(ItemToDuplicate.OwnerRoles) Then
                uscRoleOwner.SourceRoles.Clear()
                uscRoleOwner.DataBind()
                uscRoleOwner.AddRoles(Facade.DocumentSeriesItemRoleFacade.GetOwnerRoles(source), True, False, False)
            End If

            If toDuplicateItems.Contains(ItemToDuplicate.KnowledgeRoles) Then
                uscRoleAuthorization.SourceRoles.Clear()
                uscRoleAuthorization.DataBind()
                uscRoleAuthorization.AddRoles(Facade.DocumentSeriesItemRoleFacade.GetAuthorizedRoles(source), True, False, False)
            End If

            If toDuplicateItems.Contains(ItemToDuplicate.Subject) Then
                ItemSubject.Text = source.Subject
            End If

            If toDuplicateItems.Contains(ItemToDuplicate.Publication) Then
                If (source.PublishingDate.HasValue) Then
                    ItemPublishingDate.SelectedDate = source.PublishingDate.Value
                End If
                If (source.RetireDate.HasValue) Then
                    ItemRetireDate.SelectedDate = source.RetireDate.Value
                End If
            End If

            If toDuplicateItems.Contains(ItemToDuplicate.DynamicData) Then
                DataBindAttributes(SelectedArchiveInfo, source)
            End If

            If toDuplicateItems.Contains(ItemToDuplicate.Documents) Then
                uscUploadDocument.ClearNodes()
                uscUploadAnnexed.ClearNodes()
                uscUnpublishedAnnexed.ClearNodes()
                Dim documents As List(Of DocumentInfo) = Facade.DocumentSeriesItemFacade.GetMainChainInfo(source).Documents.Select(Function(s) New TempFileDocumentInfo(s.Name, s.SaveUniqueToTemp())).Cast(Of DocumentInfo).ToList()
                If Not documents.IsNullOrEmpty() Then
                    uscUploadDocument.LoadDocumentInfo(documents, False, True, False, True)
                    uscUploadDocument.InitializeNodesAsAdded(True)
                End If

                If source.LocationAnnexed IsNot Nothing Then
                    Dim annexed As List(Of DocumentInfo) = Facade.DocumentSeriesItemFacade.GetAnnexedDocuments(source).Cast(Of DocumentInfo).ToList()
                    If Not annexed.IsNullOrEmpty() Then
                        uscUploadAnnexed.LoadDocumentInfo(annexed, False, True, False, True)
                        uscUploadAnnexed.InitializeNodesAsAdded(True)
                    End If
                End If

                If source.LocationUnpublishedAnnexed IsNot Nothing Then
                    Dim unpublishedAnnexed As List(Of DocumentInfo) = Facade.DocumentSeriesItemFacade.GetUnpublishedAnnexedDocuments(source).Cast(Of DocumentInfo).ToList()
                    If Not unpublishedAnnexed.IsNullOrEmpty() Then
                        uscUnpublishedAnnexed.LoadDocumentInfo(unpublishedAnnexed, False, True, False, True)
                        uscUnpublishedAnnexed.InitializeNodesAsAdded(True)
                    End If
                End If
            End If

            If toDuplicateItems.Contains(ItemToDuplicate.Category) Then
                Dim categoryToDuplicate As Category = If(source.SubCategory IsNot Nothing, source.SubCategory, source.Category)
                If Facade.CategoryFacade.IsCategoryActive(categoryToDuplicate) Then
                    ItemCategory.DataSource.Add(categoryToDuplicate)
                End If
                ItemCategory.DataBind()
            End If

        End Sub

        Public Sub InitializeDropdownYear(IdDocumentSeries As Integer)

            Dim years As IList(Of DocumentSeriesIncremental) = Facade.DocumentSeriesIncrementalFacade.GetOpenDocumentIncrementalSeries(IdDocumentSeries)
            dropYears.DataSource = years
            dropYears.DataTextField = "Year"
            dropYears.DataValueField = "Year"
            dropYears.DataBind()

        End Sub

        Public Sub InitializeActionFromCollaboration()
            If (Page.PreviousPage Is Nothing) OrElse Not (TypeOf Page.PreviousPage Is CollaborationToSeries) Then
                Throw New DocSuiteException("Errore creazione registrazione", "Impossibile inizializzare dalla pagina di provenienza.")
            End If

            Dim prevPage As CollaborationToSeries = DirectCast(Page.PreviousPage, CollaborationToSeries)
            FileLogger.Debug(LoggerName, String.Format("Inserimento nuova registrazione in Serie Documentale da Collaboration [{0}].", prevPage.CurrentCollaborationId))

            CurrentCollaboration = prevPage.CurrentCollaboration
            ' Imposto documenti, annessi e annessi non pubblicabili
            Dim documents, annexed, unpublishedAnnexed As New List(Of DocumentInfo)
            Const initialCode As String = "coll"

            'Imposto Oggetto
            ItemSubject.Text = prevPage.CurrentCollaboration.CollaborationObject

            For Each selectedMainDocument As DocumentInfo In prevPage.SelectedMainDocument
                If ProtocolEnv.IsFilenameAutomaticRenameEnabled Then
                    selectedMainDocument.Name = WebHelper.UploadDocumentSetFilename(initialCode, selectedMainDocument.Extension, documents.Count + 1)
                End If
                documents.Add(selectedMainDocument)
            Next

            For Each selectedAnnexed As DocumentInfo In prevPage.SelectedAnnexed
                If ProtocolEnv.IsFilenameAutomaticRenameEnabled Then
                    selectedAnnexed.Name = WebHelper.UploadDocumentSetFilename(initialCode, selectedAnnexed.Extension, documents.Count + 1)
                End If
                annexed.Add(selectedAnnexed)
            Next

            For Each selectedUnpublishedAnnexed As DocumentInfo In prevPage.SelectedUnpublishedAnnexed
                If ProtocolEnv.IsFilenameAutomaticRenameEnabled Then
                    selectedUnpublishedAnnexed.Name = WebHelper.UploadDocumentSetFilename(initialCode, selectedUnpublishedAnnexed.Extension, documents.Count + 1)
                End If
                unpublishedAnnexed.Add(selectedUnpublishedAnnexed)
            Next

            uscUploadDocument.LoadDocumentInfo(documents)
            uscUploadDocument.InitializeNodesAsAdded(True)

            uscUploadAnnexed.LoadDocumentInfo(annexed)
            uscUploadAnnexed.InitializeNodesAsAdded(True)

            uscUnpublishedAnnexed.LoadDocumentInfo(unpublishedAnnexed)
            uscUnpublishedAnnexed.InitializeNodesAsAdded(True)
        End Sub

        ''' <summary> Inizializza la pagina per l'Inserimento da Resolution </summary>
        Public Sub InitializeActionFromResolution()
            If (Page.PreviousPage Is Nothing) OrElse Not (TypeOf Page.PreviousPage Is ToSeries) Then
                Throw New DocSuiteException("Errore creazione registrazione", "Impossibile inizializzare dalla pagina di provenienza.")
            End If

            FileLogger.Debug(LoggerName, String.Format("Inserimento nuova registrazione in serie documentale da atto [{0}].", CurrentResolution.Id))

            Dim prevPage As ToSeries = DirectCast(Page.PreviousPage, ToSeries)

            ' Imposto documenti, annessi e annessi non pubblicabili
            Dim documents, annexed, unpublishedAnnexed As New List(Of DocumentInfo)
            Dim initialCode As String = String.Empty

            If ProtocolEnv.IsFilenameAutomaticRenameEnabled Then
                initialCode = WebHelper.UploadDocumentRename(CurrentResolution.Type.Id, CurrentResolution.Year.Value, CurrentResolution.Number.Value)
            End If

            If ProtocolEnv.SeriesPublishingDateFromResolutionEnabled Then
                ItemPublishingDate.SelectedDate = CurrentResolution.PublishingDate
            End If

            For Each item As KeyValuePair(Of DocumentInfo, ToSeries.ChainType) In prevPage.SelectedDocuments
                Select Case item.Value
                    Case ToSeries.ChainType.Document
                        If ProtocolEnv.IsFilenameAutomaticRenameEnabled Then
                            item.Key.Name = WebHelper.UploadDocumentSetFilename(initialCode, item.Key.Extension, documents.Count + 1)
                        End If
                        documents.Add(item.Key)
                    Case ToSeries.ChainType.Annexed
                        If ProtocolEnv.IsFilenameAutomaticRenameEnabled Then
                            item.Key.Name = WebHelper.UploadDocumentSetFilename(initialCode, item.Key.Extension, annexed.Count + 1)
                        End If
                        annexed.Add(item.Key)
                    Case ToSeries.ChainType.UnpublishedAnnexed
                        If ProtocolEnv.IsFilenameAutomaticRenameEnabled Then
                            item.Key.Name = WebHelper.UploadDocumentSetFilename(initialCode, item.Key.Extension, unpublishedAnnexed.Count + 1)
                        End If
                        unpublishedAnnexed.Add(item.Key)
                End Select
            Next

            uscUploadDocument.LoadDocumentInfo(documents)
            uscUploadDocument.InitializeNodesAsAdded(True)

            uscUploadAnnexed.LoadDocumentInfo(annexed)
            uscUploadAnnexed.InitializeNodesAsAdded(True)

            uscUnpublishedAnnexed.LoadDocumentInfo(unpublishedAnnexed)
            uscUnpublishedAnnexed.InitializeNodesAsAdded(True)

        End Sub

        ''' <summary> Inizializza la pagina per l'Inserimento da Resolution con Tipologia Atto attiva </summary>
        Public Sub InitializeActionFromResolutionKind()
            If CurrentResolutionModel Is Nothing Then
                Throw New DocSuiteException("Errore creazione registrazione", "Impossibile inizializzare dalla pagina di provenienza.")
            End If

            ' Popolo i campi precompilati se arrivo dalla pagina di AVCP
            If Not CurrentAVCPFacade Is Nothing AndAlso Not CurrentPubblicazione Is Nothing Then
                For Each attribute As ArchiveAttribute In SelectedArchiveInfo.Attributes
                    Dim control As Control = DynamicControls.FindControlRecursive(attribute.Id.ToString("N"))
                    If control Is Nothing Then Continue For ' Controllo non trovato nella pagina

                    Dim valueString As String = String.Empty
                    Select Case attribute.Name
                        Case DynamicAttribute.Aggiudicatario.GetDescription()
                            valueString = CurrentAVCPFacade.GetAziendeAggiudicatarie(CurrentPubblicazione)
                        Case DynamicAttribute.Lotti.GetDescription()
                            valueString = CurrentAVCPFacade.GetLotti(CurrentPubblicazione)
                        Case DynamicAttribute.Liquidato.GetDescription()
                            valueString = CurrentAVCPFacade.GetImportoSommeLiquidate(CurrentPubblicazione)
                        Case DynamicAttribute.DitteInvitate.GetDescription()
                            valueString = CurrentAVCPFacade.GetAziendeInvitate(CurrentPubblicazione)
                        Case DynamicAttribute.DittePartecipanti.GetDescription()
                            valueString = CurrentAVCPFacade.GetAziendePartecipanti(CurrentPubblicazione)
                        Case DynamicAttribute.ProceduraAggiudicazione.GetDescription()
                            valueString = CurrentAVCPFacade.GetSceltaContraente(CurrentPubblicazione)
                        Case DynamicAttribute.ImportoComplessivo.GetDescription()
                            valueString = CurrentAVCPFacade.GetImportoAggiudicazione(CurrentPubblicazione)
                        Case DynamicAttribute.StrutturaProponente.GetDescription()
                            valueString = CurrentAVCPFacade.GetStrutturaProponente(CurrentPubblicazione)
                    End Select
                    If Not valueString.Eq(String.Empty) Then
                        SetControlValue(control, valueString, attribute.Format, False)
                    End If

                    Dim valueDatetime As DateTime = DateTime.MinValue
                    Select Case attribute.Name
                        Case DynamicAttribute.DurataAl.GetDescription()
                            Dim val As Date? = CurrentAVCPFacade.GetDataFineLavori(CurrentPubblicazione)
                            If (val.HasValue) Then
                                valueDatetime = val.Value
                            End If
                        Case DynamicAttribute.DurataDal.GetDescription()
                            Dim val As Date? = CurrentAVCPFacade.GetDataInizioLavori(CurrentPubblicazione)
                            If (val.HasValue) Then
                                valueDatetime = val.Value
                            End If
                    End Select
                    If Not valueDatetime = DateTime.MinValue Then
                        SetControlValue(control, valueDatetime, attribute.Format, False)
                    End If
                Next
            End If

            FileLogger.Debug(LoggerName, "Inserimento nuova registrazione in Serie Documentale da Resolution (Tipologia Atto)")

            'Imposto Oggetto
            Dim subject As String = CurrentResolutionModel.Subject
            If Not String.IsNullOrEmpty(ProtocolEnv.SeriesFromResolutionKindObjectFormat) Then
                Try
                    Dim deserialize As ICollection(Of SeriesObjectFormat) = JsonConvert.DeserializeObject(Of ICollection(Of SeriesObjectFormat))(ProtocolEnv.SeriesFromResolutionKindObjectFormat)
                    For Each seriesObjectFormat As SeriesObjectFormat In deserialize
                        If seriesObjectFormat.IdDocumentSeries.Equals(CurrentDocumentSeriesItem.DocumentSeries.Id) Then
                            subject = String.Format(seriesObjectFormat.ObjectFormat, CurrentResolutionModel.Subject)
                        End If
                    Next
                Catch ex As Exception
                    FileLogger.Error(LoggerName, "Errore nella deserializzazione del parametro SeriesFromResolutionKindObjectFormat", ex)
                End Try
            End If

            ItemSubject.Text = CurrentResolutionModel.Subject
            For Each idCategory As Integer In CurrentResolutionModel.Category
                ItemCategory.DataSource.Add(Facade.CategoryFacade.GetById(idCategory))
            Next

            ItemCategory.DataBind()
        End Sub

        Private Sub InitializeKindConstraint()
            If (CurrentResolutionModel.ResolutionKind.HasValue) Then
                Dim results As ICollection(Of WebAPIDto(Of Entity.Resolutions.ResolutionKindDocumentSeries)) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentResolutionKindDocumentSeriesFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.IdResolutionKind = CurrentResolutionModel.ResolutionKind.Value
                        finder.IdDocumentSeries = CurrentDocumentSeriesItem.DocumentSeries.Id
                        finder.ExpandProperties = True
                        Return finder.DoSearch()
                    End Function)

                If (results IsNot Nothing AndAlso results.Count > 0) Then
                    Dim kindSeries As Entity.Resolutions.ResolutionKindDocumentSeries = results.Select(Function(s) s.Entity).Single()
                    If (kindSeries.DocumentSeriesConstraint IsNot Nothing) Then
                        ddlConstraints.SelectedValue = kindSeries.DocumentSeriesConstraint.UniqueId.ToString()
                    End If
                End If
            End If
        End Sub

        ''' <summary> Inizializza la pagina per la Visualizzazione </summary>
        Public Sub InitializeActionView()
            If Not CurrentDocumentSeriesItemRights.IsPreviewable Then
                Throw New DocSuiteException("Impossibile visualizzare", "Verificare se si dispone di autorizzazioni necessarie alla visualizzazione.")
            End If

            FileLogger.Debug(LoggerName, String.Format("Visualizzazione Serie Documentale [{0}].", CurrentDocumentSeriesItem.Id))

            ' Impostazione visibilità dei componenti
            ContentWrapper.SetDisplay(True)

            tblSeriesView.Visible = True
            tblIdentification.Visible = True
            tblCategoryView.Visible = True
            ItemObjectTableView.Visible = True
            AnnexedPanel.Visible = CurrentDocumentSeriesItem.DocumentSeries.Container.DocumentSeriesAnnexedLocation IsNot Nothing
            UnpublishedAnnexedPanel.Visible = CurrentDocumentSeriesItem.DocumentSeries.Container.DocumentSeriesUnpublishedAnnexedLocation IsNot Nothing

            chkPriority.Enabled = False
            TogglePublication(False)

            tblSubsectionView.Visible = (CurrentDocumentSeriesItem.DocumentSeries.SubsectionEnabled.GetValueOrDefault(False))
            pnlRoles.Visible = (CurrentDocumentSeriesItem.DocumentSeries.RoleEnabled.GetValueOrDefault(False))
            uscRoleOwner.ReadOnly = True
            uscRoleAuthorization.ReadOnly = True

            ' Impostazione diritti sui componenti
            uscUploadDocument.ButtonAddDocument.Visible = False
            uscUploadDocument.ButtonRemoveEnabled = False
            uscUploadDocument.ButtonCopySeries.Visible = False
            uscUploadDocument.ButtonCopyUDS.Visible = ProtocolEnv.UDSEnabled
            uscUploadDocument.ButtonPreviewEnabled = CurrentDocumentSeriesItemRights.IsReadable

            uscUploadAnnexed.ButtonAddDocument.Visible = False
            uscUploadAnnexed.ButtonRemoveEnabled = False
            uscUploadAnnexed.ButtonCopySeries.Visible = False
            uscUploadAnnexed.ButtonPreviewEnabled = CurrentDocumentSeriesItemRights.IsReadable
            uscUploadAnnexed.ButtonCopyUDS.Visible = ProtocolEnv.UDSEnabled

            uscUnpublishedAnnexed.ButtonAddDocument.Visible = False
            uscUnpublishedAnnexed.ButtonRemoveEnabled = False
            uscUnpublishedAnnexed.ButtonCopySeries.Visible = False
            uscUnpublishedAnnexed.ButtonPreviewEnabled = CurrentDocumentSeriesItemRights.IsReadable
            uscUnpublishedAnnexed.ButtonCopyUDS.Visible = ProtocolEnv.UDSEnabled

            ItemSubject.ReadOnly = True
            ItemSubCategory.ReadOnly = True
            ItemCategory.ReadOnly = True

            VisibilityFlushButtons()

            cmdEdit.Visible = CurrentDocumentSeriesItemRights.IsEditable

            If CurrentDocumentSeriesItem.DocumentSeries.PublicationEnabled Then
                VisibilityPublicationButtons()
            End If

            ' Diritti di Assegnazione Numero a item in stato bozza
            cmdAssignNumber.Visible = CurrentDocumentSeriesItemRights.IsInsertable AndAlso CurrentDocumentSeriesItem.Status = DocumentSeriesItemStatus.Draft

            cmdCancel.Visible = CurrentDocumentSeriesItemRights.IsDeletable
            txtCancelRequired.Enabled = (CurrentDocumentSeriesItem.Status = DocumentSeriesItemStatus.Active)

            ' Posso duplicare solo se ho diritto di inserimento o bozza sulla serie documentale corrente
            cmdDuplicate.Visible = DocumentSeriesItemRights.CheckDocumentSeriesRight(CurrentDocumentSeriesItem.DocumentSeries, DocumentSeriesContainerRightPositions.Draft) OrElse
                DocumentSeriesItemRights.CheckDocumentSeriesRight(CurrentDocumentSeriesItem.DocumentSeries, DocumentSeriesContainerRightPositions.Insert)
            If cmdDuplicate.Visible Then
                cmdDuplicate.PostBackUrl = String.Format("{0}&idDocumentSeriesItem={1}", cmdDuplicate.PostBackUrl, CurrentDocumentSeriesItem.Id)
            End If

            InitializeLogButton(CurrentDocumentSeriesItem)
            ' A seguire... impostazione dei valori dei componenti

            Dim linkedSeries As DocumentSeriesItem = GetSeriesLinked()
            pnlSeriesLink.Visible = False
            If linkedSeries IsNot Nothing Then
                pnlSeriesLink.Visible = True
                documentSeriesLink.Text = String.Format("Serie: {2} N. {0} del {1}", linkedSeries.Id, linkedSeries.RegistrationDate.ToString("dd/MM/yyyy"), linkedSeries.DocumentSeries.Name)
                Dim parameters As String = String.Format("IdDocumentSeriesItem={0}&Action={1}&PreviousPage={2}&Type=Series", linkedSeries.Id, DocumentSeriesAction.View, HttpUtility.UrlEncode(Page.Request.Url.AbsoluteUri))
                documentSeriesLink.NavigateUrl = String.Format("~/Series/Item.aspx?{0}", CommonShared.AppendSecurityCheck(parameters))
            End If

            ' Verifico se l'Item è in stato Annullato
            If CurrentDocumentSeriesItem.Status = DocumentSeriesItemStatus.Canceled Then
                Dim logs As IList(Of DocumentSeriesItemLog) = Facade.DocumentSeriesItemLogFacade.GetByItemAndLogType(CurrentDocumentSeriesItem, DocumentSeriesItemLogType.Cancel)
                If Not logs.IsNullOrEmpty Then
                    pnlCancel.Visible = True
                    ItemCancelLabel.Text = logs.Last().LogDescription
                End If
            End If

            ' DataBind dei Dynamic Data
            DataBindAttributes(SelectedArchiveInfo, CurrentDocumentSeriesItem)

            ' Inizializazione area di Identificazione dell'Item
            If CurrentDocumentSeriesItem.Status = DocumentSeriesItemStatus.Draft Then
                rowActive.Visible = False
                rowDraft.Visible = True

                'Identificativo DocumentSeriesItem Draft: ID del RegistrationDate registrata da RegistrationUse
                lblDraftId.Text = String.Format("{0} del {1} registrata da {2}", CurrentDocumentSeriesItem.Id, CurrentDocumentSeriesItem.RegistrationDate, CurrentDocumentSeriesItem.RegistrationUser)
            Else
                rowActive.Visible = True
                rowDraft.Visible = False
                lblYear.Text = CurrentDocumentSeriesItem.Year.ToString()
                lblNumber.Text = CurrentDocumentSeriesItem.Number.ToString()
                lblRegistrationDate.Text = CurrentDocumentSeriesItem.RegistrationDate.ToString("dd/MM/yyyy")
            End If

            ' Documenti principali
            Dim docs As IList(Of DocumentInfo) = Facade.DocumentSeriesItemFacade.GetMainDocuments(CurrentDocumentSeriesItem, ProtocolEnv.DocumentSeriesReorderDocumentEnabled).Cast(Of DocumentInfo).ToList()
            If Not docs.IsNullOrEmpty() Then
                DocumentsPanel.Visible = True
                uscUploadDocument.LoadDocumentInfo(docs)
            Else
                DocumentsPanel.Visible = False
            End If

            ' Annessi
            AnnexedPanel.Visible = False
            If CurrentDocumentSeriesItem.DocumentSeries.Container.DocumentSeriesAnnexedLocation IsNot Nothing Then
                Dim annexed As IList(Of DocumentInfo) = Facade.DocumentSeriesItemFacade.GetAnnexedDocuments(CurrentDocumentSeriesItem, ProtocolEnv.DocumentSeriesReorderDocumentEnabled).Cast(Of DocumentInfo).ToList()
                If Not annexed.IsNullOrEmpty() Then
                    AnnexedPanel.Visible = True
                    uscUploadAnnexed.LoadDocumentInfo(annexed)
                End If
            End If

            UnpublishedAnnexedPanel.Visible = False
            If CurrentDocumentSeriesItem.DocumentSeries.Container.DocumentSeriesUnpublishedAnnexedLocation IsNot Nothing Then
                Dim unpublishedAnnexed As IList(Of DocumentInfo) = Facade.DocumentSeriesItemFacade.GetUnpublishedAnnexedDocuments(CurrentDocumentSeriesItem, ProtocolEnv.DocumentSeriesReorderDocumentEnabled).Cast(Of DocumentInfo).ToList()
                If Not unpublishedAnnexed.IsNullOrEmpty() Then
                    UnpublishedAnnexedPanel.Visible = True
                    uscUnpublishedAnnexed.LoadDocumentInfo(unpublishedAnnexed)
                End If
            End If

            If CurrentDocumentSeriesItemRights.IsReadable AndAlso (DocumentsPanel.Visible OrElse AnnexedPanel.Visible) Then
                cmdViewDocuments.Visible = True
            End If

            ' Descrizione 
            lblContainerArchive.Text = If(CurrentDocumentSeriesItem.DocumentSeries.Container.Archive IsNot Nothing, CurrentDocumentSeriesItem.DocumentSeries.Container.Archive.Name, "Non impostato")
            lblSeries.Text = CurrentDocumentSeriesItem.DocumentSeries.Container.Name
            ItemSubjectLabel.Text = CurrentDocumentSeriesItem.Subject

            If Not String.IsNullOrEmpty(CurrentDocumentSeriesItem.ConstraintValue) Then
                tblConstraintView.Visible = True
                lblConstraint.Text = CurrentDocumentSeriesItem.ConstraintValue
            End If

            ' Date di pubblicazione e ritiro
            If CurrentDocumentSeriesItem.DocumentSeries.PublicationEnabled.GetValueOrDefault(False) Then
                DataBindPublicationFields()
            End If
            ' Data ultima modifica
            If CurrentDocumentSeriesItem.LastChangedDate.HasValue Then
                DataBindLastEditFields()
            End If
            ' Classificazione
            If CurrentDocumentSeriesItem.SubCategory IsNot Nothing Then
                ItemCategoryCodeLabel.Text = Facade.CategoryFacade.GetCodeDotted(CurrentDocumentSeriesItem.SubCategory)
                ItemCategoryDescriptionLabel.Text = Facade.CategoryFacade.GetFullIncrementalName(CurrentDocumentSeriesItem.SubCategory)
            Else
                ItemCategoryCodeLabel.Text = CurrentDocumentSeriesItem.Category.Code.ToString()
                ItemCategoryDescriptionLabel.Text = Facade.CategoryFacade.GetFullIncrementalName(CurrentDocumentSeriesItem.Category)
            End If

            ' SUBSECTIONS
            If CurrentDocumentSeriesItem.DocumentSeries.SubsectionEnabled.GetValueOrDefault(False) AndAlso
                CurrentDocumentSeriesItem.DocumentSeriesSubsection IsNot Nothing Then
                ' Valorizzo sotto-sezione
                lblSubsection.Text = CurrentDocumentSeriesItem.DocumentSeriesSubsection.Description
            End If

            'Priorità
            chkPriority.Checked = CurrentDocumentSeriesItem.Priority.GetValueOrDefault(False)

            If CurrentDocumentSeriesItem.DocumentSeries.RoleEnabled Then

                Dim roles As IList(Of Role) = Facade.DocumentSeriesItemRoleFacade.GetAuthorizedRoles(CurrentDocumentSeriesItem)

                cmdMailSettori.Visible = CurrentDocumentSeriesItemRights.ViewRightChecked AndAlso Not roles.IsNullOrEmpty() AndAlso CurrentDocumentSeriesItem.Status = DocumentSeriesItemStatus.Active
                If cmdMailSettori.Visible Then
                    cmdMailSettori.PostBackUrl = String.Format("../MailSenders/SeriesMailSender.aspx?idSeriesItem={0}&recipients=true&Type=Series", CurrentDocumentSeriesItem.Id)
                End If

            End If

            If CurrentDocumentSeriesItem.DocumentSeries.Id = DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId Then
                cmdAVCPEditor.Visible = True
            End If

            btnNuovoMonitoraggio.Visible = False
            pnlTransparentMonitoringLog.Visible = False
            If ProtocolEnv.TransparentMonitoringEnabled Then
                pnlTransparentMonitoringLog.Visible = True
                If CommonShared.HasGroupTransparentManagerRight() Then
                    btnNuovoMonitoraggio.Visible = True
                End If
            End If

            'ROLES
            DataBindRoles()

            Facade.DocumentSeriesItemLogFacade.AddLog(CurrentDocumentSeriesItem, DocumentSeriesItemLogType.View, String.Empty)
        End Sub

        Private Sub LoadAvailableDocumentSeries()
            Dim results As IEnumerable(Of Container) = AvailableContainer.Where(Function(container) String.IsNullOrEmpty(ddlContainerArchive.SelectedValue) OrElse (container.Archive IsNot Nothing AndAlso container.Archive.Id = Integer.Parse(ddlContainerArchive.SelectedValue))).ToList()

            ddlDocumentSeries.DataValueField = "Id"
            ddlDocumentSeries.DataTextField = "Name"
            ddlDocumentSeries.DataSource = results
            ddlDocumentSeries.DataBind()
            If (Not results.IsNullOrEmpty AndAlso results.Count() = 1) Then
                ddlDocumentSeries.SelectedIndex = 0
                CurrentDocumentSeriesItem.DocumentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(Integer.Parse(ddlDocumentSeries.SelectedValue))
                ddlDocumentSeries_SelectedIndexChanged(Me.Page, Nothing)
            Else
                ddlDocumentSeries.Items.Insert(0, String.Empty)
            End If
        End Sub

        Private Sub TogglePublication(ByVal canEdit As Boolean)
            If Not CurrentDocumentSeriesItem.DocumentSeries.PublicationEnabled.GetValueOrDefault(False) Then
                tblPubblication.Visible = False
                tblLastEditDate.Visible = True
                Exit Sub
            End If

            tblPubblication.Visible = True
            tblLastEditDate.Visible = False
            If canEdit Then
                ItemPublishingDateLabel.Visible = False
                ItemRetireDateLabel.Visible = False

                ItemPublishingDate.Visible = True
                ItemPublishingDate.Enabled = CurrentDocumentSeriesItemRights.IsAdmin()
                ItemRetireDate.Visible = True
                ItemRetireDate.Enabled = True ' CurrentDocumentSeriesItemRights.IsAdmin()

                chkPriority.Enabled = True ' CurrentDocumentSeriesItemRights.IsAdmin()
            Else
                ItemPublishingDateLabel.Visible = True
                ItemRetireDateLabel.Visible = True

                ItemPublishingDate.Visible = False
                ItemRetireDate.Visible = False

                chkPriority.Enabled = False
            End If

        End Sub

        ''' <summary> Inizializza la pagina per la modifica </summary>
        Private Sub InitializeActionEdit()
            If Not CurrentDocumentSeriesItemRights.IsEditable Then
                Throw New DocSuiteException("Impossibile modificare", "Verificare se si dispone di autorizzazioni necessarie alla modifica.")
            End If

            FileLogger.Debug(LoggerName, String.Format("Modifica Serie Documentale: {0}", CurrentDocumentSeriesItem.Id))

            ' Visibilità componenti
            ContentWrapper.SetDisplay(True)
            tblSeriesView.Visible = True
            tblIdentification.Visible = True
            ItemObjectTable.Visible = True
            CategoryPanel.Visible = True
            AnnexedPanel.Visible = CurrentDocumentSeriesItem.DocumentSeries.Container.DocumentSeriesAnnexedLocation IsNot Nothing
            'tblPriorityEdit.Visible = True
            UnpublishedAnnexedPanel.Visible = CurrentDocumentSeriesItem.DocumentSeries.Container.DocumentSeriesUnpublishedAnnexedLocation IsNot Nothing

            VisibilityPublicationButtons()
            TogglePublication(True)

            pnlRoles.Visible = CurrentDocumentSeriesItem.DocumentSeries.RoleEnabled.GetValueOrDefault(False)

            cmdCancel.Visible = CurrentDocumentSeriesItemRights.IsDeletable
            txtCancelRequired.Enabled = (CurrentDocumentSeriesItem.Status = DocumentSeriesItemStatus.Active)

            cmdOkEdit.Visible = True

            ' Diritti di Assegnazione Numero a item in stato bozza
            cmdAssignNumber.Visible = CurrentDocumentSeriesItemRights.IsInsertable AndAlso CurrentDocumentSeriesItem.Status = DocumentSeriesItemStatus.Draft

            InitializeDocumentsInputs(uscUploadDocument, Not CurrentDocumentSeriesItem.DocumentSeries.AllowNoDocument.GetValueOrDefault(False), CurrentDocumentSeriesItem.DocumentSeries.AllowAddDocument.GetValueOrDefault(False))
            InitializeDocumentsInputs(uscUploadAnnexed, False, True)
            InitializeDocumentsInputs(uscUnpublishedAnnexed, False, True)

            ItemSubject.ReadOnly = False
            ItemSubCategory.ReadOnly = False
            ItemCategory.ReadOnly = True

            InitializeLogButton(CurrentDocumentSeriesItem)

            ' Priorità
            chkPriority.Checked = CurrentDocumentSeriesItem.Priority.GetValueOrDefault(False)

            ' Data ultima modifica
            If CurrentDocumentSeriesItem.LastChangedDate.HasValue Then
                DataBindLastEditFields()
            End If

            ' DataBind dei Dynamic Data
            DataBindAttributes(SelectedArchiveInfo, CurrentDocumentSeriesItem)

            If CurrentDocumentSeriesItem.DocumentSeries.PublicationEnabled.GetValueOrDefault(False) Then
                DataBindPublicationFields()
            End If

            ' Documenti principali
            Dim docs As IList(Of DocumentInfo) = Facade.DocumentSeriesItemFacade.GetMainDocuments(CurrentDocumentSeriesItem, ProtocolEnv.DocumentSeriesReorderDocumentEnabled).Cast(Of DocumentInfo).ToList()
            uscUploadDocument.LoadDocumentInfo(docs)

            ' Annessi
            If CurrentDocumentSeriesItem.DocumentSeries.Container.DocumentSeriesAnnexedLocation IsNot Nothing Then
                Dim annexed As IList(Of DocumentInfo) = Facade.DocumentSeriesItemFacade.GetAnnexedDocuments(CurrentDocumentSeriesItem, ProtocolEnv.DocumentSeriesReorderDocumentEnabled).Cast(Of DocumentInfo).ToList()
                uscUploadAnnexed.LoadDocumentInfo(annexed)
            End If

            If CurrentDocumentSeriesItem.DocumentSeries.Container.DocumentSeriesUnpublishedAnnexedLocation IsNot Nothing Then
                Dim unpublishedAnnexed As IList(Of DocumentInfo) = Facade.DocumentSeriesItemFacade.GetUnpublishedAnnexedDocuments(CurrentDocumentSeriesItem, ProtocolEnv.DocumentSeriesReorderDocumentEnabled).Cast(Of DocumentInfo).ToList()
                uscUnpublishedAnnexed.LoadDocumentInfo(unpublishedAnnexed)
            End If

            ' Inizializazione area di Identificazione dell'Item
            If CurrentDocumentSeriesItem.Status = DocumentSeriesItemStatus.Draft Then
                rowActive.Visible = False
                rowDraft.Visible = True

                ' Identificativo DocumentSeriesItem Draft: ID del RegistrationDate registrata da RegistrationUse
                lblDraftId.Text = String.Format("{0} del {1} registrata da {2}", CurrentDocumentSeriesItem.Id, CurrentDocumentSeriesItem.RegistrationDate, CurrentDocumentSeriesItem.RegistrationUser)
            Else
                rowActive.Visible = True
                rowDraft.Visible = False
                lblYear.Text = CurrentDocumentSeriesItem.Year.ToString()
                lblNumber.Text = CurrentDocumentSeriesItem.Number.ToString()
                lblRegistrationDate.Text = CurrentDocumentSeriesItem.RegistrationDate.ToString("dd/MM/yyyy")
            End If

            lblSeries.Text = CurrentDocumentSeriesItem.DocumentSeries.Container.Name

            ItemSubject.Text = CurrentDocumentSeriesItem.Subject
            ItemCategory.DataSource.Add(CurrentDocumentSeriesItem.Category)
            ItemCategory.DataBind()
            If CurrentDocumentSeriesItem.SubCategory IsNot Nothing Then
                ItemSubCategory.SubCategory = CurrentDocumentSeriesItem.SubCategory
            End If
            ItemSubCategory.CategoryID = CurrentDocumentSeriesItem.Category.Id

            If CurrentDocumentSeriesItem.DocumentSeries.SubsectionEnabled.GetValueOrDefault(False) Then
                tblSubsection.Visible = True
                ' Carico le Subsections 
                DataBindAvailableSubsection()
                ' imposto quella selezionata
                If CurrentDocumentSeriesItem.DocumentSeriesSubsection IsNot Nothing Then
                    ddlSubsection.SelectedValue = CurrentDocumentSeriesItem.DocumentSeriesSubsection.Id.ToString()
                End If
            Else
                tblSubsection.Visible = False
            End If

            LoadConstraints()
            If Not String.IsNullOrEmpty(CurrentDocumentSeriesItem.ConstraintValue) Then
                Dim constraint As ListItem = ddlConstraints.Items.FindByText(CurrentDocumentSeriesItem.ConstraintValue)
                If constraint IsNot Nothing Then
                    ddlConstraints.SelectedValue = constraint.Value
                End If
            End If

            DataBindRoles()

        End Sub

        Private Sub InitializeRoles()
            If uscRoleOwner.RoleListAdded.Count <> 0 OrElse (CurrentDocumentSeriesItem Is Nothing) OrElse (CurrentDocumentSeriesItem.DocumentSeries Is Nothing) OrElse Not CurrentDocumentSeriesItem.DocumentSeries.RoleEnabled.GetValueOrDefault(False) Then
                Exit Sub
            End If
            uscRoleOwner.Required = ProtocolEnv.SeriesItemOwnerRoleRequired
            If ProtocolEnv.OwnerRolesInSession AndAlso OwnerRolesInSession IsNot Nothing Then
                ' Nuovo inserimento, imposto i valori di default
                uscRoleOwner.SourceRoles.Clear()
                uscRoleOwner.AddRoles(Facade.RoleFacade.GetByIds(OwnerRolesInSession), True, False, False)
            Else
                ' Nuovo inserimento, imposto i valori di default
                uscRoleOwner.SourceRoles.Clear()

                Select Case ProtocolEnv.RoleOwnerDefaultMode
                    Case ProtocolEnv.RoleOwnerDefaultModeEnum.All
                        ' Nuovo inserimento, imposto i valori di default
                        uscRoleOwner.SourceRoles.Clear()
                        uscRoleOwner.AddRoles(MyRoles, True, False, False)
                    Case ProtocolEnv.RoleOwnerDefaultModeEnum.OnlyIfSingle
                        If MyRoles.Count = 1 Then
                            ' Nuovo inserimento, imposto i valori di default
                            uscRoleOwner.SourceRoles.Clear()
                            uscRoleOwner.AddRoles(MyRoles, True, False, False)
                        End If
                    Case ProtocolEnv.RoleOwnerDefaultModeEnum.None
                        Return ' nothing to do here
                End Select
            End If
        End Sub

        Private Sub DataBindRoles()
            If (CurrentDocumentSeriesItem Is Nothing) OrElse (CurrentDocumentSeriesItem.DocumentSeries Is Nothing) OrElse Not CurrentDocumentSeriesItem.DocumentSeries.RoleEnabled.GetValueOrDefault(False) Then
                Exit Sub
            End If

            uscRoleOwner.SourceRoles = Facade.DocumentSeriesItemRoleFacade.GetOwnerRoles(CurrentDocumentSeriesItem).ToList()
            uscRoleOwner.DataBind()

            uscRoleAuthorization.SourceRoles = Facade.DocumentSeriesItemRoleFacade.GetAuthorizedRoles(CurrentDocumentSeriesItem).ToList()
            uscRoleAuthorization.DataBind()
        End Sub

        Private Sub DataBindLastEditFields()
            If CurrentDocumentSeriesItem.LastChangedDate.HasValue Then
                ItemDateEditLabel.Text = String.Format("{0:dd/MM/yyyy} ({1})", CurrentDocumentSeriesItem.LastChangedDate, CurrentDocumentSeriesItem.LastChangedUser)
                ItemDateEditLabelPubblicationArea.Text = ItemDateEditLabel.Text
            End If
        End Sub

        Private Sub DataBindPublicationFields()
            If Editable Then
                If CurrentDocumentSeriesItem.PublishingDate.HasValue Then
                    ItemPublishingDate.SelectedDate = CurrentDocumentSeriesItem.PublishingDate.Value
                    If ProtocolEnv.DocumentSeriesPublicationDateConstraintEnabled Then
                        ItemPublishingDate.Enabled = False
                    End If
                End If
                If CurrentDocumentSeriesItem.RetireDate.HasValue Then
                    ItemRetireDate.SelectedDate = CurrentDocumentSeriesItem.RetireDate.Value
                End If
            Else
                If CurrentDocumentSeriesItem.PublishingDate.HasValue Then
                    ItemPublishingDateLabel.Text = CurrentDocumentSeriesItem.PublishingDate.Value.ToString("dd/MM/yyyy")
                End If
                If CurrentDocumentSeriesItem.RetireDate.HasValue Then
                    ItemRetireDateLabel.Text = CurrentDocumentSeriesItem.RetireDate.Value.ToString("dd/MM/yyyy")
                End If
            End If
        End Sub

        Private Sub VisibilityPublicationButtons()
            If Not CurrentDocumentSeriesItem.DocumentSeries.PublicationEnabled.GetValueOrDefault(False) Then
                Return
            End If
            cmdPublish.Visible = CurrentDocumentSeriesItemRights.IsPublishable()
            cmdRetire.Visible = CurrentDocumentSeriesItemRights.IsRetirable()
        End Sub

        Private Sub VisibilityFlushButtons()
            cmdFlushDocs.Visible = CurrentDocumentSeriesItem.IdMain <> Guid.Empty AndAlso CurrentDocumentSeriesItemRights.IsEditable
            cmdFlushAnnexed.Visible = CurrentDocumentSeriesItem.IdAnnexed <> Guid.Empty AndAlso CurrentDocumentSeriesItemRights.IsEditable
            cmdFlushUnpublishedAnnexed.Visible = CurrentDocumentSeriesItem.IdUnpublishedAnnexed <> Guid.Empty AndAlso CurrentDocumentSeriesItemRights.IsEditable
        End Sub

        ''' <summary> Inizializza le componenti non-dinamiche della pagina. </summary>
        ''' <remarks>Da eseguire una volta sola</remarks>
        Private Sub InitializeForInsert()
            ' Visibilità componenti
            tblSeries.Visible = True
            ItemObjectTable.Visible = True
            CategoryPanel.Visible = True
            AnnexedPanel.Visible = True
            ItemSubCategory.Visible = False
            cmdOk.Visible = True
            cmdSaveDraft.Visible = Action <> DocumentSeriesAction.FromCollaboration
            uscUploadDocument.SignButtonEnabled = True
            uscUploadAnnexed.SignButtonEnabled = True
            uscUnpublishedAnnexed.SignButtonEnabled = True
            chkPriority.Checked = CurrentDocumentSeriesItem.Priority.GetValueOrDefault(ProtocolEnv.PrimoPianoDefaultEnabled)
            LoadDocumentSeries()
        End Sub

        Private Function GetBehaviourField(m As Match) As String
            If m.Groups("field") IsNot Nothing Then
                Return m.Groups("field").Value
            End If
            Return Nothing
        End Function

        Private Function GetBehaviourValue(m As Match) As String
            If m.Groups("value") IsNot Nothing Then
                Return m.Groups("value").Value
            End If
            Return Nothing
        End Function

        Private Function GetBehaviourFormat(m As Match) As String
            If m.Groups("format") IsNot Nothing Then
                Return m.Groups("format").Value
            End If
            Return Nothing
        End Function

        Private Function GetBehaviourFiedType(m As Match) As String
            If m.Groups("fieldtype") IsNot Nothing Then
                Return m.Groups("fieldtype").Value
            End If
            Return Nothing
        End Function

        Private Function ReplaceTarget(ByVal m As Match) As String
            Dim behaviourFormat As String = GetBehaviourFormat(m)

            Dim val As Object = GetMatchValue(m)

            If Not String.IsNullOrEmpty(behaviourFormat) Then
                Dim f As String = String.Format("{{0:{0}}}", behaviourFormat)
                Return String.Format(f, val)
            Else
                Return String.Format("{0}", val)
            End If
        End Function

        Private Function GetMatchValue(m As Match) As Object
            Dim behaviourField As String = GetBehaviourField(m)
            Dim val As Object = Nothing
            Select Case GetBehaviourFiedType(m)
                Case "%" ' Calcolato
                    Select Case behaviourField.ToLowerInvariant()
                        Case "today"
                            val = DateTime.Today
                        Case "now"
                            val = DateTime.Now
                    End Select
                Case "@" ' Attributo
                    Dim attributes As Dictionary(Of String, String) = Facade.DocumentSeriesItemFacade.GetAttributes(CurrentDocumentSeriesItem)
                    If attributes.ContainsKey(behaviourField) Then
                        val = attributes.Item(behaviourField)
                    End If
                Case "#" ' Da Page
                    Dim control As Control = Page.FindControlRecursive(behaviourField)
                    If control IsNot Nothing Then
                        val = GetControlValue(control)
                    End If
                Case "§" ' Da Page Dynamic
                    Dim control As Control
                    Dim guid As Guid? = SelectedArchiveInfo.GetAttributeGuid(behaviourField)
                    If guid.HasValue Then
                        control = Page.FindControlRecursive(guid.Value.ToString("N"))
                    End If
                    If control IsNot Nothing Then
                        val = GetControlValue(control)
                    End If
                Case "$" ' Da Item
                    If CurrentDocumentSeriesItem IsNot Nothing Then
                        val = ReflectionHelper.GetProperty(CurrentDocumentSeriesItem, behaviourField)
                    End If
            End Select

            Dim behaviourValue As String = GetBehaviourValue(m)
            If Not String.IsNullOrEmpty(behaviourValue) Then
                Dim i As Integer = 0
                If Integer.TryParse(behaviourValue, i) Then
                    Select Case True
                        Case TypeOf val Is DateTime
                            val = CType(val, DateTime).AddDays(i)
                        Case TypeOf val Is Date
                            val = CType(val, Date).AddDays(i)
                        Case TypeOf val Is Integer
                            val = CType(val, Integer) + i
                    End Select
                End If
            End If

            Return val
        End Function

        Private Sub ApplyBehaviours(behaviours As IList(Of DocumentSeriesAttributeBehaviour))
            Try
                For Each behaviour As DocumentSeriesAttributeBehaviour In behaviours
                    ' Cerco il Target
                    Dim target As Control

                    Select Case True
                        Case behaviour.AttributeName.StartsWith("#") ' ID controllo
                            target = Page.FindControlRecursive(behaviour.AttributeName.Substring(1))
                        Case behaviour.AttributeName.StartsWith("§") ' Controllo, Nome dell'attributo
                            Dim guid As Guid? = SelectedArchiveInfo.GetAttributeGuid(behaviour.AttributeName.Substring(1))
                            If guid.HasValue Then
                                target = Page.FindControlRecursive(guid.Value.ToString("N"))
                            Else
                                ' NOOP
                                FileLogger.Warn(LoggerName, String.Format("Attributo '{0}' non trovato in Archivio {1}.", behaviour.AttributeName, SelectedArchiveInfo.Name))
                            End If
                        Case behaviour.AttributeName.StartsWith("@") ' Attributo
                            ' TODO
                        Case behaviour.AttributeName.StartsWith("$") ' Item, via Reflection
                            ' TODO
                    End Select

                    If target Is Nothing Then
                        FileLogger.Warn(LoggerName, String.Format("Target '{0}' non trovato in Archivio {1}.", behaviour.AttributeName, SelectedArchiveInfo.Name))
                        Continue For
                    End If

                    ' Verifico se il target è valorizzato già
                    Dim myValue As Object = GetControlValue(target)
                    If myValue IsNot Nothing AndAlso behaviour.KeepValue Then
                        Continue For ' Campo già valorizzato, passo al successivo
                    End If

                    Dim setValue As Object = Nothing
                    Dim r As New Regex(ProtocolEnv.BehaviourRegex, RegexOptions.IgnorePatternWhitespace)

                    Select Case behaviour.ValueType
                        Case DocumentSeriesAttributeBehaviourValueType.ConstantValue
                            ' Valore costante di tipo Stringa
                            setValue = behaviour.AttributeValue
                        Case DocumentSeriesAttributeBehaviourValueType.Calculated
                            Dim m As Match = r.Match(behaviour.AttributeValue)
                            If m.Groups IsNot Nothing AndAlso m.Groups.Count > 0 Then
                                setValue = GetMatchValue(m)
                            End If
                        Case DocumentSeriesAttributeBehaviourValueType.Pattern
                            ' Valore stringa Regex
                            Dim myEvaluator As New MatchEvaluator(AddressOf ReplaceTarget)
                            setValue = r.Replace(behaviour.AttributeValue, myEvaluator)
                    End Select

                    ' Se setValue è di tipo stringa eseguo il Format verso Resolution (se attivo)
                    If TypeOf setValue Is String AndAlso Action = DocumentSeriesAction.FromResolution Then
                        setValue = String.Format(CType(setValue, String), New FormattableResolution(CurrentResolution))
                    End If

                    ' Imposto il valore
                    SetControlValue(target, setValue, "{0}")
                Next
            Catch ex As Exception
                AjaxAlert("Errore in esecuzione comando.")
                FileLogger.Error(LoggerName, "Errore in esecuzione comando.", ex)
            End Try
        End Sub

        Private Sub InitializeDocumentsInputs(control As uscDocumentUpload, required As Boolean, allowAdd As Boolean)
            control.IsDocumentRequired = required
            control.ButtonScannerEnabled = allowAdd
            control.ButtonCopySeries.Visible = allowAdd AndAlso ProtocolEnv.CopyFromSeries
            control.ButtonCopyProtocol.Visible = allowAdd AndAlso ProtocolEnv.CopyProtocolDocumentsEnabled
            control.ButtonCopyUDS.Visible = allowAdd AndAlso ProtocolEnv.UDSEnabled

            If DocSuiteContext.Current.IsResolutionEnabled Then
                control.ButtonCopyResl.Visible = allowAdd AndAlso ResolutionEnv.CopyReslDocumentsEnabled
            End If

        End Sub

        ''' <summary>
        ''' Recupera l'elenco dei Behaviours VISIBILE e per ogni gruppo aggiunge un pulsante che scatena la relativa Request
        ''' </summary>
        Private Sub InitializeBehaviours()
            Dim groups As IList(Of String) = Facade.DocumentSeriesAttributeBehaviourFacade.GetAttributeBehaviourGroups(CurrentDocumentSeriesItem.DocumentSeries, Action)

            For Each group As String In groups
                Dim btn As New Button
                btn.CommandName = "AttributeBehaviour"
                btn.CssClass = If(group.Length >= 15, "ButtonLarge", "ButtonShort")
                btn.CommandArgument = group
                btn.Text = group
                btn.CausesValidation = False
                btn.OnClientClick = String.Format("return AjaxRequest('{0}');", group)
                BehavioursPlaceHolder.Controls.Add(btn)

                BehavioursPlaceHolder.Controls.Add(New LiteralControl(WebHelper.Space))
            Next

        End Sub

        Private Sub InitializeLogButton(item As DocumentSeriesItem)
            btnLog.Visible = String.IsNullOrWhiteSpace(DocSuiteContext.Current.ProtocolEnv.DocumentSeriesLogGroups) _
                OrElse CommonShared.UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.DocumentSeriesLogGroups)
            btnLog.PostBackUrl = "~/Series/ItemLog.aspx?" & CommonShared.AppendSecurityCheck(String.Format("IdDocumentSeriesItem={0}&Type=Series", item.Id))
        End Sub

        Private Sub FillDataFromPage(chain As BiblosChainInfo, item As DocumentSeriesItem)
            For Each attribute As ArchiveAttribute In SelectedArchiveInfo.Attributes
                Dim control As Control = DynamicControls.FindControlRecursive(attribute.Id.ToString("N"))

                If control Is Nothing Then Continue For ' Controllo non trovato nella pagina

                Dim val As String = GetControlValueFormatted(control, attribute.Format) ' Recupero il valore del controllo

                chain.AddAttribute(attribute.Name, val)
            Next

            item.PublishingDate = ItemPublishingDate.SelectedDate
            item.RetireDate = ItemRetireDate.SelectedDate
            item.DocumentSeries = CurrentDocumentSeriesItem.DocumentSeries
            item.Subject = ItemSubject.Text

        End Sub

        Private Sub DataBindAttributes(ByVal archive As ArchiveInfo, ByVal item As DocumentSeriesItem)
            Dim attributes As Dictionary(Of String, String) = Facade.DocumentSeriesItemFacade.GetAttributes(item)
            For Each a As ArchiveAttribute In archive.VisibleChainAttributes
                If Not attributes.ContainsKey(a.Name) Then
                    Continue For
                End If

                Dim val As Object = GetAttributeValue(attributes, a)
                If val Is Nothing Then
                    Continue For
                End If

                Dim control As Control = DynamicControls.FindControl(a.Id.ToString("N"))
                If (TypeOf control Is Label) AndAlso (TypeOf val Is Long) Then
                    Dim tempInt As Integer = DirectCast(val, Long)
                    Dim desc As String = (New DocumentSeriesAttributeEnumFacade).GetValueDescription(item.DocumentSeries.Id, a.Name, tempInt)
                    If Not String.IsNullOrEmpty(desc) Then
                        val = desc
                    End If
                End If

                SetControlValue(control, val, a.Format)
            Next
        End Sub

        ''' <summary> Popola la pagina con i campi dinamici della Serie </summary>
        Private Sub LoadAttributesControls(archive As ArchiveInfo)
            ' Metodo per cancellazione dei dati in ViewState dei controlli figli di DynamicControl
            DynamicControls.Controls.Clear()

            Dim attributes As List(Of ArchiveAttribute) = archive.VisibleChainAttributes
            If attributes.Count = 0 Then
                Exit Sub
            End If

            Dim table As New Table
            table.CssClass = "datatable"
            table.EnableViewState = False
            DynamicControls.Controls.Add(table)

            Dim tr As New TableHeaderRow
            table.Rows.Add(tr)

            Dim th As New TableHeaderCell
            th.Text = "Dati dinamici"
            th.ColumnSpan = 2
            tr.Cells.Add(th)

            Dim attributeEnums As List(Of DocumentSeriesAttributeEnum) = (New DocumentSeriesAttributeEnumFacade).GetByDocumentSeries(CurrentDocumentSeriesItem.DocumentSeries.Id).ToList()

            For Each a As ArchiveAttribute In archive.VisibleChainAttributes
                If attributeEnums.Exists(Function(ae) ae.AttributeName.Eq(a.Name)) Then
                    Dim ae As DocumentSeriesAttributeEnum = attributeEnums.Single(Function(x) x.AttributeName.Eq(a.Name))
                    Select Case ae.EnumType
                        Case AttributeEnumTypes.Checkbox
                            table.Rows.Add(GetCheckboxStructure(a, ae))
                        Case AttributeEnumTypes.Combo
                            table.Rows.Add(GetComboStructure(a, ae))
                    End Select

                Else
                    'todo: prevedere l'utilizzo dei nuovi helper HTML per le strutture dinamiche
                    Select Case a.DataType
                        Case "System.String"
                            table.Rows.Add(GetStringStructure(a))
                        Case "System.DateTime"
                            table.Rows.Add(GetDateTimeStructure(a))
                        Case "System.Int64"
                            table.Rows.Add(GetInt64Structure(a))
                        Case "System.Double"
                            table.Rows.Add(GetDoubleStructure(a))
                    End Select

                End If
            Next

        End Sub

        Private Function GetBasicStructure(label As String, innerControls As List(Of WebControl)) As TableRow

            Dim row As New TableRow()
            row.CssClass = "Chiaro"
            row.EnableViewState = False

            Dim cell1 As New TableCell()
            cell1.CssClass = "DocumentSeriesLabel"
            cell1.Text = String.Concat(label, ":")
            cell1.EnableViewState = False
            row.Cells.Add(cell1)

            Dim cell2 As New TableCell
            For Each item As WebControl In innerControls
                cell2.Controls.Add(item)
            Next
            cell2.EnableViewState = False
            row.Cells.Add(cell2)

            Return row
        End Function

        Private Function GetComboStructure(a As ArchiveAttribute, ae As DocumentSeriesAttributeEnum) As TableRow
            Dim itemsToAdd As List(Of WebControl) = New List(Of WebControl)

            If IsEditable(a, Action, Editable) Then
                Dim cb As New DropDownList()
                cb.ID = a.Id.ToString("N")
                cb.Width = New Unit(350, UnitType.Pixel)
                cb.Enabled = Editable

                cb.Items.Add(New ListItem(String.Empty, "-1"))

                For Each item As DocumentSeriesAttributeEnumValue In ae.EnumValues
                    cb.Items.Add(New ListItem(item.Description, item.AttributeValue.ToString()))
                Next

                itemsToAdd.Add(cb)
                ' TODO: gestire REQUIRED e valore di default
            Else
                Dim txt As New Label()
                txt.ID = a.Id.ToString("N")
                txt.Text = CType(ViewState(txt.ID), String)
                itemsToAdd.Add(txt)
            End If

            Return GetBasicStructure(a.Label, itemsToAdd)
        End Function

        Private Function GetCheckboxStructure(a As ArchiveAttribute, ae As DocumentSeriesAttributeEnum) As TableRow
            Dim itemsToAdd As List(Of WebControl) = New List(Of WebControl)

            If IsEditable(a, Action, Editable) Then
                Dim cb As New CheckBox()
                cb.ID = a.Id.ToString("N")
                cb.Width = New Unit(100, UnitType.Percentage)
                cb.Enabled = Editable
                itemsToAdd.Add(cb)

                ' TODO: gestire REQUIRED e valore di default
            Else
                Dim txt As New Label()
                txt.ID = a.Id.ToString("N")
                txt.Text = CType(ViewState(txt.ID), String)
                itemsToAdd.Add(txt)
            End If

            Return GetBasicStructure(a.Label, itemsToAdd)
        End Function

        Private Function GetStringStructure(a As ArchiveAttribute) As TableRow
            Dim itemsToAdd As List(Of WebControl) = New List(Of WebControl)

            If IsEditable(a, Action, Editable) Then
                Dim txt As New RadTextBox()
                txt.ID = a.Id.ToString("N")
                txt.Width = New Unit(100, UnitType.Percentage)
                txt.ReadOnly = Not Editable
                txt.TextMode = InputMode.MultiLine
                itemsToAdd.Add(txt)

                If a.Required Then
                    itemsToAdd.Add(GetValidatorStructure(a.Label, txt.UniqueID))
                End If
                If ActionNewItem Then
                    If Not String.IsNullOrEmpty(a.DefaultValue) Then
                        SetControlValue(txt, a.DefaultValue, a.Format)
                    End If
                End If
            Else
                Dim txt As New Label()
                txt.ID = a.Id.ToString("N")
                txt.Text = CType(ViewState(txt.ID), String)
                itemsToAdd.Add(txt)
                txt.CssClass = "BreakColumn"
            End If

            Return GetBasicStructure(a.Label, itemsToAdd)
        End Function

        Private Function GetDateTimeStructure(a As ArchiveAttribute) As TableRow
            Dim itemsToAdd As List(Of WebControl) = New List(Of WebControl)

            If IsEditable(a, Action, Editable) Then
                Dim txt As New RadDatePicker()
                txt.ID = a.Id.ToString("N")
                txt.CssClass = "BreakColumn"

                itemsToAdd.Add(txt)
                If a.Required Then
                    itemsToAdd.Add(GetValidatorStructure(a.Label, txt.UniqueID))
                End If

                If ActionNewItem Then
                    If Not String.IsNullOrEmpty(a.DefaultValue) Then
                        SetControlValue(txt, a.DefaultValue, a.Format)
                    End If
                End If

            Else
                Dim txt As New Label()
                txt.ID = a.Id.ToString("N")
                txt.Text = CType(ViewState(txt.ID), String)
                itemsToAdd.Add(txt)
                txt.CssClass = "BreakColumn"
            End If

            Return GetBasicStructure(a.Label, itemsToAdd)
        End Function

        Private Function GetInt64Structure(a As ArchiveAttribute) As TableRow
            Dim itemsToAdd As List(Of WebControl) = New List(Of WebControl)

            If IsEditable(a, Action, Editable) Then
                Dim txt As New RadNumericTextBox()
                txt.NumberFormat.DecimalDigits = 0
                txt.ID = a.Id.ToString("N")
                txt.ReadOnly = Not Editable
                txt.Width = 150

                itemsToAdd.Add(txt)

                If a.Required Then
                    itemsToAdd.Add(GetValidatorStructure(a.Label, txt.UniqueID))
                End If

                If ActionNewItem Then
                    If Not String.IsNullOrEmpty(a.DefaultValue) Then
                        SetControlValue(txt, a.DefaultValue, a.Format)
                    End If
                End If
            Else
                Dim txt As New Label()
                txt.ID = a.Id.ToString("N")
                txt.Text = CType(ViewState(txt.ID), String)
                itemsToAdd.Add(txt)
            End If
            Return GetBasicStructure(a.Label, itemsToAdd)

        End Function

        Private Function GetDoubleStructure(a As ArchiveAttribute) As TableRow
            Dim itemsToAdd As New List(Of WebControl)

            If IsEditable(a, Action, Editable) Then
                Dim txt As New RadNumericTextBox()
                txt.NumberFormat.DecimalDigits = 2
                txt.ID = a.Id.ToString("N")
                txt.ReadOnly = Not Editable
                txt.Width = 150

                itemsToAdd.Add(txt)
                If a.Required Then
                    itemsToAdd.Add(GetValidatorStructure(a.Label, txt.UniqueID))
                End If

                If ActionNewItem Then
                    If Not String.IsNullOrEmpty(a.DefaultValue) Then
                        SetControlValue(txt, a.DefaultValue, a.Format)
                    End If
                End If
            Else
                Dim txt As New Label()
                txt.ID = a.Id.ToString("N")
                txt.Text = CType(ViewState(txt.ID), String)
                itemsToAdd.Add(txt)
            End If
            Return GetBasicStructure(a.Label, itemsToAdd)

        End Function

        Private Function GetValidatorStructure(label As String, controlToValidate As String) As RequiredFieldValidator

            Dim validator As New RequiredFieldValidator()
            validator.ErrorMessage = String.Format("Campo {0} Obbligatorio", label)
            validator.ID = String.Concat("Validator_", controlToValidate)
            validator.Display = ValidatorDisplay.Dynamic
            validator.ControlToValidate = controlToValidate

            Return validator
        End Function

        Private Shared Function GetControlValueFormatted(source As Control, format As String) As String
            If String.IsNullOrEmpty(format) Then
                format = "{0}"
            End If

            Dim val As Object = GetControlValue(source)
            If val IsNot Nothing Then
                Return String.Format(format, val)
            End If

            Return Nothing
        End Function

        Private Shared Function GetControlValue(source As Control) As Object

            Dim checkBox As CheckBox = TryCast(source, CheckBox)
            If (checkBox IsNot Nothing) Then
                If checkBox.Checked Then
                    Return 1
                End If
                Return 0
            End If

            Dim comboBox As DropDownList = TryCast(source, DropDownList)
            If (comboBox IsNot Nothing) Then
                If String.IsNullOrEmpty(comboBox.SelectedValue) OrElse comboBox.SelectedValue = "-1" Then
                    Return Nothing
                End If
                Return Integer.Parse(comboBox.SelectedValue)
            End If

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

            Dim label As Label = TryCast(source, Label)
            If (label IsNot Nothing) AndAlso Not String.IsNullOrEmpty(label.Text) Then
                Return label.Text
            End If

            Dim classificatore As uscClassificatore = TryCast(source, uscClassificatore)
            If (classificatore IsNot Nothing) AndAlso Not classificatore.SelectedCategories.IsNullOrEmpty() Then
                Return classificatore.SelectedCategories.Item(0)
            End If

            FileLogger.Debug(LoggerName, String.Format("Controllo [{0}] in Item non gestito correttamente", source.ID))
            Return Nothing

        End Function

        Private Sub SetControlValue(control As Control, value As Object, format As String, Optional setEnable As Boolean = True)
            If String.IsNullOrEmpty(format) Then
                format = "{0}"
            End If

            Dim checkBox As CheckBox = TryCast(control, CheckBox)
            If (checkBox IsNot Nothing) Then
                If value Is Nothing Then
                    checkBox.Checked = False
                Else
                    checkBox.Checked = (DirectCast(value, Long) = 1)
                End If
                checkBox.Enabled = setEnable
            End If

            Dim comboBox As DropDownList = TryCast(control, DropDownList)
            If (comboBox IsNot Nothing) Then
                If value Is Nothing Then
                    comboBox.SelectedValue = "-1"
                Else
                    comboBox.SelectedValue = value.ToString()
                End If
                comboBox.Enabled = setEnable
            End If

            Dim radTextBox As RadTextBox = TryCast(control, RadTextBox)
            If (radTextBox IsNot Nothing) Then
                If value Is Nothing Then
                    radTextBox.Text = String.Empty
                Else
                    radTextBox.Text = DirectCast(value, String)
                    ' Calcolo delle righe da mettere nel campo testuale. Se supero i multipli di 100 lettere creo una nuova riga
                    radTextBox.Rows = Convert.ToInt32(Math.Floor(DirectCast(radTextBox.Text, String).Length / 100)) + 1
                End If
                radTextBox.ReadOnly = Not setEnable
                Return
            End If

            Dim radDatePicker As RadDatePicker = TryCast(control, RadDatePicker)
            If (radDatePicker IsNot Nothing) Then
                If value Is Nothing Then
                    radDatePicker.SelectedDate = Nothing
                Else
                    radDatePicker.SelectedDate = DirectCast(value, DateTime)
                End If
                radDatePicker.Enabled = setEnable
                Return
            End If

            Dim radNumericTextBox As RadNumericTextBox = TryCast(control, RadNumericTextBox)
            If (radNumericTextBox IsNot Nothing) Then

                If value Is Nothing Then
                    radNumericTextBox.Value = Nothing
                Else
                    If value Is Nothing Then
                        radNumericTextBox.Value = Nothing
                    Else
                        radNumericTextBox.Value = DirectCast(value, Long)
                    End If
                End If
                radNumericTextBox.Enabled = setEnable
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
                label.Enabled = setEnable
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

        Private Shared Function GetAttributeValue(attributes As Dictionary(Of String, String), searched As ArchiveAttribute) As Object
            If Not attributes.ContainsKey(searched.Name) Then
                Return Nothing
            End If

            Dim val As String = attributes.Item(searched.Name)

            Select Case searched.DataType
                Case "System.String"
                    Return val
                Case "System.DateTime"
                    Return DateTime.Parse(val)
                Case "System.Int64"
                    Return Int64.Parse(val)
                Case "System.Double"
                    Return Double.Parse(val)
            End Select

            Return Nothing
        End Function

        Private Shared Function IsEditable(attribute As ArchiveAttribute, action As DocumentSeriesAction, editable As Boolean) As Boolean
            If Not editable Then
                Return False
            End If

            Select Case attribute.Mode
                Case ArchiveAttributePermissionMode.ModifyAlways
                    Return True
                Case ArchiveAttributePermissionMode.ModifyIfEmpty
                    Return True ' TODO recuperare valore
                Case ArchiveAttributePermissionMode.ModifyIfNotArchived
                    Return True ' TODO verificare archiviazione
                Case ArchiveAttributePermissionMode.ReadOnlyAfterInsert
                    Return action = DocumentSeriesAction.Insert OrElse action = DocumentSeriesAction.FromResolution OrElse action = DocumentSeriesAction.Duplicate OrElse action = DocumentSeriesAction.FromCollaboration OrElse action = DocumentSeriesAction.FromResolutionKind OrElse DocumentSeriesAction.FromResolutionKindUpdate
            End Select

            Return False

        End Function

        Private Sub DataBindAvailableSubsection()
            ' Gestisco le sotto-sezioni
            If CurrentDocumentSeriesItem.DocumentSeries IsNot Nothing AndAlso CurrentDocumentSeriesItem.DocumentSeries.SubsectionEnabled Then
                tblSubsection.Visible = True
                validatorSubsection.Enabled = True
                Dim subsections As IList(Of DocumentSeriesSubsection) = Facade.DocumentSeriesSubsectionFacade.GetByDocumentSeries(CurrentDocumentSeriesItem.DocumentSeries)
                ddlSubsection.DataSource = subsections
                ddlSubsection.DataBind()
                ddlSubsection.Items.Insert(0, New ListItem(String.Empty, String.Empty))
                ddlSubsection.SelectedIndex = 0
            End If
        End Sub

        Private Sub LoadConstraints()
            If CurrentDocumentSeriesItem.DocumentSeries IsNot Nothing Then
                tblConstraints.Visible = True

                Dim results As ICollection(Of WebAPIDto(Of DocumentArchives.DocumentSeriesConstraint)) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentDocumentSeriesConstraintFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.EnablePaging = False
                        finder.IdSeries = CurrentDocumentSeriesItem.DocumentSeries.Id
                        Return finder.DoSearch()
                    End Function)

                If results Is Nothing OrElse results.Count = 0 Then
                    tblConstraints.Visible = False
                    Return
                End If

                ddlConstraints.DataSource = results.Select(Function(s) s.Entity)
                ddlConstraints.DataBind()
                ddlConstraints.Items.Insert(0, New ListItem(String.Empty, String.Empty))
                ddlConstraints.SelectedIndex = 0
            End If
        End Sub

        Private Sub GoToEdit()
            Response.Redirect(String.Format("~/Series/Item.aspx?Type=Series&Action={0}&IdDocumentSeriesItem={1}", DocumentSeriesAction.Edit, CurrentDocumentSeriesItem.Id))
        End Sub

        Private Sub GoToResolution()
            Response.Redirect("~/Resl/ReslInserimento.aspx?Type=Resl&Action=FromSeries")
        End Sub

        Private Sub GoToResolutionUpdate()
            Response.Redirect(String.Concat("~/Resl/ReslModifica.aspx?Type=Resl&Action=FromSeries&IdResolution=", CurrentResolutionId))
        End Sub

        Private Sub GoToResolutionView()
            DraftSeriesItemAdded = Nothing
            Response.Redirect(String.Concat("~/Resl/ReslVisualizza.aspx?Type=Resl&Action=FromSeries&IdResolution=", CurrentResolutionId))
        End Sub

        Private Sub GoToView(item As DocumentSeriesItem)
            Response.Redirect(String.Format("~/Series/Item.aspx?Type=Series&Action={0}&IdDocumentSeriesItem={1}", DocumentSeriesAction.View, item.Id))
        End Sub

        Private Sub GeToSearch()
            Response.Redirect("~/Series/Search.aspx?Type=Series")
        End Sub

        Private Sub GoToView()
            GoToView(CurrentDocumentSeriesItem)
        End Sub

        Private Sub LoadDocumentSeries(Optional fromIndexChanged As Boolean = False)
            If CurrentDocumentSeriesItem.DocumentSeries Is Nothing Then
                BehavioursPlaceHolder.Controls.Clear()
                Exit Sub
            End If

            ' Assicuro che sia selezionato il tipo archivio corrispondente
            If CurrentDocumentSeriesItem.DocumentSeries.Container.Archive IsNot Nothing Then
                ddlContainerArchive.SelectedValue = CurrentDocumentSeriesItem.DocumentSeries.Container.Archive.Id.ToString()
            End If

            ' Popolo l'elenco delle Sottosezioni e degli obblighi
            DataBindAvailableSubsection()
            LoadConstraints()
            ' Inizializzo l'elenco dei Settori
            InitializeRoles()
            ' Applico i valori di default
            ApplyBehaviours(Facade.DocumentSeriesAttributeBehaviourFacade.GetAttributeBehaviours(CurrentDocumentSeriesItem.DocumentSeries, Action, "DEFAULT"))

            ' Nascondo gli annessi se non gestiti
            AnnexedPanel.Visible = CurrentDocumentSeriesItem.DocumentSeries.Container.DocumentSeriesAnnexedLocation IsNot Nothing
            UnpublishedAnnexedPanel.Visible = CurrentDocumentSeriesItem.DocumentSeries.Container.DocumentSeriesUnpublishedAnnexedLocation IsNot Nothing
            TogglePublication(True)
            tblSubsection.Visible = (CurrentDocumentSeriesItem.DocumentSeries.SubsectionEnabled.GetValueOrDefault(False))
            pnlRoles.Visible = (CurrentDocumentSeriesItem.DocumentSeries.RoleEnabled.GetValueOrDefault(False))
            ' Imposto i diritti di inserimento
            cmdSaveDraft.Enabled = DocumentSeriesItemRights.CheckDocumentSeriesRight(CurrentDocumentSeriesItem.DocumentSeries, DocumentSeriesContainerRightPositions.Draft)
            cmdOk.Enabled = DocumentSeriesItemRights.CheckDocumentSeriesRight(CurrentDocumentSeriesItem.DocumentSeries, DocumentSeriesContainerRightPositions.Insert)

            cmdFlushOwner.Visible = (OwnerRolesInSession IsNot Nothing) AndAlso Not MyRoles.HasSingleItem()

            ' Imposto il valore di Required per il documento principale
            InitializeDocumentsInputs(uscUploadDocument, Not CurrentDocumentSeriesItem.DocumentSeries.AllowNoDocument.GetValueOrDefault(False), True)
            InitializeDocumentsInputs(uscUploadAnnexed, False, True)
            InitializeDocumentsInputs(uscUnpublishedAnnexed, False, True)

            If CurrentDocumentSeriesItem IsNot Nothing AndAlso CurrentDocumentSeriesItem.DocumentSeries IsNot Nothing AndAlso CurrentDocumentSeriesItem.DocumentSeries.Container IsNot Nothing Then
                Dim behaviour As ContainerBehaviour = Facade.ContainerBehaviourFacade.GetBehaviours(CurrentDocumentSeriesItem.DocumentSeries.Container, ContainerBehaviourAction.Insert, categoryTag).FirstOrDefault()
                If behaviour IsNot Nothing Then
                    If behaviour.AttributeName = categoryTag AndAlso behaviour.AttributeValue.Count > 0 Then
                        Dim category As Category = New Category
                        If Int32.TryParse(behaviour.AttributeValue, category.Id) Then
                            category = Facade.CategoryFacade.GetById(category.Id)
                            ItemCategory.DataSource.Add(category)
                            ItemCategory.DataBind()
                        End If
                    End If
                End If
                If Not ProtocolEnv.DocumentSeriesReorderDocumentEnabled AndAlso ActionNewItem Then
                    InitDocumentsPrivacyLevels(fromIndexChanged)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Imposto i dati presenti sulla pagina nel modello da salvare in sessione
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub BindModelFromPage()
            Dim model As DocumentSeriesInsertModel = New DocumentSeriesInsertModel()
            ' SottoSezione
            Dim subsection As List(Of DocumentSeriesSubsection) = TryCast(ddlSubsection.DataSource(), List(Of DocumentSeriesSubsection))
            If subsection IsNot Nothing AndAlso subsection.Count > 0 Then
                model.SubSection = ddlSubsection.SelectedValue
            End If
            'Annessi non parte integrante
            For Each annexedDocument As DocumentInfo In uscUploadAnnexed.DocumentInfos
                model.Annexed.Add(annexedDocument.Serialized)
            Next
            'Annessi da non pubblicare
            For Each unpublishedAnnexedDocument As DocumentInfo In uscUnpublishedAnnexed.DocumentInfos
                model.UnPublishedAnnexed.Add(unpublishedAnnexedDocument.Serialized)
            Next
            'Settori di appartenenza
            model.SectorMembershipAuthorizations = uscRoleOwner.SourceRoles
            'Autorizzazioni di conoscenza
            model.KnowledgeAuthorizations = uscRoleAuthorization.SourceRoles
            'Oggetto
            model.Object = ItemSubject.Text

            'Salvo lo stato degli oggetti in sessione
            CurrentDocumentSeriesModel = model
        End Sub

        ''' <summary>
        ''' Imposto i dati della pagina dal modello salvato in sessione
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub BindPageFromModel()
            ' SottoSezione
            If CurrentDocumentSeriesModel.SubSection IsNot Nothing Then
                ddlSubsection.ClearSelection()
                Dim selectedSubSection As ListItem = ddlSubsection.Items.FindByValue(CurrentDocumentSeriesModel.SubSection)
                If Not selectedSubSection Is Nothing Then
                    selectedSubSection.Selected = True
                End If
            End If

            'Annessi non parte integrante
            For Each annexed As String In CurrentDocumentSeriesModel.Annexed
                Dim docInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(annexed))
                uscUploadAnnexed.LoadDocumentInfo(docInfo)
            Next
            'Annessi da non pubblicare
            For Each unPublishedAnnexed As String In CurrentDocumentSeriesModel.UnPublishedAnnexed
                Dim docInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(unPublishedAnnexed))
                uscUploadAnnexed.LoadDocumentInfo(docInfo)
            Next
            'Settori di appartenenza. 
            'Imposto il proponente proveniente dalla pagina di Atti se è vuoto quello di questa pagina.
            If Not CurrentResolutionModel.Proposers Is Nothing AndAlso CurrentDocumentSeriesModel.SectorMembershipAuthorizations Is Nothing Then
                uscRoleOwner.SourceRoles = CurrentResolutionModel.Proposers
            Else
                uscRoleOwner.SourceRoles = CurrentDocumentSeriesModel.SectorMembershipAuthorizations
            End If
            uscRoleOwner.DataBind()
            'Autorizzazioni di conoscenza
            uscRoleAuthorization.SourceRoles = CurrentDocumentSeriesModel.KnowledgeAuthorizations
            uscRoleAuthorization.DataBind()
            'Oggetto
            ItemSubject.Text = CurrentDocumentSeriesModel.Object
        End Sub

        ''' <summary>
        ''' Compilo automaticamente la sotto-sezione.
        ''' Lancio il pannello ajax
        ''' Clicco automaticamente sul pulsante di autocomplete.
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub SkipAvcpEditorPage()
            ' Se ho almeno un elemento nella combo allora seleziono il primo.
            Dim subsection As List(Of DocumentSeriesSubsection) = TryCast(ddlSubsection.DataSource(), List(Of DocumentSeriesSubsection))
            If subsection IsNot Nothing AndAlso subsection.Count > 0 Then
                ddlSubsection.SelectedIndex = 1
            End If
            cmdAVCPAutocomplete_Click(cmdAVCPAutocomplete, New EventArgs())
        End Sub

        ''' <summary>
        ''' Recupera la serie documentale di Bandi Gara o AVCP collegate al CurrentDocumentSeriesItem
        ''' </summary>
        ''' <returns>
        ''' Ritorna La serie documentale collegata
        ''' Ritorna Nothing se non è presente un collegamento
        ''' </returns>
        Private Function GetSeriesLinked() As DocumentSeriesItem
            If CurrentDocumentSeriesItem Is Nothing Then Return Nothing
            If (Not ProtocolEnv.BandiGaraDocumentSeriesId.HasValue OrElse Not ProtocolEnv.AvcpDocumentSeriesId.HasValue) Then Return Nothing
            If (Not CurrentDocumentSeriesItem.DocumentSeries.Id.Equals(ProtocolEnv.BandiGaraDocumentSeriesId) _
                AndAlso Not CurrentDocumentSeriesItem.DocumentSeries.Id.Equals(ProtocolEnv.AvcpDocumentSeriesId)) Then
                Return Nothing
            End If

            Dim resolution As Resolution = Facade.ResolutionDocumentSeriesItemFacade.GetResolutions(CurrentDocumentSeriesItem).FirstOrDefault()
            If resolution Is Nothing Then Return Nothing

            Dim docSeriesLikedToResl As ICollection(Of DocumentSeriesItem) = Facade.ResolutionFacade.GetSeriesToAVCP(resolution)
            Dim idSeriesToFind As Integer = If(CurrentDocumentSeriesItem.DocumentSeries.Id.Equals(ProtocolEnv.BandiGaraDocumentSeriesId),
                                               ProtocolEnv.AvcpDocumentSeriesId,
                                               ProtocolEnv.BandiGaraDocumentSeriesId)
            Dim seriesToLink As DocumentSeriesItem = docSeriesLikedToResl.FirstOrDefault(Function(x) x.DocumentSeries.Id = idSeriesToFind)
            Return seriesToLink
        End Function

        Private Function GetResolutionKindDocumentSeriesId() As Guid?
            Dim IdResolutionKindDocumentSeries As Guid? = Nothing
            If CurrentResolution IsNot Nothing AndAlso CurrentResolution.ResolutionKind IsNot Nothing AndAlso Not CurrentResolutionKindFacade.CheckResolutionKindDocumentSeries(CurrentResolution, CurrentDocumentSeriesItem).IsEmpty() Then
                IdResolutionKindDocumentSeries = CurrentResolutionKindFacade.CheckResolutionKindDocumentSeries(CurrentResolution, CurrentDocumentSeriesItem)
            Else
                If CurrentResolutionModel.ResolutionKind.HasValue Then
                    IdResolutionKindDocumentSeries = ReslKindDocumentSeriesFacade.GetResolutionAndSeriesByReslAndSeries(CurrentResolutionModel.ResolutionKind.Value, CurrentDocumentSeriesItem.DocumentSeries.Id).Id
                End If
            End If
            Return IdResolutionKindDocumentSeries
        End Function

        Private Sub DeleteDocuments(toDetachIds As IList(Of Guid), location As Location)
            For Each idDocument As Guid In toDetachIds
                FileLogger.Debug(LoggerName, String.Format("CmdOkEditClick -> IdSeriesItem {0} - Delete document with Id: {1}", CurrentDocumentSeriesItem.Id, idDocument))
                Service.DetachDocument(idDocument)
            Next
        End Sub

        Private Sub InitializeDocumentControls()
            uscUploadDocument.Caption = String.Empty
            If (ProtocolEnv.DocumentSeriesDocumentsLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.MainChain)) Then
                uscUploadDocument.Caption = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.MainChain)
                uscUploadDocument.TreeViewCaption = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.MainChain)
            End If

            uscUploadAnnexed.Caption = String.Empty
            If (ProtocolEnv.DocumentSeriesDocumentsLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.AnnexedChain)) Then
                uscUploadAnnexed.Caption = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.AnnexedChain)
                uscUploadAnnexed.TreeViewCaption = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.AnnexedChain)
            End If

            uscUnpublishedAnnexed.Caption = String.Empty
            If (ProtocolEnv.DocumentSeriesDocumentsLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.UnpublishedAnnexedChain)) Then
                uscUnpublishedAnnexed.Caption = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.UnpublishedAnnexedChain)
                uscUnpublishedAnnexed.TreeViewCaption = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.UnpublishedAnnexedChain)
            End If
        End Sub

        Private Sub InitDocumentsPrivacyLevels(showAlert As Boolean)
            Dim minLevel As Integer = 0
            Dim forceValue As Integer? = Nothing
            If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso Not String.IsNullOrEmpty(ddlDocumentSeries.SelectedValue) Then
                Dim container As Container = Facade.ContainerFacade.GetById(CInt(ddlDocumentSeries.SelectedValue))
                If container IsNot Nothing Then
                    uscUploadDocument.ButtonPrivacyLevelVisible = container.PrivacyEnabled
                    uscUploadAnnexed.ButtonPrivacyLevelVisible = container.PrivacyEnabled
                    uscUnpublishedAnnexed.ButtonPrivacyLevelVisible = container.PrivacyEnabled
                    forceValue = container.PrivacyLevel
                    If container.PrivacyEnabled Then
                        forceValue = Nothing
                        Dim docs As List(Of DocumentInfo) = New List(Of DocumentInfo)(uscUploadDocument.DocumentInfosAdded)

                        docs.AddRange(uscUnpublishedAnnexed.DocumentInfosAdded)
                        docs.AddRange(uscUploadAnnexed.DocumentInfosAdded)

                        If Facade.DocumentFacade.CheckPrivacyLevel(docs, container) Then
                            forceValue = container.PrivacyLevel
                            If showAlert Then
                                AjaxAlert("Attenzione! Il livello di ", PRIVACY_LABEL, " del contenitore scelto è maggiore dei livelli attribuiti ai documenti. Ai documenti con livello di ", PRIVACY_LABEL, " minore, viene attribuito il livello del contenitore.")
                            End If
                        End If

                        If container IsNot Nothing Then
                            minLevel = container.PrivacyLevel
                        End If
                    End If
                End If
            End If
            uscUploadDocument.MinPrivacyLevel = minLevel
            uscUploadAnnexed.MinPrivacyLevel = minLevel
            uscUnpublishedAnnexed.MinPrivacyLevel = minLevel
            uscUploadDocument.RefreshPrivacyLevelAttributes(minLevel, forceValue)
            uscUploadAnnexed.RefreshPrivacyLevelAttributes(minLevel, forceValue)
            uscUnpublishedAnnexed.RefreshPrivacyLevelAttributes(minLevel, forceValue)
        End Sub

        Public Sub InitializeActionFromProtocol()
            Dim sessionDocs As Object = Session("ProtSelectedDocuments")

            If (sessionDocs Is Nothing) Then
                Throw New DocSuiteException("Errore creazione registrazione", "Impossibile inizializzare i documenti dalla pagina precedente.")
            End If

            FileLogger.Debug(LoggerName, String.Format("Inserimento nuova registrazione in Serie Documentale da Protocol [{0}].", CurrentProtocol.Id))

            Dim prevPageDocs As Dictionary(Of DocumentInfo, uscToSeries.ChainType) = DirectCast(sessionDocs, Dictionary(Of DocumentInfo, uscToSeries.ChainType))

            ' Imposto documenti, allegati, annessi
            Dim documents, annexed, attachments As New List(Of DocumentInfo)
            Dim initialCode As String = String.Empty

            If ProtocolEnv.IsFilenameAutomaticRenameEnabled Then
                initialCode = WebHelper.UploadDocumentRename(CurrentResolution.Type.Id, CurrentResolution.Year.Value, CurrentResolution.Number.Value)
            End If

            For Each item As KeyValuePair(Of DocumentInfo, uscToSeries.ChainType) In prevPageDocs
                Select Case item.Value
                    Case uscToSeries.ChainType.Document
                        LoadDocuments(initialCode, item.Key, documents)
                    Case uscToSeries.ChainType.Annexed
                        LoadDocuments(initialCode, item.Key, annexed)
                    Case uscToSeries.ChainType.UnpublishedAnnexed
                        LoadDocuments(initialCode, item.Key, attachments)
                End Select
            Next

            uscUploadDocument.LoadDocumentInfo(documents)
            uscUploadDocument.InitializeNodesAsAdded(True)

            uscUploadAnnexed.LoadDocumentInfo(annexed)
            uscUploadAnnexed.InitializeNodesAsAdded(True)

            uscUnpublishedAnnexed.LoadDocumentInfo(attachments)
            uscUnpublishedAnnexed.InitializeNodesAsAdded(True)

            Session.Remove("ProtSelectedDocuments")
        End Sub

        Public Sub LoadDocuments(code As String, doc As DocumentInfo, chain As List(Of DocumentInfo))
            If ProtocolEnv.IsFilenameAutomaticRenameEnabled Then
                doc.Name = WebHelper.UploadDocumentSetFilename(code, doc.Extension, chain.Count + 1)
            End If
            chain.Add(doc)
        End Sub

        Private Sub LoadAssociatedDocumentUnits()
            Dim documentUnitList As IList(Of DocumentUnitModel) = New List(Of DocumentUnitModel)
            Dim documentUnit As DocumentUnitModel
            ' Verifico se ci sono Resolution collegate
            If DocSuiteContext.Current.IsResolutionEnabled Then
                Dim resls As IList(Of Resolution) = Facade.ResolutionDocumentSeriesItemFacade.GetResolutions(CurrentDocumentSeriesItem)
                If Not resls.IsNullOrEmpty() Then
                    pnlEditAmmTrasp.Visible = True
                    For Each resl As Resolution In resls
                        documentUnit = New DocumentUnitModel()
                        documentUnit.UniqueId = resl.UniqueId
                        documentUnit.EntityId = resl.Id
                        documentUnit.Environment = DSWEnvironment.Resolution
                        documentUnit.Title = String.Format("{0} {1}", Facade.ResolutionFacade.GetResolutionLabel(resl), Facade.ResolutionFacade.GetResolutionNumber(resl))

                        If DocumentUnitsToDelete.FirstOrDefault(Function(s) s.UniqueId.Equals(documentUnit.UniqueId)) Is Nothing Then
                            documentUnitList.Add(documentUnit)
                        End If
                    Next
                End If
            End If

            Facade.ProtocolDocumentSeriesItemFacade.GetByProtocol(Guid.Parse("507f6947-46cd-4612-8a9c-ab0901187e7f"))
            ' Verifico se ci sono Protocolli collegati            
            If DocSuiteContext.Current.IsProtocolEnabled Then
                Dim prots As IList(Of Protocol) = Facade.ProtocolDocumentSeriesItemFacade.GetProtocols(CurrentDocumentSeriesItem)
                If Not prots.IsNullOrEmpty() Then
                    pnlEditAmmTrasp.Visible = True

                    For Each prot As Protocol In prots
                        documentUnit = New DocumentUnitModel()
                        documentUnit.UniqueId = prot.UniqueId
                        documentUnit.Number = prot.Number
                        documentUnit.Year = prot.Year
                        documentUnit.Environment = DSWEnvironment.Protocol
                        documentUnit.Title = String.Format("{0} {1}", "Protocollo", prot.FullNumber)

                        If DocumentUnitsToDelete.FirstOrDefault(Function(s) s.UniqueId.Equals(documentUnit.UniqueId)) Is Nothing Then
                            documentUnitList.Add(documentUnit)
                        End If
                    Next
                End If
            End If
            dgvLinkedDocumentUnit.DataSource = documentUnitList
            dgvLinkedDocumentUnit.DataBind()
        End Sub

        Private Sub AddDocumentUnitToDelete(documentUnit As DocumentUnitModel)
            Dim docUnitsToDelete As List(Of DocumentUnitModel) = DocumentUnitsToDelete
            docUnitsToDelete.Add(documentUnit)
            DocumentUnitsToDelete = docUnitsToDelete
        End Sub

        Private Sub SetFiltersState(Optional isErrorState As Boolean = False)
            lblItemPublishingDate.Visible = isErrorState
            cmdOk.Enabled = Not isErrorState
            cmdSaveDraft.Enabled = Not isErrorState
        End Sub
#End Region

    End Class

End Namespace