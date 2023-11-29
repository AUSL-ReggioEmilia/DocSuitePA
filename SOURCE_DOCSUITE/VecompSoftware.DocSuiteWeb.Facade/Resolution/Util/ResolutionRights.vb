Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Entity.Conservations
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Conservations

Public Class ResolutionRights

#Region "[ Fields ]"
    Private _facade As FacadeFactory
    Private _currentResolution As Resolution
    Private _statusCancel As Boolean
    Private _currentFacade As ResolutionFacade
    Private _currentContainerFacade As ContainerFacade
    Private _isPrivacyAttachmentAllowed As Nullable(Of Boolean)
    ' DIRITTI
    Private _isAdoptable As Boolean?
    Private _isAdministrable As Boolean?
    Private _isPreviewable As Boolean?
    Private _canInsertInContainer As Boolean?
    Private _isExecutive As Boolean?
    Private _isCancelable As Boolean?
    Private _isViewable As Boolean?
    Private _isResolutionExecutive As Boolean?
    Private _isFlushAnnexedEnable As Boolean?
    Private _isPrivacyViewable As Boolean?
    Private _canExecutiveModifyOC As Boolean?
    Private _hasCurrentStepVisibilityRights As Boolean?
    Private _isResponsibleUser As Boolean?
    Private _isAccountedUser As Boolean?
    Private _isConsultedUser As Boolean?
    Private _isInformedUser As Boolean?
    Private _hasCurrentStepFlowResponsabilityRights As Boolean?
    Private _currentConservation As Conservation
#End Region

#Region "[ _ctor ]"
    Public Sub New(ByVal resolution As Resolution)
        Me.New(resolution, False)
    End Sub
    Public Sub New(ByVal resolution As Resolution, ByVal statusCancel As Boolean)
        CurrentResolution = resolution
        _statusCancel = statusCancel
    End Sub
#End Region

#Region "[ Properties ]"
    Private ReadOnly Property Facade As FacadeFactory
        Get
            If _facade Is Nothing Then
                _facade = New FacadeFactory()
            End If
            Return _facade
        End Get
    End Property
    Private Property CurrentResolution As Resolution
        Get
            Return _currentResolution
        End Get
        Set(ByVal value As Resolution)
            _currentResolution = value

            _currentFacade = Nothing
            _currentContainerFacade = Nothing
            _isPrivacyAttachmentAllowed = Nothing
        End Set
    End Property
    Private ReadOnly Property StatusCancel As Boolean
        Get
            Return _statusCancel
        End Get
    End Property
    Private ReadOnly Property CurrentFacade As ResolutionFacade
        Get
            If _currentFacade Is Nothing Then
                _currentFacade = New ResolutionFacade
            End If
            Return _currentFacade
        End Get
    End Property
    Private ReadOnly Property CurrentContainerFacade As ContainerFacade
        Get
            If _currentContainerFacade Is Nothing Then
                _currentContainerFacade = New ContainerFacade
            End If
            Return _currentContainerFacade
        End Get
    End Property
    Public ReadOnly Property IsPrivacyAttachmentAllowed As Boolean
        Get
            If _isPrivacyAttachmentAllowed Is Nothing Then
                ' Verifico sia abilitata la gestione degli allegati riservati per il workflow corrente e che l'utente corrente abbia i permessi sul contenitore.
                _isPrivacyAttachmentAllowed = CurrentFacade.IsManagedProperty("IdPrivacyAttachments", CurrentResolution.Type.Id) _
                    AndAlso CurrentResolution.Container IsNot Nothing _
                    AndAlso CurrentContainerFacade.CheckContainerRight(CurrentResolution.Container.Id, DSWEnvironment.Resolution, ResolutionRightPositions.PrivacyAttachments, Nothing)
            End If
            Return _isPrivacyAttachmentAllowed.Value
        End Get
    End Property

    'TODO: Cancellare il parametro
    Public ReadOnly Property IsProposalViewable() As Boolean
        Get
            If CurrentResolution.ResolutionWorkflows(0).IsActive <> 1 Then
                ' se la Resolution è stata adottata, il documento di proposta può essere visualizzato
                Return True
            End If

            ' Condizioni di verifica per cui ad un operatore viene negato l'accesso al documento di proposta
            If Not DocSuiteContext.Current.ResolutionEnv.ProposalViewable AndAlso
                Not CommonUtil.UserConnectedBelongsTo(DocSuiteContext.Current.ResolutionEnv.EnvGroupPropostaAtto) Then

                Return VerifyResolutionProposer()
            End If

            Return IsDocumentViewable()

        End Get
    End Property

    Public ReadOnly Property IsViewable() As Boolean
        Get
            If Not _isViewable.HasValue Then
                _isViewable = Facade.ResolutionFacade.CheckUserRights(CurrentResolution, ResolutionRightPositions.View)
            End If
            Return _isViewable.Value
        End Get
    End Property
    Public ReadOnly Property IsAdoptable() As Boolean
        Get
            If Not _isAdoptable.HasValue Then
                _isAdoptable = Facade.ResolutionFacade.CheckUserRights(CurrentResolution, ResolutionRightPositions.Adoption)
            End If
            Return _isAdoptable.Value
        End Get
    End Property
    Public ReadOnly Property IsAdministrable() As Boolean
        Get
            If Not _isAdministrable.HasValue Then
                _isAdministrable = Facade.ResolutionFacade.CheckUserRights(CurrentResolution, ResolutionRightPositions.Administration)
            End If
            Return _isAdministrable.Value
        End Get
    End Property
    Public ReadOnly Property IsPreviewable() As Boolean
        Get
            If Not _isPreviewable.HasValue Then
                _isPreviewable = Facade.ResolutionFacade.CheckUserRights(CurrentResolution, ResolutionRightPositions.Preview)
            End If
            Return _isPreviewable.Value
        End Get
    End Property
    Public ReadOnly Property CanInsertInContainer() As Boolean
        Get
            If Not _canInsertInContainer.HasValue Then
                _canInsertInContainer = Facade.ResolutionFacade.CheckUserRights(CurrentResolution, ResolutionRightPositions.Insert)
            End If
            Return _canInsertInContainer.Value
        End Get
    End Property
    Public ReadOnly Property IsExecutive() As Boolean
        Get
            If Not _isExecutive.HasValue Then
                _isExecutive = Facade.ResolutionFacade.CheckUserRights(CurrentResolution, ResolutionRightPositions.Executive)
            End If
            Return _isExecutive.Value
        End Get
    End Property
    Public ReadOnly Property IsCancelable() As Boolean
        Get
            If Not _isCancelable.HasValue Then
                _isCancelable = Facade.ResolutionFacade.CheckUserRights(CurrentResolution, ResolutionRightPositions.Cancel)
            End If
            Return _isCancelable.Value

        End Get
    End Property

    Public ReadOnly Property CanExecutiveModifyOC As Boolean
        Get
            If Not _canExecutiveModifyOC.HasValue Then
                _canExecutiveModifyOC = CommonShared.UserConnectedBelongsTo(DocSuiteContext.Current.ResolutionEnv.OCExecutiveModifyGroups) AndAlso
                                            CurrentResolution.EffectivenessDate.HasValue AndAlso Not String.IsNullOrEmpty(CurrentResolution.EffectivenessUser)
            End If
            Return _canExecutiveModifyOC.Value
        End Get
    End Property

    Public Function Check(right As ResolutionRightPositions) As Boolean
        Return Check(right, False)
    End Function

    Public Function Check(right As ResolutionRightPositions, onlyContainerRight As Boolean) As Boolean
        If onlyContainerRight Then
            Return Facade.ContainerFacade.CheckContainerRight(CurrentResolution.Container.Id, DSWEnvironment.Resolution, right, Nothing)
        End If
        Return Facade.ResolutionFacade.CheckUserRights(CurrentResolution, right)
    End Function

    Public ReadOnly Property IsFlushAnnexedEnable As Boolean
        Get
            If Not _isFlushAnnexedEnable.HasValue Then
                _isFlushAnnexedEnable = False
                If Facade.ResolutionFacade.HasAnnexed(CurrentResolution) Then
                    _isFlushAnnexedEnable = IsExecutive OrElse IsAdoptable OrElse IsAdministrable
                End If
            End If
            Return _isFlushAnnexedEnable.Value
        End Get
    End Property

    Public ReadOnly Property IsExecutiveViewable As Boolean
        Get
            If Not _isResolutionExecutive.HasValue Then
                _isResolutionExecutive = False
                If DocSuiteContext.Current.ResolutionEnv.ViewAllExecutiveEnabled AndAlso CurrentResolution.EffectivenessDate IsNot Nothing Then
                    _isResolutionExecutive = True
                End If
            End If
            Return _isResolutionExecutive.Value
        End Get
    End Property

    Public ReadOnly Property IsPrivacyViewable As Boolean
        Get
            If Not _isPrivacyViewable.HasValue Then
                _isPrivacyViewable = (CurrentResolution.Container.Privacy.HasValue AndAlso Convert.ToBoolean(CurrentResolution.Container.Privacy.Value)) AndAlso IsDocumentViewable()
            End If
            Return _isPrivacyViewable.Value
        End Get
    End Property

    Public ReadOnly Property HasCurrentStepVisibilityRights As Boolean
        Get
            If Not _hasCurrentStepVisibilityRights.HasValue Then
                _hasCurrentStepVisibilityRights = False
                Dim stepVisibilityRoles As ICollection(Of Integer) = Facade.TabWorkflowFacade.GetOperationStepVisibilityRoles(CurrentResolution)
                Dim stepVisibilityContainers As IList(Of Integer) = Facade.TabWorkflowFacade.GetOperationStepVisibilityContainers(CurrentResolution).ToList()
                If (stepVisibilityRoles.Count = 0 AndAlso stepVisibilityContainers.Count = 0) Then
                    _hasCurrentStepVisibilityRights = True
                    Return _hasCurrentStepVisibilityRights.Value
                End If

                If (stepVisibilityRoles.Count > 0) Then
                    Dim roles As IList(Of Role) = Facade.RoleFacade.GetByIds(stepVisibilityRoles)
                    _hasCurrentStepVisibilityRights = Facade.RoleFacade.CurrentUserBelongsToRoles(DSWEnvironment.Resolution, roles)
                End If

                If (Not _hasCurrentStepVisibilityRights AndAlso stepVisibilityContainers.Count > 0) Then
                    Dim userActiveContainers As Dictionary(Of ResolutionRightPositions, IList(Of Integer)) = Facade.ContainerFacade.GetCurrentUserReolutionContainers(DSWEnvironment.Resolution, Nothing)
                    _hasCurrentStepVisibilityRights = userActiveContainers.Any(Function(x) x.Value.Any(Function(xx) stepVisibilityContainers.Contains(xx)))
                End If
            End If
            Return _hasCurrentStepVisibilityRights.Value
        End Get
    End Property

    Public ReadOnly Property HasCurrentStepFlowResponsabilityRights As Boolean
        Get
            If Not _hasCurrentStepFlowResponsabilityRights.HasValue Then
                _hasCurrentStepFlowResponsabilityRights = False
                Dim stepFlowResponsabilityRoles As ICollection(Of Integer) = Facade.TabWorkflowFacade.GetOperationStepFlowResponsabilityRoles(CurrentResolution)
                If (stepFlowResponsabilityRoles.Count = 0) Then
                    _hasCurrentStepFlowResponsabilityRights = True
                    Return _hasCurrentStepFlowResponsabilityRights.Value
                End If

                If (stepFlowResponsabilityRoles.Count > 0) Then
                    Dim roles As IList(Of Role) = Facade.RoleFacade.GetByIds(stepFlowResponsabilityRoles)
                    _hasCurrentStepFlowResponsabilityRights = Facade.RoleFacade.CurrentUserBelongsToRoles(DSWEnvironment.Resolution, roles)
                End If
            End If
            Return _hasCurrentStepFlowResponsabilityRights.Value
        End Get
    End Property

    Public ReadOnly Property IsResponsibleUser As Boolean
        Get
            If Not _isResponsibleUser.HasValue Then
                Dim stepActiveUsers As ICollection(Of ResolutionWorkflowUser) = Facade.ResolutionWorkflowUserFacade.GetUsersByResolution(CurrentResolution)
                _isResponsibleUser = stepActiveUsers.Any(Function(x) x.Account.Eq(DocSuiteContext.Current.User.FullUserName) _
                    AndAlso x.AuthorizationType.Equals(AuthorizationRoleType.Responsible))
            End If
            Return _isResponsibleUser.Value
        End Get
    End Property

    Public ReadOnly Property IsAccountedUser As Boolean
        Get
            If Not _isAccountedUser.HasValue Then
                Dim stepActiveUsers As ICollection(Of ResolutionWorkflowUser) = Facade.ResolutionWorkflowUserFacade.GetUsersByResolution(CurrentResolution)
                _isAccountedUser = stepActiveUsers.Any(Function(x) x.Account.Eq(DocSuiteContext.Current.User.FullUserName) _
                    AndAlso x.AuthorizationType.Equals(AuthorizationRoleType.Accounted))
            End If
            Return _isAccountedUser.Value
        End Get
    End Property

    Public ReadOnly Property IsConsultedUser As Boolean
        Get
            If Not _isConsultedUser.HasValue Then
                Dim stepActiveUsers As ICollection(Of ResolutionWorkflowUser) = Facade.ResolutionWorkflowUserFacade.GetUsersByResolution(CurrentResolution)
                _isConsultedUser = stepActiveUsers.Any(Function(x) x.Account.Eq(DocSuiteContext.Current.User.FullUserName) _
                    AndAlso x.AuthorizationType.Equals(AuthorizationRoleType.Consulted))
            End If
            Return _isConsultedUser.Value
        End Get
    End Property

    Public ReadOnly Property IsInformedUser As Boolean
        Get
            If Not _isInformedUser.HasValue Then
                Dim stepActiveUsers As ICollection(Of ResolutionWorkflowUser) = Facade.ResolutionWorkflowUserFacade.GetUsersByResolution(CurrentResolution)
                _isInformedUser = stepActiveUsers.Any(Function(x) x.Account.Eq(DocSuiteContext.Current.User.FullUserName) _
                     AndAlso x.AuthorizationType.Equals(AuthorizationRoleType.Informed))
            End If
            Return _isInformedUser.Value
        End Get
    End Property

    Private ReadOnly Property CurrentConservation As Conservation
        Get
            If _currentConservation Is Nothing Then
                Dim conservation As Conservation = WebAPIImpersonatorFacade.ImpersonateFinder(New ConservationFinder(DocSuiteContext.Current.CurrentTenant),
                    Function(impersonationType, finder)
                        finder.UniqueId = CurrentResolution.UniqueId
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
#End Region

#Region "[ Functions ]"

    Public Function IsDocumentViewable() As Boolean
        Return IsDocumentViewable(Facade.TabWorkflowFacade.GetActive(CurrentResolution))
    End Function

    Public Function IsDocumentViewable(workflow As TabWorkflow) As Boolean
        Dim viewDocumentBitRight As ResolutionRightPositions
        If [Enum].TryParse(Of ResolutionRightPositions)(workflow.ViewDocumentBitRight, viewDocumentBitRight) Then
            Return Facade.ResolutionFacade.CheckUserRights(CurrentResolution, viewDocumentBitRight)
        End If
        Return False
    End Function

    Public Function IsAttachmentViewable() As Boolean
        Return IsAttachmentViewable(Facade.TabWorkflowFacade.GetActive(CurrentResolution))
    End Function

    Public Function IsAttachmentViewable(workflow As TabWorkflow) As Boolean
        Dim viewAttachmentBitRight As ResolutionRightPositions
        If [Enum].TryParse(Of ResolutionRightPositions)(workflow.ViewAttachmentBitRight, viewAttachmentBitRight) Then
            Return Facade.ResolutionFacade.CheckUserRights(CurrentResolution, viewAttachmentBitRight)
        End If
        Return False
    End Function

    Private Function GetFullRoleList(ByVal roles As String) As List(Of Integer)
        Dim list As New List(Of Integer)()

        For Each role As String In roles.Trim().Split(","c)
            Dim temp As Integer
            If Int32.TryParse(role, temp) Then
                Dim item As Role = Facade.RoleFacade.GetById(temp)
                RecursiveFullRoleListFiller(item, list)
            End If
        Next
        Return list
    End Function

    Public Function VerifyResolutionProposer() As Boolean
        Dim current As String = DocSuiteContext.Current.User.FullUserName

        ' Verifico se si tratta della stessa persona, in tal caso può ovviamente vedere la proposta fatta
        If current.Eq(CurrentResolution.ProposeUser) Then
            Return True
        End If

        Return False
    End Function
#End Region

#Region "[ Subs ]"
    Private Sub RecursiveFullRoleListFiller(ByVal role As Role, ByVal list As List(Of Integer))

        If Not list.Contains(role.Id) Then
            list.Add(role.Id)
        End If
        ' Children
        For Each child As Role In Facade.RoleFacade.GetItemsByParentId(role.Id)
            RecursiveFullRoleListFiller(child, list)
        Next
    End Sub
#End Region

#Region "[ Shared Functions ]"
    Public Shared Function CheckRight(resl As Resolution, right As ResolutionRightPositions) As Boolean
        Return (New ResolutionFacade).CheckUserRights(resl, right)
    End Function
    Public Shared Function CheckIsViewable(resl As Resolution) As Boolean
        Return CheckRight(resl, ResolutionRightPositions.View)
    End Function
    Public Shared Function CheckIsCancelable(resl As Resolution) As Boolean
        Return CheckRight(resl, ResolutionRightPositions.Cancel)
    End Function
    Public Shared Function CheckIsExecutive(resl As Resolution) As Boolean
        Return CheckRight(resl, ResolutionRightPositions.Executive)
    End Function
    Public Shared Function CheckIsAdoptable(resl As Resolution) As Boolean
        Return CheckRight(resl, ResolutionRightPositions.Adoption)
    End Function
    Public Shared Function CheckIsAdministrable(resl As Resolution) As Boolean
        Return CheckRight(resl, ResolutionRightPositions.Administration)
    End Function
    Public Shared Function HasCancelRight(resl As Resolution) As Boolean
        Return resl.AdoptionDate.HasValue AndAlso (Not resl.EffectivenessDate.HasValue) AndAlso CheckIsCancelable(resl)
    End Function
#End Region

End Class
