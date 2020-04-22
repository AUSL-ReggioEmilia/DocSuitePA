Imports System.DirectoryServices
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Text
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports System.Collections.Concurrent
Imports System.DirectoryServices.AccountManagement

''' <summary> Classe che gestisce le chiamate a LDAP e WinNT attraverso i DirectoryServices. </summary>
Public Class CommonAD

#Region " Fields "

    Private Const WinNtRoot As String = "WinNT://"
    Public Const LDAPRoot As String = "LDAP://"

    ''' <summary>
    ''' Stringa per cercare oggetti in AD consultando il Common Name (CN)
    ''' </summary>
    ''' <remarks></remarks>
    Private Const CommonNameSearcherKey As String = "cn=*{0}*"
    ''' <summary>
    ''' Stringa per cercare oggetti in AD consultando il nome Account
    ''' </summary>
    ''' <remarks></remarks>
    Private Const AccountNameSearcherkey As String = "sAMAccountName={0}"
    ''' <summary>
    ''' Connettivo logico OR per effettuare ricerche in AD
    ''' </summary>
    ''' <remarks></remarks>
    Private Const SearcherOrCommand As String = "(|({0})({1}))"

    Private Shared _cache_getCurrentUser As ConcurrentDictionary(Of String, AccountModel) = New ConcurrentDictionary(Of String, AccountModel)()

#End Region

#Region " Methods "

    Public Shared Function GetTenantModelFromUserName(ByRef userName As String) As TenantModel
        Dim tenantModels As IReadOnlyCollection(Of TenantModel) = DocSuiteContext.Current.Tenants
        Dim domainFromUserName As String = DocSuiteContext.Current.CurrentTenant.DomainName
        Dim values As String() = userName.Split("\"c)
        If values.Length > 1 Then
            domainFromUserName = values.First()
            userName = values.Last()
        End If
        Dim domain As TenantModel = tenantModels.FirstOrDefault(Function(f) f.DomainName.Eq(domainFromUserName))
        If (domain Is Nothing AndAlso Not DocSuiteContext.Current.ProtocolEnv.EnableFederationAD) Then
            Throw New DocSuiteException(String.Concat("Dominio ", domainFromUserName, " non configurato. Contattare Assistenza"))
        End If
        If (domain Is Nothing AndAlso DocSuiteContext.Current.ProtocolEnv.EnableFederationAD) Then
            domain = DocSuiteContext.Current.CurrentTenant
        End If
        Return domain
    End Function

    Public Shared Function GetAccount(ByVal userName As String) As AccountModel
        FileLogger.Info(LogName.DirectoryServiceLog, String.Format("GetAccount [{0}]", userName))

        Dim account As String = userName
        Dim domain As String = DocSuiteContext.Current.CurrentTenant.DomainName
        If userName.Contains("\") Then
            Dim splited As String() = userName.Split("\"c)
            account = splited.Last()
            domain = splited.First()
        End If

        Dim result As AccountModel = New AccountModel(account, account, domain)
        Try
            If _cache_getCurrentUser.ContainsKey(userName) AndAlso _cache_getCurrentUser.TryGetValue(userName, result) Then
                Return result
            End If

            Dim tenantModel As TenantModel = GetTenantModelFromUserName(userName)

            Using context As PrincipalContext = New PrincipalContext(CType(tenantModel.SecurityContext, ContextType), tenantModel.DomainAddress, tenantModel.DomainUser, tenantModel.DomainPassword)
                Using user As UserPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName)
                    If user Is Nothing Then
                        Throw New Exception($"Account {userName} not found in domain controller {domain}")
                    End If
                    result = New AccountModel(user, domain)
                    _cache_getCurrentUser.TryAdd(userName, result)
                    Return result
                End Using
            End Using

        Catch ex As Exception
            FileLogger.Error(LogName.DirectoryServiceLog, $"Problema nell'accesso al dominio utenti GetADUser: [{userName}]", ex)
#If Not DEBUG Then
            Throw New DocSuiteException($"Problema nell'accesso al dominio utenti GetADUser: [{userName}]", ex)
#End If
        End Try
        Return result
    End Function

    Public Shared Function GetDisplayName(ByVal userName As String) As String
        FileLogger.Info(LogName.DirectoryServiceLog, String.Format("GetDisplayName [{0}]", userName))

        Dim fullName As String = userName
        If Not String.IsNullOrEmpty(userName) Then
            Try
                Dim accountModel As AccountModel = GetAccount(userName)
                fullName = accountModel.DisplayName
            Catch ex As Exception
                FileLogger.Warn(LogName.DirectoryServiceLog, String.Format("Problema nell'accesso al dominio utenti GetDisplayName: [{0}] CommonShared.UserDomain: [{1}]", fullName, CommonShared.UserDomain), ex)
            End Try
        End If
        Return fullName
    End Function

    ''' <summary> Restituisce l'indirizzo email dato un utente LDAP </summary>
    Public Shared Function LdapUserEmail(ByVal userName As String, domain As String) As String
        FileLogger.Debug(LogName.DirectoryServiceLog, String.Format("LdapUserEmail [{0}, {1}]", userName, domain))
        Dim evalaute As String = userName
        If Not userName.Contains("\") Then
            evalaute = $"{domain}\{userName}"
        End If
        Dim adUser As AccountModel = GetAccount(evalaute)

        Dim email As String = adUser.Email
        FileLogger.Debug(LogName.DirectoryServiceLog, String.Format("LdapUserEmail return [{0}]", email))
        Return email
    End Function

    ''' <summary> Ricerca contatti in AD </summary>
    Public Shared Function FindADContactsAndDistributionsList(ByVal filter As String, ByVal domain As String) As IList(Of ADContact)
        Dim contacts As New SortedList(Of String, ADContact)
        Dim contact As ADContact
        '' Carico i domini
        Dim tenantModels As List(Of TenantModel) = New List(Of TenantModel)(DocSuiteContext.Current.Tenants)
        '' Se il dominio non è vuoto allora rimuovo tutti quelli non corrispondenti
        If Not String.IsNullOrEmpty(domain) Then
            tenantModels.RemoveAll(Function(d) Not d.DomainName.Eq(domain))
        End If

        '' Definisco il tipo di ricerca da effettuare
        Dim searchFilters As String() = New String() {DocSuiteContext.Current.ProtocolEnv.BasicContactSearcherKey, DocSuiteContext.Current.ProtocolEnv.BasicDistributionSearcherKey}


        '' Ricerco su tutti i domini definiti (nel caso di dominio singolo corrisponde ad una ricerca sul primo)
        For Each searchFilter As String In searchFilters

            For Each configuration As TenantModel In tenantModels
                Try
                    Using entry As New DirectoryEntry(String.Concat(LDAPRoot, configuration.DomainAddress), configuration.DomainUser, configuration.DomainPassword)
                        Using searcher As New DirectorySearcher(entry, String.Format(searchFilter, filter))
                            Using results As SearchResultCollection = searcher.FindAll()
                                For Each searchResult As SearchResult In results
                                    contact = New ADContact(searchResult.Properties)
                                    contacts.Add(String.Concat(configuration.DomainAddress, contact.DisplayEmail), contact)
                                Next
                            End Using

                        End Using
                    End Using
                Catch ex As Exception
                    FileLogger.Error(LogName.DirectoryServiceLog, "Errore ricerca contatti", ex)
                    Throw New DocSuiteException("Problema nell'accesso al dominio utenti", ex)
                End Try
            Next
        Next

        Return contacts.Values
    End Function

    ''' <summary> Ricerca User in AD </summary>
    Public Shared Function FindADUsers(ByVal filter As String, ByVal domain As String) As IList(Of AccountModel)
        FileLogger.Debug(LogName.DirectoryServiceLog, $"FindADUsers [{filter}, {domain}]")

        Dim users As New SortedList(Of String, AccountModel)
        Dim user As AccountModel

        '' Carico i domini
        Dim tenantModels As List(Of TenantModel) = New List(Of TenantModel)(DocSuiteContext.Current.Tenants)

        '' Se il dominio non è vuoto allora rimuovo tutti quelli non corrispondenti
        If Not String.IsNullOrEmpty(domain) Then
            tenantModels.RemoveAll(Function(d) Not d.DomainName.Eq(domain))
        End If

        Dim searchFilter As String = String.Format(DocSuiteContext.Current.ProtocolEnv.BasicPersonSearcherKey, filter)

        Try
            For Each configuration As TenantModel In tenantModels
                Using entry As New DirectoryEntry($"{GetQueryADFormat(configuration)}{configuration.DomainAddress}", configuration.DomainUser, configuration.DomainPassword)
                    If configuration.SecurityContext = SecurityContextType.Machine Then
                        For Each found As DirectoryEntry In entry.Children.OfType(Of DirectoryEntry)
                            If found.SchemaClassName = "User" AndAlso (found.Name.Contains(filter) OrElse found.Username.Contains(filter)) Then
                                user = New AccountModel(found.Username.Split("\"c).Last(), found.Name, configuration.DomainName)
                                If Not users.ContainsKey(user.GetFullUserName()) Then
                                    users.Add(user.GetFullUserName(), user)
                                End If
                            End If
                        Next
                    Else
                        Using searcher As New DirectorySearcher(entry, String.Format(searchFilter, filter))
                            Using results As SearchResultCollection = searcher.FindAll()
                                For Each searchResult As SearchResult In results
                                    user = New AccountModel(searchResult.Properties, configuration.DomainName)
                                    If Not users.ContainsKey(user.GetFullUserName()) Then
                                        users.Add(user.GetFullUserName(), user)
                                    End If
                                Next
                            End Using

                        End Using
                    End If
                End Using
            Next
        Catch ex As Exception
            FileLogger.Error(LogName.DirectoryServiceLog, "Errore ricerca utenti", ex)
            Throw New DocSuiteException("Problema nell'accesso al dominio utenti", ex)
        End Try

        Return users.Values
    End Function

    ''' <summary> Ricerca User in AD per specifici gruppi </summary>
    Public Shared Function FindADUsers(ByVal filter As String, ByVal domain As String, groups As IList(Of String)) As IList(Of AccountModel)
        FileLogger.Debug(LogName.DirectoryServiceLog, String.Format("FindADUsers [{0}, {1}, {2}]", filter, domain, groups Is Nothing))
        If (groups Is Nothing OrElse Not groups.Any()) Then
            Return FindADUsers(filter, domain)
        End If

        Dim users As New SortedList(Of String, AccountModel)
        Dim user As AccountModel

        '' Carico i domini
        Dim tenantModels As List(Of TenantModel) = New List(Of TenantModel)(DocSuiteContext.Current.Tenants)

        '' Se il dominio non è vuoto allora rimuovo tutti quelli non corrispondenti
        If Not String.IsNullOrEmpty(domain) Then
            tenantModels.RemoveAll(Function(d) Not d.DomainName.Eq(domain))
        End If

        Dim searchFilter As String = String.Format(DocSuiteContext.Current.ProtocolEnv.BasicPersonSearcherKey, filter)
        searchFilter = String.Format("(&{0}(|{1}))", searchFilter, String.Join("", groups.Select(Function(f) String.Concat("(memberOf=", f, ")")).ToArray()))

        Try
            If DocSuiteContext.DomainPath.Eq("WINNT") Then
                Dim username As String = DocSuiteContext.Current.CurrentTenant.DomainUser
                Dim password As String = DocSuiteContext.Current.CurrentTenant.DomainPassword
                Dim currentDomainName As String = DocSuiteContext.Current.CurrentTenant.DomainName
                Using entry As New DirectoryEntry(String.Concat(WinNtRoot, currentDomainName), username, password)
                    entry.Children.SchemaFilter.Add("user")
                    For Each item As DirectoryEntry In entry.Children
                        Dim accountName As String = item.Name
                        If Not accountName.ContainsIgnoreCase(filter) Then
                            Continue For
                        End If

                        user = New AccountModel(accountName, String.Empty)
                        user.Domain = currentDomainName
                        Dim fullName As String = If(item.Properties("Fullname").Count = 1 AndAlso item.Properties("Fullname")(0) IsNot Nothing, item.Properties("Fullname")(0).ToString(), String.Empty)
                        If Not String.IsNullOrEmpty(fullName) Then
                            user.Name = fullName
                        End If
                        users.Add(user.GetFullUserName(), user)
                    Next
                End Using
            Else
                For Each configuration As TenantModel In tenantModels
                    Using entry As New DirectoryEntry(String.Concat(LDAPRoot, configuration.DomainAddress), configuration.DomainUser, configuration.DomainPassword)
                        Using searcher As New DirectorySearcher(entry, String.Format(searchFilter, filter))
                            Using results As SearchResultCollection = searcher.FindAll()
                                For Each searchResult As SearchResult In results
                                    user = New AccountModel(searchResult.Properties, configuration.DomainName)
                                    If Not users.ContainsKey(user.GetFullUserName()) Then
                                        users.Add(user.GetFullUserName(), user)
                                    End If
                                Next
                            End Using

                        End Using
                    End Using
                Next
            End If
        Catch ex As Exception
            FileLogger.Error(LogName.DirectoryServiceLog, "Errore ricerca utenti", ex)
            Throw New DocSuiteException("Problema nell'accesso al dominio utenti", ex)
        End Try

        Return users.Values
    End Function

    Private Shared Function TryGetDomainUser(entry As DirectoryEntry, defaultDomain As String) As String
        Try
            Return entry.Path.Split(New String() {",DC="}, StringSplitOptions.None)(1)
        Catch
        End Try
        Return defaultDomain
    End Function
    ''' <summary> Ricerca Users in AD </summary>
    Public Shared Function GetADUsersFromGroup(ByVal groupName As String, currentDomain As String) As IList(Of AccountModel)
        FileLogger.Debug(LogName.DirectoryServiceLog, String.Format("GetADUserFromGroup [{0}, {1}]", groupName, currentDomain))

        Dim users As SortedList(Of String, AccountModel) = New SortedList(Of String, AccountModel)
        Dim user As AccountModel

        Dim searchFilter As String = String.Format("(&(objectCategory=group)(cn={0}))", groupName)
        Try
            Using searcher As New DirectorySearcher(searchFilter)
                Dim res As SearchResult = searcher.FindOne()
                Dim entry As DirectoryEntry
                If res IsNot Nothing Then
                    Using deRes As New DirectoryEntry(res.Path)
                        Dim members As PropertyValueCollection = deRes.Properties.Item("member")
                        For Each member As String In members
                            entry = New DirectoryEntry(String.Concat(LDAPRoot, member))
                            user = New AccountModel(entry.Properties("samaccountname")(0).ToString(), entry.Properties("name")(0).ToString(), TryGetDomainUser(entry, currentDomain))
                            users.Add(user.Account, user)
                        Next
                    End Using
                End If
            End Using
        Catch ex As Exception
            FileLogger.Error(LogName.DirectoryServiceLog, "Errore ricerca utenti - GetADUsersFromGroup", ex)
        End Try

        Return users.Values
    End Function

    ''' <summary>
    ''' Metodo che permette la modifica della password di un utente di dominio
    ''' </summary>
    ''' <param name="accountName">account da modificare</param>
    ''' <param name="domain">dominio di riferimento dell'utente</param>
    ''' <param name="oldPassword">password attuale dell'utente da modificare</param>
    ''' <param name="newPassword">nuova password da impostare</param>
    ''' <remarks>Usare con cautela, è necessario logOff successivo per apportare le modifiche</remarks>
    Public Shared Function ChangeAdUserPassword(accountName As String, domain As String, oldPassword As String, newPassword As String) As Boolean
        Dim domainConfiguration As TenantModel = DocSuiteContext.Current.Tenants.SingleOrDefault(Function(x) x.DomainName.Eq(domain))
        If domainConfiguration Is Nothing Then
            Return False
        End If

        'Verifico se la vecchia password è corretta
        If Not CheckAdUserPassword(accountName, domainConfiguration.DomainAddress, oldPassword) Then
            FileLogger.Warn(LogName.DirectoryServiceLog, "Nome utente o password non validi")
            Return False
        End If

        Dim userDirToUpdate As DirectoryEntry = Nothing
        Try
            If DocSuiteContext.DomainPath.Eq("WINNT") Then
                Dim username As String = DocSuiteContext.Current.CurrentTenant.DomainUser
                Dim password As String = DocSuiteContext.Current.CurrentTenant.DomainPassword
                Dim directoryEntry As DirectoryEntry = New DirectoryEntry(String.Concat(WinNtRoot, domainConfiguration.DomainAddress, "/", accountName, ",user"), username, password, AuthenticationTypes.Secure)
                userDirToUpdate = directoryEntry
                If userDirToUpdate.NativeObject Is Nothing Then
                    Return False
                End If
            Else
                Dim directoryEntry As DirectoryEntry = New DirectoryEntry(domainConfiguration.DomainAddress, DocSuiteContext.Current.CurrentTenant.DomainUser, DocSuiteContext.Current.CurrentTenant.DomainPassword, AuthenticationTypes.Secure)
                Dim searchFilter As String = DocSuiteContext.Current.ProtocolEnv.BasicPersonSearcherKey
                searchFilter = String.Format(searchFilter, String.Format(SearcherOrCommand, String.Format(CommonNameSearcherKey, accountName), String.Format(AccountNameSearcherkey, accountName)))

                Dim directorySearcher As DirectorySearcher = New DirectorySearcher(directoryEntry) With {.SearchRoot = directoryEntry, .Filter = searchFilter}
                Dim searchResult As SearchResult = directorySearcher.FindOne()
                If searchResult Is Nothing Then Return False

                userDirToUpdate = searchResult.GetDirectoryEntry()
            End If

            userDirToUpdate.Invoke("SetPassword", New Object() {newPassword})
            userDirToUpdate.CommitChanges()
            Return True
        Finally
            If userDirToUpdate IsNot Nothing Then
                userDirToUpdate.Close()
            End If
        End Try
    End Function

    Public Shared Function CheckAdUserPassword(accountName As String, domainAddress As String, password As String) As Boolean
        Try
            Dim directoryEntryPath As String = domainAddress
            If DocSuiteContext.DomainPath.Eq("WINNT") Then
                directoryEntryPath = String.Concat(WinNtRoot, domainAddress, "/", accountName, ",user")
            End If

            Using directoryEntry As DirectoryEntry = New DirectoryEntry(directoryEntryPath, accountName, password, AuthenticationTypes.Secure)
                Dim nativeObject As Object = directoryEntry.NativeObject
                Return nativeObject IsNot Nothing
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Shared Function ImpersonateSuperUser() As Impersonator
        Dim impersonator As Impersonator = New Impersonator()
        Dim domainUserName As String = DocSuiteContext.Current.CurrentTenant.DomainUser
        Dim domainPassword As String = DocSuiteContext.Current.CurrentTenant.DomainPassword

        impersonator.ImpersonateValidUser(domainUserName, domainPassword)
        Return impersonator
    End Function

    Public Shared Function GetQueryADFormat(configuration As TenantModel) As String
        If configuration.SecurityContext = SecurityContextType.Domain Then
            Return LDAPRoot
        End If
        Return WinNtRoot
    End Function
#End Region

End Class
