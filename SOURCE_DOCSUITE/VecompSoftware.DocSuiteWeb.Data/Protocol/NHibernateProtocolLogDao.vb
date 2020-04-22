Imports System
Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateProtocolLogDao
    Inherits BaseNHibernateDao(Of ProtocolLog)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Protected Overrides ReadOnly Property NHibernateSession() As NHibernate.ISession
        Get
            Return MyBase.NHibernateSession
        End Get
    End Property

    Public Function GetMaxId() As Integer
        Dim query As String = "SELECT MAX(PL.Id) FROM ProtocolLog AS PL"

        Try
            Return NHibernateSession.CreateQuery(query).UniqueResult(Of Integer)()
        Catch ex As Exception
            Return 0
        End Try

    End Function

    Public Function SearchLogByProtocolUniqueId(uniqueId As Guid, user As String, logType As String, logDescription As String) As IList(Of ProtocolLog)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        If Not uniqueId.Equals(Guid.Empty) Then
            criteria.Add(Restrictions.Eq("UniqueIdProtocol", uniqueId))
        End If

        If Not String.IsNullOrEmpty(user) Then
            criteria.Add(Restrictions.Eq("SystemUser", user))
        End If

        If Not String.IsNullOrEmpty(logType) Then
            criteria.Add(Restrictions.Eq("LogType", logType))
        End If

        If Not String.IsNullOrEmpty(logDescription) Then
            criteria.Add(Restrictions.Like("LogDescription", logDescription, MatchMode.Anywhere))
        End If

        Return criteria.List(Of ProtocolLog)()
    End Function

    Public Function SearchLog(ByVal anno As Short, ByVal numero As Integer, ByVal user As String, ByVal logType As String) As IList(Of ProtocolLog)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Year", anno))

        If numero <> 0 Then
            criteria.Add(Restrictions.Eq("Number", numero))
        End If

        If Not String.IsNullOrEmpty(user) Then
            criteria.Add(Restrictions.Eq("SystemUser", user))
        End If

        If Not String.IsNullOrEmpty(logType) Then
            criteria.Add(Restrictions.Eq("LogType", logType))
        End If

        criteria.AddOrder(Order.Desc("Id"))
        criteria.SetFirstResult(0)
        criteria.SetMaxResults(50)

        Return criteria.List(Of ProtocolLog)()
    End Function


    Public Function GetProtocolLogStatisticsTable(Optional ByVal user As String = "", Optional ByVal dateStart As String = "", _
                                            Optional ByVal dateEnd As String = "", Optional ByVal maxOpNumber As String = "") As DataTable

        Dim sQry As String
        Dim sWhere As String = ""
        Dim sInternalWhere As String = ""
        Dim command As IDbCommand = New SqlClient.SqlCommand()

        command.Connection = NHibernateSession.Connection

        If user <> String.Empty Then
            sWhere &= String.Format("AND ([SystemUser] LIKE '%{0}%')", user.Replace("'", "''"))
        End If

        If dateStart <> String.Empty Then
            sWhere &= "AND (LogDate >= '" & dateStart & "') "
        End If

        If dateEnd <> String.Empty Then
            sWhere &= "AND (LogDate <= '" & dateEnd & " 23:59:59') "
        End If

        If sWhere <> String.Empty Then
            sWhere = "WHERE " & sWhere.Substring(3)
            sInternalWhere = sWhere & " AND"
        Else
            sInternalWhere = "WHERE "
        End If

        sQry = "SELECT m.SystemUser as SystemUser, SUM(1) as TotalOperationsCount, " & _
         "sum(case when LogType='PI' then 1 else 0 end) as ICount, " & _
         "sum(case when LogType='PS' then 1 else 0 end) as SCount, " & _
         "sum(case when LogType='PD' then 1 else 0 end) as DCount, " & _
         "sum(case when LogType='PZ' then 1 else 0 end) as ZCount, " & _
         "sum(case when LogType='PM' then 1 else 0 end) as MCount, " & _
         "sum(case when not LogType in ('PI', 'PS', 'PD', 'PZ', 'PM') then 1 else 0 end) as OTCount " & _
         "FROM ProtocolLog AS m " & sWhere & "GROUP BY [SystemUser] "

        If maxOpNumber <> String.Empty Then sQry &= "HAVING (COUNT(Id) >= " & maxOpNumber.ToString() & ") "
        sQry &= "ORDER BY COUNT(Id) DESC"

        command.CommandText = sQry

        Dim cReader As IDataReader = command.ExecuteReader()
        Dim dt As DataTable = New DataTable("Log")
        dt.Load(cReader)
        Return dt

    End Function

    Public Function GetProtocolUsersCount() As Integer
        Dim command As IDbCommand = New SqlClient.SqlCommand()

        command.Connection = NHibernateSession.Connection

        command.CommandText = "SELECT COUNT(DISTINCT [SystemUser]) AS UsersCount FROM ProtocolLog"

        Dim ret As Object = command.ExecuteScalar()
        If Not Convert.IsDBNull(ret) Then
            Return ret
        End If

        Return 0
    End Function

    Public Function CountMailRolesLogs(uniqueId As Guid) As Integer
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("UniqueIdProtocol", uniqueId))
        criteria.Add(Restrictions.Eq("LogType", "PW"))
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer)
    End Function

    Public Function GetMailRolesLogs(uniqueId As Guid) As IList(Of ProtocolLog)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("UniqueIdProtocol", uniqueId))
        criteria.Add(Restrictions.Eq("LogType", "PW"))
        Return criteria.List(Of ProtocolLog)
    End Function

End Class
