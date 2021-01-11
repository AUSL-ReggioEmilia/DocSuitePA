Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

<Serializable()>
Public Class ProtocolLog
    Inherits DomainObject(Of Guid)
    Implements IProtocolLog

#Region " Fields "
    Private _protocol As Protocol
#End Region

#Region " Properties "

    Public Overrides Property Id As Guid Implements IProtocolLog.Id
        Get
            Return MyBase.Id
        End Get
        Set(ByVal value As Guid)
            MyBase.Id = value
        End Set
    End Property

    Public Overridable Property Incremental As Integer Implements ILog.Id

    Public Overridable Property LogDate As DateTime Implements ILog.LogDate

    Public Overridable Property SystemComputer As String Implements ILog.SystemComputer

    Public Overridable Property SystemUser As String Implements ILog.SystemUser

    ''' <summary> Tipo di programma che scrive il log. </summary>
    ''' <remarks> DocSuite8, JeepService, etc </remarks>
    Public Overridable Property Program As String Implements ILog.Program

    Public Overridable Property Year As Short Implements ILog.Year

    Public Overridable Property Number As Integer Implements ILog.Number

    Public Overridable Property LogType As String Implements ILog.LogType

    Public Overridable ReadOnly Property LogTypeDescription() As String
        Get
            Dim eventEnum As ProtocolLogEvent
            If [Enum].TryParse(Of ProtocolLogEvent)(_logType, True, eventEnum) Then
                Return eventEnum.GetDescription()
            End If

            ' TODO: far sparire questa immondizia, usare ovunque l'enumeratore per favore
            Select Case _logType
                Case "PA" : Return "P - Annullamento"
                Case "PI" : Return "P - Inserimento"
                Case "PM" : Return "P - Modifica"
                Case "PZ" : Return "P - Autorizzazione"
                Case "PS" : Return "P - Visualizzazione Sommario"
                Case "P1" : Return "P - Visualizzazione Sommario"
                Case "PD" : Return "P - Visualizzazione Documento"
                Case "PE" : Return "P - Elenco Documenti"
                Case "PT" : Return "P - Interoperabilità"
                Case "PO" : Return "P - Invio Protocollo verso terzi"
                Case "PR" : Return "P - Rigetto di protocollo"
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

    Public Overridable Property LogDescription As String Implements ILog.LogDescription

    Public Overridable Property Protocol As Protocol
        Get
            Return _protocol
        End Get
        Set(ByVal value As Protocol)
            _protocol = value
            Year = value.Year
            Number = value.Number
        End Set
    End Property

    Public Overridable Property Severity As Short? Implements ILog.Severity

    Public Overridable Property Hash As String

#End Region

#Region " Constructor "

    Public Sub New()

    End Sub

#End Region

End Class



