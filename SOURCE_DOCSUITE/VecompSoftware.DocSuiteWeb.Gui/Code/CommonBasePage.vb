Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports System.Reflection
Imports System.Web
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.OData
Imports VecompSoftware.DocSuiteWeb.Facade.Interfaces
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Logging

''' <summary> Classe base delle pagine della DSW, va usata sulle master page di soluzione. </summary>
Public Class CommonBasePage
    Inherits Page
    Implements IHaveInformations

#Region " Fields "

    Private _windowBuilder As WindowBuilder = Nothing
    Public Const CONFIRM_WORKFLOW_HANDLER As String = "CompleteWorkflowHandler"
    Public Const EXTRACT_COMPRESSFILE_ERROR As String = "Impossibile estrarre il contenuto del file compresso. Il sistema non è stato in grado di leggere correttamente il file, si consiglia di procedere con l'estrazione manuale"
    Private _currentUDSRepositoryFacade As UDSRepositoryFacade = Nothing
    Private _currentODataFacade As ODataFacade = Nothing
    Public Const PRIVACY_LABEL As String = "Riservatezza/Privacy"

#End Region

#Region " Properties "
    ''' <summary> Estrae la chiave dalla querystring, dal viewstate o dalla pagina precedente. </summary>
    ''' <remarks> Oltre a leggerlo, lo salva in automatico nel viewstate in modo da renderlo disponibile nel cross post back. </remarks>
    Public Overloads Function GetKeyValue(Of T, TBasePage As CommonBasePage)(key As String) As T
        If ViewState(key) Is Nothing Then

            If HttpContext.Current.Request.QueryString(key) IsNot Nothing Then
                ViewState(key) = HttpContext.Current.Request.QueryString.GetValue(Of T)(key)
            Else
                If PreviousPage IsNot Nothing AndAlso TypeOf PreviousPage Is TBasePage Then
                    ViewState(key) = DirectCast(PreviousPage, TBasePage).GetKeyValue(Of T, TBasePage)(key)
                End If
            End If
        End If

        Return DirectCast(ViewState(key), T)
    End Function

    Public Overloads Function GetKeyValue(Of T)(key As String) As T
        Return GetKeyValue(Of T, StartWorkflow)(key)
    End Function

    Public Overloads Function GetKeyValueOrDefault(Of T)(key As String, defaultValue As T) As T
        Return Context.Request.QueryString.GetValueOrDefault(key, defaultValue)
    End Function

    Protected ReadOnly Property DSWVersion As String
        Get
            Return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion
        End Get
    End Property

    'Etichetta della privacy con la prima lettera maiuscola
    Public ReadOnly Property PrivacyLabelTitle As String
        Get
            Return String.Concat(Char.ToUpper(PRIVACY_LABEL(0)), PRIVACY_LABEL.Substring(1))
        End Get
    End Property

    Public ReadOnly Property TenantName As String
        Get
            Return DocSuiteContext.Current.ProtocolEnv.CorporateAcronym
        End Get
    End Property

    Public ReadOnly Property TenantId As Guid
        Get
            Return DocSuiteContext.Current.CurrentTenant.TenantId
        End Get
    End Property

    ''' <summary> Variabile che determina se l'utente connesso in sessione è SuperAdmin. </summary>
    Public Property SuperAdminAuthored As Boolean
        Get
            If Session("VecompSoftware.SuperAdminCheck") IsNot Nothing Then
                Return CType(Session("VecompSoftware.SuperAdminCheck"), Boolean)
            End If
            Return False
        End Get
        Set(value As Boolean)
            Session("VecompSoftware.SuperAdminCheck") = value
        End Set
    End Property

    Protected Property ChkVerificaEnabled() As Boolean
        Get
            If ViewState("ChkVerificaEnabled") Is Nothing Then
                ViewState("ChkVerificaEnabled") = True
            End If
            Return CType(ViewState("ChkVerificaEnabled"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("ChkVerificaEnabled") = value
        End Set
    End Property

    ''' <summary>Sezione in cui si trova la pagina, influenza stili e BL</summary>
    ''' <value>Nome della sezione: "Prot": Protocollo, "Docm": Pratiche, "Resl": Atti, "Comm": Comune </value>
    ''' <returns>Nome della sezione in cui lavora il controllo</returns>
    Public ReadOnly Property Type() As String
        Get
            Dim val As String
            If ViewState("type") Is Nothing Then
                val = HttpContext.Current.Request.QueryString("Type")
                ViewState("type") = val
            Else
                val = ViewState("type").ToString()
            End If
            Return val
        End Get
    End Property

    Public ReadOnly Property Env() As DSWEnvironment
        Get
            Select Case Type.ToUpperInvariant()
                Case "DOCM"
                    Return DSWEnvironment.Document
                Case "RESL"
                    Return DSWEnvironment.Resolution
                Case "SERIES"
                    Return DSWEnvironment.DocumentSeries
                Case "DOSSIER"
                    Return DSWEnvironment.Dossier
                Case Else
                    Return DSWEnvironment.Protocol
            End Select
        End Get
    End Property

    ''' <summary> Azione specifica della pagina. </summary>
    Public ReadOnly Property Action() As String
        Get
            Dim val As String
            If ViewState("Action") Is Nothing Then
                val = HttpContext.Current.Request.QueryString("Action")
                ViewState("Action") = val
            Else
                val = ViewState("Action").ToString()
            End If
            Return val
        End Get
    End Property

    ''' <summary>Istanza unica delle <see cref="CommonUtil"/></summary>
    Public ReadOnly Property CommonInstance() As CommonUtil
        Get
            Return CommonUtil.GetInstance()
        End Get
    End Property

    ''' <summary> Master page standard della docsuite. </summary>
    Public Shadows ReadOnly Property MasterDocSuite() As DocSuite2008
        Get
            Return CType(Master, DocSuite2008)
        End Get
    End Property

    ''' <summary> Torna il manager delle chiamate AJAX per la pagina attuale. </summary>
    Public ReadOnly Property AjaxManager() As RadAjaxManager
        Get
            Return RadAjaxManager.GetCurrent(Page)
        End Get
    End Property

    Public Overridable ReadOnly Property Facade() As FacadeFactory
        Get
            Select Case Type
                Case "Prot"
                    Return New FacadeFactory("ProtDB")
                Case "Docm"
                    Return New FacadeFactory("DocmDB")
                Case "Resl"
                    Return New FacadeFactory("ReslDB")
                Case Else
                    Return New FacadeFactory("ProtDB")
            End Select
        End Get
    End Property

    Public ReadOnly Property WindowBuilder() As WindowBuilder
        Get
            If _windowBuilder Is Nothing Then
                _windowBuilder = New WindowBuilder(AjaxManager)
            End If
            Return _windowBuilder
        End Get
    End Property

    ''' <summary> Nome dell'Appender per i log di default. </summary>
    Public Shared ReadOnly Property LoggerName As String
        Get
            Return LogName.FileLog
        End Get
    End Property

    Protected ReadOnly Property ProtocolEnv() As ProtocolEnv
        Get
            Return DocSuiteContext.Current.ProtocolEnv
        End Get
    End Property

    Protected ReadOnly Property DocumentEnv() As DocumentEnv
        Get
            Return DocSuiteContext.Current.DocumentEnv
        End Get
    End Property

    Protected ReadOnly Property ResolutionEnv() As ResolutionEnv
        Get
            Return DocSuiteContext.Current.ResolutionEnv
        End Get
    End Property

    ''' <summary> Indirizzo pagina di provenienza. </summary>
    Protected Friend Property PreviousPageUrl As String
        Get
            Return CType(ViewState("previousPage"), String)
        End Get
        Set(value As String)
            ViewState("previousPage") = value
        End Set
    End Property

    ''' <summary> Contiene la definizione di un messaggio da mostrare all'utente tramite la pagina di visualizzazione informazioni </summary>
    Public Property InformationsMessage() As InformationsMessage Implements IHaveInformations.InformationsMessage

    Public ReadOnly Property CurrentUDSRepositoryFacade As UDSRepositoryFacade
        Get
            If _currentUDSRepositoryFacade Is Nothing Then
                _currentUDSRepositoryFacade = New UDSRepositoryFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _currentUDSRepositoryFacade
        End Get
    End Property
    Public ReadOnly Property CurrentODataFacade As ODataFacade
        Get
            If _currentODataFacade Is Nothing Then
                _currentODataFacade = New ODataFacade()
                Return _currentODataFacade
            End If
            Return _currentODataFacade
        End Get
    End Property

    Public Property CurrentTenant As Tenant
        Get
            If Session("CurrentTenant") IsNot Nothing Then
                Return DirectCast(Session("CurrentTenant"), Tenant)
            End If
            Return Nothing
        End Get
        Set(value As Tenant)
            Session("CurrentTenant") = value
        End Set
    End Property
#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try

            If ProtocolEnv.CheckSession AndAlso Not IsSessionActive() Then
                FileLogger.Info("Session", "REDIRECT FROM (BASE): " & Request.RawUrl)
                Throw New ExpiredSessionStateException("La sessione è scaduta.")
            End If
            If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
                If Request IsNot Nothing AndAlso Request.UrlReferrer IsNot Nothing Then
                    PreviousPageUrl = Request.UrlReferrer.ToString()
                End If
            End If

        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            Throw ex
        End Try

    End Sub

    Private Sub Page_Error(sender As Object, e As EventArgs) Handles Me.Error
        CommonPageErrorHandler(Page)
    End Sub

#End Region

#Region " Methods "

    Protected Sub SetResponseNoCache()
        Page.Response.Cache.SetAllowResponseInBrowserHistory(False)
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Page.Response.Cache.SetNoStore()
        Page.Response.Cache.SetValidUntilExpires(True)
    End Sub

    ''' <summary> Verifica che la sessione non sia scaduta. </summary>
    Protected Function IsSessionActive() As Boolean
        'Return HttpContext.Current.User.Identity.IsAuthenticated

        If Session("VecompSoftware.SessionActiveCheck") Is Nothing Then
            Return False
        End If
        Dim parsed As Boolean? = CType(Session("VecompSoftware.SessionActiveCheck"), Boolean?)
        Return parsed.GetValueOrDefault(False)

    End Function

    Public Sub GridExport()
        Dim cookieExportGrid As HttpCookie = Request.Cookies("ExportGridID")
        If cookieExportGrid Is Nothing Then
            Exit Sub
        End If

        Dim cookieExportType As HttpCookie = Request.Cookies("ExportGridType")
        Dim grid As BindGrid = CType(WebHelper.FindControlRecursive(MasterDocSuite, cookieExportGrid.Value), BindGrid)
        If (grid Is Nothing) OrElse (cookieExportType Is Nothing) Then
            Exit Sub
        End If

        Dim exportType As String = cookieExportType.Value
        Request.Cookies.Remove("ExportGridID")
        Request.Cookies.Remove("ExportGridType")
        Select Case exportType
            Case "Excel"
                grid.MasterTableView.ExportToExcel()
            Case "Word"
                grid.MasterTableView.ExportToExcel()
            Case "Pdf"
                grid.MasterTableView.ExportToExcel()
        End Select
    End Sub

    Public Function ConvertRelativeUrlToAbsoluteUrl(relativeUrl As String) As String
        If Request.IsSecureConnection Then
            Return String.Format("https://{0}{1}", Request.Url.Host, ResolveUrl(relativeUrl))
        Else
            Return String.Format("http://{0}{1}", Request.Url.Host, ResolveUrl(relativeUrl))
        End If
    End Function

    Public Sub ClearSessions(Of T As Page)()
        If Session Is Nothing Then
            Return
        End If

        Dim prefix As String = String.Format("{0}.", GetType(T).FullName)
        Dim keys As IEnumerable(Of String) = Session.Keys.Cast(Of String).Where(Function(k) k.StartsWith(prefix))
        keys.ToList().ForEach(Sub(k) Session.Item(k) = Nothing)
    End Sub




    ''' <summary> Gestore delle eccezioni delle pagine della GUI </summary>
    ''' <param name="page"> Pagina nella quale è avvenuta l'eccezione </param>
    Public Shared Sub CommonPageErrorHandler(ByRef page As Page)
        ' Estrazione dell'eccezione
        Dim ex As Exception = page.Server.GetLastError()
        If ex Is Nothing Then
            Exit Sub
        End If
        ' Completo i dati inerenti l'eccezione
        If ex.Data.Item("Url") Is Nothing Then
            ex.Data.Item("Url") = HttpContext.Current.Request.Url
        End If
        If ex.Data.Item("User") Is Nothing AndAlso HttpContext.Current.User IsNot Nothing Then
            ex.Data.Item("User") = HttpContext.Current.User.Identity.Name
        End If
        ' Gestione dell'errore prima di passare alla pagina
        Select Case ex.GetType()
            Case GetType(HttpRequestValidationException)
                FileLogger.Info(LoggerName, "Errore validazione input", ex)
                ex = New DocSuiteException("Errore validazione input", "Alcuni dati inseriti non sono validi", ex)

            Case GetType(ExpiredSessionStateException)
                ' Passo l'errore a livello applicazione
                Throw ex

            Case GetType(DocSuiteException)
                FileLogger.Warn(LoggerName, "Errore pagina gestito", ex)

            Case Else
                FileLogger.Error(LoggerName, "Errore pagina non previsto", ex)
                ' Invio mail di notifica errore
                FileLogger.Error(GlobalAsax.LoggerNameMail, "Errore pagina non previsto", ex)

        End Select
        ' Passo alla pagina senza far arrivare l'errore a livello application
        page.Server.ClearError()
        page.Session("BasePageError") = ex
        page.Response.Redirect("~/ErrorPage.aspx")
    End Sub

    Public Function GetDbName(environment As DSWEnvironment) As String
        Dim dbName As String = String.Empty
        Select Case environment
            Case DSWEnvironment.Protocol
                dbName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)
            Case DSWEnvironment.Document
                dbName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.DocmDB)
            Case DSWEnvironment.Resolution
                dbName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ReslDB)
        End Select
        Return dbName
    End Function

    Public Function FillComboBoxDocumentUnitNames(Optional genericUDS As Boolean = False) As IEnumerable(Of RadComboBoxItem)
        Dim results As ICollection(Of RadComboBoxItem) = New List(Of RadComboBoxItem)()
        results.Add(New RadComboBoxItem(String.Empty, String.Empty))
        results.Add(New RadComboBoxItem("Protocollo", "Protocollo"))
        'results.Add(New RadComboBoxItem("Serie documentale", "Serie documentale"))

        If DocSuiteContext.Current.IsResolutionEnabled Then
            results.Add(New RadComboBoxItem(Facade.ResolutionTypeFacade.DeterminaCaption(), Facade.ResolutionTypeFacade.DeterminaCaption()))
            results.Add(New RadComboBoxItem(Facade.ResolutionTypeFacade.DeliberaCaption(), Facade.ResolutionTypeFacade.DeliberaCaption()))
        End If

        If Not genericUDS Then
            For Each item As UDSRepository In CurrentUDSRepositoryFacade.GetActiveRepositories()
                results.Add(New RadComboBoxItem(String.Concat("Archivio ", item.Name), item.Name))
            Next
        Else
            results.Add(New RadComboBoxItem("Archivio", "Archivio"))
        End If

        Return results.OrderBy(Function(f) f.Text)
    End Function

    Public Function GetLabel(user As SecurityUsers) As String
        If DocSuiteContext.Current.ProtocolEnv.IsSecurityGroupEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled Then
            Return String.Format("{0}\{1} - ({2})", user.UserDomain, user.Account, user.Description)
        End If
        Return String.Format("{0} - ({1})", user.Account, user.Description)
    End Function


#End Region

#Region " Ajax Alert "

    ''' <summary>Visualizza messaggio di errore popup in javascript</summary>
    ''' <param name="message">Messaggio d'errore composto</param>
    ''' <param name="args">Array di <see>Object</see> contenente zero o più argomenti da formattare</param>
    Public Sub AjaxAlert(ByVal message As String, ByVal ParamArray args() As Object)
        AjaxAlert(String.Format(message, args), True)
    End Sub

    ''' <summary> Visualizza messaggio di errore gestito in javascript. </summary>
    ''' <param name="exception"> Viene preso solo il messaggio. </param>
    ''' <remarks> Tipicamente siamo sicuri di avere messaggi leggibili solo delle DocSuiteException, non usare tutte le eccezioni. </remarks>
    Public Sub AjaxAlert(ByVal exception As DocSuiteException)
        FileLogger.Error(LoggerName, exception.Message, exception)
        AjaxAlert(exception.Message, True)
    End Sub

    ''' <summary>Visualizza messaggio di errore popup in javascript</summary>
    ''' <param name="message">Messaggio d'errore</param>
    Public Sub AjaxAlert(ByVal message As String)
        AjaxAlert(message, True)
    End Sub

    ''' <summary>Metodo che esegue l'alert</summary>
    ''' <param name="message">messaggio da mandare</param>
    ''' <param name="checkJavascript">Indica se filtrare il messaggio per evitare caratteri che invalidano il javascript</param>
    Private Sub AjaxAlert(ByVal message As String, ByVal checkJavascript As Boolean)
        If checkJavascript Then
            message = StringHelper.ReplaceAlert(message)
        End If

        If AjaxManager IsNot Nothing Then
            AjaxManager.Alert(message)
        Else
            WebHelper.Alert(Me, message)
        End If
    End Sub

    Public Sub AjaxAlertConfirm(ByVal message As String, ByVal scriptOnConfirm As String, ByVal scriptBeforeConfirm As String)
        AjaxAlertConfirm(message, scriptOnConfirm, scriptBeforeConfirm, False)
    End Sub

    Public Sub AjaxAlertConfirm(ByVal message As String, ByVal scriptOnConfirm As String, ByVal scriptBeforeConfirm As String, ByVal checkJavascript As Boolean)
        AjaxAlertConfirmAndDeny(message, scriptOnConfirm, String.Empty, scriptBeforeConfirm, checkJavascript)
    End Sub

    Public Sub AjaxAlertConfirmAndDeny(ByVal message As String, ByVal scriptOnConfirm As String, ByVal scriptOnDeny As String, ByVal scriptBeforeConfirm As String, ByVal checkJavascript As Boolean)
        ' TODO: Non si riesce a creare un'overload compatibile con la gestione principale degli alert fatta con le rad?

        If checkJavascript Then
            message = HttpUtility.JavaScriptStringEncode(message)
        End If

        Dim script As String = String.Format("function AlertConfirm() {{{0} if (confirm('{1}')){{{2}}} else {{{3}}} return true; }} AlertConfirm();", scriptBeforeConfirm, message, scriptOnConfirm, scriptOnDeny)
        AjaxManager.ResponseScripts.Add(script)
    End Sub

    Public Sub AjaxPrompt(message As String, callbackFunction As String)
        message = HttpUtility.JavaScriptStringEncode(message)

        Dim script As String = String.Format("radprompt('{0}', {1});", message, callbackFunction)
        AjaxManager.ResponseScripts.Add(script)
    End Sub
#End Region

End Class
