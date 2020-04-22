<Serializable()> _
Public Class TabMaster
    Inherits DomainObject(Of TabMasterCompositeKey)

#Region " Fields "

    Private _description As String
    Private _title As String
    Private _idResolutionFileDes As String
    Private _workflowType As String
    Private _viewAllStep As String
    Private _managedData As String
    Private _resolutionType As ResolutionType

#End Region

#Region " Properties "

    Public Overridable Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property
    Public Overridable Property Title() As String
        Get
            Return _title
        End Get
        Set(ByVal value As String)
            _title = value
        End Set
    End Property
    Public Overridable Property IdResolutionFileDes() As String
        Get
            Return _idResolutionFileDes
        End Get
        Set(ByVal value As String)
            _idResolutionFileDes = value
        End Set
    End Property
    Public Overridable Property WorkflowType() As String
        Get
            Return _workflowType
        End Get
        Set(ByVal value As String)
            _workflowType = value
        End Set
    End Property
    Public Overridable Property ViewAllStep() As String
        Get
            Return _viewAllStep
        End Get
        Set(ByVal value As String)
            _viewAllStep = value
        End Set
    End Property
    Public Overridable Property ManagedData() As String
        Get
            Return _managedData
        End Get
        Set(ByVal value As String)
            _managedData = value
        End Set
    End Property
    Public Overridable Property ReslType() As ResolutionType
        Get
            Return _resolutionType
        End Get
        Set(ByVal value As ResolutionType)
            _resolutionType = value
        End Set
    End Property

#End Region

#Region " Constructor "

    Public Sub New()
    End Sub

#End Region

End Class

