Imports System.Linq
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateDocumentSeriesFamilyDao
    Inherits BaseNHibernateDao(Of DocumentSeriesFamily)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overloads Function GetAll() As IList(Of DocumentSeriesFamily)

        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesFamily)("DSF")
        criteria.AddOrder(Order.Asc("DSF.SortOrder"))
        Return criteria.List(Of DocumentSeriesFamily)()

    End Function

    Public Function GetFamiliesByArchive(idArchive As Integer) As IList(Of DocumentSeriesFamily)
        Dim detachedExistsQuery As DetachedCriteria = DetachedCriteria.For(Of DocumentSeries)("DS")
        detachedExistsQuery.CreateAlias("DS.Container", "C")
        detachedExistsQuery.CreateAlias("C.Archive", "CA")
        detachedExistsQuery.Add(Restrictions.EqProperty("DS.Family.Id", "DSF.Id"))
        detachedExistsQuery.Add(Restrictions.Eq("CA.Id", idArchive))
        detachedExistsQuery.SetProjection(Projections.Constant(1))

        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesFamily)("DSF")
        criteria.Add(Subqueries.Exists(detachedExistsQuery))
        criteria.AddOrder(Order.Asc("DSF.SortOrder"))
        Return criteria.List(Of DocumentSeriesFamily)()
    End Function
End Class
