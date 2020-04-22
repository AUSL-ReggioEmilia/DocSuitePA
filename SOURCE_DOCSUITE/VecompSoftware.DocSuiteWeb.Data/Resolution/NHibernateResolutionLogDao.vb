Imports System
Imports System.Linq
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateResolutionLogDao
    Inherits BaseNHibernateDao(Of ResolutionLog)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Function GetResolutionLogStatisticsTable(Optional ByVal User As String = "", Optional ByVal DateStart As String = "", Optional ByVal DateEnd As String = "", Optional ByVal maxOpNumber As String = "") As System.Data.DataTable
        Dim sQry As String
        Dim sWhere As String = ""
        Dim sInternalWhere As String = ""
        Dim command As IDbCommand = New SqlClient.SqlCommand()

        command.Connection = NHibernateSession.Connection

        If User <> String.Empty Then
            sWhere &= "AND ([SystemUser] LIKE " & "'" & User.Replace("'", "''") & "%'" & ")"
        End If

        If DateStart <> String.Empty Then
            sWhere &= "AND (LogDate >= '" & DateStart & "') "
        End If

        If DateEnd <> String.Empty Then
            sWhere &= "AND (LogDate <= '" & DateEnd & " 23:59:59') "
        End If

        If sWhere <> "" Then
            sWhere = "WHERE " & sWhere.Substring(3)
            sInternalWhere = sWhere & " AND"
        Else
            sInternalWhere = "WHERE "
        End If

        sQry = "SELECT [SystemUser], COUNT(Id) AS TotalOperationsCount, " &
         "(SELECT COUNT(Id) FROM ResolutionLog AS I " & sInternalWhere & " LogType = 'RI' AND I.[SystemUser] = E.[SystemUser]) AS ICount, " &
         "(SELECT COUNT(Id) FROM ResolutionLog AS I " & sInternalWhere & " LogType = 'RS' AND I.[SystemUser] = E.[SystemUser]) AS SCount, " &
         "(SELECT COUNT(Id) FROM ResolutionLog AS I " & sInternalWhere & " LogType = 'RD' AND I.[SystemUser] = E.[SystemUser]) AS DCount, " &
         "(SELECT COUNT(Id) FROM ResolutionLog AS I " & sInternalWhere & " LogType NOT IN ('RI', 'RS', 'RD') AND I.[SystemUser] = E.[SystemUser]) AS OTCount " &
         "FROM ResolutionLog AS E " &
         sWhere &
         "GROUP BY [SystemUser] "

        If maxOpNumber <> String.Empty Then sQry &= "HAVING (COUNT(Id) >= " & maxOpNumber.ToString() & ") "
        sQry &= "ORDER BY COUNT(Id) DESC"

        command.CommandText = sQry

        Dim cReader As IDataReader = command.ExecuteReader()
        Dim dt As DataTable = New DataTable("Log")
        dt.Load(cReader)
        Return dt

    End Function

    Public Function GetResolutionUsersCount() As Integer
        Dim ret As Object
        Dim command As IDbCommand = New SqlClient.SqlCommand()

        command.Connection = NHibernateSession.Connection

        Dim qry As String = "SELECT COUNT(DISTINCT [SystemUser]) AS UsersCount FROM ResolutionLog"

        command.CommandText = qry

        ret = command.ExecuteScalar()
        If Not Convert.IsDBNull(ret) Then
            Return CType(ret, Integer)
        Else
            Return 0
        End If
    End Function

    ''' <summary> Restituisce l'id dell'ultimo log inserito </summary>
    Public Function GetMaxId() As Integer
        Dim query As String = "SELECT MAX(RL.Id) FROM ResolutionLog AS RL"

        Try
            Return NHibernateSession.CreateQuery(query).UniqueResult(Of Integer)()
        Catch ex As Exception
            Return 0
        End Try
    End Function
    Public Function GetlastResolutionLog(ByVal idResolution As Integer, ByVal resolutionLogType As ResolutionLogType) As ResolutionLog
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("IdResolution", idResolution))
        criteria.Add(Restrictions.Eq("LogType", resolutionLogType.ToString()))
        criteria.AddOrder(Order.Desc("LogDate"))
        Return criteria.List(Of ResolutionLog)().FirstOrDefault()
    End Function
End Class
