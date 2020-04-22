Imports System.IO
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.SqlCommand

Public Class NHibernateOChartItemContainerDao
    Inherits BaseNHibernateDao(Of OChartItemContainer)

    Public Function GetMastersByContainer(itemContainer As OChartItemContainer) As IList(Of OChartItemContainer)

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Container", "C", JoinType.InnerJoin)
        criteria.CreateAlias("Item", "I", JoinType.InnerJoin)
        criteria.CreateAlias("I.OrganizationChart", "O", JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("O.Id", itemContainer.Item.OrganizationChart.Id))
        criteria.Add(Restrictions.Eq("Master", True))
        criteria.Add(Restrictions.Eq("C.Id", itemContainer.Container.Id))
        criteria.Add(Restrictions.Not(Restrictions.Eq("Id", itemContainer.Id)))
        Return criteria.List(Of OChartItemContainer)()
    End Function

    Public Function GetRejectionsByContainer(itemContainer As OChartItemContainer) As IList(Of OChartItemContainer)

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Container", "C", JoinType.InnerJoin)
        criteria.CreateAlias("Item", "I", JoinType.InnerJoin)
        criteria.CreateAlias("I.OrganizationChart", "O", JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("O.Id", itemContainer.Item.OrganizationChart.Id))
        criteria.Add(Restrictions.Eq("C.Id", itemContainer.Container.Id))
        criteria.Add(Restrictions.Not(Restrictions.Eq("Id", itemContainer.Id)))
        Return criteria.List(Of OChartItemContainer)()
    End Function


    Public Function GetVariations(source As OChart, destination As OChart) As IList(Of OChartItemContainer)
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of OChartItemContainer)("S")
        dc.CreateAlias("S.Item", "SOCI")
        dc.Add(Restrictions.Eq("SOCI.OrganizationChart.Id", destination.Id))
        dc.Add(Restrictions.EqProperty("SOCI.FullCode", "MOCI.FullCode"))
        dc.Add(Restrictions.EqProperty("S.Container.Id", "M.Container.Id"))
        dc.Add(Restrictions.Or(Restrictions.EqProperty("S.Master", "M.Master"), Restrictions.And(Restrictions.IsNull("S.Master"), Restrictions.IsNull("M.Master"))))
        dc.Add(Restrictions.Or(Restrictions.EqProperty("S.Rejection", "M.Rejection"), Restrictions.And(Restrictions.IsNull("S.Rejection"), Restrictions.IsNull("M.Rejection"))))
        dc.SetProjection(Projections.Constant(True))

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of OChartItemContainer)("M")
        criteria.CreateAlias("M.Item", "MOCI")
        criteria.Add(Restrictions.Eq("MOCI.OrganizationChart.Id", source.Id))
        criteria.Add(Subqueries.NotExists(dc))

        Return criteria.List(Of OChartItemContainer)()
    End Function

End Class