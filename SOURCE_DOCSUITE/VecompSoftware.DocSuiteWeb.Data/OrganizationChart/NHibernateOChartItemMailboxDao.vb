Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate.Criterion
Imports NHibernate

Public Class NHibernateOChartItemMailboxDao
    Inherits BaseNHibernateDao(Of OChartItemMailbox)

    Public Function GetVariations(source As OChart, destination As OChart) As IList(Of OChartItemMailbox)
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of OChartItemMailbox)("S")
        dc.CreateAlias("S.Item", "SOCI")
        dc.Add(Restrictions.Eq("SOCI.OrganizationChart.Id", destination.Id))
        dc.Add(Restrictions.EqProperty("SOCI.FullCode", "MOCI.FullCode"))
        dc.Add(Restrictions.EqProperty("S.Mailbox.Id", "M.Mailbox.Id"))
        dc.SetProjection(Projections.Constant(True))

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of OChartItemMailbox)("M")
        criteria.CreateAlias("M.Item", "MOCI")
        criteria.Add(Restrictions.Eq("MOCI.OrganizationChart.Id", source.Id))
        criteria.Add(Subqueries.NotExists(dc))

        Return criteria.List(Of OChartItemMailbox)()
    End Function

End Class