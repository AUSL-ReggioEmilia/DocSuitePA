<Serializable()> _
Public Class ResolutionWorkflow
    Inherits DomainObject(Of ResolutionWorkflowCompositeKey)
    Implements IAuditable, ISupportShortLogicDelete

#Region "Private Fields"
    Private _parent As ResolutionWorkflow
    Private _isActive As Short
    Private _step As Nullable(Of Short)
    Private _documentName As String
    Private _document As Integer?
    Private _attachment As Integer?
    Private _privacyAttachment As Integer?
    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset
    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?
    Private _resolution As Resolution
    Private _incrementalFather As Short?
#End Region

#Region "Properties"
    Public Overridable Property Parent() As ResolutionWorkflow
        Get
            Return _parent
        End Get
        Set(ByVal value As ResolutionWorkflow)
            _parent = value
        End Set
    End Property
    Public Overridable Property IsActive() As Short Implements ISupportShortLogicDelete.IsActive
        Get
            Return _isActive
        End Get
        Set(ByVal value As Short)
            _isActive = value
        End Set
    End Property
    Public Overridable Property ResStep() As Nullable(Of Short)
        Get
            Return _step
        End Get
        Set(ByVal value As Nullable(Of Short))
            _step = value
        End Set
    End Property
    Public Overridable Property DocumentName() As String
        Get
            Return _documentName
        End Get
        Set(ByVal value As String)
            _documentName = value
        End Set
    End Property
    Public Overridable Property Document() As Integer?
        Get
            Return _document
        End Get
        Set(ByVal value As Integer?)
            _document = value
        End Set
    End Property

    Public Overridable Property Attachment() As Integer?
        Get
            Return _attachment
        End Get
        Set(ByVal value As Integer?)
            _attachment = value
        End Set
    End Property

    Public Overridable Property PrivacyAttachment() As Integer?
        Get
            Return _privacyAttachment
        End Get
        Set(ByVal value As Integer?)
            _privacyAttachment = value
        End Set
    End Property

    Public Overridable Property RegistrationUser() As String Implements IAuditable.RegistrationUser
        Get
            Return _registrationUser
        End Get
        Set(ByVal value As String)
            _registrationUser = value
        End Set
    End Property
    Public Overridable Property RegistrationDate() As DateTimeOffset Implements IAuditable.RegistrationDate
        Get
            Return _registrationDate
        End Get
        Set(ByVal value As DateTimeOffset)
            _registrationDate = value
        End Set
    End Property
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
        Get
            Return _lastChangedUser
        End Get
        Set(ByVal value As String)
            _lastChangedUser = value
        End Set
    End Property
    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
        Get
            Return _lastChangedDate
        End Get
        Set(ByVal value As DateTimeOffset?)
            _lastChangedDate = value
        End Set
    End Property
    Public Overridable Property Resolution() As Resolution
        Get
            Return _resolution
        End Get
        Set(ByVal value As Resolution)
            _resolution = value
        End Set
    End Property

    Public Overridable Property IncrementalFather() As Short?
        Get
            Return _incrementalFather
        End Get
        Set(ByVal value As Short?)
            _incrementalFather = value
        End Set
    End Property

    ' Guid catena allegati annexed
    Public Overridable Property Annexed As Guid

    ' Guid catena allegati documenti principali omissis
    Public Overridable Property DocumentsOmissis As Guid

    ' Guid catena allegati omissis
    Public Overridable Property AttachmentsOmissis As Guid

    Public Overridable Property ResolutionWorkflowUsers As ICollection(Of ResolutionWorkflowUser)
#End Region

#Region "Ctor/init"
    Public Sub New()
        Id = New ResolutionWorkflowCompositeKey
        ResolutionWorkflowUsers = New List(Of ResolutionWorkflowUser)()
    End Sub
#End Region

End Class

