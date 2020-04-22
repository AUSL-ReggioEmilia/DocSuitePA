Imports System.Configuration
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager
Imports System.Web
Imports Newtonsoft.Json
Imports System.IO
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports VecompSoftware.DocSuiteWeb.Model.WebAPI.Client
Imports System.Text
Imports System.Web.Hosting
Imports VecompSoftware.DocSuiteWeb.Entity.Commons

Public Class DocSuiteContext

#Region "Thread-safe, lazy Singleton"

    Public Shared ReadOnly Property Current() As DocSuiteContext
        Get
            Try

                Return Nested.Context
            Catch ex As Exception
                ' Salto l'eccezione TypeInitializationException, che nasconde il vero errore sottostante
                If TypeOf ex Is TypeInitializationException AndAlso ex.InnerException IsNot Nothing Then
                    ex = ex.InnerException
                End If
                Throw New DocSuiteException("Errore inizializzazione", "Impossibile inizializzare il contesto.", ex)
            End Try
        End Get
    End Property

    Private Sub New()
        InitContext(False)
    End Sub

    Public Sub Reset()
        InitContext(False)
    End Sub

    Private Class Nested
        Shared Sub New()

        End Sub
        Friend Shared ReadOnly Context As New DocSuiteContext()
    End Class

#End Region

#Region " Fields "

    Public Const Program As String = "D8"

    Private Const CONFIGURATION_MENUJSONCONFIG_FILE_PATH As String = "~/Config/DocSuiteMenuConfig.json"
    Private Const CONFIGURATION_RICERCAFLUSSOJSONCONFIG_FILE_PATH As String = "~/Config/RicercaFlussoConfig.json"

    Private _isCustomInstance As Boolean?
    Private _protocolEnv As ProtocolEnv
    Private _resolutionEnv As ResolutionEnv
    Private _documentEnv As DocumentEnv
    Private _lastUpdate As Date
    Private _tenantsModels As IReadOnlyCollection(Of TenantModel) = Nothing
    Private _docSuiteMenuConfiguration As IDictionary(Of String, MenuNodeModel) = Nothing
    Private _docSuiteWorkflowOperation As WorkflowOperationConfig = Nothing
    Private _ricercaFlussoConfiguration As IDictionary(Of String, Dictionary(Of String, String)) = Nothing
    Private _protocolDefaultAdaptiveSearchConfigurations As ICollection(Of AdaptiveSearchMappingControl)

    Private Shared _jsonUDSSerializerSettings As JsonSerializerSettings = New JsonSerializerSettings() With {
        .PreserveReferencesHandling = PreserveReferencesHandling.Objects
    }

    Private Shared _jsonSerializerSettings As JsonSerializerSettings = New JsonSerializerSettings() With {
        .NullValueHandling = NullValueHandling.Ignore,
        .TypeNameHandling = TypeNameHandling.All,
        .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    }

    Private Shared _jsonWebAPISerializerSettings As JsonSerializerSettings = New JsonSerializerSettings() With {
        .NullValueHandling = NullValueHandling.Ignore,
        .TypeNameHandling = TypeNameHandling.Objects,
        .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        .PreserveReferencesHandling = PreserveReferencesHandling.All
    }

    Private Shared _currentPrivacyLevels As ICollection(Of PrivacyLevel) = Nothing
    Private Shared _invoiceResources As ICollection(Of InvoiceResources)
    Private _enableAttachmentByPage As IDictionary(Of String, Boolean) = Nothing
#End Region

#Region " Properties "

    Public Property ProtocolEnv() As ProtocolEnv
        Get
            Return _protocolEnv
        End Get
        Set(ByVal value As ProtocolEnv)
            _protocolEnv = value
        End Set
    End Property

    Public Property ResolutionEnv() As ResolutionEnv
        Get
            Return _resolutionEnv
        End Get
        Set(ByVal value As ResolutionEnv)
            _resolutionEnv = value
        End Set
    End Property

    Public Property DocumentEnv() As DocumentEnv
        Get
            Return _documentEnv
        End Get
        Set(ByVal value As DocumentEnv)
            _documentEnv = value
        End Set
    End Property

    Public ReadOnly Property User() As DocSuiteUser
        Get
            Return New DocSuiteUser()
        End Get
    End Property

    ''' <summary> Indica se l'istanza corrente è personalizzata. </summary>
    ''' <remarks> Nata per gestire parametri personalizzati. </remarks>
    Public ReadOnly Property IsCustomInstance() As Boolean
        Get
            If Not _isCustomInstance.HasValue Then
                _isCustomInstance = Not String.IsNullOrWhiteSpace(CustomInstanceName)
            End If
            Return _isCustomInstance.Value
        End Get
    End Property
    Public Shared ReadOnly Property DefaultJsonSerializerSettings As JsonSerializerSettings
        Get
            Return _jsonSerializerSettings
        End Get
    End Property

    Public Shared ReadOnly Property DefaultWebAPIJsonSerializerSettings As JsonSerializerSettings
        Get
            Return _jsonWebAPISerializerSettings
        End Get
    End Property

    Public Shared ReadOnly Property DefaultUDSJsonSerializerSettings As JsonSerializerSettings
        Get
            Return _jsonUDSSerializerSettings
        End Get
    End Property
    Public Shared ReadOnly Property SignalRServerAddress As String
        Get
            Return DocSuiteContext.Current.CurrentTenant.SignalRAddress
        End Get
    End Property

    ''' <summary> Indica se l'istanza corrente ha parametri personalizzati. </summary>
    ''' <remarks> Nata per gestire parametri personalizzati. </remarks>
    Public Shared ReadOnly Property CustomInstanceName() As String
        Get
            Return ConfigurationManager.AppSettings.Item("CustomInstanceName")
        End Get
    End Property

    Public ReadOnly Property IsLogEnable() As Boolean
        Get
            Return (IsProtocolEnabled AndAlso ProtocolEnv.IsLogEnabled) _
                OrElse (IsDocumentEnabled AndAlso DocumentEnv.IsEnvLogEnabled) _
                OrElse (IsResolutionEnabled AndAlso ResolutionEnv.IsLogEnabled)
        End Get
    End Property

    Public Shared ReadOnly Property IsFullApplication() As Boolean
        Get
            Return ConfigurationManager.AppSettings.Item("FullVersion").Eq("1")
        End Get
    End Property

    Public Shared ReadOnly Property ExternalAppTempPath As String
        Get
            Return ConfigurationManager.AppSettings.Item("DocSuiteWeb.Biblos.TempFolder")
        End Get
    End Property

    Public ReadOnly Property IsProtocolEnabled() As Boolean
        Get
            Return NHibernateSessionManager.Instance.SessionConnectionStrings("ProtConnection") IsNot Nothing
        End Get
    End Property

    Public ReadOnly Property IsResolutionEnabled() As Boolean
        Get
            Return NHibernateSessionManager.Instance.SessionConnectionStrings("ReslConnection") IsNot Nothing
        End Get
    End Property

    Public ReadOnly Property DossierAndPraticheLabel As String
        Get
            Return If(ProtocolEnv.PraticheEnabled AndAlso Not ProtocolEnv.DossierEnabled, "Pratiche", If(Not ProtocolEnv.PraticheEnabled AndAlso ProtocolEnv.DossierEnabled, "Dossier", "Dossier e Pratiche"))
        End Get
    End Property

    Public ReadOnly Property IsDocumentEnabled() As Boolean
        Get
            Return NHibernateSessionManager.Instance.SessionConnectionStrings("DocmConnection") IsNot Nothing
        End Get
    End Property

    ''' <summary> Nome del prodotto. </summary>
    Public Shared ReadOnly Property ProductName() As String
        Get
            Dim name As String = ConfigurationManager.AppSettings("DocSuite.ProductName")
            If String.IsNullOrEmpty(name) Then
                name = "DocSuite"
            End If
            Return name
        End Get
    End Property

    Public Shared ReadOnly Property PecSegnature() As String
        Get
            Return ConfigurationManager.AppSettings("DocSuite.PEC.Segnature")
        End Get
    End Property

    Public Shared ReadOnly Property LocalServerName() As String
        Get
            Return ConfigurationManager.AppSettings.Item("LocalServerName")
        End Get
    End Property

    Public Shared ReadOnly Property LocalServerWorkgroup() As String
        Get
            Return ConfigurationManager.AppSettings.Item("LocalServerWorkgroup")
        End Get
    End Property

    Public Shared ReadOnly Property RadGridLocalizeConfiguration As String
        Get
            Return My.Resources.RadGridLocalizeConfig
        End Get
    End Property

    Public ReadOnly Property ProtocolDefaultAdaptiveSearchConfigurations As ICollection(Of AdaptiveSearchMappingControl)
        Get
            If _protocolDefaultAdaptiveSearchConfigurations Is Nothing Then
                _protocolDefaultAdaptiveSearchConfigurations = JsonConvert.DeserializeObject(Of ICollection(Of AdaptiveSearchMappingControl))(My.Resources.ProtocolAdaptiveSearchMapping)
            End If
            Return _protocolDefaultAdaptiveSearchConfigurations
        End Get
    End Property

    Private Function RecursiveMenuReplacing(origin As MenuNodeModel, source As MenuNodeModel) As MenuNodeModel
        If source Is Nothing Then
            Return origin
        End If

        origin.Name = source.Name
        If source.Nodes Is Nothing Then
            Return origin
        End If

        For Each item As KeyValuePair(Of String, MenuNodeModel) In source.Nodes
            If origin.Nodes.ContainsKey(item.Key) Then
                origin.Nodes(item.Key) = RecursiveMenuReplacing(origin.Nodes(item.Key), item.Value)
            End If
        Next
        Return origin
    End Function


    Public ReadOnly Property RicercaFlussoConfiguration As IDictionary(Of String, Dictionary(Of String, String))
        Get
            If _ricercaFlussoConfiguration Is Nothing Then
                Dim defaultJsonConfigFile As String = HostingEnvironment.MapPath(CONFIGURATION_RICERCAFLUSSOJSONCONFIG_FILE_PATH)
                Dim jsonRicercaFlussoContent As String = File.ReadAllText(defaultJsonConfigFile, Encoding.UTF8)
                _ricercaFlussoConfiguration = JsonConvert.DeserializeObject(Of Dictionary(Of String, Dictionary(Of String, String)))(jsonRicercaFlussoContent)
            End If

            Return _ricercaFlussoConfiguration
        End Get
    End Property

    Public ReadOnly Property DocSuiteMenuConfiguration As IDictionary(Of String, MenuNodeModel)
        Get
            If _docSuiteMenuConfiguration Is Nothing Then

                Dim defaultJsonConfigFile As String = HostingEnvironment.MapPath(CONFIGURATION_MENUJSONCONFIG_FILE_PATH)
                Dim currentTenantJsonConfigFile As String = HostingEnvironment.MapPath(String.Concat("~/Config/DocSuiteMenuConfig_", CurrentTenant.TenantName, ".json"))
                Dim jsonMenuContent As String = File.ReadAllText(defaultJsonConfigFile, Encoding.UTF8)
                _docSuiteMenuConfiguration = JsonConvert.DeserializeObject(Of Dictionary(Of String, MenuNodeModel))(jsonMenuContent)

                If File.Exists(currentTenantJsonConfigFile) Then
                    Dim tenantJsonMenuContent As String = File.ReadAllText(currentTenantJsonConfigFile, Encoding.UTF8)
                    Dim tenantMenuConfigurations As Dictionary(Of String, MenuNodeModel) = JsonConvert.DeserializeObject(Of Dictionary(Of String, MenuNodeModel))(tenantJsonMenuContent)
                    If tenantMenuConfigurations IsNot Nothing Then
                        For Each item As KeyValuePair(Of String, MenuNodeModel) In tenantMenuConfigurations
                            If _docSuiteMenuConfiguration.ContainsKey(item.Key) Then
                                _docSuiteMenuConfiguration(item.Key) = RecursiveMenuReplacing(_docSuiteMenuConfiguration(item.Key), item.Value)
                            End If
                        Next
                    End If
                End If
            End If

            Return _docSuiteMenuConfiguration
        End Get
    End Property

    Public ReadOnly Property DocSuiteWorkflowOperation As WorkflowOperationConfig
        Get
            If _docSuiteWorkflowOperation Is Nothing Then
                _docSuiteWorkflowOperation = JsonConvert.DeserializeObject(Of WorkflowOperationConfig)(My.Resources.WorkflowOperationConfig)
            End If

            Return _docSuiteWorkflowOperation
        End Get
    End Property

    ''' <summary>
    ''' IP della macchina usata dall'utente corrente
    ''' </summary>
    ''' <returns>Se <see>HttpContext</see> o la variabile <see>REMOTE_ADDR</see> sono vuote viene impostato a <code>127.0.0.1</code></returns>
    ''' <remarks>
    ''' Non ha senso in VecompSoftware.NHibernateManager richiamare HttpContext.Current o WindowsIdentity.GetCurrent.
    ''' Trovare un modo per richiamare questo valore in Facade e passarla alla VecompSoftware.NHibernateManager nel DocSuiteUser.
    ''' </remarks>
    Public ReadOnly Property UserComputer() As String
        Get
            ' TODO: spostare in Docsuiteuser
            If HttpContext.Current Is Nothing OrElse String.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")) Then
                Return "127.0.0.1"
            Else
                Return HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
            End If
        End Get
    End Property

    Public Shared ReadOnly Property DomainPath() As String
        Get
            Dim sTest As String = ConfigurationManager.AppSettings.Item("DomainPath")
            Return If(String.IsNullOrEmpty(sTest), "LDAP", sTest)
        End Get
    End Property

    Public ReadOnly Property HasInfocertProxySign As Boolean
        Get

            Return Not String.IsNullOrEmpty(InfocertProxySignUrl)
        End Get
    End Property

    Public ReadOnly Property InfocertProxySignUrl As String
        Get
            If ConfigurationManager.AppSettings.Item("Signer.Infocert.ProxySign") IsNot Nothing Then
                Return ConfigurationManager.AppSettings.Item("Signer.Infocert.ProxySign")
            End If
            Return String.Empty
        End Get
    End Property
    Public ReadOnly Property HasArubaActalisSign As Boolean
        Get

            Return Not String.IsNullOrEmpty(ArubaActalisSignUrl)
        End Get
    End Property

    Public ReadOnly Property ArubaActalisSignUrl As String
        Get
            If ConfigurationManager.AppSettings.Item("Signer.Aruba.Actalis") IsNot Nothing Then
                Return ConfigurationManager.AppSettings.Item("Signer.Aruba.Actalis")
            End If
            Return String.Empty
        End Get
    End Property


    Public ReadOnly Property DefaultODataTopQuery As Integer
        Get
            If ConfigurationManager.AppSettings.Item("DocSuite.Default.ODATA.Finder.TopQuery") IsNot Nothing Then
                Return Convert.ToInt32(ConfigurationManager.AppSettings.Item("DocSuite.Default.ODATA.Finder.TopQuery"))
            End If
            Return 500
        End Get
    End Property

    Public ReadOnly Property LastUpdate() As Date
        Get
            Return _lastUpdate
        End Get
    End Property

    Public ReadOnly Property Tenants As IReadOnlyCollection(Of TenantModel)
        Get
            If _tenantsModels Is Nothing Then
                _tenantsModels = Current.ProtocolEnv.TenantModels.AsReadOnly()
                Dim restEndpoints As HttpClientConfiguration = JsonConvert.DeserializeObject(Of HttpClientConfiguration)(Clients.WebAPI.Properties.Resources.RestEndpoints, DefaultJsonSerializerSettings)
                Dim originalEndpoints As HttpClientConfiguration = JsonConvert.DeserializeObject(Of HttpClientConfiguration)(Clients.WebAPI.Properties.Resources.RestEndpoints, DefaultJsonSerializerSettings)
                originalEndpoints.Addresses = Current.CurrentTenant.WebApiClientConfig.Addresses
                Dim odataCurrentTenantEndpoints As Dictionary(Of String, TenantEntityConfiguration) = JsonConvert.DeserializeObject(Of Dictionary(Of String, TenantEntityConfiguration))(Clients.WebAPI.Properties.Resources.OdataEndpoints, DefaultJsonSerializerSettings)
                Dim odataExternalTenatEndpoints As Dictionary(Of String, TenantEntityConfiguration) = JsonConvert.DeserializeObject(Of Dictionary(Of String, TenantEntityConfiguration))(Clients.WebAPI.Properties.Resources.OdataEndpoints, DefaultJsonSerializerSettings)
                For Each item As KeyValuePair(Of String, TenantEntityConfiguration) In odataExternalTenatEndpoints
                    item.Value.IsActive = False
                Next
                For Each tenatModel As TenantModel In _tenantsModels.Where(Function(f) f.CurrentTenant)
                    tenatModel.WebApiClientConfig.EndPoints = restEndpoints.EndPoints
                    tenatModel.OriginalConfiguration = originalEndpoints
                    tenatModel.Entities = odataCurrentTenantEndpoints
                Next
                For Each tenatModel As TenantModel In _tenantsModels.Where(Function(f) Not f.CurrentTenant)
                    tenatModel.WebApiClientConfig.EndPoints = restEndpoints.EndPoints
                    tenatModel.OriginalConfiguration = originalEndpoints
                    tenatModel.Entities = odataExternalTenatEndpoints
                Next
            End If

            Return _tenantsModels
        End Get
    End Property

    Public ReadOnly Property CurrentPrivacyLevels As ICollection(Of PrivacyLevel)
        Get
            Return _currentPrivacyLevels
        End Get
    End Property

    Public ReadOnly Property HasPrivacyLevels As Boolean
        Get
            Return CurrentPrivacyLevels IsNot Nothing AndAlso CurrentPrivacyLevels.Any()
        End Get
    End Property

    Public ReadOnly Property CurrentTenant As TenantModel
        Get
            Return Tenants.SingleOrDefault(Function(x) x.CurrentTenant)
        End Get
    End Property

    Public ReadOnly Property CurrentDomainName() As String
        Get
            If CurrentTenant IsNot Nothing Then
                Return CurrentTenant.DomainName
            End If
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property DocumentPrivacyEnabled As Boolean
        Get
            If ProtocolEnv.PrivacyTypology.HasValue AndAlso ProtocolEnv.PrivacyTypology.Value = PrivacyType.Document Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property ChainPrivacyEnabled As Boolean
        Get
            If ProtocolEnv.PrivacyTypology.HasValue AndAlso ProtocolEnv.PrivacyTypology.Value = PrivacyType.Chain Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property DocumentUnitPrivacyEnabled As Boolean
        Get
            If ProtocolEnv.PrivacyTypology.HasValue AndAlso ProtocolEnv.PrivacyTypology.Value = PrivacyType.DocumentUnit Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property SimplifiedPrivacyEnabled As Boolean
        Get
            If ProtocolEnv.PrivacyTypology.HasValue AndAlso ProtocolEnv.PrivacyTypology.Value = PrivacyType.Simplified Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property PrivacyEnabled As Boolean
        Get
            If ProtocolEnv.PrivacyTypology.HasValue AndAlso [Enum].IsDefined(GetType(PrivacyType), ProtocolEnv.PrivacyTypology.Value) Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property PrivacyLevelsEnabled As Boolean
        Get
            If ProtocolEnv.PrivacyTypology.HasValue AndAlso ProtocolEnv.PrivacyTypology.Value <> PrivacyType.Simplified Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public Shared ReadOnly Property InvoiceResources As ICollection(Of InvoiceResources)
        Get
            If _invoiceResources Is Nothing Then
                Dim invoiceFactory As InvoiceResourcesFactory = New InvoiceResourcesFactory()
                _invoiceResources = New List(Of InvoiceResources) From
                {
                    invoiceFactory.CreateInvoicePA10(),
                    invoiceFactory.CreateInvoicePA11(),
                    invoiceFactory.CreateInvoicePA12(),
                    invoiceFactory.CreateInvoicePR12()
                }
            End If
            Return _invoiceResources
        End Get
    End Property
    Public ReadOnly Property EnableAttachmentByPage As IDictionary(Of String, Boolean)
        Get
            If _enableAttachmentByPage Is Nothing Then
                _enableAttachmentByPage = DocSuiteContext.Current.ProtocolEnv.EnabledEmailAttachmentPages
            End If
            Return _enableAttachmentByPage
        End Get
    End Property

#End Region

#Region " Methods "

    ''' <summary> Carica da DB tutti i parametri in eager loading. </summary>
    ''' <param name="closeAllSession"> Indica se chiudere tutte le sessioni prima di caricare i parametri. </param>
    Private Sub InitContext(ByVal closeAllSession As Boolean)
        Try
            _lastUpdate = DateTime.Now
            If closeAllSession Then
                NHibernateSessionManager.Instance.CloseAllSession()
            End If

            Dim dao As New NHibernateParameterEnvDao()
            If IsProtocolEnabled Then
                dao.ConnectionName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)

                Dim protocolParameters As IEnumerable(Of ParameterEnv) = dao.GetAll()
                _protocolEnv = New ProtocolEnv(Me, protocolParameters)
            Else
                _protocolEnv = New ProtocolEnv(Me)
            End If

            If IsResolutionEnabled Then
                dao.ConnectionName = "ReslDB"

                Dim resolutionParameters As IEnumerable(Of ParameterEnv) = dao.GetAll()
                _resolutionEnv = New ResolutionEnv(Me, resolutionParameters)
            Else
                _resolutionEnv = New ResolutionEnv(Me)
            End If

            If IsDocumentEnabled Then
                dao.ConnectionName = "DocmDB"

                Dim documentParameters As IEnumerable(Of ParameterEnv) = dao.GetAll()
                _documentEnv = New DocumentEnv(Me, documentParameters)
            Else
                _documentEnv = New DocumentEnv(Me)
            End If

            'NHibernateSessionManager.Instance.ClearFactories()
        Catch ex As Exception
            Throw New DocSuiteException("Si è verificato un errore nell'inizializzazione del contesto per ParameterEnvDao.", ex)
        End Try
    End Sub

    Public Sub RefreshPrivacyLevel(levels As ICollection(Of PrivacyLevel))
        If (levels IsNot Nothing AndAlso levels.Any()) Then
            _currentPrivacyLevels = levels
        End If
    End Sub
    Public Function GetEnableAttachmentByPage(pagenName As String) As Boolean
        Dim enableAttachment As Boolean = False
        If Not EnableAttachmentByPage Is Nothing AndAlso EnableAttachmentByPage.ContainsKey(pagenName) Then
            enableAttachment = EnableAttachmentByPage(pagenName)
        End If
        Return enableAttachment
    End Function
#End Region

End Class
