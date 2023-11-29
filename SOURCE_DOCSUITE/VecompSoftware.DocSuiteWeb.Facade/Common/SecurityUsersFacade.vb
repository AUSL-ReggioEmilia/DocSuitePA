Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Core.Command
Imports VecompSoftware.Core.Command.CQRS.Commands.Entities.Commons
Imports VecompSoftware.Core.Command.CQRS.Events.Entities.Commons
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons
Imports VecompSoftware.Services.Command
Imports VecompSoftware.Services.Command.CQRS.Commands
Imports VecompSoftware.Services.Command.CQRS.Commands.Entities.Commons
Imports VecompSoftware.Services.Command.CQRS.Events
Imports VecompSoftware.Services.Command.CQRS.Events.Entities
Imports VecompSoftware.Services.Command.CQRS.Events.Entities.Commons
Imports VecompSoftware.Services.Logging
Imports APICommons = VecompSoftware.DocSuiteWeb.Entity.Commons

Public Class SecurityUsersFacade
    Inherits CommonFacade(Of SecurityUsers, Integer, NHibernateSecurityUsersDao)

#Region " Fields "
    Private _mapperSecurityUserEntity As MapperSecurityUserEntity
#End Region

#Region " Properties "
    Public ReadOnly Property MapperSecurityUserEntity As MapperSecurityUserEntity
        Get
            If _mapperSecurityUserEntity Is Nothing Then
                _mapperSecurityUserEntity = New MapperSecurityUserEntity
            End If
            Return _mapperSecurityUserEntity
        End Get
    End Property
#End Region

#Region " Constructor "
    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub
#End Region

#Region " Methods "
    Public Overrides Sub Save(ByRef obj As SecurityUsers)
        obj.Id = _dao.GetMaxId() + 1
        MyBase.Save(obj)
    End Sub

    ''' <summary> Esegue l'inserimento di un Nuovo gruppo nella tabella SecurityUsers. </summary>
    Public Overridable Function Insert(ByVal domain As String, ByVal account As String, ByVal description As String, ByRef group As SecurityGroups) As SecurityUsers
        Dim user As New SecurityUsers With {
            .UserDomain = domain,
            .Account = account,
            .Description = description,
            .Group = group
        }
        Save(user)
        FacadeFactory.Instance.TableLogFacade.Insert("SecurityGroups", LogEvent.INS, String.Format("Inserito Utente {0}\{1}", user.UserDomain, user.Account), group.UniqueId)
        SendInsertSecurityUserCommand(user)
        CacheSingleton.Instance.ClearSecurityCache()
        Return user
    End Function

    Public Function GetUsersByGroup(idGroup As Integer) As IList(Of SecurityUsers)
        Return _dao.GetUsersByGroup(idGroup)
    End Function

    Public Function GetUsersByGroup(group As SecurityGroups) As IList(Of SecurityUsers)
        Return _dao.GetUsersByGroup(group.Id)
    End Function

    Public Function GetUsersByAccount(account As String, excludeDomain As String) As IList(Of SecurityUsers)
        Return _dao.GetUsersByAccount(account, excludeDomain)
    End Function

    Public Function GetUsersByAccountOrDescription(searchText As String, domain As String) As IList(Of SecurityUsers)
        Return _dao.GetUsersByAccountOrDescription(searchText, domain)
    End Function

    Public Function GetUsersByAccountAndGroups(account As String, domain As String, idGroups As Integer()) As IList(Of SecurityUsers)
        Return _dao.GetUsersByAccountAndGroups(account, domain, idGroups)
    End Function

    Public Function GetUsersByDescription(description As String, domain As String) As IList(Of SecurityUsers)
        Return _dao.GetUsersByDescription(description, domain)
    End Function

    Public Function GetSecurityUsersCount(account As String, domain As String) As Long
        Return _dao.GetSecurityUsersCount(account, domain)
    End Function
    Public Function ExistsUser(account As String, domain As String) As Boolean
        Return _dao.ExistsUser(account, domain)
    End Function
    Public Function ExistsUser(user As AccountModel) As Boolean
        Return _dao.ExistsUser(user.Account, user.Domain)
    End Function

    Public Function GetGroupsByAccount(ByVal account As String, Optional groups As Integer() = Nothing) As IList(Of SecurityGroups)
        Dim userDomain As String = Nothing
        Dim userName As String = account
        Dim splitted As String() = account.Split("\"c)
        If splitted.Length > 1 Then
            userDomain = splitted(0)
            userName = splitted(1)
        End If

        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled Then
            ' Se gestisco più domini e userDomain = null utilizzo il dominio dell'utente attualmente collegato e non quanto impostato in default
            Return GetGroupsByAccount(userDomain, DocSuiteContext.Current.User.Domain, userName, groups)
        End If
        Return GetGroupsByAccount(Nothing, DocSuiteContext.Current.CurrentDomainName, userName, groups)
    End Function

    Public Function CountGroupsByAccount(ByVal account As String, Optional groups As Integer() = Nothing) As Integer
        Dim userDomain As String = Nothing
        Dim userName As String = account
        Dim splitted As String() = account.Split("\"c)
        If splitted.Length > 1 Then
            userDomain = splitted(0)
            userName = splitted(1)
        End If

        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled Then
            ' Se gestisco più domini e userDomain = null utilizzo il dominio dell'utente attualmente collegato e non quanto impostato in default
            Return CountGroupsByAccount(userDomain, DocSuiteContext.Current.User.Domain, userName, groups)
        End If
        Return CountGroupsByAccount(Nothing, DocSuiteContext.Current.CurrentDomainName, userName, groups)
    End Function

    Private Function GetGroupsByAccount(userDomain As String, defaultDomain As String, account As String, Optional groups As Integer() = Nothing) As IList(Of SecurityGroups)
        Return _dao.GetGroupsByAccount(userDomain, defaultDomain, account, groups)
    End Function

    Private Function CountGroupsByAccount(userDomain As String, defaultDomain As String, account As String, Optional groups As Integer() = Nothing) As Integer
        Return _dao.CountGroupsByAccount(userDomain, defaultDomain, account, groups)
    End Function

    Public Function IsUserInGroup(group As SecurityGroups, account As String) As Boolean
        Return GetUsersByGroupAndAccount(group.Id, account).Count > 0
    End Function

    Public Function GetUsersByGroupAndAccount(idGroup As Integer, account As String) As IList(Of SecurityUsers)
        Dim userDomain As String = Nothing
        Dim userName As String = account
        Dim splitted As String() = account.Split("\"c)
        If splitted.Length > 1 Then
            userDomain = splitted(0)
            userName = splitted(1)
        End If

        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled Then
            Return GetUsersByGroupAndAccount(idGroup, userDomain, DocSuiteContext.Current.User.Domain, userName)
        End If
        Return GetUsersByGroupAndAccount(idGroup, Nothing, DocSuiteContext.Current.CurrentDomainName, userName)
    End Function

    Private Function GetUsersByGroupAndAccount(idGroup As Integer, userDomain As String, defaultDomain As String, account As String) As IList(Of SecurityUsers)
        Return _dao.GetUsersByGroupAndAccount(idGroup, userDomain, defaultDomain, account)
    End Function

    Public Overrides Function Delete(ByRef entity As SecurityUsers) As Boolean
        Dim retval As Boolean = False
        Dim name As String = entity.DisplayName
        Dim id As Guid = entity.Group.UniqueId
        retval = MyBase.Delete(entity)
        FacadeFactory.Instance.TableLogFacade.Insert("SecurityGroups", LogEvent.DL, String.Format("Eliminato Utente {0}", name), id)
        SendDeleteSecurityUserCommand(entity)
        CacheSingleton.Instance.ClearSecurityCache()
        Return retval
    End Function

    Public Function SendInsertSecurityUserCommand(securityUser As SecurityUsers) As Guid?
        Try
            Dim identityContext As IIdentityContext = New IdentityContext(DocSuiteContext.Current.User.FullUserName)
            Dim commandCreateSecurityUser As CommandCreateSecurityUser = PrepareSecurityUserCommand(securityUser,
                Function(apiSecurityUser)
                    Return New CommandCreateSecurityUser(DocSuiteContext.Current.CurrentTenant.TenantName,
                    DocSuiteContext.Current.CurrentTenant.TenantId, CurrentTenant.TenantAOO.UniqueId, identityContext, apiSecurityUser)
                End Function)

            Dim commandFacade As CommandFacade(Of ICommandCreateSecurityUser) = New CommandFacade(Of ICommandCreateSecurityUser)
            commandFacade.Push(commandCreateSecurityUser)
            Return commandCreateSecurityUser.Id
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Concat("SendInsertSecurityUserCommand => ", ex.Message), ex)
        End Try
        Return Nothing
    End Function

    Public Function SendDeleteSecurityUserCommand(securityUser As SecurityUsers) As Guid?
        Try
            Dim identityContext As IIdentityContext = New IdentityContext(DocSuiteContext.Current.User.FullUserName)
            Dim commandDeleteSecurityUser As CommandDeleteSecurityUser = PrepareSecurityUserCommand(securityUser,
                Function(apiSecurityUser)
                    Return New CommandDeleteSecurityUser(DocSuiteContext.Current.CurrentTenant.TenantName,
                    DocSuiteContext.Current.CurrentTenant.TenantId, CurrentTenant.TenantAOO.UniqueId, identityContext, apiSecurityUser)
                End Function)

            Dim commandFacade As CommandFacade(Of ICommandDeleteSecurityUser) = New CommandFacade(Of ICommandDeleteSecurityUser)
            commandFacade.Push(commandDeleteSecurityUser)
            Return commandDeleteSecurityUser.Id
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Concat("SendDeleteSecurityUserCommand => ", ex.Message), ex)
        End Try
        Return Nothing
    End Function

    Private Function PrepareSecurityUserCommand(Of T As ICommand)(securityUser As SecurityUsers, commandInitializeFunc As Func(Of APICommons.SecurityUser, T)) As T
        Dim apiSecurityUser As APICommons.SecurityUser = MapperSecurityUserEntity.MappingDTO(securityUser)
        Return commandInitializeFunc(apiSecurityUser)
    End Function
#End Region

End Class
