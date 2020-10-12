using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.VSW.DocumentUnitLink.Configuration;
using VecompSoftware.BPM.Integrations.Modules.VSW.DocumentUnitLink.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.Services.Command.CQRS.Events.Models.Workflows;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.DocumentUnitLink
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;

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
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IWebAPIClient webAPIClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _webAPIClient = webAPIClient;
                _needInitializeModule = true;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.DocumentUnitLink -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("DocumentUnitLink -> critical error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.DocumentUnitLink"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventWorkflowActionDocumentUnitLink>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.CreateDocumentUnitLinkSubscription, CreateDocumentUnitLinkCallback));

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

        private async Task CreateDocumentUnitLinkCallback(IEventWorkflowActionDocumentUnitLink evt, IDictionary<string, object> properties)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("CreateDocumentUnitLinkCallback -> evaluate event id ", evt.Id)), LogCategories);

            try
            {
                WorkflowActionDocumentUnitLinkModel item = (WorkflowActionDocumentUnitLinkModel)evt.ContentType.ContentTypeValue;
                DocumentUnitModel referenced = item.GetReferenced();
                DocumentUnitModel destinationLink = item.GetDestinationLink();

                switch (destinationLink.Environment)
                {
                    case (int)DSWEnvironmentType.Protocol:
                        {
                            await LinkIntoProtocol(destinationLink, referenced);
                            break;
                        }
                    case (int)DSWEnvironmentType.Resolution:
                        {
                            await LinkIntoResolution();
                            break;
                        }
                    case (int)DSWEnvironmentType.DocumentSeries:
                        {
                            await LinkIntoDocumentSeriesItem(destinationLink, referenced);
                            break;
                        }
                    case (int)DSWEnvironmentType.PECMail:
                        {
                            await LinkIntoPECMail(destinationLink, referenced);
                            break;
                        }
                    case (int)DSWEnvironmentType.Collaboration:
                        {
                            await LinkIntoCollaboration(destinationLink, referenced);
                            break;
                        }
                    case (int)DSWEnvironmentType.Dossier:
                        {
                            await LinkIntoDossier(destinationLink, referenced);
                            break;
                        }
                    default:
                        {
                            if (destinationLink.Environment >= 100)
                            {
                                await LinkIntoUDS(destinationLink, referenced);
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("CreateDocumentUnitLinkCallback -> error complete call"), ex, LogCategories);
                throw;
            }
        }

        private async Task LinkIntoProtocol(DocumentUnitModel destinationLink, DocumentUnitModel referenced)
        {
            throw new NotImplementedException("Link into protocol not implemented");
        }

        private async Task LinkIntoDocumentSeriesItem(DocumentUnitModel destinationLink, DocumentUnitModel referenced)
        {
            throw new NotImplementedException("Link into document series not implemented");
        }

        private async Task LinkIntoResolution()
        {
            throw new NotImplementedException("Link resolution not implemented");
        }

        private async Task LinkIntoPECMail(DocumentUnitModel destinationLink, DocumentUnitModel referenced)
        {
            PECMail pecMail = new PECMail
            {
                UniqueId = destinationLink.UniqueId,
                EntityId = destinationLink.EntityId.Value,
                Year = referenced.Year,
                Number = int.Parse(referenced.Number),
                DocumentUnit = new DocumentUnit(referenced.UniqueId),
                RecordedInDocSuite = 1
            };
            await _webAPIClient.PutAsync(pecMail, actionType: UpdateActionType.PECMailManaged, retryPolicyEnabled: true);
            _logger.WriteInfo(new LogMessage($"LinkIntoPECMail -> UDSDocumentUnit {referenced.UniqueId}/{referenced.Environment} has been successfully inserted"), LogCategories);
        }

        private async Task LinkIntoUDS(DocumentUnitModel destinationLink, DocumentUnitModel referenced)
        {
            UDSDocumentUnit udsDocumentUnit = new UDSDocumentUnit
            {
                Repository = new UDSRepository() { UniqueId = destinationLink.IdUDSRepository.Value },
                RelationType = UDSRelationType.ProtocolArchived,
                Relation = new DocumentUnit(referenced.UniqueId),
                IdUDS = destinationLink.UniqueId,
                Environment = destinationLink.Environment
            };

            await _webAPIClient.PostAsync(udsDocumentUnit, retryPolicyEnabled: true);
            _logger.WriteInfo(new LogMessage($"LinkIntoUDS -> UDSDocumentUnit {referenced.UniqueId}/{referenced.Environment} has been successfully inserted"), LogCategories);
        }

        private async Task LinkIntoCollaboration(DocumentUnitModel destinationLink, DocumentUnitModel referenced)
        {
            Collaboration collaboration = new Collaboration()
            {
                UniqueId = destinationLink.UniqueId,
                Year = referenced.Year,
                Number = int.Parse(referenced.Number),
                DocumentUnit = new DocumentUnit(referenced.UniqueId)
            };

            await _webAPIClient.PutAsync(collaboration, actionType: UpdateActionType.CollaborationManaged, retryPolicyEnabled: true);
            _logger.WriteInfo(new LogMessage($"LinkIntoCollaboration -> DocumentUnit {referenced.UniqueId}/{referenced.Environment} has been successfully inserted"), LogCategories);
        }

        private async Task LinkIntoDossier(DocumentUnitModel destinationLink, DocumentUnitModel referenced)
        {
            _logger.WriteInfo(new LogMessage($"LinkIntoDossier -> Linking DocumentUnit {referenced.UniqueId}/{referenced.Environment} into dossier {destinationLink.UniqueId}"), LogCategories);
            if (referenced.Environment == (int)DSWEnvironmentType.Fascicle)
            {
                DossierFolder fascicleDossierFolder = new DossierFolder
                {
                    Dossier = new Dossier { UniqueId = destinationLink.UniqueId },
                    Fascicle = new Fascicle { UniqueId = referenced.UniqueId },
                    ParentInsertId = destinationLink.UniqueId,
                    Category = new Category { EntityShortId = (short)referenced.Category.IdCategory.Value }
                };

                await _webAPIClient.PostAsync(fascicleDossierFolder, retryPolicyEnabled: true);
                _logger.WriteInfo(new LogMessage($"LinkIntoDossier -> DocumentUnit {referenced.UniqueId}/{referenced.Environment} has been successfully inserted"), LogCategories);
            }
        }


        #endregion
    }
}
