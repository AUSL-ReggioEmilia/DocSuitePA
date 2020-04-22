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

    ''' <summary> Restituisce tutti i gruppi presenti nelle tabelle RoleGroup</summary>
    Public Function GetAllGroups() As IList(Of String)
        Dim returns As List(Of String) = New List(Of String)
        Dim containerGroupDao As NHibernateContainerGroupDao = New NHibernateContainerGroupDao(SessionFactoryName)
        Dim roleGroupDao As NHibernateRoleGroupDao = New NHibernateRoleGroupDao(SessionFactoryName)
        returns.AddRange(containerGroupDao.GetAll().Select(Function(f) f.Name))
        returns.AddRange(roleGroupDao.GetAll().Select(Function(f) f.Name))
        Return returns.Distinct().ToList()
    End Function

    Private Sub MergeOnSession(ByVal tableName As String, ByVal sessionName As String)
        Dim transaction As ITransaction = Nothing
        Dim sqlStatement As String = "UPDATE " & tableName & " SET idGroup = SG.idGroup" & _
                              " FROM " & tableName & " AS DEST" & _
                              " INNER JOIN SecurityGroups as SG ON SG.GroupName = DEST.GroupName"

        SessionFactoryName = sessionName
        transaction = NHibernateSession.BeginTransaction()
        NHibernateHelper.ExecuteNonQuery(sqlStatement, NHibernateSession.Connection, transaction)
    End Sub

    ''' <summary> Collega i gruppi nella SecurityGroups con i gruppi nella tabella passata come parametro. </summary>
    ''' <param name="tableName">Nome della tabella che gestisce gruppi di dominio</param>
    Public Sub Merge(ByVal tableName As String)
        If DocSuiteContext.Current.IsProtocolEnabled Then
            MergeOnSession(tableName, System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
        End If
        If DocSuiteContext.Current.IsDocumentEnabled Then
            MergeOnSession(tableName, "DocmDB")
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            MergeOnSession(tableName, "ReslDB")
        End If
    End Sub

    ''' <summary> Restituisce tutti i gruppi ROOT (senza padre). </summary>
    Public Function GetRootGroups() As IList(Of SecurityGroups)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.IsNull("Parent"))
        criteria.AddOrder(Order.Asc("GroupName"))
        criteria.Add(Restrictions.Eq("TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))

        Return criteria.List(Of SecurityGroups)()
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
