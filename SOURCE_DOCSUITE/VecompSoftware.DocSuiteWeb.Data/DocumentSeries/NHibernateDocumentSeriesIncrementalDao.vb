Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.Helpers.NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform

Public Class NHibernateDocumentSeriesIncrementalDao
    Inherits BaseNHibernateDao(Of DocumentSeriesIncremental)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetNewIncremental(series As DocumentSeries, year As Integer) As DocumentSeriesIncremental
        Dim session As ISession = NHibernateSessionManager.Instance.GetSessionFrom(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
        Dim commandText As String = "select * from DocumentSeriesIncremental with (xlock, rowlock) where IdDocumentSeries= :IdDocumentSeries and Year= :Year"
        Dim sqlQuery As ISQLQuery = session.CreateSQLQuery(commandText)
        With sqlQuery
            .SetParameter("IdDocumentSeries", series.Id)
            .SetParameter("Year", year)
            .AddEntity(GetType(DocumentSeriesIncremental))
            .SetMaxResults(1)
        End With
        Dim tx As ITransaction = session.BeginTransaction(IsolationLevel.Serializable)
        Try
            Dim incremental As DocumentSeriesIncremental = sqlQuery.UniqueResult(Of DocumentSeriesIncremental)()
            If incremental Is Nothing Then
                incremental = New DocumentSeriesIncremental() With {.DocumentSeries = series, .Year = year, .IsOpen = True}
            End If
            session.Refresh(incremental)
            incremental.LastUsedNumber = incremental.LastUsedNumber.GetValueOrDefault(0) + 1
            UpdateWithoutTransaction(incremental)
            tx.Commit()
            Return incremental
        Catch ex As Exception
            tx.Rollback()
            Throw ex
        Finally
            session.Flush()
        End Try
    End Function


    Public Function GetOpenDocumentSeries() As IList(Of DocumentSeriesIncremental)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesIncremental)("DSI")
        criteria.Add(Restrictions.Eq("DSI.IsOpen", True))
        Return criteria.List(Of DocumentSeriesIncremental)()
    End Function

    Public Function GetAllDocumentSeries(IdSeries As Integer) As IList(Of DocumentSeriesIncremental)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesIncremental)("DSI")
        criteria.CreateAliasIfNotExists("DocumentSeries", "DS")
        criteria.Add(Restrictions.Eq("DS.Id", IdSeries))
        Return criteria.List(Of DocumentSeriesIncremental)()
    End Function

    Public Function GetDocumentIncrementalSeriesById(IdSeries As Integer) As DocumentSeriesIncremental
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesIncremental)("DSI")
        criteria.Add(Restrictions.Eq("DSI.Id", IdSeries))
        Return criteria.UniqueResult(Of DocumentSeriesIncremental)()
    End Function


    Public Function GetOpenDocumentSeries(IdSeries As Integer) As IList(Of DocumentSeriesIncremental)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesIncremental)("DSI")
        criteria.CreateAliasIfNotExists("DocumentSeries", "DS")
        criteria.Add(Restrictions.Eq("DS.Id", IdSeries))
        criteria.Add(Restrictions.Eq("DSI.IsOpen", True))
        Return criteria.List(Of DocumentSeriesIncremental)()
    End Function

    Public Function CountOpenDocumentSeries(IdSeries As Integer) As Integer
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesIncremental)("DSI")
        criteria.CreateAliasIfNotExists("DocumentSeries", "DS")
        criteria.Add(Restrictions.Eq("DS.Id", IdSeries))
        criteria.Add(Restrictions.Eq("DSI.IsOpen", True))
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        criteria.SetProjection(Projections.Count(Projections.Id))
        Return criteria.UniqueResult(Of Integer)()
    End Function

    Public Function CountOpenDocumentIncrementalSeriesByYear(idSeries As Integer, year As Integer) As Integer
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesIncremental)("DSI")
        criteria.CreateAliasIfNotExists("DocumentSeries", "DS")
        criteria.Add(Restrictions.Eq("DS.Id", idSeries))
        criteria.Add(Restrictions.Eq("DSI.Year", year))
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        criteria.SetProjection(Projections.Count(Projections.Id))
        Return criteria.UniqueResult(Of Integer)()
    End Function

    Public Function GetOpenDocumentById(IdSeries As Integer) As DocumentSeriesIncremental
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesIncremental)("DSI")
        criteria.CreateAliasIfNotExists("DocumentSeries", "DS")
        criteria.Add(Restrictions.Eq("DS.Id", IdSeries))
        criteria.Add(Restrictions.Eq("DSI.IsOpen", True))
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        criteria.SetProjection(Projections.Count(Projections.Id))
        Return criteria.UniqueResult(Of DocumentSeriesIncremental)()
    End Function

    Public Function GetLastDocumentSeriesIncremental(IdSeries As Integer) As DocumentSeriesIncremental
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesIncremental)("DSI")
        criteria.CreateAliasIfNotExists("DSI.DocumentSeries", "DS")
        criteria.Add(Restrictions.Eq("DS.Id", IdSeries))
        criteria.SetMaxResults(1)
        criteria.AddOrder(Order.Desc("Year"))
        Return criteria.UniqueResult(Of DocumentSeriesIncremental)()
    End Function
End Class
