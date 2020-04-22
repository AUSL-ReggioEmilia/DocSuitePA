''' <summary>
''' Log delle pratiche
''' </summary>
<Serializable()>
Public Class DocumentLog
    Inherits DomainObject(Of Int32)
    Implements ILog

#Region " Fields "
    Private _logDate As Date?
    Private _systemComputer As String
    Private _systemUser As String
    Private _program As String
    Private _year As Short
    Private _number As Integer
    Private _incremental As Short
    Private _logType As String
    Private _logDescription As String
    Private _document As Document
    Private _descr As IDictionary(Of String, String)
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

    Public Overridable Property Program() As String Implements ILog.Program
        Get
            Return _program
        End Get
        Set(ByVal value As String)
            _program = value
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

    Public Overridable Property Number() As Integer Implements ILog.Number
        Get
            Return _number
        End Get
        Set(ByVal value As Integer)
            _number = value
        End Set
    End Property

    Public Overridable Property Incremental() As Short
        Get
            Return _incremental
        End Get
        Set(ByVal value As Short)
            _incremental = value
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

    Public Overridable ReadOnly Property LogTypeDescription() As String
        Get
            If TypeDescription.ContainsKey(_logType) Then
                Return TypeDescription(_logType)
            Else
                Return String.Empty
            End If
        End Get
    End Property

    Public Overridable ReadOnly Property TypeDescription() As IDictionary(Of String, String)
        Get
            If _descr Is Nothing Then
                _descr = New Dictionary(Of String, String)
                _descr.Add("PI", "P - Inserimento")
                _descr.Add("PM", "P - Modifica")
                _descr.Add("PZ", "P - Autorizzazione")
                _descr.Add("PS", "P - Visualizzazione Sommario")
                _descr.Add("PD", "P - Visualizzazione Documento")
                _descr.Add("PE", "P - Elenco Documenti")
                _descr.Add("PT", "P - Interoperabilità")
                _descr.Add("PO", "P - Invio Protocollo verso terzi")
                _descr.Add("FV", "F - Visualizzazione Fascicolo")
                _descr.Add("FC", "F - Cancellazione link")
                _descr.Add("FI", "F - Inserimento link")
                _descr.Add("DS", "D - Visualizzazione Sommario")
                _descr.Add("DD", "D - Visualizzazione Documento")
                _descr.Add("DI", "D - Inserimento")
                _descr.Add("DZ", "D - Autorizzazione")
                _descr.Add("PA", "D - Annullamento")
                _descr.Add("RS", "R - Visualizzazione Sommario")
                _descr.Add("RP", "R - Stampe")
                _descr.Add("RD", "R - Visualizzazione Documento")
                _descr.Add("RDA", "R - Aggiunta Documento")
                _descr.Add("RI", "R - Inserimento")
                _descr.Add("RZ", "R - Autorizzazione")
                _descr.Add("RC", "R - Annullamento Atto")
                _descr.Add("RL", "R - Visualizzazione LOG")
                _descr.Add("RM", "R - Modifica Atto")
                _descr.Add("RX", "R - Errore generico")
                _descr.Add("RF", "R - Gestione flusso")
                _descr.Add("SD", "S - " & DocSuiteContext.Current.ProtocolEnv.DocumentSeriesName)
            End If
            Return _descr
        End Get
    End Property

    Public Overridable Property LogDescription() As String Implements ILog.LogDescription
        Get
            Return _logDescription
        End Get
        Set(ByVal value As String)
            _logDescription = value
        End Set
    End Property

    Public Overridable Property Document() As Document
        Get
            Return _document
        End Get
        Set(ByVal value As Document)
            _document = value
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

#Region " Constructors "
    Public Sub New()

    End Sub
#End Region

End Class

