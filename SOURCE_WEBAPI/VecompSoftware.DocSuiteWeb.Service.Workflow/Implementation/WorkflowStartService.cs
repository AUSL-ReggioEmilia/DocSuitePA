using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuite.Document.Generator.OpenXml.Word;
using VecompSoftware.DocSuite.Document.Generator.PDF;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Configuration;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Mapper.Workflow;
using VecompSoftware.DocSuiteWeb.Model.Validations;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Service.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Service.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Service.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.Helpers.Workflow;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;
using StorageDocument = VecompSoftware.DocSuite.Document;

namespace VecompSoftware.DocSuiteWeb.Service.Workflow
{
    [LogCategory(LogCategoryDefinition.SERVICEWF)]
    public class WorkflowStartService : WorkflowBaseService<WorkflowStart>, IWorkflowStartService, IDisposable
    {
        #region [ Fields ]
        private const string _biblos_attribute_signature = "signature";

        private readonly IWorkflowInstanceService _workflowInstanceService;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IWorkflowArgumentMapper _workflowArgumentMapper;
        private readonly StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> _documentService;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public WorkflowStartService(ILogger logger, IWorkflowInstanceService workflowInstanceService, IWorkflowArgumentMapper workflowArgumentMapper,
            IWorkflowInstanceRoleService workflowInstanceRoleService, IWorkflowActivityService workflowActivityService,
            ITopicService topicServiceBus, ICQRSMessageMapper mapper_eventServiceBusMessage,
            IDataUnitOfWork unitOfWork, StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService,
            ICollaborationService collaborationService, ISecurity security, IParameterEnvService parameterEnvService, IFascicleRoleService fascicleRoleService,
            IMessageService messageService, IDossierRoleService dossierRoleService, IQueueService queueService, IWordOpenXmlDocumentGenerator wordOpenXmlDocumentGenerator,
            IMessageConfiguration messageConfiguration, IProtocolLogService protocolLogService, IPDFDocumentGenerator pdfDocumentGenerator,
            IFascicleService fascicleService, IFascicleDocumentService fascicleDocumentService, IFascicleFolderService fascicleFolderService,
            IFascicleDocumentUnitService fascDocumentUnitService, IFascicleLinkService fascicleLinkService)
            : base(logger, workflowInstanceService, workflowInstanceRoleService, workflowActivityService, topicServiceBus, mapper_eventServiceBusMessage,
                  unitOfWork, documentService, collaborationService, security, parameterEnvService, fascicleRoleService, messageService, dossierRoleService, queueService,
                  wordOpenXmlDocumentGenerator, messageConfiguration, protocolLogService, pdfDocumentGenerator, fascicleService, fascicleDocumentService, fascicleFolderService,
                  fascDocumentUnitService, fascicleLinkService)
        {
            _unitOfWork = unitOfWork;
            _workflowInstanceService = workflowInstanceService;
            _workflowArgumentMapper = workflowArgumentMapper;
            _documentService = documentService;
        }
        #endregion

        #region [ Methods ]
        protected override async Task<WorkflowResult> BeforeCreateAsync(WorkflowStart content)
        {
            return await StartWorkflowJson(content);
        }

        private async Task<WorkflowResult> StartWorkflowJson(WorkflowStart content)
        {
            WorkflowResult validationResult = new WorkflowResult();

            WorkflowRepository workflowRepository = _unitOfWork.Repository<WorkflowRepository>().GetByName(content.WorkflowName);
            if (workflowRepository == null)
            {
                throw new DSWValidationException("Evaluate start workflow validation error",
                    new List<ValidationMessageModel>()
                        {
                            new ValidationMessageModel
                            {
                                Key = "WorkflowStart",
                                Message = $"Impossibile avviare il workflow '{content.WorkflowName}' in quanto non esiste nel repositories dei workflow validi."
                            }
                        }, null, DSWExceptionCode.VA_RulesetValidation);
            }

            WorkflowInstance workflowInstance = new WorkflowInstance()
            {
                Status = WorkflowStatus.Todo,
                WorkflowRepository = workflowRepository,
                Json = workflowRepository.Json,
                Subject = workflowRepository.Name
            };
            WorkflowProperty prop;
            foreach (KeyValuePair<string, WorkflowArgument> item in content.Arguments)
            {
                prop = _workflowArgumentMapper.Map(item.Value, new WorkflowProperty());
                prop.WorkflowType = WorkflowType.Workflow;
                workflowInstance.WorkflowProperties.Add(prop);
            }

            WorkflowProperty _dsw_p_WorkflowStartMotivationRequired = workflowInstance.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_MOTIVATION_REQUIRED);
            WorkflowProperty _dsw_p_InstanceSubject = workflowInstance.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_INSTANCE_SUBJECT);
            WorkflowProperty _dsw_v_Workflow_ActiveInstanceSubjectUnique = workflowInstance.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_VALIDATION_INSTANCE_ACTIVE_SUBJECT_UNIQUE);
            WorkflowProperty _dsw_v_Workflow_StartValidations = workflowInstance.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_VALIDATION_WORKFLOW_START);
            WorkflowProperty _dsw_p_ReferenceModel = workflowInstance.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL);

            if (_dsw_v_Workflow_StartValidations != null && !string.IsNullOrEmpty(_dsw_v_Workflow_StartValidations.ValueString))
            {
                Dictionary<DSWEnvironmentType, WorkflowRuleDefinition> deserializedWorkflowStartValidation = JsonConvert.DeserializeObject<Dictionary<DSWEnvironmentType, WorkflowRuleDefinition>>(_dsw_v_Workflow_StartValidations.ValueString);
                Guid fascicleId = JsonConvert.DeserializeObject<WorkflowReferenceModel>(_dsw_p_ReferenceModel.ValueString).ReferenceId;
                #region Intermediate Validations
                if (!(deserializedWorkflowStartValidation is Dictionary<DSWEnvironmentType, WorkflowRuleDefinition>))
                {
                    throw new DSWValidationException("Evaluate start workflow validation error",
                            new List<ValidationMessageModel>()
                                 {
                             new ValidationMessageModel
                                {
                                    Key = "WorkflowStart",
                                    Message = $"Impossibile avviare il workflow '{content.WorkflowName}' in quanto le definizioni delle regole di avvio non sono valide (struttura non valida)."
                                }
                                }, null, DSWExceptionCode.VA_RulesetValidation);
                }

                if (deserializedWorkflowStartValidation.Keys.FirstOrDefault().ToString() != EnumHelper.GetDescription(DSWEnvironmentType.Fascicle))
                {
                    throw new DSWValidationException("Evaluate start workflow validation error",
                            new List<ValidationMessageModel>()
                                 {
                             new ValidationMessageModel
                                {
                                    Key = "WorkflowStart",
                                    Message = $"Impossibile avviare il workflow '{content.WorkflowName}' in quanto il motore supporta definizioni solo per il modulo Fascicoli (environment non supportato)."
                                }
                            }, null, DSWExceptionCode.VA_RulesetValidation);
                }
                #endregion
                List<ValidationMessageModel> errorResult = new List<ValidationMessageModel>();
                ICollection<WorkflowRule> deserializedWorkflowRules = deserializedWorkflowStartValidation[DSWEnvironmentType.Fascicle].Rules;

               await ValidateWorkflowRules(fascicleId, errorResult, deserializedWorkflowRules);
            }
            if (_dsw_p_InstanceSubject != null)
            {
                workflowInstance.Subject = _dsw_p_InstanceSubject.ValueString;
                _logger.WriteDebug(new LogMessage($"SET INSTANCE SUBJECT: {workflowInstance.Subject}"), LogCategories);
            }
            if (_dsw_p_WorkflowStartMotivationRequired != null && _dsw_p_WorkflowStartMotivationRequired.ValueBoolean.HasValue && _dsw_p_WorkflowStartMotivationRequired.ValueBoolean.Value)
            {
                WorkflowProperty dsw_p_Subject = workflowInstance.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT);
                if (dsw_p_Subject == null || string.IsNullOrEmpty(dsw_p_Subject.ValueString))
                {
                    throw new DSWValidationException("Evaluate start workflow validation error",
                            new List<ValidationMessageModel>()
                            {
                                new ValidationMessageModel
                                {
                                    Key = "WorkflowStart",
                                    Message = $"Impossibile avviare il flusso di lavoro se non è stata fornita una motivazione"
                                }
                            }, null, DSWExceptionCode.VA_RulesetValidation);
                }
            }
            if (_dsw_v_Workflow_ActiveInstanceSubjectUnique != null && _dsw_v_Workflow_ActiveInstanceSubjectUnique.ValueBoolean.HasValue && _dsw_v_Workflow_ActiveInstanceSubjectUnique.ValueBoolean.Value)
            {
                if (_unitOfWork.Repository<WorkflowInstance>().CountActiveInstances(workflowInstance.WorkflowRepository.UniqueId, workflowInstance.Subject) > 0)
                {
                    throw new DSWValidationException("Evaluate start workflow validation error",
                            new List<ValidationMessageModel>()
                            {
                                new ValidationMessageModel
                                {
                                    Key = "WorkflowStart",
                                    Message = $"Impossibile avviare un nuovo flusso di lavoro in cui l'oggetto '{workflowInstance.Subject}' è stato come unico attivo. Completare i restanti flussi di lavoro attivi."
                                }
                            }, null, DSWExceptionCode.VA_RulesetValidation);
                }
            }

            _unitOfWork.BeginTransaction();
            Guid instanceId = Guid.NewGuid();
            workflowInstance = await _workflowInstanceService.CreateAsync(workflowInstance);
            await PopulateActivityAsync(workflowInstance, instanceId, workflowInstance.WorkflowRepository,
                workflowInstance.WorkflowProperties.Where(f => !f.Name.Equals(WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE)));
            bool result = await _unitOfWork.SaveAsync();

            _unitOfWork.BeginTransaction();
            workflowInstance.Status = WorkflowStatus.Progress;
            if (!workflowInstance.InstanceId.HasValue)
            {
                workflowInstance.InstanceId = instanceId;
            }
            workflowInstance = await _workflowInstanceService.UpdateAsync(workflowInstance);
            result = await _unitOfWork.SaveAsync();
            validationResult.InstanceId = workflowInstance.InstanceId;
            validationResult.IsValid = true;
            _logger.WriteInfo(new LogMessage($"Assing workflowInstance.InstanceId [{workflowInstance.InstanceId}] for {workflowInstance.WorkflowRepository.Name}"), LogCategories);

            return validationResult;
        }

        private async Task ValidateWorkflowRules(Guid fascicleId, List<ValidationMessageModel> errorResult, ICollection<WorkflowRule> deserializedWorkflowRules)
        {
            foreach (WorkflowRule workflowRule in deserializedWorkflowRules)
            {
                WorkflowRule wfRule = workflowRule;
                FascicleFolder fascicleFolder = _unitOfWork.Repository<FascicleFolder>().GetByIdFascicleAndLevel(fascicleId, 2).FirstOrDefault();
                bool isExist = _unitOfWork.Repository<FascicleFolder>().NameAlreadyExists(wfRule.Name, fascicleFolder.UniqueId, fascicleId);
                if (isExist == false)
                {
                    errorResult.Add(new ValidationMessageModel 
                    {
                        Key = "WorkflowStart",
                        Message = $"Non esiste cartella di fascicolo {wfRule.Name}"
                    });
                }

                ValidateWorkflowRules(wfRule.HasFile, errorResult,
                   $"Convalida del nome {EnumHelper.GetDescription(WorkflowValidationRulesType.HasFile)} non è selezionata per { wfRule.Name}",
                   $"Non c'è valore per {wfRule.Name} - {EnumHelper.GetDescription(WorkflowValidationRulesType.HasFile)} la convalida {wfRule.HasFile}", 
                   _unitOfWork.Repository<FascicleDocument>().HasFascicleDocument(wfRule.Name, fascicleId));

                ValidateWorkflowRules(wfRule.HasDocumentUnit, errorResult,
                    $"Convalida del nome {EnumHelper.GetDescription(WorkflowValidationRulesType.HasDocumentUnit)} non è selezionata per { wfRule.Name}",
                    $"Non c'è valore per {wfRule.Name} - {EnumHelper.GetDescription(WorkflowValidationRulesType.HasDocumentUnit)} la convalida {wfRule.HasDocumentUnit}", 
                    _unitOfWork.Repository<FascicleDocumentUnit>().HasFascicleDocumentUnit(wfRule.Name, fascicleId));

                List<FascicleDocument> fascicleDocuments = _unitOfWork.Repository<FascicleDocument>().HasFascicleDocumentSigned(wfRule.Name, fascicleId, true).ToList();
                bool hasSignedFile = await _documentService.IsDocumentsSignedAsync(fascicleDocuments.Select(x => x.UniqueId).ToList());

                ValidateWorkflowRules(wfRule.HasSignedFile, errorResult,
                    $"Convalida del nome {EnumHelper.GetDescription(WorkflowValidationRulesType.HasSignedFile)} non è selezionata per { wfRule.Name}",
                    $"Non c'è valore per {wfRule.Name} - {EnumHelper.GetDescription(WorkflowValidationRulesType.HasSignedFile)} la convalida {wfRule.HasSignedFile}", hasSignedFile);
            }

            if (errorResult.Count() > 0)
            {
                throw new DSWValidationException("Evaluate start workflow validation error", errorResult, null, DSWExceptionCode.VA_RulesetValidation);
            }
        }

        private List<ValidationMessageModel> ValidateWorkflowRules(bool workflowRule, List<ValidationMessageModel> errorResult, string validationRuleMessage, string validationEntityMessage, bool entityResult)
        {
            if (workflowRule)
            {
                if (!entityResult)
                {
                    errorResult.Add(
                         new ValidationMessageModel
                         {
                             Key = "WorkflowStart",
                             Message = validationEntityMessage
                         });
                }
            }
            else
            {
                errorResult.Add(
                           new ValidationMessageModel
                           {
                               Key = "WorkflowStart",
                               Message = validationRuleMessage
                           });
            }
            return errorResult;
        }

        #endregion
    }
}