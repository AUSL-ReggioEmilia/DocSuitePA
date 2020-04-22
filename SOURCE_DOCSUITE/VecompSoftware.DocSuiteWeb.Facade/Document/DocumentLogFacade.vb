Imports System
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Logging

<ComponentModel.DataObject()> _
Public Class DocumentLogFacade
    Inherits BaseDocumentFacade(Of DocumentLog, Integer, NHibernateDocumentLogDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetLogByYearNumber(ByVal year As Short, ByVal number As Integer) As IList(Of DocumentLog)
        Return _dao.GetLogByYearNumber(year, number)
    End Function

    Public Sub Insert(ByVal Year As Short, ByVal Number As Integer, ByVal LogType As String, ByVal LogDescription As String)
        Dim log As New DocumentLog

        log.Year = Year
        log.Number = Number
        log.LogType = LogType
        log.LogDescription = LogDescription
        log.LogDate = _dao.GetServerDate()
        log.SystemComputer = DocSuiteContext.Current.UserComputer
        log.SystemUser = DocSuiteContext.Current.User.FullUserName
        log.Program = DocSuiteContext.Program

        Try
            Me.Save(log)
        Catch ex As Exception
            Dim message As String = "Errore nell'inserimento del log delle pratiche: LogType: {0} LogDescription: {1}"
            message = String.Format(message, log.LogType, log.LogDescription)
            FileLogger.Error(LoggerName, message, ex)
        End Try
    End Sub

    Public Sub InsertRoles(ByRef Document As Document, ByVal roles As ICollection(Of Integer), ByVal type As String)
        If roles IsNot Nothing Then
            For Each idRole As Integer In roles
                Dim roleName As String = String.Concat(idRole, " ", (New RoleFacade("DocmDB")).GetById(idRole).Name)
                Insert(Document.Year, Document.Number, "DZ", String.Format("Autorizzazione ({0}): {1}", type, roleName))
            Next
        End If
    End Sub

    ''' <summary>
    ''' Ritorna una datatable con le statistiche estrapolate dal log
    ''' </summary>
    ''' <param name="User">utente che ha eseguito le operazioni</param>
    ''' <param name="DateStart">data inizio</param>
    ''' <param name="DateEnd">data fine</param>
    ''' <param name="maxOpNumber">minimo numero delle operazioni da cercare</param>
    ''' <returns>datatable</returns>
    ''' <remarks> le date devono essere stringhe in formato yyyyMMdd</remarks>
    Function GetDocumentLogStatisticsTable(Optional ByVal User As String = "", Optional ByVal DateStart As String = "", Optional ByVal DateEnd As String = "", Optional ByVal maxOpNumber As String = "") As System.Data.DataTable
        Return _dao.GetDocumentLogStatisticsTable(User, DateStart, DateEnd, maxOpNumber)
    End Function

    ''' <summary>
    ''' Funzione che cerca il numero degli utenti che hanno avuto accesso al database
    ''' </summary>
    ''' <returns>il numero degli utenti che hanno avuto accesso al database</returns>
    ''' <remarks></remarks>
    Public Function GetDocumentlUsersCount() As Integer
        Return _dao.GetDocumentUsersCount()
    End Function

End Class