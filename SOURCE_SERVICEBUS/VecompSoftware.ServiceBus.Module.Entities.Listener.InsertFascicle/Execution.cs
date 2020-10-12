using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command.CQRS.Events.Models.Fascicle;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.BiblosDS.BiblosDS;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.Receiver.Base.Exceptions;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Fascicles;
using VecompSoftware.Services.Command.CQRS.Events.Models.Fascicles;

namespace VecompSoftware.ServiceBus.Module.Entities.Listener.InsertFascicle
{
    public class Execution : IListenerExecution<ICommandBuildFascicle>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly BiblosClient _biblosClient;
        private readonly IWebAPIClient _webApiClient;
        protected static IEnumerable<LogCategory> _logCategories = null;
        private readonly List<Archive> _biblosArchives;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IDictionary<DocSuiteWeb.Model.Entities.Fascicles.FascicleType, string> _fascicleTypeInsertActionDescription;
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
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
        public IDictionary<string, object> Properties { get; set; }
        public EvaluationModel RetryPolicyEvaluation { get; set; }
        public Guid? IdWorkflowActivity { get; set; }

        #endregion

        #region [ Constructor ]
        public Execution(ILogger logger, IWebAPIClient webApiClient, BiblosClient biblosClient)
        {
            _logger = logger;
            _biblosClient = biblosClient;
            _webApiClient = webApiClient;
            _serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
            _fascicleTypeInsertActionDescription = new Dictionary<DocSuiteWeb.Model.Entities.Fascicles.FascicleType, string> {
                { DocSuiteWeb.Model.Entities.Fascicles.FascicleType.Activity, EnumHelper.GetDescription(InsertActionType.InsertActivityFascicle) },
                {  DocSuiteWeb.Model.Entities.Fascicles.FascicleType.Procedure, EnumHelper.GetDescription(InsertActionType.InsertProcedureFascicle) }
            };
        }
        #endregion

        public async Task ExecuteAsync(ICommandBuildFascicle command)
        {
            Fascicle fascicle = new Fascicle();
            FascicleBuildModel fascicleBuildModel = command.ContentType.ContentTypeValue;
            FascicleModel fascicleModel = fascicleBuildModel.Fascicle;
            IdWorkflowActivity = fascicleBuildModel.IdWorkflowActivity;
            List<DossierFolderModel> dossierFolderDoAction = new List<DossierFolderModel>();
            string fascicleInsertActionType;
            if (!fascicleModel.FascicleType.HasValue || !_fascicleTypeInsertActionDescription.TryGetValue(fascicleModel.FascicleType.Value, out fascicleInsertActionType))
            {
                _logger.WriteError(new LogMessage($"Undefined or unsupported fascicle type {fascicleModel.FascicleType}"), LogCategories);

                throw new ServiceBusEvaluationException(new EvaluationModel { CommandName = nameof(command), CorrelationId = command.CorrelationId });
            }

            try
            {
                if (RetryPolicyEvaluation != null && !string.IsNullOrEmpty(RetryPolicyEvaluation.ReferenceModel))
                {
                    fascicleModel = JsonConvert.DeserializeObject<FascicleModel>(RetryPolicyEvaluation.ReferenceModel, _serializerSettings);
                }
                else
                {
                    RetryPolicyEvaluation = new EvaluationModel();
                }

                //Attraverso le WebAPI comunicando col verbo POST col controller Rest Fascicle creare l'entità Fascicle
                #region [ Creazione Fascicle ]

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY"))
                {
                    if (!fascicleModel.Category.IdCategory.HasValue)
                    {
                        throw new ArgumentNullException("fascicleModel.Category.IdCategory", "IdCategory must be defined");
                    }

                    if (fascicleModel.UniqueId == Guid.Empty)
                    {
                        throw new ArgumentException("fascicleModel.UniqueId", $"Invalid FascicleModel UniqueId {fascicleModel.UniqueId}");
                    }

                    fascicle.MetadataValues = fascicleModel.MetadataValues;
                    fascicle.UniqueId = fascicleModel.UniqueId;
                    fascicle.Rack = fascicleModel.Rack;
                    fascicle.Note = fascicleModel.Note;
                    fascicle.Manager = fascicleModel.Manager;
                    fascicle.FascicleObject = fascicleModel.FascicleObject;
                    fascicle.MetadataDesigner = fascicleModel.MetadataDesigner;

                    if (fascicleModel.FascicleTemplate != null)
                    {
                        fascicle.FascicleTemplate = new ProcessFascicleTemplate
                        {
                            JsonModel = fascicleModel.FascicleTemplate.JsonModel,
                            Name = fascicleModel.FascicleTemplate.Name,
                            UniqueId = fascicleModel.FascicleTemplate.UniqueId ?? Guid.NewGuid(),
                            StartDate = fascicleModel.FascicleTemplate.StartDate,
                            EndDate = fascicleModel.FascicleTemplate.EndDate,
                            RegistrationDate = fascicleModel.FascicleTemplate.RegistrationDate,
                            RegistrationUser = fascicleModel.FascicleTemplate.RegistrationUser
                        };
                    }

                    if (fascicleModel.MetadataRepository != null)
                    {
                        fascicle.MetadataRepository = new MetadataRepository() { UniqueId = fascicleModel.MetadataRepository.Id };
                    }

                    if (fascicleModel.FascicleType != null)
                    {
                        fascicle.FascicleType = (DocSuiteWeb.Entity.Fascicles.FascicleType)fascicleModel.FascicleType.Value;
                    }

                    if (fascicleModel.VisibilityType != null)
                    {
                        fascicle.VisibilityType = (DocSuiteWeb.Entity.Fascicles.VisibilityType)fascicleModel.VisibilityType.Value;
                    }

                    if (fascicleModel.Conservation != null)
                    {
                        fascicle.Conservation = fascicleModel.Conservation.Value;
                    }

                    if (fascicleModel.DossierFolders.Any())
                    {
                        dossierFolderDoAction.AddRange(fascicleModel.DossierFolders.Where(f => f.Status == DocSuiteWeb.Model.Entities.Dossiers.DossierFolderStatus.DoAction));
                        List<DossierFolder> dossierFolders = new List<DossierFolder>();
                        DossierFolder folder;
                        foreach (DossierFolderModel dossierFolderModel in fascicleModel.DossierFolders.Where(f => f.Status != DocSuiteWeb.Model.Entities.Dossiers.DossierFolderStatus.DoAction))
                        {
                            if (dossierFolderModel.Status == DocSuiteWeb.Model.Entities.Dossiers.DossierFolderStatus.Fascicle || dossierFolderModel.Status == DocSuiteWeb.Model.Entities.Dossiers.DossierFolderStatus.FascicleClose)
                            {
                                _logger.WriteWarning(new LogMessage($"Fascicle {fascicle.GetTitle()} has wrong dossierfolder({dossierFolderModel.UniqueId}) reference type and it going to replace with parent association"), LogCategories);
                                folder = await _webApiClient.GetDossierFolderParentAsync(dossierFolderModel.UniqueId);
                                folder.Category = new Category() { EntityShortId = (short)fascicleModel.Category.IdCategory.Value };
                                if (folder == null)
                                {
                                    _logger.WriteError(new LogMessage($"DossierFolder {dossierFolderModel.UniqueId} doesn't have parent. it will be skip"), LogCategories);
                                    continue;
                                }
                                dossierFolders.Add(folder);
                            }
                            else
                            {
                                dossierFolders.Add(new DossierFolder()
                                {
                                    UniqueId = dossierFolderModel.UniqueId,
                                    Status = (VecompSoftware.DocSuiteWeb.Entity.Dossiers.DossierFolderStatus)dossierFolderModel.Status,
                                    Category = dossierFolderModel.IdCategory.HasValue ? new Category() { EntityShortId = dossierFolderModel.IdCategory.Value} : null
                                });
                            }

                        }
                        fascicle.DossierFolders = dossierFolders;
                    }

                    fascicle.Contacts = new List<Contact>();
                    if (fascicleModel.Contacts != null && fascicleModel.Contacts.Count > 0)
                    {
                        foreach (ContactModel contact in fascicleModel.Contacts.Where(f => f.EntityId.HasValue))
                        {
                            fascicle.Contacts.Add(new Contact() { EntityId = contact.EntityId.Value });
                        }
                    }

                    fascicle.Category = new Category() { EntityShortId = (short)fascicleModel.Category.IdCategory.Value };

                    fascicle.FascicleRoles = new List<FascicleRole>();
                    if (fascicleModel.FascicleRoles != null && fascicleModel.FascicleRoles.Count > 0)
                    {
                        fascicle.FascicleRoles = fascicleModel.FascicleRoles.Where(x => x.Role.IdRole.HasValue).Select(s => 
                            new FascicleRole() 
                            { 
                                Role = new Role() 
                                { 
                                    EntityShortId = s.Role.IdRole.Value 
                                }, 
                                IsMaster = s.IsMaster,
                                AuthorizationRoleType = (AuthorizationRoleType)s.AuthorizationRoleType
                            }).ToList();
                    }

                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "ENTITY",
                        LocalReference = JsonConvert.SerializeObject(fascicle, _serializerSettings)
                    });
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY");
                    fascicle = JsonConvert.DeserializeObject<Fascicle>(messageStatus.LocalReference);
                }

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY_CREATED"))
                {
                    fascicle.WorkflowName = fascicleBuildModel.WorkflowName;
                    fascicle.IdWorkflowActivity = fascicleBuildModel.IdWorkflowActivity;
                    fascicle.WorkflowAutoComplete = fascicleBuildModel.WorkflowAutoComplete;
                    foreach (IWorkflowAction workflowAction in fascicleBuildModel.WorkflowActions)
                    {
                        fascicle.WorkflowActions.Add(workflowAction);
                    }

                    _logger.WriteDebug(new LogMessage(JsonConvert.SerializeObject(fascicle, _serializerSettings)), LogCategories);
                    fascicle = await _webApiClient.PostEntityAsync(fascicle, fascicleInsertActionType);
                    _logger.WriteInfo(new LogMessage($"Fascicle {fascicle.GetTitle()} has been created"), LogCategories);
                    DossierFolder dossierFolder;
                    foreach (DossierFolderModel item in dossierFolderDoAction)
                    {
                        dossierFolder = await _webApiClient.PutEntityAsync(new DossierFolder() 
                        {
                            UniqueId = item.UniqueId,
                            Category = new Category() { EntityShortId = fascicle.Category.EntityShortId },
                            Fascicle = fascicle,
                            JsonMetadata = item.JsonMetadata,
                            Name = item.Name,
                            Status = DocSuiteWeb.Entity.Dossiers.DossierFolderStatus.Fascicle
                        });
                        _logger.WriteInfo(new LogMessage($"DoAction DossierFolder {item.Name} update to new fascicle"), LogCategories);
                        fascicle.DossierFolders.Add(dossierFolder);
                    }
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "ENTITY_CREATED",
                        LocalReference = JsonConvert.SerializeObject(fascicle, _serializerSettings)
                    });
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY_CREATED"); //Posso fare first direttamente perche sono nell else
                    fascicle = JsonConvert.DeserializeObject<Fascicle>(messageStatus.LocalReference);
                }
                #endregion

                #region [ Creazione FascicleFolders ]
                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "FASCICLE_FOLDERS"))
                {
                    if (fascicleModel.FascicleFolders.Count > 0)
                    {
                        FascicleFolder defaultFolder = await _webApiClient.GetDefaultFascicleFolderAsync(fascicle.UniqueId);
                        FascicleFolder fascicleFolder;
                        foreach (FascicleFolderModel fascicleFolderModel in fascicleModel.FascicleFolders)
                        {
                            fascicleFolder = new FascicleFolder()
                            {
                                UniqueId = fascicleFolderModel.UniqueId,
                                Fascicle = fascicle,
                                ParentInsertId = defaultFolder.UniqueId,
                                Name = fascicleFolderModel.Name,
                                Status = (DocSuiteWeb.Entity.Fascicles.FascicleFolderStatus)fascicleFolderModel.Status,
                                Typology = (DocSuiteWeb.Entity.Fascicles.FascicleFolderTypology)fascicleFolderModel.Typology
                            };
                            await _webApiClient.PostEntityAsync(fascicleFolder);
                            _logger.WriteInfo(new LogMessage($"Fascicle folder {fascicleFolder.Name} has been created"), LogCategories);
                        }
                    }
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "FASCICLE_FOLDERS",
                        LocalReference = JsonConvert.SerializeObject(fascicle, _serializerSettings)
                    });
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "FASCICLE_FOLDERS");
                    fascicle = JsonConvert.DeserializeObject<Fascicle>(messageStatus.LocalReference);
                }
                #endregion

                //Attraverso le WebAPI comunicando col verbo POST col controller Rest FascicleDocument e inserire tutti gli inserti inseriti in Biblos popolando la navigation property FascicleDocuments (FascicleDocument table) coi relativi identificativi Guid di Biblos
                #region [ Creazione FascicleDocument ]
                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "FASCICLE"))
                {
                    foreach (FascicleDocumentModel model in fascicleModel.FascicleDocuments)
                    {
                        fascicle.FascicleDocuments.Add(new FascicleDocument
                        {
                            UniqueId = model.UniqueId,
                            ChainType = (DocSuiteWeb.Entity.DocumentUnits.ChainType)model.ChainType,
                            IdArchiveChain = model.IdArchiveChain,
                            RegistrationUser = model.RegistrationUser,
                            RegistrationDate = model.RegistrationDate.Value,
                            LastChangedUser = model.LastChangedUser
                        });
                    }
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "FASCICLE",
                        LocalReference = JsonConvert.SerializeObject(fascicle, _serializerSettings)
                    });
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "FASCICLE");
                    fascicle = JsonConvert.DeserializeObject<Fascicle>(messageStatus.LocalReference);
                }

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "FASCICLE_CREATED"))
                {
                    fascicle = await _webApiClient.PutEntityAsync(fascicle, fascicle.FascicleType == DocSuiteWeb.Entity.Fascicles.FascicleType.Activity ? UpdateActionType.ActivityFascicleUpdate.ToString() : string.Empty);
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "FASCICLE_CREATED",
                        LocalReference = JsonConvert.SerializeObject(fascicle, _serializerSettings)
                    });
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "FASCICLE_CREATED");
                    fascicle = JsonConvert.DeserializeObject<Fascicle>(messageStatus.LocalReference);
                }
                #endregion

                //Attraverso le WebAPI comunicando col verbo POST inviare l'evento EventoCompleteFascicleBuild
                #region [ EventoCompleteFascicleBuild ]

                fascicleModel.RegistrationDate = fascicle.RegistrationDate;
                fascicleModel.RegistrationUser = fascicle.RegistrationUser;
                fascicleModel.Year = fascicle.Year;
                fascicleModel.Title = fascicle.Title;
                fascicleModel.StartDate = fascicle.StartDate;
                fascicleModel.Number = fascicle.Number;
                fascicleBuildModel.Fascicle = fascicleModel;
                IEventCompleteFascicleBuild eventCompleteFascicleBuild = new EventCompleteFascicleBuild(Guid.NewGuid(), command.CorrelationId ?? fascicleBuildModel.UniqueId,
                    command.TenantName, command.TenantId, command.TenantAOOId, command.Identity, fascicleBuildModel, null);

                if (!await _webApiClient.PushEventAsync(eventCompleteFascicleBuild))
                {
                    _logger.WriteError(new LogMessage($"EventCompleteFascicleBuild {fascicle.GetTitle()} has not been sended"), LogCategories);
                    throw new Exception("IEventCompleteFascicleBuild not sended");
                }
                _logger.WriteInfo(new LogMessage($"EventCompleteFascicleBuild {eventCompleteFascicleBuild.Id} has been sended"), LogCategories);

                #endregion
            }
            catch (Exception ex)
            {
                RetryPolicyEvaluation.ReferenceModel = JsonConvert.SerializeObject(fascicleModel, _serializerSettings);

                _logger.WriteError(ex, LogCategories);
                throw new ServiceBusEvaluationException(RetryPolicyEvaluation);
            }
        }
    }
}
