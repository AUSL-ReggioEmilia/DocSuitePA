Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data

Public Class DocumentSeriesItemRights

#Region " Fields "

    Private _currentGroups As String()
    Private _currentSecurityGroups As IList(Of SecurityGroups)
    Private _currentFacade As DocumentSeriesItemFacade
    Private _currentSecurityUsersFacade As SecurityUsersFacade
    Private _isPreviewable As Boolean?
    Private _isDraftOwner As Boolean?
    Private _isOwner As Boolean?
    Private _isKnowledge As Boolean?
    Private _isAuthorized As Boolean?
    Private _isReadable As Boolean?
    Private _isDraftable As Boolean?
    Private _isInsertable As Boolean?
    Private _isEditable As Boolean?
    Private _isDeletable As Boolean?
    Private _hasOwnerRole As Boolean?

#End Region

#Region " Constructors "

    Public Sub New(ByVal documentSeriesItem As DocumentSeriesItem)
        DocumentSeriesItemSource = documentSeriesItem
    End Sub

#End Region

#Region " Properties "

    Private Property DocumentSeriesItemSource As DocumentSeriesItem

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

    Private ReadOnly Property CurrentFacade As DocumentSeriesItemFacade
        Get
            If _currentFacade Is Nothing Then
                _currentFacade = New DocumentSeriesItemFacade
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

    Private ReadOnly Property IsMine As Boolean
        Get
            Return DocSuiteContext.Current.ProtocolEnv.DocumentSeriesIsMineRightEnabled _
                AndAlso DocumentSeriesItemSource.RegistrationUser.Eq(DocSuiteContext.Current.User.FullUserName)
        End Get
    End Property

    Private ReadOnly Property IsDraft() As Boolean
        Get
            Return (DocumentSeriesItemSource.Status = DocumentSeriesItemStatus.Draft)
        End Get
    End Property

    Private ReadOnly Property IsCanceled() As Boolean
        Get
            Return (DocumentSeriesItemSource.Status = DocumentSeriesItemStatus.Canceled)
        End Get
    End Property

    Private ReadOnly Property IsActive() As Boolean
        Get
            Return (DocumentSeriesItemSource.Status = DocumentSeriesItemStatus.Active)
        End Get
    End Property

    Private ReadOnly Property IsOwner() As Boolean
        Get
            If _isOwner.HasValue Then
                Return _isOwner.Value
            End If

            Dim roles As IList(Of Role) = CurrentFacade.Factory.DocumentSeriesItemRoleFacade.GetOwnerRoles(DocumentSeriesItemSource)

            _isOwner = CurrentFacade.Factory.RoleFacade.CurrentUserBelongsToRoles(DSWEnvironment.DocumentSeries, roles)

            Return _isOwner.GetValueOrDefault(False)
        End Get
    End Property

    Private ReadOnly Property IsKnowledge() As Boolean
        Get
            If _isKnowledge.HasValue Then
                Return _isKnowledge.Value
            End If

            Dim roles As IList(Of Role) = CurrentFacade.Factory.DocumentSeriesItemRoleFacade.GetKnowledgeRoles(DocumentSeriesItemSource)

            _isKnowledge = CurrentFacade.Factory.RoleFacade.CurrentUserBelongsToRoles(DSWEnvironment.DocumentSeries, roles)

            Return _isKnowledge.GetValueOrDefault(False)

        End Get
    End Property

    Private ReadOnly Property IsAuthorized() As Boolean
        Get
            If _isAuthorized.HasValue Then
                Return _isAuthorized.Value
            End If

            Dim roles As IList(Of Role) = CurrentFacade.Factory.DocumentSeriesItemRoleFacade.GetAuthorizedRoles(DocumentSeriesItemSource)

            _isAuthorized = CurrentFacade.Factory.RoleFacade.CurrentUserBelongsToRoles(DSWEnvironment.DocumentSeries, roles)

            Return _isAuthorized.GetValueOrDefault(False)
        End Get
    End Property

    Private ReadOnly Property IsDraftOwner() As Boolean
        Get
            If _isDraftOwner.HasValue Then
                Return _isDraftOwner.Value
            End If

            Select Case True
                Case Not IsDraft
                    _isDraftOwner = False
                Case IsMine, IsOwner
                    _isDraftOwner = True
            End Select

            Return _isDraftOwner.GetValueOrDefault(False)

        End Get
    End Property

    Public ReadOnly Property IsPublished As Boolean
        Get
            Return Me.DocumentSeriesItemSource.DocumentSeries.PublicationEnabled.GetValueOrDefault() _
                AndAlso Me.DocumentSeriesItemSource.PublishingDate.HasValue _
                AndAlso Me.DocumentSeriesItemSource.PublishingDate.Value <= Date.Today _
                AndAlso Me.DocumentSeriesItemSource.RetireDate.GetValueOrDefault(Date.MaxValue) > Date.Today
        End Get
    End Property

    Public ReadOnly Property IsPublishable As Boolean
        Get
            Return Me.DocumentSeriesItemSource.Status.Equals(DocumentSeriesItemStatus.Active) _
                AndAlso Not Me.DocumentSeriesItemSource.PublishingDate.HasValue _
                AndAlso (Me.IsOwner OrElse Me.CheckRight(DocumentSeriesContainerRightPositions.Admin))
        End Get
    End Property

    Public ReadOnly Property IsRetirable As Boolean
        Get
            Return Me.DocumentSeriesItemSource.Status.Equals(DocumentSeriesItemStatus.Active) _
                AndAlso Me.DocumentSeriesItemSource.PublishingDate.HasValue _
                AndAlso Not Me.DocumentSeriesItemSource.RetireDate.HasValue _
                AndAlso (Me.IsOwner OrElse Me.CheckRight(DocumentSeriesContainerRightPositions.Admin))
        End Get
    End Property

    ''' <summary> Sommario. </summary>
    ''' <remarks> Permette la visione della serie documentale. </remarks>
    Public ReadOnly Property IsPreviewable As Boolean
        Get
            If _isPreviewable.HasValue Then
                Return _isPreviewable.Value
            End If

            Select Case DocumentSeriesItemSource.Status
                Case DocumentSeriesItemStatus.Canceled ' Documenti in stato cancellato
                    _isPreviewable = CheckRight(DocumentSeriesContainerRightPositions.ViewCanceled) AndAlso CheckRight(DocumentSeriesContainerRightPositions.Preview)
                Case DocumentSeriesItemStatus.Draft ' Documenti in stato Draft
                    _isPreviewable = IsMine OrElse IsOwner
                Case DocumentSeriesItemStatus.Active
                    _isPreviewable = IsPublished OrElse IsMine OrElse CheckRight(DocumentSeriesContainerRightPositions.Preview) OrElse IsOwner OrElse IsAuthorized
            End Select

            Return _isPreviewable.GetValueOrDefault(False)
        End Get
    End Property

    ''' <summary> Visualizzazione. </summary>
    ''' <remarks> Permette la visione del documento legato alla serie documentale. </remarks>
    Public ReadOnly Property IsReadable As Boolean
        Get
            If _isReadable.HasValue Then
                Return _isReadable.Value
            End If

            Select Case DocumentSeriesItemSource.Status
                Case DocumentSeriesItemStatus.Canceled ' Documenti in stato cancellato
                    _isReadable = CheckRight(DocumentSeriesContainerRightPositions.ViewCanceled) AndAlso CheckRight(DocumentSeriesContainerRightPositions.View)
                Case DocumentSeriesItemStatus.Draft ' Documenti in stato Draft
                    _isReadable = IsMine OrElse IsOwner
                Case DocumentSeriesItemStatus.Active
                    _isReadable = IsPublished OrElse IsMine OrElse CheckRight(DocumentSeriesContainerRightPositions.View) OrElse IsOwner OrElse IsAuthorized
            End Select

            Return _isReadable.GetValueOrDefault(False)
        End Get
    End Property

    ''' <summary>
    ''' Verifica se l'utente corrente ha diritto di visualizzazione derivato dal Contenitore
    ''' </summary>
    Public ReadOnly Property ViewRightChecked As Boolean
        Get
            Return CheckRight(DocumentSeriesContainerRightPositions.View)
        End Get
    End Property

    ''' <summary> Bozza. </summary>
    Public ReadOnly Property IsInsertableAsDraft As Boolean
        Get
            If _isDraftable.HasValue Then
                Return _isDraftable.Value
            End If

            _isDraftable = CheckRight(DocumentSeriesContainerRightPositions.Draft)

            Return _isDraftable.GetValueOrDefault(False)
        End Get
    End Property

    ''' <summary> Inserimento. </summary>
    Public ReadOnly Property IsInsertable As Boolean
        Get
            If _isInsertable.HasValue Then
                Return _isInsertable.Value
            End If

            _isInsertable = CheckRight(DocumentSeriesContainerRightPositions.Insert)

            Return _isInsertable.GetValueOrDefault(False)
        End Get
    End Property

    ''' <summary> Modifica. </summary>
    Public ReadOnly Property IsEditable As Boolean
        Get
            If _isEditable.HasValue Then
                Return _isEditable.Value
            End If

            Select Case True
                Case IsCanceled
                    _isEditable = False
                Case IsMine
                    _isEditable = True
                Case CheckRight(DocumentSeriesContainerRightPositions.Modify)
                    _isEditable = True
            End Select

            Return _isEditable.GetValueOrDefault(False)
        End Get
    End Property

    ''' <summary>
    ''' Cancellazione Abilitata:
    ''' 
    ''' Se Item in stato Draft e l'utente corrente ha diritti di cancellazione
    ''' Se Item è in stato Attivo e la Data di publicazione è presente e antecedente alla data di oggi e data di ritiro non presente o successiva ad oggi 
    ''' e l'utente corrente ha diritti di cancellazione
    ''' </summary>
    Public ReadOnly Property IsDeletable As Boolean
        Get
            If _isDeletable.HasValue Then
                Return _isDeletable.Value
            End If

            Select Case True
                Case IsCanceled
                    _isDeletable = False
                Case IsMine
                    _isDeletable = True
                Case IsDraft, Not IsPublished
                    _isDeletable = CheckRight(DocumentSeriesContainerRightPositions.Cancel)
            End Select

            Return _isDeletable.GetValueOrDefault(False)
        End Get
    End Property

    Public ReadOnly Property IsAdmin() As Boolean
        Get
            ' TODO: segnaposto per determinare se ha diritti di amministrazione
            Return CheckRight(DocumentSeriesContainerRightPositions.Admin)
        End Get
    End Property

    Public ReadOnly Property HasOwnerRoles As Boolean
        Get
            If _hasOwnerRole.HasValue Then
                Return _hasOwnerRole.Value
            End If

            _hasOwnerRole = IsOwner()

            Return _hasOwnerRole.Value
        End Get
    End Property
#End Region

#Region " Methods "

    Private Function CheckRight(ByVal position As DocumentSeriesContainerRightPositions) As Boolean
        Return CurrentFacade.CheckSecurityGroupsRight(DocumentSeriesItemSource.DocumentSeries, CurrentSecurityGroups, position)
    End Function

    Public Shared Function CheckDocumentSeriesRight(series As DocumentSeries, ByVal position As DocumentSeriesContainerRightPositions) As Boolean
        Dim facade As New DocumentSeriesItemFacade
        Dim securityFacade As New SecurityUsersFacade
        Dim groups As IList(Of SecurityGroups) = securityFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName)
        Return facade.CheckSecurityGroupsRight(series, groups, position)
    End Function

#End Region

End Class


