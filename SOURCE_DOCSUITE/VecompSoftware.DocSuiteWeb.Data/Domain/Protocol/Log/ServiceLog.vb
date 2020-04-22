<Serializable()> _
Public Class ServiceLog
    Inherits DomainObject(Of Guid)

#Region "private data"
    Private _dateTime As DateTime
    Private _server As String
    Private _client As String
    Private _application As String
    Private _session As String
    Private _level As Integer
    Private _text As String

#End Region

#Region "Properties"
    Public Overridable Property DateTime() As DateTime
        Get
            Return _dateTime
        End Get
        Set(ByVal value As DateTime)
            _dateTime = value
        End Set
    End Property
    Public Overridable Property Server() As String
        Get
            Return _server
        End Get
        Set(ByVal value As String)
            _server = value
        End Set
    End Property
    Public Overridable Property Client() As String
        Get
            Return _client
        End Get
        Set(ByVal value As String)
            _client = value
        End Set
    End Property
    Public Overridable Property Application() As String
        Get
            Return _application
        End Get
        Set(ByVal value As String)
            _application = value
        End Set
    End Property
    Public Overridable Property Session() As String
        Get
            Return _session
        End Get
        Set(ByVal value As String)
            _session = value
        End Set
    End Property
    Public Overridable Property Level() As Short
        Get
            Return _level
        End Get
        Set(ByVal value As Short)
            _level = value
        End Set
    End Property
    Public Overridable Property Text() As String
        Get
            Return _text
        End Get
        Set(ByVal value As String)
            _text = value
        End Set
    End Property
#End Region

#Region "Ctor/init"
    Public Sub New()

    End Sub
#End Region
End Class


