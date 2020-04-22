
Imports VecompSoftware.DocSuiteWeb.Data

<Serializable()>
Public Class FacadeFactory

#Region " Fields "

    Private ReadOnly _dbName As String
    Private Shared _instance As FacadeFactory
    'Private _filterItemFacade As FilterItemFacade
    Private _oChartFacade As OChartFacade
    Private _oChartItemFacade As OChartItemFacade
    Private _oChartItemContactFacade As OChartItemContactFacade
    Private _oChartItemContainerFacade As OChartItemContainerFacade
    Private _oChartItemMailboxFacade As OChartItemMailboxFacade
    Private _oChartItemRoleFacade As OChartItemRoleFacade
    Private _oChartItemWorkflowFacade As OChartItemWorkflowFacade
    Private _apiProviderFacade As APIProviderFacade
    Private _incrementalFacade As IncrementalFacade

#End Region

#Region " Properties "

    Public Shared ReadOnly Property Instance As FacadeFactory
        Get
            If _instance Is Nothing Then
                _instance = New FacadeFactory()
            End If
            Return _instance
        End Get
    End Property

    Public ReadOnly Property IncrementalFacade As IncrementalFacade
        Get
            If _incrementalFacade Is Nothing Then
                _incrementalFacade = New IncrementalFacade()
            End If
            Return _incrementalFacade
        End Get
    End Property

    Public ReadOnly Property OChartFacade As OChartFacade
        Get
            If _oChartFacade Is Nothing Then
                _oChartFacade = New OChartFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _oChartFacade
        End Get
    End Property

    Public ReadOnly Property OChartItemFacade As OChartItemFacade
        Get
            If _oChartItemFacade Is Nothing Then
                _oChartItemFacade = New OChartItemFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _oChartItemFacade
        End Get
    End Property

    Public ReadOnly Property OChartItemContactFacade As OChartItemContactFacade
        Get
            If _oChartItemContactFacade Is Nothing Then
                _oChartItemContactFacade = New OChartItemContactFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _oChartItemContactFacade
        End Get
    End Property

    Public ReadOnly Property OChartItemContainerFacade As OChartItemContainerFacade
        Get
            If _oChartItemContainerFacade Is Nothing Then
                _oChartItemContainerFacade = New OChartItemContainerFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _oChartItemContainerFacade
        End Get
    End Property

    Public ReadOnly Property OChartItemMailboxFacade As OChartItemMailboxFacade
        Get
            If _oChartItemMailboxFacade Is Nothing Then
                _oChartItemMailboxFacade = New OChartItemMailboxFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _oChartItemMailboxFacade
        End Get
    End Property

    Public ReadOnly Property OChartItemRoleFacade As OChartItemRoleFacade
        Get
            If _oChartItemRoleFacade Is Nothing Then
                _oChartItemRoleFacade = New OChartItemRoleFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _oChartItemRoleFacade
        End Get
    End Property

    Public ReadOnly Property OChartItemWorkflowFacade As OChartItemWorkflowFacade
        Get
            If _oChartItemWorkflowFacade Is Nothing Then
                _oChartItemWorkflowFacade = New OChartItemWorkflowFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _oChartItemWorkflowFacade
        End Get
    End Property

    Public ReadOnly Property APIProviderFacade As APIProviderFacade
        Get
            If _apiProviderFacade Is Nothing Then
                _apiProviderFacade = New APIProviderFacade()
            End If
            Return _apiProviderFacade
        End Get
    End Property

#End Region

#Region " Common facade members"
    Private _roleUserFacade As RoleUserFacade
    Private _containerFacade As ContainerFacade
    Private _containerGroupFacade As ContainerGroupFacade
    Private _containerDocTypeFacade As ContainerDocTypeFacade
    Private _contactFacade As ContactFacade
    Private _containerPropertyFacade As ContainerPropertyFacade
    Private _contactNameFacade As ContactNameFacade
    Private _contactTypeFacade As ContactTypeFacade
    Private _roleFacade As RoleFacade
    Private _roleNameFacade As RoleNameFacade
    Private _roleGroupFacade As RoleGroupFacade
    Private _contactplacename As ContactPlaceNameFacade
    Private _categoryFacade As CategoryFacade
    Private _documentTypeFacade As TableDocTypeFacade
    Private _parameterFacade As ParameterFacade
    Private _contacttitlefacade As ContactTitleFacade
    Private _recipientFacade As RecipientFacade
    Private _securityGroupsFacade As SecurityGroupsFacade
    Private _securityUsersFacade As SecurityUsersFacade
    Private _userErrorFacade As UserErrorFacade
    Private _taskLockFacade As ITaskExecutionManager
    Private _commonObjectFacade As CommonObjectFacade
    Private _categorySchemaFacade As CategorySchemaFacade
    Private _tableLogFacade As TableLogFacade

#End Region

#Region " Context Facade members "
    ''' <summary>
    ''' Facade per l'impostazione dei parametri di ambiente
    ''' </summary>
    Private _parameterEnvFacade As ParameterEnvFacade
#End Region

#Region " Fascicle facade members"
    Private _fascicleFacade As FascicleFacade
    Private _fascicleDocumentUnitFacade As FascicleDocumentUnitFacade
#End Region

#Region " Protocol facade members"
    Private _taskHeaderFacade As TaskHeaderFacade
    Private _taskDetailFacade As TaskDetailFacade
    Private _collaborationlogfacade As CollaborationLogFacade
    Private _collaborationFacade As CollaborationFacade
    Private _collaborationAggregateFacade As CollaborationAggregateFacade
    Private _collaborationVersioningFacade As CollaborationVersioningFacade
    Private _collaborationUsersFacade As CollaborationUsersFacade
    Private _collaborationStatusFacade As CollaborationStatusFacade
    Private _collaborationStatusRecipientFacade As CollaborationStatusRecipientFacade
    Private _protocolFacade As ProtocolFacade
    Private _protocolRoleUserFacade As ProtocolRoleUserFacade
    Private _protocolRejectedRoleFacade As ProtocolRejectedRoleFacade
    Private _packageFacade As PackageFacade
    Private _protocolTypeFacade As ProtocolTypeFacade
    Private _protocolTransfertFacade As ProtocolTransfertFacade
    Private _protocolStatusFacade As ProtocolStatusFacade
    Private _protocolLogFacade As ProtocolLogFacade
    Private _locationFacade As LocationFacade
    Private _userLogFacade As UserLogFacade
    Private _computerLogFacade As ComputerLogFacade
    Private _collaborationSignsFacade As CollaborationSignsFacade
    Private _protocolContactFacade As ProtocolContactFacade
    Private _protocolContactManuanlFacade As ProtocolContactManualFacade
    Private _zebraPrinterFacade As ZebraPrinterFacade
    Private _scannerConfigurationFacade As ScannerConfigurationFacade
    Private _scannerParameterFacade As ScannerParameterFacade
    Private _protocolDraftFacade As ProtocolDraftFacade
    Private _containerBehaviourFacade As ContainerBehaviourFacade
    Private _containerArchiveFacade As ContainerArchiveFacade
    Private _templateProtocolFacade As TemplateProtocolFacade
    Private _unifiedDiary As UnifiedDiaryFacade
    Private _protocolUserFacade As ProtocolUserFacade
#End Region

#Region " PEC Facades "

    Private _pecMailboxFacade As PECMailBoxFacade
    Private _pecMailboxConfigurationFacade As PECMailBoxConfigurationFacade
    Private _pecMailboxRoleFacade As PECMailBoxRoleFacade
    Private _pecMailBoxLogFacade As PECMailBoxLogFacade
    Private _pecMailContentFacade As PECMailContentFacade
    Private _pecMailFacade As PECMailFacade
    Private _pecMailReceiptFacade As PECMailReceiptFacade
    Private _pecMailAttachmentFacade As PECMailAttachmentFacade
    Private _pecMailLogFacade As PECMailLogFacade
    Private _pecMailViewFacade As PECMailViewFacade

#End Region

#Region " Biblos facade members"
    Private _biblosFacade As BiblosFacade
#End Region

#Region " Finders members"
    Private _resolutionFinder As NHibernateResolutionFinder 'IResolutionFinder
    Private _protocolFinder As NHibernateProtocolFinder 'IProtocolFinder
    Private _documentFinder As NHibernateDocumentFinder 'IDocumentFinder
    Private _tableLogFinder As NHibernateTableLogFinder
    Private _protocolLogFinder As NHibernateProtocolLogFinder
    Private _documentLogFinder As NHibernateDocumentLogFinder
    Private _documentTokenUserFinder As NHibernateDocumentTokenUserFinder
    Private _documentObjectFinder As NHibernateDocumentObjectFinder
    Private _documentTokenFinder As NHibernateDocumentTokenFinder
    Private _documentVersioningFinder As NHibernateDocumentVersioningFinder
    Private _serviceLogFinder As NHibernateServiceLogFinder
    Private _userErrorFinder As NHibernateUserErrorFinder
    Private _userLogFinder As NHibernateUserLogFinder
    Private _pecMailFinder As NHibernatePECMailFinder
    Private _pecMailBoxLogFinder As NHibernatePECMailBoxLogFinder
    Private _protocolHeaderFinderTd As NHibernateProtocolHeaderFinderTD
    Private _resolutionJournalFinder As NHibernateResolutionJournalFinder
    Private _pecOcFinder As NHibernatePECOCFinder
    Private _posteOnlineRequestFinder As NHPosteOnlineRequestFinder
#End Region

#Region " Document facade members"
    Private _containerExtensionFacade As ContainerExtensionFacade
    Private _documentFolderFacade As DocumentFolderFacade
    Private _documentObjectFacade As DocumentObjectFacade
    Private _documentLogFacade As DocumentLogFacade
    Private _documenttokenuser As DocumentTokenUserFacade
    Private _documentTabTokenFacade As DocumentTabTokenFacade
    Private _documentTokenFacade As DocumentTokenFacade
    Private _documentFacade As DocumentFacade
    Private _documentVersioningFacade As DocumentVersioningFacade
    Private _documentTabStatusFacade As DocumentTabStatusFacade
    Private _documentRoleFacade As RoleFacade
#End Region

#Region " Resolution facade members"

    Private _resolutionFacade As ResolutionFacade
    Private _resolutionRoleFacade As ResolutionRoleFacade
    Private _resolutionRoleTypeFacade As ResolutionRoleTypeFacade
    Private _tabledoctypefacade As TableDocTypeFacade
    Private _resolutionLogFinder As NHibernateResolutionLogFinder
    Private _resolutionLogFacade As ResolutionLogFacade
    Private _resolutionRecipientFacade As ResolutionRecipientFacade
    Private _resolutionWorkflowFacade As ResolutionWorkflowFacade
    Private _tabWorkflowFacade As TabWorkflowFacade
    Private _fileResolutionFacade As FileResolutionFacade
    Private _bidTypeFacade As BidTypeFacade
    Private _controllerStatusResolutionFacade As ControllerStatusResolutionFacade
    Private _resolutionStatusFacade As ResolutionStatusFacade
    Private _resolutionTypeFacade As ResolutionTypeFacade
    Private _resolutionTabMasterFacade As TabMasterFacade
    Private _resolutionJournalFacade As ResolutionJournalFacade
    Private _resolutionJournalDetailFacade As ResolutionJournalDetailFacade
    Private _resolutionJournalTemplateFacade As ResolutionJournalTemplateFacade
    Private _serviceCodeDescriptorFacade As ServiceCodeDescriptorFacade
    Private _PECOCFacade As PECOCFacade
    Private _PECOCLogFacade As PECOCLogFacade
    Private _webPublicationFacade As WebPublicationFacade
    Private _resolutionWpFacade As ResolutionWPFacade
    Private _documentSeriesFacade As DocumentSeriesFacade
    Private _documentSeriesFamilyFacade As DocumentSeriesFamilyFacade
    Private _documentSeriesItemFacade As DocumentSeriesItemFacade
    Private _documentSeriesItemMessageFacade As DocumentSeriesItemMessageFacade
    Private _documentSeriesItemResolutionLinkFacade As DocumentSeriesItemLinkFacade
    Private _documentSeriesItemLogFacade As DocumentSeriesItemLogFacade
    Private _documentSeriesIncrementalFacade As DocumentSeriesIncrementalFacade
    Private _documentSeriesItemRoleFacade As DocumentSeriesItemRoleFacade
    Private _messageFacade As MessageFacade
    Private _messageContactFacade As MessageContactFacade
    Private _messageContactEmailFacade As MessageContactEmailFacade
    Private _messageAttachmentFacade As MessageAttachmentFacade
    Private _messageEmailFacade As MessageEmailFacade
    Private _messageLogFacade As MessageLogFacade
    Private _protocolMessageFacade As ProtocolMessageFacade
    Private _resolutionMessageFacade As ResolutionMessageFacade
    Private _printLetterRoleFacade As PrintLetterRoleFacade
    Private _resolutionActivityFacade As ResolutionActivityFacade
#End Region

#Region " PosteWeb facade members"
    Private _polRequestFacade As PosteOnLineRequestFacade
    Private _polContactFacade As PosteOnLineContactFacade
    Private _polAccountFacade As PosteOnLineAccountFacade
#End Region

#Region " Parer facade members "
    Private _protocolParerFacade As ProtocolParerFacade
    Private _resolutionParerFacade As ResolutionParerFacade
#End Region

#Region " Series facade members "
    Private _documentSeriesAttributeBehaviourFacade As DocumentSeriesAttributeBehaviourFacade
    Private _resolutionDocumentSeriesItemFacade As ResolutionDocumentSeriesItemFacade
    Private _documentSeriesSubsectionFacade As DocumentSeriesSubsectionFacade
    Private _protocolDocumentSeriesItemFacade As ProtocolDocumentSeriesItemFacade
#End Region

    Private _tenderHeaderFacade As TenderHeaderFacade
    Private _tenderLotFacade As TenderLotFacade

#Region " Constructors"

    Public Sub New()
        _dbName = "ProtDB"
    End Sub

    Public Sub New(ByVal dbName As String)
        _dbName = dbName
    End Sub

#End Region

#Region " Common facade properties"
    Public ReadOnly Property RoleUserFacade() As RoleUserFacade
        Get
            If _roleUserFacade Is Nothing Then
                _roleUserFacade = New RoleUserFacade(_dbName)
            End If
            Return _roleUserFacade
        End Get
    End Property

    Public ReadOnly Property ContainerFacade() As ContainerFacade
        Get
            If _containerFacade Is Nothing Then
                _containerFacade = New ContainerFacade(_dbName)
            End If
            Return _containerFacade
        End Get
    End Property

    Public ReadOnly Property ContainerPropertyFacade() As ContainerPropertyFacade
        Get
            If _containerPropertyFacade Is Nothing Then
                _containerPropertyFacade = New ContainerPropertyFacade(_dbName)
            End If
            Return _containerPropertyFacade
        End Get
    End Property

    Public ReadOnly Property ContainerGroupFacade() As ContainerGroupFacade
        Get
            If _containerGroupFacade Is Nothing Then
                _containerGroupFacade = New ContainerGroupFacade(_dbName)
            End If
            Return _containerGroupFacade
        End Get
    End Property

    Public ReadOnly Property ContainerDocTypeFacade() As ContainerDocTypeFacade
        Get
            If _containerDocTypeFacade Is Nothing Then
                _containerDocTypeFacade = New ContainerDocTypeFacade(_dbName)
            End If
            Return _containerDocTypeFacade
        End Get
    End Property

    Public ReadOnly Property ContactTitleFacade() As ContactTitleFacade
        Get
            If _contacttitlefacade Is Nothing Then
                _contacttitlefacade = New ContactTitleFacade(_dbName)
            End If
            Return _contacttitlefacade
        End Get
    End Property
    Public ReadOnly Property ContactNameFacade() As ContactNameFacade
        Get
            If _contactNameFacade Is Nothing Then
                _contactNameFacade = New ContactNameFacade(_dbName)
            End If
            Return _contactNameFacade
        End Get
    End Property
    Public ReadOnly Property ContactFacade() As ContactFacade
        Get
            If _contactFacade Is Nothing Then
                _contactFacade = New ContactFacade(_dbName)
            End If
            Return _contactFacade
        End Get
    End Property

    Public ReadOnly Property ContactTypeFacade As ContactTypeFacade
        Get
            If _contactTypeFacade Is Nothing Then
                _contactTypeFacade = New ContactTypeFacade()
            End If
            Return _contactTypeFacade
        End Get
    End Property

    Public ReadOnly Property RoleFacade() As RoleFacade
        Get
            If _roleFacade Is Nothing Then
                _roleFacade = New RoleFacade(_dbName)
            End If
            Return _roleFacade
        End Get
    End Property

    Public ReadOnly Property RoleNameFacade() As RoleNameFacade
        Get
            If _roleNameFacade Is Nothing Then
                _roleNameFacade = New RoleNameFacade(_dbName)
            End If
            Return _roleNameFacade
        End Get
    End Property

    Public ReadOnly Property RoleGroupFacade() As RoleGroupFacade
        Get
            If _roleGroupFacade Is Nothing Then
                _roleGroupFacade = New RoleGroupFacade(_dbName)
            End If
            Return _roleGroupFacade
        End Get
    End Property

    Public ReadOnly Property ContactPlaceNameFacade() As ContactPlaceNameFacade
        Get
            If _contactplacename Is Nothing Then
                _contactplacename = New ContactPlaceNameFacade(_dbName)
            End If
            Return _contactplacename
        End Get
    End Property

    Public ReadOnly Property CategoryFacade() As CategoryFacade
        Get
            If _categoryFacade Is Nothing Then
                _categoryFacade = New CategoryFacade(_dbName)
            End If
            Return _categoryFacade
        End Get
    End Property

    Public ReadOnly Property CategorySchemaFacade As CategorySchemaFacade
        Get
            If _categorySchemaFacade Is Nothing Then
                _categorySchemaFacade = New CategorySchemaFacade(_dbName, DocSuiteContext.Current.User.UserName)
            End If
            Return _categorySchemaFacade
        End Get
    End Property

    Public ReadOnly Property DocumentTypeFacade() As TableDocTypeFacade
        Get
            If _documentTypeFacade Is Nothing Then
                _documentTypeFacade = New TableDocTypeFacade(_dbName)
            End If
            Return _documentTypeFacade
        End Get
    End Property

    Public ReadOnly Property ParameterFacade() As ParameterFacade
        Get
            If _parameterFacade Is Nothing Then
                _parameterFacade = New ParameterFacade(_dbName)
            End If
            Return _parameterFacade
        End Get
    End Property

    Public ReadOnly Property LocationFacade() As LocationFacade
        Get
            If _locationFacade Is Nothing Then
                _locationFacade = New LocationFacade(_dbName)
            End If
            Return _locationFacade
        End Get
    End Property

    Public ReadOnly Property RecipientFacade() As RecipientFacade
        Get
            If _recipientFacade Is Nothing Then
                _recipientFacade = New RecipientFacade(_dbName)
            End If
            Return _recipientFacade
        End Get
    End Property

    Public ReadOnly Property SecurityGroupsFacade() As SecurityGroupsFacade
        Get
            If _securityGroupsFacade Is Nothing Then
                _securityGroupsFacade = New SecurityGroupsFacade(_dbName)
            End If
            Return _securityGroupsFacade
        End Get
    End Property

    Public ReadOnly Property SecurityUsersFacade() As SecurityUsersFacade
        Get
            If _securityUsersFacade Is Nothing Then
                _securityUsersFacade = New SecurityUsersFacade(_dbName)
            End If
            Return _securityUsersFacade
        End Get
    End Property

    Public ReadOnly Property UserErrorFacade() As UserErrorFacade
        Get
            If _userErrorFacade Is Nothing Then
                _userErrorFacade = New UserErrorFacade()
            End If
            Return _userErrorFacade
        End Get
    End Property

    Public ReadOnly Property TaskLockFacade() As ITaskExecutionManager
        Get
            If _taskLockFacade Is Nothing Then
                _taskLockFacade = New TaskLockFacade()
            End If
            Return _taskLockFacade
        End Get
    End Property

    Public ReadOnly Property TaskHeaderFacade() As TaskHeaderFacade
        Get
            If _taskHeaderFacade Is Nothing Then
                _taskHeaderFacade = New TaskHeaderFacade()
            End If
            Return _taskHeaderFacade
        End Get
    End Property

    Public ReadOnly Property TaskDetailFacade() As TaskDetailFacade
        Get
            If _taskDetailFacade Is Nothing Then
                _taskDetailFacade = New TaskDetailFacade()
            End If
            Return _taskDetailFacade
        End Get
    End Property

    Public ReadOnly Property CommonObjectFacade() As CommonObjectFacade
        Get
            If _commonObjectFacade Is Nothing Then
                _commonObjectFacade = New CommonObjectFacade(_dbName)
            End If
            Return _commonObjectFacade
        End Get
    End Property

    Public ReadOnly Property TabMasterFacade() As TabMasterFacade
        Get
            If _resolutionTabMasterFacade Is Nothing Then
                _resolutionTabMasterFacade = New TabMasterFacade(_dbName)
            End If
            Return _resolutionTabMasterFacade
        End Get
    End Property
    Public ReadOnly Property TableLogFacade() As TableLogFacade
        Get
            If _tableLogFacade Is Nothing Then
                _tableLogFacade = New TableLogFacade()
            End If
            Return _tableLogFacade
        End Get
    End Property
#End Region

#Region " Context Facade properties "
    ''' <summary>
    ''' Gestione della tabella parameterEnv e del rispettivo context
    ''' </summary>
    Public ReadOnly Property ParameterEnvFacade() As ParameterEnvFacade
        Get
            If _parameterEnvFacade Is Nothing Then
                _parameterEnvFacade = New ParameterEnvFacade()
            End If
            Return _parameterEnvFacade
        End Get
    End Property
#End Region

#Region " Protocol facade properties "
    '_collaborationAggregateFacade
    Public ReadOnly Property CollaborationAggregateFacade() As CollaborationAggregateFacade
        Get
            If _collaborationAggregateFacade Is Nothing Then
                _collaborationAggregateFacade = New CollaborationAggregateFacade()
            End If
            Return _collaborationAggregateFacade
        End Get
    End Property


    Public ReadOnly Property CollaborationLogFacade() As CollaborationLogFacade
        Get
            If _collaborationlogfacade Is Nothing Then
                _collaborationlogfacade = New CollaborationLogFacade()
            End If
            Return _collaborationlogfacade
        End Get
    End Property

    Public ReadOnly Property CollaborationSignsFacade() As CollaborationSignsFacade
        Get
            If _collaborationSignsFacade Is Nothing Then
                _collaborationSignsFacade = New CollaborationSignsFacade()
            End If
            Return _collaborationSignsFacade
        End Get
    End Property

    Public ReadOnly Property CollaborationUsersFacade() As CollaborationUsersFacade
        Get
            If _collaborationUsersFacade Is Nothing Then
                _collaborationUsersFacade = New CollaborationUsersFacade()
            End If
            Return _collaborationUsersFacade
        End Get
    End Property

    Public ReadOnly Property CollaborationFacade() As CollaborationFacade
        Get
            If _collaborationFacade Is Nothing Then
                _collaborationFacade = New CollaborationFacade()
                _collaborationFacade.Factory = Me
            End If
            Return _collaborationFacade
        End Get
    End Property

    Public ReadOnly Property CollaborationVersioningFacade() As CollaborationVersioningFacade
        Get
            If _collaborationVersioningFacade Is Nothing Then
                _collaborationVersioningFacade = New CollaborationVersioningFacade()
            End If
            Return _collaborationVersioningFacade
        End Get
    End Property

    Public ReadOnly Property CollaborationStatusFacade() As CollaborationStatusFacade
        Get
            If _collaborationStatusFacade Is Nothing Then
                _collaborationStatusFacade = New CollaborationStatusFacade()
            End If
            Return _collaborationStatusFacade
        End Get
    End Property
    Public ReadOnly Property CollaborationStatusRecipientFacade() As CollaborationStatusRecipientFacade
        Get
            If _collaborationStatusRecipientFacade Is Nothing Then
                _collaborationStatusRecipientFacade = New CollaborationStatusRecipientFacade()
            End If
            Return _collaborationStatusRecipientFacade
        End Get
    End Property

    Public ReadOnly Property ProtocolRejectedRoleFacade As ProtocolRejectedRoleFacade
        Get
            If _protocolRejectedRoleFacade Is Nothing Then
                _protocolRejectedRoleFacade = New ProtocolRejectedRoleFacade()
            End If
            Return _protocolRejectedRoleFacade
        End Get
    End Property

    Public ReadOnly Property ProtocolFacade() As ProtocolFacade
        Get
            If _protocolFacade Is Nothing Then
                _protocolFacade = New ProtocolFacade()
                _protocolFacade.Factory = Me

            End If
            Return _protocolFacade
        End Get
    End Property

    Public ReadOnly Property ProtocolRoleUserFacade() As ProtocolRoleUserFacade
        Get
            If _protocolRoleUserFacade Is Nothing Then
                _protocolRoleUserFacade = New ProtocolRoleUserFacade()
                _protocolRoleUserFacade.Factory = Me

            End If
            Return _protocolRoleUserFacade
        End Get
    End Property

    Public ReadOnly Property FascicleFacade() As FascicleFacade
        Get
            If _fascicleFacade Is Nothing Then
                _fascicleFacade = New FascicleFacade()
            End If
            Return _fascicleFacade
        End Get
    End Property

    Public ReadOnly Property FascicleDocumentUnitFacade As FascicleDocumentUnitFacade
        Get
            If _fascicleDocumentUnitFacade Is Nothing Then
                _fascicleDocumentUnitFacade = New FascicleDocumentUnitFacade()
            End If
            Return _fascicleDocumentUnitFacade
        End Get
    End Property

    Public ReadOnly Property PackageFacade() As PackageFacade
        Get
            If _packageFacade Is Nothing Then
                _packageFacade = New PackageFacade()
            End If
            Return _packageFacade
        End Get
    End Property

    Public ReadOnly Property ProtocolTypeFacade() As ProtocolTypeFacade
        Get
            If _protocolTypeFacade Is Nothing Then
                _protocolTypeFacade = New ProtocolTypeFacade()
            End If
            Return _protocolTypeFacade
        End Get
    End Property

    Public ReadOnly Property ProtocolTransfertFacade() As ProtocolTransfertFacade
        Get
            If _protocolTransfertFacade Is Nothing Then
                _protocolTransfertFacade = New ProtocolTransfertFacade()
            End If
            Return _protocolTransfertFacade
        End Get
    End Property

    Public ReadOnly Property ProtocolStatusFacade As ProtocolStatusFacade
        Get
            If _protocolStatusFacade Is Nothing Then
                _protocolStatusFacade = New ProtocolStatusFacade
            End If
            Return _protocolStatusFacade
        End Get
    End Property

    Public ReadOnly Property ProtocolLogFacade() As ProtocolLogFacade
        Get
            If _protocolLogFacade Is Nothing Then
                _protocolLogFacade = New ProtocolLogFacade
            End If
            Return _protocolLogFacade
        End Get
    End Property

    Public ReadOnly Property UserLogFacade() As UserLogFacade
        Get
            If _userLogFacade Is Nothing Then
                _userLogFacade = New UserLogFacade
            End If
            Return _userLogFacade
        End Get
    End Property

    Public ReadOnly Property ComputerLogFacade As ComputerLogFacade
        Get
            If _computerLogFacade Is Nothing Then
                _computerLogFacade = New ComputerLogFacade
            End If
            Return _computerLogFacade
        End Get
    End Property

    Public ReadOnly Property ProtocolContactFacade() As ProtocolContactFacade
        Get
            If _protocolContactFacade Is Nothing Then
                _protocolContactFacade = New ProtocolContactFacade
            End If
            Return _protocolContactFacade
        End Get
    End Property

    Public ReadOnly Property ProtocolContactManualFacade() As ProtocolContactManualFacade
        Get
            If _protocolContactManuanlFacade Is Nothing Then
                _protocolContactManuanlFacade = New ProtocolContactManualFacade
            End If
            Return _protocolContactManuanlFacade
        End Get
    End Property

    Public ReadOnly Property ZebraPrinterFacade() As ZebraPrinterFacade
        Get
            If _zebraPrinterFacade Is Nothing Then
                _zebraPrinterFacade = New ZebraPrinterFacade
            End If
            Return _zebraPrinterFacade
        End Get
    End Property

    Public ReadOnly Property ScannerConfigurationFacade() As ScannerConfigurationFacade
        Get
            If _scannerConfigurationFacade Is Nothing Then
                _scannerConfigurationFacade = New ScannerConfigurationFacade
            End If
            Return _scannerConfigurationFacade
        End Get
    End Property

    Public ReadOnly Property ScannerParameterFacade() As ScannerParameterFacade
        Get
            If _scannerParameterFacade Is Nothing Then
                _scannerParameterFacade = New ScannerParameterFacade
            End If
            Return _scannerParameterFacade
        End Get
    End Property

    Public ReadOnly Property ProtocolDraftFacade() As ProtocolDraftFacade
        Get
            If _protocolDraftFacade Is Nothing Then
                _protocolDraftFacade = New ProtocolDraftFacade
            End If
            Return _protocolDraftFacade
        End Get
    End Property

    Public ReadOnly Property ContainerArchiveFacade() As ContainerArchiveFacade
        Get
            If _containerArchiveFacade Is Nothing Then
                _containerArchiveFacade = New ContainerArchiveFacade()
            End If
            Return _containerArchiveFacade
        End Get
    End Property

    Public ReadOnly Property TemplateProtocolFacade() As TemplateProtocolFacade
        Get
            If _templateProtocolFacade Is Nothing Then
                _templateProtocolFacade = New TemplateProtocolFacade()
            End If
            Return _templateProtocolFacade
        End Get
    End Property

    Public ReadOnly Property UnifiedDiaryFacade As UnifiedDiaryFacade
        Get
            If _unifiedDiary Is Nothing Then
                _unifiedDiary = New UnifiedDiaryFacade()
            End If
            Return _unifiedDiary
        End Get
    End Property

    Public ReadOnly Property ProtocolUserFacade As ProtocolUserFacade
        Get
            If _protocolUserFacade Is Nothing Then
                _protocolUserFacade = New ProtocolUserFacade
            End If
            Return _protocolUserFacade
        End Get
    End Property

#End Region

#Region " PEC Facade properties "

    Public ReadOnly Property PECMailboxFacade() As PECMailBoxFacade
        Get
            If _pecMailboxFacade Is Nothing Then
                _pecMailboxFacade = New PECMailBoxFacade
            End If
            Return _pecMailboxFacade
        End Get
    End Property

    Public ReadOnly Property PECMailboxConfigurationFacade As PECMailBoxConfigurationFacade
        Get
            If _pecMailboxConfigurationFacade Is Nothing Then
                _pecMailboxConfigurationFacade = New PECMailBoxConfigurationFacade
            End If
            Return _pecMailboxConfigurationFacade
        End Get
    End Property

    Public ReadOnly Property PECMailboxRoleFacade() As PECMailBoxRoleFacade
        Get
            If _pecMailboxRoleFacade Is Nothing Then
                _pecMailboxRoleFacade = New PECMailBoxRoleFacade
            End If
            Return _pecMailboxRoleFacade
        End Get
    End Property

    Public ReadOnly Property PECMailboxLogFacade() As PECMailBoxLogFacade
        Get
            If _pecMailBoxLogFacade Is Nothing Then
                _pecMailBoxLogFacade = New PECMailBoxLogFacade
            End If
            Return _pecMailBoxLogFacade
        End Get
    End Property

    Public ReadOnly Property PECMailContentFacade() As PECMailContentFacade
        Get
            If _pecMailContentFacade Is Nothing Then
                _pecMailContentFacade = New PECMailContentFacade
            End If
            Return _pecMailContentFacade
        End Get
    End Property

    Public ReadOnly Property PECMailFacade() As PECMailFacade
        Get
            If _pecMailFacade Is Nothing Then
                _pecMailFacade = New PECMailFacade
            End If
            Return _pecMailFacade
        End Get
    End Property

    Public ReadOnly Property PECMailReceiptFacade() As PECMailReceiptFacade
        Get
            If _pecMailReceiptFacade Is Nothing Then
                _pecMailReceiptFacade = New PECMailReceiptFacade
            End If
            Return _pecMailReceiptFacade
        End Get
    End Property

    Public ReadOnly Property PECMailAttachmentFacade() As PECMailAttachmentFacade
        Get
            If _pecMailAttachmentFacade Is Nothing Then
                _pecMailAttachmentFacade = New PECMailAttachmentFacade
            End If
            Return _pecMailAttachmentFacade
        End Get
    End Property

    Public ReadOnly Property PECMailLogFacade() As PECMailLogFacade
        Get
            If _pecMailLogFacade Is Nothing Then
                _pecMailLogFacade = New PECMailLogFacade
            End If
            Return _pecMailLogFacade
        End Get
    End Property

    Public ReadOnly Property PECMailViewFacade() As PECMailViewFacade
        Get
            If _pecMailViewFacade Is Nothing Then
                _pecMailViewFacade = New PECMailViewFacade
            End If
            Return _pecMailViewFacade
        End Get
    End Property

#End Region

#Region " Document facade properties"

    Public ReadOnly Property ContainerExtensionFacade() As ContainerExtensionFacade
        Get
            If _containerExtensionFacade Is Nothing Then
                _containerExtensionFacade = New ContainerExtensionFacade()
            End If
            Return _containerExtensionFacade
        End Get
    End Property

    Public ReadOnly Property DocumentFolderFacade() As DocumentFolderFacade
        Get
            If _documentFolderFacade Is Nothing Then
                _documentFolderFacade = New DocumentFolderFacade()
            End If
            Return _documentFolderFacade
        End Get
    End Property

    Public ReadOnly Property DocumentObjectFacade() As DocumentObjectFacade
        Get
            If _documentObjectFacade Is Nothing Then
                _documentObjectFacade = New DocumentObjectFacade()
            End If
            Return _documentObjectFacade
        End Get
    End Property

    Public ReadOnly Property DocumentLogFacade() As DocumentLogFacade
        Get
            If _documentLogFacade Is Nothing Then
                _documentLogFacade = New DocumentLogFacade
            End If
            Return _documentLogFacade
        End Get
    End Property

    Public ReadOnly Property DocumentTokenUserFacade() As DocumentTokenUserFacade
        Get
            If _documenttokenuser Is Nothing Then
                _documenttokenuser = New DocumentTokenUserFacade()
            End If
            Return _documenttokenuser
        End Get
    End Property

    Public ReadOnly Property DocumentTabTokenFacade() As DocumentTabTokenFacade
        Get
            If _documentTabTokenFacade Is Nothing Then
                _documentTabTokenFacade = New DocumentTabTokenFacade
            End If
            Return _documentTabTokenFacade
        End Get
    End Property

    Public ReadOnly Property DocumentTokenFacade() As DocumentTokenFacade
        Get
            If _documentTokenFacade Is Nothing Then
                _documentTokenFacade = New DocumentTokenFacade
            End If
            Return _documentTokenFacade
        End Get
    End Property

    Public ReadOnly Property DocumentFacade() As DocumentFacade
        Get
            If _documentFacade Is Nothing Then
                _documentFacade = New DocumentFacade
            End If
            Return _documentFacade
        End Get
    End Property

    Public ReadOnly Property DocumentVersioningFacade() As DocumentVersioningFacade
        Get
            If _documentVersioningFacade Is Nothing Then
                _documentVersioningFacade = New DocumentVersioningFacade
            End If
            Return _documentVersioningFacade
        End Get
    End Property

    Public ReadOnly Property DocumentRoleFacade() As RoleFacade
        Get
            If _documentRoleFacade Is Nothing Then
                _documentRoleFacade = New RoleFacade()
            End If
            Return _documentRoleFacade
        End Get
    End Property

    Public ReadOnly Property DocumentTabStatusFacade() As DocumentTabStatusFacade
        Get
            If _documentTabStatusFacade Is Nothing Then
                _documentTabStatusFacade = New DocumentTabStatusFacade()
            End If
            Return _documentTabStatusFacade
        End Get
    End Property

#End Region

#Region " Biblos facade properties"
    Public ReadOnly Property BiblosFacade() As BiblosFacade
        Get
            If _biblosFacade Is Nothing Then
                _biblosFacade = New BiblosFacade()
            End If
            Return _biblosFacade
        End Get
    End Property
#End Region

#Region " Finder Properties "

    Public ReadOnly Property ProtocolFinder() As NHibernateProtocolFinder
        Get
            If _protocolFinder Is Nothing Then
                _protocolFinder = New NHibernateProtocolFinder(_dbName)
            End If
            Return _protocolFinder
        End Get
    End Property

    Public ReadOnly Property ProtocolHeaderFinderTD() As NHibernateProtocolHeaderFinderTD
        Get
            If _protocolHeaderFinderTd Is Nothing Then
                _protocolHeaderFinderTd = New NHibernateProtocolHeaderFinderTD()
            End If
            Return _protocolHeaderFinderTd
        End Get
    End Property

    Public ReadOnly Property ResolutionFinder() As NHibernateResolutionFinder
        Get
            If _resolutionFinder Is Nothing Then
                _resolutionFinder = New NHibernateResolutionFinder(_dbName)
            End If
            Return _resolutionFinder
        End Get
    End Property

    Public ReadOnly Property DocumentFinder() As NHibernateDocumentFinder
        Get
            If _documentFinder Is Nothing Then
                _documentFinder = New NHibernateDocumentFinder(_dbName)
            End If
            Return _documentFinder
        End Get
    End Property

    Public ReadOnly Property TableLogFinder() As NHibernateTableLogFinder
        Get
            If _tableLogFinder Is Nothing Then
                '
                _tableLogFinder = New NHibernateTableLogFinder()
            End If
            Return _tableLogFinder
        End Get
    End Property

    Public ReadOnly Property ProtocolLogFinder() As NHibernateProtocolLogFinder
        Get
            If _protocolLogFinder Is Nothing Then
                _protocolLogFinder = New NHibernateProtocolLogFinder()
            End If
            Return _protocolLogFinder
        End Get
    End Property

    Public ReadOnly Property DocumentLogFinder() As NHibernateDocumentLogFinder
        Get
            If _documentLogFinder Is Nothing Then
                _documentLogFinder = New NHibernateDocumentLogFinder()
            End If
            Return _documentLogFinder
        End Get
    End Property

    Public ReadOnly Property DocumentTokenUserFinder() As NHibernateDocumentTokenUserFinder
        Get
            If _documentTokenUserFinder Is Nothing Then
                _documentTokenUserFinder = New NHibernateDocumentTokenUserFinder()
            End If
            Return _documentTokenUserFinder
        End Get
    End Property

    Public ReadOnly Property DocumentObjectFinder() As NHibernateDocumentObjectFinder
        Get
            If _documentObjectFinder Is Nothing Then
                _documentObjectFinder = New NHibernateDocumentObjectFinder()
            End If
            Return _documentObjectFinder
        End Get
    End Property

    Public ReadOnly Property DocumentTokenFinder() As NHibernateDocumentTokenFinder
        Get
            If _documentTokenFinder Is Nothing Then
                _documentTokenFinder = New NHibernateDocumentTokenFinder()
            End If
            Return _documentTokenFinder
        End Get
    End Property

    Public ReadOnly Property DocumentVersioningFinder() As NHibernateDocumentVersioningFinder
        Get
            If _documentVersioningFinder Is Nothing Then
                _documentVersioningFinder = New NHibernateDocumentVersioningFinder()
            End If
            Return _documentVersioningFinder
        End Get
    End Property

    Public ReadOnly Property ResolutionLogFinder() As NHibernateResolutionLogFinder
        Get
            If _resolutionLogFinder Is Nothing Then
                _resolutionLogFinder = New NHibernateResolutionLogFinder()
            End If
            Return _resolutionLogFinder
        End Get
    End Property

    Public ReadOnly Property ServiceLogFinder() As NHibernateServiceLogFinder
        Get
            If _serviceLogFinder Is Nothing Then
                _serviceLogFinder = New NHibernateServiceLogFinder()
            End If
            Return _serviceLogFinder
        End Get
    End Property

    Public ReadOnly Property UserLogFinder() As NHibernateUserLogFinder
        Get
            If _userLogFinder Is Nothing Then
                _userLogFinder = New NHibernateUserLogFinder()
            End If
            Return _userLogFinder
        End Get
    End Property

    Public ReadOnly Property UserErrorFinder() As NHibernateUserErrorFinder
        Get
            If _userErrorFinder Is Nothing Then
                _userErrorFinder = New NHibernateUserErrorFinder()
            End If
            Return _userErrorFinder
        End Get
    End Property

    Public ReadOnly Property PECMailFinder() As NHibernatePECMailFinder
        Get
            If _pecMailFinder Is Nothing Then
                _pecMailFinder = New NHibernatePECMailFinder()
            End If
            Return _pecMailFinder
        End Get
    End Property

    Public ReadOnly Property PECMailBoxLogFinder() As NHibernatePECMailBoxLogFinder
        Get
            If _pecMailBoxLogFinder Is Nothing Then
                _pecMailBoxLogFinder = New NHibernatePECMailBoxLogFinder()
            End If
            Return _pecMailBoxLogFinder
        End Get
    End Property

    Public ReadOnly Property ResolutionJournalFinder() As NHibernateResolutionJournalFinder
        Get
            If _resolutionJournalFinder Is Nothing Then
                _resolutionJournalFinder = New NHibernateResolutionJournalFinder(_dbName)
            End If
            Return _resolutionJournalFinder
        End Get
    End Property

    Public ReadOnly Property PECOCFinder() As NHibernatePECOCFinder
        Get
            If _pecOcFinder Is Nothing Then
                _pecOcFinder = New NHibernatePECOCFinder(_dbName)
            End If
            Return _pecOcFinder
        End Get
    End Property

    Public ReadOnly Property PosteOnlineRequestFinder() As NHPosteOnlineRequestFinder
        Get
            If _posteOnlineRequestFinder Is Nothing Then
                _posteOnlineRequestFinder = New NHPosteOnlineRequestFinder(_dbName)
            End If
            Return _posteOnlineRequestFinder
        End Get
    End Property
#End Region

#Region " Resolution facade properties"

    Public ReadOnly Property ResolutionFacade() As ResolutionFacade
        Get
            If _resolutionFacade Is Nothing Then
                _resolutionFacade = New ResolutionFacade()
            End If
            Return _resolutionFacade
        End Get
    End Property

    Public ReadOnly Property ResolutionRoleFacade() As ResolutionRoleFacade
        Get
            If _resolutionRoleFacade Is Nothing Then
                _resolutionRoleFacade = New ResolutionRoleFacade()
            End If
            Return _resolutionRoleFacade
        End Get
    End Property
    Public ReadOnly Property ResolutionRoleTypeFacade() As ResolutionRoleTypeFacade
        Get
            If _resolutionRoleTypeFacade Is Nothing Then
                _resolutionRoleTypeFacade = New ResolutionRoleTypeFacade()
            End If
            Return _resolutionRoleTypeFacade
        End Get
    End Property

    Public ReadOnly Property ResolutionRecipientFacade() As ResolutionRecipientFacade
        Get
            If _resolutionRecipientFacade Is Nothing Then
                _resolutionRecipientFacade = New ResolutionRecipientFacade()
            End If
            Return _resolutionRecipientFacade
        End Get
    End Property

    Public ReadOnly Property TableDocTypeFacade() As TableDocTypeFacade
        Get
            If _tabledoctypefacade Is Nothing Then
                _tabledoctypefacade = New TableDocTypeFacade()
            End If
            Return _tabledoctypefacade
        End Get
    End Property

    Public ReadOnly Property ResolutionLogFacade() As ResolutionLogFacade
        Get
            If _resolutionLogFacade Is Nothing Then
                _resolutionLogFacade = New ResolutionLogFacade
            End If
            Return _resolutionLogFacade
        End Get
    End Property

    Public ReadOnly Property ResolutionWorkflowFacade() As ResolutionWorkflowFacade
        Get
            If _resolutionWorkflowFacade Is Nothing Then
                _resolutionWorkflowFacade = New ResolutionWorkflowFacade
            End If
            Return _resolutionWorkflowFacade
        End Get
    End Property

    Public ReadOnly Property TabWorkflowFacade() As TabWorkflowFacade
        Get
            If _tabWorkflowFacade Is Nothing Then
                _tabWorkflowFacade = New TabWorkflowFacade
            End If
            Return _tabWorkflowFacade
        End Get
    End Property

    Public ReadOnly Property FileResolutionFacade() As FileResolutionFacade
        Get
            If _fileResolutionFacade Is Nothing Then
                _fileResolutionFacade = New FileResolutionFacade
            End If
            Return _fileResolutionFacade
        End Get
    End Property

    Public ReadOnly Property BidTypeFacade() As BidTypeFacade
        Get
            If _bidTypeFacade Is Nothing Then
                _bidTypeFacade = New BidTypeFacade()
            End If
            Return _bidTypeFacade
        End Get
    End Property

    Public ReadOnly Property ControllerStatusResolutionFacade() As ControllerStatusResolutionFacade
        Get
            If _controllerStatusResolutionFacade Is Nothing Then
                _controllerStatusResolutionFacade = New ControllerStatusResolutionFacade()
            End If
            Return _controllerStatusResolutionFacade
        End Get
    End Property

    Public ReadOnly Property ResolutionStatusFacade() As ResolutionStatusFacade
        Get
            If _resolutionStatusFacade Is Nothing Then
                _resolutionStatusFacade = New ResolutionStatusFacade()
            End If
            Return _resolutionStatusFacade
        End Get
    End Property

    Public ReadOnly Property ResolutionTypeFacade() As ResolutionTypeFacade
        Get
            If _resolutionTypeFacade Is Nothing Then
                _resolutionTypeFacade = New ResolutionTypeFacade()
            End If
            Return _resolutionTypeFacade
        End Get
    End Property

    Public ReadOnly Property ResolutionJournalFacade() As ResolutionJournalFacade
        Get
            If _resolutionJournalFacade Is Nothing Then
                _resolutionJournalFacade = New ResolutionJournalFacade()
            End If
            Return _resolutionJournalFacade
        End Get
    End Property
    Public ReadOnly Property ResolutionJournalDetailFacade() As ResolutionJournalDetailFacade
        Get
            If _resolutionJournalDetailFacade Is Nothing Then
                _resolutionJournalDetailFacade = New ResolutionJournalDetailFacade()
            End If
            Return _resolutionJournalDetailFacade
        End Get
    End Property

    Public ReadOnly Property ResolutionJournalTemplateFacade() As ResolutionJournalTemplateFacade
        Get
            If _resolutionJournalTemplateFacade Is Nothing Then
                _resolutionJournalTemplateFacade = New ResolutionJournalTemplateFacade()
            End If
            Return _resolutionJournalTemplateFacade
        End Get
    End Property

    Public ReadOnly Property ResolutionMessageFacade As ResolutionMessageFacade
        Get
            If _resolutionMessageFacade Is Nothing Then
                _resolutionMessageFacade = New ResolutionMessageFacade()
            End If
            Return _resolutionMessageFacade
        End Get
    End Property

    Public ReadOnly Property ServiceCodeDescriptorFacade As ServiceCodeDescriptorFacade
        Get
            If _serviceCodeDescriptorFacade Is Nothing Then
                _serviceCodeDescriptorFacade = New ServiceCodeDescriptorFacade()
            End If
            Return _serviceCodeDescriptorFacade
        End Get
    End Property

    Public ReadOnly Property WebPublicationFacade() As WebPublicationFacade
        Get
            If _webPublicationFacade Is Nothing Then
                _webPublicationFacade = New WebPublicationFacade()
            End If
            Return _webPublicationFacade
        End Get
    End Property

    Public ReadOnly Property ResolutionWPFacade As ResolutionWPFacade
        Get
            If _resolutionWpFacade Is Nothing Then
                _resolutionWpFacade = New ResolutionWPFacade()
            End If
            Return _resolutionWpFacade
        End Get
    End Property

    Public ReadOnly Property PrintLetterRoleFacade As PrintLetterRoleFacade
        Get
            If _printLetterRoleFacade Is Nothing Then
                _printLetterRoleFacade = New PrintLetterRoleFacade()
            End If
            Return _printLetterRoleFacade
        End Get
    End Property

    Public ReadOnly Property ResolutionActivityFacade As ResolutionActivityFacade
        Get
            If _resolutionActivityFacade Is Nothing Then
                _resolutionActivityFacade = New ResolutionActivityFacade()
            End If
            Return _resolutionActivityFacade
        End Get
    End Property
#End Region

#Region " PosteWeb facade properties "

    Public ReadOnly Property PosteOnLineRequestFacade() As PosteOnLineRequestFacade
        Get
            If _polRequestFacade Is Nothing Then
                _polRequestFacade = New PosteOnLineRequestFacade()
            End If
            Return _polRequestFacade
        End Get
    End Property


    Public ReadOnly Property PosteOnLineContactFacade() As PosteOnLineContactFacade
        Get
            If _polContactFacade Is Nothing Then
                _polContactFacade = New PosteOnLineContactFacade()
            End If
            Return _polContactFacade
        End Get
    End Property

    Public ReadOnly Property PosteOnLineAccountFacade() As PosteOnLineAccountFacade
        Get
            If _polAccountFacade Is Nothing Then
                _polAccountFacade = New PosteOnLineAccountFacade()
            End If
            Return _polAccountFacade
        End Get
    End Property

#End Region

#Region " Parer facade properties "

    Public ReadOnly Property ProtocolParerFacade() As ProtocolParerFacade
        Get
            If _protocolParerFacade Is Nothing Then
                _protocolParerFacade = New ProtocolParerFacade()
            End If
            Return _protocolParerFacade
        End Get
    End Property

    Public ReadOnly Property ResolutionParerFacade() As ResolutionParerFacade
        Get
            If _resolutionParerFacade Is Nothing Then
                _resolutionParerFacade = New ResolutionParerFacade()
            End If
            Return _resolutionParerFacade
        End Get
    End Property

#End Region

#Region " PEC all'Organo di Controllo "

    Public ReadOnly Property PECOCFacade() As PECOCFacade
        Get
            If _PECOCFacade Is Nothing Then
                _PECOCFacade = New PECOCFacade()
            End If
            Return _PECOCFacade
        End Get
    End Property

    Public ReadOnly Property PECOCLogFacade() As PECOCLogFacade
        Get
            If _PECOCLogFacade Is Nothing Then
                _PECOCLogFacade = New PECOCLogFacade()
            End If
            Return _PECOCLogFacade
        End Get
    End Property

#End Region

#Region " Message facade properties "

    Public ReadOnly Property MessageFacade() As MessageFacade
        Get
            If _messageFacade Is Nothing Then
                _messageFacade = New MessageFacade()
            End If
            Return _messageFacade
        End Get
    End Property

    Public ReadOnly Property MessageContactFacade() As MessageContactFacade
        Get
            If _messageContactFacade Is Nothing Then
                _messageContactFacade = New MessageContactFacade()
            End If
            Return _messageContactFacade
        End Get
    End Property

    Public ReadOnly Property MessageContactEmailFacade() As MessageContactEmailFacade
        Get
            If _messageContactEmailFacade Is Nothing Then
                _messageContactEmailFacade = New MessageContactEmailFacade()
            End If
            Return _messageContactEmailFacade
        End Get
    End Property

    Public ReadOnly Property MessageAttachmentFacade() As MessageAttachmentFacade
        Get
            If _messageAttachmentFacade Is Nothing Then
                _messageAttachmentFacade = New MessageAttachmentFacade()
            End If
            Return _messageAttachmentFacade
        End Get
    End Property

    Public ReadOnly Property MessageEmailFacade() As MessageEmailFacade
        Get
            If _messageEmailFacade Is Nothing Then
                _messageEmailFacade = New MessageEmailFacade()
            End If
            Return _messageEmailFacade
        End Get
    End Property

    Public ReadOnly Property MessageLogFacade() As MessageLogFacade
        Get
            If _messageLogFacade Is Nothing Then
                _messageLogFacade = New MessageLogFacade()
            End If
            Return _messageLogFacade
        End Get
    End Property

#End Region

#Region " Protocol Message facade properties "

    Public ReadOnly Property ProtocolMessageFacade As ProtocolMessageFacade
        Get
            If _protocolMessageFacade Is Nothing Then
                _protocolMessageFacade = New ProtocolMessageFacade()
            End If
            Return _protocolMessageFacade
        End Get
    End Property

#End Region

#Region " Document Series Properties "

    Public ReadOnly Property DocumentSeriesFacade() As DocumentSeriesFacade
        Get
            If _documentSeriesFacade Is Nothing Then
                _documentSeriesFacade = New DocumentSeriesFacade()
            End If
            Return _documentSeriesFacade
        End Get
    End Property

    Public ReadOnly Property DocumentSeriesFamilyFacade() As DocumentSeriesFamilyFacade
        Get
            If _documentSeriesFamilyFacade Is Nothing Then
                _documentSeriesFamilyFacade = New DocumentSeriesFamilyFacade()
            End If
            Return _documentSeriesFamilyFacade
        End Get
    End Property

    Public ReadOnly Property DocumentSeriesItemFacade() As DocumentSeriesItemFacade
        Get
            If _documentSeriesItemFacade Is Nothing Then
                _documentSeriesItemFacade = New DocumentSeriesItemFacade()
            End If
            Return _documentSeriesItemFacade
        End Get
    End Property

    Public ReadOnly Property DocumentSeriesItemMessageFacade() As DocumentSeriesItemMessageFacade
        Get
            If _documentSeriesItemMessageFacade Is Nothing Then
                DocumentSeriesItemMessageFacade = New DocumentSeriesItemMessageFacade()
            End If
            Return DocumentSeriesItemMessageFacade
        End Get
    End Property
    Public ReadOnly Property DocumentSeriesItemResolutionLinkFacade() As DocumentSeriesItemLinkFacade
        Get
            If _documentSeriesItemResolutionLinkFacade Is Nothing Then
                _documentSeriesItemResolutionLinkFacade = New DocumentSeriesItemLinkFacade()
            End If
            Return _documentSeriesItemResolutionLinkFacade
        End Get
    End Property

    Public ReadOnly Property DocumentSeriesItemRoleFacade() As DocumentSeriesItemRoleFacade
        Get
            If _documentSeriesItemRoleFacade Is Nothing Then
                _documentSeriesItemRoleFacade = New DocumentSeriesItemRoleFacade()
            End If
            Return _documentSeriesItemRoleFacade
        End Get
    End Property

    Public ReadOnly Property DocumentSeriesItemLogFacade() As DocumentSeriesItemLogFacade
        Get
            If _documentSeriesItemLogFacade Is Nothing Then
                _documentSeriesItemLogFacade = New DocumentSeriesItemLogFacade()
            End If
            Return _documentSeriesItemLogFacade
        End Get
    End Property

    Public ReadOnly Property DocumentSeriesIncrementalFacade As DocumentSeriesIncrementalFacade
        Get
            If _documentSeriesIncrementalFacade Is Nothing Then
                _documentSeriesIncrementalFacade = New DocumentSeriesIncrementalFacade()
            End If
            Return _documentSeriesIncrementalFacade
        End Get
    End Property

    Public ReadOnly Property DocumentSeriesAttributeBehaviourFacade() As DocumentSeriesAttributeBehaviourFacade
        Get
            If _documentSeriesAttributeBehaviourFacade Is Nothing Then
                _documentSeriesAttributeBehaviourFacade = New DocumentSeriesAttributeBehaviourFacade()
            End If
            Return _documentSeriesAttributeBehaviourFacade
        End Get
    End Property

    Public ReadOnly Property DocumentSeriesSubsectionFacade() As DocumentSeriesSubsectionFacade
        Get
            If _documentSeriesSubsectionFacade Is Nothing Then
                _documentSeriesSubsectionFacade = New DocumentSeriesSubsectionFacade()
            End If
            Return _documentSeriesSubsectionFacade
        End Get
    End Property

    Public ReadOnly Property ResolutionDocumentSeriesItemFacade() As ResolutionDocumentSeriesItemFacade
        Get
            If _resolutionDocumentSeriesItemFacade Is Nothing Then
                _resolutionDocumentSeriesItemFacade = New ResolutionDocumentSeriesItemFacade()
            End If
            Return _resolutionDocumentSeriesItemFacade
        End Get
    End Property

    Public ReadOnly Property ContainerBehaviourFacade() As ContainerBehaviourFacade
        Get
            If _containerBehaviourFacade Is Nothing Then
                _containerBehaviourFacade = New ContainerBehaviourFacade()
            End If
            Return _containerBehaviourFacade
        End Get
    End Property

#End Region

    Public ReadOnly Property TenderHeaderFacade() As TenderHeaderFacade
        Get
            If _tenderHeaderFacade Is Nothing Then
                _tenderHeaderFacade = New TenderHeaderFacade()
            End If
            Return _tenderHeaderFacade
        End Get
    End Property
    Public ReadOnly Property TenderLotFacade() As TenderLotFacade
        Get
            If _tenderLotFacade Is Nothing Then
                _tenderLotFacade = New TenderLotFacade()
            End If
            Return _tenderLotFacade
        End Get
    End Property

    Public ReadOnly Property ProtocolDocumentSeriesItemFacade() As ProtocolDocumentSeriesItemFacade
        Get
            If _protocolDocumentSeriesItemFacade Is Nothing Then
                _protocolDocumentSeriesItemFacade = New ProtocolDocumentSeriesItemFacade()
            End If
            Return _protocolDocumentSeriesItemFacade
        End Get
    End Property
End Class
