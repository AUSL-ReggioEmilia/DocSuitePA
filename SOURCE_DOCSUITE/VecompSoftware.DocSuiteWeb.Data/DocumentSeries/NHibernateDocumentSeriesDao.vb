Imports System.Linq
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports VecompSoftware.Helpers.NHibernate
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateDocumentSeriesDao
    Inherits BaseNHibernateDao(Of DocumentSeries)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub
    
    Public Function GetByIdentifiers(identifiers As Integer()) As IList(Of DocumentSeries)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeries)("DS")
        criteria.Add(Restrictions.In("DS.Id", identifiers))
        criteria.AddOrder(Order.Asc("DS.Id"))
        Return criteria.List(Of DocumentSeries)()
    End Function

    Public Function GetDocumentByContainerId(containerId As Integer) As DocumentSeries
        If (containerId = 0) Then
            Throw New ArgumentException(String.Format("Contenitore '{0}' inesistente", containerId))
        End If
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Container.Id", containerId))
        Dim results As ICollection(Of DocumentSeries) = criteria.List(Of DocumentSeries)()
        If (results.Count > 1) Then
            Throw New InvalidOperationException( _
                String.Format("Impossibile determinare quale serie documentale è associato al contenitore con Id {0}", containerId))
        End If
        Return results.SingleOrDefault()
    End Function

    Public Function GetByContainer(container As Container) As DocumentSeries
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Container", container))
        Return criteria.UniqueResult(Of DocumentSeries)()
    End Function

    Public Function GetByContainer(idContainer As Integer) As DocumentSeries
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Container.Id", idContainer))
        Return criteria.UniqueResult(Of DocumentSeries)()
    End Function

    Public Function GetPublicationEnabledDocumentSeries() As IList(Of DocumentSeries)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("PublicationEnabled", True))
        criteria.Add(Restrictions.Eq("Container.IsActive", 1))
        Return criteria.List(Of DocumentSeries)()
    End Function

    Public Function GetEmptyLogSummaries(idArchive As Integer?) As IList(Of DocumentSeriesLogSummaryDTO)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeries)("DS")

        If idArchive.HasValue Then
            criteria.CreateAliasIfNotExists("DS.Container", "DISCO")
            criteria.CreateAliasIfNotExists("DISCO.Archive", "DISCOCA")
            criteria.Add(Restrictions.Eq("DISCOCA.Id", idArchive.Value))
        End If

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.GroupProperty("DS.Id"), "Id")
        proj.Add(Projections.GroupProperty("DS.Name"), "Name")
        proj.Add(Projections.SubQuery(GetDetachedAvailableRowCount()), "Total")
        proj.Add(Projections.SubQuery(GetDetachedDraftedRowCount()), "Drafted")

        criteria.SetProjection(proj)
        criteria.SetResultTransformer(Transformers.AliasToBean(Of DocumentSeriesLogSummaryDTO))
        criteria.AddOrder(Order.Asc("DS.Name")).AddOrder(Order.Asc("DS.Id"))
        Return criteria.List(Of DocumentSeriesLogSummaryDTO)()
    End Function

    Private Function GetDetachedAvailableRowCount() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of DocumentSeriesItem)("AvailableRowCount")
        dc.Add(Restrictions.EqProperty("AvailableRowCount.DocumentSeries.Id", "DS.Id"))
        Dim canceled As DocumentSeriesItemStatus() = New DocumentSeriesItemStatus() {DocumentSeriesItemStatus.NotActive, DocumentSeriesItemStatus.Canceled}
        dc.Add(Restrictions.Not(Restrictions.In("AvailableRowCount.Status", canceled)))
        dc.SetProjection(Projections.RowCount())

        Return dc
    End Function

    Private Function GetDetachedDraftedRowCount() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of DocumentSeriesItem)("DraftedRowCount")
        dc.Add(Restrictions.EqProperty("DraftedRowCount.DocumentSeries.Id", "DS.Id"))
        dc.Add(Restrictions.Eq("DraftedRowCount.Status", DocumentSeriesItemStatus.Draft))
        dc.SetProjection(Projections.RowCount())

        Return dc
    End Function

    Public Function GetDocumentSeries() As IList(Of DocumentSeries)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("PublicationEnabled", True))
        criteria.Add(Restrictions.Eq("Container.IsActive", 1))
        Return criteria.List(Of DocumentSeries)()
    End Function

    Public Function GetSeriesByFamilyAndArchive(idFamily As Integer, idArchive As Integer) As IList(Of DocumentSeries)
        Dim detachedExistsQuery As DetachedCriteria = DetachedCriteria.For(Of Container)("C")
        detachedExistsQuery.CreateAlias("C.Archive", "CA")
        detachedExistsQuery.Add(Restrictions.EqProperty("C.Id", "DS.Container.Id"))
        detachedExistsQuery.Add(Restrictions.Eq("CA.Id", idArchive))
        detachedExistsQuery.SetProjection(Projections.Constant(1))

        criteria = NHibernateSession.CreateCriteria(Of DocumentSeries)("DS")
        criteria.Add(Subqueries.Exists(detachedExistsQuery))
        criteria.Add(Restrictions.Eq("DS.Family.Id", idFamily))
        Return criteria.List(Of DocumentSeries)
    End Function

    Public Function GetSeriesByArchive(idSeries As Integer, idArchive As Integer) As DocumentSeries
        Dim detachedExistsQuery As DetachedCriteria = DetachedCriteria.For(Of Container)("C")
        detachedExistsQuery.CreateAlias("C.Archive", "CA")
        detachedExistsQuery.Add(Restrictions.EqProperty("C.Id", "DS.Container.Id"))
        detachedExistsQuery.Add(Restrictions.Eq("CA.Id", idArchive))
        detachedExistsQuery.SetProjection(Projections.Constant(1))

        criteria = NHibernateSession.CreateCriteria(Of DocumentSeries)("DS")
        criteria.Add(Subqueries.Exists(detachedExistsQuery))
        criteria.Add(Restrictions.Eq("DS.Id", idSeries))
        Return criteria.UniqueResult(Of DocumentSeries)
    End Function
End Class
