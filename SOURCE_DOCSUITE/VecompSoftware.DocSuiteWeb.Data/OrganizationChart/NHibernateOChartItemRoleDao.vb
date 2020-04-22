Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate.Criterion
Imports NHibernate

Public Class NHibernateOChartItemRoleDao
    Inherits BaseNHibernateDao(Of OChartItemRole)


    Public Function GetVariations(source As OChart, destination As OChart) As IList(Of OChartItemRole)
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of OChartItemRole)("S")
        dc.CreateAlias("S.Item", "SOCI")
        dc.Add(Restrictions.Eq("SOCI.OrganizationChart.Id", destination.Id))
        dc.Add(Restrictions.EqProperty("SOCI.FullCode", "MOCI.FullCode"))
        dc.Add(Restrictions.EqProperty("S.Role.Id", "M.Role.Id"))
        dc.SetProjection(Projections.Constant(True))

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of OChartItemRole)("M")
        criteria.CreateAlias("M.Item", "MOCI")
        criteria.Add(Restrictions.Eq("MOCI.OrganizationChart.Id", source.Id))
        criteria.Add(Subqueries.NotExists(dc))

        Return criteria.List(Of OChartItemRole)()
    End Function

End Class