using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities.HR;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.HR.Configurations;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.HR.Helpers;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.HR.Models;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.HR.StringFormatters;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Metadata;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders.Metadata;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Models;
using VecompSoftware.Services.Command.CQRS.Events.Models.Dossiers;
using VecompSoftware.Services.Command.CQRS.Events.Models.Fascicles;
using VecompSoftware.Services.Command.CQRS.Events.Models.Protocols;
using DossierType = VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers.DossierType;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.HR
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private bool _needInitializeModule = false;

        private const string DOSSIER_EMPLOYEENUMBER_KEYNAME = "EmployeeNumber";

        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IDocumentClient _documentClient;
        private readonly IServiceBusClient _serviceBusClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private readonly IdentityContext _identityContext;

        private readonly ModuleConfigurationModel _moduleConfiguration;
        private ERPDbContext _dbContext;
        private Location _workflowLocation;

        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            MaxDepth = 10
        };
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

                ICollection<SkyDocCommand> commandsToProcess = _dbContext.SkyDocCommands
                    .Include(c => c.Documents)
                    .Where(command => !command.WFSkyDocStarted.HasValue && !command.WFSkyDocStatus.HasValue).ToList();

                // Process Automatic Protocol commands
                ICollection<SkyDocCommand> automaticProtocolCommands = commandsToProcess.Where(skyDocCommand => skyDocCommand.CommandType == SkyDocCommandType.AutomaticProtocol).ToList();
                ProcessAutomaticProtocolCommands(automaticProtocolCommands).Wait();

                // Process Manual Protocol commands
                // TODO: *Reminder* add workflowrepositories definitions to support this workflow
                //ICollection<SkyDocCommand> manualProtocolCommands = commandsToProcess.Where(skyDocCommand => skyDocCommand.CommandType == SkyDocCommandType.ManualProtocol).ToList();
                //ProcessManualProtocolCommands(manualProtocolCommands).Wait();

                // TODO: uncomment when implementing "Digital Signature" workflow
                //ProcessDigitalSignatureCommands(commandsToProcess.Where(skyDocCommand => skyDocCommand.CommandType == SkyDocCommandType.DigitalSignature).ToList()).Wait();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("***REMOVED***.HR -> Critical error in costruction module"), ex, LogCategories);
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

                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteDossierBuild>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent,
                    _moduleConfiguration.WorkflowStartEHCDossierCreatedSubscription, DossierCreatedWorkflowEventCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteFascicleBuild>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent,
                    _moduleConfiguration.WorkflowStartEHCFascicleCreatedSubscription, FascicleCreatedWorkflowEventCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteProtocolBuild>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent,
                    _moduleConfiguration.WorkflowStartEHCProtocolCreatedSubscription, ProtocolWorkflowEventCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteProtocolDelete>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent,
                    _moduleConfiguration.WorkflowStartEHCProtocolCanceledSubscription, ProtocolWorkflowEventCallback));

                _needInitializeModule = false;
            }
        }

        #region [ Event Callbacks ]
        private async Task DossierCreatedWorkflowEventCallback(IEventCompleteDossierBuild evt, IDictionary<string, object> properties)
        {
            _logger.WriteInfo(new LogMessage($"DossierCreatedWorkflowEventCallback -> received callback with event id {evt.Id}"), LogCategories);

            try
            {
                if (!evt.CorrelationId.HasValue)
                {
                    _logger.WriteError(new LogMessage($"DossierCreatedWorkflowEventCallback -> Undefined CorrelationId for IEventCompleteDossierBuild ({evt.Id})"), LogCategories);
                    throw new ArgumentNullException("CorrelationId", "CorrelationId is null.");
                }

                Guid commandId = evt.CorrelationId.Value;
                DossierBuildModel buildModel = evt.ContentType.ContentTypeValue;
                DossierModel dossier = buildModel.Dossier;

                if (dossier == null)
                {
                    _logger.WriteError(new LogMessage($"DossierCreatedWorkflowEventCallback -> Dossier is null"), LogCategories);
                    throw new ArgumentNullException("Dossier", "Dossier is null.");
                }
                using (ERPDbContext dbContext = new ERPDbContext(_logger, ModuleConfigurationHelper.JsonSerializerSettings, _moduleConfiguration.ConnectionString))
                {
                    _logger.WriteInfo(new LogMessage($"DossierCreatedWorkflowEventCallback -> Creating 'Dossier Creato' skydoc event"), LogCategories);

                    SkyDocCommand eventCommand = await dbContext.SkyDocCommands.FindAsync(commandId);

                    SkyDocEvent dossierCreatedEvent = new SkyDocEvent
                    {
                        Number = dossier.Number.ToString(),
                        Date = dossier.RegistrationDate,
                        EventType = SkyDocEventType.DossierCreated,
                        Subject = dossier.Subject,
                        Year = dossier.Year,
                        EntityUniqueId = dossier.UniqueId,
                        Command = eventCommand
                    };

                    dbContext.SkyDocEvents.Add(dossierCreatedEvent);
                    await dbContext.SaveChangesAsync();

                    _logger.WriteInfo(new LogMessage($"DossierCreatedWorkflowEventCallback -> 'Dossier Creato' skydoc event ({dossierCreatedEvent.Id}/{dossierCreatedEvent.Subject}) has been successfully inserted in the database"), LogCategories);
                    _logger.WriteInfo(new LogMessage($"DossierCreatedWorkflowEventCallback -> SkyDocEvent with id {evt.Id} successfully processed"), LogCategories);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("DossierCreatedWorkflowEventCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private async Task FascicleCreatedWorkflowEventCallback(IEventCompleteFascicleBuild evt, IDictionary<string, object> properties)
        {
            _logger.WriteInfo(new LogMessage($"FascicleCreatedWorkflowEventCallback -> received callback with event id {evt.Id}"), LogCategories);

            try
            {
                if (!evt.CorrelationId.HasValue)
                {
                    _logger.WriteError(new LogMessage($"FascicleCreatedWorkflowEventCallback -> Undefined CorrelationId for IEventCompleteFascicleBuild ({evt.Id})"), LogCategories);
                    throw new ArgumentNullException("CorrelationId", "CorrelationId is null.");
                }

                Guid commandId = evt.CorrelationId.Value;
                FascicleBuildModel buildModel = evt.ContentType.ContentTypeValue;
                FascicleModel fascicle = buildModel.Fascicle;

                if (fascicle == null)
                {
                    _logger.WriteError(new LogMessage($"FascicleCreatedWorkflowEventCallback -> Fascicle is null"), LogCategories);
                    throw new ArgumentNullException("Fascicle", "Fascicle is null.");
                }

                if (fascicle.Category == null)
                {
                    _logger.WriteError(new LogMessage($"FascicleCreatedWorkflowEventCallback -> Undefined Category for fascicle ({fascicle.UniqueId})"), LogCategories);
                    throw new ArgumentNullException("Category", "Category is null.");
                }

                if (!fascicle.RegistrationDate.HasValue)
                {
                    _logger.WriteError(new LogMessage($"FascicleCreatedWorkflowEventCallback -> Undefined RegistrationDate for fascicle ({fascicle.UniqueId})"), LogCategories);
                    throw new ArgumentNullException("RegistrationDate", "RegistrationDate is null.");
                }

                if (!fascicle.Year.HasValue)
                {
                    _logger.WriteError(new LogMessage($"FascicleCreatedWorkflowEventCallback -> Undefined Year for fascicle ({fascicle.UniqueId})"), LogCategories);
                    throw new ArgumentNullException("Year", "Year is null.");
                }

                using (ERPDbContext dbContext = new ERPDbContext(_logger, ModuleConfigurationHelper.JsonSerializerSettings, _moduleConfiguration.ConnectionString))
                {
                    _logger.WriteInfo(new LogMessage($"FascicleCreatedWorkflowEventCallback -> Creating 'Fascicolo Creato' skydoc event"), LogCategories);

                    SkyDocCommand eventCommand = await dbContext.SkyDocCommands.FindAsync(commandId);
                    SkyDocEvent fascicleCreatedEvent = new SkyDocEvent
                    {
                        Number = fascicle.Title,
                        CategoryName = fascicle.Category.Name,
                        Date = fascicle.RegistrationDate.Value,
                        EventType = SkyDocEventType.FascicleCreated,
                        Subject = fascicle.FascicleObject,
                        Year = fascicle.Year.Value,
                        EntityUniqueId = fascicle.UniqueId,
                        Command = eventCommand
                    };

                    dbContext.SkyDocEvents.Add(fascicleCreatedEvent);
                    await dbContext.SaveChangesAsync();

                    _logger.WriteInfo(new LogMessage($"FascicleCreatedWorkflowEventCallback -> 'Fascicolo Creato' skydoc event ({fascicleCreatedEvent.Id}/{fascicleCreatedEvent.Subject}) has been successfully inserted in the database"), LogCategories);
                    _logger.WriteInfo(new LogMessage($"FascicleCreatedWorkflowEventCallback -> SkyDocEvent with id {evt.Id} successfully processed"), LogCategories);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("FascicleCreatedWorkflowEventCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private async Task ProtocolWorkflowEventCallback<TProtocolEvent>(TProtocolEvent evt, IDictionary<string, object> properties)
            where TProtocolEvent : IEventModel<ProtocolBuildModel>
        {
            bool isCreateProtocolEvent = typeof(TProtocolEvent).Name == typeof(IEventCompleteProtocolBuild).Name;
            string protocolEventSpecificCallbackName = isCreateProtocolEvent ? "ProtocolCreateWorkflowEventCallback" : "ProtocolCancelWorkflowEventCallback";
            string protocolEventName = isCreateProtocolEvent ? "Protocollo Creato" : "Protocollo Annullato";

            _logger.WriteInfo(new LogMessage($"{protocolEventSpecificCallbackName} -> received callback with event id {evt.Id}"), LogCategories);

            try
            {
                if (!evt.CorrelationId.HasValue)
                {
                    _logger.WriteError(new LogMessage($"{protocolEventSpecificCallbackName} -> Undefined CorrelationId for event ({evt.Id})"), LogCategories);
                    throw new ArgumentNullException($"CorrelationId", "CorrelationId is null.");
                }

                ProtocolBuildModel buildModel = evt.ContentType.ContentTypeValue;
                ProtocolModel protocol = buildModel.Protocol;
                Guid commandId = evt.CorrelationId.Value;

                if (protocol == null)
                {
                    _logger.WriteError(new LogMessage($"{protocolEventSpecificCallbackName} -> Protocol is null"), LogCategories);
                    throw new ArgumentNullException("Protocol", "Protocol is null.");
                }

                if (protocol.Category == null)
                {
                    _logger.WriteError(new LogMessage($"{protocolEventSpecificCallbackName} -> protocol doesn't have a Category"), LogCategories);
                    throw new ArgumentNullException("Category", "Category is null.");
                }

                using (ERPDbContext dbContext = new ERPDbContext(_logger, ModuleConfigurationHelper.JsonSerializerSettings, _moduleConfiguration.ConnectionString))
                {
                    _logger.WriteInfo(new LogMessage($"{protocolEventSpecificCallbackName} -> Creating '{protocolEventName}' skydoc event"), LogCategories);

                    SkyDocCommand eventCommand = await dbContext.SkyDocCommands.FindAsync(commandId);
                    SkyDocEvent skydocProtocolEvent = new SkyDocEvent
                    {
                        Number = protocol.Number.ToString(),
                        CategoryName = protocol.Category.Name,
                        Date = protocol.RegistrationDate,
                        EventType = isCreateProtocolEvent ? SkyDocEventType.ProtocolCreated : SkyDocEventType.ProtocolCanceled,
                        Subject = protocol.Object,
                        Year = protocol.Year,
                        EntityUniqueId = protocol.UniqueId,
                        Command = eventCommand
                    };

                    dbContext.SkyDocEvents.Add(skydocProtocolEvent);
                    await dbContext.SaveChangesAsync();

                    _logger.WriteInfo(new LogMessage($"{protocolEventSpecificCallbackName} -> '{protocolEventName}' skydoc event ({skydocProtocolEvent.Id}/{skydocProtocolEvent.Subject}) has been successfully inserted in the database"), LogCategories);
                    _logger.WriteInfo(new LogMessage($"{protocolEventSpecificCallbackName} -> SkyDocEvent with id {evt.Id} successfully processed"), LogCategories);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"{protocolEventSpecificCallbackName} -> Critical Error"), ex, LogCategories);
                throw;
            }

        }
        #endregion

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
            _logger.WriteInfo(new LogMessage("OnStop -> ***REMOVED***.HR"), LogCategories);
        }

        #region [ Evaluate Commands Methods ]
        private async Task ProcessAutomaticProtocolCommands(ICollection<SkyDocCommand> automaticProtocolCommands)
        {
            if (!automaticProtocolCommands.Any())
            {
                return;
            }

            _logger.WriteInfo(new LogMessage($"ProcessAutomaticProtocolCommands -> Evaluating Automatic Protocol Commands..."), LogCategories);

            await EvaluateCommandsAsync(automaticProtocolCommands, SkyDocCommandType.AutomaticProtocol);

            _logger.WriteInfo(new LogMessage($"ProcessAutomaticProtocolCommands -> Automatic Protocol Commands have been evaluated"), LogCategories);
        }

        private async Task ProcessManualProtocolCommands(ICollection<SkyDocCommand> manualProtocolCommands)
        {
            if (!manualProtocolCommands.Any())
            {
                return;
            }

            _logger.WriteInfo(new LogMessage($"ProcessManualProtocolCommands -> Evaluating Manual Protocol Commands..."), LogCategories);

            await EvaluateCommandsAsync(manualProtocolCommands, SkyDocCommandType.ManualProtocol);

            _logger.WriteInfo(new LogMessage($"ProcessManualProtocolCommands -> Manual Protocol Commands have been evaluated"), LogCategories);
        }

        // TODO: uncomment when implementing "Digital Signature" workflow
        //private async Task ProcessDigitalSignatureCommands()
        //{
        //    _logger.WriteInfo(new LogMessage($"ProcessDigitalSignatureCommands -> Reading Digital Signature Commands from the database..."), LogCategories);

        //    ICollection<SkyDocCommand> digitalSignatureCommands = _dbContext.SkyDocCommands
        //        .Where(command => command.CommandType == SkyDocCommandType.DigitalSignature && !command.WFSkyDocStarted.HasValue && !command.WFSkyDocStatus.HasValue).ToList();

        //    _logger.WriteInfo(new LogMessage($"ProcessDigitalSignatureCommands -> Digital Signature Commands successfully read from the database"), LogCategories);
        //    _logger.WriteInfo(new LogMessage($"ProcessDigitalSignatureCommands -> Evaluating Digital Signature Commands..."), LogCategories);

        //    await EvaluateCommandsAsync(digitalSignatureCommands, SkyDocCommandType.DigitalSignature);

        //    _logger.WriteInfo(new LogMessage($"ProcessDigitalSignatureCommands -> Digital Signature have been evaluated"), LogCategories);
        //}

        private async Task EvaluateCommandsAsync(ICollection<SkyDocCommand> commandCollection, SkyDocCommandType commandType)
        {
            int successfullCount = 0;
            int totalCount = 0;

            try
            {
                foreach (SkyDocCommand currentCommand in commandCollection)
                {
                    totalCount++;
                    currentCommand.WFSkyDocStarted = DateTimeOffset.UtcNow;

                    try
                    {
                        _logger.WriteInfo(new LogMessage($"EvaluateCommands -> evaluating {commandType} Command with id: {currentCommand.Id}"), LogCategories);

                        switch (commandType)
                        {
                            case SkyDocCommandType.AutomaticProtocol:
                                await WorkflowStartAutomaticProtocolAsync(currentCommand);
                                break;
                            case SkyDocCommandType.ManualProtocol:
                                await WorkflowStartManualProtocolAsync(currentCommand);
                                break;
                            case SkyDocCommandType.DigitalSignature:
                                throw new Exception("'Firma Digitale' command type not implemented");
                            // TODO: uncomment when implementing "Digital Signature" workflow
                            //await WorkflowStartDigitalSignatureAsync(currentCommand);
                            default:
                                throw new Exception($"Unsupported command type {commandType}");
                        }

                        currentCommand.WFSkyDocStatus = ERP.Data.Entities.HR.WorkflowStatus.Started;
                        UpdateCommandDocumentsWorkflowProperties(currentCommand);
                        await _dbContext.SaveChangesAsync();

                        _logger.WriteInfo(new LogMessage($"{commandType} Command with id {currentCommand.Id} and workflow id {currentCommand.WFSkyDocId} has been successfully processed"), LogCategories);

                        successfullCount++;
                    }
                    catch (ArgumentException ex)
                    {
                        _logger.WriteWarning(new LogMessage($"***REMOVED***.HR -> {commandType} command with id {currentCommand.Id} has no pass validation"), ex, LogCategories);
                        currentCommand.WFSkyDocStatus = ERP.Data.Entities.HR.WorkflowStatus.MetadataValidationError;

                        UpdateCommandDocumentsWorkflowProperties(currentCommand);
                        await _dbContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(new LogMessage($"***REMOVED***.HR -> Error on evaluating {commandType} command with id: {currentCommand.Id}"), ex, LogCategories);
                        currentCommand.WFSkyDocStatus = ERP.Data.Entities.HR.WorkflowStatus.Error;

                        UpdateCommandDocumentsWorkflowProperties(currentCommand);
                        await _dbContext.SaveChangesAsync();
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

        private async Task WorkflowStartAutomaticProtocolAsync(SkyDocCommand automaticProtocolCommand)
        {
            try
            {
                CommandValidatorHelper.ValidateCommandFields(automaticProtocolCommand, _webAPIClient);

                Guid correlationId = automaticProtocolCommand.Id;
                automaticProtocolCommand.WFSkyDocId = correlationId;

                WorkflowStart automaticWorkflowModel = await BuildProtocolCommandWorkflowModelAsync(automaticProtocolCommand);
                await StartCommandWorkflowAsync(automaticWorkflowModel, automaticProtocolCommand.Id, "Automatic Protocol Command");
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"WorkflowStartAutomaticProtocolAsync -> Critical Error while starting workflow for Automatic Protocol Command with id {automaticProtocolCommand.Id}"), ex, LogCategories);
                throw;
            }
        }

        private async Task WorkflowStartManualProtocolAsync(SkyDocCommand manualProtocolCommand)
        {
            try
            {
                CommandValidatorHelper.ValidateCommandFields(manualProtocolCommand, _webAPIClient);

                Guid correlationId = manualProtocolCommand.Id;
                manualProtocolCommand.WFSkyDocId = correlationId;

                WorkflowStart commandWorkflowModel = await BuildProtocolCommandWorkflowModelAsync(manualProtocolCommand);

                AppendManualProtocolArguments(manualProtocolCommand, commandWorkflowModel.Arguments);

                await StartCommandWorkflowAsync(commandWorkflowModel, manualProtocolCommand.Id, "Manual Protocol Command");
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"WorkflowStartManualProtocolAsync -> Critical Error while starting workflow for Manual Protocol Command with id {manualProtocolCommand.Id}"), ex, LogCategories);
                throw;
            }
        }

        // TODO: Complete/revise when implementing "Digital Signature" workflow
        //private async Task WorkflowStartDigitalSignatureAsync(SkyDocCommand digitalSignatureCommand)
        //{
        //    try
        //    {
        //        await ValidateDigitalSignatureCommandAsync(digitalSignatureCommand);

        //        Guid correlationId = digitalSignatureCommand.Id;

        //        WorkflowReferenceModel[] digitalSignatureWorkflowRefereceModels = new WorkflowReferenceModel[0];
        //        //await StartCommandWorkflowAsync(_moduleConfiguration.DigitalSignatureWorkflowRepositoryName, digitalSignatureCommand.Id, "Digital Signature Command", digitalSignatureWorkflowRefereceModels);

        //        digitalSignatureCommand.WFSkyDocId = correlationId;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.WriteError(new LogMessage($"WorkflowStartODA -> Critical Error while starting workflow for Digital Signature Command with id {digitalSignatureCommand.Id}"), ex, LogCategories);
        //        throw;
        //    }
        //}

        private async Task<WorkflowStart> BuildProtocolCommandWorkflowModelAsync(SkyDocCommand protocolCommand)
        {
            WorkflowStart workflowStart = new WorkflowStart();
            List<WorkflowReferenceModel> protocolReferenceModels = new List<WorkflowReferenceModel>();

            Dossier protocolDossier = await FindDossierAsync(protocolCommand);
            Fascicle protocolFascicle = await FindFascicleAsync(protocolCommand);

            if (protocolDossier != null && protocolFascicle != null)
            {
                workflowStart.WorkflowName = protocolCommand.CommandType == SkyDocCommandType.AutomaticProtocol
                    ? _moduleConfiguration.AutomaticProtocolWorkflowRepositoryName
                    : _moduleConfiguration.ManualProtocolWorkflowRepositoryName;
                WorkflowReferenceModel protocolCommandWorkflowReferenceModel = protocolCommand.CommandType == SkyDocCommandType.AutomaticProtocol
                    ? await BuildAutomaticProtocolReferenceModelAsync(protocolCommand, _moduleConfiguration.AutomaticProtocolWorkflowRepositoryName, protocolFascicle.UniqueId)
                    : await BuildManualProtocolReferenceModelAsync(protocolCommand);

                protocolReferenceModels.Add(protocolCommandWorkflowReferenceModel);
            }

            Guid protocolFascicleId = Guid.NewGuid();
            if (protocolDossier != null && protocolFascicle == null)
            {
                workflowStart.WorkflowName = _moduleConfiguration.FascicleProtocolWorkflowRepositoryName;
                protocolReferenceModels = await BuildFascicleProtocolReferenceModelsAsync(protocolCommand, protocolDossier.UniqueId, protocolFascicleId);
            }

            if (protocolDossier == null)
            {
                workflowStart.WorkflowName = _moduleConfiguration.FullProtocolWorkflowRepositoryName;
                protocolReferenceModels = await BuildFullProtocolReferenceModelsAsync(protocolCommand, protocolFascicleId);
            }

            // TODO: *Reminder* docsuite doesn't analyze the DSW_ACTION_TO_FASCICLE argument in ProtInserimento page
            if (protocolCommand.CommandType == SkyDocCommandType.ManualProtocol)
            {
                workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_ACTION_TO_FASCICLE, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.PropertyGuid,
                    Name = WorkflowPropertyHelper.DSW_ACTION_TO_FASCICLE,
                    ValueGuid = protocolFascicle == null ? protocolFascicleId : protocolFascicle.UniqueId
                });
            }

            AppendWorkflowReferenceModelsSteps(protocolReferenceModels, workflowStart.Arguments);
            await AppendTenantArgumentsAsync(protocolCommand.TenantId, workflowStart.Arguments);

            return workflowStart;
        }

        private async Task<WorkflowReferenceModel> BuildManualProtocolReferenceModelAsync(SkyDocCommand command)
        {
            ProtocolModel protocolModel = await CreateProtocolModelAsync(command);
            WorkflowReferenceModel protocolReferenceModel = WorkflowReferenceModelHelper.CreateWorkflowReferenceModel(protocolModel, command.Id);

            return protocolReferenceModel;
        }

        private async Task<WorkflowReferenceModel> BuildAutomaticProtocolReferenceModelAsync(SkyDocCommand command, string workflowName, Guid fascicleId)
        {
            ProtocolBuildModel protocolBuildModel = await CreateProtocolBuildModelAsync(command, workflowName, fascicleId);
            WorkflowReferenceModel protocolReferenceModel = WorkflowReferenceModelHelper.CreateWorkflowReferenceModel(protocolBuildModel, command.Id);

            return protocolReferenceModel;
        }

        private async Task<List<WorkflowReferenceModel>> BuildFascicleProtocolReferenceModelsAsync(SkyDocCommand command, Guid dossierId, Guid fascicleId)
        {
            FascicleBuildModel fascicleBuildModel = await CreateFascicleBuildModelAsync(command, _moduleConfiguration.FascicleProtocolWorkflowRepositoryName, fascicleId, dossierId);
            WorkflowReferenceModel fascicleReferenceModel = WorkflowReferenceModelHelper.CreateWorkflowReferenceModel(fascicleBuildModel, command.Id);

            WorkflowReferenceModel protocolReferenceModel = command.CommandType == SkyDocCommandType.ManualProtocol
                ? await BuildManualProtocolReferenceModelAsync(command)
                : await BuildAutomaticProtocolReferenceModelAsync(command, _moduleConfiguration.FascicleProtocolWorkflowRepositoryName, fascicleId);

            return new List<WorkflowReferenceModel> { fascicleReferenceModel, protocolReferenceModel };
        }

        private async Task<List<WorkflowReferenceModel>> BuildFullProtocolReferenceModelsAsync(SkyDocCommand command, Guid fascicleId)
        {
            Guid dossierId = Guid.NewGuid();

            DossierBuildModel dossierBuildModel = await CreateDossierBuildModelAsync(command, _moduleConfiguration.FullProtocolWorkflowRepositoryName, dossierId);
            WorkflowReferenceModel dossierReferenceModel = WorkflowReferenceModelHelper.CreateWorkflowReferenceModel(dossierBuildModel, command.Id);

            FascicleBuildModel fascicleBuildModel = await CreateFascicleBuildModelAsync(command, _moduleConfiguration.FullProtocolWorkflowRepositoryName, fascicleId, dossierId);
            WorkflowReferenceModel fascicleReferenceModel = WorkflowReferenceModelHelper.CreateWorkflowReferenceModel(fascicleBuildModel, command.Id);

            WorkflowReferenceModel protocolReferenceModel = command.CommandType == SkyDocCommandType.ManualProtocol
                ? await BuildManualProtocolReferenceModelAsync(command)
                : await BuildAutomaticProtocolReferenceModelAsync(command, _moduleConfiguration.FullProtocolWorkflowRepositoryName, fascicleId);

            return new List<WorkflowReferenceModel> { dossierReferenceModel, fascicleReferenceModel, protocolReferenceModel };
        }

        private async Task<DossierBuildModel> CreateDossierBuildModelAsync(SkyDocCommand command, string workflowName, Guid dossierUniqueId)
        {
            MetadataRepository dossierMetadataRepository = await GetDossierMetadataRepositoryAsync();

            List<MetadataValueModel> dossierMetadataValues = new List<MetadataValueModel>
            {
                new MetadataValueModel
                {
                    KeyName = DOSSIER_EMPLOYEENUMBER_KEYNAME,
                    Value = command.DossierReference
                }
            };

            Role authorizedRole = await GetWorkflowMappingTagRole(command.AuthorizedRoleMappingTag, workflowName);

            List<DossierRoleModel> dossierRoles = new List<DossierRoleModel>
            {
                new DossierRoleModel
                {
                    Type = DocSuiteWeb.Model.Commons.AuthorizationRoleType.Responsible,
                    Role = new RoleModel { IdRole = authorizedRole.EntityShortId }
                }
            };

            Container commandContainer = (await _webAPIClient.GetContainerAsync(short.Parse(command.ContainerId))).SingleOrDefault();

            DossierBuildModel buildModel = new DossierBuildModel
            {
                WorkflowName = workflowName,
                WorkflowAutoComplete = true,
                UniqueId = command.Id,
                Dossier = new DossierModel
                {
                    UniqueId = dossierUniqueId,
                    Subject = string.Format(new SkyDocCommandFormatter(), _moduleConfiguration.DossierSubjectFormat, command),
                    ContainerId = commandContainer.EntityShortId,
                    ContainerName = commandContainer.Name,
                    MetadataDesigner = dossierMetadataRepository.JsonMetadata,
                    MetadataRepository = new MetadataRepositoryModel { Id = dossierMetadataRepository.UniqueId },
                    MetadataValues = JsonConvert.SerializeObject(dossierMetadataValues),
                    Roles = dossierRoles,
                    StartDate = DateTimeOffset.UtcNow,
                    Category = new CategoryModel() { IdCategory = _moduleConfiguration.CategoryId },
                    DossierType = DossierType.Person
                }
            };

            return buildModel;
        }

        private async Task<FascicleBuildModel> CreateFascicleBuildModelAsync(SkyDocCommand command, string workflowName, Guid fascicleUniqueId, Guid dossierUniqueId)
        {
            MetadataRepository dossierMetadataRepository = await GetDossierMetadataRepositoryAsync();

            List<MetadataValueModel> fascicleMetadataValues = new List<MetadataValueModel>
            {
                new MetadataValueModel
                {
                    KeyName = DOSSIER_EMPLOYEENUMBER_KEYNAME,
                    Value = command.DossierReference
                }
            };

            Role authorizedRole = await GetWorkflowMappingTagRole(command.AuthorizedRoleMappingTag, workflowName);

            List<FascicleRoleModel> fascicleRoles = new List<FascicleRoleModel>
            {
                new FascicleRoleModel
                {
                    AuthorizationRoleType = DocSuiteWeb.Model.Commons.AuthorizationRoleType.Accounted,
                    IsMaster = false,
                    Role = new RoleModel { IdRole = authorizedRole.EntityShortId }
                }
            };

            FascicleBuildModel buildModel = new FascicleBuildModel
            {
                WorkflowName = workflowName,
                WorkflowAutoComplete = true,
                UniqueId = command.Id,
                Fascicle = new FascicleModel
                {
                    UniqueId = fascicleUniqueId,
                    FascicleObject = command.FascicleReference,
                    FascicleType = DocSuiteWeb.Model.Entities.Fascicles.FascicleType.Activity,
                    VisibilityType = DocSuiteWeb.Model.Entities.Fascicles.VisibilityType.Confidential,
                    MetadataDesigner = dossierMetadataRepository.JsonMetadata,
                    MetadataValues = JsonConvert.SerializeObject(fascicleMetadataValues),
                    MetadataRepository = new MetadataRepositoryModel { Id = dossierMetadataRepository.UniqueId },
                    FascicleRoles = fascicleRoles,
                    Category = new CategoryModel() { IdCategory = short.Parse(command.CategoryId) }
                }
            };

            buildModel.WorkflowActions.Add(new WorkflowActionDocumentUnitLinkModel(
                new DocumentUnitModel { UniqueId = fascicleUniqueId, Category = buildModel.Fascicle.Category, Environment = (int)DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType.Fascicle },
                new DocumentUnitModel { UniqueId = dossierUniqueId, Environment = (int)DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType.Dossier }
                ));

            return buildModel;
        }

        private async Task<ProtocolBuildModel> CreateProtocolBuildModelAsync(SkyDocCommand command, string workflowName, Guid fascicleUniqueId)
        {
            Guid protocolUniqueId = Guid.NewGuid();
            Container container = _webAPIClient.GetContainerAsync(short.Parse(command.ContainerId)).Result.SingleOrDefault();
            ProtocolBuildModel protocolBuildModel = new ProtocolBuildModel
            {
                WorkflowName = workflowName,
                WorkflowAutoComplete = true,
                UniqueId = command.Id,
                Protocol = new ProtocolModel
                {
                    Category = new CategoryModel() { IdCategory = int.Parse(command.CategoryId) },
                    Container = new ContainerModel()
                    {
                        IdContainer = container.EntityShortId,
                        Name = container.Name,
                        ProtLocation = new LocationModel()
                        {
                            IdLocation = container.ProtLocation.EntityShortId,
                            ProtocolArchive = container.ProtLocation.ProtocolArchive
                        }
                    }
                }
            };

            protocolBuildModel.Protocol.UniqueId = protocolUniqueId;
            protocolBuildModel.Protocol.ProtocolType = new ProtocolTypeModel(DocSuiteWeb.Model.Entities.Protocols.ProtocolTypology.Inbound);
            protocolBuildModel.Protocol.Object = command.Object;

            ProtocolContactManual senderContact = await GetContactByDescriptionAsync(command.Contact1);
            protocolBuildModel.Protocol.Contacts.Add(new ProtocolContactModel()
            {
                ComunicationType = DocSuiteWeb.Model.Entities.Commons.ComunicationType.Sender,
                IdContact = senderContact.EntityId,
                Description = senderContact.Description
            });

            ProtocolContactManual recipientContact = await GetContactByDescriptionAsync(command.Contact2);
            protocolBuildModel.Protocol.Contacts.Add(new ProtocolContactModel()
            {
                ComunicationType = DocSuiteWeb.Model.Entities.Commons.ComunicationType.Recipient,
                IdContact = recipientContact.EntityId,
                Description = recipientContact.Description
            });

            Role protocolAuthorizedRole = await GetWorkflowMappingTagRole(command.AuthorizedRoleMappingTag, workflowName);

            protocolBuildModel.Protocol.Roles.Add(new ProtocolRoleModel
            {
                NoteType = DocSuiteWeb.Model.Entities.Protocols.ProtocolRoleNoteType.Accessible,
                Status = DocSuiteWeb.Model.Entities.Protocols.ProtocolRoleStatus.ToEvaluate,
                Role = new RoleModel { IdRole = protocolAuthorizedRole.EntityShortId }
            });

            SkyDocDocument mainCommandDocument = command.Documents.SingleOrDefault(doc => doc.DocumentType == SkyDocDocumentType.MainDocument
                   && !doc.WFSkyDocStarted.HasValue
                   && !doc.WFSkyDocStatus.HasValue);
            protocolBuildModel.Protocol.MainDocument = (await InsertDocumentsAsync(new List<SkyDocDocument> { mainCommandDocument })).FirstOrDefault();

            ICollection<SkyDocDocument> attachedCommandDocuments = command.Documents.Where(doc => doc.DocumentType == SkyDocDocumentType.AttachedDocument
                   && !doc.WFSkyDocStarted.HasValue
                   && !doc.WFSkyDocStatus.HasValue).ToList();
            protocolBuildModel.Protocol.Attachments = await InsertDocumentsAsync(attachedCommandDocuments);

            protocolBuildModel.WorkflowActions.Add(new WorkflowActionFascicleModel(
                new FascicleModel { UniqueId = fascicleUniqueId },
                new DocumentUnitModel { UniqueId = protocolUniqueId, Environment = (int)DocSuiteWeb.Entity.Commons.DSWEnvironmentType.Protocol }, null));

            return protocolBuildModel;
        }

        private async Task<List<DocumentModel>> InsertDocumentsAsync(ICollection<SkyDocDocument> documents)
        {
            ICollection<ArchiveDocument> archiveDocuments = await _documentClient.InsertDocumentsAsync(documents.Select(doc => new ArchiveDocument
            {
                Archive = _workflowLocation.ProtocolArchive,
                ContentStream = doc.Content,
                Name = doc.FileName
            }).ToList());

            return archiveDocuments.Select(archiveDoc => new DocumentModel
            {
                FileName = archiveDoc.Name,
                DocumentToStoreId = archiveDoc.IdDocument
            }).ToList();
        }

        private async Task<ProtocolModel> CreateProtocolModelAsync(SkyDocCommand command)
        {
            ProtocolModel protocolModel = new ProtocolModel
            {
                Object = command.Object
            };

            ProtocolContactManual recipient = await GetContactByDescriptionAsync(command.Contact2);
            if (recipient != null)
            {
                protocolModel.ContactManuals.Add(new ProtocolContactManualModel() { Description = recipient.Description, Address = recipient.Address, ComunicationType = DocSuiteWeb.Model.Entities.Commons.ComunicationType.Recipient });
            }

            ProtocolContactManual sender = await GetContactByDescriptionAsync(command.Contact1);
            if (sender != null)
            {
                protocolModel.ContactManuals.Add(new ProtocolContactManualModel() { Description = sender.Description, ComunicationType = DocSuiteWeb.Model.Entities.Commons.ComunicationType.Sender });
            }

            protocolModel.ProtocolType = new ProtocolTypeModel(DocSuiteWeb.Model.Entities.Protocols.ProtocolTypology.Outgoing);

            SkyDocDocument mainDocument = command.Documents.SingleOrDefault(doc => doc.DocumentType == SkyDocDocumentType.MainDocument
                    && !doc.WFSkyDocStarted.HasValue
                    && !doc.WFSkyDocStatus.HasValue);
            protocolModel.MainDocument = new DocumentModel
            {
                FileName = mainDocument.FileName,
                ContentStream = mainDocument.Content
            };

            ICollection<SkyDocDocument> attachedDocuments = command.Documents.Where(doc => doc.DocumentType == SkyDocDocumentType.AttachedDocument
                    && !doc.WFSkyDocStarted.HasValue
                    && !doc.WFSkyDocStatus.HasValue).ToList();
            protocolModel.Attachments = attachedDocuments.Select(skyDoc => new DocumentModel
            {
                FileName = skyDoc.FileName,
                ContentStream = skyDoc.Content
            }).ToList();

            return protocolModel;
        }

        private async Task StartCommandWorkflowAsync(WorkflowStart workflowModel, Guid commandId, string commandName)
        {
            WorkflowResult workflowResult = await _webAPIClient.StartWorkflow(workflowModel);
            _logger.WriteInfo(new LogMessage($"Workflow started correctly [IsValid: {workflowResult.IsValid}] with instanceId {workflowResult.InstanceId}"), LogCategories);

            if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
            {
                _logger.WriteError(new LogMessage($"An error occured in starting workflow for command with id {commandId} of type {commandName}"), LogCategories);
                throw new Exception(string.Join(", ", workflowResult.Errors));
            }
        }

        #endregion

        #region [ Helper Methods ]

        private async Task<Dossier> FindDossierAsync(SkyDocCommand command)
        {
            MetadataRepository metadataRepository = await GetDossierMetadataRepositoryAsync();

            DossierFinderModel finderModel = new DossierFinderModel();
            finderModel.IdMetadataRepository = metadataRepository.UniqueId;
            finderModel.Top = 1;
            finderModel.MetadataValues = new List<MetadataFinderModel>
            {
                new MetadataFinderModel
                {
                    MetadataType = MetadataFinderType.Text,
                    KeyName = DOSSIER_EMPLOYEENUMBER_KEYNAME,
                    Value = command.DossierReference
                }
            };

            Dossier dossier = await _webAPIClient.FindDossier(finderModel);

            return dossier;
        }

        private async Task<Fascicle> FindFascicleAsync(SkyDocCommand command)
        {
            MetadataRepository metadataRepository = await GetDossierMetadataRepositoryAsync();

            FascicleFinderModel finderModel = new FascicleFinderModel();
            finderModel.IdMetadataRepository = metadataRepository.UniqueId;
            finderModel.Top = 1;
            finderModel.MetadataValues = new List<MetadataFinderModel>
            {
                new MetadataFinderModel
                {
                    MetadataType = MetadataFinderType.Text,
                    KeyName = DOSSIER_EMPLOYEENUMBER_KEYNAME,
                    Value = command.DossierReference
                }
            };

            Fascicle fascicle = await _webAPIClient.FindFascicle(finderModel);

            return fascicle;
        }

        private void UpdateCommandDocumentsWorkflowProperties(SkyDocCommand command)
        {
            ICollection<SkyDocDocument> commandDocuments = command.Documents.Where(doc => !doc.WFSkyDocStatus.HasValue && !doc.WFSkyDocStarted.HasValue).ToList();

            foreach (SkyDocDocument document in commandDocuments)
            {
                document.WFSkyDocId = command.WFSkyDocId;
                document.WFSkyDocStarted = command.WFSkyDocStarted;
                document.WFSkyDocStatus = command.WFSkyDocStatus;
            }
        }

        private async Task<Tenant> GetTenantAsync(Guid tenantId)
        {
            return await _webAPIClient.GetTenantAsync(tenantId, "$expand=TenantAOO");
        }

        private async Task<Role> GetWorkflowMappingTagRole(string mappingTag, string workflowRepositoryName)
        {
            WorkflowRoleMapping roleMap = (await _webAPIClient.GetWorkflowRoleMappingAsync($"$filter=MappingTag eq '{mappingTag}' and WorkflowRepository/Name eq '{workflowRepositoryName}' &$expand=Role")).SingleOrDefault();

            return roleMap.Role;
        }

        private async Task<MetadataRepository> GetDossierMetadataRepositoryAsync()
        {
            ICollection<MetadataRepository> metadataRepo = await _webAPIClient.GetMetadataRepositoryAsync($"$filter=UniqueId eq {_moduleConfiguration.DossierMetadataRepositoryId}");
            MetadataRepository dossierMetadataRepository = metadataRepo.SingleOrDefault();

            return dossierMetadataRepository;
        }

        private async Task<ProtocolContactManual> GetContactByDescriptionAsync(string description)
        {
            ProtocolContactManual contact = (await _webAPIClient.GetProtocolContactManualsAsync($"$filter=Description eq '{description}'")).FirstOrDefault();
            return contact;
        }

        private void AppendManualProtocolArguments(SkyDocCommand manualProtocolCommand, IDictionary<string, WorkflowArgument> workflowArguments)
        {
            workflowArguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MAPPING_TAG, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_MAPPING_TAG,
                ValueString = manualProtocolCommand.ResponsibleRoleMappingTag
            });

            workflowArguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME,
                ValueString = $"Attività - Protocollazione manuale {manualProtocolCommand.Object}"
            });

            workflowArguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER,
                ValueGuid = manualProtocolCommand.Id
            });
        }

        private async Task AppendTenantArgumentsAsync(Guid tenantId, IDictionary<string, WorkflowArgument> workflowArguments)
        {
            Tenant commandTenant = await GetTenantAsync(tenantId);
            workflowArguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = commandTenant.UniqueId
            });
            workflowArguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                ValueGuid = commandTenant.TenantAOO.UniqueId
            });
            workflowArguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = commandTenant.TenantName
            });
        }

        private void AppendWorkflowReferenceModelsSteps(List<WorkflowReferenceModel> workflowReferenceModels, IDictionary<string, WorkflowArgument> workflowArguments)
        {
            if (workflowArguments.Count == 1)
            {
                workflowArguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                    new WorkflowArgument()
                    {
                        Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                        PropertyType = ArgumentType.Json,
                        ValueString = JsonConvert.SerializeObject(workflowReferenceModels[0])
                    });

                return;
            }

            for (int currentStep = 0; currentStep < workflowReferenceModels.Count; currentStep++)
            {
                workflowArguments.Add($"{WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL}_{currentStep}",
                    new WorkflowArgument()
                    {
                        Name = $"{WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL}_{currentStep}",
                        PropertyType = ArgumentType.Json,
                        ValueString = JsonConvert.SerializeObject(workflowReferenceModels[currentStep])
                    });
            }
        }
        #endregion

        #endregion
    }
}
