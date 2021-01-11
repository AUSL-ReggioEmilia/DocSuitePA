Imports NHibernate.Criterion
Imports NHibernate
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernatePECMailViewDao
    Inherits BaseNHibernateDao(Of PECMailView)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByAllowedPageType(ByVal allowedPageType As String) As IList(Of PECMailView)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        Dim disj As New Disjunction()
        disj.Add(Restrictions.IsNull("ExclusivePageType"))
        disj.Add(Restrictions.Eq("ExclusivePageType", allowedPageType))
        crit.Add(disj)
        Return crit.List(Of PECMailView)()
    End Function
End Class
