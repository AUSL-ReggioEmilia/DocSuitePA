<Serializable()> _
Public Class CollaborationLog
    Inherits DomainObject(Of Integer)
    Implements ILog

#Region " Fields "

    Private _collaboration As Collaboration
    Private _idCollaboration As Integer
    Private _collaborationIncremental As Integer?
    Private _incremental As Short?
    Private _idChain As Integer?
    Private _logDate As DateTime?
    Private _systemComputer As String
    Private _systemUser As String
    Private _sessionId As String
    Private _program As String
    Private _logType As String
    Private _logDescription As String
    Private _year As Short
    Private _number As Integer
    Private _severity As Short?

#End Region

#Region " Properties "

    Public Overrides Property Id() As Integer Implements ILog.Id
        Get
            Return MyBase.Id
        End Get
        Set(ByVal value As Integer)
            MyBase.Id = value
        End Set
    End Property

    Public Overridable Property IdCollaboration() As Integer
        Get
            Return _idCollaboration
        End Get
        Set(value As Integer)
            _idCollaboration = value
        End Set
    End Property

    Public Overridable Property CollaborationIncremental() As Integer?
        Get
            Return _collaborationIncremental
        End Get
        Set(ByVal value As Integer?)
            _collaborationIncremental = value
        End Set
    End Property

    Public Overridable Property Incremental() As Short?
        Get
            Return _incremental
        End Get
        Set(ByVal value As Short?)
            _incremental = value
        End Set
    End Property

    Public Overridable Property IdChain() As Integer?
        Get
            Return _idChain
        End Get
        Set(ByVal value As Integer?)
            _idChain = value
        End Set
    End Property

    Public Overridable Property LogDate As DateTime Implements ILog.LogDate
        Get
            Return _logDate
        End Get
        Set(ByVal value As DateTime)
            _logDate = value
        End Set
    End Property

    Public Overridable Property SystemComputer() As String Implements ILog.SystemComputer
        Get
            Return _systemComputer
        End Get
        Set(ByVal value As String)
            _systemComputer = value
        End Set
    End Property

    Public Overridable Property SystemUser() As String Implements ILog.SystemUser
        Get
            Return _systemUser
        End Get
        Set(ByVal value As String)
            _systemUser = value
        End Set
    End Property

    Public Overridable Property SessionId() As String
        Get
            Return _sessionId
        End Get
        Set(ByVal value As String)
            _sessionId = value
        End Set
    End Property

    Public Overridable Property Program() As String Implements ILog.Program
        Get
            Return _program
        End Get
        Set(ByVal value As String)
            _program = value
        End Set
    End Property

    Public Overridable Property LogType() As String Implements ILog.LogType
        Get
            Return _logType
        End Get
        Set(ByVal value As String)
            _logType = value
        End Set
    End Property

    Public Overridable Property LogDescription() As String Implements ILog.LogDescription
        Get
            Return _logDescription
        End Get
        Set(ByVal value As String)
            _logDescription = value
        End Set
    End Property

    Public Overridable Property Collaboration() As Collaboration
        Get
            Return _collaboration
        End Get
        Set(ByVal value As Collaboration)
            _collaboration = value
        End Set
    End Property

    Public Overridable Property Number() As Integer Implements ILog.Number
        Get
            Return _number
        End Get
        Set(ByVal value As Integer)
            _number = value
        End Set
    End Property

    Public Overridable Property Year() As Short Implements ILog.Year
        Get
            Return _year
        End Get
        Set(ByVal value As Short)
            _year = value
        End Set
    End Property

    Public Overridable Property Severity As Short? Implements ILog.Severity
        Get
            Return _severity
        End Get
        Set(value As Short?)
            _severity = value
        End Set
    End Property

#End Region

End Class
