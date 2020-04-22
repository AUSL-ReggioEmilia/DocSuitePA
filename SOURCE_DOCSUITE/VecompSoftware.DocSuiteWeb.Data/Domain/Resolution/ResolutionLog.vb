Imports VecompSoftware.Helpers.ExtensionMethods

<Serializable()> _
Public Class ResolutionLog
    Inherits DomainObject(Of Int32)
    Implements ILog

#Region " Fields "

    Private _logDate As DateTime?
    Private _systemComputer As String
    Private _systemUser As String
    Private _program As String
    Private _logType As String
    Private _logDescription As String
    Private _year As Short
    Private _number As Integer
    Private _idResolution As Integer
    Private _resolution As Resolution
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

    Public Overridable Property LogDate() As DateTime Implements ILog.LogDate
        Get
            Return _logDate.Value
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

    Public Overridable Property Year() As Short Implements ILog.Year
        Get
            Return Resolution.Year.GetValueOrDefault()
        End Get
        Set(ByVal value As Short)
            _year = value
        End Set
    End Property

    Public Overridable Property Number() As Integer Implements ILog.Number
        Get
            Return Resolution.Number.GetValueOrDefault()
        End Get
        Set(ByVal value As Integer)
            _number = value
        End Set
    End Property

    Public Overridable Property IdResolution() As Integer
        Get
            Return _idResolution
        End Get
        Set(ByVal value As Integer)
            _idResolution = value
        End Set
    End Property

    Public Overridable Property UniqueIdResolution As Guid

    Public Overridable Property Resolution() As Resolution
        Get
            Return _resolution
        End Get
        Set(ByVal value As Resolution)
            _resolution = value
        End Set
    End Property

    Public Overridable ReadOnly Property LogTypeDescription() As String
        Get
            Dim eventEnum As ResolutionLogType
            If [Enum].TryParse(Of ResolutionLogType)(_logType, True, eventEnum) Then
                Return eventEnum.GetDescription()
            End If

            ' TODO: far sparire questa immondizia, usare ovunque l'enumeratore per favore
            Select Case _logType
                Case "PA" : Return "P - Annullamento"
                Case "PI" : Return "P - Inserimento"
                Case "PM" : Return "P - Modifica"
                Case "PZ" : Return "P - Autorizzazione"
                Case "PS" : Return "P - Visualizzazione Sommario"
                Case "PD" : Return "P - Visualizzazione Documento"
                Case "PE" : Return "P - Elenco Documenti"
                Case "PT" : Return "P - Interoperabilità"
                Case "PX" : Return "P - Errore"
                Case "PO" : Return "P - Invio Protocollo verso terzi"
                Case "FV" : Return "F - Visualizzazione Fascicolo"
                Case "FC" : Return "F - Cancellazione link"
                Case "FI" : Return "F - Inserimento link"
                Case "DS" : Return "D - Visualizzazione Sommario"
                Case "DD" : Return "D - Visualizzazione Documento"
                Case "DI" : Return "D - Inserimento"
                Case "DZ" : Return "D - Autorizzazione"
                Case "RS" : Return "R - Visualizzazione Sommario"
                Case "RD" : Return "R - Visualizzazione Documento"
                Case "RI" : Return "R - Inserimento"
                Case "RZ" : Return "D - Autorizzazione"
                Case "RC" : Return "R - Annullamento Atto"
                Case "RL" : Return "R - Visualizzazione LOG"
                Case "RM" : Return "R - Modifica Atto"
                Case "RP" : Return "R - Composizione Documenti Atto"
                Case "RX" : Return "R - Errore generico"
                Case "RF" : Return "R - Gestione flusso"
                Case "SD" : Return "S - " & DocSuiteContext.Current.ProtocolEnv.DocumentSeriesName
                Case Else : Return String.Empty
            End Select
        End Get
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

#Region " Constructor "

    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub

#End Region

End Class

