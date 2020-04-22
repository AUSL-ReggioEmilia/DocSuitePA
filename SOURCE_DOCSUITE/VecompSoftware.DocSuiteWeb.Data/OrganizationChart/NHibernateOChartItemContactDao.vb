Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion

Public Class NHibernateOChartItemContactDao
    Inherits BaseNHibernateDao(Of OChartItemContact)

    Public Function GetVariations(source As OChart, destination As OChart) As IList(Of OChartItemContact)
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of OChartItemContact)("S")
        dc.CreateAlias("S.Item", "SOCI")
        dc.Add(Restrictions.Eq("SOCI.OrganizationChart.Id", destination.Id))
        dc.Add(Restrictions.EqProperty("SOCI.FullCode", "MOCI.FullCode"))
        dc.Add(Restrictions.EqProperty("S.Contact.Id", "M.Contact.Id"))
        dc.SetProjection(Projections.Constant(True))

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of OChartItemContact)("M")
        criteria.CreateAlias("M.Item", "MOCI")
        criteria.Add(Restrictions.Eq("MOCI.OrganizationChart.Id", source.Id))
        criteria.Add(Subqueries.NotExists(dc))

        Return criteria.List(Of OChartItemContact)()
    End Function

End Class