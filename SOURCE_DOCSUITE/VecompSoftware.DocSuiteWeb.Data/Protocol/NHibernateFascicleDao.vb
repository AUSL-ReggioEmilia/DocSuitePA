Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateFascicleDao
    Inherits BaseNHibernateDao(Of Fascicle)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub


    Function GetByYearCategoryNumber(year As Short, idCategory As Integer, number As Integer) As Fascicle
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Year", year))
        criteria.Add(Restrictions.Eq("Number", number))
        criteria.Add(Restrictions.Eq("Category.Id", idCategory))
        Return criteria.UniqueResult(Of Fascicle)
    End Function

    Function CountFascicleByCategory(ByVal category As Category) As Long
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Category.Id", category.Id))
        criteria.SetProjection(Projections.RowCountInt64())
        Return criteria.UniqueResult(Of Long)
    End Function

End Class
