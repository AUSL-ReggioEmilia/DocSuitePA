Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

<ComponentModel.DataObject()>
Public Class RoleUserFacade
    Inherits CommonFacade(Of RoleUser, Integer, NHibernateRoleUserDao)

#Region " Fields "


#End Region

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(DbName As String)
        MyBase.New(DbName)
    End Sub

#End Region

#Region " Methods "

    Public Overloads Sub Update()

    End Sub

    Public Function GetByUserType(userType As RoleUserType?, account As String, onlyEnabled As Boolean, idRoles As List(Of Integer), idTenantAOO As Guid) As IList(Of RoleUser)
        Return _dao.GetByUserType(userType, account, onlyEnabled, idRoles, idTenantAOO)
    End Function

    Public Function GetSecretaryRolesByAccount(account As String, environment As DSWEnvironment?, idTenantAOO As Guid) As IList(Of Role)
        Return _dao.GetSecretaryRolesByAccount(account, environment, idTenantAOO)
    End Function

    ''' <summary>
    ''' Restituisce il grado più alto ricoperto dall'operatore corrente nelle collaborazioni per i soli settori attivi
    ''' </summary>
    ''' <returns>Restituisce una stringa rappresentante il livello (D, V, S, X)</returns>
    ''' <remarks>Utilizzare le costanti di RoleUserFacade per confrontare il valore restituito</remarks>
    Public Function GetHighestUserType(idTenantAOO As Guid) As RoleUserType
        Return GetHighestUserType(DocSuiteContext.Current.User.FullUserName, idTenantAOO)
    End Function

    ''' <summary>
    ''' Restituisce il grado più alto ricoperto dall'operatore nelle collaborazioni per i soli settori attivi
    ''' </summary>
    ''' <param name="userName">Identificativo dell'operatore</param>
    ''' <returns>Restituisce una stringa rappresentante il livello (D, V, S, X)</returns>
    ''' <remarks>Utilizzare le costanti di RoleUserFacade per confrontare il valore restituito</remarks>
    Public Function GetHighestUserType(userName As String, idTenantAOO As Guid) As RoleUserType
        Return GetHighestUserType(userName, True, idTenantAOO)
    End Function

    ''' <summary>
    ''' Restituisce il grado più alto ricoperto dall'operatore nelle collaborazioni
    ''' </summary>
    ''' <param name="userName">Identificativo dell'operatore</param>
    ''' <param name="onlyActive">Indica se devono essere presi in considerazione solo i settori attivi</param>
    ''' <returns>Restituisce una stringa rappresentante il livello (D, V, S, X)</returns>
    ''' <remarks>Utilizzare le costanti di RoleUserFacade per confrontare il valore restituito</remarks>
    Public Function GetHighestUserType(userName As String, onlyActive As Boolean, idTenantAOO As Guid) As RoleUserType

        Dim dType As IList(Of RoleUser) = GetByUserType(RoleUserType.D, userName, onlyActive, Nothing, idTenantAOO)
        If Not dType.IsNullOrEmpty() Then
            Return RoleUserType.D
        End If

        dType = GetByUserType(RoleUserType.V, userName, onlyActive, Nothing, idTenantAOO)
        If Not dType.IsNullOrEmpty() Then
            Return RoleUserType.V
        End If

        dType = GetByUserType(RoleUserType.S, userName, onlyActive, Nothing, idTenantAOO)
        If Not dType.IsNullOrEmpty() Then
            Return RoleUserType.S
        End If

        Return RoleUserType.X
    End Function

    Public Function GetByType(type As RoleUserType, enabled As Boolean, roleNameFilter As String, idTenantAOO As Guid) As IList(Of RoleUser)
        Return _dao.GetByType(type.ToString(), enabled, roleNameFilter, idTenantAOO)
    End Function

    Public Function GetByRoleId(roleId As Integer) As IList(Of RoleUser)
        Return _dao.GetByRoleId(roleId)
    End Function

    Public Function GetByRoleIdAndType(roleId As Integer, type As RoleUserType, onlyEnabled As Boolean?, mainRoleOnly As Boolean?) As IList(Of RoleUser)
        Return _dao.GetByRoleIdAndType(roleId, type.ToString, onlyEnabled, mainRoleOnly)
    End Function

    Public Function GetByRoleIdAndAccount(roleId As Integer, account As String, type As String) As IList(Of RoleUser)
        Return _dao.GetByRoleIdAndAccount(roleId, account, type)
    End Function

    Public Function GetByRoleIdAndAccount(roleId As Integer, account As String, type As RoleUserType) As IList(Of RoleUser)
        Return _dao.GetByRoleIdAndAccount(roleId, account, type.ToString())
    End Function

    Public Function GetByRoleIdAndAccount(roleId As Integer, account As String, onlyEnabled As Boolean) As IList(Of RoleUser)
        Return _dao.GetByRoleIdAndAccount(roleId, account, onlyEnabled)
    End Function

    Public Function GetByRoleIdsAndAccount(roleIds As List(Of Integer), account As String, type As String) As IList(Of RoleUser)
        Return _dao.GetByRoleIdsAndAccount(roleIds, account, type)
    End Function

    Public Function GetByAccountsAndNotType(userConnected As String, type As RoleUserType?, onlyEnabled As Boolean) As IList(Of RoleUser)
        Return _dao.GetByAccountsAndNotType(userConnected, type, onlyEnabled)
    End Function

    Public Function GetByAccount(username As String) As IList(Of RoleUser)
        Return _dao.GetByAccount(username)
    End Function

    Public Function IsCurrentUserPrivacyManager(roleIds As Guid()) As Boolean
        Return _dao.IsCurrentUserPrivacyManager(roleIds)
    End Function

    Public Function GetManagersByCollaboration(collaborationId As Integer, account As String, idTenantAOO As Guid) As IList(Of RoleUser)
        Return _dao.GetManagersByCollaboration(collaborationId, account, idTenantAOO)
    End Function

    Public Function GetCountManagersByPecMailBox(pecMailBoxId As Short, account As String, idTenantAOO As Guid) As Integer
        Return _dao.GetCountManagersByPecMailBox(pecMailBoxId, account, idTenantAOO)
    End Function

    Public Function GetByCollaboration(collaborationId As Integer, roleUserType As RoleUserType?, destinationFirst As Boolean?, mainRoleOnly As Boolean?, idTenantAOO As Guid) As IList(Of RoleUser)
        Return _dao.GetByCollaboration(collaborationId, roleUserType, destinationFirst, mainRoleOnly, idTenantAOO)
    End Function

    Public Overrides Sub Save(ByRef obj As RoleUser)
        'Recupero ID dalla tabella parameter
        Dim pf As New ParameterFacade(_dbName)
        Dim parameter As Parameter = pf.GetAll()(0)
        obj.UniqueId = Guid.NewGuid()
        obj.Id = parameter.LastUsedIdRoleUser + 1
        If (pf.UpdateReplicateLastIdRoleUser(obj.Id, parameter.LastUsedIdRoleUser)) Then
            MyBase.Save(obj)
        End If
        parameter.LastUsedIdRoleUser = parameter.LastUsedIdRoleUser + 1S
    End Sub

    Public Function GetRoleUserTypeName(type As String) As String
        Select Case type
            Case RoleUserType.D.ToString()
                Return DocSuiteContext.Current.ProtocolEnv.NomeDirigentiCollaborazione
            Case RoleUserType.V.ToString()
                Return DocSuiteContext.Current.ProtocolEnv.NomeViceCollaborazione
            Case RoleUserType.S.ToString()
                Return DocSuiteContext.Current.ProtocolEnv.NomeSegreteria
        End Select
        Return String.Empty
    End Function

    ''' <summary>
    ''' Carica tutti i settori per i quali l'utente "account" è impostato come firmatario o vice
    ''' </summary>
    ''' <param name="account"></param>
    ''' <param name="mainRoleOnly">Definisce se ricercare solo per i disegni di collaborazione in cui l'utente è impostato come posizione principale</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSecretaryRoles(account As String, mainRoleOnly As Boolean, idTenantAOO As Guid) As IList(Of Role)
        Dim roles As New List(Of Role)
        ' Ruoli dove sono direttore
        roles.AddRange((From roleUser In GetByUserType(RoleUserType.D, account, True, Nothing, idTenantAOO) Where roleUser.IsMainRole OrElse Not mainRoleOnly Select roleUser.Role).ToList())
        ' Ruoli dove sono vice
        roles.AddRange((From roleUser In GetByUserType(RoleUserType.V, account, True, Nothing, idTenantAOO) Where roleUser.IsMainRole OrElse Not mainRoleOnly Select roleUser.Role).ToList())
        ' Effettuo un distinct sull'ID
        Return roles.GroupBy(Function(r) New With {Key r.Id}).Select(Function(c) c.First()).ToList()
    End Function

    Public Function GetAccountSecretaryRoles(account As String, environment As DSWEnvironment?, idTenantAOO As Guid, onlyActive As Boolean) As IList(Of Role)
        Dim roleUsers As ICollection(Of RoleUser) = _dao.GetAccountSecretaryRoles(account, idTenantAOO, onlyActive, environment)
        Return roleUsers.Select(Function(s) s.Role).ToList()
    End Function

    Public Function GetFirstParentWithSecretaryRoles(account As String, idTenantAOO As Guid, baseRole As Integer?, environment As DSWEnvironment?) As IList(Of Role)
        Dim parentRoles As ICollection(Of Role) = _dao.GetParentWithSecretaryRoles(account, idTenantAOO, baseRole, environment)
        If parentRoles.IsNullOrEmpty() Then
            Return New List(Of Role)
        End If

        Return parentRoles.Where(Function(x) Not parentRoles.Any(Function(xx) xx.FullIncrementalPath.Contains(x.Id.ToString()) AndAlso xx.Id <> x.Id)).ToList()
    End Function

    Public Function GetAccounts(userConnected As String, Optional onlyEnabled As Boolean = False) As IList(Of String)
        Return _dao.GetAccounts(userConnected, onlyEnabled)
    End Function

    Public Shared Function InitializeNewInstanceFromExistingRoleUser(roleUser As RoleUser, role As Role) As RoleUser
        Dim newInstanceRoleUser As RoleUser = New RoleUser With {
            .Role = role,
            .Account = roleUser.Account,
            .Description = roleUser.Description,
            .DSWEnvironment = roleUser.DSWEnvironment,
            .Email = roleUser.Email,
            .Enabled = roleUser.Enabled,
            .IdUDSRepository = roleUser.IdUDSRepository,
            .IsMainRole = roleUser.IsMainRole,
            .Type = roleUser.Type
        }
        Return newInstanceRoleUser
    End Function

#End Region

End Class
