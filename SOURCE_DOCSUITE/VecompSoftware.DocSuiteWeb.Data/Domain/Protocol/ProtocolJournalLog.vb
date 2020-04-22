<Serializable()> _
Public Class ProtocolJournalLog
    Inherits DomainObject(Of Integer)

#Region "Private data"
    Private _logDate As Nullable(Of Date)
    Private _ProtocolJournalDate As Nullable(Of Date)
    Private _systemComputer As String
    Private _systemUser As String
    Private _startDate As Nullable(Of Date)
    Private _endDate As Nullable(Of Date)
    Private _protocolTotal As Integer?
    Private _protocolRegister As Integer?
    Private _protocolError As Integer?
    Private _protocolCancelled As Integer?
    Private _protocolActive As Integer?
    Private _protocolOthers As Integer?
    Private _idDocument As Integer?
    Private _location As Location
    Private _logDescription As String
#End Region

#Region "Public properties"
    Public Overridable Property LogDate() As Nullable(Of Date)
        Get
            Return _logDate
        End Get
        Set(ByVal value As Nullable(Of Date))
            _logDate = value
        End Set
    End Property

    Public Overridable Property ProtocolJournalDate() As Nullable(Of Date)
        Get
            Return _ProtocolJournalDate
        End Get
        Set(ByVal value As Nullable(Of Date))
            _ProtocolJournalDate = value
        End Set
    End Property

    Public Overridable Property SystemComputer() As String
        Get
            Return _systemComputer
        End Get
        Set(ByVal value As String)
            _systemComputer = value
        End Set
    End Property

    Public Overridable Property SystemUser() As String
        Get
            Return _systemUser
        End Get
        Set(ByVal value As String)
            _systemUser = value
        End Set
    End Property

    Public Overridable Property StartDate() As Nullable(Of Date)
        Get
            Return _startDate
        End Get
        Set(ByVal value As Nullable(Of Date))
            _startDate = value
        End Set
    End Property

    Public Overridable Property EndDate() As Nullable(Of Date)
        Get
            Return _endDate
        End Get
        Set(ByVal value As Nullable(Of Date))
            _endDate = value
        End Set
    End Property

    Public Overridable Property ProtocolTotal() As Integer?
        Get
            Return _protocolTotal
        End Get
        Set(ByVal value As Integer?)
            _protocolTotal = value
        End Set
    End Property

    Public Overridable Property ProtocolRegister() As Integer?
        Get
            Return _protocolRegister
        End Get
        Set(ByVal value As Integer?)
            _protocolRegister = value
        End Set
    End Property

    Public Overridable Property ProtocolError() As Integer?
        Get
            Return _protocolError
        End Get
        Set(ByVal value As Integer?)
            _protocolError = value
        End Set
    End Property

    Public Overridable Property ProtocolCancelled() As Integer?
        Get
            Return _protocolCancelled
        End Get
        Set(ByVal value As Integer?)
            _protocolCancelled = value
        End Set
    End Property

    Public Overridable Property ProtocolActive() As Integer?
        Get
            Return _protocolActive
        End Get
        Set(ByVal value As Integer?)
            _protocolActive = value
        End Set
    End Property

    Public Overridable Property ProtocolOthers() As Integer?
        Get
            Return _protocolOthers
        End Get
        Set(ByVal value As Integer?)
            _protocolOthers = value
        End Set
    End Property

    Public Overridable Property IdDocument() As Integer?
        Get
            Return _idDocument
        End Get
        Set(ByVal value As Integer?)
            _idDocument = value
        End Set
    End Property

    Public Overridable Property Location() As Location
        Get
            Return _location
        End Get
        Set(ByVal value As Location)
            _location = value
        End Set
    End Property

    Public Overridable Property LogDescription() As String
        Get
            Return _logDescription
        End Get
        Set(ByVal value As String)
            _logDescription = value
        End Set
    End Property
#End Region

#Region "Init"
    Public Sub New()
    End Sub
#End Region


End Class
