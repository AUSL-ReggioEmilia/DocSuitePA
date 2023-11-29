Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Entity.Conservations
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Conservations

Public Class ProtocolRights

#Region " Fields "

    Private _sourceProtocol As Protocol
    Private ReadOnly _statusCancel As Boolean
    Private _currentGroups As String()
    Private _currentSecurityGroups As IList(Of SecurityGroups)
    Private _currentFacade As ProtocolFacade
    Private _currentSecurityUsersFacade As SecurityUsersFacade
    Private _isDocumentsReadable As Boolean?
    Private _isReadable As Boolean?
    Private _isPreviewable As Boolean?
    Private _isEditable As Boolean?
    Private _isIncludable As Boolean?
    Private _isRiparable As Boolean?
    Private _isEditableAttachment As Boolean?
    Private _isFlushAnnexedEnable As Boolean?
    Private _isInteroperable As Boolean?
    Private _isPECSendable As Boolean?
    Private _isPECAnswerable As Boolean?
    Private _isTNoticeSendable As Boolean?
    Private _isErrorEditable As Boolean?
    Private _isLinkable As Boolean?
    Private _isAuthorizable As Boolean?
    Private _isRoleAuthorized As Boolean?
    Private _isDistributable As Boolean?
    Private _isCurrentUserDistributionManager As Boolean?
    Private _isDistributionAssigned As Boolean?
    Private _isDistributionAssignedToMe As Boolean?
    Private _isCurrentUserDistributionCc As Boolean?
    Private _isContainerDistributable As Boolean?
    Private _isLogSecurity As Boolean?
    Private _isCancelable As Boolean?
    Private _isRejectable As Boolean?
    Private _isRejected As Boolean?
    Private _isHighlightViewable As Boolean?
    Private _isUserAuthorized As Boolean?
    Private _hasPrivacyAuthorizations As Boolean?
    Private _isProtocolDistributableType As Boolean?
    Private _isArchivable As Boolean?
    Private _currentConservation As Conservation
    Private _refusedRolesViewable As Boolean?

#End Region

#Region " Constructors "

    Public Sub New(ByVal protocol As Protocol)
        Me.New(protocol, False)
    End Sub

    Public Sub New(ByVal protocol As Protocol, ByVal statusCancel As Boolean)
        SourceProtocol = protocol
        _statusCancel = statusCancel
    End Sub

#End Region

#Region " Properties "

    Public Property ContainerRightDictionary As Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer))

    Public Property RoleRightDictionary As Dictionary(Of ProtocolRoleRightPositions, IList(Of Integer))

    Public Property SourceProtocol As Protocol
        Get
            Return _sourceProtocol
        End Get
        Set(ByVal value As Protocol)
            _sourceProtocol = value

            ResetInternalStatus()
        End Set
    End Property

    Private ReadOnly Property StatusCancel As Boolean
        Get
            Return _statusCancel
        End Get
    End Property

    Private ReadOnly Property CurrentGroups As String()
        Get
            If _currentGroups Is Nothing Then
                _currentGroups = CommonUtil.GetArrayUserFromString()
            End If
            Return _currentGroups
        End Get
    End Property

    Private ReadOnly Property CurrentSecurityGroups As IList(Of SecurityGroups)
        Get
            If _currentSecurityGroups Is Nothing Then
                _currentSecurityGroups = CurrentSecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName)
            End If
            Return _currentSecurityGroups
        End Get
    End Property

    Private ReadOnly Property CurrentFacade As ProtocolFacade
        Get
            If _currentFacade Is Nothing Then
                _currentFacade = New ProtocolFacade
            End If
            Return _currentFacade
        End Get
    End Property

    Private ReadOnly Property CurrentSecurityUsersFacade As SecurityUsersFacade
        Get
            If _currentSecurityUsersFacade Is Nothing Then
                _currentSecurityUsersFacade = New SecurityUsersFacade
            End If
            Return _currentSecurityUsersFacade
        End Get
    End Property

    Private Shared ReadOnly Property CurrentEnv As ProtocolEnv
        Get
            Return DocSuiteContext.Current.ProtocolEnv
        End Get
    End Property
    ''' <summary> Inserimento. </summary>
    Public ReadOnly Property IsIncludable As Boolean
        Get
            If _isIncludable.HasValue Then
                Return _isIncludable.Value
            End If
            If ContainerRightDictionary IsNot Nothing Then
                _isIncludable = CheckRight(ProtocolContainerRightPositions.Insert)
                Return _isIncludable.Value
            End If

            _isIncludable = CurrentFacade.SecurityGroupsUserRight(SourceProtocol, CurrentSecurityGroups, ProtocolContainerRightPositions.Insert, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.View, StatusCancel, DocSuiteContext.Current.User.FullUserName)

            Return _isIncludable.GetValueOrDefault(False)
        End Get
    End Property
    ''' <summary> Visualizzazione documento. </summary>
    Public ReadOnly Property IsDocumentReadable As Boolean
        Get
            If _isDocumentsReadable.HasValue Then
                Return _isDocumentsReadable.Value
            End If
            _isDocumentsReadable = IsReadable

            If DocSuiteContext.Current.SimplifiedPrivacyEnabled Then
                If HasPrivacyAuthorizations AndAlso Not IsUserAuthorized Then
                    Dim privacyRoles As Guid() = SourceProtocol.Roles.Where(Function(t) Not String.IsNullOrEmpty(t.Type) AndAlso t.Type.Equals(ProtocolRoleTypes.Privacy)).Select(Function(r) r.Role.UniqueId).ToArray()
                    Dim publicRoles As Guid() = SourceProtocol.Roles.Where(Function(t) String.IsNullOrEmpty(t.Type) OrElse Not t.Type.Equals(ProtocolRoleTypes.Privacy)).Select(Function(r) r.Role.UniqueId).ToArray()

                    _isDocumentsReadable = FacadeFactory.Instance.RoleFacade.CheckCurrentUserPrivacyRoles(publicRoles, privacyRoles, DSWEnvironment.Protocol)
                End If
            End If
            _isDocumentsReadable = _isDocumentsReadable OrElse IsUserAuthorized OrElse IsHighilightViewable

            Return _isDocumentsReadable.Value
        End Get
    End Property

    ''' <summary> Visualizzazione. </summary>
    Public ReadOnly Property IsReadable As Boolean
        Get
            If _isReadable.HasValue Then
                Return _isReadable.Value
            End If

            ' Per AUSL-PC concedo i permessi di lettura per tutti i protocolli di mia registrazione.
            If CurrentEnv.CorporateAcronym.ContainsIgnoreCase("AUSL-PC") AndAlso SourceProtocol.RegistrationUser.Eq(DocSuiteContext.Current.User.FullUserName) Then
                _isReadable = True
                Return _isReadable.Value
            End If

            ''Se il protocollo è rigettato, allora i documenti sono visualizzabili solo da chi ha diritto di modifica sul contenitore di rigetto
            If CurrentEnv.ProtocolRejectionEnabled AndAlso SourceProtocol.IdStatus = ProtocolStatusId.Rejected AndAlso SourceProtocol.Container.Id = CurrentEnv.ProtocolRejectionContainerId Then
                _isReadable = FacadeFactory.Instance.ContainerFacade.CheckContainerRight(CurrentEnv.ProtocolRejectionContainerId, DSWEnvironment.Protocol, ProtocolContainerRightPositions.Modify, True)
                Return _isReadable.Value
            End If

            If ContainerRightDictionary IsNot Nothing Then
                _isReadable = CheckRight(ProtocolContainerRightPositions.View, ProtocolRoleRightPositions.Enabled)
                Return _isReadable.Value
            End If

            _isReadable = CurrentFacade.SecurityGroupsUserRight(SourceProtocol, CurrentSecurityGroups, ProtocolContainerRightPositions.View, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.View, StatusCancel, DocSuiteContext.Current.User.FullUserName)

            Return _isReadable.GetValueOrDefault(False)
        End Get
    End Property

    ''' <summary>
    ''' Verifica se si hanno i diritti per vedere i log di protocollo
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property EnableViewLog As Boolean
        Get
            If CommonShared.HasGroupAdministratorRight() Then
                Return True
            End If

            If String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.EnvGroupLogView) Then
                Return IsLogSecurity
            Else
                Return CommonShared.HasGroupLogViewRight()
            End If
        End Get
    End Property
    ''' <summary> Sommario. </summary>
    Public ReadOnly Property IsPreviewable As Boolean
        Get
            If _isPreviewable.HasValue Then
                Return _isPreviewable.Value
            End If

            ' Per AUSL-PC concedo i permessi di lettura per tutti i protocolli di mia registrazione.
            If CurrentEnv.CorporateAcronym.ContainsIgnoreCase("AUSL-PC") Then
                If SourceProtocol.RegistrationUser.Eq(DocSuiteContext.Current.User.FullUserName) Then
                    _isPreviewable = True
                    Return _isPreviewable.Value
                End If
            End If

            ''Se il protocollo è rigettato, allora è visualizzabile da sommario solo da chi ha diritto di modifica sul contenitore di rigetto
            If CurrentEnv.ProtocolRejectionEnabled AndAlso SourceProtocol.IdStatus = ProtocolStatusId.Rejected AndAlso SourceProtocol.Container.Id = CurrentEnv.ProtocolRejectionContainerId Then
                _isPreviewable = FacadeFactory.Instance.ContainerFacade.CheckContainerRight(CurrentEnv.ProtocolRejectionContainerId, DSWEnvironment.Protocol, ProtocolContainerRightPositions.Modify, True)
                Return _isPreviewable.Value
            End If

            If ContainerRightDictionary IsNot Nothing Then
                _isPreviewable = CheckRight(ProtocolContainerRightPositions.Preview, ProtocolRoleRightPositions.Enabled)
                Return _isPreviewable.Value
            End If

            _isPreviewable = CurrentFacade.SecurityGroupsUserRight(SourceProtocol, CurrentSecurityGroups, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.View, StatusCancel, DocSuiteContext.Current.User.FullUserName)

            Return _isPreviewable.GetValueOrDefault(False)
        End Get
    End Property

    Public ReadOnly Property IsEditable As Boolean
        Get
            If _isEditable.HasValue Then
                Return _isEditable.Value
            End If

            ''Se il protocollo è rigettato, allora è modificabile solo da chi ha diritto di modifica sul contenitore di rigetto
            If CurrentEnv.ProtocolRejectionEnabled AndAlso SourceProtocol.IdStatus = ProtocolStatusId.Rejected AndAlso SourceProtocol.Container.Id = CurrentEnv.ProtocolRejectionContainerId Then
                _isEditable = FacadeFactory.Instance.ContainerFacade.CheckContainerRight(CurrentEnv.ProtocolRejectionContainerId, DSWEnvironment.Protocol, ProtocolContainerRightPositions.Modify, True)
                Return _isEditable.Value
            End If

            If ContainerRightDictionary IsNot Nothing Then
                _isEditable = CheckRight(ProtocolContainerRightPositions.Modify)
                Return _isEditable.Value
            End If

            _isEditable = CurrentFacade.SecurityGroupsUserRight(SourceProtocol, CurrentSecurityGroups, ProtocolContainerRightPositions.Modify, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.View, StatusCancel, DocSuiteContext.Current.User.FullUserName)

            Return _isEditable.GetValueOrDefault(False)
        End Get
    End Property


    Public ReadOnly Property IsEditableAttachment As Nullable(Of Boolean)
        Get
            If _isEditableAttachment Is Nothing Then
                _isEditableAttachment = CurrentEnv.EditableAttachment AndAlso (IsEditable OrElse IsReadable)
            End If
            Return _isEditableAttachment
        End Get
    End Property

    Public ReadOnly Property IsFlushAnnexedEnable As Boolean
        Get
            If Not _isFlushAnnexedEnable.HasValue Then
                _isFlushAnnexedEnable = False
                If Not SourceProtocol.IdAnnexed.Equals(Guid.Empty) Then
                    _isFlushAnnexedEnable = IsEditable
                End If
            End If
            Return _isFlushAnnexedEnable.Value
        End Get
    End Property

    Public ReadOnly Property IsInteroperable As Boolean
        Get
            If _isInteroperable Is Nothing Then
                _isInteroperable = CurrentEnv.IsPECEnabled AndAlso CurrentEnv.IsInteropEnabled AndAlso (DocSuiteContext.Current.ProtocolEnv.ProtNewPECMailEnabled OrElse SourceProtocol.Type.ShortDescription.Eq("U")) _
                    AndAlso CurrentFacade.SecurityGroupsUserRight(SourceProtocol, CurrentSecurityGroups, ProtocolContainerRightPositions.InteropOut, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.View, StatusCancel, DocSuiteContext.Current.User.FullUserName)
            End If
            Return _isInteroperable.Value
        End Get
    End Property

    ''' <summary> Determina se è possibile mandare pec. </summary>
    Public ReadOnly Property IsPECSendable As Boolean
        Get
            If _isPECSendable Is Nothing Then
                'Se il protocollo è in uscita è sempre inviabile per PEC
                'se invece è ingresso lo è solo con il nuovo meccanismo di invio PEC
                _isPECSendable = CurrentEnv.IsPECEnabled AndAlso (DocSuiteContext.Current.ProtocolEnv.ProtNewPECMailEnabled OrElse SourceProtocol.Type.ShortDescription.Eq("U")) _
                    AndAlso CurrentFacade.SecurityGroupsUserRight(SourceProtocol, CurrentSecurityGroups, ProtocolContainerRightPositions.PECOut, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.View, StatusCancel, DocSuiteContext.Current.User.FullUserName)
            End If
            Return _isPECSendable.Value
        End Get
    End Property

    Public ReadOnly Property IsTNoticeSendable As Boolean
        Get
            If _isTNoticeSendable Is Nothing Then
                _isTNoticeSendable = CurrentEnv.TNoticeEnabled AndAlso SourceProtocol.Type.ShortDescription.Eq("U") _
                    AndAlso CurrentFacade.SecurityGroupsUserRight(SourceProtocol, CurrentSecurityGroups, ProtocolContainerRightPositions.PECOut, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.View, StatusCancel, DocSuiteContext.Current.User.FullUserName)
            End If
            Return _isTNoticeSendable.Value
        End Get
    End Property

    Public ReadOnly Property IsPECAnswerable As Nullable(Of Boolean)
        Get
            If _isPECAnswerable Is Nothing Then
                _isPECAnswerable = CurrentEnv.IsPECEnabled AndAlso SourceProtocol.Type.ShortDescription.Eq("I") AndAlso SourceProtocol.PecMails IsNot Nothing AndAlso SourceProtocol.PecMails.Count > 0
            End If
            Return _isPECAnswerable
        End Get
    End Property

    Public ReadOnly Property IsErrorEditable As Nullable(Of Boolean)
        Get
            If _isErrorEditable Is Nothing Then
                _isErrorEditable = CurrentFacade.SecurityGroupsUserRight(SourceProtocol, CurrentSecurityGroups, ProtocolContainerRightPositions.Modify, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.View, False, True, True, DocSuiteContext.Current.User.FullUserName)
            End If
            Return _isErrorEditable
        End Get
    End Property

    Public ReadOnly Property IsLinkable As Boolean
        Get
            If _isLinkable Is Nothing Then
                _isLinkable = CurrentFacade.SecurityGroupsUserRight(SourceProtocol, CurrentSecurityGroups, DocSuiteContext.Current.ProtocolEnv.LinkSecurity, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.View, DocSuiteContext.Current.User.FullUserName)
            End If
            Return _isLinkable.Value
        End Get
    End Property

    Public ReadOnly Property IsAuthorizable As Boolean
        Get
            If _isAuthorizable IsNot Nothing Then
                Return _isAuthorizable.Value
            End If

            If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
                _isAuthorizable = CurrentFacade.SecurityGroupsUserRole(SourceProtocol, CurrentSecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName), "11") _
                        OrElse CurrentFacade.SecurityGroupsUserRight(SourceProtocol, CurrentSecurityGroups, ProtocolContainerRightPositions.DocDistribution, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.View, DocSuiteContext.Current.User.FullUserName)
            Else
                _isAuthorizable = CurrentFacade.SecurityGroupsUserRight(SourceProtocol, CurrentSecurityGroups, DocSuiteContext.Current.ProtocolEnv.AuthorizedSecurity, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.View, DocSuiteContext.Current.User.FullUserName)
            End If

            ' Se esiste parametro di "AuthorizableIfRoleAutorized" allora per essere autorizzabile basta aver l'autorizzazione sul settore
            If Not _isAuthorizable.GetValueOrDefault(False) AndAlso CurrentEnv.AuthorizableIfRoleAutorized Then
                _isAuthorizable = IsRoleAuthorized
            End If

            Return _isAuthorizable.Value
        End Get
    End Property

    ''' <summary> Controlla che l'utente di un settore autorizzato in visualizzazione possa autorizzare a sua volta il documento ad altri settori. </summary>
    Public ReadOnly Property IsRoleAuthorized As Boolean
        Get
            If _isRoleAuthorized.HasValue Then
                Return _isRoleAuthorized.Value
            End If
            _isRoleAuthorized = CurrentFacade.SecurityGroupsUserRight(SourceProtocol, CurrentSecurityGroups, ProtocolContainerRightPositions.View, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.View, StatusCancel, False, False, DocSuiteContext.Current.User.FullUserName)
            Return _isRoleAuthorized.GetValueOrDefault(False)
        End Get
    End Property

    ''' <summary> Indica se sono disponibili i diritti di distribuzione </summary>
    Public ReadOnly Property IsDistributable As Boolean
        Get
            If _isDistributable Is Nothing Then
                ' Verifico di avere il permesso di distribuzione a livello di contenitore - OPPURE - di essere manager del settore autorizzato.
                _isDistributable = IsContainerDistributable OrElse IsCurrentUserDistributionManager
            End If
            Return _isDistributable.Value
        End Get
    End Property

    Public ReadOnly Property IsDistributionAssigned As Boolean
        Get
            If Not _isDistributionAssigned.HasValue Then
                _isDistributionAssigned = Not SourceProtocol.RoleUsers.IsNullOrEmpty()
            End If
            Return _isDistributionAssigned.GetValueOrDefault(False)
        End Get
    End Property

    Public ReadOnly Property IsDistributionAssignedToMe As Boolean
        Get
            If Not _isDistributionAssignedToMe.HasValue Then
                _isDistributionAssignedToMe = CurrentFacade.SecurityGroupsUserAuthoriz(SourceProtocol, CurrentSecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName), DocSuiteContext.Current.User.FullUserName)
            End If
            Return _isDistributionAssignedToMe.GetValueOrDefault(False)
        End Get
    End Property

    Public ReadOnly Property IsCurrentUserDistributionManager As Nullable(Of Boolean)
        Get
            If _isCurrentUserDistributionManager Is Nothing Then
                ' Verifico di avere il permesso di essere manager del settore autorizzato.
                _isCurrentUserDistributionManager = CurrentFacade.SecurityGroupsUserRole(SourceProtocol, CurrentSecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName), "11")
            End If
            Return _isCurrentUserDistributionManager
        End Get
    End Property

    Public ReadOnly Property IsCurrentUserDistributionCc As Nullable(Of Boolean)
        Get
            If _isCurrentUserDistributionCc Is Nothing Then
                _isCurrentUserDistributionCc = CurrentFacade.CheckProtocolRoleCC(SourceProtocol, CurrentGroups, True)
            End If
            Return _isCurrentUserDistributionCc
        End Get
    End Property

    Public ReadOnly Property IsContainerDistributable As Boolean
        Get
            If _isContainerDistributable Is Nothing Then
                ' Verifico di avere il permesso di distribuzione a livello di contenitore.
                _isContainerDistributable = CurrentFacade.SecurityGroupsUserRight(SourceProtocol, CurrentSecurityGroups, ProtocolContainerRightPositions.DocDistribution, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.View, DocSuiteContext.Current.User.FullUserName)
            End If
            Return _isContainerDistributable.Value
        End Get
    End Property

    Public ReadOnly Property IsLogSecurity As Boolean
        Get
            If _isLogSecurity Is Nothing Then
                _isLogSecurity = CurrentFacade.SecurityGroupsUserRight(SourceProtocol, CurrentSecurityGroups, DocSuiteContext.Current.ProtocolEnv.LogSecurity, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.View, DocSuiteContext.Current.User.FullUserName)
            End If
            Return _isLogSecurity.Value
        End Get
    End Property

    Public ReadOnly Property IsCancelable As Boolean
        Get
            If _isCancelable.HasValue Then
                Return _isCancelable.Value
            End If
            _isCancelable = CurrentFacade.SecurityGroupsUserRight(SourceProtocol, CurrentSecurityGroups, ProtocolContainerRightPositions.Cancel, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.View, DocSuiteContext.Current.User.FullUserName)
            Return _isCancelable.GetValueOrDefault(False)
        End Get
    End Property

    ''' <summary> Indica se il protocollo è rigettabile. </summary>
    Public ReadOnly Property IsRejectable As Boolean
        Get
            If _isRejectable.HasValue Then
                Return _isRejectable.Value
            End If

            If CurrentEnv.ProtocolRejectionEnabled Then
                If CurrentFacade.RejectionContainer Is Nothing Then
                    Throw New DocSuiteException("Parametro non configurato", "Nessun contenitore per il rigetto specificato tra i parametri di protocollo.")
                End If

                _isRejectable = SourceProtocol.IdStatus.GetValueOrDefault(ProtocolStatusId.Attivo) <> ProtocolStatusId.Rejected _
                    AndAlso FacadeFactory.Instance.OChartFacade.IsRejectionContainer(SourceProtocol.Container)
            Else
                _isRejectable = False
            End If

            Return _isRejectable.Value
        End Get
    End Property

    ''' <summary> Indica se il protocollo è rigettato. </summary>
    Public ReadOnly Property IsRejected As Boolean
        Get
            If _isRejected.HasValue Then
                Return _isRejected.Value
            End If

            If CurrentEnv.ProtocolRejectionEnabled Then
                If CurrentFacade.RejectionContainer Is Nothing Then
                    Throw New DocSuiteException("Parametro non configurato", "Nessun contenitore per il rigetto specificato tra i parametri di protocollo.")
                End If

                _isRejected = SourceProtocol.Container.Id = CurrentEnv.ProtocolRejectionContainerId AndAlso SourceProtocol.IdStatus.GetValueOrDefault(ProtocolStatusId.Attivo) = ProtocolStatusId.Rejected
            Else
                _isRejected = False
            End If

            Return _isRejected.Value
        End Get
    End Property

    Public ReadOnly Property CanAddDocuments() As Boolean
        Get
            If Not String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.EnvGroupModifyAttachmentProt) Then
                Return CommonShared.UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.EnvGroupModifyAttachmentProt)
            End If
            Return True
        End Get
    End Property

    Public ReadOnly Property IsHighilightViewable As Boolean
        Get
            If Not _isHighlightViewable.HasValue Then
                _isHighlightViewable = DocSuiteContext.Current.ProtocolEnv.ProtocolHighlightEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.ProtocolHighlightSecurityEnabled AndAlso Not IsPreviewable _
                AndAlso SourceProtocol.Users IsNot Nothing AndAlso SourceProtocol.Users.Any(Function(p) p.Account.Eq(DocSuiteContext.Current.User.FullUserName) AndAlso p.Type = ProtocolUserType.Highlight)
            End If
            Return _isHighlightViewable.Value
        End Get
    End Property

    Public ReadOnly Property HasPrivacyAuthorizations As Boolean
        Get
            If Not _hasPrivacyAuthorizations.HasValue Then
                _hasPrivacyAuthorizations = DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso SourceProtocol.Roles.Any(Function(r) Not String.IsNullOrEmpty(r.Type) AndAlso r.Type = ProtocolRoleTypes.Privacy)
            End If
            Return _hasPrivacyAuthorizations.Value
        End Get
    End Property

    Public ReadOnly Property IsUserAuthorized As Boolean
        Get
            If Not _isUserAuthorized.HasValue Then
                _isUserAuthorized = DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso SourceProtocol.Users.Any(Function(u) u.Account.Eq(DocSuiteContext.Current.User.FullUserName) AndAlso u.Type = ProtocolUserType.Authorization)
            End If
            Return _isUserAuthorized.Value
        End Get
    End Property

    Public ReadOnly Property IsProtocolTypeDistributable As Boolean
        Get
            If Not _isProtocolDistributableType.HasValue Then
                _isProtocolDistributableType = DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.ProtocolDistributionTypologies.Contains(SourceProtocol.Type.Id)
            End If
            Return _isProtocolDistributableType.Value
        End Get
    End Property

    Public ReadOnly Property HasHighlightRights As Boolean
        Get
            Return DocSuiteContext.Current.ProtocolEnv.ProtocolHighlightEnabled AndAlso SourceProtocol.Users.Any(Function(x) x.Account.Eq(DocSuiteContext.Current.User.FullUserName) AndAlso x.Type = ProtocolUserType.Highlight)
        End Get
    End Property

    Public ReadOnly Property IsArchivable As Boolean
        Get
            If Not _isArchivable.HasValue Then
                _isArchivable = IsEditable OrElse HasHighlightRights OrElse IsRoleAuthorized
            End If
            Return _isArchivable.Value
        End Get
    End Property

    Private ReadOnly Property CurrentConservation As Conservation
        Get
            If _currentConservation Is Nothing Then
                Dim conservation As Conservation = WebAPIImpersonatorFacade.ImpersonateFinder(New ConservationFinder(DocSuiteContext.Current.CurrentTenant),
                    Function(impersonationType, finder)
                        finder.UniqueId = SourceProtocol.Id
                        finder.EnablePaging = False
                        Return finder.DoSearch().Select(Function(x) x.Entity).FirstOrDefault()
                    End Function)

                _currentConservation = conservation
            End If
            Return _currentConservation
        End Get
    End Property

    Public ReadOnly Property IsConservated As Boolean
        Get
            Return CurrentConservation IsNot Nothing AndAlso CurrentConservation.Status = ConservationStatus.Conservated
        End Get
    End Property

    Public ReadOnly Property RefusedRolesViewable As Boolean
        Get
            If Not _refusedRolesViewable.HasValue Then
                _refusedRolesViewable = ((DocSuiteContext.Current.ProtocolEnv.RefusedProtocolAuthorizationEnabled AndAlso CommonShared.HasRefusedProtocolGroupsRight) OrElse
                    (DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.DistributionRejectableEnabled)) AndAlso
                    SourceProtocol.RejectedRoles.Any(Function(r) r.Status = ProtocolRoleStatus.Refused)
            End If
            Return _refusedRolesViewable.Value
        End Get
    End Property

#End Region

#Region " Methods "

    Private Function CheckRight(containerRight As ProtocolContainerRightPositions) As Boolean
        Return CheckRight(containerRight, Nothing)
    End Function

    Private Function CheckRight(containerRight As ProtocolContainerRightPositions, roleRight As ProtocolRoleRightPositions?) As Boolean
        If ContainerRightDictionary.ContainsKey(containerRight) Then
            If ContainerRightDictionary(containerRight).Contains(SourceProtocol.Container.Id) Then
                Return True
            End If
        End If

        If roleRight.HasValue AndAlso Not DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
            If RoleRightDictionary.ContainsKey(roleRight.Value) Then
                Return SourceProtocol.Roles.Any(Function(pr) RoleRightDictionary(ProtocolRoleRightPositions.Enabled).Contains(pr.Role.Id))
            End If
        End If

        If roleRight.HasValue AndAlso DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
            If RoleRightDictionary.ContainsKey(roleRight.Value) Then
                Return (IsCurrentUserDistributionManager.HasValue AndAlso IsCurrentUserDistributionManager.Value) OrElse IsDistributionAssignedToMe
            End If
        End If

        Return False
    End Function

    Private Sub ResetInternalStatus()
        _currentGroups = Nothing
        _currentFacade = Nothing
        _isReadable = Nothing
        _isEditable = Nothing
        _isEditableAttachment = Nothing
        _isInteroperable = Nothing
        _isPECSendable = Nothing
        _isPECAnswerable = Nothing
        _isPreviewable = Nothing
        _isDistributable = Nothing
        _isErrorEditable = Nothing
        _isLinkable = Nothing
        _isAuthorizable = Nothing
        _isDistributable = Nothing
        _isContainerDistributable = Nothing
        _isLogSecurity = Nothing
        _isCancelable = Nothing
        _isRejectable = Nothing
    End Sub

#End Region

End Class
