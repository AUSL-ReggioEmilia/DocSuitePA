using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.Workflow;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Events.Models.Workflows;
using ContactModel = VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters.ContactModel;
using DocumentModel = VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters.DocumentModel;
using DSWEnvironmentType = VecompSoftware.DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType;
using PrivateMetadaModel = VecompSoftware.DocSuiteWeb.Model.Metadata;

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

        #endregion

        #region [ Constructor ]
        public StartWorkflowController(IDataUnitOfWork unitOfWork, ILogger logger, IWorkflowStartService workflowStartService,
            IParameterEnvService parameterEnvService, ITopicService topicService, ICQRSMessageMapper cqrsMessageMapper)
            : base()
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _workflowStartService = workflowStartService;
            _topicService = topicService;
            _parameterEnvService = parameterEnvService;
            _mapper_cqrsMessageMapper = cqrsMessageMapper;
            _instanceId = Guid.NewGuid();
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
        public async Task<IHttpActionResult> Post([FromBody]StartWorkflowCommand startWorkflowCommand)
        {
            _logger.WriteInfo(new LogMessage(string.Concat("StartWorkflow controller receive message ",
                startWorkflowCommand == null ? "null" : startWorkflowCommand.GetType().ToString())), LogCategories);

            if (startWorkflowCommand == null || startWorkflowCommand.ContentType == null || startWorkflowCommand.IdentityContext == null ||
                startWorkflowCommand.ContentType.Content == null)
            {
                _logger.WriteWarning(new LogMessage("Modello di avvio workflow non valido. Verificare se i parametri identity, contenttype sono inizializzati"), LogCategories);
                return BadRequest("Modello di avvio workflow non valido. Verificare se i parametri identity, contenttype sono inizializzati");
            }

            if (startWorkflowCommand.ContentType.Content.Name.StartsWith("SkyDoc-"))
            {
                return await EvaluateSkyDocWorkflowsAsync(startWorkflowCommand);
            }

            if (startWorkflowCommand.ContentType.Content.Name.StartsWith("SPID "))
            {
                return await EvaluateSPIDWorkflowsAsync(startWorkflowCommand);
            }
            return BadRequest("Modello di avvio workflow non valido. Verificare se i parametri identity, contenttype sono inizializzati");
        }

        #region Enav Workflows
        [NonAction]
        public async Task<IHttpActionResult> EvaluateSkyDocWorkflowsAsync(StartWorkflowCommand startWorkflowCommand)
        {
            if (!startWorkflowCommand.ContentType.Content.Name.Equals("SkyDoc-IP4D - Protocolla Semplice", StringComparison.InvariantCulture) &&
                    !startWorkflowCommand.ContentType.Content.Name.Equals("SkyDoc-IP4D - Protocolla Firma Digitale", StringComparison.InvariantCulture))
            {
                _logger.WriteWarning(new LogMessage(string.Concat("Il nome di workflow ", startWorkflowCommand.ContentType.Content.Name, " non è stato riconosciuto da quelli validi")), LogCategories);
                return BadRequest(string.Concat("Il nome di workflow ", startWorkflowCommand.ContentType.Content.Name, " non è stato riconosciuto da quelli validi"));
            }

            if (startWorkflowCommand.ContentType.Content.Name.Equals("SkyDoc-IP4D - Protocolla Semplice", StringComparison.InvariantCulture) &&
                (!startWorkflowCommand.ContentType.Content.WorkflowParameters.Any(f => f.ParameterModel is DocumentModel && (f.ParameterModel as DocumentModel).DocumentType == DocumentType.Main) ||
                !startWorkflowCommand.ContentType.Content.WorkflowParameters.Any(f => f.ParameterName == WorkflowParameterNames.ProtocolNames.MANAGE && f.ParameterModel is DocSuiteSectorModel)))
            {
                _logger.WriteWarning(new LogMessage(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " non ha un documento specificato")), LogCategories);
                return BadRequest(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " non ha un documento specificato"));
            }

            if (startWorkflowCommand.ContentType.Content.Name.Equals("SkyDoc-IP4D - Protocolla Firma Digitale", StringComparison.InvariantCulture) &&
                (!startWorkflowCommand.ContentType.Content.WorkflowParameters.Any(f => f.ParameterModel is DocumentModel && (f.ParameterModel as DocumentModel).DocumentType == DocumentType.Main) ||
                !startWorkflowCommand.ContentType.Content.WorkflowParameters.Any(f => f.ParameterModel is SignerModel && f.ParameterName == WorkflowParameterNames.CollaborationNames.SIGNER)))
            {
                _logger.WriteWarning(new LogMessage(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " non ha tutti i parametri richiesti. Verificare se esiste un documento principale e/o i firmatari.")), LogCategories);
                return BadRequest(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " non ha tutti i parametri richiesti. Verificare se esiste un documento principale e/o i firmatari."));
            }

            DocumentModel documentModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.FirstOrDefault(f => f.ParameterModel is DocumentModel && (f.ParameterModel as DocumentModel).DocumentType == DocumentType.Main).ParameterModel as DocumentModel;
            WorkflowParameterModel docSuiteSectorModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.ProtocolNames.MANAGE && f.ParameterModel is DocSuiteSectorModel);
            WorkflowParameterModel protocolModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.ProtocolNames.PROTOCOL_MODEL && f.ParameterModel is CollaborationModel);
            WorkflowParameterModel signerModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.CollaborationNames.SIGNER && f.ParameterModel is SignerModel);
            WorkflowParameterModel collaborationModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.CollaborationNames.DOCUMENT_UNIT_MODEL && f.ParameterModel is CollaborationModel);
            if (docSuiteSectorModel == null)
            {
                docSuiteSectorModel = startWorkflowCommand.ContentType.Content.WorkflowParameters.SingleOrDefault(f => f.ParameterName == WorkflowParameterNames.CollaborationNames.MANAGE && f.ParameterModel is DocSuiteSectorModel);
            }

            if (documentModel != null && string.IsNullOrEmpty(documentModel.DocumentName))
            {
                _logger.WriteWarning(new LogMessage(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " ha un documento senza nome")), LogCategories);
                return BadRequest(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " ha un documento senza nome"));
            }

            if (docSuiteSectorModel != null)
            {
                WorkflowRepository workflowRepository = _unitOfWork.Repository<WorkflowRepository>().Query(f => f.Name == startWorkflowCommand.ContentType.Content.Name).Include(f => f.WorkflowRoleMappings).Select().FirstOrDefault();
                if (workflowRepository == null)
                {
                    _logger.WriteWarning(new LogMessage(string.Concat("Il nome di workflow ", startWorkflowCommand.ContentType.Content.Name, " non è stato riconosciuto da quelli validi")), LogCategories);
                    return BadRequest(string.Concat("Il nome di workflow ", startWorkflowCommand.ContentType.Content.Name, " non è stato riconosciuto da quelli validi"));
                }
                if (!workflowRepository.WorkflowRoleMappings.Any(f => f.MappingTag.Equals((docSuiteSectorModel.ParameterModel as DocSuiteSectorModel).MappingTag, StringComparison.InvariantCultureIgnoreCase)))
                {
                    _logger.WriteWarning(new LogMessage(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name,
                        " ha specificato un valore non riconosciuto per il parametro MappingTag : ", (docSuiteSectorModel.ParameterModel as DocSuiteSectorModel).MappingTag)), LogCategories);
                    return BadRequest(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name,
                        " ha specificato un valore non riconosciuto per il parametro MappingTag : ", (docSuiteSectorModel.ParameterModel as DocSuiteSectorModel).MappingTag));
                }
            }
            else
            {
                _logger.WriteWarning(new LogMessage(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " non ha tutti i parametri richiesti. Verificare se esiste il MappingTag")), LogCategories);
                return BadRequest(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " non ha tutti i parametri richiesti. Verificare se esiste il MappingTag"));
            }

            if (signerModel != null)
            {
                SignerModel model = (signerModel.ParameterModel as SignerModel);
                if (model.SignerType != SignerType.AD || model.Identity.Authorization != Core.Models.Securities.AuthorizationType.NTLM || model.Identity.Account.Contains("|"))
                {
                    _logger.WriteWarning(new LogMessage(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " non ha tutti i parametri richiesti. Verificare se l'account del firmatario è nel formato NTLM corretto col dominio e samAccountName")), LogCategories);
                    return BadRequest(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " non ha tutti i parametri richiesti. non ha tutti i parametri richiesti. Verificare se l'account del firmatario è nel formato NTLM corretto col dominio e samAccountName"));
                }
            }

            try
            {
                WorkflowResult workflowResult = null;
                if (startWorkflowCommand.ContentType.Content.Name.Equals("SkyDoc-IP4D - Protocolla Semplice", StringComparison.InvariantCulture))
                {
                    workflowResult = await StartWorkflowProtocollaSempliceAsync(startWorkflowCommand.Id, startWorkflowCommand.IdentityContext,
                        (docSuiteSectorModel.ParameterModel as DocSuiteSectorModel), documentModel, protocolModel.ParameterModel as CollaborationModel);
                }
                if (startWorkflowCommand.ContentType.Content.Name.Equals("SkyDoc-IP4D - Protocolla Firma Digitale", StringComparison.InvariantCulture))
                {
                    workflowResult = await StartWorkflowProtocollaFirmaDigitaleAsync(startWorkflowCommand.Id, startWorkflowCommand.IdentityContext,
                        (docSuiteSectorModel.ParameterModel as DocSuiteSectorModel), documentModel, collaborationModel.ParameterModel as CollaborationModel,
                        (signerModel.ParameterModel as SignerModel));
                }
                if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
                {
                    throw new SystemException(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " non avviato"));
                }
                var result = new { InstanceId = workflowResult.InstanceId.Value, Result = string.Concat("Workflow avviato ", startWorkflowCommand.ContentType.Content.Name, " correttamente") };
                _logger.WriteInfo(new LogMessage(string.Concat("Workflow avviato ", startWorkflowCommand.ContentType.Content.Name, " correttamente")), LogCategories);

                return Ok(result);
            }
            catch (Exception)
            {
                _logger.WriteWarning(new LogMessage(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " non è stato avviato correttamente")), LogCategories);
                return BadRequest(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " non è stato avviato correttamente"));
            }
        }

        public async Task<WorkflowResult> StartWorkflowProtocollaFirmaDigitaleAsync(Guid DocumentUniqueId, Core.Models.Securities.IdentityContext identityContext, DocSuiteSectorModel docSuiteSectorModel,
            DocumentModel documentModel, CollaborationModel workCollaborationModel, SignerModel signerModel)
        {
            WorkflowStart start = new WorkflowStart
            {
                WorkflowName = "SkyDoc-IP4D - Protocolla Firma Digitale"
            };
            string username = identityContext.Identity.Account.Split('\\').Last();
            string domain = identityContext.Identity.Account.Split('\\').First();

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MAPPING_TAG, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_MAPPING_TAG,
                ValueString = docSuiteSectorModel.MappingTag
            });

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.RelationGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = _parameterEnvService.CurrentTenantId
            });

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = _parameterEnvService.CurrentTenantName
            });

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER,
                ValueGuid = DocumentUniqueId
            });

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_LABEL, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_LABEL,
                ValueString = "FIRMA SKYDOC - IP4D"
            });

            string proposer = identityContext.Identity.Account;
            if (!proposer.Contains("\\"))
            {
                proposer = string.Concat("***REMOVED***\\", identityContext.Identity.Account);
            }
            DocSuiteWeb.Model.Entities.Collaborations.CollaborationModel collaborationModel = new DocSuiteWeb.Model.Entities.Collaborations.CollaborationModel
            {
                IdPriority = "N",
                IdStatus = "IN",
                SignCount = 1,
                Subject = workCollaborationModel.Subject,
                RegistrationUser = proposer,
                RegistrationName = proposer
            };

            collaborationModel.CollaborationSigns.Add(new DocSuiteWeb.Model.Entities.Collaborations.CollaborationSignModel()
            {
                Incremental = 1,
                IsActive = true,
                SignUser = signerModel.Identity.Account,
                SignName = signerModel.Identity.Name,
                SignEmail = "noreply@enav.it",
                IsRequired = true
            });


            collaborationModel.CollaborationVersionings.Add(new DocSuiteWeb.Model.Entities.Collaborations.CollaborationVersioningModel()
            {
                CollaborationIncremental = 0,
                Incremental = 1,
                DocumentName = documentModel.DocumentName,
                DocumentContent = documentModel.Stream,
                IsActive = true,
                DocumentGroup = DocSuiteWeb.Model.Entities.Collaborations.CollaborationDocumentGroupName.MainDocument
            });

            DocSuiteWeb.Model.Entities.Collaborations.CollaborationProtocolModel protocolModel = new DocSuiteWeb.Model.Entities.Collaborations.CollaborationProtocolModel
            {
                Subject = workCollaborationModel.Subject
            };
            ContactModel recipient = workCollaborationModel.Contacts.Single(f => f.ContactDirectionType == ContactDirectionType.Recipient);
            ContactModel sender = workCollaborationModel.Contacts.Single(f => f.ContactDirectionType == ContactDirectionType.Sender);

            protocolModel.ProtocolContactManuals.Add(new ProtocolContactManualModel() { Description = recipient.Description, Address = recipient.Address, ComunicationType = ComunicationType.Recipient });
            protocolModel.ProtocolContactManuals.Add(new ProtocolContactManualModel() { Description = sender.Description, ComunicationType = ComunicationType.Sender });
            protocolModel.ProtocolType = new ProtocolTypeModel(ProtocolTypology.Outgoing);
            collaborationModel.Protocol = protocolModel;

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, new WorkflowArgument()
            {
                PropertyType = ArgumentType.Json,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_MODEL,
                ValueString = JsonConvert.SerializeObject(collaborationModel, Defaults.DefaultJsonSerializer)
            });

            return await _workflowStartService.CreateAsync(start);
        }

        public async Task<WorkflowResult> StartWorkflowProtocollaSempliceAsync(Guid DocumentUniqueId, Core.Models.Securities.IdentityContext identityContext, DocSuiteSectorModel docSuiteSectorModel,
            DocumentModel documentModel, CollaborationModel workProtocolModel)
        {
            WorkflowStart start = new WorkflowStart
            {
                WorkflowName = "SkyDoc-IP4D - Protocolla Semplice"
            };
            string username = identityContext.Identity.Account.Split('\\').Last();
            string domain = identityContext.Identity.Account.Split('\\').First();
            bool hasProtocolInserRight = _unitOfWork.Repository<Container>().CountProtocolInsertRight(username, domain) > 0;
            if (hasProtocolInserRight)
            {
                start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS, new WorkflowArgument()
                {
                    PropertyType = ArgumentType.PropertyString,
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS,
                    ValueString = JsonConvert.SerializeObject(new List<WorkflowAccount>() { new WorkflowAccount() { AccountName = identityContext.Identity.Account } })
                });
            }

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MAPPING_TAG, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_MAPPING_TAG,
                ValueString = docSuiteSectorModel.MappingTag
            });

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME,
                ValueString = string.Concat("Attività - Protocolla Documento ", documentModel.DocumentName)
            });

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.RelationGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = _parameterEnvService.CurrentTenantId
            });

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = _parameterEnvService.CurrentTenantName
            });

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER,
                ValueGuid = DocumentUniqueId
            });

            ProtocolModel protocolModel = new ProtocolModel
            {
                Object = workProtocolModel.Subject
            };
            ContactModel recipient = workProtocolModel.Contacts.Single(f => f.ContactDirectionType == ContactDirectionType.Recipient);
            ContactModel sender = workProtocolModel.Contacts.Single(f => f.ContactDirectionType == ContactDirectionType.Sender);
            protocolModel.ContactManuals.Add(new ProtocolContactManualModel() { Description = recipient.Description, Address = recipient.Address, ComunicationType = ComunicationType.Recipient });
            protocolModel.ContactManuals.Add(new ProtocolContactManualModel() { Description = sender.Description, ComunicationType = ComunicationType.Sender });
            protocolModel.ProtocolType = new ProtocolTypeModel(ProtocolTypology.Outgoing);
            protocolModel.MainDocument = new DocSuiteWeb.Model.Entities.Commons.DocumentModel() { FileName = documentModel.DocumentName, ContentStream = documentModel.Stream };

            start.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, new WorkflowArgument()
            {
                PropertyType = ArgumentType.Json,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_MODEL,
                ValueString = JsonConvert.SerializeObject(protocolModel, Defaults.DefaultJsonSerializer)
            });
            return await _workflowStartService.CreateAsync(start);
        }

        #endregion

        #region SPID Workflows
        [NonAction]
        public async Task<IHttpActionResult> EvaluateSPIDWorkflowsAsync(StartWorkflowCommand startWorkflowCommand)
        {
            if (!startWorkflowCommand.ContentType.Content.Name.Equals("SPID Accesso agli Atti", StringComparison.InvariantCulture))
            {
                _logger.WriteWarning(new LogMessage(string.Concat("Il nome di workflow ", startWorkflowCommand.ContentType.Content.Name, " non è stato riconosciuto da quelli validi")), LogCategories);
                return BadRequest(string.Concat("Il nome di workflow ", startWorkflowCommand.ContentType.Content.Name, " non è stato riconosciuto da quelli validi"));
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
                PrivateMetadaModel.MetadataModel privateMetadaModel = new PrivateMetadaModel.MetadataModel();
                if (dataInvio != null)
                {
                    privateMetadaModel.DateFields.Add(new PrivateMetadaModel.BaseFieldModel()
                    {
                        Label = "Data invio",
                        Value = Convert.ToDateTime(dataInvio.Value).ToString("yyyyMMddHHmmss")
                    });
                }
                privateMetadaModel.ContactFileds.Add(new DocSuiteWeb.Model.Entities.Commons.ContactModel()
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
                });
                foreach (MetadataModel item in model.Metadatas.Where(x => !x.KeyName.Equals("Data invio", StringComparison.InvariantCultureIgnoreCase)))
                {
                    privateMetadaModel.TextFields.Add(new PrivateMetadaModel.TextFieldModel()
                    {
                        Label = item.KeyName,
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
                        ReferenceModel = JsonConvert.SerializeObject(privateMetadaModel, DocSuiteWeb.Service.Workflow.ServiceHelper.SerializerSettings)
                    }, DocSuiteWeb.Service.Workflow.ServiceHelper.SerializerSettings)
                });
                IIdentityContext identityContext = new IdentityContext(startWorkflowCommand.IdentityContext.Identity.Account);
                IEventWorkflowStartRequest eventWorkflowStartRequest = new EventWorkflowStartRequest(Guid.NewGuid(), instanceId, _parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId,
                    identityContext, workflowStart, null);
                ServiceBusMessage message = _mapper_cqrsMessageMapper.Map(eventWorkflowStartRequest, new ServiceBusMessage());
                ServiceBusMessage response = await _topicService.SendToTopicAsync(message);
                _logger.WriteDebug(new LogMessage($"IEventWorkflowStartRequest {response.MessageId} sent successfully"), LogCategories);
                var result = new { InstanceId = instanceId, Result = string.Concat("Workflow avviato ", startWorkflowCommand.ContentType.Content.Name, " correttamente") };
                _logger.WriteInfo(new LogMessage(string.Concat("Workflow avviato ", startWorkflowCommand.ContentType.Content.Name, " correttamente")), LogCategories);

                return Ok(result);
            }
            catch (Exception)
            {
                _logger.WriteWarning(new LogMessage(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " non è stato avviato correttamente")), LogCategories);
                return BadRequest(string.Concat("Il workflow ", startWorkflowCommand.ContentType.Content.Name, " non è stato avviato correttamente"));
            }
        }

        #endregion
        #endregion
    }
}
