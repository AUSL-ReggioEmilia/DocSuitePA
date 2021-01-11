Imports System
Imports System.Linq
Imports NHibernate
Imports VecompSoftware.Helpers.NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateSecurityGroupsDao
    Inherits BaseNHibernateDao(Of SecurityGroups)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary> Restituisce l'Id con valore massimo. </summary>
    Public Function GetMaxId() As Integer
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.SetProjection(Projections.Max("Id"))

        Return criteria.UniqueResult(Of Integer)()
    End Function

    Public Function GetGroupByName(groupName As String) As SecurityGroups
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("GroupName", groupName))
        criteria.Add(Restrictions.Eq("TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))

        Return criteria.UniqueResult(Of SecurityGroups)()
    End Function

    ''' <summary> Restituisce tutti i gruppi ROOT (senza padre) filtrando per nome del gruppo. </summary>
    Public Function GetRootGroups(name As String) As IList(Of SecurityGroups)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.IsNull("Parent"))
        criteria.Add(Restrictions.Like("GroupName", name, MatchMode.Anywhere))
        criteria.Add(Restrictions.Eq("TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
        criteria.AddOrder(Order.Asc("GroupName"))

        Return criteria.List(Of SecurityGroups)()
    End Function

    ''' <summary> Restituisce tutti i gruppi filtrando per nome del gruppo. </summary>
    Public Function GetGroupsFlat(name As String) As IList(Of SecurityGroups)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Like("GroupName", name, MatchMode.Anywhere))
        criteria.Add(Restrictions.Eq("TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
        criteria.AddOrder(Order.Asc("GroupName"))

        Return criteria.List(Of SecurityGroups)()
    End Function

    Public Function GetByUser(account As String, domain As String) As IList(Of SecurityGroups)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("SecurityUsers", "SG")
        criteria.Add(Restrictions.Eq("SG.Account", account))
        criteria.Add(Restrictions.Eq("SG.UserDomain", domain))
        criteria.AddOrder(Order.Asc("GroupName"))

        Return criteria.List(Of SecurityGroups)()
    End Function
End Class
