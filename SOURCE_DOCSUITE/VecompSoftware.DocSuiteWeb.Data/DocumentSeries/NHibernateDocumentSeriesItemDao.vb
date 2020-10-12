Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateDocumentSeriesItemDao
    Inherits BaseNHibernateDao(Of DocumentSeriesItem)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByIdentifiers(identifiers As List(Of Integer)) As IList(Of DocumentSeriesItem)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItem)("DSI")
        criteria.Add(Restrictions.In("DSI.Id", identifiers.ToArray()))
        criteria.AddOrder(Order.Asc("DSI.Id"))
        Return criteria.List(Of DocumentSeriesItem)()
    End Function

    Public Function GetItemNotDraftByIdentifiers(identifiers As List(Of Integer)) As IList(Of DocumentSeriesItem)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItem)("DSI")
        criteria.Add(Restrictions.In("DSI.Id", identifiers.ToArray()))
        criteria.Add(Restrictions.Not(Restrictions.Eq("DSI.Status", DocumentSeriesItemStatus.Draft)))
        criteria.AddOrder(Order.Asc("DSI.Id"))
        Return criteria.List(Of DocumentSeriesItem)()
    End Function

    Public Function GetItemDraftByIdentifiers(identifiers As List(Of Integer)) As IList(Of DocumentSeriesItem)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItem)("DSI")
        criteria.Add(Restrictions.In("DSI.Id", identifiers.ToArray()))
        criteria.Add(Restrictions.Eq("DSI.Status", DocumentSeriesItemStatus.Draft))
        criteria.AddOrder(Order.Asc("DSI.Id"))
        Return criteria.List(Of DocumentSeriesItem)()
    End Function

    Public Function GetByYearAndNumber(series As DocumentSeries, year As Integer, number As Integer) As DocumentSeriesItem
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItem)("DSI")
        criteria.Add(Restrictions.Eq("DSI.DocumentSeries", series))
        criteria.Add(Restrictions.Eq("DSI.Year", year))
        criteria.Add(Restrictions.Eq("DSI.Number", number))
        Return criteria.UniqueResult(Of DocumentSeriesItem)()
    End Function

    Public Function GetDocumentSeriesItem() As IList(Of Integer)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItem)("DSI")
        criteria.Add(Restrictions.Eq("DSI.Status", DocumentSeriesItemStatus.Draft))
        GetResolutionDocumentSeriesItem()
        criteria.SetProjection(Projections.Property("DSI.Id"))
        Return criteria.List(Of Integer)()
    End Function

    Public Sub GetResolutionDocumentSeriesItem()
        Dim rdsi As DetachedCriteria = DetachedCriteria.For(GetType(ResolutionDocumentSeriesItem), "RDSI")
        rdsi.SetProjection(Projections.Property("RDSI.IdDocumentSeriesItem"))
        criteria.Add(Subqueries.PropertyIn("DSI.Id", rdsi))
    End Sub

    Public Function GetCountByCategory(category As Category) As Long
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Category", category))
        criteria.SetProjection(Projections.RowCountInt64())
        Return criteria.UniqueResult(Of Long)()
    End Function

    Public Function GetByDocument(idDocument As Guid) As IList(Of DocumentSeriesItem)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItem)("DSI")
        Dim disjunction As Disjunction = New Disjunction()
        disjunction.Add(Restrictions.Eq("DSI.IdMain", idDocument))
        disjunction.Add(Restrictions.Eq("DSI.IdAnnexed", idDocument))
        disjunction.Add(Restrictions.Eq("DSI.IdUnpublishedAnnexed", idDocument))
        criteria.Add(disjunction)
        Return criteria.List(Of DocumentSeriesItem)()
    End Function

    Public Function GetByUniqueId(uniqueId As Guid) As DocumentSeriesItem
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItem)("DSI")
        criteria.Add(Restrictions.Eq("DSI.UniqueId", uniqueId))
        Return criteria.UniqueResult(Of DocumentSeriesItem)
    End Function

    Public Function CountBySubCategory(category As Category) As Long
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("SubCategory", category))
        criteria.SetProjection(Projections.RowCountInt64())
        Return criteria.UniqueResult(Of Long)()
    End Function

    Public Function GetLastSeriesByArchive(idArchive As Integer, topResults As Integer) As IList(Of DocumentSeriesItem)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItem)("DSI")
        criteria.CreateAlias("DSI.DocumentSeries", "DocumentSeries", SqlCommand.JoinType.InnerJoin)
        criteria.CreateAlias("DocumentSeries.Container", "Container", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("Container.Archive.Id", idArchive))
        criteria.Add(Restrictions.Eq("DSI.Status", DocumentSeriesItemStatus.Active))
        criteria.Add(Restrictions.IsNotNull("PublishingDate"))

        Dim disjunction As Disjunction = New Disjunction()
        disjunction.Add(Restrictions.IsNull("RetireDate"))
        Dim conjunction As Conjunction = New Conjunction()
        conjunction.Add(Restrictions.IsNotNull("RetireDate"))
        conjunction.Add(Restrictions.Ge("RetireDate", Date.Now))
        disjunction.Add(conjunction)
        criteria.Add(disjunction)

        criteria.SetMaxResults(topResults)
        criteria.AddOrder(Order.Desc("PublishingDate"))
        Return criteria.List(Of DocumentSeriesItem)
    End Function

    Public Function GetItemNotDraftByProtocol(year As Short, number As Integer) As IList(Of DocumentSeriesItem)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItem)("DSI")
        criteria.CreateAlias("DSI.ProtocolDocumentSeriesItems", "ProtocolDocumentSeriesItems", SqlCommand.JoinType.InnerJoin)
        criteria.CreateAlias("ProtocolDocumentSeriesItems.Protocol", "P")
        criteria.Add(Restrictions.Eq("P.Year", year))
        criteria.Add(Restrictions.Eq("P.Number", number))
        criteria.Add(Restrictions.Not(Restrictions.Eq("DSI.Status", DocumentSeriesItemStatus.Draft)))
        criteria.AddOrder(Order.Asc("DSI.Id"))
        Return criteria.List(Of DocumentSeriesItem)()
    End Function
End Class
