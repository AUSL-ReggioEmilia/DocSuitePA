<Serializable()> _
Public Class ComputerLog
    Inherits DomainObject(Of String)

#Region "private data"
    Private _systemServer As String
    Private _systemUser As String
    Private _lastOperationDate As Date?
    Private _accessNumber As Integer
    Private _prevOperationDate As Date?
    Private _sessionId As String
    Private _advancedScanner As Integer
    Private _advancedViewer As Integer
#End Region

#Region "Properties"
    Public Overridable Property SystemServer() As String
        Get
            Return _systemServer
        End Get
        Set(ByVal value As String)
            _systemServer = value
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
    Public Overridable Property LastOperationDate() As Date?
        Get
            Return _lastOperationDate

        End Get
        Set(ByVal value As Date?)
            _lastOperationDate = value
        End Set
    End Property
    Public Overridable Property AccessNumber() As Integer
        Get
            Return _accessNumber
        End Get
        Set(ByVal value As Integer)
            _accessNumber = value
        End Set
    End Property
    Public Overridable Property PrevOperationDate() As Date?
        Get
            Return _prevOperationDate

        End Get
        Set(ByVal value As Date?)
            _prevOperationDate = value
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
    Public Overridable Property AdvancedScanner() As Integer
        Get
            Return _advancedScanner
        End Get
        Set(ByVal value As Integer)
            _advancedScanner = value
        End Set
    End Property
    Public Overridable Property AdvancedViewer() As Integer
        Get
            Return _advancedViewer
        End Get
        Set(ByVal value As Integer)
            _advancedViewer = value
        End Set
    End Property

    Private _zebraPrinter As ZebraPrinter
    Public Overridable Property ZebraPrinter As ZebraPrinter
        Get
            Return _zebraPrinter
        End Get
        Set(ByVal value As ZebraPrinter)
            _zebraPrinter = value
        End Set
    End Property

    Private _scannerConfiguration As ScannerConfiguration
    Public Overridable Property ScannerConfiguration As ScannerConfiguration
        Get
            Return _scannerConfiguration
        End Get
        Set(ByVal value As ScannerConfiguration)
            _scannerConfiguration = value
        End Set
    End Property

#End Region

#Region "Ctor/init"
    Public Sub New()

    End Sub
#End Region
End Class


