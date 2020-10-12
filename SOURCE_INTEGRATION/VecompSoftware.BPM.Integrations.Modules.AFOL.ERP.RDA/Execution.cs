using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities.Common;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.RDA.Configurations;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.RDA.Helpers;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.RDA.Models;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.RDA.StringFormatters;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Entities.Fascicles;
using VecompSoftware.Core.Command.CQRS.Commands.Models.Fascicles;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Metadata;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders.Metadata;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Models.Resolutions;
using Entities = VecompSoftware.DocSuiteWeb.Model.Entities;
using UDSEInvoiceHelper = VecompSoftware.Helpers.EInvoice.UDS.EInvoice1_2.EInvoiceHelper;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.RDA
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private readonly IWebAPIClient _webAPIClient;
        private readonly IDocumentClient _documentClient;
        private readonly IServiceBusClient _serviceBusClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private readonly ILogger _logger;
        private bool _needInitializeModule = false;
        private ERPDbContext _dbContext;
        private Location _workflowLocation;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IdentityContext _identityContext;
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
        public Execution(ILogger logger, IWebAPIClient webAPIClient, IDocumentClient documentClient, IServiceBusClient serviceBusClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            _logger = logger;
            _webAPIClient = webAPIClient;
            _documentClient = documentClient;
            _serviceBusClient = serviceBusClient;
            _needInitializeModule = true;
            _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
            string username = "anonymous";
            if (WindowsIdentity.GetCurrent() != null)
            {
                username = WindowsIdentity.GetCurrent().Name;
            }
            _identityContext = new IdentityContext(username);
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

                ProcessRDACommands().Wait();
                ProcessODACommands().Wait();
                ProcessPreventivoCommands().Wait();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("ERP.RDA -> Critical error in costruction module"), ex, LogCategories);
                throw;
            }
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _dbContext = new ERPDbContext(_logger, ModuleConfigurationHelper.JsonSerializerSettings, _moduleConfiguration.ConnectionString);

                int? workflowLocationId = _webAPIClient.GetParameterWorkflowLocationIdAsync().Result;
                if (!workflowLocationId.HasValue)
                {
                    throw new ArgumentNullException("Parameter WorkflowLocationId is not defined");
                }
                _workflowLocation = _webAPIClient.GetLocationAsync(workflowLocationId.Value).Result.Single();

                _subscriptions.Add(_serviceBusClient.StartListening<IEventCreateResolution>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowStartResolutionRDASubscription, WorkflowStartResolutionRDACallback));

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

        protected override void OnStop()
        {
            CleanSubscriptions();
            _dbContext.Dispose();
            _logger.WriteInfo(new LogMessage("OnStop -> AFOL.ERP.RDA"), LogCategories);
        }

        #region [ WorkflowStartResolutionRDACallback ] 
        private async Task WorkflowStartResolutionRDACallback(IEventCreateResolution evt, IDictionary<string, object> properties)
        {
            _logger.WriteInfo(new LogMessage($"WorkflowStartResolutionRDACallback -> received callback with event id {evt.Id}"), LogCategories);

            try
            {
                ResolutionModel resolutionModel = evt.ContentType.ContentTypeValue as ResolutionModel;
                
                if (resolutionModel == null)
                {
                    _logger.WriteError(new LogMessage($"WorkflowStartResolutionRDACallback -> ResolutionModel is null"), LogCategories);
                    throw new ArgumentNullException("ResolutionModel", "ResolutionModel is null.");
                }

                if (resolutionModel.Category == null)
                {
                    _logger.WriteError(new LogMessage($"WorkflowStartResolutionRDACallback -> ResolutionModel doesn't have a CategoryModel"), LogCategories);
                    throw new ArgumentNullException("CategoryModel", "CategoryModel is null.");
                }

                if (!resolutionModel.Year.HasValue)
                {
                    _logger.WriteError(new LogMessage($"WorkflowStartResolutionRDACallback -> Undefined Year for ResolutionModel"), LogCategories);
                    throw new ArgumentNullException("ResolutionModel", "Year is null.");
                }

                if (!resolutionModel.PublishingDate.HasValue)
                {
                    _logger.WriteError(new LogMessage($"WorkflowStartResolutionRDACallback -> Undefined PublishingDate for ResolutionModel"), LogCategories);
                    throw new ArgumentNullException("ResolutionModel", "PublishingDate is null.");
                }

                if (!resolutionModel.Number.HasValue)
                {
                    _logger.WriteError(new LogMessage($"WorkflowStartResolutionRDACallback -> Undefined Number for ResolutionModel"), LogCategories);
                    throw new ArgumentNullException("ResolutionModel", "Number is null.");
                }


                ResolutionEvent existingResolutionEvent = _dbContext.ResolutionEvents.FirstOrDefault(x => x.Id == resolutionModel.UniqueId);

                if (existingResolutionEvent != null)
                {
                    _logger.WriteWarning(new LogMessage($"WorkflowStartResolutionRDACallback -> ResolutionEvent with year {existingResolutionEvent.Year} and number {existingResolutionEvent.Number} already exists ({existingResolutionEvent.Id})"), LogCategories);
                    return;
                }

                _logger.WriteInfo(new LogMessage($"WorkflowStartResolutionRDACallback -> Creating ResolutionEvent"), LogCategories);

                ResolutionEvent resolutionEvent = new ResolutionEvent
                {
                    Year = resolutionModel.Year.Value,
                    Number = resolutionModel.Number.Value.ToString(),
                    CategoryName = resolutionModel.Category.Name,
                    Subject = resolutionModel.Subject,
                    Date = resolutionModel.PublishingDate.Value
                };

                _logger.WriteInfo(new LogMessage($"WorkflowStartResolutionRDACallback -> ResolutionEvent with id {resolutionEvent.Id} created successfully"), LogCategories);
                _logger.WriteInfo(new LogMessage($"WorkflowStartResolutionRDACallback -> Saving the ResolutionEvent in the database"), LogCategories);

                _dbContext.ResolutionEvents.Add(resolutionEvent);
                await _dbContext.SaveChangesAsync();

                _logger.WriteInfo(new LogMessage($"WorkflowStartResolutionRDACallback -> ResolutionEvent {resolutionEvent.Id}/{resolutionEvent.Subject} has been successfully inserted"), LogCategories);
                _logger.WriteInfo(new LogMessage($"WorkflowStartResolutionRDACallback -> Event with id {evt.Id} successfully processed"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowStartResolutionRDACallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }
        #endregion

        #region [ Evaluate Commands Methods ]
        private async Task ProcessRDACommands()
        {
            _logger.WriteInfo(new LogMessage($"ProcessRDACommands -> Reading RDA Commands from the database..."), LogCategories);

            ICollection<RDADocSuiteCommand> rdaCommands = _dbContext.RDADocSuiteCommands
                    .Where(command => !command.WFDocSuiteStarted.HasValue && !command.WFDocSuiteStatus.HasValue).ToList();

            _logger.WriteInfo(new LogMessage($"ProcessRDACommands -> RDA Commands successfully read from the database"), LogCategories);
            _logger.WriteInfo(new LogMessage($"ProcessRDACommands -> Evaluating RDA Commands..."), LogCategories);

            await EvaluateCommandsAsync(rdaCommands, DocSuiteCommandType.RDA);

            _logger.WriteInfo(new LogMessage($"ProcessRDACommands -> RDA Commands have been evaluated"), LogCategories);
        }

        private async Task ProcessODACommands()
        {
            _logger.WriteInfo(new LogMessage($"ProcessODACommands -> Reading ODA Commands from the database..."), LogCategories);

            ICollection<ODADocSuiteCommand> odaCommands = _dbContext.ODADocSuiteCommands
                .Where(command => !command.WFDocSuiteStarted.HasValue && !command.WFDocSuiteStatus.HasValue).ToList();

            _logger.WriteInfo(new LogMessage($"ProcessODACommands -> ODA Commands successfully read from the database"), LogCategories);
            _logger.WriteInfo(new LogMessage($"ProcessODACommands -> Evaluating ODA Commands..."), LogCategories);

            await EvaluateCommandsAsync(odaCommands, DocSuiteCommandType.ODA);

            _logger.WriteInfo(new LogMessage($"ProcessODACommands -> ODA Commands have been evaluated"), LogCategories);
        }

        private async Task ProcessPreventivoCommands()
        {
            _logger.WriteInfo(new LogMessage($"ProcessPreventivoCommands -> Reading Preventivo Commands from the database..."), LogCategories);

            ICollection<PreventivoDocSuiteCommand> preventivoCommands = _dbContext.PreventivoDocSuiteCommands
                .Where(command => !command.WFDocSuiteStarted.HasValue && !command.WFDocSuiteStatus.HasValue).ToList();

            _logger.WriteInfo(new LogMessage($"ProcessPreventivoCommands -> Preventivo Commands successfully read from the database"), LogCategories);
            _logger.WriteInfo(new LogMessage($"ProcessPreventivoCommands -> Evaluating Preventivo Commands..."), LogCategories);

            await EvaluateCommandsAsync(preventivoCommands, DocSuiteCommandType.Preventivo);

            _logger.WriteInfo(new LogMessage($"ProcessPreventivoCommands -> Preventivo Commands have been evaluated"), LogCategories);
        }

        private async Task EvaluateCommandsAsync<TCommand>(ICollection<TCommand> commandCollection, DocSuiteCommandType commandType)
            where TCommand : DocSuiteCommand
        {
            int successfullCount = 0;
            int totalCount = 0;

            try
            {
                foreach (TCommand currentCommand in commandCollection)
                {
                    totalCount++;
                    currentCommand.WFDocSuiteStarted = DateTimeOffset.UtcNow;

                    try
                    {
                        _logger.WriteInfo(new LogMessage($"EvaluateCommands -> evaluating {commandType} Command with id: {currentCommand.Id} supplier name: {currentCommand.SupplierName} and PIVACF: {currentCommand.SupplierPIVACF}"), LogCategories);

                        switch (commandType)
                        {
                            case DocSuiteCommandType.RDA:
                                await WorkflowStartRDAAsync(currentCommand as RDADocSuiteCommand);
                                break;
                            case DocSuiteCommandType.ODA:
                                await WorkflowStartODAAsync(currentCommand as ODADocSuiteCommand);
                                break;
                            case DocSuiteCommandType.Preventivo:
                                await WorkflowStartPreventivoAsync(currentCommand as PreventivoDocSuiteCommand);
                                break;
                            default:
                                break;
                        }

                        currentCommand.WFDocSuiteStatus = WorkflowStatus.Started;
                        _dbContext.SaveChanges();

                        _logger.WriteInfo(new LogMessage($"{commandType} Command with id {currentCommand.Id} and workflow id {currentCommand.WFDocSuiteId} has been successfully processed"), LogCategories);

                        successfullCount++;
                    }
                    catch (ArgumentException ex)
                    {
                        _logger.WriteWarning(new LogMessage($"ERP.RDA -> {commandType} command with id {currentCommand.Id} has no pass validation"), ex, LogCategories);
                        currentCommand.WFDocSuiteStatus = WorkflowStatus.MetadataValidationError;
                        _dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(new LogMessage($"ERP.RDA -> Error on evaluating {commandType} command with id: {currentCommand.Id}"), ex, LogCategories);
                        currentCommand.WFDocSuiteStatus = WorkflowStatus.Error;
                        _dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"Failed processing {commandType} commands"), ex, LogCategories);
                throw;
            }

            _logger.WriteInfo(new LogMessage($"{commandType} Commands have been evaluated (success: {successfullCount}/total: {totalCount})"), LogCategories);
        }

        private async Task WorkflowStartRDAAsync(RDADocSuiteCommand rdaCommand)
        {
            try
            {
                await ValidateRDACommandAsync(rdaCommand);

                Guid correlationId = Guid.NewGuid();
                Guid udsId = Guid.NewGuid();

                WorkflowReferenceModel[] rdaWorkflowRefereceModels = await BuildRDAWorkflowReferenceModels(rdaCommand);
                WorkflowResult workflowResult = await StartWorkflowAsync(_moduleConfiguration.RDAWorkflowRepositoryName, rdaWorkflowRefereceModels);

                if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
                {
                    _logger.WriteError(new LogMessage($"An error occured in starting workflow for command with id {rdaCommand.Id} of type {nameof(RDADocSuiteCommand)}"), LogCategories);
                    throw new Exception(string.Join(", ", workflowResult.Errors));
                }

                rdaCommand.UDSId = udsId;
                rdaCommand.WFDocSuiteId = correlationId;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"WorkflowStartRDA -> Critical Error while starting workflow for {nameof(RDADocSuiteCommand)} with id {rdaCommand.Id}"), ex, LogCategories);
                throw;
            }
        }

        private async Task WorkflowStartODAAsync(ODADocSuiteCommand odaCommand)
        {
            try
            {
                ValidateODACommand(odaCommand);

                Guid correlationId = Guid.NewGuid();
                Guid udsId = Guid.NewGuid();

                WorkflowReferenceModel[] odaWorkflowRefereceModels = await BuildODAWorkflowReferenceModels(odaCommand);
                WorkflowResult workflowResult = await StartWorkflowAsync(_moduleConfiguration.ODAWorkflowRepositoryName, odaWorkflowRefereceModels);

                if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
                {
                    _logger.WriteError(new LogMessage($"An error occured in starting workflow for command with id {odaCommand.Id} of type {nameof(ODADocSuiteCommand)}"), LogCategories);
                    throw new Exception(string.Join(", ", workflowResult.Errors));
                }

                odaCommand.UDSId = udsId;
                odaCommand.WFDocSuiteId = correlationId;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"WorkflowStartODA -> Critical Error while starting workflow for {nameof(ODADocSuiteCommand)} with id {odaCommand.Id}"), ex, LogCategories);
                throw;
            }
        }

        private async Task WorkflowStartPreventivoAsync(PreventivoDocSuiteCommand preventivoCommand)
        {
            try
            {
                ValidatePreventivoCommand(preventivoCommand);

                Guid correlationId = Guid.NewGuid();
                Guid udsId = Guid.NewGuid();

                WorkflowReferenceModel[] preventivoWorkflowRefereceModels = await BuildPreventivoWorkflowReferenceModels(preventivoCommand);
                WorkflowResult workflowResult = await StartWorkflowAsync(_moduleConfiguration.PreventivoWorkflowRepositoryName, preventivoWorkflowRefereceModels);

                if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
                {
                    _logger.WriteError(new LogMessage($"An error occured in starting workflow for command with id {preventivoCommand.Id} of type {nameof(PreventivoDocSuiteCommand)}"), LogCategories);
                    throw new Exception(string.Join(", ", workflowResult.Errors));
                }

                preventivoCommand.UDSId = udsId;
                preventivoCommand.WFDocSuiteId = correlationId;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"WorkflowStartPreventivo -> Critical Error while starting workflow for {nameof(PreventivoDocSuiteCommand)} with id {preventivoCommand.Id}"), ex, LogCategories);
                throw;
            }
        }

        private async Task<WorkflowResult> StartWorkflowAsync(string workflowName, params WorkflowReferenceModel[] workflowReferenceModels)
        {
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = workflowName
            };

            if (workflowReferenceModels.Length > 1)
            {
                for (int currentStep = 0; currentStep < workflowReferenceModels.Length; currentStep++)
                {
                    workflowStart.Arguments.Add(string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, $"_{currentStep}"),
                        new WorkflowArgument()
                        {
                            Name = string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, $"_{currentStep}"),
                            PropertyType = ArgumentType.Json,
                            ValueString = JsonConvert.SerializeObject(workflowReferenceModels[currentStep])
                        });
                }
            }
            else
            {
                workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                    new WorkflowArgument()
                    {
                        Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                        PropertyType = ArgumentType.Json,
                        ValueString = JsonConvert.SerializeObject(workflowReferenceModels[0])
                    });
            }

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.RelationGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = _moduleConfiguration.TenantId
            });

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.RelationGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                ValueGuid = _moduleConfiguration.TenantAOOId
            });

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = _moduleConfiguration.TenantName
            });

            WorkflowResult workflowResult = await _webAPIClient.StartWorkflow(workflowStart);
            _logger.WriteInfo(new LogMessage(string.Concat("Workflow started correctly [IsValid: ", workflowResult.IsValid, "] with instanceId ", workflowResult.InstanceId)), LogCategories);
            return workflowResult;
        }
        #endregion

        #region [ Build WorkflowReferenceModel Methods ]
        private async Task<WorkflowReferenceModel[]> BuildPreventivoWorkflowReferenceModels(PreventivoDocSuiteCommand preventivoCommand)
        {
            List<WorkflowReferenceModel> odaWorkflowReferenceModels = new List<WorkflowReferenceModel>();

            Fascicle rdaFascicle = await TryGetRDACommandFascicleAsync(preventivoCommand.RDAReference);

            Guid correlationId = Guid.NewGuid();
            Guid udsId = Guid.NewGuid();

            Contact contact = await GetOrCreateContactAsync(preventivoCommand.SupplierPIVACF, preventivoCommand.SupplierName);

            IDictionary<string, object> uds_metadatas = new Dictionary<string, object>
                {
                    { CommonDefinition.UDSMETADATA_NUMERO, double.Parse(preventivoCommand.Number) },
                    { CommonDefinition.UDSMETADATA_DATA, preventivoCommand.Date }
                };

            WorkflowReferenceModel workflowReferenceModelUDS = await CreateUDSBuildModelAsync(correlationId, udsId, rdaFascicle.UniqueId, uds_metadatas, preventivoCommand, contact, _moduleConfiguration.PreventivoWorkflowRepositoryName, _moduleConfiguration.PreventivoUdsRepositoryId);
            odaWorkflowReferenceModels.Add(workflowReferenceModelUDS);

            return odaWorkflowReferenceModels.ToArray();
        }

        private async Task UpdateFascicleMetadataValues(Fascicle fascicle, string odaCommandName, string cig)
        {
            _logger.WriteInfo(new LogMessage($"Updating metadata values for fascicle with id {fascicle.UniqueId}"), LogCategories);

            ICollection<MetadataValueModel> metadataValueModels = JsonConvert.DeserializeObject<ICollection<MetadataValueModel>>(fascicle.MetadataValues);
            MetadataValueModel odaMetadataValue = metadataValueModels.FirstOrDefault(metadata => metadata.KeyName == CommonDefinition.ODA_METADATA_KEYNAME);
            odaMetadataValue.Value = odaCommandName;

            MetadataValueModel cigMetadataValue = metadataValueModels.FirstOrDefault(metadata => metadata.KeyName == CommonDefinition.CIG_METADATA_KEYNAME);
            cigMetadataValue.Value = cig;

            FascicleBuildModel fascicleBuildModel = new FascicleBuildModel()
            {
                UniqueId = Guid.NewGuid(),
                Fascicle = new FascicleModel()
                {
                    UniqueId = fascicle.UniqueId,
                    Name = fascicle.Name,
                    Note = fascicle.Note,
                    Rack = fascicle.Rack,
                    FascicleObject = fascicle.FascicleObject,
                    FascicleType = (Entities.Fascicles.FascicleType?)fascicle.FascicleType,
                    Manager = fascicle.Manager
                }
            };
            fascicleBuildModel.Fascicle.Contacts = fascicle.Contacts.Select(s => new ContactModel() { EntityId = s.EntityId }).ToList();
            fascicleBuildModel.Fascicle.MetadataValues = JsonConvert.SerializeObject(metadataValueModels);

            CommandUpdateFascicleData commandUpdateFascicleData = new CommandUpdateFascicleData(_moduleConfiguration.TenantName, _moduleConfiguration.TenantId, _moduleConfiguration.TenantAOOId, _identityContext, fascicleBuildModel);

            await _webAPIClient.SendCommandAsync(commandUpdateFascicleData);

            _logger.WriteInfo(new LogMessage($"Update fascicle metadata values command with id {commandUpdateFascicleData.Id} has been sent"), LogCategories);
        }

        private async Task<WorkflowReferenceModel[]> BuildODAWorkflowReferenceModels(ODADocSuiteCommand odaCommand)
        {
            List<WorkflowReferenceModel> odaWorkflowReferenceModels = new List<WorkflowReferenceModel>();

            Fascicle rdaFascicle = await TryGetRDACommandFascicleAsync(odaCommand.RDAReference);

            await UpdateFascicleMetadataValues(rdaFascicle, odaCommand.SupplierName, odaCommand.CIG);

            Guid correlationId = Guid.NewGuid();
            Guid udsId = Guid.NewGuid();

            Contact contact = await GetOrCreateContactAsync(odaCommand.SupplierPIVACF, odaCommand.SupplierName);

            IDictionary<string, object> uds_metadatas = new Dictionary<string, object>
                {
                    { CommonDefinition.UDSMETADATA_NUMEROORDINE, odaCommand.Number },
                    { CommonDefinition.UDSMETADATA_DATAORDINE, odaCommand.Date },
                    { CommonDefinition.UDSMETADATA_CIG, odaCommand.CIG }
                };

            WorkflowReferenceModel workflowReferenceModelUDS = await CreateUDSBuildModelAsync(correlationId, udsId, rdaFascicle.UniqueId, uds_metadatas,
                odaCommand, contact, _moduleConfiguration.ODAWorkflowRepositoryName, _moduleConfiguration.ODAUdsRepositoryId);

            odaWorkflowReferenceModels.Add(workflowReferenceModelUDS);
            return odaWorkflowReferenceModels.ToArray();
        }
        private async Task<WorkflowReferenceModel[]> BuildRDAWorkflowReferenceModels(RDADocSuiteCommand rdaCommand)
        {
            List<WorkflowReferenceModel> rdaWorkflowReferenceModels = new List<WorkflowReferenceModel>();

            Guid correlationId = Guid.NewGuid();
            Guid fascicleUniqueId = Guid.NewGuid();
            Guid udsId = Guid.NewGuid();

            Contact contact = await GetOrCreateContactAsync(rdaCommand.SupplierPIVACF, rdaCommand.SupplierName);

            List<string> richiedenteValues = new List<string> { rdaCommand.ApplicantArea };

            IDictionary<string, object> uds_metadatas = new Dictionary<string, object>
                {
                    { CommonDefinition.UDSMETADATA_NUMERO, double.Parse(rdaCommand.Number) },
                    { CommonDefinition.UDSMETADATA_DATA, rdaCommand.Date },
                    { CommonDefinition.UDSMETADATA_RICHIEDENTE, JsonConvert.SerializeObject(richiedenteValues)}
                };

            WorkflowReferenceModel fascicleWorkflowReferenceModel = CreateFascicleBuildModel(fascicleUniqueId, correlationId, contact, rdaCommand);
            WorkflowReferenceModel workflowReferenceModelUDS = await CreateUDSBuildModelAsync(correlationId, udsId, fascicleUniqueId, uds_metadatas, 
                rdaCommand, contact, _moduleConfiguration.RDAWorkflowRepositoryName, _moduleConfiguration.RDAUdsRepositoryId);

            rdaWorkflowReferenceModels.Add(fascicleWorkflowReferenceModel);
            rdaWorkflowReferenceModels.Add(workflowReferenceModelUDS);

            return rdaWorkflowReferenceModels.ToArray();
        }
        #endregion

        #region [ Helper Methods ]
        private async Task<Fascicle> TryGetRDACommandFascicleAsync(string rdaReference)
        {
            if (!Guid.TryParse(rdaReference, out Guid rdaCommandId))
            {
                throw new ArgumentException($"Invalid RDA Command id: {rdaReference}");
            }

            RDADocSuiteCommand rdaCommand = await _dbContext.RDADocSuiteCommands.FindAsync(rdaCommandId);

            if (rdaCommand == null)
            {
                throw new ArgumentException($"RDA Command with with id {rdaCommandId} not found");
            }

            Fascicle fascicle = await FindFascicleByRDANumberAsync(rdaCommand.Number);

            if (fascicle == null)
            {
                throw new ArgumentException($"Fascicle not found for RDA command with id {rdaCommandId} and number {rdaCommand.Number}");
            }

            return fascicle;
        }

        private async Task<Fascicle> FindFascicleByRDANumberAsync(string rdaNumber)
        {
            FascicleFinderModel finderModel = new FascicleFinderModel
            {
                IdMetadataRepository = _moduleConfiguration.IdMetadataRepository,
                Top = 1,
                MetadataValues = new List<MetadataFinderModel> 
                {
                    new MetadataFinderModel 
                    {
                        MetadataType = MetadataFinderType.Text,
                        KeyName = CommonDefinition.RDA_METADATA_KEYNAME,
                        Value = rdaNumber 
                    }
                }
            };

            Fascicle fascicle = await _webAPIClient.FindFascicle(finderModel);

            return fascicle;
        }

        private async Task<Contact> GetOrCreateContactAsync(string supplierPIVACF, string supplierName)
        {
            int parentContactId = _moduleConfiguration.RootContact;

            ICollection<Contact> contacts = await _webAPIClient.GetContactAsync($"$filter=FiscalCode eq '{supplierPIVACF}' and IncrementalFather eq {parentContactId}");
            Contact entityContact = contacts.FirstOrDefault();
            if (entityContact == null)
            {
                _logger.WriteDebug(new LogMessage($"Contact with fiscal code '{supplierPIVACF}' not found and it's going to be creating."), LogCategories);

                entityContact = await CreateContactAsync(parentContactId, supplierPIVACF, supplierName); 
            }

            return entityContact;
        }

        private async Task<Contact> CreateContactAsync(int contactFatherId, string fiscalCode, string description)
        {
            Guid uniqueId = Guid.NewGuid();
            Contact contact = new Contact()
            {
                IdContactType = DocSuiteWeb.Entity.Commons.ContactType.Citizen,
                IncrementalFather = contactFatherId,
                Description = description,
                FiscalCode = fiscalCode,
                IsActive = 1,
                UniqueId = uniqueId
            };
            await _webAPIClient.PostAsync(contact);
            contact = (await _webAPIClient.GetContactAsync($"$filter=UniqueId eq {uniqueId}")).Single();
            _logger.WriteInfo(new LogMessage($"Contact '{description}' ({contact.EntityId}) has been create succesfully"), LogCategories);
            return contact;
        }

        private async Task AddDocumentsToUdsModelAsync(UDSModel udsModel, Guid commandId, Guid workflowId)
        {
            List<MainDocument> mainDocuments = _dbContext.MainDocuments.Where(doc => doc.Command.Id == commandId).ToList();
            udsModel.Model.Documents.Document.Instances = await InsertDocumentsAsync(mainDocuments, workflowId);

            List<AttachedDocument> attachedDocuments = _dbContext.AttachedDocuments.Where(doc => doc.Command.Id == commandId).ToList();

            if (attachedDocuments.Count > 0)
            {
                udsModel.Model.Documents.DocumentAttachment.Instances = await InsertDocumentsAsync(attachedDocuments, workflowId);
            }
        }

        private async Task<DocumentInstance[]> InsertDocumentsAsync<TDocument>(ICollection<TDocument> documents, Guid workflowId)
            where TDocument : DocSuiteDocument
        {
            ICollection<ArchiveDocument> archiveDocuments = await _documentClient.InsertDocumentsAsync(documents.Select(doc => new ArchiveDocument
            {
                Archive = _workflowLocation.ProtocolArchive,
                ContentStream = doc.Content,
                Name = doc.FileName
            }).ToList());

            List<DocumentInstance> archiveDocumentsInstances = archiveDocuments.Select(doc => new DocumentInstance
            {
                DocumentName = doc.Name,
                IdDocumentToStore = doc.IdDocument.ToString(),
                StoredChainId = doc.IdChain.ToString()                
            }).ToList();

            foreach (TDocument document in documents)
            {
                document.WFDocSuiteId = workflowId;
                document.WFDocSuiteStarted = DateTimeOffset.UtcNow;
                document.WFDocSuiteStatus = WorkflowStatus.Started;
            }

            return archiveDocumentsInstances.ToArray();
        }

        private async Task<WorkflowReferenceModel> CreateUDSBuildModelAsync<TCommand>(Guid correlationId, Guid udsId, Guid fascicleId, 
            IDictionary<string, object> uds_metadatas, TCommand command, Contact contact, string workflowName, Guid udsRepositoryId)
            where TCommand : DocSuiteCommand
        {
            string commandName = typeof(TCommand).Name;
            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = correlationId
            };

            ICollection<UDSRepository> udsRepositoryApiResult = await _webAPIClient.GetUDSRepository(udsRepositoryId);
            UDSRepository udsRepository = udsRepositoryApiResult.SingleOrDefault();

            if (udsRepository == null)
            {
                throw new ArgumentNullException($"UDS Repository with id {udsRepositoryId} not found");
            }

            UDSModel udsModel = UDSModel.LoadXml(udsRepository.ModuleXML);
            udsModel.Model.UDSId = udsId.ToString();
            udsModel.Model.Subject.Value = $"{command.SupplierName} - {commandName} n° {command.Number} del {command.Date.LocalDateTime.ToShortDateString()}";
            udsModel.FillMetaData(uds_metadatas);
            udsModel = UDSEInvoiceHelper.InitDocumentStructures(udsModel);

            await AddDocumentsToUdsModelAsync(udsModel, command.Id, workflowReferenceModel.ReferenceId);
            
            Contacts contacts = udsModel.Model.Contacts.Single();
            if (contacts.ContactInstances == null)
            {
                contacts.ContactInstances = new ContactInstance[]
                {
                    new ContactInstance() { IdContact = contact.EntityId }
                };
            }

            List<Role> roles = new List<Role>();
            foreach (short roleId in _moduleConfiguration.AuthorizationRoles)
            {
                roles.Add(await _webAPIClient.GetRoleAsync(roleId));
            }

            List<RoleModel> roleModels = roles.Select(role => new RoleModel
            {
                IdRole = role.EntityShortId,
                Name = role.Name,
                TenantId = role.TenantId,
                UniqueId = role.UniqueId,
                RoleLabel = "Autorizzazione",
                FullIncrementalPath = role.FullIncrementalPath
            }).ToList();

            udsModel.Model.Authorizations = udsModel.Model.Authorizations ?? new Authorizations();
            udsModel = UDSEInvoiceHelper.FillAuthorizations(udsModel, roleModels);

            UDSBuildModel udsBuildModel = new UDSBuildModel(udsModel.SerializeToXml())
            {
                WorkflowName = workflowName,
                WorkflowAutoComplete = true,
                WorkflowActions = new List<IWorkflowAction>(),
                Documents = new List<UDSDocumentModel>(),
                Roles = new List<RoleModel>(),
                Users = new List<UserModel>(),
                UniqueId = correlationId,
                UDSRepository = new UDSRepositoryModel(udsRepository.UniqueId)
                {
                    DSWEnvironment = udsRepository.DSWEnvironment,
                    Name = udsRepository.Name
                },
                Subject = udsModel.Model.Subject.Value
            };
            udsBuildModel.Roles = roleModels;
            udsBuildModel.WorkflowActions.Add(new WorkflowActionFascicleModel(
                new FascicleModel { UniqueId = fascicleId },
                new DocumentUnitModel { UniqueId = udsId, Environment = (int)DocSuiteWeb.Entity.Commons.DSWEnvironmentType.UDS }, null));

            workflowReferenceModel.ReferenceType = Entities.Commons.DSWEnvironmentType.Build;
            workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(udsBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);

            return workflowReferenceModel;
        }

        private WorkflowReferenceModel CreateFascicleBuildModel(Guid fascicleUniqueId, Guid correlationId, Contact contact, RDADocSuiteCommand rdaCommand)
        {
            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = correlationId
            };

            ICollection<ContactModel> contactModels = new List<ContactModel> {
                new ContactModel() { Id = _moduleConfiguration.FascicleResponsibleContact }
            };
            CategoryModel categoryModel = new CategoryModel() { IdCategory = _moduleConfiguration.CategoryId };

            ICollection<MetadataRepository> metadataRepo = _webAPIClient.GetMetadataRepositoryAsync($"$filter=UniqueId eq {_moduleConfiguration.IdMetadataRepository}").Result;
            MetadataRepository fascMetadataRepository = metadataRepo.SingleOrDefault();

            ICollection<MetadataValueModel> metadataValueModels = new List<MetadataValueModel>
            {
                new MetadataValueModel { KeyName = CommonDefinition.SUPPLIER_METADATA_KEYNAME, Value = rdaCommand.SupplierName},
                new MetadataValueModel { KeyName = CommonDefinition.RDA_METADATA_KEYNAME, Value = rdaCommand.Number},
                new MetadataValueModel { KeyName = CommonDefinition.ODA_METADATA_KEYNAME, Value = null},
                new MetadataValueModel { KeyName = CommonDefinition.CIG_METADATA_KEYNAME, Value = null},
                new MetadataValueModel { KeyName = CommonDefinition.COSTCENTER_METADATA_KEYNAME, Value = rdaCommand.CostCenter},
                new MetadataValueModel { KeyName = CommonDefinition.COMMENTS_METADATA_KEYNAME, Value = null},
                new MetadataValueModel { KeyName = CommonDefinition.TYPOLOGY_METADATA_KEYNAME, Value = rdaCommand.Typology}
            };

            FascicleBuildModel fascicleBuildModel = new FascicleBuildModel
            {
                WorkflowName = _moduleConfiguration.RDAWorkflowRepositoryName,
                WorkflowAutoComplete = true,
                Fascicle = new FascicleModel
                {
                    UniqueId = fascicleUniqueId,
                    Category = categoryModel,
                    Conservation = _moduleConfiguration.ConservationPeriod,
                    FascicleObject = string.Format(new RDACommandFormatter(), _moduleConfiguration.FascicleObject, rdaCommand),
                    MetadataDesigner = fascMetadataRepository.JsonMetadata,
                    MetadataValues = JsonConvert.SerializeObject(metadataValueModels),
                    MetadataRepository = new MetadataRepositoryModel() { Id = _moduleConfiguration.IdMetadataRepository },
                    FascicleType = Entities.Fascicles.FascicleType.Procedure,
                    Note = contact.Description,
                    StartDate = DateTimeOffset.UtcNow,
                    Contacts = contactModels
                }
            };

            workflowReferenceModel.ReferenceType = Entities.Commons.DSWEnvironmentType.Build;
            workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(fascicleBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);
            return workflowReferenceModel;
        }
        #endregion

        #region [ Validation Methods ] 
        private void ValidatePreventivoCommand(PreventivoDocSuiteCommand command)
        {
            ValidateCommandBaseFields(command, DocSuiteCommandType.Preventivo);

            if (string.IsNullOrEmpty(command.RDAReference))
            {
                throw new ArgumentException($"Undefined Riferimento_RDA for ODA Command with id {command.Id}");
            }
        }

        private void ValidateODACommand(ODADocSuiteCommand command)
        {
            ValidateCommandBaseFields(command, DocSuiteCommandType.ODA);

            if (string.IsNullOrEmpty(command.RDAReference))
            {
                throw new ArgumentException($"Undefined Riferimento_RDA for ODA Command with id {command.Id}");
            }

            if (string.IsNullOrEmpty(command.CostCenter))
            {
                throw new ArgumentException($"Undefined Centro_Costo for ODA Command with id {command.Id}");
            }

            if (string.IsNullOrEmpty(command.CIG))
            {
                throw new ArgumentException($"Undefined CIG for ODA Command with id {command.Id}");
            }
        }

        private async Task ValidateRDACommandAsync(RDADocSuiteCommand command)
        {
            ValidateCommandBaseFields(command, DocSuiteCommandType.RDA);

            Fascicle existingFascicle = await FindFascicleByRDANumberAsync(command.Number);

            if (existingFascicle != null)
            {
                throw new ArgumentException($"Already existing a fascicle for RDA Command (fascicleId: {existingFascicle.UniqueId} -> commandId: {command.Id}");
            }

            if (string.IsNullOrEmpty(command.CostCenter))
            {
                throw new ArgumentException($"Undefined Centro_Costo for RDA Command with id {command.Id}");
            }

            if (string.IsNullOrEmpty(command.Typology))
            {
                throw new ArgumentException($"Undefined Tipologia for RDA Command with id {command.Id}");
            }

            if (string.IsNullOrEmpty(command.ApplicantArea))
            {
                throw new ArgumentException($"Undefined Area_Richiedente for RDA Command with id {command.Id}");
            }
        }

        private void ValidateCommandBaseFields(DocSuiteCommand command, DocSuiteCommandType commandType)
        {
            if (!_dbContext.MainDocuments.Any(doc => doc.Command.Id == command.Id))
            {
                throw new ArgumentException($"Undefined Documento Principale for {commandType} Command with id {command.Id}");
            }

            if (string.IsNullOrEmpty(command.SupplierName))
            {
                throw new ArgumentException($"Undefined Fornitore_Denominazione for {commandType} Command with id {command.Id}");
            }

            if (string.IsNullOrEmpty(command.SupplierPIVACF))
            {
                throw new ArgumentException($"Undefined Fornitore_PIVACF for {commandType} Command with id {command.Id}");
            }

            if (string.IsNullOrEmpty(command.Number))
            {
                throw new ArgumentException($"Undefined Numero for {commandType} Command with id {command.Id}");
            }
        }
        #endregion

        #endregion
    }
}
