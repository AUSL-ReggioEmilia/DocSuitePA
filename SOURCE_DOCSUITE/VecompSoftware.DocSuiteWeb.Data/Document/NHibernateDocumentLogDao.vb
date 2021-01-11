Imports System
Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateDocumentLogDao
    Inherits BaseNHibernateDao(Of DocumentLog)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetMaxId() As Integer
        Dim query As String = "SELECT MAX(DL.Id) FROM DocumentLog AS DL"

        Try
            Dim _maxID As Object = NHibernateSession.CreateQuery(query).UniqueResult(Of Integer)
            If _maxID Is Nothing Then
                Return 0
            Else
                Return CType(_maxID, Integer)
            End If
        Catch ex As Exception
            Return 0
        End Try

    End Function

    Public Function GetLogByYearNumber(ByVal year As Short, ByVal number As Integer) As IList(Of DocumentLog)


        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Year", year))
        criteria.Add(Restrictions.Eq("Number", number))

        criteria.AddOrder(Order.Desc("Id"))

        Return criteria.List(Of DocumentLog)()

    End Function

    Function GetDocumentLogStatisticsTable(Optional ByVal User As String = "", Optional ByVal DateStart As String = "", Optional ByVal DateEnd As String = "", Optional ByVal maxOpNumber As String = "") As System.Data.DataTable

        Dim sQry As String
        Dim sWhere As String = ""
        Dim sInternalWhere As String = ""
        Dim command As IDbCommand = New SqlClient.SqlCommand()

        command.Connection = NHibernateSession.Connection

        If User <> String.Empty Then
            sWhere &= "AND ([SystemUser] LIKE " & "'%" & User.Replace("'", "''") & "%'" & ")"
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

        sQry = "SELECT [SystemUser], COUNT(Id) AS TotalOperationsCount, " & _
         "(SELECT COUNT(Id) FROM DocumentLog AS I " & sInternalWhere & " LogType = 'DI' AND I.[SystemUser] = E.[SystemUser]) AS ICount, " & _
         "(SELECT COUNT(Id) FROM DocumentLog AS I " & sInternalWhere & " LogType = 'DS' AND I.[SystemUser] = E.[SystemUser]) AS SCount, " & _
         "(SELECT COUNT(Id) FROM DocumentLog AS I " & sInternalWhere & " LogType = 'DD' AND I.[SystemUser] = E.[SystemUser]) AS DCount, " & _
         "(SELECT COUNT(Id) FROM DocumentLog AS I " & sInternalWhere & " LogType NOT IN ('DI', 'DS', 'DD') AND I.[SystemUser] = E.[SystemUser]) AS OTCount " & _
         "FROM DocumentLog AS E " & _
         sWhere & _
         "GROUP BY [SystemUser] "

        If maxOpNumber <> String.Empty Then sQry &= "HAVING (COUNT(Id) >= " & maxOpNumber.ToString() & ") "
        sQry &= "ORDER BY COUNT(Id) DESC"

        command.CommandText = sQry

        Dim cReader As IDataReader = command.ExecuteReader()
        Dim dt As DataTable = New DataTable("Log")
        dt.Load(cReader)
        Return dt

    End Function

    Public Function GetDocumentUsersCount() As Integer
        Dim qry As String = String.Empty
        Dim ret As Object
        Dim command As IDbCommand = New SqlClient.SqlCommand()

        command.Connection = NHibernateSession.Connection

        qry = "SELECT COUNT(DISTINCT [SystemUser]) AS UsersCount FROM DocumentLog"

        command.CommandText = qry

        ret = command.ExecuteScalar()
        If Not Convert.IsDBNull(ret) Then
            Return ret.ToString()
        Else
            Return "0"
        End If
    End Function


End Class
