Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform
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
    Public Function GetByUserType(userType As RoleUserType?, account As String, onlyEnabled As Boolean, idRoles As List(Of Integer), idTenantAOO As Guid) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("R.IdTenantAOO", idTenantAOO))
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

    Public Function GetAccountSecretaryRoles(account As String, idTenantAOO As Guid, onlyActive As Boolean, Optional environment As DSWEnvironment? = Nothing) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType, "RU")
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("RU.Account", account))
        criteria.Add(Restrictions.Eq("RU.Enabled", True))
        criteria.Add(Restrictions.In("RU.Type", New List(Of String) From {RoleUserType.D.ToString(), RoleUserType.V.ToString()}))
        criteria.Add(Restrictions.Eq("R.IdTenantAOO", idTenantAOO))
        If onlyActive Then
            criteria.Add(Restrictions.Eq("R.IsActive", True))
        End If

        Dim roleExistCriteria As DetachedCriteria = DetachedCriteria.For(Of RoleUser)("RUU")
        roleExistCriteria.Add(Restrictions.Eq("RUU.Type", RoleUserType.S.ToString()))
        roleExistCriteria.Add(Restrictions.EqProperty("R.Id", "RUU.Role.Id"))
        If environment.HasValue Then
            roleExistCriteria.Add(Restrictions.Eq("RUU.DSWEnvironment", environment.Value))
        End If
        roleExistCriteria.SetProjection(Projections.Constant(1))
        roleExistCriteria.SetMaxResults(1)
        criteria.Add(Subqueries.Exists(roleExistCriteria))

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.GroupProperty("RU.Role"), "Role")
        criteria.SetProjection(Projections.Distinct(proj))
        criteria.SetResultTransformer(Transformers.AliasToBean(Of RoleUser))

        Return criteria.List(Of RoleUser)()
    End Function

    Public Function GetParentWithSecretaryRoles(account As String, idTenantAOO As Guid, Optional baseIdRole As Integer? = Nothing, Optional environment As DSWEnvironment? = Nothing) As IList(Of Role)
        Return NHibernateSession.GetNamedQuery("AllParentWithSecretaryRoles") _
            .SetParameter("enabled", True) _
            .SetParameter("secretaryType", RoleUserType.S.ToString()) _
            .SetParameter("environment", environment) _
            .SetParameter("account", account) _
            .SetParameter("idTenantAOO", idTenantAOO) _
            .SetParameter("directorType", RoleUserType.D.ToString()) _
            .SetParameter("viceType", RoleUserType.V.ToString()) _
            .SetParameter("baseRole", baseIdRole).List(Of Role)

    End Function

    Public Function GetSecretaryRolesByAccount(account As String, environment As DSWEnvironment?, idTenantAOO As Guid) As IList(Of Role)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("Type", RoleUserType.S.ToString()))
        criteria.Add(Restrictions.Eq("Account", account))
        criteria.Add(Restrictions.Eq("Enabled", True))
        criteria.Add(Restrictions.Eq("R.IsActive", True))
        criteria.Add(Restrictions.Eq("R.IdTenantAOO", idTenantAOO))

        If environment.HasValue Then
            criteria.Add(Restrictions.Eq("DSWEnvironment", environment))
        End If
        criteria.SetProjection(Projections.Property("Role"))
        criteria.SetResultTransformer(Transform.Transformers.DistinctRootEntity)
        Return criteria.List(Of Role)()
    End Function

    Public Function GetByType(type As String, enabled As Boolean, roleNameFilter As String, idTenantAOO As Guid) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("Type", type))
        criteria.Add(Restrictions.Eq("R.IsActive", True))
        criteria.Add(Restrictions.Eq("R.IdTenantAOO", idTenantAOO))

        If enabled Then
            criteria.Add(Restrictions.Eq("Enabled", enabled))
        End If

        If Not String.IsNullOrWhiteSpace(roleNameFilter) Then
            criteria.Add(Restrictions.Like("R.Name", roleNameFilter, MatchMode.Anywhere))
        End If

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
    Public Function GetAccounts(userConnected As String, Optional onlyEnabled As Boolean = False) As IList(Of String)
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
    Public Function GetByAccountsAndNotType(accounts As String, type As RoleUserType?, onlyEnabled As Boolean) As IList(Of RoleUser)
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

    Public Function GetByRoleId(roleId As Integer) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Role.Id", roleId))
        criteria.AddOrder(New Order("Description", True))

        Return criteria.List(Of RoleUser)()
    End Function

    Public Function GetByRoleIdAndType(roleId As Integer, type As String, onlyEnabled As Boolean?, mainRoleOnly As Boolean?) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)

        criteria.Add(Restrictions.Eq("R.Id", roleId))
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

    Public Function GetByRoleIdAndAccount(roleId As Integer, account As String, type As String) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)

        criteria.Add(Restrictions.Eq("R.Id", roleId))
        criteria.Add(Restrictions.Eq("Account", account))
        If Not String.IsNullOrEmpty(type) Then
            criteria.Add(Restrictions.Eq("Type", type))
        End If

        Return criteria.List(Of RoleUser)()
    End Function

    Public Function GetByRoleIdsAndAccount(roleIds As List(Of Integer), account As String, type As String) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.In("R.Id", roleIds))
        criteria.Add(Restrictions.Eq("Account", account))
        If Not String.IsNullOrEmpty(type) Then
            criteria.Add(Restrictions.Eq("Type", type))
        End If

        Return criteria.List(Of RoleUser)()
    End Function

    ''' <summary> Per WSColl </summary>
    Public Function GetByRoleIdAndAccount(roleId As Integer, account As String, onlyEnabled As Boolean) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)

        criteria.Add(Restrictions.Eq("R.Id", roleId))
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
    Public Function GetManagersByCollaboration(collaborationId As Integer, account As String, idTenantAOO As Guid) As IList(Of RoleUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        Dim detach As DetachedCriteria = DetachedCriteria.For(GetType(CollaborationUser))
        detach.Add(Restrictions.Eq("IdCollaboration", collaborationId))
        detach.Add(Restrictions.Eq("DestinationType", DestinatonType.S.ToString))
        detach.SetProjection(Projections.Property("IdRole"))

        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("R.IdTenantAOO", idTenantAOO))
        criteria.Add(Subqueries.PropertyIn("R.Id", detach))
        criteria.Add(Restrictions.Not(Restrictions.Eq("Type", RoleUserType.S.ToString())))
        If Not String.IsNullOrEmpty(account) Then
            criteria.Add(Restrictions.Eq("Account", account))
        End If

        Return criteria.List(Of RoleUser)()
    End Function

    'Restituisce la lista di RoleUser in base ai settori associati alla pecMailBox e dove l'utente è manager o vice
    Public Function GetCountManagersByPecMailBox(pecMailBoxId As Short, account As String, idTenantAOO As Guid) As Integer
        criteria = NHibernateSession.CreateCriteria(persitentType)
        Dim detach As DetachedCriteria = DetachedCriteria.For(GetType(PECMailBoxRole))
        detach.CreateAlias("PECMailBox", "PMB")
        detach.Add(Restrictions.Eq("PMB.Id", pecMailBoxId))
        detach.SetProjection(Projections.Property("Role"))
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("R.IdTenantAOO", idTenantAOO))
        criteria.Add(Subqueries.PropertyIn("R.Id", detach))
        criteria.Add(Restrictions.Or(Restrictions.Eq("Type", RoleUserType.D.ToString()), Restrictions.Eq("Type", RoleUserType.V.ToString())))
        If Not String.IsNullOrEmpty(account) Then
            criteria.Add(Restrictions.Eq("Account", account))
        End If

        Return criteria.List(Of RoleUser)().Count()
    End Function

    Public Function GetByCollaboration(collaborationId As Integer, roleUserType As RoleUserType?, destinationFirst As Boolean?, mainRoleOnly As Boolean?, idTenantAOO As Guid) As IList(Of RoleUser)
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
        criteria.Add(Restrictions.Eq("R.IdTenantAOO", idTenantAOO))
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

