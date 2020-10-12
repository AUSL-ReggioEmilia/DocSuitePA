Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Hosting
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.OData
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Tenants
Imports VecompSoftware.DocSuiteWeb.Model.Securities
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.LongTimeTask
Imports VecompSoftware.Services.Logging

Partial Public Class _default
    Inherits Page

#Region " Fields "

    Private _facade As FacadeFactory

#End Region

#Region " Properties "

    Private ReadOnly Property Facade As FacadeFactory
        Get
            If _facade Is Nothing Then
                _facade = New FacadeFactory()
            End If
            Return _facade
        End Get
    End Property

    Public ReadOnly Property LoggerName As String
        Get
            Return LogName.FileLog
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Error(sender As Object, e As EventArgs) Handles Me.Error
        CommonBasePage.CommonPageErrorHandler(Page)
    End Sub

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        ' Attenzione! E' importante che la sessione sentinella sia inizializzata nell'init della pagina di avvio.
        Session("VecompSoftware.SessionActiveCheck") = True
        If Session("CurrentTenant") Is Nothing Then
            Dim TenantFacade As TenantFacade = New TenantFacade()
            Session("CurrentTenant") = TenantFacade.GetCurrentTenant()
            Dim ODataFacade As ODataFacade = New ODataFacade()
            Session("CurrentDomainUser") = ODataFacade.domainUserModel()
        End If
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles Me.Load
        myTitle.Text = "DocSuite"
        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled AndAlso DocSuiteContext.Current.Tenants.Count > 1 Then
            myTitle.Text = String.Format("DocSuite - {0}", DocSuiteContext.Current.CurrentTenant.TenantName)
        End If

        If DocSuiteContext.Current.ProtocolEnv.RedirectToRootSite AndAlso Not String.IsNullOrEmpty(DocSuiteContext.Current.CurrentTenant.DSWUrl) Then
            Dim protocol As String = If(Request.IsSecureConnection, "https", "http")
            Dim port As String = If(Request.Url.Port = 80 OrElse (Request.IsSecureConnection AndAlso Request.Url.Port = 443), String.Empty, String.Concat(":", Request.Url.Port.ToString()))
            Dim actualSite As String = String.Format("{0}://{1}{2}{3}", protocol, Request.ServerVariables("SERVER_NAME"), port, HostingEnvironment.ApplicationVirtualPath)
            If Not actualSite.EndsWith("/") Then
                actualSite = String.Format("{0}/", actualSite)
            End If

            If Not actualSite.Eq(DocSuiteContext.Current.CurrentTenant.DSWUrl) AndAlso Not actualSite.Eq(String.Concat(DocSuiteContext.Current.CurrentTenant.DSWUrl, "/")) Then
                FileLogger.Warn(LoggerName, String.Format("Rilevato indirizzo {0} differente da quanto previsto dal valore del tenantmodel DSWUrl. Effettuato redirect su {1}", actualSite, DocSuiteContext.Current.CurrentTenant.DSWUrl))
                Response.Redirect(DocSuiteContext.Current.CurrentTenant.DSWUrl)
            End If
        End If

        Dim startPage As String = "frameset.aspx"
        Dim qs As String = Request.QueryString.ToString()
        If Not String.IsNullOrEmpty(qs) Then
            startPage = String.Format("{0}?{1}", startPage, qs)
        End If
        defaultFrame.ContentUrl = startPage

        CommonUtil.Dispose()
        CommonUtil.GetInstance.LoadUserConfiguration()
        ' SET NO CACHE
        Page.Response.Cache.SetAllowResponseInBrowserHistory(False)
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Page.Response.Cache.SetNoStore()
        Page.Response.Cache.SetValidUntilExpires(True)

        ' TODO: verificare relazione con CommonUtil.InitializePaths
        ' Home
        Dim s As String = Request.ServerVariables("PATH_INFO")
        CommonUtil.GetInstance.HomeDirectory = Left(s, InStrRev(s, "/") - 1)

        ' Verifica utente anonimo
        If String.IsNullOrEmpty(CommonUtil.UserFullName) Then
            CommonUtil.GetInstance.AppAccessOk = False
            Throw New DocSuiteException("Errore in accesso Utente", String.Format("[{0}] Verificare nella pubblicazione la 'Protezione Directory' ed eliminare l'accesso per l'Utente Anonimo.", Me.GetType.Name))
        End If

        ' Inizializza classi
        CommInitialize()

        If Not CommonUtil.GetInstance.AppAccessOk Then
            Throw New DocSuiteException("Accesso utente", "Errore in accesso utente")
        End If

        If DocSuiteContext.Current.ProtocolEnv.IsSecurityGroupEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.CheckSecurityUsersDomainEnabled Then
            Dim securityUsers As IList(Of SecurityUsers) = Facade.SecurityUsersFacade.GetUsersByAccount(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain)
            For Each item As SecurityUsers In securityUsers
                item.UserDomain = DocSuiteContext.Current.User.Domain
                Facade.SecurityUsersFacade.Update(item)
            Next

            Dim roleUsers As IList(Of RoleUser) = Facade.RoleUserFacade.GetByAccount(DocSuiteContext.Current.User.UserName)
            For Each item As RoleUser In roleUsers.Where(Function(x) Not x.Account.Equals(DocSuiteContext.Current.User.FullUserName))
                item.Account = DocSuiteContext.Current.User.FullUserName
                Facade.RoleUserFacade.Update(item)
            Next

            For Each item As CollaborationSign In Facade.CollaborationSignsFacade.GetByAccount(DocSuiteContext.Current.User.UserName).Where(Function(x) Not x.SignUser.Equals(DocSuiteContext.Current.User.FullUserName))
                item.SignUser = DocSuiteContext.Current.User.FullUserName
                Facade.CollaborationSignsFacade.Update(item)
            Next

            For Each item As CollaborationUser In Facade.CollaborationUsersFacade.GetByAccount(DocSuiteContext.Current.User.UserName).Where(Function(x) Not x.Account.Equals(DocSuiteContext.Current.User.FullUserName))
                item.Account = DocSuiteContext.Current.User.FullUserName
                Facade.CollaborationUsersFacade.Update(item)
            Next

            For Each item As Collaboration In Facade.CollaborationFacade.GetByAccount(DocSuiteContext.Current.User.UserName).Where(Function(x) Not x.RegistrationUser.Equals(DocSuiteContext.Current.User.FullUserName))
                item.RegistrationUser = DocSuiteContext.Current.User.FullUserName
                Facade.CollaborationFacade.Update(item)
            Next

            For Each item As CollaborationVersioning In Facade.CollaborationVersioningFacade.GetByAccount(DocSuiteContext.Current.User.UserName).Where(Function(x) Not x.CheckOutUser.Equals(DocSuiteContext.Current.User.FullUserName))
                item.CheckOutUser = DocSuiteContext.Current.User.FullUserName
                Facade.CollaborationVersioningFacade.Update(item)
            Next
        End If

        CommonUtil.GetInstance.InitializeUser()

        Dim currentUserName As String = DocSuiteContext.Current.User.UserName
        Dim longAction As Action = Function() CommonUtil.ClearTemporaryDirectory(CommonUtil.GetInstance.AppTempPath, currentUserName, DocSuiteContext.Current.ProtocolEnv.TimeIntervalForCleaningTemp)
        LongTimeTaskHelper.ImmediateOperation(longAction)

        InitializeSections()

        'tabella utenti SqlLog/application
        ComputerLog()
        UserLog()

        ' Pulizia dizionari 
        CommonShared.ClearRightDictionaries()

    End Sub

#End Region

#Region " Methods "

    Sub CommInitialize()
        CommonUtil.GetInstance.InitializePaths(Request)
        CommonUtil.GetInstance.AppAccessOk = True
    End Sub

    Public Sub UserLog()
        Try
            Dim finder As NHibernateUserLogFinder = New NHibernateUserLogFinder()
            finder.SystemUser = DocSuiteContext.Current.User.FullUserName
            Dim userLog As UserLog = finder.DoSearch().FirstOrDefault()
            If userLog Is Nothing Then
                Dim authorizedTenant As Tenant = New TenantFacade().GetAuthorizedTenants().FirstOrDefault()
                userLog = New UserLog With {
                    .Id = DocSuiteContext.Current.User.FullUserName,
                    .CurrentTenantId = Guid.Empty
                }
                If authorizedTenant IsNot Nothing Then
                    userLog.CurrentTenantId = authorizedTenant.UniqueId
                End If
                Facade.UserLogFacade.Save(userLog)
            End If

            Dim dayAccessNumber As Integer = 1
            Dim currentAccessDate As DateTimeOffset = DateTimeOffset.UtcNow
            If userLog.LastOperationDate.HasValue AndAlso currentAccessDate.Date = userLog.LastOperationDate.Value.Date Then
                dayAccessNumber = userLog.AccessNumber + 1
            End If

            userLog.AccessNumber = dayAccessNumber
            userLog.LastOperationDate = currentAccessDate
            userLog.PrevOperationDate = userLog.LastOperationDate
            userLog.SystemComputer = CommonUtil.GetInstance.UserComputer
            userLog.SystemServer = CommonShared.MachineName
            userLog.SessionId = CommonShared.UserSessionId

            Facade.UserLogFacade.Update(userLog)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore salvataggio user log", ex)
        End Try
    End Sub

    Public Sub ComputerLog()
        Try
            Dim cLog As ComputerLog = Facade.ComputerLogFacade.GetById(CommonShared.DSUserComputer)

            If cLog Is Nothing Then
                cLog = New ComputerLog()
                cLog.Id = CommonShared.DSUserComputer
                cLog.AdvancedViewer = DocSuiteContext.Current.ProtocolEnv.DefaultAdvancedViewer
                cLog.AdvancedScanner = DocSuiteContext.Current.ProtocolEnv.DefaultAdvancedScanner
                Facade.ComputerLogFacade.Save(cLog)
            End If

            Dim accn As Integer = 1
            Dim serverDate As Date = DateTime.Now
            If String.Format("{0:yyyyMMdd}", serverDate) = String.Format("{0:yyyyMMdd}", cLog.LastOperationDate) Then
                accn = cLog.AccessNumber + 1
            End If

            With cLog
                .AccessNumber = accn
                .LastOperationDate = serverDate
                .PrevOperationDate = cLog.LastOperationDate
                .SystemUser = DocSuiteContext.Current.User.UserName
                .SystemServer = CommonShared.MachineName
                .SessionId = CommonShared.UserSessionId
            End With
            Facade.ComputerLogFacade.Update(cLog)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore salvataggio computer log", ex)
        End Try
    End Sub

    ''' <summary> Inizializza le tre sezioni principali della DocSuite. </summary>
    Private Sub InitializeSections()
        If Not String.IsNullOrEmpty(DocSuiteContext.LocalServerWorkgroup) Then
            CommonUtil.GetInstance.AppAccessOk = True
            Exit Sub
        End If

        If Not DocSuiteContext.Current.Tenants.Any(Function(x) x.DomainName.Eq(CommonShared.UserDomain)) Then
            CommonUtil.GetInstance.AppAccessOk = False
        End If
    End Sub

#End Region

End Class
