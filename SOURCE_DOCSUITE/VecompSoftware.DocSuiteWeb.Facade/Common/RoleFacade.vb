Imports System.Linq
Imports VecompSoftware.Core.Command
Imports VecompSoftware.Core.Command.CQRS.Commands.Entities.Commons
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Command.CQRS.Commands.Entities.Commons
Imports VecompSoftware.Services.Logging
Imports APICommons = VecompSoftware.DocSuiteWeb.Entity.Commons

<ComponentModel.DataObject()>
Public Class RoleFacade
    Inherits CommonFacade(Of Role, Integer, NHibernateRoleDao)
#Region " Fields "
    Private _defaultAllUserRole As Role
    Private _commandInsertFacade As CommandFacade(Of ICommandCreateRole)
    Private _commandUpdateFacade As CommandFacade(Of ICommandUpdateRole)

#End Region

#Region " Properties "

    Public ReadOnly Property DefaultAllUserRole As Role
        Get
            If _defaultAllUserRole Is Nothing Then
                _defaultAllUserRole = GetByUniqueID(Guid.Empty)
            End If
            Return _defaultAllUserRole
        End Get
    End Property

    Private ReadOnly Property CommandInsertFacade As CommandFacade(Of ICommandCreateRole)
        Get
            If _commandInsertFacade Is Nothing Then
                _commandInsertFacade = New CommandFacade(Of ICommandCreateRole)()
            End If
            Return _commandInsertFacade
        End Get
    End Property

    Private ReadOnly Property CommandUpdateFacade As CommandFacade(Of ICommandUpdateRole)
        Get
            If _commandUpdateFacade Is Nothing Then
                _commandUpdateFacade = New CommandFacade(Of ICommandUpdateRole)()
            End If
            Return _commandUpdateFacade
        End Get
    End Property
#End Region

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal dbName As String)
        MyBase.New(dbName)
    End Sub

#End Region

#Region " Methods "

    Public Function AlreadyExists(name As String, idTenantAOO As Guid) As Boolean
        Return Me._dao.AlreadyExists(name, idTenantAOO)
    End Function

    Public Function SendUpdateRoleCommand(role As Role) As Guid?
        Try
            Dim identity As IdentityContext = New IdentityContext(DocSuiteContext.Current.User.FullUserName)
            Dim roleEntityMapper As MapperRoleEntity = New MapperRoleEntity()
            Dim apiRoleEntity As APICommons.Role = roleEntityMapper.MappingDTO(role)

            Dim commandUpdateRole As ICommandUpdateRole = New CommandUpdateRole(CurrentTenant.TenantName, CurrentTenant.UniqueId, CurrentTenant.TenantAOO.UniqueId, identity, apiRoleEntity)
            CommandUpdateFacade.Push(commandUpdateRole)
            Return commandUpdateRole.Id
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Concat("SendUpdateRoleCommand => ", ex.Message), ex)
        End Try
        Return Nothing
    End Function

    Public Function SendCreateRoleCommand(role As Role) As Guid?
        Try
            Dim identity As IdentityContext = New IdentityContext(DocSuiteContext.Current.User.FullUserName)
            Dim roleEntityMapper As MapperRoleEntity = New MapperRoleEntity()
            Dim apiRoleEntity As APICommons.Role = roleEntityMapper.MappingDTO(role)

            Dim commandCreateRole As ICommandCreateRole = New CommandCreateRole(CurrentTenant.TenantName, CurrentTenant.UniqueId, CurrentTenant.TenantAOO.UniqueId, identity, apiRoleEntity)
            CommandInsertFacade.Push(commandCreateRole)
            Return commandCreateRole.Id
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Concat("SendCreateRoleCommand => ", ex.Message), ex)
        End Try
        Return Nothing
    End Function

#End Region
    Public Sub Move(toMoveRole As Role, destinationRole As Role)

        If destinationRole IsNot Nothing Then
            destinationRole.Children.Add(toMoveRole)
        End If

        toMoveRole.Father = destinationRole
        CalculateFullIncremental(toMoveRole)

        If destinationRole IsNot Nothing Then
            Me.UpdateOnly(destinationRole)
        End If

        Me.UpdateOnly(toMoveRole)
        toMoveRole = GetById(toMoveRole.Id)
        CalculateFullIncremental(toMoveRole.Children)
    End Sub

    Private Sub CalculateFullIncremental(ByRef role As Role)
        If role.Father Is Nothing Then
            role.FullIncrementalPath = role.Id.ToString()
        Else
            role.FullIncrementalPath = String.Format("{0}|{1}", role.Father.FullIncrementalPath, role.Id.ToString())
        End If
    End Sub
    Private Sub CalculateFullIncremental(ByRef childs As IList(Of Role))
        If (childs Is Nothing) Then
            Return
        End If
        For Each role As Role In childs
            Dim obj As Role = GetById(role.Id, False)
            CalculateFullIncremental(obj)
            Me.UpdateOnly(obj)
            obj = GetById(role.Id, False)
            CalculateFullIncremental(obj.Children)
        Next
    End Sub

    Public Overrides Sub Save(ByRef role As Role)
        'Recupero ID dalla tabella parameter
        Dim pf As New ParameterFacade(_dbName)
        Dim parameter As Parameter = pf.GetAll()(0)
        role.Id = parameter.LastUsedIdRole + 1
        role.IsActive = True
        If (pf.UpdateReplicateLastIdRole(parameter.LastUsedIdRole + 1, parameter.LastUsedIdRole)) Then
            CalculateFullIncremental(role)
            MyBase.Save(role)
        End If
    End Sub

    Public Overrides Function GetAll() As IList(Of Role)
        Return _dao.GetAll()
    End Function

    Public Overrides Function GetById(id As Integer) As Role
        Return _dao.GetById(id)
    End Function

    Public Function GetByUniqueID(UniqueID As Guid) As Role
        Return _dao.GetByUniqueID(UniqueID)
    End Function

    Public Overrides Function IsUsed(ByRef obj As Role) As Boolean
        If _dao.RoleUsedProtocol(obj) Then
            Return True
        End If
        If DocSuiteContext.Current.IsDocumentEnabled Then
            If _dao.RoleUsedDocument(obj) Then
                Return True
            End If
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            If _dao.RoleUsedResolution(obj) Then
                Return True
            End If
        End If

        Return False
    End Function

    Public Function GetManageableRoles(manageableRoleIds As IList(Of Integer), isActive As Boolean?, idTenantAOO As Guid) As IList(Of Role)
        Return _dao.GetManageableRoles(manageableRoleIds, isActive, idTenantAOO)
    End Function

    Public Function GetUserRights(ByVal type As String, ByVal role As Role, ByVal rights As String) As IList(Of Role)
        Return _dao.GetUserRights(type, role, rights)
    End Function

    ''' <summary> Restituisce tutti i ruoli di cui il ruolo passato è padre. </summary>
    ''' <param name="parentId">Id settore padre.</param>
    ''' <param name="onlyWithPECAddress">Se includere solo i settori con indirizzo PEC associato.</param>
    ''' <returns>True se il settore è padre di un altro settore, false altrimenti.</returns>
    Public Function GetItemsByParentId(ByVal parentId As Integer, Optional ByVal onlyWithPECAddress As Boolean = False, Optional ByVal isActive As Boolean? = Nothing) As IList(Of Role)
        If onlyWithPECAddress Then
            Return _dao.GetRolesWithPECMailboxByParentId(parentId)
        Else
            Return _dao.GetRolesByParentId(parentId, isActive)
        End If
    End Function

    ''' <summary> Restituisce tutti i ruoli che non hanno alcun padre. </summary>
    ''' <param name="isActive">Se solo quelli attivi</param>
    ''' <param name="withPECMailbox">Se includere solo i settori con indirizzo PEC associato.</param>
    Public Function GetRootItems(idTenantAOO As Guid, Optional isActive As Boolean? = Nothing, Optional withPECMailbox As Boolean = False) As IList(Of Role)
        If withPECMailbox Then
            Return _dao.GetRootRolesWithPECMailbox(idTenantAOO)
        Else
            Return _dao.GetRootRoles(idTenantAOO, isActive)
        End If
    End Function

    ''' <summary>
    ''' Restituisce tutti i ruoli di cui il ruolo passato è padare
    ''' </summary>
    ''' <param name="Role">Settore padre</param>
    ''' <param name="Groups">Lista di gruppi che devono appartenere ai settori</param>
    ''' <returns>True se il settore è padre di un altro settore, false altrimenti</returns>
    Public Function GetChildren(ByVal role As Role, ByVal groups As String, idTenantAOO As Guid) As IList(Of Role)
        Return _dao.GetChildren(role, groups, idTenantAOO)
    End Function


    Public Function GetByIds(ByVal roleIds As ICollection(Of Integer)) As IList(Of Role)
        Return _dao.GetByIds(roleIds)
    End Function

    Public Function GetByServiceCode(ByVal serviceCode As String, idTenantAOO As Guid, roleIds As IList(Of Integer), Optional roleUserType As RoleUserType? = Nothing, Optional env As DSWEnvironment? = Nothing) As Role
        Dim roles As IList(Of Role) = _dao.GetByServiceCode(serviceCode, idTenantAOO)
        If roles.IsNullOrEmpty() Then
            Return Nothing
        End If

        If roleIds IsNot Nothing AndAlso env.HasValue Then
            Dim userRoles As IList(Of Role) = GetUserRolesByCategory(env.Value, roleIds, Nothing, Nothing, Nothing, False, idTenantAOO, roleUserType)
            If Not userRoles.Contains(roles(0)) Then
                Return Nothing
            End If
        End If

        Return roles(0)
    End Function

    ''' <summary> Verifica che l'operatore collegato appartenga al settore passato per parametro. </summary>
    ''' <param name="env">Ambiente di riferimento</param>
    ''' <param name="role">Settore che a cui si vuole verificare l'appartenenza</param>
    ''' <returns>Restituisce true se l'operatore appartiene al Settore</returns>
    ''' <remarks>Gestisce internamente anche i Security Groups</remarks>
    Public Function CurrentUserBelongsToRoles(env As DSWEnvironment, role As Role) As Boolean
        Dim tor As Boolean = CurrentUserBelongsToRoles(env, New List(Of Role) From {role})
        Return tor
    End Function

    ''' <summary> Verifica che l'operatore collegato appartenga al settore passato per parametro. </summary>
    ''' <param name="env"> Ambiente di riferimento </param>
    ''' <param name="roles"> Settore che a cui si vuole verificare l'appartenenza </param>
    ''' <returns> Restituisce true se l'operatore appartiene al Settore </returns>
    ''' <remarks> Gestisce internamente anche i Security Groups </remarks>
    Public Function CurrentUserBelongsToRoles(env As DSWEnvironment, roles As IList(Of Role)) As Boolean
        If roles.IsNullOrEmpty() Then
            Return False
        End If

        Dim idTenantAOO As Guid = roles.First().IdTenantAOO
        Dim targetRoles As IList(Of Role) = GetUserRoles(env, 1, True, idTenantAOO)

        Dim belongs As Boolean = False
        If Not targetRoles.IsNullOrEmpty() Then
            belongs = roles.Any(Function(role) targetRoles.Contains(role))
        End If
        Return belongs
    End Function

    ''' <summary> Verifica se il settore è in uno dei rami dei settori padri </summary>
    ''' <param name="parentRoles">Settori padri</param>
    ''' <param name="role">Settore da cercare</param>
    ''' <returns>True se il settore è in uno dei sottorami dei settori padri</returns>
    ''' <remarks>Ritorna true anche se Role è uno dei settori padri</remarks>
    Public Function HierarchicalCheck(ByVal parentRoles As IList(Of Role), role As Role) As Boolean
        For Each parentRole As Role In parentRoles
            If role.FullIncrementalPath.Contains(parentRole.Id.ToString()) OrElse role.Id = parentRole.Id Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Verifica se il settore passato è tra i settori dell'operatore corrente o in uno dei relativi sotto-settori
    ''' </summary>
    ''' <param name="env">Ambiente di verifica</param>
    ''' <param name="role">Settore da verificare</param>
    Public Function CurrentUserHierarchicalCheck(env As DSWEnvironment, role As Role, idTenantAOO As Guid) As Boolean
        Dim tor As Boolean = HierarchicalCheck(GetUserRoles(env, 1, True, idTenantAOO), role)
        Return tor
    End Function

    ''' <summary>
    ''' Verifica se il settore è uno dei rami dei settori figli
    ''' </summary>
    ''' <param name="parentRoles">Settori figli</param>
    ''' <param name="role">Settore da verificare</param>
    ''' <returns>Ritorna true se il settore è uno dei settori figli o è lo stesso settore</returns>
    Public Function IsRoleChildCheck(parentRoles As IList(Of Role), role As Role) As Boolean
        For Each parentRole As Role In parentRoles
            If parentRole.FullIncrementalPath.Contains(role.Id.ToString()) OrElse parentRole.Id = role.Id Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Verifica se il settore è uno dei rami dei settori figli dell'utente corrente
    ''' </summary>
    Public Function CurrentUserIsRoleChildCheck(env As DSWEnvironment, role As Role) As Boolean
        Return Me.IsRoleChildCheck(GetUserRoles(env, 1, True, role.IdTenantAOO), role)
    End Function

    Public Function UserBelongsToRoles(domain As String, username As String, env As DSWEnvironment, ByVal roles As IList(Of Role)) As Boolean
        Dim idTenantAOO As Guid = roles.First().IdTenantAOO
        Dim targetRoles As IList(Of Role) = GetUserRoles(domain, username, env, 1, True, idTenantAOO)
        Return roles.Any(Function(role) targetRoles.Contains(role))
    End Function

    ''' <summary> Verifica se l'utente corrente è manager di distribuzione di un determinato settore. </summary>
    ''' <param name="idRole">Id del settore da verificare</param>
    Public Function IsRoleDistributionManager(ByVal idRole As Integer, idTenantAOO As Guid) As Boolean
        Return _dao.HasRoleRights("ProtDB", idRole, CommonShared.GetArrayUserFromString, "11", idTenantAOO)
    End Function

    ''' <summary>
    ''' Funzione per imperdire la cancellazione di Settori utilizzati nella Collaborazione
    ''' La funzione ritorna True se esiste almeno una collaborazione che fa riferimento al settore in esame, False altrimenti.
    ''' </summary>
    ''' <param name="role">Settore da prendere in esame</param>
    ''' <returns>True se esiste almeno una collaborazione che fa riferimento al settore in esame, False altrimenti</returns>
    Public Function CanDelete(ByRef role As Role) As Boolean
        Return _dao.CanDelete(role)
    End Function

    ''' <summary>
    ''' Restituisce l'elenco dei Settori a cui appartiene l'utente corrente
    ''' </summary>
    ''' <param name="env">Ambiente di riferimento</param>
    ''' <param name="rightPosition">Diritto necessario per includere il settore</param>
    ''' <param name="active">Indica se restituire solo Settori Attivi (true), sono Disattivati (false) o entrambi (NULL)</param>
    ''' <returns>Lista dei Settori che soddisfano i filtri</returns>
    ''' <remarks>Lavora sia per AD che per SG</remarks>
    Public Function GetUserRoles(env As DSWEnvironment, rightPosition As Integer?, active As Boolean?, idTenantAOO As Guid) As IList(Of Role)
        Return GetUserRoles(env, rightPosition, active, Nothing, Nothing, Nothing, idTenantAOO)
    End Function

    ''' <summary>
    ''' Restituisce il numero dei Settori a cui appartiene l'utente corrente
    ''' </summary>
    ''' <param name="env">Ambiente di riferimento</param>
    ''' <param name="rightPosition">Diritto necessario per includere il settore</param>
    ''' <param name="active">Indica se restituire solo Settori Attivi (true), sono Disattivati (false) o entrambi (NULL)</param>
    ''' <returns>Lista dei Settori che soddisfano i filtri</returns>
    ''' <remarks>Lavora sia per AD che per SG</remarks>
    Public Function GetUserRolesCount(env As DSWEnvironment, rightPosition As Integer?, active As Boolean?, idTenantAOO As Guid) As Integer
        Return GetUserRolesCount(env, rightPosition, active, Nothing, Nothing, Nothing, idTenantAOO)
    End Function


    ''' <summary>
    ''' Restituisce l'elenco dei Settori a cui appartiene l'utente specificato
    ''' </summary>
    Public Function GetRolesFromUserName(env As DSWEnvironment, userName As String, idTenantAOO As Guid) As IList(Of Role)
        Dim account As String = userName
        Dim domain As String = CommonShared.UserDomain

        Dim values As String() = userName.Split("\"c)
        If values.Length > 1 Then
            domain = values.First()
            account = values.Last()
        End If
        Dim fullUserName As String = String.Concat(domain, "\", account)

        Dim groups As IList(Of SecurityGroups) = Factory.SecurityUsersFacade.GetGroupsByAccount(fullUserName)
        If groups.IsNullOrEmpty() Then
            Return Nothing
        End If
        Return _dao.GetRolesBySG(groups.Select(Function(g) g.Id).ToList(), env, 1, True, String.Empty, Nothing, Nothing, idTenantAOO)

    End Function

    Public Function GetUserRolesCount(env As DSWEnvironment, rightPosition As Integer?, isActive As Boolean?, name As String, root As Boolean?, parent As Role, idTenantAOO As Guid) As Integer
        Dim groups As IList(Of SecurityGroups) = Factory.SecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName)
        If groups.IsNullOrEmpty() Then
            Return Nothing
        End If
        Return _dao.GetRolesCountBySG(groups.Select(Function(g) g.Id).ToList(), env, rightPosition, isActive, name, root, parent, idTenantAOO)
    End Function

    ''' <summary>
    ''' Restituisce l'elenco dei Settori a cui appartiene l'utente corrente
    ''' </summary>
    ''' <param name="env">Ambiente di riferimento</param>
    ''' <param name="rightPosition">Diritto necessario per includere il settore</param>
    ''' <param name="isActive">Indica se restituire solo Settori Attivi (true), sono Disattivati (false) o entrambi (NULL)</param>
    ''' <param name="name">String per il filtro per nome del Settore.</param>
    ''' <param name="root">Indica se si vogliono solo Settori radice.</param>
    ''' <returns>Lista dei Settori che soddisfano i filtri</returns>
    ''' <remarks>Lavora sia per AD che per SG</remarks>
    Public Function GetUserRoles(env As DSWEnvironment, rightPosition As Integer?, isActive As Boolean?, name As String, root As Boolean?, parent As Role, idTenantAOO As Guid) As IList(Of Role)
        Dim groups As IList(Of SecurityGroups) = Factory.SecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName)
        If groups.IsNullOrEmpty() Then
            Return Nothing
        End If
        Return _dao.GetRolesBySG(groups.Select(Function(g) g.Id).ToList(), env, rightPosition, isActive, name, root, parent, idTenantAOO)
    End Function

    ''' <summary>
    ''' Restituisce l'elenco dei Settori a cui appartiene l'utente passato per parametro
    ''' </summary>
    ''' <param name="domain">Dominio di appartenenza dell'utente</param>
    ''' <param name="userName">Username dell'utente</param>
    ''' <param name="env">Ambiente di riferimento</param>
    ''' <param name="rightPosition">Diritto necessario per includere il settore</param>
    ''' <param name="active">Indica se restituire solo Settori Attivi (true), sono Disattivati (false) o entrambi (NULL)</param>
    ''' <returns>Lista dei Settori che soddisfano i filtri</returns>
    ''' <remarks>Lavora sia per AD che per SG</remarks>
    Public Function GetUserRoles(domain As String, userName As String, env As DSWEnvironment, rightPosition As Integer?, active As Boolean?, idTenantAOO As Guid) As IList(Of Role)
        Return GetUserRoles(domain, userName, env, rightPosition, active, Nothing, Nothing, Nothing, idTenantAOO)
    End Function

    ''' <summary>
    ''' Restituisce l'elenco dei Settori a cui appartiene l'utente passato per parametro
    ''' </summary>
    ''' <param name="domain">Dominio di appartenenza dell'utente</param>
    ''' <param name="userName">Username dell'utente</param>
    ''' <param name="env">Ambiente di riferimento</param>
    ''' <param name="rightPosition">Diritto necessario per includere il settore</param>
    ''' <param name="active">Indica se restituire solo Settori Attivi (true), sono Disattivati (false) o entrambi (NULL)</param>
    ''' <param name="name">String per il filtro per nome del Settore.</param>
    ''' <param name="root">Indica se si vogliono solo Settori radice.</param>
    ''' <returns>Lista dei Settori che soddisfano i filtri</returns>
    ''' <remarks>Lavora sia per AD che per SG</remarks>
    Public Function GetUserRoles(domain As String, userName As String, env As DSWEnvironment, rightPosition As Integer?, active As Boolean?, name As String, root As Boolean?, parent As Role, idTenantAOO As Guid) As IList(Of Role)
        Dim groups As IList(Of SecurityGroups) = Factory.SecurityUsersFacade.GetGroupsByAccount(userName)
        If groups.IsNullOrEmpty() Then
            Return Nothing
        End If
        Return _dao.GetRolesBySG(groups.Select(Function(g) g.Id).ToList(), env, rightPosition, active, name, root, parent, idTenantAOO)
    End Function

    Public Function GetRolesBySG(env As DSWEnvironment, groups As IList(Of SecurityGroups), rightPosition As Integer?, active As Boolean?, name As String, root As Boolean?, idTenantAOO As Guid) As IList(Of Role)
        Return _dao.GetRolesBySG(groups.Select(Function(g) g.Id).ToList(), env, rightPosition, active, name, root, Nothing, idTenantAOO)
    End Function

    Public Function GetRoles(env As DSWEnvironment, rightPosition As Integer?, isActive As Boolean?, name As String, root As Boolean?, parent As Role, idTenantAOO As Guid) As IList(Of Role)
        Return _dao.GetRoles(env, rightPosition, isActive, name, root, parent, idTenantAOO)
    End Function

    ''' <summary> Insieme di settori senza controllo sicurezza </summary>
    ''' <remarks> Ottimizzato per caricare i <see cref="SecurityGroups" />. </remarks>
    Public Function GetNoSecurityRoles(env As DSWEnvironment, name As String, idTenantAOO As Guid, Optional isActive As Boolean? = Nothing) As IList(Of Role)
        Return _dao.GetNoSecurityRoles(env, name, idTenantAOO, isActive)
    End Function

    Public Function GetRolesCount(env As DSWEnvironment, rightPosition As Integer?, active As Boolean?, name As String, root As Boolean?, parent As Role, idTenantAOO As Guid) As Integer
        Return _dao.GetRolesCount(env, rightPosition, active, name, root, parent, idTenantAOO)
    End Function

    Public Function GetCurrentUserRoles(env As DSWEnvironment, idTenantAOO As Guid) As Dictionary(Of ProtocolRoleRightPositions, IList(Of Integer))
        Return GetCurrentUserRoles(env, Nothing, idTenantAOO)
    End Function

    Public Function GetCurrentUserRoles(env As DSWEnvironment, active As Boolean?, idTenantAOO As Guid) As Dictionary(Of ProtocolRoleRightPositions, IList(Of Integer))
        Dim tor As New Dictionary(Of ProtocolRoleRightPositions, IList(Of Integer))
        For Each val As ProtocolRoleRightPositions In [Enum].GetValues(GetType(ProtocolRoleRightPositions))
            Dim temp As IList(Of Integer) = GetUserRoles(env, val, active, idTenantAOO).Select(Function(r) r.Id).ToList()
            If Not temp.IsNullOrEmpty() Then
                tor.Add(val, temp)
            End If
        Next
        Return tor
    End Function

    ''' <summary> Stringa con email valide appartenenti ai settori. </summary>
    ''' <param name="roles">Elenco di settori da cui estrarre gli indirizzi email.</param>
    Public Shared Function GetEmailAddresses(roles As IList(Of Role)) As String
        Dim available As IEnumerable(Of Role) = roles.Where(Function(r) Not String.IsNullOrWhiteSpace(r.EMailAddress))
        Dim separators As Char() = {";"c, ","c}
        Dim emails As IEnumerable(Of String) = available.SelectMany(Function(r) r.EMailAddress.Split(separators)).
            Select(Function(a) a.Trim().ToLowerInvariant()).
            Where(Function(a) Not String.IsNullOrEmpty(a)).
            Distinct().
            Where(Function(a) RegexHelper.IsValidEmail(a))

        Return String.Join(";", emails)
    End Function

    ''' <summary> Get manual contacts with valid email. </summary>
    Public Shared Function GetValidContacts(ByVal roles As IList(Of Role)) As IList(Of ContactDTO)
        Dim separators As Char() = {";"c, ","c}
        Dim contacts As IList(Of ContactDTO) = New List(Of ContactDTO)
        For Each role As Role In roles
            If String.IsNullOrEmpty(role.EMailAddress) Then Continue For

            For Each email As String In role.EMailAddress.Split(separators)
                If (String.IsNullOrEmpty(email) OrElse Not RegexHelper.IsValidEmail(email)) Then
                    Continue For
                End If

                If Not contacts.Any(Function(x) x.Contact.CertifiedMail.Eq(email)) Then contacts.Add(MailFacade.CreateManualContact(role.Name, email, ContactType.Role, True, True))
            Next
        Next
        Return contacts
    End Function


    ''' <summary> Get manual contacts with valid email. </summary>
    Public Shared Function CopyGetValidContacts(roles As IList(Of Entity.Commons.Role)) As IList(Of ContactDTO)
        Dim separators As Char() = {";"c, ","c}
        Dim contacts As IList(Of ContactDTO) = New List(Of ContactDTO)
        For Each role As Entity.Commons.Role In roles
            If String.IsNullOrEmpty(role.EMailAddress) Then Continue For

            For Each email As String In role.EMailAddress.Split(separators)
                If (String.IsNullOrEmpty(email) OrElse Not RegexHelper.IsValidEmail(email)) Then
                    Continue For
                End If

                If Not contacts.Any(Function(x) x.Contact.CertifiedMail.Eq(email)) Then contacts.Add(MailFacade.CreateManualContact(role.Name, email, ContactType.Role, True, True))
            Next
        Next
        Return contacts
    End Function

    Public Function GetContacts(roles As IList(Of Role)) As IList(Of Contact)
        Return _dao.GetContact(roles)
    End Function

    Public Function CheckCurrentUserPrivacyRoles(roleIds As Guid(), privacyRoleIds As Guid(), env As Integer) As Boolean
        Return _dao.CheckCurrentUserPrivacyRoles(roleIds, privacyRoleIds, env)
    End Function

    Public Function GetPrivacyUserRoles(privacyRoleIds As Guid()) As IList(Of Role)
        Return _dao.GetPrivacyUserRoles(privacyRoleIds)
    End Function

    Public Function GetUserRolesByIds(roleIds As Guid(), env As DSWEnvironment, right As Integer?) As IList(Of Role)
        Return _dao.GetUserRolesByIds(roleIds, env, right)
    End Function

    Public Function GetUserRolesByCategory(env As DSWEnvironment, roleIds As IList(Of Integer), rightPosition As Integer?, active As Boolean?, name As String, root As Boolean?, idTenantAOO As Guid, Optional roleUserType As RoleUserType? = Nothing) As IList(Of Role)
        Return _dao.GetRolesByCategoryFascicleRights(roleIds, env, rightPosition, active, name, root, Nothing, idTenantAOO, roleUserType)
    End Function

    Public Function GetRolesBySecurityGroups(securityGroups As IList(Of SecurityGroups)) As IList(Of Role)
        Return _dao.GetRolesBySecurityGroups(securityGroups)
    End Function

    Public Sub ActivateRole(role As Role)
        ActivateRole(role, False)
    End Sub

    Public Sub ActivateRole(role As Role, activateAllChildren As Boolean)
        ChangeRoleActiveState(role, True, activateAllChildren)
    End Sub

    Public Sub DisableRole(role As Role)
        DisableRole(role, False)
    End Sub

    Public Sub DisableRole(role As Role, disableAllChildren As Boolean)
        ChangeRoleActiveState(role, False, disableAllChildren)
    End Sub

    Private Sub ChangeRoleActiveState(role As Role, isActive As Boolean, recursiveChildren As Boolean)
        CommonTransactionalActions(Sub() _dao.BatchChangeRoleActiveState(role, isActive, recursiveChildren),
                                   Sub()
                                       _dao.ConnectionName = ReslDB
                                       _dao.BatchChangeRoleActiveState(role, isActive, recursiveChildren)
                                   End Sub,
                                   Sub()
                                       _dao.ConnectionName = DocmDB
                                       _dao.BatchChangeRoleActiveState(role, isActive, recursiveChildren)
                                   End Sub)
    End Sub

    Public Sub Clone(roleToClone As Role, name As String)
        CommonTransactionalSingleAction(Sub() RecursiveClone(New List(Of Role) From {roleToClone}, roleToClone.Father, name))
    End Sub

    Private Sub RecursiveClone(rolesToClone As ICollection(Of Role), parentRole As Role, Optional newName As String = "")
        If rolesToClone.IsNullOrEmpty() Then
            Exit Sub
        End If

        Dim roleSaved As Role
        Dim ochartRole As OChartItemRole
        For Each roleToClone As Role In rolesToClone.Where(Function(f) f.IsActive)
            roleSaved = CloneRoleRelations(roleToClone, parentRole, newName)
            Dim ochartRoleItems As ICollection(Of OChartItem) = FacadeFactory.Instance.OChartItemFacade.GetByRole(roleToClone)
            If Not ochartRoleItems.IsNullOrEmpty() Then
                For Each ochartRoleItem As OChartItem In ochartRoleItems.Where(Function(f) f.IsEnabled)
                    ochartRole = New OChartItemRole() With {.Item = ochartRoleItem, .Role = roleSaved}
                    FacadeFactory.Instance.OChartItemRoleFacade.SaveWithoutTransaction(ochartRole)
                Next
            End If

            RecursiveClone(roleToClone.Children, roleSaved)
        Next
    End Sub

    Private Function CloneRoleRelations(roleToClone As Role, parentRole As Role, Optional newName As String = "") As Role
        Dim lastUsedIdRole As Short = FacadeFactory.Instance.ParameterFacade.GetLastUsedIdRole()
        Dim lastUsedIdRoleUser As Short = FacadeFactory.Instance.ParameterFacade.GetLastUsedIdRoleUser()
        Dim roleToSave As Role = InitializeNewInstanceFromExistingRole(roleToClone)
        roleToSave.Id = lastUsedIdRole + 1
        If Not String.IsNullOrEmpty(newName) Then
            roleToSave.Name = newName
        End If

        If parentRole IsNot Nothing Then
            roleToSave.Father = parentRole
        End If

        CalculateFullIncremental(roleToSave)
        FacadeFactory.Instance.ParameterFacade.UpdateLastIdRoleWithoutTransaction(roleToSave.Id, lastUsedIdRole)
        SaveWithoutTransaction(roleToSave)

        If Not roleToClone.Mailboxes.IsNullOrEmpty() Then
            roleToSave.Mailboxes = New List(Of PECMailBox)
            For Each mailBox As PECMailBox In roleToClone.Mailboxes
                roleToSave.Mailboxes.Add(FacadeFactory.Instance.PECMailboxFacade.GetById(mailBox.Id))
            Next
            UpdateOnly(roleToSave, ProtDB, False)
        End If

        Dim roleGroups As ICollection(Of RoleGroup) = FacadeFactory.Instance.RoleGroupFacade.GetByRole(roleToClone.Id)
        If Not roleGroups.IsNullOrEmpty() Then
            Dim roleGroupToSave As RoleGroup
            For Each roleGroup As RoleGroup In roleGroups
                roleGroupToSave = RoleGroupFacade.InitializeNewInstanceFromExistingRoleGroup(roleGroup, roleToSave)
                FacadeFactory.Instance.RoleGroupFacade.SaveWithoutTransaction(roleGroupToSave)
            Next
        End If

        Dim roleUsers As IList(Of RoleUser) = FacadeFactory.Instance.RoleUserFacade.GetByRoleId(roleToClone.Id)
        Dim tmpLastUsedIdRoleUser As Integer = lastUsedIdRoleUser
        If Not roleUsers.IsNullOrEmpty() Then
            Dim roleUserToSave As RoleUser
            For Each roleUser As RoleUser In roleUsers
                roleUserToSave = RoleUserFacade.InitializeNewInstanceFromExistingRoleUser(roleUser, roleToSave)
                roleUserToSave.Id = tmpLastUsedIdRoleUser + 1
                FacadeFactory.Instance.RoleUserFacade.SaveWithoutTransaction(roleUserToSave)
                tmpLastUsedIdRoleUser = roleUserToSave.Id
            Next
        End If

        FacadeFactory.Instance.ParameterFacade.UpdateLastIdRoleUserWithoutTransaction(tmpLastUsedIdRoleUser, lastUsedIdRoleUser)
        Return roleToSave
    End Function

    Public Shared Function InitializeNewInstanceFromExistingRole(role As Role) As Role
        Dim newInstanceRole As Role = New Role With {
            .IsActive = False,
            .Collapsed = role.Collapsed,
            .EMailAddress = role.EMailAddress,
            .ServiceCode = role.ServiceCode,
            .IdTenantAOO = role.IdTenantAOO,
            .UriSharepoint = role.UriSharepoint,
            .Name = role.Name
        }
        Return newInstanceRole
    End Function

End Class