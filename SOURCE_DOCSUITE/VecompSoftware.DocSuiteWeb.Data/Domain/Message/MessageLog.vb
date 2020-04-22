Imports System.ComponentModel

<Serializable()>
Public Class MessageLog
    Inherits DomainObject(Of Int32)
    Implements ILog

    Public Enum MessageLogType
        <Description("Creazione")> _
        Created = 1
        <Description("Lettura")> _
        Viewed = 2
        <Description("Modifica")> _
        Edited = 3
        <Description("Cancellazione")> _
        Deleted = 4
        <Description("Inviata")> _
        Sent = 5
        <Description("Errore")> _
        [Error] = 6
    End Enum

#Region "Fields"
    Private _message As DSWMessage
    Private _description As String
    Private _type As MessageLogType
    Private _logDate As DateTime
    Private _systemComputer As String
    Private _systemUser As String
    Private _program As String
    Private _logType As String
    Private _logDescription As String
    Private _year As Short
    Private _number As Integer
    Private _severity As Short?
#End Region

#Region " Properties "

    Public Overridable Property Message As DSWMessage
        Get
            Return _message
        End Get
        Set(value As DSWMessage)
            _message = value
        End Set
    End Property

    Public Overridable Property Description As String
        Get
            Return _description
        End Get
        Set(value As String)
            _description = value
        End Set
    End Property

    Public Overridable Property Type As MessageLogType
        Get
            Return _type
        End Get
        Set(value As MessageLogType)
            _type = value
        End Set
    End Property

    Public Overrides Property Id As Integer Implements ILog.Id
        Get
            Return MyBase.Id
        End Get
        Set(value As Integer)
            MyBase.Id = value
        End Set
    End Property

    Public Overridable Property LogDate As DateTime Implements ILog.LogDate
        Get
            Return _logDate
        End Get
        Set(value As DateTime)
            _logDate = value
        End Set
    End Property

    Public Overridable Property SystemComputer As String Implements ILog.SystemComputer
        Get
            Return _systemComputer
        End Get
        Set(value As String)
            _systemComputer = value
        End Set
    End Property

    Public Overridable Property SystemUser As String Implements ILog.SystemUser
        Get
            Return _systemUser
        End Get
        Set(value As String)
            _systemUser = value
        End Set
    End Property

    Public Overridable Property Program As String Implements ILog.Program
        Get
            Return _program
        End Get
        Set(value As String)
            _program = value
        End Set
    End Property

    Public Overridable Property LogType As String Implements ILog.LogType
        Get
            Return _logType
        End Get
        Set(value As String)
            _logType = value
        End Set
    End Property

    Public Overridable Property LogDescription As String Implements ILog.LogDescription
        Get
            Return _logDescription
        End Get
        Set(value As String)
            _logDescription = value
        End Set
    End Property

    Public Overridable Property Year As Short Implements ILog.Year
        Get
            Return _year
        End Get
        Set(value As Short)
            _year = value
        End Set
    End Property

    Public Overridable Property Number As Integer Implements ILog.Number
        Get
            Return _number
        End Get
        Set(value As Integer)
            _number = value
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
