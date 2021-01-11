Imports System.Configuration
Imports System.IO
Imports System.Reflection
Imports System.Web
Imports System.Web.Optimization
Imports HibernatingRhinos.Profiler.Appender.NHibernate
Imports Limilabs.Mail.Licensing
Imports NHibernate
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.Services.Logging

Public Class GlobalAsax
    Inherits HttpApplication

#Region " Fields "

    Private Shared _lrtask As MultiStepLongRunningTask = Nothing
    Public Const LoggerNameSession As String = "Session"
    Public Const LoggerNameMail As String = "Mail"
    Public Const LoggerName As String = LogName.FileLog

#End Region

#Region " Properties "

    Public Shared ReadOnly Property LongRunningTask() As MultiStepLongRunningTask
        Get
            If _lrtask Is Nothing Then
                _lrtask = New MultiStepLongRunningTask
            End If
            Return _lrtask
        End Get
    End Property

#End Region

#Region " Events "

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)

        BundleConfig.RegisterBundles(BundleTable.Bundles)
        FileLogger.Initialize()

        If ConfigurationManager.AppSettings.GetValueOrDefault(Of Boolean)("rhino", False) Then
            NHibernateProfiler.Initialize()
        End If

        If ConfigurationManager.AppSettings.GetValueOrDefault(Of Boolean)("limilabs", False) Then
            validateLimilabsLicense()
        End If

        ApplyTenantListenerCallback()

        VecompSoftware.Services.StampaConforme.Service.InitializeSignatureTemplateXml(DocSuiteContext.Current.ProtocolEnv.SignatureTemplate)

        Dim ass As Assembly = Assembly.GetExecutingAssembly()
        FileLogger.Info(LoggerName, String.Format("START [{0}] Versione [{1}]", ass.GetName().Name, ass.GetName().Version.ToString(4)))

        AddHandler PostAuthenticateRequest, AddressOf Application_PostAuthenticateRequest
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)

    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)

    End Sub

    Private Sub Application_PostAuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        FileLogger.SetLogicalThreadProperty("dswUser", DocSuiteContext.Current.User.FullUserName)
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        Dim ass As Assembly = Assembly.GetExecutingAssembly()
        FileLogger.Info(LoggerName, String.Format("STOP [{0}] Versione [{1}]", ass.GetName().Name, ass.GetName().Version.ToString(4)))
        NHibernateSessionManager.Instance.ClearFactories()
    End Sub

    ''' <summary> Gestisce gli errori applicativi scappati alla CommonBasePage </summary>
    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Estrazione dell'eccezione
        Dim ex As Exception = Server.GetLastError()
        ex = If(ex.InnerException IsNot Nothing, ex.InnerException, ex)

        ' Completo i dati inerenti l'eccezione
        If ex.Data.Item("Url") Is Nothing Then
            ex.Data.Item("Url") = Context.Request.Url
        End If
        If ex.Data.Item("User") Is Nothing AndAlso HttpContext.Current.User IsNot Nothing Then
            ex.Data.Item("User") = HttpContext.Current.User.Identity.Name
        End If

        ' Log dell'errore prima di passare alla pagina
        Select Case ex.GetType()
            Case GetType(ExpiredSessionStateException)
                FileLogger.Info(LoggerNameSession, String.Format("SESSIONE SCADUTA [{0}]", Session.SessionID), ex)

            Case GetType(DocSuiteException)
                FileLogger.Warn(LoggerName, "Errore applicativo gestito", ex)

            Case Else
                FileLogger.Error(LoggerName, "Errore applicativo non previsto", ex)
                ' Invio mail di notifica errore
                FileLogger.Error(LoggerNameMail, "Errore applicativo non previsto", ex)

        End Select
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Eseguo il log di avvio nuova sessione
        FileLogger.Info(LoggerNameSession, "Avvio nuova sessione: " & Session.SessionID)
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        FileLogger.Info(LoggerNameSession, "Chiusura sessione: " & Session.SessionID)
    End Sub

    Private Sub GlobalAsax_EndRequest(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.EndRequest
        NHibernateSessionManager.Instance.CloseTransactionAndSessions()
    End Sub

#End Region

#Region " Methods "

    ''' <summary> Verifica la validità del file di licenza. </summary>
    ''' <remarks> MailLicense.xml di Mail.dll (www.limilabs.com) </remarks>
    Private Sub validateLimilabsLicense()
        Dim license As String = LicenseHelper.GetLicensePath
        Dim status As LicenseStatus = LicenseHelper.GetLicenseStatus

        If Not File.Exists(license) Then
            Throw New DocSuiteException("File.Exists - Licenza MailLicense.xml non trovata: " & license)
        End If

        Select Case status
            Case LicenseStatus.Invalid
                Throw New DocSuiteException("Licenza MailLicense.xml non valida.")
            Case LicenseStatus.NoLicenseFile
                Throw New DocSuiteException("Licenza MailLicense.xml non trovata: " & license)
            Case LicenseStatus.Valid
                'NOOP - Throw New Exception("Licenza MailLicense.xml valida")
            Case Else
                Throw New DocSuiteException("Stato licenza MailLicense.xml sconosciuto")
        End Select
    End Sub

    Private Sub ApplyTenantListenerCallback()
        Dim instance As DocSuiteTenantListener = New DocSuiteTenantListener()
        EventListenerUtil.CustomEventInstances.Add(NameOf(TenantInsertEventListener), Sub(obj As ISupportTenant) instance.Handle(obj))

        Dim defaultFilterInstance As DocSuiteTenantDefaultFilterListener = New DocSuiteTenantDefaultFilterListener()
        NHibernateSessionUtil.ApplyFilterActions.Add(Sub(session As ISession) defaultFilterInstance.Handle(session))

        Dim needTenantInstance As DocSuiteNeedTenantListener = New DocSuiteNeedTenantListener()
        FacadeUtil.NeedTenantAction = Sub(lambda As Action(Of Tenant)) needTenantInstance.Handle(lambda)
    End Sub

#End Region

End Class