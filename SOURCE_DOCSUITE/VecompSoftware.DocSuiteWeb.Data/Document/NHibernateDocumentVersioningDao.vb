Imports System.Collections.Generic
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion

Public Class NHibernateDocumentVersioningDao
    Inherits BaseNHibernateDao(Of DocumentVersioning)


    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Function GetDocumentVersion(ByVal year As Short, ByVal number As Integer, ByVal incrementalObject As Short, ByVal checkStatus As String) As DocumentVersioning
        Dim list As IList(Of DocumentVersioning)

        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("DocumentObject", "DocumentObject", SqlCommand.JoinType.LeftOuterJoin)

        criteria.Add(Restrictions.Eq("Id.Year", year))
        criteria.Add(Restrictions.Eq("Id.Number", number))

        criteria.Add(Restrictions.Eq("CheckStatus", checkStatus))

        If incrementalObject <> 0 Then
            criteria.Add(Restrictions.Eq("IncrementalObject", incrementalObject))
        End If

        list = criteria.List(Of DocumentVersioning)()
        If list.Count > 0 Then
            Return list(list.Count - 1)
        Else
            Return Nothing
        End If
    End Function

    Function DocumentVersionings(ByVal year As Short, ByVal number As Integer, ByVal incrementalObject As Short) As IList(Of DocumentVersioning)

        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("DocumentObject", "DocumentObject", SqlCommand.JoinType.LeftOuterJoin)

        criteria.Add(Restrictions.Eq("Id.Year", year))
        criteria.Add(Restrictions.Eq("Id.Number", number))

        criteria.Add(Expression.Or(Restrictions.Eq("IncrementalObject", incrementalObject), Restrictions.Eq("DocumentObject.ValidIncremental", incrementalObject)))

        Return criteria.List(Of DocumentVersioning)()

    End Function

    Function GetDocumentVersionCount(ByVal YearNumberInc As YearNumberIncrCompositeKey) As Integer
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Id.Year", YearNumberInc.Year))
        criteria.Add(Restrictions.Eq("Id.Number", YearNumberInc.Number))
        criteria.Add(Restrictions.Eq("Id.Incremental", YearNumberInc.Incremental))
        criteria.SetProjection(Projections.RowCount())

        Return criteria.UniqueResult(Of Integer)()
    End Function

    ''' <summary>
    ''' Controlla quante righe di Versioning sono associate ad uno specifico documentObject 
    ''' </summary>
    ''' <param name="YearNumberInc">Chiave per identificare univocamente il documentObject</param>
    ''' <returns>Il numero dei versioning relativi al documento</returns>
    Function GetVersioningForDocumentObject(ByVal YearNumberInc As YearNumberIncrCompositeKey) As Integer

        Dim command As IDbCommand = New SqlClient.SqlCommand()

        Const sqlQuery As String = "SELECT * FROM DocumentVersioning AS V " & _
                                   "LEFT JOIN DocumentObject AS O ON (O.year=V.year AND O.Number=V.Number AND O.ValidIncremental=@incremental ) " & _
                                   "WHERE V.Year = @year AND V.Number = @number " & _
                                   "AND (V.IncrementalObject = @incremental OR O.ValidIncremental=@incremental)"

        command.Connection = NHibernateSession.Connection
        command.CommandText = sqlQuery

        command.Parameters.Add(New SqlClient.SqlParameter("@year", YearNumberInc.Year.Value))
        command.Parameters.Add(New SqlClient.SqlParameter("@number", YearNumberInc.Number.Value))
        command.Parameters.Add(New SqlClient.SqlParameter("@incremental", YearNumberInc.Incremental.Value))

        Dim dt As DataTable = New DataTable("Count")
        Dim cReader As IDataReader = command.ExecuteReader()
        dt.Load(cReader)

        Return dt.Rows.Count

    End Function

    Public Function DocumentVersioningSearch(ByVal YearNumberIncr As YearNumberIncrCompositeKey, ByVal CheckStatus As String) As IList(Of DocumentVersioning)
        criteria = NHibernateSession.CreateCriteria(persitentType, "DV")

        criteria.Add(Restrictions.Eq("DV.Id", YearNumberIncr))
        criteria.Add(Restrictions.Eq("DV.CheckStatus", CheckStatus))

        Return criteria.List(Of DocumentVersioning)()

    End Function

    Public Function GetMaxId(ByVal year As Short, ByVal number As Integer) As Short
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Id.Year", year))
        criteria.Add(Restrictions.Eq("Id.Number", number))

        criteria.SetProjection(Projections.ProjectionList.Add(Projections.Max("Id.Incremental")))

        Return criteria.UniqueResult(Of Short)() + 1S

    End Function

    Public Function GetVersioningAll(ByVal year As Short, ByVal number As Integer, ByVal incrementalObject As Short, ByVal checkStatus As String) As IList(Of DocumentVersioning)

        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("DocumentObject", "DocumentObject", SqlCommand.JoinType.LeftOuterJoin)

        criteria.Add(Restrictions.Eq("Id.Year", year))
        criteria.Add(Restrictions.Eq("Id.Number", number))

        If incrementalObject <> 0 Then
            criteria.Add(Restrictions.Eq("IncrementalObject", incrementalObject))
        End If

        criteria.Add(Restrictions.Eq("CheckStatus", checkStatus))

        Return criteria.List(Of DocumentVersioning)()

    End Function

End Class


