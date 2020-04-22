Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate

Public Class NHibernateResolutionRoleTypeDao
    Inherits BaseNHibernateDao(Of ResolutionRoleType)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetEnables() As IList(Of ResolutionRoleType)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ResolutionRoleType)("RRT")
       
        criteria.Add(Restrictions.Eq("RRT.Enabled", True))
        criteria.AddOrder(Order.Asc("RRT.SortOrder"))

        Return criteria.List(Of ResolutionRoleType)()
    End Function


    Public Function GetAllOrdered() As IList(Of ResolutionRoleType)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ResolutionRoleType)("RRT")

        criteria.AddOrder(Order.Asc("RRT.SortOrder"))

        Return criteria.List(Of ResolutionRoleType)()
    End Function


End Class
