Imports System
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()>
Public Class ResolutionLogFacade
    Inherits BaseResolutionFacade(Of ResolutionLog, Integer, NHibernateResolutionLogDao)

#Region " Methods "

    ''' <summary> Ritorna una datatable con le statistiche estrapolate dal log. </summary>
    ''' <param name="user">utente che ha eseguito le operazioni</param>
    ''' <param name="dateStart">data inizio</param>
    ''' <param name="dateEnd">data fine</param>
    ''' <param name="maxOpNumber">minimo numero delle operazioni da cercare</param>
    ''' <returns>datatable</returns>
    ''' <remarks> le date devono essere stringhe in formato yyyyMMdd</remarks>
    Function GetResolutionLogStatisticsTable(Optional ByVal user As String = "", Optional ByVal dateStart As String = "", Optional ByVal dateEnd As String = "", Optional ByVal maxOpNumber As String = "") As DataTable
        Return _dao.GetResolutionLogStatisticsTable(user, dateStart, dateEnd, maxOpNumber)
    End Function

    ''' <summary> Funzione che cerca il numero degli utenti che hanno avuto accesso al database. </summary>
    ''' <returns>il numero degli utenti che hanno avuto accesso al database</returns>
    Public Function GetResolutionUsersCount() As Integer
        Return _dao.GetResolutionUsersCount
    End Function

    Public Overloads Sub Log(res As Resolution, type As ResolutionLogType)
        Log(res, type, "")
    End Sub

    Public Overloads Sub Log(res As Resolution, type As ResolutionLogType, codice As Integer)
        Log(res, type, "ERRORE # " + codice.ToString())
    End Sub

    Public Overloads Sub Log(res As Resolution, type As ResolutionLogType, message As String, Optional needTransaction As Boolean = True)
        Try
            Insert(res, type, message, needTransaction)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore nell'inserimento del log degli atti", ex)
        End Try
    End Sub

    Public Sub InsertRoles(ByRef resolution As Resolution, ByVal roles As ICollection(Of Integer), ByVal type As String)
        If roles Is Nothing Then
            Exit Sub
        End If

        Dim roleFac As RoleFacade = New RoleFacade("ReslDB")
        For Each idRole As Integer In roles
            Dim roleName As String = idRole & " " & roleFac.GetById(idRole).Name
            Insert(resolution, ResolutionLogType.RZ, String.Format("Autorizzazione ({0}): {1}", type, roleName))
        Next
    End Sub

    ''' <summary> Inserisce nel log degli atti un evento. </summary>
    Public Function Insert(ByVal resolution As Resolution, ByVal type As ResolutionLogType, ByVal description As String, Optional needTransaction As Boolean = True) As Boolean
        Return Insert(resolution, type.ToString(), description, needTransaction)
    End Function

    Public Function Insert(ByVal idResolution As Integer, ByVal type As ResolutionLogType, ByVal description As String, Optional needTransaction As Boolean = True) As Boolean
        Dim resolution As Resolution = Factory.ResolutionFacade.GetById(idResolution)
        Return Insert(resolution, type.ToString(), description, needTransaction)
    End Function

    ''' <summary> Inserisce nel log degli atti un evento. </summary>
    ''' <param name="resolution">Atto</param>
    ''' <param name="type">Tipo di log, viene ridotto a 255 caratteri</param>
    ''' <param name="logDescription">Descrizione</param>
    ''' <remarks>Attenzione alle eccezioni</remarks>
    Private Function Insert(ByVal resolution As Resolution, ByVal type As String, ByVal logDescription As String, Optional needTransaction As Boolean = True) As Boolean

        If EnableLogInsert(resolution, type) Then

            Dim log As New ResolutionLog()
            log.LogType = type
            log.LogDescription = logDescription
            log.LogDate = _dao.GetServerDate()
            log.SystemComputer = DocSuiteContext.Current.UserComputer
            log.SystemUser = DocSuiteContext.Current.User.FullUserName
            log.Program = DocSuiteContext.Program
            log.IdResolution = resolution.Id
            log.UniqueIdResolution = resolution.UniqueId

            Try
                Save(log, _dbName, needTransaction)
            Catch ex As Exception
                Dim message As String = "Errore nell'inserimento del log degli atti: LogType: {0} LogDescription: {1}"
                message = String.Format(message, log.LogType, log.LogDescription)
                FileLogger.Error(LoggerName, message, ex)
            End Try
        End If

        Return True
    End Function

    ''' <summary>Controlla se è possibile loggare l'atto</summary>
    Protected Function EnableLogInsert(ByVal resolution As Resolution, ByVal logType As String) As Boolean
        If Not DocSuiteContext.Current.ResolutionEnv.IsLogEnabled Then
            Return False
        End If

        If resolution Is Nothing OrElse String.IsNullOrEmpty(logType) Then
            Return False
        End If

        Return True
    End Function

    Public Sub InsertSbSuccesfullSendLog(resl As Resolution)
        Insert(resl, ResolutionLogType.SC, "Serializzazione comando Attestazione di Conformità")
    End Sub
    Public Function GetlastResolutionLog(ByVal idResolution As Integer, ByVal type As ResolutionLogType) As ResolutionLog
        Return _dao.GetlastResolutionLog(idResolution, type)
    End Function
#End Region

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

End Class