Imports System
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateParameterDao
    Inherits BaseNHibernateDao(Of Parameter)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetCurrent() As Parameter
        Dim vCriteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        vCriteria.Add(Restrictions.Eq("Locked", False))
        Return vCriteria.UniqueResult(Of Parameter)()
    End Function

    Public Function GetCurrentAndRefresh() As Parameter
        Dim param As Parameter = GetCurrent()
        NHibernateSession.Refresh(param)
        Return param
    End Function

    Public Function GetLastUsedResolutionYear() As Short
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Parameter)()
        criteria.Add(Restrictions.Eq("Id", 1))
        criteria.SetProjection(Projections.Property("LastUsedResolutionYear"))
        Return criteria.UniqueResult(Of Short)()
    End Function

    Sub UpdateLastUsedNumber(ByVal year As Int32)
        Dim session As ISession = NHibernateSessionManager.Instance.GetSessionFrom(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
        Dim sqlQuery As ISQLQuery = session.CreateSQLQuery("UPDATE Parameter SET LastUsedNumber = (SELECT MAX(Number) + 1 FROM Protocol WHERE [Year] = " & year.ToString() & ") WHERE Incremental = 1")
        sqlQuery.ExecuteUpdate()
    End Sub

    Sub UpdateLastIdRole(idRole As Integer, oldIdRole As Integer)
        Dim sqlQuery As ISQLQuery = NHibernateSession.CreateSQLQuery(String.Format("UPDATE Parameter SET LastUsedIdRole={0} WHERE LastUsedIdRole= {1}", idRole, oldIdRole))
        sqlQuery.ExecuteUpdate()
    End Sub

    Sub UpdateLastIdRoleUser(idRoleUser As Integer, oldIdRoleUser As Integer)
        Dim sqlQuery As ISQLQuery = NHibernateSession.CreateSQLQuery(String.Format("UPDATE Parameter SET LastUsedIdRoleUser={0} WHERE LastUsedIdRoleUser= {1}", idRoleUser, oldIdRoleUser))
        sqlQuery.ExecuteUpdate()
    End Sub

    Public Function GetLastUsedIdRole() As Short
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Locked", False))
        criteria.SetProjection(Projections.Property("LastUsedIdRole"))
        Return criteria.UniqueResult(Of Short)()
    End Function

    Public Function GetLastUsedIdRoleUser() As Short
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Locked", False))
        criteria.SetProjection(Projections.Property("LastUsedIdRoleUser"))
        Return criteria.UniqueResult(Of Short)()
    End Function
End Class
