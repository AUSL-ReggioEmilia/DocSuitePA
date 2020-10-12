using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Events.Models.Workflows;
using VecompSoftware.DocSuite.Public.Core.Models.Commands.Workflows;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuite.WebAPI.Common;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.PECMails;
using VecompSoftware.DocSuiteWeb.Finder.UDS;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.Documents;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.PECMails;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.Workflow;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Events.Models.Workflows;
using CollaborationModel = VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters.CollaborationModel;
using ContactModel = VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters.ContactModel;
using DocumentModel = VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters.DocumentModel;
using DSWEnvironmentType = VecompSoftware.DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType;
using FascicleModel = VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles.FascicleModel;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;
using PECMailActiveType = VecompSoftware.DocSuiteWeb.Model.Entities.PECMails.PECMailActiveType;
using PECMailDirection = VecompSoftware.DocSuiteWeb.Model.Entities.PECMails.PECMailDirection;
using PECMailModel = VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters.PECMailModel;
using PrivateMetadaModel = VecompSoftware.DocSuiteWeb.Model.Metadata;
using StorageDocument = VecompSoftware.DocSuite.Document;
using WFFascicleModel = VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters.FascicleModel;
using WFCategoryModel = VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters.CategoryModel;
using WFContainerModel = VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters.ContainerModel;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Workflows
{
    [LogCategory(LogCategoryDefinition.WEBAPIWORKFLOW)]
    public class StartWorkflowController : ApiController
    {
        #region [ Fields ]

        private static IEnumerable<LogCategory> _logCategories;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        protected readonly Guid _instanceId;
        private readonly IWorkflowStartService _workflowStartService;
        private readonly IParameterEnvService _parameterEnvService;
        private readonly ITopicService _topicService;
        private readonly ICQRSMessageMapper _mapper_cqrsMessageMapper;
        private readonly StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> _documentService;
        private readonly Location _workflowLocation;
        private const string API_ServiceBusAddress = "API-ServiceBusAddress";
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion

        #region [ Constructor ]
        public StartWorkflowController(IDataUnitOfWork unitOfWork, ILogger logger, IWorkflowStartService workflowStartService,
            IParameterEnvService parameterEnvService, ITopicService topicService, ICQRSMessageMapper cqrsMessageMapper,
            StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService, IMapperUnitOfWork mapperUnitOfWork)
            : base()
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _workflowStartService = workflowStartService;
            _topicService = topicService;
            _parameterEnvService = parameterEnvService;
            _mapper_cqrsMessageMapper = cqrsMessageMapper;
            _documentService = documentService;
            _mapperUnitOfWork = mapperUnitOfWork;
            _instanceId = Guid.NewGuid();
            short workflowLocationId = _parameterEnvService.WorkflowLocationId;
            _workflowLocation = _unitOfWork.Repository<Location>().Find(workflowLocationId);
            if (_workflowLocation == null)
            {
                throw new DSWException($"Workflow Location {workflowLocationId} not found", null, DSWExceptionCode.WF_Mapper);
            }

        }
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(StartWorkflowController));
                }
                return _logCategories;
            }
        }

        #endregion

        #region [ Methods ]

        [AcceptVerbs("OPTIONS")]
        [AllowAnonymous]
        public IHttpActionResult Options()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] StartWorkflowCommand startWorkflowCommand)
        {
            try
            {
                _logger.WriteInfo(new LogMessage($"StartWorkflow controller receive message {(startWorkflowCommand == null ? "null" : startWorkflowCommand.GetType().ToString())}"), LogCategories);

                if (startWorkflowCommand == null)
                {
                    _logger.WriteWarning(new LogMessage("Modello json di avvio workflow non è corretto e il deserializzatore non è stato in grado di riprodurre il modello. Verificare la struttura completa del Json."), LogCategories);
                    return BadRequest("Modello json di avvio workflow non è corretto e il deserializzatore non è stato in grado di riprodurre il modello.Verificare la struttura completa del Json.");
                }
                if (startWorkflowCommand.ContentType == null)
                {
                    _logger.WriteWarning(new LogMessage("Modello di avvio workflow non valido. Verificare se il parametro ContentType è stato inizializzato e se la struttura json è corretta."), LogCategories);
                    return BadRequest("Modello di avvio workflow non valido. Verificare se il parametro ContentType è stato inizializzato e se la struttura json è corretta.");
                }
                if (startWorkflowCommand.IdentityContext == null)
                {
                    _logger.WriteWarning(new LogMessage("Modello di avvio workflow non valido. Verificare se il parametro IdentityContext è stato inizializzato e se la struttura json è corretta."), LogCategories);
                    return BadRequest("Modello di avvio workflow non valido. Verificare se il parametro IdentityContext è stato inizializzato e se la struttura json è corretta.");
                }
                if (startWorkflowCommand.IdentityContext.Identity == null)
                {
                    _logger.WriteWarning(new LogMessage("Modello di avvio workflow non valido. Verificare se il parametro IdentityContext.Identity è stato inizializzato e se la struttura json è corretta."), LogCategories);
                    return BadRequest("Modello di avvio workflow non valido. Verificare se il parametro IdentityContext.Identity è stato inizializzato e se la struttura json è corretta.");
                }
                if (string.IsNullOrEmpty(startWorkflowCommand.IdentityContext.Identity.Account))
                {
                    _logger.WriteWarning(new LogMessage("Modello di avvio workflow non valido. Non è stata fornita una identità valida nella proprietà IdentityContext.Identity.Account."), LogCategories);
                    return BadRequest("Modello di avvio workflow non valido. Non è stata fornita una identità valida nella proprietà IdentityContext.Identity.Account.");
                }
                if (startWorkflowCommand.ContentType.Content == null)
                {
                    _logger.WriteWarning(new LogMessage("Modello di avvio workflow non valido. Verificare se il parametro ContentType.Content è stato inizializzato e se la struttura json è corretta."), LogCategories);
                    return BadRequest("Modello di avvio workflow non valido. Verificare se il parametro ContentType.Content è stato inizializzato e se la struttura json è corretta.");
                }
                if (startWorkflowCommand.ContentType.Content.WorkflowParameters == null)
                {
                    _logger.WriteWarning(new LogMessage("Modello di avvio workflow non valido. Verificare se il parametro ContentType.Content.WorkflowParameters è stato inizializzato e se la struttura json è corretta."), LogCategories);
                    return BadRequest("Modello di avvio workflow non valido. Verificare se il parametro ContentType.Content.WorkflowParameters è stato inizializzato e se la struttura json è corretta.");
                }
                WorkflowRepository workflowRepository = _unitOfWork.Repository<WorkflowRepository>().GetPublicWorkflowByName(startWorkflowCommand.ContentType.Content.Name);
                if (workflowRepository == null)
                {
                    _logger.WriteWarning(new LogMessage($"Il nome di workflow '{startWorkflowCommand.ContentType.Content.Name}' non è stato riconosciuto da quelli validi"), LogCategories);
                    return BadRequest($"Il nome di workflow '{startWorkflowCommand.ContentType.Content.Name}' non è stato riconosciuto da quelli validi");
                }

                if (startWorkflowCommand.ContentType.Content.Name.StartsWith("AUSL-RE -"))
                {
                    return await EvaluateAUSLREWorkflowsAsync(startWorkflowCommand, workflowRepository);
                }
                if (startWorkflowCommand.ContentType.Content.Name.StartsWith("AUSL-PC -"))
                {
                    return await EvaluateAUSLPCWorkflowsAsync(startWorkflowCommand, workflowRepository);
                }
                if (startWorkflowCommand.ContentType.Content.Name.StartsWith("SPID "))
                {
                    return await EvaluateSPIDWorkflowsAsync(startWorkflowCommand);
                }
                _logger.WriteWarning(new LogMessage("Modello di avvio workflow semanticamente valido, ma non conforme all'integrazione in essere"), LogCategories);
                return BadRequest("Modello di avvio workflow semanticamente valido, ma non conforme all'integrazione in essere");
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(ex.Message), ex, LogCategories);
                return BadRequest("Modello di avvio workflow semanticamente valido, ma non conforme all'integrazione in essere");
            }
        }

        #region AUSL-RE Workflows
        [NonAction]
        public async Task<IHttpActionResult> EvaluateAUSLREWorkflowsAsync(StartWorkflowCommand startWorkflowCommand, WorkflowRepository workflowRepository)
        {
            _logger.WriteInfo(new LogMessage($"EvaluateAUSLREWorkflowsAsync ...."), LogCategories);

            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_protocol_main_document ...."), LogCategories);
            DocumentModel protocolDocumentModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.ProtocolNames.MAIN_DOCUMENT)?.ParameterModel as DocumentModel;
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_collaboration_main_document ...."), LogCategories);
            DocumentModel collaborationDocumentModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.CollaborationNames.MAIN_DOCUMENT)?.ParameterModel as DocumentModel;
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_pecmail_main_document ...."), LogCategories);
            DocumentModel pecmailDocumentModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.PECMailNames.MAIN_DOCUMENT)?.ParameterModel as DocumentModel;
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_protocol_manage ...."), LogCategories);
            WorkflowParameterModel protocolManageRole = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.ProtocolNames.MANAGE && f.ParameterModel is DocSuiteSectorModel);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_protocol_model ...."), LogCategories);
            WorkflowParameterModel protocolModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.ProtocolNames.PROTOCOL_MODEL && f.ParameterModel is CollaborationModel);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_collaboration_model ...."), LogCategories);
            WorkflowParameterModel collaborationModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.CollaborationNames.COLLABORATION_MODEL && f.ParameterModel is CollaborationModel);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_collaboration_manage_model ...."), LogCategories);
            WorkflowParameterModel manageModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.CollaborationNames.DOCUMENT_UNIT_MODEL && f.ParameterModel is CollaborationModel);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_collaboration_signer ...."), LogCategories);
            List<WorkflowParameterModel> signerModels = startWorkflowCommand.ContentType.Content.WorkflowParameters.Where(f => f.ParameterName == WorkflowParameterNames.CollaborationNames.SIGNER && f.ParameterModel is SignerModel).ToList();
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_collaboration_manage ...."), LogCategories);
            List<WorkflowParameterModel> manageModels = startWorkflowCommand.ContentType.Content.WorkflowParameters.Where(f => f.ParameterName == WorkflowParameterNames.CollaborationNames.MANAGE && f.ParameterModel is DocSuiteSectorModel).ToList();
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_pecmail_model ...."), LogCategories);
            WorkflowParameterModel pecmailModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.PECMailNames.PECMAIL_MODEL && f.ParameterModel is PECMailModel);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_fascicle_model ...."), LogCategories);
            WorkflowParameterModel fascicleModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.FascicleNames.FASCICLE_MODEL && f.ParameterModel is WFFascicleModel);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_fascicle_manage ...."), LogCategories);
            WorkflowParameterModel fascicleManageRole = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.FascicleNames.MANAGE && f.ParameterModel is DocSuiteSectorModel);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_fascicle_folder ...."), LogCategories);
            WorkflowParameterModel fascicleFolder = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.FascicleNames.FASCICLE_FOLDER && f.ParameterModel is FolderModel);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_fascicle_miscellanea ...."), LogCategories);
            ICollection<WorkflowParameterModel> fascicleMiscellanea = startWorkflowCommand.ContentType.Content.WorkflowParameters.Where(f => f.ParameterName == WorkflowParameterNames.FascicleNames.FASCICLE_MISCELLANEA && f.ParameterModel is DocumentModel).ToList();
            _logger.WriteInfo(new LogMessage($"Evaluating validation of specific workflow ...."), LogCategories);
            try
            {
                WorkflowResult workflowResult = null;
                if ("AUSL-RE - AVELCO - Protocolla semplice".Equals(startWorkflowCommand.ContentType.Content.Name, StringComparison.InvariantCulture))
                {
                    _logger.WriteInfo(new LogMessage($"Evaluate AUSL-RE - AVELCO - Protocolla semplice ...."), LogCategories);
                    if (protocolManageRole == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il mapping tag / settore che deve gestire la richiesta di protocollazione, vedere sezione > dsw_p_protocol_manage"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il mapping tag / settore che deve gestire la richiesta di protocollazione, vedere sezione > dsw_p_protocol_manage");
                    }

                    if (!workflowRepository.WorkflowRoleMappings.Any(f => f.MappingTag.Equals((protocolManageRole.ParameterModel as DocSuiteSectorModel).MappingTag, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} ha specificato un valore non valido per il parametro MappingTag : {(protocolManageRole.ParameterModel as DocSuiteSectorModel).MappingTag}"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} ha specificato un valore non valido per il parametro MappingTag : {(protocolManageRole.ParameterModel as DocSuiteSectorModel).MappingTag}");
                    }

                    if (protocolModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il protocol model, vedere sezione dsw_p_protocol_model"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il protocol model, vedere sezione dsw_p_protocol_model");
                    }

                    if (protocolDocumentModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato documento principale, vedere sezione > dsw_p_protocol_main_document"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato documento principale, vedere sezione > dsw_p_protocol_main_document");
                    }

                    if (string.IsNullOrEmpty(protocolDocumentModel.DocumentName))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il documento principale non ha il nome del file"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il documento principale non ha il nome del file");
                    }
                    ICollection<MetadataModel> metadatas = (protocolModel.ParameterModel as CollaborationModel).Metadatas;
                    MetadataModel fascicleAction = metadatas.SingleOrDefault(f => f.KeyName == WorkflowPropertyHelper.DSW_ACTION_TO_FASCICLE);
                    IEnumerable<WorkflowRoleMapping> workflowRoleMappings = _unitOfWork.Repository<WorkflowRoleMapping>().GetByMappingTag((protocolManageRole.ParameterModel as DocSuiteSectorModel).MappingTag, workflowRepository.UniqueId);
                    if (fascicleAction != null && fascicleAction.Value.Equals("true"))
                    {
                        _logger.WriteInfo(new LogMessage($"Evaluate AUSL-RE - AVELCO - Protocolla semplice con fascicolazione...."), LogCategories);
                        #region Fascicle Validations
                        _logger.WriteDebug(new LogMessage($"check fascicles metadatas"), LogCategories);

                        MetadataModel metadataAvelco = metadatas.SingleOrDefault(f => f.KeyName == "_ext_Avelco_PraticaId");
                        if (metadataAvelco == null || string.IsNullOrEmpty(metadataAvelco.Value))
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il codice Avelco, vedere sezione _ext_Avelco_PraticaId"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il codice Avelco, vedere sezione _ext_Avelco_PraticaId");
                        }
                        MetadataModel metadata = metadatas.SingleOrDefault(f => f.KeyName == "fascicleCategoryId");
                        short idShort = -1;
                        if (metadata == null || string.IsNullOrEmpty(metadata.Value) || !short.TryParse(metadata.Value, out idShort))
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il metadato fascicleCategoryId"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il metadato fascicleCategoryId");
                        }
                        Category fascicleCategory = _unitOfWork.Repository<Category>().GetById(idShort).SingleOrDefault();
                        if (fascicleCategory == null)
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il metadato fascicleCategoryId non ha un valore valido"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il metadato fascicleCategoryId non ha un valore valido");
                        }
                        metadata = metadatas.SingleOrDefault(f => f.KeyName == "fascicleResponsibleRole");
                        idShort = -1;
                        if (metadata == null || string.IsNullOrEmpty(metadata.Value))
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il metadato fascicleResponsibleRole"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il metadato fascicleResponsibleRole");
                        }
                        Role fascicleResponsibleRole = null;
                        if (!short.TryParse(metadata.Value, out idShort) || (fascicleResponsibleRole = _unitOfWork.Repository<Role>().GetByRoleId(idShort).SingleOrDefault()) == null)
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il metadato fascicleResponsibleRole non ha un valore valido"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il metadato fascicleResponsibleRole non ha un valore valido");
                        }
                        metadata = metadatas.SingleOrDefault(f => f.KeyName == "fascicleAccountedRoles");
                        idShort = -1;
                        if (metadata != null && string.IsNullOrEmpty(metadata.Value))
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il metadato fascicleAccountedRoles"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il metadato fascicleAccountedRoles");
                        }
                        if (metadata != null && (!short.TryParse(metadata.Value, out idShort) || _unitOfWork.Repository<Role>().GetByRoleId(idShort).SingleOrDefault() == null))
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il metadato fascicleAccountedRoles non ha un valore valido"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il metadato fascicleAccountedRoles non ha un valore valido");
                        }
                        metadata = metadatas.SingleOrDefault(f => f.KeyName == "fascicleManagerContact");
                        if (metadata == null || string.IsNullOrEmpty(metadata.Value))
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il metadato fascicleManagerContact"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il metadato fascicleManagerContact");
                        }
                        Contact fascicleManager = _unitOfWork.Repository<Contact>().GetContactBySearchCode(metadata.Value, null).FirstOrDefault();
                        if (fascicleManager == null)
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il metadato fascicleManagerContact non ha un valore valido"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il metadato fascicleManagerContact non ha un valore valido");
                        }
                        metadata = metadatas.SingleOrDefault(f => f.KeyName == "FascicleContainerId");
                        if (metadata == null || string.IsNullOrEmpty(metadata.Value))
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il metadato FascicleContainerId"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il metadato FascicleContainerId");
                        }
                        Container fascicleContainer;
                        if (!short.TryParse(metadata.Value, out idShort) || (fascicleContainer = _unitOfWork.Repository<Container>().Find(idShort)) == null)
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il metadato FascicleContainerId non ha un valore valido"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il metadato FascicleContainerId non ha un valore valido");
                        }
                        MetadataModel fascicleObject = metadatas.SingleOrDefault(f => f.KeyName == "fascicleObject");
                        if (fascicleObject == null || string.IsNullOrEmpty(fascicleObject.Value))
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il metadato fascicleObject"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il metadato fascicleObject");
                        }
                        #endregion
                        workflowResult = await StartWorkflowProtocollaConFascicoloAsync(startWorkflowCommand.Id, startWorkflowCommand, fascicleCategory, fascicleContainer, fascicleResponsibleRole,
                            fascicleManager, fascicleObject, metadataAvelco, protocolManageRole.ParameterModel as DocSuiteSectorModel, protocolDocumentModel, protocolModel.ParameterModel as CollaborationModel, workflowRoleMappings.ToList(),
                            "AUSL-RE - AVELCO - Protocolla con fascicolazione", "Collaborazioni AVELCO");
                    }
                    else
                    {
                        workflowResult = await StartWorkflowProtocollaSempliceAsync(startWorkflowCommand.Id, startWorkflowCommand,
                            protocolManageRole.ParameterModel as DocSuiteSectorModel, protocolDocumentModel, protocolModel.ParameterModel as CollaborationModel, 
                            workflowRoleMappings.Where(f=> f.Role != null).Select(f=> f.Role).ToList(),
                            "AUSL-RE - AVELCO - Protocolla semplice", "Collaborazioni AVELCO");
                    }
                }
                if ("AUSL-RE - AVELCO - Firma digitale con protocollazione".Equals(startWorkflowCommand.ContentType.Content.Name, StringComparison.InvariantCulture))
                {
                    _logger.WriteInfo(new LogMessage($"Evaluate AUSL-RE - AVELCO - Firma digitale con protocollazione...."), LogCategories);

                    if (collaborationModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il collaboration model, vedere sezione dsw_p_collaboration_model"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il collaboration model, vedere sezione dsw_p_collaboration_model");
                    }

                    if (collaborationDocumentModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato documento principale, vedere sezione > dsw_p_collaboration_main_document"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato documento principale, vedere sezione > dsw_p_collaboration_main_document");
                    }

                    if (string.IsNullOrEmpty(collaborationDocumentModel.DocumentName))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il documento principale non ha il nome del file"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il documento principale non ha il nome del file");
                    }

                    if (!signerModels.Any() || !signerModels.Select(f => f.ParameterModel as SignerModel).All(f => f.SignerType == SignerType.AD && f.Identity.Authorization == Core.Models.Securities.AuthorizationType.NTLM && f.Identity.Account.Contains("\\")))
                    {
                        _logger.WriteWarning(new LogMessage($"Il workflow {startWorkflowCommand.ContentType.Content.Name} non ha tutti i parametri richiesti. Verificare se l'account del firmatario è nel formato NTLM corretto col dominio e samAccountName"), LogCategories);
                        return BadRequest($"Il workflow {startWorkflowCommand.ContentType.Content.Name} non ha tutti i parametri richiesti. non ha tutti i parametri richiesti. Verificare se l'account del firmatario è nel formato NTLM corretto col dominio e samAccountName");
                    }

                    if (!manageModels.Any() || !manageModels.Select(f => f.ParameterModel as DocSuiteSectorModel).All(f => !string.IsNullOrEmpty(f.MappingTag)))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non sono stati trovati i mapping tag / settore che deve gestire la richiesta di protocollazione, vedere sezione > dsw_collaboration_manage"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non sono stati trovati i mapping tag / settore che deve gestire la richiesta di protocollazione, vedere sezione > dsw_collaboration_manage");
                    }
                    List<WorkflowRoleMapping> workflowRoleMappings = new List<WorkflowRoleMapping>();
                    foreach (DocSuiteSectorModel item in manageModels.Select(f => f.ParameterModel as DocSuiteSectorModel))
                    {
                        if (!workflowRepository.WorkflowRoleMappings.Any(f => f.MappingTag.Equals(item.MappingTag, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} è stato specificato un non valido per il parametro MappingTag : {item.MappingTag}"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} ha specificato un valore non valido per il parametro MappingTag : {item.MappingTag}");
                        }
                        workflowRoleMappings.AddRange(_unitOfWork.Repository<WorkflowRoleMapping>().GetByMappingTag(item.MappingTag, workflowRepository.UniqueId));
                    }
                    workflowResult = await StartWorkflowProtocollaFirmaDigitaleAsync(startWorkflowCommand.Id, startWorkflowCommand, collaborationDocumentModel, collaborationModel.ParameterModel as CollaborationModel,
                        signerModels.Select(f => f.ParameterModel as SignerModel).ToList(), workflowRoleMappings.Select(f => f.Role).ToList(), manageModel.ParameterModel as CollaborationModel,
                        "AUSL-RE - AVELCO - Firma digitale con protocollazione", "Collaborazioni AVELCO");
                }
                if ("AUSL-RE - ZEN - Firma digitale GASP".Equals(startWorkflowCommand.ContentType.Content.Name, StringComparison.InvariantCulture))
                {
                    _logger.WriteInfo(new LogMessage($"Evaluate AUSL-RE - ZEN - Firma digitale GASP...."), LogCategories);

                    if (collaborationModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il collaboration model, vedere sezione dsw_p_collaboration_model"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il collaboration model, vedere sezione dsw_p_collaboration_model");
                    }

                    if (collaborationDocumentModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato documento principale, vedere sezione > dsw_p_collaboration_main_document"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato documento principale, vedere sezione > dsw_p_collaboration_main_document");
                    }

                    if (string.IsNullOrEmpty(collaborationDocumentModel.DocumentName))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il documento principale non ha il nome del file"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il documento principale non ha il nome del file");
                    }

                    if (!signerModels.Any() || !signerModels.Select(f => f.ParameterModel as SignerModel).All(f => f.SignerType == SignerType.AD && f.Identity.Authorization == Core.Models.Securities.AuthorizationType.NTLM && f.Identity.Account.Contains("\\")))
                    {
                        _logger.WriteWarning(new LogMessage($"Il workflow {startWorkflowCommand.ContentType.Content.Name} non ha tutti i parametri richiesti. Verificare se l'account del firmatario è nel formato NTLM corretto col dominio e samAccountName"), LogCategories);
                        return BadRequest($"Il workflow {startWorkflowCommand.ContentType.Content.Name} non ha tutti i parametri richiesti. non ha tutti i parametri richiesti. Verificare se l'account del firmatario è nel formato NTLM corretto col dominio e samAccountName");
                    }

                    if (!manageModels.Any() || !manageModels.Select(f => f.ParameterModel as DocSuiteSectorModel).All(f => f.SectorRoleId.HasValue))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non sono stati trovati gli identificativi dei settori che devono gestire la richiesta di protocollazione, vedere sezione > dsw_collaboration_manage"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non sono stati trovati gli identificativi dei settori che devono gestire la richiesta di protocollazione, vedere sezione > dsw_collaboration_manage");
                    }
                    List<Role> manageRoles = new List<Role>();
                    Role role = null;
                    foreach (DocSuiteSectorModel item in manageModels.Select(f => f.ParameterModel as DocSuiteSectorModel))
                    {
                        role = _unitOfWork.Repository<Role>().GetByUniqueId(item.SectorRoleId.Value).SingleOrDefault();
                        if (role == null)
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} è stato specificato un identificativo non valido del settore : {item.SectorRoleId}"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} ha specificato un valore un identificativo non valido del settore : {item.SectorRoleId}");
                        }
                        manageRoles.Add(role);
                    }

                    workflowResult = await StartWorkflowProtocollaFirmaDigitaleAsync(startWorkflowCommand.Id, startWorkflowCommand, collaborationDocumentModel, collaborationModel.ParameterModel as CollaborationModel,
                        signerModels.Select(f => f.ParameterModel as SignerModel).ToList(), manageRoles, manageModel.ParameterModel as CollaborationModel, "AUSL-RE - ZEN - Firma digitale GASP", "GASP");
                }
                if ("AUSL-RE - ZEN - Firma digitale Lettera UOIA".Equals(startWorkflowCommand.ContentType.Content.Name, StringComparison.InvariantCulture))
                {
                    _logger.WriteInfo(new LogMessage($"Evaluate AUSL-RE - ZEN - Firma digitale Lettera UOIA...."), LogCategories);

                    if (collaborationModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il collaboration model, vedere sezione dsw_p_collaboration_model"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il collaboration model, vedere sezione dsw_p_collaboration_model");
                    }

                    if (collaborationDocumentModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato documento principale, vedere sezione > dsw_p_collaboration_main_document"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato documento principale, vedere sezione > dsw_p_collaboration_main_document");
                    }

                    if (string.IsNullOrEmpty(collaborationDocumentModel.DocumentName))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il documento principale non ha il nome del file"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il documento principale non ha il nome del file");
                    }

                    if (!signerModels.Any() || !signerModels.Select(f => f.ParameterModel as SignerModel).All(f => f.SignerType == SignerType.AD && f.Identity.Authorization == Core.Models.Securities.AuthorizationType.NTLM && f.Identity.Account.Contains("\\")))
                    {
                        _logger.WriteWarning(new LogMessage($"Il workflow {startWorkflowCommand.ContentType.Content.Name} non ha tutti i parametri richiesti. Verificare se l'account del firmatario è nel formato NTLM corretto col dominio e samAccountName"), LogCategories);
                        return BadRequest($"Il workflow {startWorkflowCommand.ContentType.Content.Name} non ha tutti i parametri richiesti. non ha tutti i parametri richiesti. Verificare se l'account del firmatario è nel formato NTLM corretto col dominio e samAccountName");
                    }

                    if (!manageModels.Any() || !manageModels.Select(f => f.ParameterModel as DocSuiteSectorModel).All(f => f.SectorRoleId.HasValue))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non sono stati trovati gli identificativi dei settori che devono gestire la richiesta di protocollazione, vedere sezione > dsw_collaboration_manage"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non sono stati trovati gli identificativi dei settori che devono gestire la richiesta di protocollazione, vedere sezione > dsw_collaboration_manage");
                    }
                    List<Role> manageRoles = new List<Role>();
                    Role role = null;
                    foreach (DocSuiteSectorModel item in manageModels.Select(f => f.ParameterModel as DocSuiteSectorModel))
                    {
                        role = _unitOfWork.Repository<Role>().GetByUniqueId(item.SectorRoleId.Value).SingleOrDefault();
                        if (role == null)
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} è stato specificato un identificativo non valido del settore : {item.SectorRoleId}"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} ha specificato un valore un identificativo non valido del settore : {item.SectorRoleId}");
                        }
                        manageRoles.Add(role);
                    }
                    if (manageModel.ParameterModel as CollaborationModel != null)
                    {
                        CollaborationModel collaboration = manageModel.ParameterModel as CollaborationModel;
                        if (collaboration.Category != null && collaboration.Category.UniqueId.HasValue)
                        {
                            Category category = _unitOfWork.Repository<Category>().GetByUniqueId(collaboration.Category.UniqueId.Value).SingleOrDefault();
                            if (category == null)
                            {
                                _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} è stato specificato un identificativo di classificatore non valido: {collaboration.Category.UniqueId}"), LogCategories);
                                return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} è stato specificato un identificativo di classificatore non valido: {collaboration.Category.UniqueId}");
                            }
                        }
                        if (collaboration.Container != null && collaboration.Container.UniqueId.HasValue)
                        {
                            Container container = _unitOfWork.Repository<Container>().GetByUniqueId(collaboration.Container.UniqueId.Value).SingleOrDefault();
                            if (container == null)
                            {
                                _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} è stato specificato un identificativo di contenitore non valido: {collaboration.Container.UniqueId}"), LogCategories);
                                return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} è stato specificato un identificativo di contenitore non valido: {collaboration.Container.UniqueId}");
                            }
                        }
                    }

                    workflowResult = await StartWorkflowProtocollaFirmaDigitaleAsync(startWorkflowCommand.Id, startWorkflowCommand, collaborationDocumentModel, collaborationModel.ParameterModel as CollaborationModel,
                        signerModels.Select(f => f.ParameterModel as SignerModel).ToList(), manageRoles, manageModel.ParameterModel as CollaborationModel, "AUSL-RE - ZEN - Firma digitale Lettera UOIA", "Lettera UOIA");
                }
                if ("AUSL-RE - ZEN - Protocolla semplice".Equals(startWorkflowCommand.ContentType.Content.Name, StringComparison.InvariantCulture))
                {
                    _logger.WriteInfo(new LogMessage($"Evaluate AUSL-RE - ZEN - Protocolla semplice ...."), LogCategories);
                    DocSuiteSectorModel docSuiteSector;
                    if (protocolManageRole == null || !(protocolManageRole.ParameterModel is DocSuiteSectorModel) || !(docSuiteSector = protocolManageRole.ParameterModel as DocSuiteSectorModel).SectorRoleId.HasValue)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il mapping tag / settore che deve gestire la richiesta di protocollazione, vedere sezione > dsw_p_protocol_manage"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il mapping tag / settore che deve gestire la richiesta di protocollazione, vedere sezione > dsw_p_protocol_manage");
                    }

                    if (protocolModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il protocol model, vedere sezione dsw_p_protocol_model"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il protocol model, vedere sezione dsw_p_protocol_model");
                    }

                    if (protocolDocumentModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato documento principale, vedere sezione > dsw_p_protocol_main_document"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato documento principale, vedere sezione > dsw_p_protocol_main_document");
                    }

                    if (string.IsNullOrEmpty(protocolDocumentModel.DocumentName))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il documento principale non ha il nome del file"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il documento principale non ha il nome del file");
                    }

                    List<Role> manageRoles = new List<Role>();
                    Role role = _unitOfWork.Repository<Role>().GetByUniqueId(docSuiteSector.SectorRoleId.Value).SingleOrDefault();
                    if (role == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} è stato specificato un identificativo non valido del settore : {docSuiteSector.SectorRoleId}"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} ha specificato un valore un identificativo non valido del settore : {docSuiteSector.SectorRoleId}");
                    }
                    manageRoles.Add(role);
                    workflowResult = await StartWorkflowProtocollaSempliceAsync(startWorkflowCommand.Id, startWorkflowCommand,
                        protocolManageRole.ParameterModel as DocSuiteSectorModel, protocolDocumentModel, protocolModel.ParameterModel as CollaborationModel, manageRoles,
                        "AUSL-RE - ZEN - Protocolla semplice", "Collaborazioni ZEN");
                }
                if ("AUSL-RE - AVELCO - Invia PEC".Equals(startWorkflowCommand.ContentType.Content.Name, StringComparison.InvariantCulture))
                {
                    _logger.WriteInfo(new LogMessage($"Evaluate AUSL-RE - AVELCO - Invia PEC...."), LogCategories);
                    if (pecmailModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il pecmail model, vedere sezione dsw_p_pecmail_model"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il pecmail model, vedere sezione dsw_p_pecmail_model");
                    }

                    if (pecmailDocumentModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato documento principale, vedere sezione > dsw_p_pecmail_main_document"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato documento principale, vedere sezione > dsw_p_pecmail_main_document");
                    }

                    if (string.IsNullOrEmpty(pecmailDocumentModel.DocumentName))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il documento principale non ha il nome del file"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il documento principale non ha il nome del file");
                    }

                    IQueryable<PECMailBox> pECMailBoxes = _unitOfWork.Repository<PECMailBox>().GetPECMailByEmail((pecmailModel.ParameterModel as PECMailModel).Sender, optimization: true);
                    if (!pECMailBoxes.Any() || pECMailBoxes.Count() > 1)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il mittente della PEC {(pecmailModel.ParameterModel as PECMailModel).Sender} non è censito o non è valido"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il mittente della PEC {(pecmailModel.ParameterModel as PECMailModel).Sender} non è censito o non è valido");
                    }

                    workflowResult = await StartWorkflowInviaPECAsync(startWorkflowCommand.Id, startWorkflowCommand, pecmailDocumentModel, pecmailModel.ParameterModel as PECMailModel, pECMailBoxes.Single(),
                        "AUSL-RE - AVELCO - Invia PEC");
                }
                if ("AUSL-RE - AVELCO - Crea Fascicolo".Equals(startWorkflowCommand.ContentType.Content.Name, StringComparison.InvariantCulture))
                {
                    _logger.WriteInfo(new LogMessage($"Evaluate AUSL-RE - AVELCO - Crea Fascicolo...."), LogCategories);
                    if (fascicleModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il fascicle model, vedere sezione dsw_p_fascicle_model"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il fascicle model, vedere sezione dsw_p_fasicle_model");
                    }
                    WFFascicleModel fascicleParameter = (fascicleModel.ParameterModel as WFFascicleModel);
                    if (fascicleParameter.Category == null || !fascicleParameter.Category.UniqueId.HasValue || fascicleParameter.Category.UniqueId == Guid.Empty)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato un classificatore valido"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato un classificatore valido");
                    }
                    Category fascicleCategory = _unitOfWork.Repository<Category>().GetByUniqueId(fascicleParameter.Category.UniqueId.Value).SingleOrDefault();
                    if (fascicleCategory == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il valore del classificatore {fascicleParameter.Category.UniqueId.Value} non è censito o non è valido"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il valore del classificatore {fascicleParameter.Category.UniqueId.Value} non è valido");
                    }
                    if (fascicleManageRole == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il fascicle manage model, vedere sezione dsw_p_fascicle_manage"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il fascicle manage model, vedere sezione dsw_p_fascicle_manage");
                    }
                    if (!(fascicleManageRole.ParameterModel as DocSuiteSectorModel).SectorRoleId.HasValue)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato l'identificativo del settore responsabile di fascicolo"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato l'identificativo del settore responsabile di fascicolo");
                    }
                    Role fascicleResponsibleRole = _unitOfWork.Repository<Role>().GetByUniqueId((fascicleManageRole.ParameterModel as DocSuiteSectorModel).SectorRoleId.Value).SingleOrDefault();
                    if (fascicleResponsibleRole == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il valore del settore responsabile di fascicolo {(fascicleManageRole.ParameterModel as DocSuiteSectorModel).SectorRoleId} non è censito o non è valido"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il valore del settore responsabile di fascicolo {(fascicleManageRole.ParameterModel as DocSuiteSectorModel).SectorRoleId} non è censito o non è valido");
                    }
                    if (fascicleParameter.Manager == null || !fascicleParameter.Manager.ContactId.HasValue || fascicleParameter.Manager.ContactId == Guid.Empty)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato il responsabile di procedimento"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato il responsabile di procedimento");
                    }
                    Contact fascicleManager = _unitOfWork.Repository<Contact>().GetByUniqueId(fascicleParameter.Manager.ContactId.Value, _parameterEnvService.FascicleContactId).FirstOrDefault();
                    if (fascicleManager == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il valore del manager {fascicleParameter.Manager.ContactId.Value} non è censito o non è valido"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il valore del manager {fascicleParameter.Manager.ContactId.Value} non è valido");
                    }
                    if (string.IsNullOrEmpty(fascicleParameter.Subject))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato l'oggetto del fascicolo"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato l'oggetto del fascicolo");
                    }
                    ICollection<Role> fascicleAuthorizedRoles = new List<Role>();
                    Role role = null;
                    foreach (DocSuiteSectorModel item in fascicleParameter.Sectors)
                    {
                        role = _unitOfWork.Repository<Role>().GetByUniqueId(item.SectorRoleId.Value).SingleOrDefault();
                        if (role == null)
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} è stato specificato un identificativo non valido del settore: {item.SectorRoleId}"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} è stato specificato un identificativo non valido del settore: {item.SectorRoleId}");
                        }
                        fascicleAuthorizedRoles.Add(role);
                    }
                    workflowResult = await StartWorkflowCreaFascicoloAsync(startWorkflowCommand.Id, startWorkflowCommand, fascicleParameter, fascicleCategory, fascicleManager, fascicleResponsibleRole, fascicleAuthorizedRoles);
                }
                if ("AUSL-RE - AVELCO - Aggiungi inserto a Fascicolo".Equals(startWorkflowCommand.ContentType.Content.Name, StringComparison.InvariantCulture))
                {
                    _logger.WriteInfo(new LogMessage($"Evaluate AUSL-RE - AVELCO - Aggiungi inserto a Fascicolo...."), LogCategories);
                    if (fascicleModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il fascicle model, vedere sezione dsw_p_fascicle_model"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il fascicle model, vedere sezione dsw_p_fasicle_model");
                    }
                    WFFascicleModel fascicleParameter = (fascicleModel.ParameterModel as WFFascicleModel);
                    if (!fascicleParameter.FascicleId.HasValue || fascicleParameter.FascicleId == Guid.Empty)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato un identificativo del fascicolo valido"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato un identificativo del fascicolo valido");
                    }
                    Fascicle entityFascicle = _unitOfWork.Repository<Fascicle>().GetWithContacts(fascicleParameter.FascicleId.Value, true).SingleOrDefault();
                    if (entityFascicle == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il valore del fascicolo {fascicleParameter.FascicleId} non è censito o non è valido"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il valore del fascicolo {fascicleParameter.FascicleId} non è valido");
                    }
                    FascicleFolder entityFascicleFolder = null;
                    if (fascicleFolder != null && (fascicleFolder.ParameterModel as FolderModel).UniqueId.HasValue)
                    {
                        FolderModel fascicleFolderParameter = (fascicleFolder.ParameterModel as FolderModel);
                        entityFascicleFolder = _unitOfWork.Repository<FascicleFolder>().GetByFolderId(fascicleFolderParameter.UniqueId.Value);
                        if (entityFascicleFolder == null)
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il valore della cartella {fascicleFolderParameter.UniqueId} non è censito o non è valido"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il valore della cartella {fascicleFolderParameter.UniqueId} non è valido");
                        }
                    }
                    if (fascicleMiscellanea == null || fascicleMiscellanea.Count == 0)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato nessun documento, vedere sezione dsw_p_fascicle_miscellanea"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato nessun documento, vedere sezione dsw_p_fascicle_miscellanea");
                    }

                    if (fascicleMiscellanea.Any(x => string.IsNullOrEmpty((x.ParameterModel as DocumentModel).DocumentName)))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} alcuni documenti non hanno il nome del file"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} alcuni documenti non hanno il nome del file");
                    }
                    workflowResult = await StartWorkflowAggiungiInsertoFascicoloAsync(startWorkflowCommand.Id, startWorkflowCommand, entityFascicle,
                        entityFascicleFolder, fascicleMiscellanea.Select(s => (s.ParameterModel as DocumentModel)).ToList());
                }
                if (workflowResult == null)
                {
                    throw new SystemException($"Il workflow '{startWorkflowCommand.ContentType.Content.Name}' specificato non esiste. Verificare se l'identificativo è correttamente censito nel sistema.");
                }
                if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
                {
                    throw new SystemException($"Il workflow '{startWorkflowCommand.ContentType.Content.Name}' non è stato avviato correttamente");
                }
                var result = new { InstanceId = workflowResult.InstanceId.Value, Result = $"Workflow '{startWorkflowCommand.ContentType.Content.Name}' avviato correttamente" };
                _logger.WriteInfo(new LogMessage($"Workflow '{startWorkflowCommand.ContentType.Content.Name}' avviato correttamente con istanza {workflowResult.InstanceId.Value} "), LogCategories);

                return Ok(result.InstanceId);
            }
            catch (Exception ex)
            {
                _logger.WriteWarning(new LogMessage($"Il workflow '{startWorkflowCommand.ContentType.Content.Name}' non è stato avviato correttamente"), ex, LogCategories);
                return BadRequest($"Il workflow '{startWorkflowCommand.ContentType.Content.Name}' non è stato avviato correttamente");
            }
        }

        public async Task<WorkflowResult> StartWorkflowProtocollaSempliceAsync(Guid referenceId, StartWorkflowCommand startWorkflowCommand, DocSuiteSectorModel docSuiteSectorModel,
            DocumentModel documentModel, CollaborationModel workProtocolModel, ICollection<Role> manageRoles, string workflowName, string collaborationTemplateName)
        {
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = workflowName
            };

            if (!string.IsNullOrEmpty(docSuiteSectorModel.MappingTag))
            {
                workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MAPPING_TAG, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.PropertyString,
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_MAPPING_TAG,
                    ValueString = docSuiteSectorModel.MappingTag
                });
            }
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME,
                ValueString = $"Attività - Protocolla documento {documentModel.DocumentName}"
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = startWorkflowCommand.TenantId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                ValueGuid = startWorkflowCommand.TenantAOOId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = startWorkflowCommand.TenantName
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER,
                ValueGuid = referenceId
            });

            CollaborationBuildModel collaborationBuildModel = new CollaborationBuildModel()
            {
                Collaboration = new DocSuiteWeb.Model.Entities.Collaborations.CollaborationModel()
                {
                    TemplateName = collaborationTemplateName,
                    RegistrationName = startWorkflowCommand.IdentityContext.Identity.Name,
                    RegistrationUser = startWorkflowCommand.IdentityContext.Identity.Account,
                    Subject = workProtocolModel.Subject,
                    Account = workProtocolModel.Author,
                    PublicationUser = workProtocolModel.Author,
                    Note = workProtocolModel.Note,
                    MemorandumDate = workProtocolModel.DueDate?.DateTime,
                    IdStatus = CollaborationStatusType.ToProtocol,
                    IdPriority = "N",
                    DocumentType = CastCollaborationDocumentType(workProtocolModel.DocumentUnitType),
                    AlertDate = workProtocolModel.DueDate?.DateTime,
                },
                WorkflowAutoComplete = true,
            };

            foreach (Role role in manageRoles)
            {
                collaborationBuildModel.Collaboration.CollaborationUsers.Add(new CollaborationUserModel()
                {
                    Incremental = 1,
                    IdRole = role.EntityShortId,
                    DestinationType = CollaborationDestinationType.S.ToString(),
                    DestinationFirst = true,
                    DestinationName = role.Name,
                    DestinationEmail = role.EMailAddress,
                });
            }

            ArchiveDocument archiveDocument = await _documentService.InsertDocumentAsync(new ArchiveDocument()
            {
                Archive = _workflowLocation.ProtocolArchive,
                ContentStream = documentModel.Stream,
                Name = documentModel.DocumentName,
            });
            collaborationBuildModel.Collaboration.CollaborationVersionings.Add(new CollaborationVersioningModel()
            {
                CollaborationIncremental = 1,
                Incremental = 1,
                DocumentName = documentModel.DocumentName,
                DocumentGroup = CollaborationDocumentGroupName.MainDocument,
                IsActive = true,
                Document = new DocSuiteWeb.Model.Entities.Commons.DocumentModel()
                {
                    FileName = documentModel.DocumentName,
                    DocumentToStoreId = archiveDocument.IdDocument
                }
            });

            CollaborationProtocolModel protocolModel = new CollaborationProtocolModel
            {
                Subject = workProtocolModel.Subject,
                Note = workProtocolModel.Note
            };
            ContactModel recipient = workProtocolModel.Contacts.SingleOrDefault(f => f.ContactDirectionType == ContactDirectionType.Recipient);
            ContactModel sender = workProtocolModel.Contacts.SingleOrDefault(f => f.ContactDirectionType == ContactDirectionType.Sender);

            if (recipient != null)
            {
                protocolModel.ProtocolContactManuals.Add(new ProtocolContactManualModel() 
                { 
                    Description = recipient.Description, 
                    Address = recipient.Address, 
                    ComunicationType = ComunicationType.Recipient,
                    CertifiedEmail = recipient.PECAddress,
                    EMail = recipient.EmailAddress
                });
            }
            if (sender != null)
            {
                protocolModel.ProtocolContactManuals.Add(new ProtocolContactManualModel() 
                { 
                    Description = sender.Description, 
                    ComunicationType = ComunicationType.Sender,
                    CertifiedEmail = sender.PECAddress,
                    EMail = sender.EmailAddress
                });
            }

            Role docSuiteSector;
            foreach (DocSuiteSectorModel sector in workProtocolModel.Sectors)
            {
                docSuiteSector = _unitOfWork.Repository<Role>().GetByUniqueId(sector.SectorRoleId.Value).SingleOrDefault();
                protocolModel.Roles.Add(new RoleModel() { IdRole = docSuiteSector.EntityShortId });
            }

            protocolModel.ProtocolType = new ProtocolTypeModel(ProtocolTypology.Outgoing);
            collaborationBuildModel.Collaboration.Protocol = protocolModel;

            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = referenceId,
                ReferenceType = DSWEnvironmentType.Build,
                ReferenceModel = JsonConvert.SerializeObject(collaborationBuildModel, Defaults.DefaultJsonSerializer)
            };

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, new WorkflowArgument()
            {
                PropertyType = ArgumentType.Json,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                ValueString = JsonConvert.SerializeObject(workflowReferenceModel, Defaults.DefaultJsonSerializer)
            });
            return await _workflowStartService.CreateAsync(workflowStart);
        }

        public async Task<WorkflowResult> StartWorkflowProtocollaConFascicoloAsync(Guid referenceId, StartWorkflowCommand startWorkflowCommand, Category fascicleCategory,
            Container fascicleContainer, Role fascicleResponsibleRole, Contact fascicleManager, MetadataModel fascicleObject, MetadataModel metadataKey, DocSuiteSectorModel docSuiteSectorModel,
            DocumentModel documentModel, CollaborationModel workProtocolModel, ICollection<WorkflowRoleMapping> workflowRoleMappings, string workflowName, string collaborationTemplateName)
        {
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = workflowName
            };

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MAPPING_TAG, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_MAPPING_TAG,
                ValueString = docSuiteSectorModel.MappingTag
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME,
                ValueString = $"Attività - Protocolla documento {documentModel.DocumentName}"
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = startWorkflowCommand.TenantId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                ValueGuid = startWorkflowCommand.TenantAOOId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = startWorkflowCommand.TenantName
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER,
                ValueGuid = referenceId
            });

            #region Fascicle
            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = referenceId
            };

            ICollection<DocSuiteWeb.Model.Entities.Commons.ContactModel> contactModels = new List<DocSuiteWeb.Model.Entities.Commons.ContactModel>
            {
                new DocSuiteWeb.Model.Entities.Commons.ContactModel() { Id = fascicleManager.EntityId, EntityId = fascicleManager.EntityId  }
            };
            DocSuiteWeb.Model.Entities.Commons.CategoryModel categoryModel = new DocSuiteWeb.Model.Entities.Commons.CategoryModel() { IdCategory = fascicleCategory.EntityShortId };

            FascicleBuildModel fascicleBuildModel = new FascicleBuildModel
            {
                WorkflowName = workflowStart.WorkflowName,
                WorkflowAutoComplete = true,
                UniqueId = referenceId,
                Fascicle = new FascicleModel
                {
                    UniqueId = Guid.NewGuid(),
                    Category = categoryModel,
                    Conservation = 0,
                    FascicleObject = fascicleObject.Value,
                    Note = $"Riferimento esterno {metadataKey.Value}",
                    FascicleType = DocSuiteWeb.Model.Entities.Fascicles.FascicleType.Procedure,
                    StartDate = DateTimeOffset.UtcNow,
                    Contacts = contactModels
                }
            };
            workflowReferenceModel.ReferenceType = DSWEnvironmentType.Build;
            workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(fascicleBuildModel, Defaults.DefaultJsonSerializer);
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, new WorkflowArgument()
            {
                Name = $"{WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL}_0",
                PropertyType = ArgumentType.Json,
                ValueString = JsonConvert.SerializeObject(workflowReferenceModel)
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_ACTION_TO_FASCICLE, new WorkflowArgument()
            {
                Name = WorkflowPropertyHelper.DSW_ACTION_TO_FASCICLE,
                PropertyType = ArgumentType.PropertyGuid,
                ValueGuid = fascicleBuildModel.Fascicle.UniqueId
            });
            #endregion

            #region Collaboration 
            CollaborationBuildModel collaborationBuildModel = new CollaborationBuildModel()
            {
                Collaboration = new DocSuiteWeb.Model.Entities.Collaborations.CollaborationModel()
                {
                    TemplateName = collaborationTemplateName,
                    RegistrationName = startWorkflowCommand.IdentityContext.Identity.Name,
                    RegistrationUser = startWorkflowCommand.IdentityContext.Identity.Account,
                    Subject = workProtocolModel.Subject,
                    Account = workProtocolModel.Author,
                    PublicationUser = workProtocolModel.Author,
                    Note = workProtocolModel.Note,
                    MemorandumDate = workProtocolModel.DueDate?.DateTime,
                    IdStatus = CollaborationStatusType.ToProtocol,
                    IdPriority = "N",
                    DocumentType = CastCollaborationDocumentType(workProtocolModel.DocumentUnitType),
                    AlertDate = workProtocolModel.DueDate?.DateTime,
                },
                WorkflowAutoComplete = true,
            };

            bool hasRole;
            foreach (WorkflowRoleMapping sector in workflowRoleMappings)
            {
                hasRole = sector.Role != null;
                collaborationBuildModel.Collaboration.CollaborationUsers.Add(new CollaborationUserModel()
                {
                    Incremental = 1,
                    IdRole = hasRole ? sector.Role.EntityShortId : (short?)null,
                    DestinationType = hasRole ? CollaborationDestinationType.S.ToString() : CollaborationDestinationType.P.ToString(),
                    DestinationFirst = true,
                    DestinationName = hasRole ? sector.Role.Name : string.Empty,
                    DestinationEmail = hasRole ? sector.Role.EMailAddress : string.Empty,
                });
            }

            ArchiveDocument archiveDocument = await _documentService.InsertDocumentAsync(new ArchiveDocument()
            {
                Archive = _workflowLocation.ProtocolArchive,
                ContentStream = documentModel.Stream,
                Name = documentModel.DocumentName,
            });
            collaborationBuildModel.Collaboration.CollaborationVersionings.Add(new CollaborationVersioningModel()
            {
                CollaborationIncremental = 1,
                Incremental = 1,
                DocumentName = documentModel.DocumentName,
                DocumentGroup = CollaborationDocumentGroupName.MainDocument,
                IsActive = true,
                Document = new DocSuiteWeb.Model.Entities.Commons.DocumentModel()
                {
                    FileName = documentModel.DocumentName,
                    DocumentToStoreId = archiveDocument.IdDocument
                }
            });
            CollaborationProtocolModel protocolModel = new CollaborationProtocolModel
            {
                Subject = workProtocolModel.Subject,
                Note = workProtocolModel.Note
            };
            ContactModel recipient = workProtocolModel.Contacts.SingleOrDefault(f => f.ContactDirectionType == ContactDirectionType.Recipient);
            ContactModel sender = workProtocolModel.Contacts.SingleOrDefault(f => f.ContactDirectionType == ContactDirectionType.Sender);

            if (recipient != null)
            {
                protocolModel.ProtocolContactManuals.Add(new ProtocolContactManualModel() 
                { 
                    Description = recipient.Description, 
                    Address = recipient.Address, 
                    ComunicationType = ComunicationType.Recipient,
                    CertifiedEmail = recipient.PECAddress,
                    EMail = recipient.EmailAddress
                });
            }
            if (sender != null)
            {
                protocolModel.ProtocolContactManuals.Add(new ProtocolContactManualModel() 
                { 
                    Description = sender.Description, 
                    ComunicationType = ComunicationType.Sender,
                    CertifiedEmail = sender.PECAddress,
                    EMail = sender.EmailAddress
                });
            }

            Role docSuiteSector;
            foreach (DocSuiteSectorModel sector in workProtocolModel.Sectors)
            {
                docSuiteSector = _unitOfWork.Repository<Role>().GetByUniqueId(sector.SectorRoleId.Value).SingleOrDefault();
                protocolModel.Roles.Add(new RoleModel() { IdRole = docSuiteSector.EntityShortId });
            }
            protocolModel.ProtocolType = new ProtocolTypeModel(ProtocolTypology.Outgoing);
            collaborationBuildModel.Collaboration.Protocol = protocolModel;
            WorkflowReferenceModel collaborationWorkflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = referenceId,
                ReferenceType = DSWEnvironmentType.Build,
                ReferenceModel = JsonConvert.SerializeObject(collaborationBuildModel, Defaults.DefaultJsonSerializer)
            };
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, new WorkflowArgument()
            {
                PropertyType = ArgumentType.Json,
                Name = $"{WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL}_1",
                ValueString = JsonConvert.SerializeObject(collaborationWorkflowReferenceModel, Defaults.DefaultJsonSerializer)
            });
            #endregion

            return await _workflowStartService.CreateAsync(workflowStart);
        }

        private string CastCollaborationDocumentType(DocumentUnitType documentUnitType)
        {
            switch (documentUnitType)
            {
                case DocumentUnitType.Protocol:
                    {
                        return CollaborationDocumentType.Protocol;
                    }
                case DocumentUnitType.Resolution:
                    {
                        return CollaborationDocumentType.ResolutionDelibera;
                    }
                case DocumentUnitType.DocumentSeries:
                    {
                        return CollaborationDocumentType.DocumentSeries;
                    }
                case DocumentUnitType.Archive:
                    {
                        return CollaborationDocumentType.UDS;
                    }
                default:
                    break;
            }
            return CollaborationDocumentType.Protocol;
        }

        public async Task<WorkflowResult> StartWorkflowProtocollaFirmaDigitaleAsync(Guid referenceId, StartWorkflowCommand startWorkflowCommand, DocumentModel documentModel, CollaborationModel workCollaborationModel,
            List<SignerModel> signerModels, ICollection<Role> manageRoles, CollaborationModel manageModel, string workflowName, string collaborationTemplateName)
        {
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = workflowName
            };

            CollaborationBuildModel collaborationBuildModel = new CollaborationBuildModel()
            {
                Collaboration = new DocSuiteWeb.Model.Entities.Collaborations.CollaborationModel()
                {
                    TemplateName = collaborationTemplateName,
                    RegistrationName = startWorkflowCommand.IdentityContext.Identity.Name,
                    RegistrationUser = startWorkflowCommand.IdentityContext.Identity.Account,
                    Subject = workCollaborationModel.Subject,
                    Account = workCollaborationModel.Author,
                    PublicationUser = workCollaborationModel.Author,
                    Note = workCollaborationModel.Note,
                    MemorandumDate = workCollaborationModel.DueDate?.DateTime,
                    IdStatus = CollaborationStatusType.Insert,
                    IdPriority = "N",
                    DocumentType = CastCollaborationDocumentType(workCollaborationModel.DocumentUnitType),
                    AlertDate = workCollaborationModel.DueDate?.DateTime,
                },
                WorkflowAutoComplete = true,
            };
            short incremental = 1;
            foreach (SignerModel item in signerModels.OrderBy(f => f.Order))
            {
                collaborationBuildModel.Collaboration.CollaborationSigns.Add(new CollaborationSignModel()
                {
                    Incremental = incremental++,
                    IsActive = true,
                    SignUser = item.Identity.Account,
                    SignName = item.Identity.Name,
                    SignEmail = item.Identity.Email,
                    IsRequired = item.Required,
                });
            }
            incremental = 1;
            foreach (Role item in manageRoles)
            {
                collaborationBuildModel.Collaboration.CollaborationUsers.Add(new CollaborationUserModel()
                {
                    Incremental = incremental++,
                    IdRole = item.EntityShortId,
                    DestinationType = CollaborationDestinationType.S.ToString(),
                    DestinationFirst = false,
                    DestinationName = item.Name,
                    DestinationEmail = item.EMailAddress,
                });
            }
            ArchiveDocument archiveDocument = await _documentService.InsertDocumentAsync(new ArchiveDocument()
            {
                Archive = _workflowLocation.ProtocolArchive,
                ContentStream = documentModel.Stream,
                Name = documentModel.DocumentName,
            });
            collaborationBuildModel.Collaboration.CollaborationVersionings.Add(new CollaborationVersioningModel()
            {
                CollaborationIncremental = 1,
                Incremental = 1,
                DocumentName = documentModel.DocumentName,
                DocumentGroup = DocSuiteWeb.Model.Entities.Collaborations.CollaborationDocumentGroupName.MainDocument,
                IsActive = true,
                Document = new DocSuiteWeb.Model.Entities.Commons.DocumentModel()
                {
                    FileName = documentModel.DocumentName,
                    DocumentToStoreId = archiveDocument.IdDocument
                }
            });
            if (manageModel != null)
            {
                CollaborationProtocolModel protocolModel = new DocSuiteWeb.Model.Entities.Collaborations.CollaborationProtocolModel
                {
                    Subject = manageModel.Subject,
                    Note = manageModel.Note,
                };
                if (manageModel.Category != null)
                {
                    Category category = _unitOfWork.Repository<Category>().GetByUniqueId(manageModel.Category.UniqueId.Value).SingleOrDefault();
                    protocolModel.Category = new DocSuiteWeb.Model.Entities.Commons.CategoryModel()
                    {
                        UniqueId = category.UniqueId,
                        Name = category.Name,
                        IdCategory = category.EntityShortId,
                    };
                }
                if (manageModel.Container != null)
                {
                    Container container = _unitOfWork.Repository<Container>().GetByUniqueId(manageModel.Container.UniqueId.Value).SingleOrDefault();
                    protocolModel.Container = new DocSuiteWeb.Model.Entities.Commons.ContainerModel()
                    {
                        UniqueId = container.UniqueId,
                        Name = container.Name,
                        IdContainer = container.EntityShortId,
                    };
                }

                foreach (ContactModel item in manageModel.Contacts.Where(f => f.ContactDirectionType == ContactDirectionType.Recipient))
                {
                    protocolModel.ProtocolContactManuals.Add(new ProtocolContactManualModel() 
                    { 
                        Description = item.Description, 
                        Address = item.Address, 
                        ComunicationType = ComunicationType.Recipient,
                        CertifiedEmail = item.PECAddress,
                        EMail = item.EmailAddress
                    });
                }

                foreach (ContactModel item in manageModel.Contacts.Where(f => f.ContactDirectionType == ContactDirectionType.Sender))
                {
                    protocolModel.ProtocolContactManuals.Add(new ProtocolContactManualModel() 
                    { 
                        Description = item.Description, 
                        ComunicationType = ComunicationType.Sender,
                        CertifiedEmail = item.PECAddress,
                        EMail = item.EmailAddress
                    });
                }

                protocolModel.ProtocolType = new ProtocolTypeModel(ProtocolTypology.Outgoing);
                collaborationBuildModel.Collaboration.Protocol = protocolModel;
            }

            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = referenceId,
                ReferenceType = DSWEnvironmentType.Build,
                ReferenceModel = JsonConvert.SerializeObject(collaborationBuildModel, Defaults.DefaultJsonSerializer)
            };
            //workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MAPPING_TAG, new WorkflowArgument()
            //{
            //    PropertyType = ArgumentType.PropertyString,
            //    Name = WorkflowPropertyHelper.DSW_PROPERTY_MAPPING_TAG,
            //    ValueString = docSuiteSectorModel.MappingTag
            //});
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = startWorkflowCommand.TenantId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                ValueGuid = startWorkflowCommand.TenantAOOId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = startWorkflowCommand.TenantName
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER,
                ValueGuid = referenceId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, new WorkflowArgument()
            {
                PropertyType = ArgumentType.Json,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                ValueString = JsonConvert.SerializeObject(workflowReferenceModel, Defaults.DefaultJsonSerializer)
            });

            return await _workflowStartService.CreateAsync(workflowStart);
        }


        public async Task<WorkflowResult> StartWorkflowInviaPECAsync(Guid referenceId, StartWorkflowCommand startWorkflowCommand, DocumentModel documentModel,
            PECMailModel workPECMailModel, PECMailBox pecMailBox, string workflowName)
        {
            _logger.WriteInfo(new LogMessage($"StartWorkflowInviaPEC pecMailBox: {pecMailBox?.Location?.Name}/{pecMailBox?.Location?.EntityShortId}"), LogCategories);

            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = workflowName
            };
            PECMailBuildModel pecMailBuildModel = new PECMailBuildModel()
            {
                PECMail = new DocSuiteWeb.Model.Entities.PECMails.PECMailModel()
                {
                    Direction = PECMailDirection.Outgoing,
                    IsActive = PECMailActiveType.Active,
                    MailRecipients = workPECMailModel.Recipients,
                    MailRecipientsCc = workPECMailModel.RecipientsCc,
                    MailSenders = pecMailBox.MailBoxRecipient,
                    MailSubject = workPECMailModel.Subject,
                    MailBody = workPECMailModel.Body,
                    PECMailBox = new PECMailBoxModel()
                    {
                        PECMailBoxId = pecMailBox.EntityShortId,
                        UniqueId = pecMailBox.UniqueId,
                        MailBoxRecipient = pecMailBox.MailBoxRecipient,
                        Location = new LocationModel()
                        {
                            IdLocation = pecMailBox.Location.EntityShortId,
                            ProtocolArchive = pecMailBox.Location.ProtocolArchive,
                            UniqueId = pecMailBox.Location.UniqueId,
                        }
                    }
                },
                WorkflowAutoComplete = true
            };
            ArchiveDocument archiveDocument = await _documentService.InsertDocumentAsync(new ArchiveDocument()
            {
                Archive = _workflowLocation.ProtocolArchive,
                ContentStream = documentModel.Stream,
                Name = documentModel.DocumentName,
            });
            pecMailBuildModel.PECMail.Attachments.Add(new DocSuiteWeb.Model.Entities.Commons.DocumentModel()
            {
                FileName = documentModel.DocumentName,
                DocumentToStoreId = archiveDocument.IdDocument
            });

            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = referenceId,
                ReferenceType = DSWEnvironmentType.Build,
                ReferenceModel = JsonConvert.SerializeObject(pecMailBuildModel, Defaults.DefaultJsonSerializer)
            };

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = startWorkflowCommand.TenantId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                ValueGuid = startWorkflowCommand.TenantAOOId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = startWorkflowCommand.TenantName
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER,
                ValueGuid = referenceId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, new WorkflowArgument()
            {
                PropertyType = ArgumentType.Json,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                ValueString = JsonConvert.SerializeObject(workflowReferenceModel, Defaults.DefaultJsonSerializer)
            });

            return await _workflowStartService.CreateAsync(workflowStart);
        }

        public async Task<WorkflowResult> StartWorkflowCreaFascicoloAsync(Guid referenceId, StartWorkflowCommand startWorkflowCommand, WFFascicleModel workFascicleModel,
            Category fascicleCategory, Contact fascicleManager, Role fascicleResponsibleRole, ICollection<Role> fascicleAuthorizedRoles)
        {
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = "AUSL-RE - AVELCO - Crea Fascicolo"
            };

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME,
                ValueString = $"Attività - Crea fascicolo"
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = startWorkflowCommand.TenantId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                ValueGuid = startWorkflowCommand.TenantAOOId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = startWorkflowCommand.TenantName
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER,
                ValueGuid = referenceId
            });

            ICollection<FascicleRoleModel> fascicleRoles = new List<FascicleRoleModel>()
            {
                new FascicleRoleModel() { Role = new RoleModel() { IdRole = fascicleResponsibleRole.EntityShortId }, AuthorizationRoleType = DocSuiteWeb.Model.Commons.AuthorizationRoleType.Responsible, IsMaster = true }
            };
            if (fascicleAuthorizedRoles != null && fascicleAuthorizedRoles.Count > 0)
            {
                foreach (Role fascicleAuthorizedRole in fascicleAuthorizedRoles)
                {
                    fascicleRoles.Add(new FascicleRoleModel() { Role = new RoleModel() { IdRole = fascicleAuthorizedRole.EntityShortId }, AuthorizationRoleType = DocSuiteWeb.Model.Commons.AuthorizationRoleType.Accounted });
                }
            }
            ICollection<DocSuiteWeb.Model.Entities.Commons.ContactModel> contactModels = new List<DocSuiteWeb.Model.Entities.Commons.ContactModel>
            {
                new DocSuiteWeb.Model.Entities.Commons.ContactModel() { Id = fascicleManager.EntityId, EntityId = fascicleManager.EntityId  }
            };
            DocSuiteWeb.Model.Entities.Commons.CategoryModel categoryModel = new DocSuiteWeb.Model.Entities.Commons.CategoryModel() { IdCategory = fascicleCategory.EntityShortId };

            FascicleBuildModel fascicleBuildModel = new FascicleBuildModel
            {
                WorkflowName = workflowStart.WorkflowName,
                WorkflowAutoComplete = true,
                UniqueId = referenceId,
                Fascicle = new FascicleModel
                {
                    UniqueId = Guid.NewGuid(),
                    Category = categoryModel,
                    Conservation = 0,
                    FascicleObject = workFascicleModel.Subject,
                    Note = workFascicleModel.Note,
                    FascicleType = DocSuiteWeb.Model.Entities.Fascicles.FascicleType.Procedure,
                    StartDate = DateTimeOffset.UtcNow,
                    Contacts = contactModels,
                    FascicleRoles = fascicleRoles
                }
            };

            WorkflowActionFascicleCloseModel workflowAction = new WorkflowActionFascicleCloseModel(new FascicleModel() { UniqueId = fascicleBuildModel.Fascicle.UniqueId }) { WorkflowName = workflowStart.WorkflowName };
            if (workFascicleModel.DocumentUnits != null && workFascicleModel.DocumentUnits.Count > 0)
            {
                foreach (DocumentUnitModel documentUnit in workFascicleModel.DocumentUnits)
                {
                    workflowAction.WorkflowActions.Add(new WorkflowActionFascicleModel(new FascicleModel() { UniqueId = fascicleBuildModel.Fascicle.UniqueId },
                        new DocSuiteWeb.Model.Entities.DocumentUnits.DocumentUnitModel() { UniqueId = documentUnit.UniqueId }, null)
                    { CorrelationId = workflowAction.UniqueId, WorkflowName = workflowStart.WorkflowName });
                }
            }
            fascicleBuildModel.WorkflowActions.Add(workflowAction);

            WorkflowRepository workflowRepository = _unitOfWork.Repository<WorkflowRepository>().GetPublicWorkflowByName(workflowStart.WorkflowName, true);
            WorkflowEvaluationProperty fascicleTemplateProperty = _unitOfWork.Repository<WorkflowEvaluationProperty>().GetByPropertyName(WorkflowPropertyHelper.DSW_PROPERTY_TEMPLATE_FASCICLE, workflowRepository.UniqueId, optimization: true).SingleOrDefault();
            Fascicle fascicleTemplate = null;
            if (fascicleTemplateProperty != null && !string.IsNullOrEmpty(fascicleTemplateProperty.ValueString))
            {
                fascicleTemplate = JsonConvert.DeserializeObject<Fascicle>(fascicleTemplateProperty.ValueString);
            }
            if (fascicleTemplate != null)
            {
                fascicleBuildModel.Fascicle.Conservation = fascicleTemplate.Conservation;
                fascicleBuildModel.Fascicle.FascicleType = (DocSuiteWeb.Model.Entities.Fascicles.FascicleType)fascicleTemplate.FascicleType;
                if (fascicleTemplate.MetadataRepository != null)
                {
                    MetadataRepository metadataRepository = _unitOfWork.Repository<MetadataRepository>().Find(fascicleTemplate.MetadataRepository.UniqueId);
                    fascicleBuildModel.Fascicle.MetadataRepository = new MetadataRepositoryModel() { Id = fascicleTemplate.MetadataRepository.UniqueId };
                    fascicleBuildModel.Fascicle.MetadataDesigner = metadataRepository.JsonMetadata;
                }
                if (fascicleTemplate.FascicleFolders.Count > 0)
                {
                    foreach (FascicleFolder fascicleFolder in fascicleTemplate.FascicleFolders)
                    {
                        fascicleBuildModel.Fascicle.FascicleFolders.Add(new FascicleFolderModel() { UniqueId = Guid.NewGuid(), Name = fascicleFolder.Name });
                    }
                }
            }

            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = referenceId,
                ReferenceType = DSWEnvironmentType.Build,
                ReferenceModel = JsonConvert.SerializeObject(fascicleBuildModel, Defaults.DefaultJsonSerializer)
            };

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, new WorkflowArgument()
            {
                PropertyType = ArgumentType.Json,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                ValueString = JsonConvert.SerializeObject(workflowReferenceModel, Defaults.DefaultJsonSerializer)
            });
            return await _workflowStartService.CreateAsync(workflowStart);
        }

        public async Task<WorkflowResult> StartWorkflowAggiungiInsertoFascicoloAsync(Guid referenceId, StartWorkflowCommand startWorkflowCommand, Fascicle fascicle,
            FascicleFolder fascicleFolder, ICollection<DocumentModel> fascicleMiscellanea)
        {
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = "AUSL-RE - AVELCO - Aggiungi inserto a Fascicolo"
            };

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME,
                ValueString = $"Attività - Aggiungi inserto a Fascicolo"
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = startWorkflowCommand.TenantId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                ValueGuid = startWorkflowCommand.TenantAOOId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = startWorkflowCommand.TenantName
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER,
                ValueGuid = referenceId
            });

            FascicleBuildModel fascicleBuildModel = new FascicleBuildModel
            {
                WorkflowName = workflowStart.WorkflowName,
                WorkflowAutoComplete = true,
                UniqueId = referenceId,
                Fascicle = _mapperUnitOfWork.Repository<IFascicleModelMapper>().Map(fascicle, new FascicleModel())
            };

            ArchiveDocument archiveDocument;
            foreach (DocumentModel documentModel in fascicleMiscellanea)
            {
                archiveDocument = await _documentService.InsertDocumentAsync(new ArchiveDocument()
                {
                    Archive = _workflowLocation.ProtocolArchive,
                    ContentStream = documentModel.Stream,
                    Name = documentModel.DocumentName,
                });
                fascicleBuildModel.Fascicle.FascicleDocuments.Add(new FascicleDocumentModel()
                {
                    FascicleFolder = fascicleFolder != null ? new FascicleFolderModel() { UniqueId = fascicleFolder.UniqueId } : null,
                    Document = new DocSuiteWeb.Model.Entities.Commons.DocumentModel()
                    {
                        FileName = documentModel.DocumentName,
                        DocumentToStoreId = archiveDocument.IdDocument
                    }
                });
            }

            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = referenceId,
                ReferenceType = DSWEnvironmentType.Build,
                ReferenceModel = JsonConvert.SerializeObject(fascicleBuildModel, Defaults.DefaultJsonSerializer)
            };

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, new WorkflowArgument()
            {
                PropertyType = ArgumentType.Json,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                ValueString = JsonConvert.SerializeObject(workflowReferenceModel, Defaults.DefaultJsonSerializer)
            });
            return await _workflowStartService.CreateAsync(workflowStart);
        }
        #endregion

        #region AUSL-PC Workflows
        [NonAction]
        public async Task<IHttpActionResult> EvaluateAUSLPCWorkflowsAsync(StartWorkflowCommand startWorkflowCommand, WorkflowRepository workflowRepository)
        {
            _logger.WriteInfo(new LogMessage($"EvaluateAUSLPCWorkflowsAsync ...."), LogCategories);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_integration_event ...."), LogCategories);
            WorkflowParameterModel externalReferenceModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.IntegrationNames.EVENT && f.ParameterModel is ReferenceModel);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_protocol_model ...."), LogCategories);
            WorkflowParameterModel protocolModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.ProtocolNames.PROTOCOL_MODEL && f.ParameterModel is CollaborationModel);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_protocol_main_document ...."), LogCategories);
            WorkflowParameterModel protocolDocumentModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.ProtocolNames.MAIN_DOCUMENT && f.ParameterModel is DocumentModel);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_protocol_attachment_document ...."), LogCategories);
            ICollection<WorkflowParameterModel> protocolAttachmentDocumentModels = startWorkflowCommand.ContentType.Content.WorkflowParameters.Where(f => f.ParameterName == WorkflowParameterNames.ProtocolNames.ATTACHMENT_DOCUMENT && f.ParameterModel is DocumentModel).ToList();
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_protocol_category ...."), LogCategories);
            WorkflowParameterModel protocolCategoryModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.ProtocolNames.CATEGORY_MODEL && f.ParameterModel is WFCategoryModel);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_protocol_container ...."), LogCategories);
            WorkflowParameterModel protocolContainerModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.ProtocolNames.CONTAINER_MODEL && f.ParameterModel is WFContainerModel);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_archive_model ...."), LogCategories);
            WorkflowParameterModel archiveModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.ArchiveNames.ARCHIVE_MODEL && f.ParameterModel is ArchiveModel);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_archive_main_document ...."), LogCategories);
            WorkflowParameterModel archiveDocumentModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.ArchiveNames.MAIN_DOCUMENT && f.ParameterModel is DocumentModel);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_archive_attachment_document ...."), LogCategories);
            ICollection<WorkflowParameterModel> archiveAttachmentDocumentModels = startWorkflowCommand.ContentType.Content.WorkflowParameters.Where(f => f.ParameterName == WorkflowParameterNames.ArchiveNames.ATTACHMENT_DOCUMENT && f.ParameterModel is DocumentModel).ToList();
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_archive_category ...."), LogCategories);
            WorkflowParameterModel archiveCategoryModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.ArchiveNames.CATEGORY_MODEL && f.ParameterModel is WFCategoryModel);
            _logger.WriteInfo(new LogMessage($"Evaluating dsw_p_archive_contact ...."), LogCategories);
            ICollection<WorkflowParameterModel> archiveContactModels = startWorkflowCommand.ContentType.Content.WorkflowParameters.Where(f => f.ParameterName == WorkflowParameterNames.ArchiveNames.CONTACT && f.ParameterModel is ContactModel).ToList();
            _logger.WriteInfo(new LogMessage($"Evaluating validation of specific workflow ...."), LogCategories);
            try
            {
                WorkflowResult workflowResult = null;
                if ("AUSL-PC - SWAF - Notifica evento".Equals(startWorkflowCommand.ContentType.Content.Name, StringComparison.InvariantCulture)
                    || "AUSL-PC - Portale - Notifica evento".Equals(startWorkflowCommand.ContentType.Content.Name, StringComparison.InvariantCulture))
                {
                    _logger.WriteInfo(new LogMessage($"Evaluate {startWorkflowCommand.ContentType.Content.Name}...."), LogCategories);
                    if (externalReferenceModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato l'evento di riferimento, vedere sezione dsw_p_integration_event"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato l'evento di riferimento, vedere sezione dsw_p_integration_event");
                    }
                    ReferenceModel externalEvent = (externalReferenceModel.ParameterModel as ReferenceModel);
                    if (externalEvent.DocSuiteEntityType != DocSuiteEntityType.IntegrationEvent)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} la proprietà DocSuiteEntityType deve corrispondere ad External"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} la proprietà DocSuiteEntityType deve corrispondere ad External");
                    }
                    if (string.IsNullOrEmpty(externalEvent.SerializedModel))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato l'oggetto di riferimento nella proprietà SerializedModel"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato l'oggetto di riferimento nella proprietà SerializedModel");
                    }
                    DocSuiteEvent externalEventModel = null;
                    try
                    {
                        externalEventModel = JsonConvert.DeserializeObject<DocSuiteEvent>(externalEvent.SerializedModel);
                    }
                    catch
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} l'oggetto nella proprietà Reference non corrisponde ad un DocSuiteEvent"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} l'oggetto nella proprietà Reference non corrisponde ad un DocSuiteEvent");
                    }
                    if (externalEventModel.EventModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato un evento valido nella proprietà EventModel"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato un evento valido nella proprietà EventModel");
                    }
                    if (!(externalEventModel.EventModel.CustomProperties.Any(x => x.Key.Equals("SWAFEventType") && !string.IsNullOrEmpty(x.Value))
                            && externalEventModel.EventModel.CustomProperties.Any(x => x.Key.Equals("FiscalCode") && !string.IsNullOrEmpty(x.Value))))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non sono state specificate nella proprietà EventModel > CustomProperties le proprietà \"SWAFEventType\" e \"FiscalCode\""), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non sono state specificate nella proprietà EventModel > CustomProperties le proprietà \"SWAFEventType\" e \"FiscalCode\"");
                    }
                    DocumentUnit documentUnit = null;
                    if (externalEventModel.ReferenceModel != null)
                    {
                        if (externalEventModel.ReferenceModel.UniqueId == Guid.Empty)
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato un identificativo dell'unità documentale valido"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato un identificativo del'unità documentale valido");
                        }
                        documentUnit = _unitOfWork.Repository<DocumentUnit>().GetById(externalEventModel.ReferenceModel.UniqueId).SingleOrDefault();
                        if (documentUnit == null)
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il valore dell'unità documentale {externalEventModel.ReferenceModel.UniqueId} non è censito o non è valido"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il valore dell'unità documentale {externalEventModel.ReferenceModel.UniqueId} non è censito o non è valido");
                        }
                    }
                    workflowResult = await StartWorkflowNotificaSwafAsync(startWorkflowCommand.Id, startWorkflowCommand, externalEventModel, startWorkflowCommand.ContentType.Content.Name);
                }
                if ("AUSL-PC - SWAF - Crea protocollo".Equals(startWorkflowCommand.ContentType.Content.Name, StringComparison.InvariantCulture))
                {
                    _logger.WriteInfo(new LogMessage($"Evaluate AUSL-PC - SWAF - Crea protocollo ...."), LogCategories);
                    if (protocolModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il protocol model, vedere sezione dsw_p_protocol_model"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il protocol model, vedere sezione dsw_p_protocol_model");
                    }

                    if (protocolDocumentModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato documento principale, vedere sezione dsw_p_protocol_main_document"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato documento principale, vedere sezione dsw_p_protocol_main_document");
                    }

                    DocumentModel mainDocument = (protocolDocumentModel.ParameterModel as DocumentModel);
                    if (string.IsNullOrEmpty(mainDocument.DocumentName))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il documento principale non ha il nome del file"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il documento principale non ha il nome del file");
                    }

                    ICollection<DocumentModel> attachments = new List<DocumentModel>();
                    if (protocolAttachmentDocumentModels != null)
                    {
                        attachments = protocolAttachmentDocumentModels.Select(s => (s.ParameterModel as DocumentModel)).ToList();
                    }

                    if (protocolCategoryModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il classificatore, vedere sezione dsw_p_protocol_category"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il classificatore, vedere sezione dsw_p_protocol_category");
                    }
                    WFCategoryModel categoryModel = (protocolCategoryModel.ParameterModel as WFCategoryModel);
                    if (!categoryModel.UniqueId.HasValue || categoryModel.UniqueId == Guid.Empty)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato un classificatore valido"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato un classificatore valido");
                    }
                    Category category = _unitOfWork.Repository<Category>().GetByUniqueId(categoryModel.UniqueId.Value).SingleOrDefault();
                    if (category == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il valore del classificatore {categoryModel.UniqueId} non è censito o non è valido"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il valore del classificatore {categoryModel.UniqueId} non è censito o non è valido");
                    }

                    if (protocolContainerModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il contenitore, vedere sezione dsw_p_protocol_container"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il contenitore, vedere sezione dsw_p_protocol_container");
                    }
                    WFContainerModel containerModel = (protocolContainerModel.ParameterModel as WFContainerModel);
                    if (!containerModel.UniqueId.HasValue || containerModel.UniqueId == Guid.Empty)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato un contenitore valido"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato un contenitore valido");
                    }
                    Container container = _unitOfWork.Repository<Container>().GetByUniqueId(containerModel.UniqueId.Value).SingleOrDefault();
                    if (container == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il valore del contenitore {containerModel.UniqueId} non è censito o non è valido"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il valore del contenitore {containerModel.UniqueId} non è censito o non è valido");
                    }

                    CollaborationModel source = (protocolModel.ParameterModel as CollaborationModel);
                    if (!source.Contacts.Any(x => x.ContactDirectionType == ContactDirectionType.Recipient))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato un destinatario per il protocollo"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato un destinatario per il protocollo");
                    }

                    if (!source.Contacts.Any(x => x.ContactDirectionType == ContactDirectionType.Recipient && !string.IsNullOrEmpty(x.FiscalCode)))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato un destinatario con un codice fiscale valido"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato un destinatario con un codice fiscale valido");
                    }

                    short workflowLocationId = _parameterEnvService.WorkflowLocationId;
                    Location workflowLocation = _unitOfWork.Repository<Location>().Find(workflowLocationId);
                    if (workflowLocation == null)
                    {
                        throw new DSWException($"Workflow Location {workflowLocationId} not found", null, DSWExceptionCode.WF_Mapper);
                    }

                    workflowResult = await StartWorkflowCreaProtocolloSwafAsync(startWorkflowCommand.Id, startWorkflowCommand, source,
                        mainDocument, attachments, category, container, workflowLocation);
                }
                if ("AUSL-PC - SWAF - Crea archivio".Equals(startWorkflowCommand.ContentType.Content.Name, StringComparison.InvariantCulture))
                {
                    _logger.WriteInfo(new LogMessage($"Evaluate AUSL-PC - SWAF - Crea archivio ...."), LogCategories);
                    if (archiveModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato l'archive model, vedere sezione dsw_p_archive_model"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato l'archive model, vedere sezione dsw_p_archive_model");
                    }

                    if (archiveDocumentModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato documento principale, vedere sezione dsw_p_archive_main_document"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato documento principale, vedere sezione dsw_p_archive_main_document");
                    }

                    DocumentModel mainDocument = (archiveDocumentModel.ParameterModel as DocumentModel);
                    if (string.IsNullOrEmpty(mainDocument.DocumentName))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il documento principale non ha il nome del file"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il documento principale non ha il nome del file");
                    }

                    ICollection<DocumentModel> attachments = new List<DocumentModel>();
                    if (archiveAttachmentDocumentModels != null)
                    {
                        attachments = archiveAttachmentDocumentModels.Select(s => (s.ParameterModel as DocumentModel)).ToList();
                        if (attachments.Any(x => string.IsNullOrEmpty(x.DocumentName)))
                        {
                            _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} uno o più allegati non hanno il nome del file"), LogCategories);
                            return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} uno o più allegati non hanno il nome del file");
                        }
                    }

                    ArchiveModel model = (archiveModel.ParameterModel as ArchiveModel);
                    if (string.IsNullOrEmpty(model.ArchiveName))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato il nome dell'archivio"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato specificato il nome dell'archivio");
                    }

                    UDSRepository udsRepository = _unitOfWork.Repository<UDSRepository>().GetByName(model.ArchiveName);
                    if (udsRepository == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} l'archivio {model.ArchiveName} non è censito o non è valido"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} l'archivio {model.ArchiveName} non è censito o non è valido");
                    }

                    if (archiveCategoryModel == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il classificatore dell'archivio, vedere sezione dsw_p_archive_category"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato il classificatore dell'archivio, vedere sezione dsw_p_archive_category");
                    }

                    WFCategoryModel wfCategoryModel = (archiveCategoryModel.ParameterModel as WFCategoryModel);
                    Category category = _unitOfWork.Repository<Category>().GetByUniqueId(wfCategoryModel.UniqueId.Value).SingleOrDefault();
                    if (category == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il classificatore {wfCategoryModel.UniqueId} non è censito o non è valido"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} il classificatore {wfCategoryModel.UniqueId} non è censito o non è valido");
                    }                    

                    VecompSoftware.Helpers.UDS.UDSModel udsRepositoryXmlModel = VecompSoftware.Helpers.UDS.UDSModel.LoadXml(udsRepository.ModuleXML);
                    IDictionary<string, object> metadatas = new Dictionary<string, object>();
                    object metadataValue;
                    MetadataModel metadataModel;
                    foreach (VecompSoftware.Helpers.UDS.Section section in udsRepositoryXmlModel.Model.Metadata)
                    {
                        foreach (VecompSoftware.Helpers.UDS.FieldBaseType sectionField in section.Items)
                        {
                            metadataValue = null;
                            if (model.Metadatas.Any(x => x.KeyName == new VecompSoftware.Helpers.UDS.UDSModelField(sectionField).ColumnName))
                            {
                                metadataModel = model.Metadatas.Single(x => x.KeyName == new VecompSoftware.Helpers.UDS.UDSModelField(sectionField).ColumnName);
                                metadataValue = metadataModel.Value;
                                if (sectionField is VecompSoftware.Helpers.UDS.NumberField)
                                {
                                    metadataValue = int.Parse(metadataModel.Value);
                                }
                                if (sectionField is VecompSoftware.Helpers.UDS.BoolField)
                                {
                                    metadataValue = bool.Parse(metadataModel.Value);
                                }
                                if (sectionField is VecompSoftware.Helpers.UDS.DateField)
                                {
                                    metadataValue = DateTime.Parse(metadataModel.Value);
                                }                                
                            }
                            metadatas.Add(new VecompSoftware.Helpers.UDS.UDSModelField(sectionField).ColumnName, metadataValue);
                        }
                    }

                    if (udsRepositoryXmlModel.Model.Documents == null || udsRepositoryXmlModel.Model.Documents.Document == null)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} è stato definito un archivio senza la sezione relativa al documento principale"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} è stato definito un archivio senza la sezione relativa al documento principale");
                    }

                    if (udsRepositoryXmlModel.Model.Contacts == null || udsRepositoryXmlModel.Model.Contacts.Length == 0)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} è stato definito un archivio senza la sezione relativa ai contatti"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} è stato definito un archivio senza la sezione relativa ai contatti");
                    }

                    if (archiveContactModels == null || archiveContactModels.Count == 0)
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato un contatto dell'archivio, vedere sezione dsw_p_archive_contact"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} non è stato trovato un contatto dell'archivio, vedere sezione dsw_p_archive_contact");
                    }
                    ICollection<ContactModel> contacts = archiveContactModels.Select(s => (s.ParameterModel as ContactModel)).ToList();
                    if (contacts.Any(x => string.IsNullOrEmpty(x.ArchiveSection)))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} per alcuni contatti non è stato trovato il valore relativo all'ArchiveSection"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} per alcuni contatti non è stato trovato il valore relativo all'ArchiveSection");
                    }
                    if (!contacts.All(x => udsRepositoryXmlModel.Model.Contacts.Any(c => c.Label == x.ArchiveSection)))
                    {
                        _logger.WriteWarning(new LogMessage($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} per alcuni contatti il valore relativo all'ArchiveSection non è presente nell'archivio scelto"), LogCategories);
                        return BadRequest($"Nei parametri del workflow {startWorkflowCommand.ContentType.Content.Name} per alcuni contatti il valore relativo all'ArchiveSection non è presente nell'archivio scelto");
                    }

                    short workflowLocationId = _parameterEnvService.WorkflowLocationId;
                    Location workflowLocation = _unitOfWork.Repository<Location>().Find(workflowLocationId);
                    if (workflowLocation == null)
                    {
                        throw new DSWException($"Workflow Location {workflowLocationId} not found", null, DSWExceptionCode.WF_Mapper);
                    }

                    workflowResult = await StartWorkflowCreaArchivioSwaftAsync(startWorkflowCommand.Id, startWorkflowCommand, model, udsRepository, mainDocument, attachments, category, metadatas, contacts, workflowLocation);
                }
                if (workflowResult == null)
                {
                    throw new SystemException($"Il workflow '{startWorkflowCommand.ContentType.Content.Name}' specificato non esiste. Verificare se l'identificativo è correttamente censito nel sistema.");
                }
                if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
                {
                    throw new SystemException($"Il workflow '{startWorkflowCommand.ContentType.Content.Name}' non è stato avviato correttamente");
                }
                var result = new { InstanceId = workflowResult.InstanceId.Value, Result = $"Workflow '{startWorkflowCommand.ContentType.Content.Name}' avviato correttamente" };
                _logger.WriteInfo(new LogMessage($"Workflow '{startWorkflowCommand.ContentType.Content.Name}' avviato correttamente con istanza {workflowResult.InstanceId.Value} "), LogCategories);

                return Ok(result.InstanceId);
            }
            catch (Exception ex)
            {
                _logger.WriteWarning(new LogMessage($"Il workflow '{startWorkflowCommand.ContentType.Content.Name}' non è stato avviato correttamente"), ex, LogCategories);
                return BadRequest($"Il workflow '{startWorkflowCommand.ContentType.Content.Name}' non è stato avviato correttamente");
            }
        }

        public async Task<WorkflowResult> StartWorkflowNotificaSwafAsync(Guid referenceId, StartWorkflowCommand startWorkflowCommand, DocSuiteEvent docSuiteEvent, string workflowName)
        {
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = workflowName
            };

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME,
                ValueString = $"Attività - Notifica evento SWAF"
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = startWorkflowCommand.TenantId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                ValueGuid = startWorkflowCommand.TenantAOOId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = startWorkflowCommand.TenantName
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER,
                ValueGuid = referenceId
            });

            DocSuiteEvent toSendEventModel = new DocSuiteEvent()
            {
                WorkflowName = workflowStart.WorkflowName,
                WorkflowAutoComplete = true,
                WorkflowReferenceId = docSuiteEvent.WorkflowReferenceId,
                EventDate = docSuiteEvent.EventDate,
                EventModel = docSuiteEvent.EventModel,
                ReferenceModel = docSuiteEvent.ReferenceModel,
            };

            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = referenceId,
                ReferenceType = DSWEnvironmentType.Build,
                ReferenceModel = JsonConvert.SerializeObject(toSendEventModel, DocSuiteWeb.Service.Workflow.ServiceHelper.SerializerSettings)
            };

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, new WorkflowArgument()
            {
                PropertyType = ArgumentType.Json,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                ValueString = JsonConvert.SerializeObject(workflowReferenceModel, Defaults.DefaultJsonSerializer)
            });
            return await _workflowStartService.CreateAsync(workflowStart);
        }

        public async Task<WorkflowResult> StartWorkflowCreaProtocolloSwafAsync(Guid referenceId, StartWorkflowCommand startWorkflowCommand, CollaborationModel protocolModel,
            DocumentModel mainDocument, ICollection<DocumentModel> attachments, Category category, Container container, Location workflowLocation)
        {
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = "AUSL-PC - SWAF - Crea protocollo"
            };

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME,
                ValueString = $"Attività - Crea protocollo SWAF"
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = startWorkflowCommand.TenantId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                ValueGuid = startWorkflowCommand.TenantAOOId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = startWorkflowCommand.TenantName
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER,
                ValueGuid = referenceId
            });

            //MainDocument
            ArchiveDocument archiveDocument = await _documentService.InsertDocumentAsync(new ArchiveDocument()
            {
                Archive = workflowLocation.ProtocolArchive,
                ContentStream = mainDocument.Stream,
                Name = mainDocument.DocumentName
            });

            //Attachments
            ICollection<ArchiveDocument> attachmentArchiveDocuments = new List<ArchiveDocument>();
            foreach (ArchiveDocument attachment in attachments.Select(s => new ArchiveDocument()
            {
                Archive = workflowLocation.ProtocolArchive,
                ContentStream = s.Stream,
                Name = s.DocumentName
            }))
            {
                attachmentArchiveDocuments.Add(await _documentService.InsertDocumentAsync(attachment));
            }
            ProtocolBuildModel protocolBuildModel = new ProtocolBuildModel()
            {
                WorkflowAutoComplete = true,
                WorkflowName = workflowStart.WorkflowName,
                Protocol = new ProtocolModel()
                {
                    Object = protocolModel.Subject,
                    Note = protocolModel.Note,
                    AdvancedProtocols = new AdvancedProtocolModel() {ServiceCategory = protocolModel.Metadatas.Single(x => x.KeyName == "_dsw_p_ServiceCategory").Value },
                    ProtocolType = new ProtocolTypeModel() { EntityShortId = (short)protocolModel.Direction },
                    MainDocument = new DocSuiteWeb.Model.Entities.Commons.DocumentModel()
                    {
                        DocumentToStoreId = archiveDocument.IdDocument,
                        FileName = archiveDocument.Name
                    },
                    Attachments = attachmentArchiveDocuments.Select(s => new DocSuiteWeb.Model.Entities.Commons.DocumentModel()
                    {
                        DocumentToStoreId = s.IdDocument,
                        FileName = s.Name
                    }).ToList(),
                    Category = new DocSuiteWeb.Model.Entities.Commons.CategoryModel()
                    {
                        UniqueId = category.UniqueId,
                        Name = category.Name,
                        IdCategory = category.EntityShortId
                    },
                    Container = new DocSuiteWeb.Model.Entities.Commons.ContainerModel()
                    {
                        UniqueId = container.UniqueId,
                        Name = container.Name,
                        IdContainer = container.EntityShortId,
                        ProtLocation = new LocationModel(container.ProtLocation.EntityShortId) { ProtocolArchive = container.ProtLocation.ProtocolArchive },
                        ProtocolAttachmentLocation = container.ProtAttachLocation != null ? new LocationModel(container.ProtAttachLocation.EntityShortId) { ProtocolArchive = container.ProtAttachLocation.ProtocolArchive } : null
                    }
                }
            };

            Role docSuiteSector;
            foreach (DocSuiteSectorModel sector in protocolModel.Sectors)
            {
                docSuiteSector = _unitOfWork.Repository<Role>().GetByUniqueId(sector.SectorRoleId.Value).SingleOrDefault();
                protocolBuildModel.Protocol.Roles.Add(new ProtocolRoleModel() 
                {  
                   Role = new RoleModel(docSuiteSector.EntityShortId),
                   Status = ProtocolRoleStatus.ToEvaluate
                });
            }

            foreach (ContactModel item in protocolModel.Contacts.Where(f => f.ContactDirectionType == ContactDirectionType.Recipient))
            {
                protocolBuildModel.Protocol.ContactManuals.Add(new ProtocolContactManualModel()
                {
                    Description = item.Description,
                    EMail = item.EmailAddress,
                    FiscalCode = item.FiscalCode,
                    TelephoneNumber = item.TelephoneNumber,
                    BirthDate = item.BirthDate,
                    CivicNumber = item.CivicNumber,
                    ZipCode = item.ZipCode,
                    City = item.City,
                    Address = item.Address,
                    CityCode = item.CityCode,
                    BaseContactType = (DocSuiteWeb.Model.Entities.Commons.ContactType)item.ContactType,
                    ComunicationType = ComunicationType.Recipient
                });
            }

            foreach (ContactModel item in protocolModel.Contacts.Where(f => f.ContactDirectionType == ContactDirectionType.Sender))
            {
                protocolBuildModel.Protocol.ContactManuals.Add(new ProtocolContactManualModel() { 
                    Description = item.Description,
                    BaseContactType = (DocSuiteWeb.Model.Entities.Commons.ContactType)item.ContactType,
                    ComunicationType = ComunicationType.Sender 
                });
            }

            protocolBuildModel.WorkflowActions.Add(new WorkflowActionShareDocumentUnitModel(new DocSuiteWeb.Model.Entities.DocumentUnits.DocumentUnitModel()
            {
                UniqueId = protocolBuildModel.Protocol.UniqueId,
                Environment = (int)DSWEnvironmentType.Protocol,
            }) { WorkflowName = workflowStart.WorkflowName, IdWorkflowActivity = referenceId });

            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = referenceId,
                ReferenceType = DSWEnvironmentType.Build,
                ReferenceModel = JsonConvert.SerializeObject(protocolBuildModel, Defaults.DefaultJsonSerializer)
            };

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, new WorkflowArgument()
            {
                PropertyType = ArgumentType.Json,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                ValueString = JsonConvert.SerializeObject(workflowReferenceModel, Defaults.DefaultJsonSerializer)
            });
            return await _workflowStartService.CreateAsync(workflowStart);
        }

        public async Task<WorkflowResult> StartWorkflowCreaArchivioSwaftAsync(Guid referenceId, StartWorkflowCommand startWorkflowCommand, ArchiveModel archiveModel, UDSRepository udsRepository,
            DocumentModel mainDocument, ICollection<DocumentModel> attachments, Category category, IDictionary<string, object> metadatas, ICollection<ContactModel> contacts, Location workflowLocation)
        {
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = "AUSL-PC - SWAF - Crea archivio"
            };

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME,
                ValueString = $"Attività - Crea archivio SWAF"
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = startWorkflowCommand.TenantId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                ValueGuid = startWorkflowCommand.TenantAOOId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = startWorkflowCommand.TenantName
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER,
                ValueGuid = referenceId
            });

            Guid udsId = Guid.NewGuid();
            VecompSoftware.Helpers.UDS.UDSModel udsModel = VecompSoftware.Helpers.UDS.UDSModel.LoadXml(udsRepository.ModuleXML);
            udsModel.Model.UDSId = udsId.ToString();
            udsModel.Model.Subject.Value = archiveModel.Subject;

            udsModel.FillMetaData(metadatas);

            //MainDocument
            ArchiveDocument archiveDocument = new ArchiveDocument()
            {
                Archive = workflowLocation.ProtocolArchive,
                ContentStream = mainDocument.Stream,
                Name = mainDocument.DocumentName
            };
            archiveDocument = await _documentService.InsertDocumentAsync(archiveDocument);
            udsModel.Model.Documents.Document.Instances = new VecompSoftware.Helpers.UDS.DocumentInstance[]
            {
                new VecompSoftware.Helpers.UDS.DocumentInstance()
                {
                    DocumentName = mainDocument.DocumentName,
                    IdDocumentToStore = archiveDocument.IdDocument.ToString()
                }
            };

            //Attachments
            if (udsModel.Model.Documents.DocumentAttachment != null)
            {                
                ICollection<ArchiveDocument> attachmentArchiveDocuments = new List<ArchiveDocument>();
                foreach (ArchiveDocument attachment in attachments.Select(s => new ArchiveDocument()
                {
                    Archive = workflowLocation.ProtocolArchive,
                    ContentStream = s.Stream,
                    Name = s.DocumentName
                }))
                {
                    attachmentArchiveDocuments.Add(await _documentService.InsertDocumentAsync(attachment));
                }

                udsModel.Model.Documents.DocumentAttachment.Instances = attachmentArchiveDocuments.Select(s => 
                    new VecompSoftware.Helpers.UDS.DocumentInstance()
                    {
                        DocumentName = s.Name,
                        IdDocumentToStore = s.IdDocument.ToString()
                    }).ToArray();
            }            

            string contactManual;
            VecompSoftware.Helpers.UDS.Contacts contactsSection;
            foreach (ContactModel contactModel in contacts)
            {
                contactManual = JsonConvert.SerializeObject(new
                {
                    Contact = new
                    {
                        Description = contactModel.Description,
                        EmailAddress = contactModel.EmailAddress,
                        CertifiedMail = contactModel.PECAddress,
                        FiscalCode = contactModel.FiscalCode,
                        TelephoneNumber = contactModel.TelephoneNumber,
                        BirthDate = contactModel.BirthDate,
                        Address = new
                        {
                            Address = contactModel.Address,
                            CivicNumber = contactModel.CivicNumber,
                            ZipCode = contactModel.ZipCode,
                            City = contactModel.City,
                            CityCode = contactModel.CityCode
                        },
                        ContactType = new
                        {
                            ContactTypeId = DocSuiteWeb.Entity.Commons.ContactType.Citizen
                        }
                    },
                    Type = 0
                });
                contactsSection = udsModel.Model.Contacts.FirstOrDefault(x => x.Label == contactModel.ArchiveSection);
                if (contactsSection != null)
                {
                    contactsSection.ContactManualInstances = (contactsSection.ContactManualInstances ?? Enumerable.Empty<VecompSoftware.Helpers.UDS.ContactManualInstance>()).Concat(new VecompSoftware.Helpers.UDS.ContactManualInstance[] { new VecompSoftware.Helpers.UDS.ContactManualInstance() { ContactDescription = contactManual } }).ToArray();
                }
            }

            UDSBuildModel buildModel = new UDSBuildModel()
            {
                UDSId = udsId,
                WorkflowName = workflowStart.WorkflowName,
                WorkflowAutoComplete = true,
                Category = new DocSuiteWeb.Model.Entities.Commons.CategoryModel()
                {
                    IdCategory = category.EntityId
                },
                UDSRepository = new UDSRepositoryModel()
                {
                    Id = udsRepository.UniqueId,
                    DSWEnvironment = udsRepository.DSWEnvironment
                },
                Subject = archiveModel.Subject,
                Title = archiveModel.ArchiveName,
                XMLContent = udsModel.SerializeToXml()
            };

            buildModel.WorkflowActions.Add(new WorkflowActionShareDocumentUnitModel(new DocSuiteWeb.Model.Entities.DocumentUnits.DocumentUnitModel()
            {
                UniqueId = buildModel.UDSId.Value,
                Environment = buildModel.UDSRepository.DSWEnvironment
            }) { WorkflowName = workflowStart.WorkflowName, IdWorkflowActivity = referenceId });

            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = referenceId,
                ReferenceType = DSWEnvironmentType.Build,
                ReferenceModel = JsonConvert.SerializeObject(buildModel, Defaults.DefaultJsonSerializer)
            };

            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, new WorkflowArgument()
            {
                PropertyType = ArgumentType.Json,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                ValueString = JsonConvert.SerializeObject(workflowReferenceModel, Defaults.DefaultJsonSerializer)
            });
            return await _workflowStartService.CreateAsync(workflowStart);
        }
        #endregion

        #region SPID Workflows
        //TODO: Gestire modulo in integration
        [NonAction]
        public async Task<IHttpActionResult> EvaluateSPIDWorkflowsAsync(StartWorkflowCommand startWorkflowCommand)
        {
            _logger.WriteInfo(new LogMessage($"EvaluateSPIDWorkflowsAsync ...."), LogCategories);

            if (!startWorkflowCommand.ContentType.Content.Name.Equals("SPID Accesso agli Atti", StringComparison.InvariantCulture))
            {
                _logger.WriteWarning(new LogMessage(string.Concat("Il nome di workflow ", startWorkflowCommand.ContentType.Content.Name, " non è stato riconosciuto da quelli validi")), LogCategories);
                return BadRequest($"Il nome di workflow {startWorkflowCommand.ContentType.Content.Name} non è stato riconosciuto da quelli validi");
            }

            if (startWorkflowCommand.ContentType.Content.Name.Equals("SPID Accesso agli Atti", StringComparison.InvariantCulture) &&
                !startWorkflowCommand.ContentType.Content.WorkflowParameters.Any(f => f.ParameterModel is ContactModel && f.ParameterName == WorkflowParameterNames.ArchiveNames.CONTACT))
            {
                _logger.WriteWarning(new LogMessage(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " non ha tutti i parametri richiesti. Verificare se è stato specifico il contatto del richiedente.")), LogCategories);
                return BadRequest(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " non ha tutti i parametri richiesti. Verificare se è stato specifico il contatto del richiedente."));
            }

            try
            {
                Guid instanceId = Guid.NewGuid();
                ContactModel contactModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.First(f => f.ParameterModel is ContactModel && f.ParameterName == WorkflowParameterNames.ArchiveNames.CONTACT).ParameterModel as ContactModel;
                WorkflowParameterModel archiveModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.DocumentUnitNames.METADATA && f.ParameterModel is ArchiveModel);
                ArchiveModel model = archiveModel.ParameterModel as ArchiveModel;

                MetadataModel dataInvio = model.Metadatas.SingleOrDefault(x => x.KeyName.Equals("Data invio", StringComparison.InvariantCultureIgnoreCase));
                MetadataModel tipologiaUtente = model.Metadatas.SingleOrDefault(x => x.KeyName.Equals("Tipologia utente", StringComparison.InvariantCultureIgnoreCase));
                MetadataModel tipologiaAccesso = model.Metadatas.SingleOrDefault(x => x.KeyName.Equals("Tipologia accesso", StringComparison.InvariantCultureIgnoreCase));
                MetadataModel documenti = model.Metadatas.SingleOrDefault(x => x.KeyName.Equals("Documenti", StringComparison.InvariantCultureIgnoreCase));
                MetadataModel motivazioni = model.Metadatas.SingleOrDefault(x => x.KeyName.Equals("Motivazioni", StringComparison.InvariantCultureIgnoreCase));
                MetadataModel ritornoDocumentazione = model.Metadatas.SingleOrDefault(x => x.KeyName.Equals("Ritorno documentazione", StringComparison.InvariantCultureIgnoreCase));
                MetadataModel statoRichiesta = model.Metadatas.SingleOrDefault(x => x.KeyName.Equals("Stato richiesta", StringComparison.InvariantCultureIgnoreCase));
                ICollection<PrivateMetadaModel.MetadataValueModel> privateMetadaModels = new List<PrivateMetadaModel.MetadataValueModel>();
                if (dataInvio != null)
                {
                    privateMetadaModels.Add(new PrivateMetadaModel.MetadataValueModel()
                    {
                        KeyName = "Data invio",
                        Value = Convert.ToDateTime(dataInvio.Value).ToString("yyyyMMddHHmmss")
                    });
                }
                privateMetadaModels.Add(new PrivateMetadaModel.MetadataValueModel()
                {
                    KeyName = "utente",
                    Value = JsonConvert.SerializeObject(new DocSuiteWeb.Model.Entities.Commons.ContactModel()
                    {
                        Address = contactModel.Address,
                        BirthDate = contactModel.BirthDate,
                        BirthPlace = contactModel.BirthPlace,
                        CertifiedMail = contactModel.PECAddress,
                        City = contactModel.City,
                        CityCode = contactModel.ZipCode,
                        CivicNumber = contactModel.CivicNumber,
                        Code = contactModel.ExternalCode,
                        ContactType = (VecompSoftware.DocSuiteWeb.Model.Entities.Commons.ContactType)contactModel.ContactType,
                        Description = contactModel.Description,
                        Email = contactModel.EmailAddress,
                        FaxNumber = contactModel.FaxNumber,
                        FiscalCode = contactModel.FiscalCode,
                        PhoneNumber = contactModel.PhoneNumber,
                        TelephoneNumber = contactModel.TelephoneNumber,
                        ZipCode = contactModel.ZipCode
                    })
                });
                foreach (MetadataModel item in model.Metadatas.Where(x => !x.KeyName.Equals("Data invio", StringComparison.InvariantCultureIgnoreCase)))
                {
                    privateMetadaModels.Add(new PrivateMetadaModel.MetadataValueModel()
                    {
                        KeyName = item.KeyName,
                        Value = item.Value.ToString()
                    });
                }
                WorkflowStart workflowStart = new WorkflowStart
                {
                    WorkflowName = startWorkflowCommand.ContentType.Content.Name
                };
                workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, new WorkflowArgument()
                {
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL,
                    PropertyType = ArgumentType.Json,
                    ValueString = JsonConvert.SerializeObject(new WorkflowReferenceModel()
                    {
                        ReferenceId = instanceId,
                        ReferenceType = DSWEnvironmentType.Workflow,
                        ReferenceModel = JsonConvert.SerializeObject(privateMetadaModels, DocSuiteWeb.Service.Workflow.ServiceHelper.SerializerSettings)
                    }, DocSuiteWeb.Service.Workflow.ServiceHelper.SerializerSettings)
                });
                workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.PropertyGuid,
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                    ValueGuid = startWorkflowCommand.TenantId
                });
                workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.PropertyGuid,
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                    ValueGuid = startWorkflowCommand.TenantAOOId
                });
                workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.PropertyString,
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                    ValueString = startWorkflowCommand.TenantName
                });
                using (HttpClient httpClient = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
                {
                    IIdentityContext identityContext = new IdentityContext(startWorkflowCommand.IdentityContext.Identity.Account);
                    Uri url = _parameterEnvService.CurrentTenantModel.WebApiClientConfig.Addresses.Single(f => f.AddressName.Equals(API_ServiceBusAddress, StringComparison.InvariantCultureIgnoreCase)).Address;
                    _logger.WriteInfo(new LogMessage($"Connetting to webapi {url.AbsoluteUri}"), LogCategories);
                    IEventWorkflowStartRequest eventWorkflowStartRequest = new EventWorkflowStartRequest(Guid.NewGuid(), instanceId, startWorkflowCommand.TenantName,
                       startWorkflowCommand.TenantId, startWorkflowCommand.TenantAOOId, new IdentityContext(startWorkflowCommand.IdentityContext.Identity.Account), workflowStart, null);
                    StringContent data = new StringContent(JsonConvert.SerializeObject(eventWorkflowStartRequest, DocSuiteWeb.Service.Workflow.ServiceHelper.SerializerSettings), Encoding.UTF8, "application/json");
                    HttpResponseMessage reponse = await httpClient.PostAsync($"{url.AbsoluteUri}/Topic", data);
                    string message = await reponse.Content.ReadAsStringAsync();
                    if (!reponse.IsSuccessStatusCode)
                    {
                        throw new Exception(message);
                    }
                    _logger.WriteInfo(new LogMessage($"IEventWorkflowStartRequest {eventWorkflowStartRequest.Id} sent successfully"), LogCategories);
                }

                var result = new { InstanceId = instanceId, Result = $"Workflow '{startWorkflowCommand.ContentType.Content.Name}' avviato correttamente" };
                _logger.WriteInfo(new LogMessage($"Workflow '{startWorkflowCommand.ContentType.Content.Name}' avviato correttamente con istanza {instanceId} "), LogCategories);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.WriteWarning(new LogMessage($"Il workflow '{startWorkflowCommand.ContentType.Content.Name}' non è stato avviato correttamente"), ex, LogCategories);
                return BadRequest($"Il workflow '{startWorkflowCommand.ContentType.Content.Name}' non è stato avviato correttamente");
            }
        }

        #endregion        

        #endregion
    }
}