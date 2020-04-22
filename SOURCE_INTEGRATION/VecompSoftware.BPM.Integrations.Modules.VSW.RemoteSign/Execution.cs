using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.VSW.RemoteSign.Configuration;
using VecompSoftware.BPM.Integrations.Modules.VSW.RemoteSign.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command.CQRS.Events.Models.Integrations.GenericProcesses;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Documents.Signs;
using VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses;
using VecompSoftware.Services.SignService.ArubaSignService.Models;
using VecompSoftware.Services.SignService.ProxySignService.Models;
using VecompSoftware.Services.SignService.Services;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using SignModel = VecompSoftware.Services.SignService.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService;
using VecompSoftware.BPM.Integrations.Services.WebAPI.Models;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.RemoteSign
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private const string _biblos_attribute_filename = "filename";
        private const string _biblos_attribute_signature = "signature";

        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IDocumentClient _documentClient;
        private readonly IServiceBusClient _serviceBusClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        private readonly Dictionary<ProviderSignType, Func<EventDematerialisationRequest, RemoteSignProperty, Task>> _providerOTPTypes;
        private readonly Dictionary<RemoteSignType, Func<EventDematerialisationRequest, ICollection<WorkflowReferenceBiblosModel>, ProviderSignType, RemoteSignProperty, Task>> _providerSignTypes;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(Execution));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        [ImportingConstructor]
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IWebAPIClient webAPIClient, IDocumentClient documentClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _webAPIClient = webAPIClient;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _documentClient = documentClient;
                _needInitializeModule = true;
                _providerOTPTypes = new Dictionary<ProviderSignType, Func<EventDematerialisationRequest, RemoteSignProperty, Task>>
                 {
                    {ProviderSignType.ArubaRemote, RequestOTPArubaAsync},
                    {ProviderSignType.InfocertRemote, RequestOTPInfocertAsync}
                 };

                _providerSignTypes = new Dictionary<RemoteSignType, Func<EventDematerialisationRequest, ICollection<WorkflowReferenceBiblosModel>, ProviderSignType, RemoteSignProperty, Task>>
                {
                    {RemoteSignType.Automatic, SignAutomatic},
                    {RemoteSignType.Remote, SignRemote}
                };


            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.RemoteSign -> Critical error in costruction module"), ex, LogCategories);
                throw;
            }
        }
        #endregion

        #region [ Methods ]
        protected override void Execute()
        {
            if (Cancel)
            {
                return;
            }
            try
            {
                InitializeModule();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.RemoteSign -> Execute critical error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.RemoteSign"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<EventDematerialisationRequest>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration, 
                    _moduleConfiguration.WorkflowStartOTPRequestSubscription, WorkflowStartOTPRequestallback));
                _subscriptions.Add(_serviceBusClient.StartListening<EventDematerialisationRequest>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowStartRemoteSignSubscription, WorkflowStartRemoteSignCallback));
                _needInitializeModule = false;
            }
        }

        internal void CleanSubscriptions()
        {
            foreach (Guid item in _subscriptions)
            {
                _serviceBusClient.CloseListeningAsync(item).Wait();
            }
            _subscriptions.Clear();
            _needInitializeModule = true;
        }
        #region [ ServiceBus Callbacks ]

        #region [ Sign Documents ]
        private async Task WorkflowStartRemoteSignCallback(EventDematerialisationRequest arg)
        {
            DocumentManagementRequestModel documentManagementRequestModel = arg.Content.ContentValue as DocumentManagementRequestModel;
            RemoteSignProperty signOption = documentManagementRequestModel.UserProfileRemoteSignProperty;
            ICollection<WorkflowReferenceBiblosModel> documentChains = documentManagementRequestModel.Documents;

            IDictionary<string, string> signProvider = documentManagementRequestModel.UserProfileRemoteSignProperty.CustomProperties;
            string providerType = signProvider[RemoteSignProperty.PROVIDER_TYPE];
            ProviderSignType provider = (ProviderSignType)int.Parse(providerType);

            await SendSignRequest(signOption.RemoteSignType, arg, documentChains, provider, signOption);
        }
        private async Task SignRemote(EventDematerialisationRequest evt, ICollection<WorkflowReferenceBiblosModel> documentChains, ProviderSignType provider, RemoteSignProperty signOption)
        {
            SignService signService = new SignService((info) =>
            {
                _logger.WriteInfo(new LogMessage($"Remote sign service successfully initiated...\n {info}"), LogCategories);
            }, (err) =>
            {
                _logger.WriteError(new LogMessage($"Remote sign service not initiated correctly...\n {err}"), LogCategories);
            });

            foreach (WorkflowReferenceBiblosModel documentChain in documentChains)
            {
                byte[] documentStream = await _documentClient.GetDocumentStreamAsync(documentChain.ArchiveDocumentId.Value);

                if (provider == ProviderSignType.ArubaRemote)
                {
                    try
                    {
                        ArubaSignModel arubaSignModel = new ArubaSignModel
                        {
                            User = signOption.Alias,
                            OTPPassword = signOption.Password,
                            OTPAuthType = signOption.CustomProperties[RemoteSignProperty.ARUBA_OTP_AUTHTYPE],
                            CertificateId = signOption.CustomProperties[RemoteSignProperty.ARUBA_CERTIFICATEID],
                            SignType = SignModel.SignType.Remote,
                            UserPassword = signOption.PIN,
                            RequestType = SignModel.SignRequestType.Pades
                        };

                        if (signOption.CustomProperties[RemoteSignProperty.REQUEST_TYPE] == SignModel.SignRequestType.Cades.ToString())
                        {
                            arubaSignModel.RequestType = SignModel.SignRequestType.Cades;
                        }

                        documentStream = signService.SignDocument(arubaSignModel, documentStream, documentChain.DocumentName, SignModel.SignatureType.ArubaSign);

                        UpdateDocumentAsync(documentChain.ArchiveDocumentId.Value, documentChain.DocumentName, documentStream);

                        _logger.WriteInfo(new LogMessage($"[ArubaRemote] Sign document: signing document successfully with ArubaRemote"), new LogCategory("SignDocument"));
                        await _webAPIClient.PushCorrelatedNotificationAsync($"Documento {documentChain.DocumentName} firmato con successo.",
                            ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusDone);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(new LogMessage($"[ArubaRemote] Sign document: Critical error occured during signing document: {ex.Message}"), new LogCategory("SignDocument"));

                        await _webAPIClient.PushCorrelatedNotificationAsync($"Firma documento: {documentChain.DocumentName} errore.",
                            ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusError);
                        _logger.WriteError(new LogMessage("[ArubaRemote] SignRemote -> Critical Error"), ex, LogCategories);
                        throw;
                    }
                }

                if (provider == ProviderSignType.InfocertRemote)
                {

                    try
                    {
                        ProxySignModel proxyModel = new ProxySignModel
                        {
                            Alias = signOption.Alias,
                            OTPPassword = signOption.OTP,
                            PINPassword = signOption.PIN,
                            SignType = SignModel.SignType.Remote,
                            RequestType = SignModel.SignRequestType.Pades
                        };

                        if (signOption.CustomProperties[RemoteSignProperty.REQUEST_TYPE] == SignModel.SignRequestType.Cades.ToString())
                        {
                            proxyModel.RequestType = SignModel.SignRequestType.Cades;
                        }


                        documentStream = signService.SignDocument(proxyModel, documentStream, documentChain.DocumentName, SignModel.SignatureType.ProxySign);
                        UpdateDocumentAsync(documentChain.ArchiveDocumentId.Value, documentChain.DocumentName, documentStream);

                        _logger.WriteInfo(new LogMessage($"[InfocertRemote] Sign document: signing document successfully with InfocertRemote"), new LogCategory("SignDocument"));
                        await _webAPIClient.PushCorrelatedNotificationAsync($"Documento {documentChain.DocumentName} firmato con successo.",
                            ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusDone);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(new LogMessage($"[InfocertRemote] Sign document: Critical error occured during signing document: {ex.Message}"), new LogCategory("SignDocument"));

                        await _webAPIClient.PushCorrelatedNotificationAsync($"Firma documento {documentChain.DocumentName} errore.",
                            ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusError);
                        _logger.WriteError(new LogMessage("[InfocertRemote] SignRemote -> Critical Error"), ex, LogCategories);
                        throw;
                    }
                }

            }
        }
        private async Task SignAutomatic(EventDematerialisationRequest evt, ICollection<WorkflowReferenceBiblosModel> documentChains, ProviderSignType provider, RemoteSignProperty signOption)
        {
            SignService signService = new SignService((info) =>
            {
                _logger.WriteInfo(new LogMessage($"Automatic sign service successfully initiated...\n {info}"), LogCategories);
            }, (err) =>
            {
                _logger.WriteError(new LogMessage($"Automatic sign service not initiated correctly...\n {err}"), LogCategories);
            });

            foreach (WorkflowReferenceBiblosModel documentChain in documentChains)
            {
                byte[] documentStream = await _documentClient.GetDocumentStreamAsync(documentChain.ArchiveDocumentId.Value);

                if (provider == ProviderSignType.ArubaAutomatic)
                {
                    try
                    {
                        ArubaSignModel arubaSignModel = new ArubaSignModel
                        {
                            DelegatedDomain = signOption.CustomProperties[RemoteSignProperty.ARUBA_DELEGATED_DOMAIN],
                            DelegatedPassword = signOption.CustomProperties[RemoteSignProperty.ARUBA_DELEGATED_PASSWORD],
                            DelegatedUser = signOption.CustomProperties[RemoteSignProperty.ARUBA_DELEGATED_USER],
                            OTPPassword = signOption.OTP,
                            OTPAuthType = signOption.CustomProperties[RemoteSignProperty.ARUBA_OTP_AUTHTYPE],
                            User = signOption.CustomProperties[RemoteSignProperty.ARUBA_USER],
                            CertificateId = signOption.CustomProperties[RemoteSignProperty.ARUBA_CERTIFICATEID],
                            RequestType = SignModel.SignRequestType.Pades
                        };

                        if (signOption.CustomProperties[RemoteSignProperty.REQUEST_TYPE] == SignModel.SignRequestType.Cades.ToString())
                        {
                            arubaSignModel.RequestType = SignModel.SignRequestType.Cades;
                        }

                        documentStream = signService.SignDocument(arubaSignModel, documentStream, documentChain.DocumentName, SignModel.SignatureType.ArubaSign);

                        UpdateDocumentAsync(documentChain.ArchiveDocumentId.Value, documentChain.DocumentName, documentStream);
                        _logger.WriteInfo(new LogMessage($"[ArubaAutomatic] Sign document: signing document successfully with ArubaAutomatic"), new LogCategory("SignDocument"));

                        await _webAPIClient.PushCorrelatedNotificationAsync($"Documento {documentChain.DocumentName} firmato con successo.",
                           ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusDone);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(new LogMessage($"[ArubaAutomatic] Sign document: Critical error occured during signing document: {ex.Message}"), new LogCategory("SignDocument"));

                        await _webAPIClient.PushCorrelatedNotificationAsync($"Signing document: {documentChain.DocumentName} failed.",
                            ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusError);
                        _logger.WriteError(new LogMessage("[ArubaAutomatic] SignAutomatic -> Critical Error"), ex, LogCategories);
                        throw;
                    }
                }

                if (provider == ProviderSignType.InfocertAutomatic)
                {
                    try
                    {
                        ProxySignModel proxyModel = new ProxySignModel
                        {
                            Alias = signOption.Alias,
                            OTPPassword = signOption.OTP,
                            PINPassword = signOption.PIN,
                            SignType = SignModel.SignType.Automatic
                        };

                        if (signOption.CustomProperties[RemoteSignProperty.REQUEST_TYPE] == SignModel.SignRequestType.Cades.ToString())
                        {
                            proxyModel.RequestType = SignModel.SignRequestType.Cades;
                        }

                        documentStream = signService.SignDocument(proxyModel, documentStream, documentChain.DocumentName, SignModel.SignatureType.ProxySign);

                        UpdateDocumentAsync(documentChain.ArchiveDocumentId.Value, documentChain.DocumentName, documentStream);
                        _logger.WriteInfo(new LogMessage($"[InfocertAutomatic] Sign document: signing document successfully with InfocertAutomatic"), new LogCategory("SignDocument"));

                        await _webAPIClient.PushCorrelatedNotificationAsync($"Documento {documentChain.DocumentName} firmato con successo.",
                            ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusDone);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(new LogMessage($"[InfocertAutomatic] Sign document: Critical error occured during signing document: {ex.Message}"), new LogCategory("SignDocument"));

                        await _webAPIClient.PushCorrelatedNotificationAsync($"Signing document: {documentChain.DocumentName} failed.",
                            ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusError);
                        _logger.WriteError(new LogMessage("[InfocertAutomatic] SignAutomatic -> Critical Error"), ex, LogCategories);
                        throw;
                    }
                }
            }

        }
        private Guid UpdateDocumentAsync(Guid documentId, string documentName, byte[] content)
        {
            DocumentsClient document = new DocumentsClient();

            Document checkedout = document.DocumentCheckOut(documentId, true, ModuleConfigurationHelper.MODULE_NAME);
            List<Services.BiblosDS.DocumentService.Attribute> attributes = document.GetAttributesDefinition(checkedout.Archive.Name);

            AttributeValue fileNameAttr = checkedout.AttributeValues.Single(f => f.Attribute.Name.Equals(_biblos_attribute_filename, StringComparison.InvariantCultureIgnoreCase));
            fileNameAttr.Value = documentName;


            //this is not working correctly 
            //Services.BiblosDS.DocumentService.Attribute signatureAttr = attributes.Single(f => f.Name.Equals(_biblos_attribute_signature, StringComparison.InvariantCultureIgnoreCase));
            //signatureAttr.Value = checkedout.AttributesValue[_biblos_attribute_signature].Value;

            checkedout.Content = new Content()
            {
                Blob = content
            };

            Guid savedId = document.DocumentCheckIn(checkedout, ModuleConfigurationHelper.MODULE_NAME);
            _logger.WriteInfo(new LogMessage($"[InfocertRemote] Document {documentId} correctly updated"), new LogCategory("SignDocument"));

            document.ConfirmDocumentAsync(checkedout.IdDocument);

            return savedId;

        }
        #endregion

        #region [ Request OTP]
        private async Task WorkflowStartOTPRequestallback(EventDematerialisationRequest arg)
        {
            DocumentManagementRequestModel documentManagementRequestModel = arg.Content.ContentValue as DocumentManagementRequestModel;
            RemoteSignProperty signOption = documentManagementRequestModel.UserProfileRemoteSignProperty;

            IDictionary<string, string> signProvider = documentManagementRequestModel.UserProfileRemoteSignProperty.CustomProperties;
            string providerType = signProvider[RemoteSignProperty.PROVIDER_TYPE];
            ProviderSignType provider = (ProviderSignType)int.Parse(providerType);

            SendOTPRequest(provider, arg, signOption);
        }

        private void SendOTPRequest(ProviderSignType provider, EventDematerialisationRequest evt, RemoteSignProperty signOptions)
        {
            _providerOTPTypes[provider].Invoke(evt, signOptions);
        }
        private async Task SendSignRequest(RemoteSignType signType, EventDematerialisationRequest evt, ICollection<WorkflowReferenceBiblosModel> documentChains, ProviderSignType provider, RemoteSignProperty signOption)
        {
            await _providerSignTypes[signType].Invoke(evt, documentChains, provider, signOption);
        }
        private async Task RequestOTPInfocertAsync(EventDematerialisationRequest evt, RemoteSignProperty signOption)
        {
            try
            {
                SignService signService = new SignService((info) =>
                {
                    _logger.WriteInfo(new LogMessage($"InfocertRemote sign service successfully initiated...\n {info}"), LogCategories);
                }, (err) =>
                {
                    _logger.WriteError(new LogMessage($"InfocertRemote sign service not initiated correctly...\n {err}"), LogCategories);
                });

                ProxySignModel proxySignModel = new ProxySignModel
                {
                    Alias = signOption.Alias,
                    SignType = SignModel.SignType.Remote
                };

                signService.RequestOTP(proxySignModel, SignModel.SignatureType.ProxySign);

                await _webAPIClient.PushCorrelatedNotificationAsync("OTP richiesto \u00E8 stato inviato",
                         ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusDone);

                _logger.WriteInfo(new LogMessage($"[RequestOTPInfocert] OTP request: OTP request sent successfully with Infocert"), new LogCategory("OTPRequest"));
            }
            catch (Exception ex)
            {
                await _webAPIClient.PushCorrelatedNotificationAsync("Errore durante l'invio della richiesta OTP",
                       ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusError);

                _logger.WriteError(new LogMessage($"[RequestOTPInfocert] OTP request: Critical error occured during OTP request: {ex.Message}"), new LogCategory("OTPRequest"));
                throw;
            }
        }
        private async Task RequestOTPArubaAsync(EventDematerialisationRequest evt, RemoteSignProperty signOption)
        {
            try
            {
                SignService signService = new SignService((info) =>
                {
                    _logger.WriteInfo(new LogMessage($"ArubaRemote sign service successfully initiated...\n {info}"), LogCategories);
                }, (err) =>
                {
                    _logger.WriteError(new LogMessage($"ArubaRemote sign service not initiated correctly...\n {err}"), LogCategories);
                });
                ArubaSignModel arubaSignModel = new ArubaSignModel()
                {
                    User = signOption.Alias,
                    OTPPassword = signOption.Password,
                    OTPAuthType = RemoteSignProperty.ARUBA_OTP_AUTHTYPE,
                    CertificateId = RemoteSignProperty.ARUBA_CERTIFICATEID,
                    SignType = SignModel.SignType.Remote
                };

                signService.RequestOTP(arubaSignModel, SignModel.SignatureType.ArubaSign);

                await _webAPIClient.PushCorrelatedNotificationAsync("OTP richiesto \u00E8 stato inviato",
                         ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusDone);

                _logger.WriteInfo(new LogMessage($"[RequestOTPAruba] OTP request: OTP request sent successfully with Aruba"), new LogCategory("OTPRequest"));
            }
            catch (Exception ex)
            {
                await _webAPIClient.PushCorrelatedNotificationAsync("Errore durante l'invio della richiesta OTP",
                        ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusError);

                _logger.WriteError(new LogMessage($"[RequestOTPAruba] OTP request: Critical error occured during OTP request: {ex.Message}"), new LogCategory("OTPRequest"));
                throw;
            }
        }
        #endregion

        #endregion

        #endregion
    }

}