Imports System
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel
Imports VecompSoftware.Helpers.Signer.Security

<DataObject()>
Public Class ProtocolLogFacade
    Inherits BaseProtocolFacade(Of ProtocolLog, Guid, NHibernateProtocolLogDao)

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Fields "
    Private _hashHelper As HashGenerator
#End Region

#Region " Properties "
    Private ReadOnly Property HashHelper As Helpers.Signer.Security.HashGenerator
        Get
            If _hashHelper Is Nothing Then
                _hashHelper = New Helpers.Signer.Security.HashGenerator()
            End If
            Return _hashHelper
        End Get
    End Property
#End Region

#Region " Methods "

    ''' <summary>
    ''' Inizializza un nuovo oggetto ProtocolLog
    ''' </summary>
    ''' <param name="year">Anno del protocollo</param>
    ''' <param name="number">Numero del protocollo</param>
    ''' <returns>Ritorna un nuovo oggetto ProtocolLog con proprietà base popolate</returns>
    Public Function InitializeNewProtocolLog(year As Short, number As Integer) As ProtocolLog
        Return InitializeNewProtocolLog(Factory.ProtocolFacade.GetById(year, number))
    End Function

    Public Function InitializeNewProtocolLog(idProtocol As Guid) As ProtocolLog
        Return InitializeNewProtocolLog(Factory.ProtocolFacade.GetById(idProtocol))
    End Function

    Public Function InitializeNewProtocolLog(protocol As Protocol) As ProtocolLog
        Dim pLog As ProtocolLog = New ProtocolLog() With {
                    .LogDate = _dao.GetServerDate(),
                    .SystemComputer = DocSuiteContext.Current.UserComputer,
                    .SystemUser = DocSuiteContext.Current.User.FullUserName,
                    .Program = DocSuiteContext.Program,
                    .Protocol = protocol
                }

        Return pLog
    End Function

    Public Overrides Sub Save(ByRef obj As ProtocolLog)
        Dim str As String = String.Concat(obj.SystemUser, "|", obj.Year, "|", obj.Number, "|", obj.LogType, "|", obj.LogDescription, "|", obj.UniqueId, "|", obj.Protocol.Id, "|", obj.LogDate.ToString("yyyyMMddHHmmss"))
        obj.Hash = HashHelper.GenerateHash(str)
        MyBase.Save(obj)
    End Sub

    Public Overrides Sub SaveWithoutTransaction(ByRef obj As ProtocolLog)
        Dim str As String = String.Concat(obj.SystemUser, "|", obj.Year, "|", obj.Number, "|", obj.LogType, "|", obj.LogDescription, "|", obj.UniqueId, "|", obj.Protocol.Id, "|", obj.LogDate.ToString("yyyyMMddHHmmss"))
        obj.Hash = HashHelper.GenerateHash(str)
        MyBase.SaveWithoutTransaction(obj)
    End Sub

    Public Overloads Sub Log(prot As Protocol, logEvent As ProtocolLogEvent, message As String)
        Try
            Insert(prot, logEvent, message)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore nell'inserimento del log dei protocolli", ex)
        End Try
    End Sub

    Public Sub Insert(ByRef protocol As Protocol, ByVal logType As ProtocolLogEvent, ByVal message As String)
        Insert(protocol, logType.ToString(), message, DocSuiteContext.Current.User.FullUserName)
    End Sub

    Public Sub Handle(ByRef protocol As Protocol, subject As String, status As Boolean)
        If status Then
            Log(protocol, ProtocolLogEvent.PM, String.Concat("Protocollo preso in carico da: ", subject))
        Else
            Log(protocol, ProtocolLogEvent.PM, String.Concat("Protocollo rilasciato da: ", subject))
        End If
    End Sub

    Public Sub Insert(ByVal Year As Short, ByVal Number As Integer, ByVal LogType As String, ByVal LogDescription As String)
        Insert(Year, Number, LogType, LogDescription, DocSuiteContext.Current.User.FullUserName)
    End Sub

    Public Sub InsertLogWithoutRead(year As Short, number As Integer, logType As ProtocolLogEvent, description As String)
        Insert(year, number, logType.ToString(), description, DocSuiteContext.Current.User.FullUserName, False)
    End Sub
    Private Sub InternalInsert(inizialize As Func(Of ProtocolLog), year As Short, number As Integer, ByVal logType As String, ByVal logDescription As String,
                      userName As String, Optional setReadLog As Boolean = True)
        If (Not logType.Eq(ProtocolLogEvent.P1.ToString()) AndAlso Not HasP1Log(year, number, userName) AndAlso setReadLog) Then
            Dim readLog As ProtocolLog = inizialize()
            readLog.LogType = ProtocolLogEvent.P1.ToString()
            readLog.LogDescription = "Prima visualizzazione protocollo."

            Try
                Me.Save(readLog)
            Catch ex As Exception
                Dim errorMessage As String = String.Format("Errore nell'inserimento del log dei protocolli: LogType: {0} LogDescription: {1}", readLog.LogType, readLog.LogDescription)
                FileLogger.Warn(LoggerName, errorMessage, ex)
            End Try
        End If

        Dim currentLog As ProtocolLog = inizialize()
        currentLog.LogType = logType
        currentLog.LogDescription = logDescription

        Try
            Me.Save(currentLog)
        Catch ex As Exception
            Dim errorMessage As String = String.Format("Errore nell'inserimento del log dei protocolli: LogType: {0} LogDescription: {1}", currentLog.LogType, currentLog.LogDescription)
            FileLogger.Error(LoggerName, errorMessage, ex)
        End Try
    End Sub
    Public Sub Insert(protocol As Protocol, ByVal logType As String, ByVal logDescription As String,
                      userName As String, Optional setReadLog As Boolean = True)

        InternalInsert(Function() InitializeNewProtocolLog(protocol), protocol.Year, protocol.Number, logType, logDescription, userName, setReadLog)
    End Sub
    Public Sub Insert(ByVal year As Short, ByVal number As Integer, ByVal logType As String, ByVal logDescription As String,
                      userName As String, Optional setReadLog As Boolean = True)
        InternalInsert(Function() InitializeNewProtocolLog(year, number), year, number, logType, logDescription, userName, setReadLog)

    End Sub


    Private Sub InsertRoles(ByRef protocol As Protocol, ByVal roles As ICollection(Of Integer), ByVal type As String, ByVal description As String, Optional setReadLog As Boolean = True)
        If roles Is Nothing Then
            Exit Sub
        End If

        For Each idRole As Integer In roles
            Dim role As Role = Factory.RoleFacade.GetById(idRole)
            If role Is Nothing Then
                Continue For
            End If

            Dim roleName As String = String.Concat(idRole, " ", role.Name)
            If setReadLog Then
                Insert(protocol, ProtocolLogEvent.PZ, String.Format("{0} ({1}): {2}", description, type, roleName))
            Else
                InsertLogWithoutRead(protocol.Year, protocol.Number, ProtocolLogEvent.PZ, String.Format("{0} ({1}): {2}", description, type, roleName))
            End If
        Next
    End Sub

#Region " Log Authorization 2.0 "

    Public Sub InsertRolesLog(ByRef protocol As Protocol, ByVal roles As ICollection(Of Integer), ByVal type As String)
        InsertRoles(protocol, roles, type, "Autorizzazione")
    End Sub

    Public Sub InsertRolesLogWithoutRead(ByRef protocol As Protocol, ByVal roles As ICollection(Of Integer), ByVal type As String)
        InsertRoles(protocol, roles, type, "Autorizzazione", setReadLog:=False)
    End Sub

    Public Sub AddCCRoleAuthorization(protocol As Protocol, role As Role)
        ProtocolAuthorization(protocol, String.Format("Autorizzazione CC (Add): {0} {1}", role.Id.ToString(), role.Name))
    End Sub

    Public Sub DelCCRoleAuthorization(protocol As Protocol, role As Role)
        ProtocolAuthorization(protocol, String.Format("Autorizzazione CC (Del): {0} {1}", role.Id.ToString(), role.Name))
    End Sub

    Public Sub AddRoleUserAuthorization(protocol As Protocol, user As ProtocolRoleUser)
        ProtocolAuthorization(protocol, String.Format("Autorizzazione utente (Add): {0}", user.Id.ToString()))
    End Sub

    Public Sub DelRoleUserAuthorization(protocol As Protocol, user As ProtocolRoleUser)
        ProtocolAuthorization(protocol, String.Format("Autorizzazione utente (Del): {0}", user.Id.ToString()))
    End Sub

    Public Sub AddUserAuthorization(protocol As Protocol, account As String)
        ProtocolAuthorization(protocol, String.Concat("Autorizzazione utente (Add): ", account))
    End Sub

    Public Sub DelUserAuthorization(protocol As Protocol, account As String)
        ProtocolAuthorization(protocol, String.Concat("Autorizzazione utente (Del): ", account))
    End Sub

    Public Sub AddRoleAuthorization(protocol As Protocol, role As Role)
        ProtocolAuthorization(protocol, String.Format("Autorizzazione (Add): {0} {1}", role.Id.ToString(), role.Name))
    End Sub

    Public Sub DelRoleAuthorization(protocol As Protocol, role As Role)
        ProtocolAuthorization(protocol, String.Format("Autorizzazione (Del): {0} {1}", role.Id.ToString(), role.Name))
    End Sub

    Private Sub ProtocolAuthorization(protocol As Protocol, message As String)
        Insert(protocol, ProtocolLogEvent.PZ, message)
    End Sub

#End Region

    Public Sub InsertFullRolesLog(ByRef protocol As Protocol, ByVal roles As ICollection(Of Integer), ByVal type As String)
        InsertRoles(protocol, roles, type, "Autorizzazione Completa")
    End Sub

    Public Sub InsertFullRolesLogWithoutRead(ByRef protocol As Protocol, ByVal roles As ICollection(Of Integer), ByVal type As String)
        InsertRoles(protocol, roles, type, "Autorizzazione Completa", setReadLog:=False)
    End Sub

    Public Function SearchLog(ByVal year As Short, ByVal number As Integer, ByVal user As String, ByVal logType As ProtocolLogEvent) As IList(Of ProtocolLog)
        Return _dao.SearchLog(year, number, user, logType.ToString())
    End Function

    Public Function SearchLogByProtocolUniqueId(ByVal uniqueId As Guid, ByVal user As String, ByVal logType As ProtocolLogEvent, Optional logDescription As String = "") As IList(Of ProtocolLog)
        Return _dao.SearchLogByProtocolUniqueId(uniqueId, user, logType.ToString(), logDescription)
    End Function

    Private Function HasP1Log(ByVal year As Short, ByVal number As Integer, ByVal user As String) As Boolean
        Dim items As IList(Of ProtocolLog) = SearchLog(year, number, user, ProtocolLogEvent.P1)
        Return Not items.IsNullOrEmpty()
    End Function

    ''' <summary> Restituisce il numero progressivo. </summary>
    Public Function GetMaxId() As Integer
        Return _dao.GetMaxId()
    End Function

    ''' <summary>
    ''' Ritorna una datatable con le statistiche estrapolate dal log
    ''' </summary>
    ''' <param name="User">utente che ha eseguito le operazioni</param>
    ''' <param name="DateStart">data inizio</param>
    ''' <param name="DateEnd">data fine</param>
    ''' <param name="maxOpNumber">minimo numero delle operazioni da cercare</param>
    ''' <returns>datatable</returns>
    ''' <remarks> le date devono essere stringhe in formato yyyyMMdd</remarks>
    Public Function GetProtocolLogStatisticsTable(Optional ByVal User As String = "", Optional ByVal DateStart As String = "", Optional ByVal DateEnd As String = "", Optional ByVal maxOpNumber As String = "") As DataTable

        Return _dao.GetProtocolLogStatisticsTable(User, DateStart, DateEnd, maxOpNumber)
    End Function

    ''' <summary>
    ''' Funzione che cerca il numero degli utenti che hanno avuto accesso al database
    ''' </summary>
    ''' <returns>il numero degli utenti che hanno avuto accesso al database</returns>
    ''' <remarks></remarks>
    Public Function GetProtocolUsersCount() As Integer
        Return _dao.GetProtocolUsersCount()
    End Function

    Public Function CountMailRolesLogs(uniqueId As Guid) As Integer
        Return _dao.CountMailRolesLogs(uniqueId)
    End Function

    Public Function GetMailRolesLogs(uniqueId As Guid) As IList(Of ProtocolLog)
        Return _dao.GetMailRolesLogs(uniqueId)
    End Function

#End Region
End Class