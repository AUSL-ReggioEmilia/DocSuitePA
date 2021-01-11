Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateRoleUserDao
    Inherits BaseNHibernateDao(Of RoleUser)

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DBName As String)
        MyBase.New(DBName)
    End Sub

#End Region

    ''' <summary> Elenco ruoli/utente del tipo e account specificato. </summary>
    Public Function GetByUserType(ByVal userType As RoleUserType?, ByVal account As String, ByVal onlyEnabled As Boolean, ByVal idRoles As List(Of Integer), ByVal tenantId As Guid?) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        If tenantId.HasValue Then
            criteria.Add(Restrictions.Eq("R.TenantId", tenantId))
        End If
        If userType.HasValue Then
            criteria.Add(Restrictions.Eq("Type", userType.ToString()))
        End If
        criteria.Add(Restrictions.Eq("Account", account))
        If onlyEnabled Then
            criteria.Add(Restrictions.Eq("Enabled", True))
        End If
        If Not idRoles.IsNullOrEmpty() Then
            criteria.Add(Restrictions.In("R.Id", idRoles))
        End If

        Return criteria.List(Of RoleUser)()
    End Function
    Public Function GetSecretaryRolesByAccount(account As String, environment As DSWEnvironment?) As IList(Of Role)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Type", RoleUserType.S.ToString()))
        criteria.Add(Restrictions.Eq("Account", account))
        criteria.Add(Restrictions.Eq("Enabled", True))
        If environment.HasValue Then
            criteria.Add(Restrictions.Eq("DSWEnvironment", environment))
        End If

        criteria.SetProjection(Projections.Property("Role"))
        Return criteria.List(Of Role)()
    End Function

    Public Function GetByType(ByVal type As String, ByVal enabled As Boolean, roleNameFilter As String) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("Type", type))

        If enabled Then criteria.Add(Restrictions.Eq("Enabled", enabled))

        If Not String.IsNullOrWhiteSpace(roleNameFilter) Then
            criteria.Add(Restrictions.Like("R.Name", roleNameFilter, MatchMode.Anywhere))
        End If

        criteria.Add(Restrictions.Eq("R.IsActive", 1S))
        criteria.Add(Restrictions.Eq("R.TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))

        criteria.AddOrder(Order.Asc("R.Name"))
        criteria.AddOrder(Order.Asc("Description"))

        Return criteria.List(Of RoleUser)()
    End Function


    ''' <summary>
    ''' Restituisce l'elenco degli account che sono Direttori o Vice negli uffici in cui l'account connesso è Segretario
    ''' </summary>
    ''' <param name="userConnected">Account connesso</param>
    ''' <param name="onlyEnabled">Indica se deve restituire solamente account attivi</param>
    ''' <returns>Restituisce la lista degli account.</returns>
    Public Function GetAccounts(ByVal userConnected As String, Optional ByVal onlyEnabled As Boolean = False) As IList(Of String)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.In("Type", New String() {RoleUserType.D.ToString(), RoleUserType.V.ToString()}))
        If onlyEnabled Then
            criteria.Add(Restrictions.Eq("Enabled", 1))
        End If

        Dim detach As DetachedCriteria = DetachedCriteria.For(persitentType)
        detach.Add(Restrictions.Eq("Type", RoleUserType.S.ToString()))
        detach.Add(Restrictions.Eq("Account", userConnected))
        detach.SetProjection(Projections.Property("Role.Id"))

        criteria.Add(Subqueries.PropertyIn("Role.Id", detach))
        criteria.SetProjection(Projections.Property("Account"))

        Return criteria.List(Of String)()
    End Function

    ''' <summary> Per WSColl </summary>
    Public Function GetByAccountsAndNotType(ByVal accounts As String, ByVal type As RoleUserType?, ByVal onlyEnabled As Boolean) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Account", accounts))
        criteria.AddOrder(New Order("Description", True))

        If onlyEnabled Then
            criteria.Add(Restrictions.Eq("Enabled", onlyEnabled))
        End If

        If type.HasValue Then
            criteria.Add(Restrictions.Not(Restrictions.Eq("Type", type.ToString())))
        End If
        '' Inserisce prima i contatti con e-mail
        criteria.AddOrder(Order.Desc("Email"))

        Return criteria.List(Of RoleUser)()
    End Function

    Public Function GetByAccount(username As String) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Like("Account", String.Format("\{0}", username), MatchMode.End))

        Return criteria.List(Of RoleUser)()
    End Function

    Public Function GetByRoleId(ByVal roleId As Integer) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Role.Id", roleId))
        criteria.AddOrder(New Order("Description", True))

        Return criteria.List(Of RoleUser)()
    End Function

    Public Function GetByRoleIdAndType(ByVal roleId As Integer, ByVal type As String, onlyEnabled As Boolean?, ByVal mainRoleOnly As Boolean?) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)

        criteria.Add(Restrictions.Eq("R.Id", roleId))
        criteria.Add(Restrictions.Eq("R.TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
        criteria.Add(Restrictions.Eq("Type", type))
        criteria.AddOrder(New Order("Description", True))

        If onlyEnabled.HasValue Then
            criteria.Add(Restrictions.Eq("Enabled", onlyEnabled.Value))
        End If

        If mainRoleOnly.HasValue Then
            criteria.Add(Restrictions.Eq("IsMainRole", mainRoleOnly.Value))
        End If

        Return criteria.List(Of RoleUser)()
    End Function

    Public Function GetByRoleIdAndAccount(ByVal roleId As Integer, ByVal account As String, ByVal type As String) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)

        criteria.Add(Restrictions.Eq("R.Id", roleId))
        criteria.Add(Restrictions.Eq("R.TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
        criteria.Add(Restrictions.Eq("Account", account))
        If Not String.IsNullOrEmpty(type) Then
            criteria.Add(Restrictions.Eq("Type", type))
        End If

        Return criteria.List(Of RoleUser)()
    End Function

    Public Function GetByRoleIdsAndAccount(roleIds As List(Of Integer), account As String, ByVal type As String) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.In("R.Id", roleIds))
        criteria.Add(Restrictions.Eq("R.TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
        criteria.Add(Restrictions.Eq("Account", account))
        If Not String.IsNullOrEmpty(type) Then
            criteria.Add(Restrictions.Eq("Type", type))
        End If

        Return criteria.List(Of RoleUser)()
    End Function

    ''' <summary> Per WSColl </summary>
    Public Function GetByRoleIdAndAccount(ByVal roleId As Integer, ByVal account As String, ByVal onlyEnabled As Boolean) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)

        criteria.Add(Restrictions.Eq("R.Id", roleId))
        criteria.Add(Restrictions.Eq("R.TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
        criteria.Add(Restrictions.Eq("Account", account))

        If onlyEnabled Then
            criteria.Add(Restrictions.Eq("Enabled", onlyEnabled))
        End If

        Return criteria.List(Of RoleUser)()
    End Function

    Public Function IsCurrentUserPrivacyManager(roleIds As Guid()) As Boolean
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Type", RoleUserType.MP.ToString()))
        criteria.Add(Restrictions.Eq("Account", DocSuiteContext.Current.User.FullUserName))
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.In("R.UniqueId", roleIds))
        Return criteria.List(Of RoleUser)().Count > 0
    End Function

    'Restituisce la lista di RoleUser in base ai settori associati alla collaborazione e dove l'utente è manager o vice
    Public Function GetManagersByCollaboration(collaborationId As Integer, account As String) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        Dim detach As DetachedCriteria = DetachedCriteria.For(GetType(CollaborationUser))
        detach.Add(Restrictions.Eq("IdCollaboration", collaborationId))
        detach.Add(Restrictions.Eq("DestinationType", DestinatonType.S.ToString))
        detach.SetProjection(Projections.Property("IdRole"))

        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Subqueries.PropertyIn("R.Id", detach))
        criteria.Add(Restrictions.Not(Restrictions.Eq("Type", RoleUserType.S.ToString())))
        criteria.Add(Restrictions.Eq("R.TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
        If Not String.IsNullOrEmpty(account) Then
            criteria.Add(Restrictions.Eq("Account", account))
        End If

        Return criteria.List(Of RoleUser)()
    End Function

    'Restituisce la lista di RoleUser in base ai settori associati alla pecMailBox e dove l'utente è manager o vice
    Public Function GetCountManagersByPecMailBox(pecMailBoxId As Short, account As String) As Integer
        criteria = NHibernateSession.CreateCriteria(persitentType)
        Dim detach As DetachedCriteria = DetachedCriteria.For(GetType(PECMailBoxRole))
        detach.CreateAlias("PECMailBox", "PMB")
        detach.Add(Restrictions.Eq("PMB.Id", pecMailBoxId))
        detach.SetProjection(Projections.Property("Role"))
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Subqueries.PropertyIn("R.Id", detach))
        criteria.Add(Restrictions.Or(Restrictions.Eq("Type", RoleUserType.D.ToString()), Restrictions.Eq("Type", RoleUserType.V.ToString())))
        criteria.Add(Restrictions.Eq("R.TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
        If Not String.IsNullOrEmpty(account) Then
            criteria.Add(Restrictions.Eq("Account", account))
        End If

        Return criteria.List(Of RoleUser)().Count()
    End Function

    Public Function GetByCollaboration(ByVal collaborationId As Integer, ByVal roleUserType As RoleUserType?, ByVal destinationFirst As Boolean?, ByVal mainRoleOnly As Boolean?) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        '' Sottoquery per ricercare gli IdRole
        Dim detach As DetachedCriteria = DetachedCriteria.For(GetType(CollaborationUser))
        detach.Add(Restrictions.Eq("IdCollaboration", collaborationId))
        detach.Add(Restrictions.Eq("DestinationType", DestinatonType.S.ToString))

        If destinationFirst.HasValue Then
            detach.Add(Restrictions.Eq("DestinationFirst", destinationFirst.Value))
        End If
        detach.SetProjection(Projections.Property("IdRole"))

        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Subqueries.PropertyIn("R.Id", detach))

        If roleUserType.HasValue Then
            criteria.Add(Restrictions.Eq("Type", roleUserType.Value.ToString))
        End If
        If mainRoleOnly.HasValue Then
            criteria.Add(Restrictions.Eq("IsMainRole", mainRoleOnly.Value))
        End If

        Return criteria.List(Of RoleUser)()
    End Function

End Class

