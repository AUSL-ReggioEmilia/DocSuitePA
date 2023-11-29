Imports System.Linq
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports System.Text
Imports NHibernate.SqlCommand
Imports VecompSoftware.NHibernateManager.Dao
Imports VecompSoftware.Helpers.NHibernate
Imports NHibernate.Linq
Imports VecompSoftware.NHibernateManager

Public Class NHibernateRoleDao
    Inherits BaseNHibernateDao(Of Role)

#Region " Constructors "

    Public Sub New(sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    Public Function AlreadyExists(name As String, idTenantAOO As Guid) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Role)()
        criteria.Add(Restrictions.Eq("Name", name))
        criteria.Add(Restrictions.Eq("IsActive", True))
        criteria.Add(Restrictions.Eq("IdTenantAOO", idTenantAOO))
        criteria.SetProjection(Projections.Constant(True))

        criteria.SetMaxResults(1)
        Return criteria.List(Of Boolean).Count > 0
    End Function


#End Region

    Public Overrides Sub Save(ByRef entity As Role)
        MyBase.Save(entity)
        NHibernateSession.SessionFactory.Evict(GetType(Role))
    End Sub

    Public Overrides Sub Update(ByRef entity As Role)
        MyBase.Update(entity)
        NHibernateSession.SessionFactory.Evict(GetType(Role))
    End Sub

    Public Overrides Sub UpdateNoLastChange(ByRef entity As Role)
        MyBase.UpdateNoLastChange(entity)
        NHibernateSession.SessionFactory.Evict(GetType(Role))
    End Sub

    Public Overrides Sub UpdateOnly(ByRef entity As Role)
        MyBase.UpdateOnly(entity)
        NHibernateSession.SessionFactory.Evict(GetType(Role))
    End Sub

    Public Overrides Sub Delete(ByRef entity As Role)
        MyBase.Delete(entity)
        NHibernateSession.SessionFactory.Evict(GetType(Role))
    End Sub

    Public Overrides Function GetAll() As IList(Of Role)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Role)()
        criteria.Add(Restrictions.Eq("RoleTypology", RoleTypology.InternalRole))
        Return criteria.List(Of Role)()
    End Function

    Public Function GetByUniqueID(UniqueId As Guid) As Role
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Role)()
        criteria.Add(Restrictions.Eq("UniqueId", UniqueId))
        Return criteria.UniqueResult(Of Role)()
    End Function

    Public Function RoleUsedProtocol(ByRef role As Role) As Boolean
        Dim criteria As ICriteria = NHibernateSessionManager.Instance.GetSessionFrom("ProtDB").CreateCriteria(Of ProtocolRole)()
        criteria.CreateAlias("Role", "R", JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("R.Id", role.Id))
        criteria.SetProjection(Projections.Count("Id"))
        Return criteria.UniqueResult(Of Long)() > 0
    End Function

    Public Function RoleUsedResolution(ByRef role As Role) As Boolean
        Dim criteria As ICriteria = NHibernateSessionManager.Instance.GetSessionFrom("ReslDB").CreateCriteria(Of ResolutionRole)()
        criteria.CreateAlias("Role", "R", JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("R.Id", role.Id))
        criteria.SetProjection(Projections.Count("Id"))
        Return criteria.UniqueResult(Of Long)() > 0
    End Function

    Public Function RoleUsedDocument(ByRef role As Role) As Boolean
        Dim docmDao As New NHibernateDocumentDao("DocmDB")
        Dim isUsed As Boolean = docmDao.GetCountByRole(role) > 0

        Dim docmTokenDao As New NHibernateDocumentTokenDao("DocmDB")
        If Not isUsed AndAlso docmTokenDao.GetCountByRoleSource(role) > 0 Then
            isUsed = True
        End If
        If Not isUsed AndAlso docmTokenDao.GetCountByRoleDestination(role) > 0 Then
            isUsed = True
        End If

        Return isUsed
    End Function

    ''' <summary> Tutti i settori nei quali risulto essere implicitamente o esplicitamente manager di distribuzione. </summary>
    ''' <param name="manageableRolesId"> Id dei settori in cui sono esplicitamente manager </param>
    Public Function GetManageableRoles(manageableRolesId As IList(Of Integer), isActive As Boolean?, idTenantAOO As Guid) As IList(Of Role)
        ConnectionName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(GetType(Role))
        criteria.Add(Restrictions.Eq("IdTenantAOO", idTenantAOO))
        For Each id As Integer In manageableRolesId
            criteria.Add(Restrictions.Like("FullIncrementalPath", id.ToString(), MatchMode.Anywhere))
        Next
        If isActive.HasValue Then
            criteria.Add(Restrictions.Eq("IsActive", isActive.Value))
        End If
        Return criteria.List(Of Role)()
    End Function

    Public Function GetRoleRigths(type As String, groups As String, active As Boolean, rights As String, parentRole As Role, onlyRoot As Boolean, filter As String) As IList(Of Role)
        If Not String.IsNullOrEmpty(type) Then
            type = type.ToUpperInvariant()
        End If

        Dim env As DSWEnvironment
        Select Case type
            Case "DOCM"
                env = DSWEnvironment.Document
            Case "RESL"
                env = DSWEnvironment.Resolution
            Case "SERIES"
                env = DSWEnvironment.DocumentSeries
            Case Else
                env = DSWEnvironment.Protocol
        End Select

        Return GetRoleRigths(env, groups, active, rights, parentRole, onlyRoot, filter)
    End Function

    ''' <summary> Ritira i ruoli. </summary>
    ''' <param name="env">Tipo di ambiente</param>
    ''' <param name="Groups">Gruppi tra i quali cercare i ruoli</param>
    ''' <param name="Active">Indica se filtrare gli attivi</param>
    ''' <param name="Rights">Diritti che deve possedere l'utente</param>
    ''' <param name="parentRole">Eventuale ruolo padre</param>
    ''' <param name="onlyRoot">Indica se ottenere solo i root</param>
    ''' <param name="filter">Filtro sul nome</param>
    Public Function GetRoleRigths(env As DSWEnvironment, groups As String, active As Boolean, rights As String, parentRole As Role, onlyRoot As Boolean, filter As String, idTenantAOO As Guid) As IList(Of Role)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "R")
        criteria.CreateAlias("R.RoleGroups", "RoleGroups", JoinType.LeftOuterJoin)

        Dim fields As String = String.Empty
        Select Case env
            Case DSWEnvironment.Document
                fields = "_documentRights"
                ConnectionName = "DocmDB"
            Case DSWEnvironment.Resolution
                fields = "_resolutionRights"
                ConnectionName = "ReslDB"
            Case DSWEnvironment.Protocol
                fields = "_protocolRights"
                ConnectionName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)
            Case DSWEnvironment.DocumentSeries
                fields = "DocumentSeriesRights"
                ConnectionName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)
        End Select

        'Solo diritti appartenente al gruppo dell'utente collegato alla docsuite
        If Not String.IsNullOrEmpty(groups) Then
            Dim gruppi As String() = groups.Split(","c)
            For i As Integer = 0 To gruppi.Length - 1
                gruppi(i) = gruppi(i).Trim("'"c)
            Next
            criteria.Add(Restrictions.In("RoleGroups.Name", gruppi))
        End If

        criteria.Add(Restrictions.Eq("R.IsActive", active))

        'Il gruppo dell'utente deve possedere diritti 
        If Not String.IsNullOrEmpty(rights) Then
            criteria.Add(Restrictions.IsNotNull("RoleGroups." & fields))
            criteria.Add(Restrictions.Like("RoleGroups." & fields, rights, MatchMode.Start))
        End If

        'Eventuale ruolo padre
        If parentRole IsNot Nothing Then
            criteria.Add(Restrictions.Eq("Father.Id", parentRole.Id))
        ElseIf (onlyRoot) Then
            criteria.Add(Restrictions.IsNull("Father.Id"))
        End If

        ' Filtro per nome
        If Not String.IsNullOrEmpty(filter) Then
            criteria.Add(Restrictions.Like("R.Name", filter, MatchMode.Anywhere))
        End If
        criteria.Add(Restrictions.Eq("R.IdTenantAOO", idTenantAOO))
        criteria.AddOrder(Order.Asc("R.Name"))
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        Return criteria.List(Of Role)()
    End Function

    Private Function CreateGetRoleCriteria(env As DSWEnvironment, rightPosition As Integer?, isActive As Boolean?, name As String, root As Boolean?, parent As Role, getRowCount As Boolean, idTenantAOO As Guid, Optional roleUserType As RoleUserType? = Nothing) As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Role)("R")
        criteria.Add(Restrictions.Eq("R.IdTenantAOO", idTenantAOO))
        criteria.Add(Restrictions.Not(Restrictions.Eq("R.UniqueId", Guid.Empty)))

        If rightPosition.HasValue Then
            criteria.CreateAlias("R.RoleGroups", "RG")
            Dim pattern As String = "1".PadLeft(rightPosition.Value, "_"c)
            Select Case env
                Case DSWEnvironment.Protocol
                    criteria.Add(Restrictions.Like("RG._protocolRights", pattern, MatchMode.Start))
                Case DSWEnvironment.Resolution
                    criteria.Add(Restrictions.Like("RG._resolutionRights", pattern, MatchMode.Start))
                Case DSWEnvironment.Document, DSWEnvironment.Dossier
                    criteria.Add(Restrictions.Like("RG._documentRights", pattern, MatchMode.Start))
                Case DSWEnvironment.DocumentSeries, DSWEnvironment.UDS
                    criteria.Add(Restrictions.Like("RG.DocumentSeriesRights", pattern, MatchMode.Start))
                Case Else
                    Throw New InvalidOperationException("È necessario specificare un DSWEnvironment valido se si valorizza rightPosition.")
            End Select
        End If

        If env = DSWEnvironment.Any Then
            criteria.CreateAlias("R.RoleGroups", "RG")
            Dim dis As New Disjunction()
            dis.Add(Restrictions.Like("RG._protocolRights", 1S.ToString(), MatchMode.Anywhere))
            dis.Add(Restrictions.Like("RG._resolutionRights", 1S.ToString(), MatchMode.Anywhere))
            dis.Add(Restrictions.Like("RG._documentRights", 1S.ToString(), MatchMode.Anywhere))
            dis.Add(Restrictions.Like("RG.DocumentSeriesRights", 1S.ToString(), MatchMode.Anywhere))
            criteria.Add(dis)
        End If

        If isActive.HasValue Then
            criteria.Add(Restrictions.Eq("R.IsActive", isActive.Value))
        End If

        If roleUserType.HasValue Then
            Dim detachedExistsQuery As DetachedCriteria = DetachedCriteria.For(Of RoleUser)("RU")
            detachedExistsQuery.SetProjection(Projections.Property("Role.Id"))
            detachedExistsQuery.Add(Restrictions.Eq("Type", roleUserType.ToString()))
            detachedExistsQuery.Add(Restrictions.EqProperty("R.Id", "RU.Role.Id"))
            criteria.Add(Subqueries.Exists(detachedExistsQuery))
        End If

        ' Filtro per nome
        If Not String.IsNullOrEmpty(name) Then
            criteria.Add(Restrictions.Like("R.Name", name, MatchMode.Anywhere))
        End If

        ' Root
        If root.GetValueOrDefault(False) Then
            criteria.Add(Restrictions.IsNull("R.Father.Id"))
        End If

        ' Root
        If parent IsNot Nothing Then
            criteria.Add(Restrictions.Eq("R.Father.Id", parent.Id))
        End If

        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        If getRowCount Then
            criteria.SetProjection(Projections.RowCount())
        Else
            criteria.AddOrder(Order.Asc("R.Name"))
        End If

        Return criteria
    End Function

    Public Function GetRolesByCategoryFascicleRights(roleIds As IList(Of Integer), env As DSWEnvironment, rightPosition As Integer?, isActive As Boolean?, name As String, root As Boolean?, parent As Role, idTenantAOO As Guid, Optional roleUserType As RoleUserType? = Nothing) As IList(Of Role)
        Dim criteria As ICriteria = CreateGetRoleCriteria(env, rightPosition, isActive, name, root, parent, False, idTenantAOO, roleUserType)
        criteria.Add(Restrictions.In("R.Id", roleIds.ToArray()))
        Return criteria.List(Of Role)()
    End Function

    Public Function GetRolesBySG(idGroupIn As IList(Of Integer), env As DSWEnvironment, rightPosition As Integer?, isActive As Boolean?, name As String, root As Boolean?, parent As Role, idTenantAOO As Guid, Optional roleUserType As RoleUserType? = Nothing) As IList(Of Role)
        Dim criteria As ICriteria = CreateGetRoleCriteria(env, rightPosition, isActive, name, root, parent, False, idTenantAOO, roleUserType)
        criteria.CreateAliasIfNotExists("R.RoleGroups", "RG")
        criteria.Add(Restrictions.In("RG.SecurityGroup.Id", idGroupIn.ToArray()))

        Return criteria.List(Of Role)()
    End Function

    Public Function GetRoles(env As DSWEnvironment, rightPosition As Integer?, isActive As Boolean?, name As String, root As Boolean?, parent As Role, idTenantAOO As Guid) As IList(Of Role)
        Dim criteria As ICriteria = CreateGetRoleCriteria(env, rightPosition, isActive, name, root, parent, False, idTenantAOO)

        Return criteria.List(Of Role)()
    End Function

    ''' <summary> Insieme di settori senza controllo sicurezza </summary>
    ''' <remarks> Ottimizzato per caricare i <see cref="SecurityGroups" />. </remarks>
    Public Function GetNoSecurityRoles(env As DSWEnvironment, name As String, idTenantAOO As Guid, Optional isActive As Boolean? = Nothing = True) As IList(Of Role)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Role)("R")
        criteria.CreateAlias("R.RoleGroups", "RG", JoinType.LeftOuterJoin)
        criteria.Add(Restrictions.Eq("R.IdTenantAOO", idTenantAOO))

        If Not String.IsNullOrEmpty(name) Then
            criteria.Add(Restrictions.Like("R.Name", name, MatchMode.Anywhere))
        End If

        If isActive.HasValue Then
            criteria.Add(Restrictions.Eq("R.IsActive", isActive.Value))
        End If

        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        criteria.AddOrder(Order.Asc("R.Name"))

        Return criteria.List(Of Role)()
    End Function

    Public Function GetRolesCountBySG(idGroupIn As IList(Of Integer), env As DSWEnvironment, rightPosition As Integer?, active As Boolean?, name As String, root As Boolean?, parent As Role, idTenantAOO As Guid, Optional roleUserType As RoleUserType? = Nothing) As Integer
        Dim criteria As ICriteria = CreateGetRoleCriteria(env, rightPosition, active, name, root, parent, True, idTenantAOO, roleUserType)
        criteria.CreateAliasIfNotExists("R.RoleGroups", "RG")
        criteria.Add(Restrictions.In("RG.SecurityGroup.Id", idGroupIn.ToArray()))

        Return criteria.UniqueResult(Of Integer)
    End Function

    Public Function GetRolesCount(env As DSWEnvironment, rightPosition As Integer?, active As Boolean?, name As String, root As Boolean?, parent As Role, idTenantAOO As Guid) As Integer
        Dim criteria As ICriteria = CreateGetRoleCriteria(env, rightPosition, active, name, root, parent, True, idTenantAOO)

        Return criteria.UniqueResult(Of Integer)
    End Function

    Public Function GetUserRights(type As String, role As Role, rights As String) As IList(Of Role)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "R")
        criteria.CreateAlias("R.RoleGroups", "RoleGroups", JoinType.InnerJoin)

        If Not String.IsNullOrEmpty(type) Then
            type = type.ToUpperInvariant()
        End If

        Dim fields As String
        Select Case type
            Case "DOCM"
                fields = "_documentRights"
                ConnectionName = "DocmDB"
            Case "PROT"
                fields = "_protocolRights"
                ConnectionName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)
            Case Else
                fields = "_resolutionRights"
                ConnectionName = "ReslDB"
        End Select

        criteria.Add(Restrictions.Eq("Id", role.Id))
        criteria.Add(Restrictions.Eq("R.IsActive", True))
        'Il gruppo dell'utente deve possedere diritti 
        If Not String.IsNullOrEmpty(rights) Then
            criteria.Add(Restrictions.Not(Restrictions.Eq("RoleGroups." & fields, rights)))
            criteria.Add(Restrictions.IsNotNull("RoleGroups." & fields))
        End If
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        Return criteria.List(Of Role)()
    End Function

    ''' <summary> Restituisce tutti i ruoli di cui il ruolo passato è padre. </summary>
    ''' <param name="parentId">Id settore padre</param>
    ''' <returns>True se il settore è padre di un altro settore, false altrimenti</returns>
    Public Function GetRolesByParentId(parentId As Integer, Optional isActive As Boolean? = Nothing) As IList(Of Role)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "R")
        criteria.Add(Restrictions.Eq("Father.Id", parentId))

        If isActive.HasValue Then
            criteria.Add(Restrictions.Eq("R.IsActive", isActive.Value))
        End If

        criteria.Add(Restrictions.Eq("RoleTypology", RoleTypology.InternalRole))
        criteria.AddOrder(Order.Asc("Name"))
        Return criteria.List(Of Role)()
    End Function

    ''' <summary> Restituisce tutti i ruoli con caselle PEC associate, di cui il ruolo passato sia padre. </summary>
    ''' <param name="ParentId">Id settore padre</param>
    ''' <returns>Lista dei figli</returns>
    Public Function GetRolesWithPECMailboxByParentId(parentId As Integer) As IList(Of Role)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "R")
        criteria.Add(Restrictions.Eq("Father.Id", parentId))
        criteria.Add(Restrictions.Eq("RoleTypology", RoleTypology.InternalRole))

        Dim childRoles As IList(Of Role) = criteria.List(Of Role)()
        Dim pecAssociatedRoles As IList(Of Role) = GetRolesWithPECMailbox()
        Dim survivorRoles As New List(Of Role)
        For Each childRole As Role In childRoles
            For Each pecRole As Role In pecAssociatedRoles
                If pecRole.FullIncrementalPath.IndexOf(childRole.Id.ToString()) <> -1 Then
                    survivorRoles.Add(childRole)
                End If
            Next
        Next

        If survivorRoles.Count > 0 Then
            Return survivorRoles
        End If

        Return Nothing
    End Function

    ''' <summary> Restituisce tutti i ruoli che non hanno alcun padre (Root). </summary>
    Function GetRootRoles(idTenantAOO As Guid, Optional isActive As Boolean? = Nothing) As IList(Of Role)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.IsNull("Father"))
        criteria.Add(Restrictions.Eq("IdTenantAOO", idTenantAOO))

        If isActive.HasValue Then
            criteria.Add(Restrictions.Eq("IsActive", isActive.Value))
        End If
        criteria.Add(Restrictions.Eq("RoleTypology", RoleTypology.InternalRole))
        criteria.AddOrder(Order.Asc("Name"))
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        Return criteria.List(Of Role)()
    End Function

    ''' <summary> Restituisce tutti i ruoli con caselle PEC associate che non hanno alcun padre. </summary>
    Function GetRootRolesWithPECMailbox(idTenantAOO As Guid) As IList(Of Role)
        Dim pecAssociatedRoles As IList(Of Role) = GetRolesWithPECMailbox()

        Dim rootPecIds As New List(Of String)
        For Each pecRole As Role In pecAssociatedRoles
            If pecRole.FullIncrementalPathArray.Length > 0 Then
                rootPecIds.Add(pecRole.FullIncrementalPathArray(0).ToString())
            End If
        Next

        If rootPecIds.Count > 0 Then
            Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Role)()
            criteria.Add(Restrictions.In("Id", rootPecIds.ToArray()))
            criteria.Add(Restrictions.Eq("IdTenantAOO", idTenantAOO))
            criteria.Add(Restrictions.Eq("RoleTypology", RoleTypology.InternalRole))
            criteria.AddOrder(Order.Asc("Name"))
            Return criteria.List(Of Role)()
        End If

        Return Nothing
    End Function

    ''' <summary> Restituisce tutti i ruoli con caselle PEC associate. </summary>
    Function GetRolesWithPECMailbox() As IList(Of Role)
        Dim qry As String = "from Role role where exists elements(role.Mailboxes)"

        Return NHibernateSession.CreateQuery(qry).List(Of Role)()
    End Function

    ''' <summary> Restituisce tutti i ruoli di cui il ruolo passato è padre </summary>
    ''' <param name="Role">Settore padre</param>
    ''' <returns>True se il settore è padre di un altro settore, false altrimenti</returns>
    Public Function GetChildren(role As Role, groups As String, idTenantAOO As Guid) As IList(Of Role)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("RoleGroups", "RoleGroups", JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("IdTenantAOO", idTenantAOO))

        Dim fields As String
        Select Case UCase(ConnectionName)
            Case "DOCMDB"
                fields = "_documentRights"
            Case "RESLDB"
                fields = "_resolutionRights"
            Case Else
                fields = "_protocolRights"
        End Select

        If role IsNot Nothing Then
            criteria.Add(Restrictions.Eq("Father.Id", role.Id))
        Else
            criteria.Add(Restrictions.IsNull("Father.Id"))
        End If

        criteria.Add(Restrictions.Eq("IsActive", True))
        'I gruppi devono possedere i seguenti diritti 
        criteria.Add(Restrictions.Not(Restrictions.Eq("RoleGroups." & fields, "00000000000000000000")))
        'I gruppi devono essere questi
        If Not String.IsNullOrEmpty(groups) Then
            Dim gruppi As String() = groups.Split(","c)
            For i As Integer = 0 To gruppi.Length - 1
                gruppi(i) = gruppi(i).Trim("'"c)
            Next
            criteria.Add(Restrictions.In("RoleGroups.Name", gruppi))
        End If

        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        Return criteria.List(Of Role)()
    End Function

    Function GetUserRoleList(type As String, groups As String, rights As String, Optional onlyActive As Boolean = True) As IList(Of Role)
        Dim filtered As IList(Of Role) = New List(Of Role)
        Dim roles As IList(Of Role) = GetRoleRigths(type, groups, onlyActive, "", Nothing, False, "")
        For Each role As Role In roles
            'Se l'utente è presente in più gruppi risulta una duplicazione del settore per cui mi basta il primo inserimento
            Dim roleFound As Boolean = False
            Dim roleGroups As RoleGroup() = New List(Of RoleGroup)(role.RoleGroups).ToArray()
            Dim i As Integer = 0
            While Not roleFound AndAlso i < roleGroups.Length
                Dim roleGroup As RoleGroup = roleGroups(i)

                Dim fields As String
                Select Case UCase$(ConnectionName)
                    Case "DOCMDB"
                        fields = roleGroup.DocumentRights
                    Case "RESLDB"
                        fields = roleGroup.ResolutionRights
                    Case Else
                        fields = roleGroup.ProtocolRightsString
                End Select
                If (Mid(fields, 1, Len(rights)) = rights) Then
                    filtered.Add(role)
                    roleFound = True
                End If
                i = i + 1
            End While
        Next
        Return filtered
    End Function

    ''' <summary> Costruisce il FullIncrementalPath di una role </summary>
    ''' <param name="role">Role</param>
    ''' <param name="path">variabile che conterrà il FullIncrementalPath</param>
    Public Sub GetFullIncrementalPath(ByRef role As Role, ByRef path As StringBuilder)
        If role Is Nothing Then
            Exit Sub
        End If

        If path.Length <> 0 Then
            path.Insert(0, "|")
        End If
        path.Insert(0, role.Id)

        Dim father As Role = role.Father
        If father IsNot Nothing Then
            GetFullIncrementalPath(father, path)
        End If
    End Sub

    Public Function GetByIds(roleIds As ICollection(Of Integer)) As IList(Of Role)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.In("Id", roleIds.ToList()))
        Return criteria.List(Of Role)()
    End Function

    Public Function GetByServiceCode(serviceCode As String, idTenantAOO As Guid) As IList(Of Role)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("IdTenantAOO", idTenantAOO))
        criteria.Add(Restrictions.Eq("ServiceCode", serviceCode))
        Return criteria.List(Of Role)()
    End Function

    ''' <summary> Verifica se almeno un gruppo ha per un determinato settore determinati permessi. </summary>
    ''' <param name="env"><code>"DOCMDB"</code>, <code>"RESLDB"</code> o il default <code>"PROTDB"</code>.</param>
    ''' <param name="roleId">Id del settore</param>
    ''' <param name="userConnectedGroups">Elenco di gruppi da verificare</param>
    ''' <param name="rights">Permessi da verificare (es. "10", "11")</param>
    Public Function HasRoleRights(env As String, roleId As Integer, userConnectedGroups As String(), rights As String, idTenantAOO As Guid) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(GetType(RoleGroup))
        criteria.SetMaxResults(1)
        criteria.CreateAlias("Role", "R", JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("R.Id", roleId))
        criteria.Add(Restrictions.In("Name", userConnectedGroups))
        criteria.Add(Restrictions.Eq("R.IdTenantAOO", idTenantAOO))

        If Not String.IsNullOrEmpty(ConnectionName) Then
            ConnectionName = ConnectionName.ToUpperInvariant()
        End If

        Dim rightsFieldName As String
        Select Case env
            Case "DOCMDB"
                ConnectionName = "DocmDB"
                rightsFieldName = "_documentRights"
            Case "RESLDB"
                ConnectionName = "ReslDB"
                rightsFieldName = "_resolutionRights"
            Case Else
                ConnectionName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)
                rightsFieldName = "_protocolRights"
        End Select
        criteria.Add(Restrictions.Like(rightsFieldName, rights, MatchMode.Start)) ' Attenzione! Da modificare qualora i permessi dovessero essere più di due.

        Dim retval As IList(Of RoleGroup) = criteria.List(Of RoleGroup)()

        Return retval IsNot Nothing AndAlso retval.Count > 0
    End Function

    ''' <summary>
    ''' Funzione per imperdire la cancellazione di Settori utilizzati nella Collaborazione
    ''' La funzione ritorna True se esiste almeno una collaborazione che fa riferimento al settore in esame, False altrimenti.
    ''' </summary>
    ''' <param name="role">Settore da prendere in esame</param>
    ''' <returns>True se esiste almeno una collaborazione che fa riferimento al settore in esame, False altrimenti</returns>
    Public Function CanDelete(ByRef role As Role) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(GetType(CollaborationUser))
        criteria.Add(Restrictions.Eq("IdRole", Convert.ToInt16(role.Id)))
        Dim number As Integer = criteria.SetProjection(Projections.Count("Account")).UniqueResult(Of Integer)

        Return number = 0
    End Function

    Public Function GetContact(roles As IList(Of Role)) As IList(Of Contact)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(GetType(Contact))
        criteria.Add(Restrictions.In("Role", roles.ToArray()))
        Return criteria.List(Of Contact)()
    End Function

    Public Function GetRolesBySecurityGroups(securityGroups As IList(Of SecurityGroups)) As IList(Of Role)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("RoleGroups", "RG", JoinType.InnerJoin)
        criteria.Add(Restrictions.In("RG.SecurityGroup.Id", securityGroups.Select(Function(f) f.Id).ToArray()))
        criteria.AddOrder(Order.Asc("Name"))
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        Return criteria.List(Of Role)()
    End Function

    Public Function CheckCurrentUserPrivacyRoles(roleIds As Guid(), privacyRoleIds As Guid(), env As Integer) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "R")
        criteria.CreateAlias("R.RoleGroups", "RG", JoinType.InnerJoin)
        Dim disj As Disjunction = New Disjunction()
        If roleIds.Count > 0 Then
            Dim detachedPublicQuery As DetachedCriteria = DetachedCriteria.For(Of RoleGroup)("RG")
            detachedPublicQuery.SetProjection(Projections.Property("RG.Id"))
            detachedPublicQuery.CreateAlias("RG.Role", "RGG", JoinType.InnerJoin)
            detachedPublicQuery.Add(Restrictions.EqProperty("RGG.Id", "R.Id"))
            detachedPublicQuery.Add(Restrictions.In("RGG.UniqueId", roleIds))
            Select Case env
                Case DSWEnvironment.Protocol
                    criteria.Add(Restrictions.Like("RG._protocolRights", "1", MatchMode.Start))
                Case Else
                    Throw New InvalidOperationException("È necessario specificare un DSWEnvironment valido se si valorizza containerRight.")
            End Select

            detachedPublicQuery.CreateAlias("RG.SecurityGroup", "SG", JoinType.InnerJoin)
            detachedPublicQuery.CreateAlias("SG.SecurityUsers", "SU", JoinType.InnerJoin)
            detachedPublicQuery.Add(Restrictions.Eq("SU.UserDomain", DocSuiteContext.Current.User.Domain))
            detachedPublicQuery.Add(Restrictions.Eq("SU.Account", DocSuiteContext.Current.User.UserName))
            disj.Add(Subqueries.Exists(detachedPublicQuery))
        End If
        If privacyRoleIds.Count > 0 Then
            Dim detachedPrivacyQuery As DetachedCriteria = DetachedCriteria.For(Of RoleUser)("RU")
            detachedPrivacyQuery.SetProjection(Projections.Property("RU.Id"))
            detachedPrivacyQuery.CreateAlias("RU.Role", "RUU", JoinType.InnerJoin)
            detachedPrivacyQuery.Add(Restrictions.EqProperty("RUU.Id", "R.Id"))
            detachedPrivacyQuery.Add(Restrictions.In("RUU.UniqueId", privacyRoleIds))
            detachedPrivacyQuery.Add(Restrictions.Eq("RU.Type", RoleUserType.MP.ToString()))
            detachedPrivacyQuery.Add(Restrictions.Eq("RU.Account", DocSuiteContext.Current.User.FullUserName))
            disj.Add(Subqueries.Exists(detachedPrivacyQuery))
        End If
        criteria.Add(disj)
        criteria.SetProjection(Projections.RowCountInt64())
        Return criteria.UniqueResult(Of Long)() > 0
    End Function

    Public Function GetPrivacyUserRoles(privacyRoleIds As Guid()) As IList(Of Role)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.In("UniqueId", privacyRoleIds))
        criteria.CreateAlias("RoleUsers", "RU", JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("RU.Account", DocSuiteContext.Current.User.FullUserName))
        criteria.Add(Restrictions.Eq("RU.Type", RoleUserType.MP.ToString()))
        criteria.AddOrder(Order.Asc("Name"))
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        Return criteria.List(Of Role)()
    End Function

    Public Function GetUserRolesByIds(roleIds As Guid(), env As DSWEnvironment, right As Integer?) As IList(Of Role)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.In("UniqueId", roleIds))
        criteria.CreateAlias("RoleGroups", "RG", JoinType.InnerJoin)
        criteria.CreateAlias("RG.SecurityGroup", "SG", JoinType.InnerJoin)
        criteria.CreateAlias("SG.SecurityUsers", "SU", JoinType.InnerJoin)
        If right.HasValue Then
            Dim pattern As String = "1".PadLeft(right.Value, "_"c)
            Select Case env
                Case DSWEnvironment.Protocol
                    criteria.Add(Restrictions.Like("RG._protocolRights", pattern, MatchMode.Start))
                Case DSWEnvironment.Resolution
                    criteria.Add(Restrictions.Like("RG._resolutionRights", pattern, MatchMode.Start))
                Case DSWEnvironment.Document, DSWEnvironment.Dossier
                    criteria.Add(Restrictions.Like("RG._documentRights", pattern, MatchMode.Start))
                Case DSWEnvironment.DocumentSeries, DSWEnvironment.UDS
                    criteria.Add(Restrictions.Like("RG.DocumentSeriesRights", pattern, MatchMode.Start))
                Case Else
                    Throw New InvalidOperationException("È necessario specificare un DSWEnvironment valido se si valorizza rightPosition.")
            End Select
        End If

        criteria.Add(Restrictions.Eq("SU.Account", DocSuiteContext.Current.User.UserName))
        criteria.Add(Restrictions.Eq("SU.UserDomain", DocSuiteContext.Current.User.Domain))
        criteria.AddOrder(Order.Asc("Name"))
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        Return criteria.List(Of Role)()
    End Function

    Public Sub BatchChangeRoleActiveState(role As Role, isActive As Boolean, Optional recursiveChildren As Boolean = False)
        NHibernateSession.Query(Of Role)() _
                        .Where(Function(x) x.Id = role.Id) _
                        .UpdateBuilder() _
                        .Set(Function(p) p.IsActive, isActive) _
                        .Update()

        If recursiveChildren Then
            NHibernateSession.Query(Of Role)() _
                        .Where(Function(x) x.FullIncrementalPath.StartsWith(String.Concat(role.FullIncrementalPath, "|"))) _
                        .UpdateBuilder() _
                        .Set(Function(p) p.IsActive, isActive) _
                        .Update()
        End If
    End Sub

End Class