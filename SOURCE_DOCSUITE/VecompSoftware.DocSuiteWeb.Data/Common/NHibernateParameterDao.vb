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

    Sub UpdateProtocolLastUsedNumber(year As Integer)
        Dim currentParameter As Parameter = GetCurrent()
        Dim session As ISession = NHibernateSessionManager.Instance.GetSessionFrom(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
        Dim sqlQuery As ISQLQuery = session.CreateSQLQuery($"UPDATE Parameter SET LastUsedNumber = (SELECT MAX(Number) + 1 FROM Protocol WHERE [Year] = {year}) WHERE Incremental = {currentParameter.Id}")
        sqlQuery.ExecuteUpdate()
    End Sub

    Sub UpdateDocumentLastUsedNumber(lastUsedNumber As Integer)
        Dim session As ISession = NHibernateSessionManager.Instance.GetSessionFrom(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.DocmDB))
        Dim sqlQuery As ISQLQuery = session.CreateSQLQuery($"UPDATE Parameter SET LastUsedNumber={lastUsedNumber}, Locked=0")
        sqlQuery.ExecuteUpdate()
    End Sub

    Sub UpdateResolutionLastUsedNumber(lastUsedNumber As Integer)
        Dim currentParameter As Parameter = GetCurrent()
        Dim session As ISession = NHibernateSessionManager.Instance.GetSessionFrom(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ReslDB))
        Dim sqlQuery As ISQLQuery = session.CreateSQLQuery($"UPDATE Parameter SET LastUsedResolutionNumber={lastUsedNumber}, Locked=0")
        sqlQuery.ExecuteUpdate()
    End Sub

    Sub UpdateResolutionLastUsedBillNumber(lastUsedBillNumber As Integer)
        Dim currentParameter As Parameter = GetCurrent()
        Dim session As ISession = NHibernateSessionManager.Instance.GetSessionFrom(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ReslDB))
        Dim sqlQuery As ISQLQuery = session.CreateSQLQuery($"UPDATE Parameter SET LastUsedBillNumber={lastUsedBillNumber}, Locked=0")
        sqlQuery.ExecuteUpdate()
    End Sub

    Sub UpdateLastUsedIdResolution(lastUsedIdResolution As Integer)
        Dim session As ISession = NHibernateSessionManager.Instance.GetSessionFrom(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ReslDB))
        Dim sqlQuery As ISQLQuery = session.CreateSQLQuery($"UPDATE Parameter SET LastUsedIdResolution={lastUsedIdResolution}, Locked=0")
        sqlQuery.ExecuteUpdate()
    End Sub

    Sub UpdateLastIdRole(idRole As Integer, oldIdRole As Integer)
        Dim sqlQuery As ISQLQuery = NHibernateSession.CreateSQLQuery($"UPDATE Parameter SET LastUsedIdRole={idRole} WHERE LastUsedIdRole={oldIdRole}")
        sqlQuery.ExecuteUpdate()
    End Sub

    Sub UpdateLastIdRoleUser(idRoleUser As Integer, oldIdRoleUser As Integer)
        Dim currentParameter As Parameter = GetCurrentAndRefresh()
        Dim sqlQuery As ISQLQuery = NHibernateSession.CreateSQLQuery($"UPDATE Parameter SET LastUsedIdRoleUser = {idRoleUser} WHERE LastUsedIdRoleUser = {oldIdRoleUser}")
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

    Public Sub ResetResolutionNumbers()
        Dim session As ISession = NHibernateSessionManager.Instance.GetSessionFrom(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ReslDB))
        Dim sqlQuery As ISQLQuery = session.CreateSQLQuery($"UPDATE Parameter SET LastUsedResolutionYear = {DateTime.Now.Year}, LastUsedResolutionNumber = 0, LastUsedBillNumber = 0")
        sqlQuery.ExecuteUpdate()
    End Sub

    Public Sub ResetDocumentNumbers()
        Dim session As ISession = NHibernateSessionManager.Instance.GetSessionFrom(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.DocmDB))
        Dim sqlQuery As ISQLQuery = session.CreateSQLQuery($"UPDATE Parameter SET LastUsedYear = {DateTime.Now.Year}, LastUsedNumber = 0")
        sqlQuery.ExecuteUpdate()
    End Sub
End Class
