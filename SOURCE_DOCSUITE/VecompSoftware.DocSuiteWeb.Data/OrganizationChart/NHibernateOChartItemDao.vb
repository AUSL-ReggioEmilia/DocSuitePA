Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao


Public Class NHibernateOChartItemDao
    Inherits BaseNHibernateDao(Of OChartItem)

#Region " Constants "

#End Region

#Region " Methods "

    Public Function GetByFullCode(fullCode As String) As IList(Of OChartItem)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of OChartItem)()
        criteria.Add(Restrictions.Eq("FullCode", fullCode))
        Return criteria.List(Of OChartItem)()
    End Function

    Public Function GetVariations(source As OChart, destination As OChart) As IList(Of OChartItem)
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of OChartItem)("S")
        dc.Add(Restrictions.Eq("S.OrganizationChart.Id", destination.Id))
        dc.Add(Restrictions.EqProperty("S.FullCode", "M.FullCode"))
        dc.Add(Restrictions.Or(Restrictions.EqProperty("S.Title", "M.Title"), Restrictions.And(Restrictions.IsNull("S.Title"), Restrictions.IsNull("M.Title"))))
        dc.Add(Restrictions.Or(Restrictions.EqProperty("S.Description", "M.Description"), Restrictions.And(Restrictions.IsNull("S.Description"), Restrictions.IsNull("M.Description"))))
        dc.Add(Restrictions.Or(Restrictions.EqProperty("S.Acronym", "M.Acronym"), Restrictions.And(Restrictions.IsNull("S.Acronym"), Restrictions.IsNull("M.Acronym"))))
        dc.SetProjection(Projections.Constant(True))

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of OChartItem)("M")
        criteria.Add(Restrictions.Eq("OrganizationChart.Id", source.Id))
        criteria.Add(Subqueries.NotExists(dc))

        Return criteria.List(Of OChartItem)()
    End Function

    Public Function GetByCode(code As String) As IList(Of OChartItem)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Code", code))
        Return criteria.List(Of OChartItem)()
    End Function

    Public Function GetByRole(role As Role) As ICollection(Of OChartItem)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "O")
        criteria.CreateAlias("O.Roles", "OR")
        criteria.CreateAlias("OR.Role", "R")
        criteria.Add(Restrictions.Eq("R.Id", role.Id))
        Return criteria.List(Of OChartItem)()
    End Function

    Public Function GetByContact(contact As Contact) As ICollection(Of OChartItem)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "O")
        criteria.CreateAlias("O.Contacts", "OC")
        criteria.CreateAlias("OC.Contact", "C")
        criteria.Add(Restrictions.Eq("C.Id", contact.Id))
        Return criteria.List(Of OChartItem)()
    End Function

#End Region

End Class