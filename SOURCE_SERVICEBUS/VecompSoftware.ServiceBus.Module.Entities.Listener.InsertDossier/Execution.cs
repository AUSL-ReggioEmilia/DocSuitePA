using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Core.Command.CQRS.Events.Models.Dossiers;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.Receiver.Base.Exceptions;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Dossiers;
using VecompSoftware.Services.Command.CQRS.Events.Models.Dossiers;
using DossierStatus = VecompSoftware.DocSuiteWeb.Entity.Dossiers.DossierStatus;
using DossierType = VecompSoftware.DocSuiteWeb.Entity.Dossiers.DossierType;
using DossierStatusModel = VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers.DossierStatus;
using DossierTypeModel = VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers.DossierType;

namespace VecompSoftware.ServiceBus.Module.Entities.Listener.InsertDossier
{
    public class Execution : IListenerExecution<ICommandBuildDossier>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webApiClient;
        protected static IEnumerable<LogCategory> _logCategories = null;
        private readonly JsonSerializerSettings _serializerSettings;

        private const string STEP_PREPARE_ENTITY = "DOSSIER_PREPARE";
        private const string STEP_CREATE_ENTITY = "DOSSIER_CREATE";
        private const string STEP_ADD_DOCUMENTS = "DOSSIER_DOCUMENTS";
        private const string STEP_UPDATE_DOSSIER = "DOSSIER_UPDATE";
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
        public Execution(ILogger logger, IWebAPIClient webApiClient)
        {
            _logger = logger;
            _webApiClient = webApiClient;
            _serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
        }
        #endregion

        public async Task ExecuteAsync(ICommandBuildDossier command)
        {
            Dossier dossier = new Dossier();
            DossierBuildModel dossierBuildModel = command.ContentType.ContentTypeValue;
            DossierModel dossierModel = dossierBuildModel.Dossier;
            IdWorkflowActivity = dossierBuildModel.IdWorkflowActivity;

            try
            {
                if (RetryPolicyEvaluation != null && !string.IsNullOrEmpty(RetryPolicyEvaluation.ReferenceModel))
                {
                    dossierModel = JsonConvert.DeserializeObject<DossierModel>(RetryPolicyEvaluation.ReferenceModel, _serializerSettings);
                }
                else
                {
                    RetryPolicyEvaluation = new EvaluationModel();
                }

                #region [ STEP PREPARE ENTITY ]
                if (!RetryPolicyEvaluation.Steps.Any(d => d.Name == STEP_PREPARE_ENTITY))
                {
                    ProcessPrepareEntityStepEvaluation(dossier, dossierModel);

                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = STEP_PREPARE_ENTITY,
                        LocalReference = JsonConvert.SerializeObject(dossier, _serializerSettings)
                    });
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == STEP_PREPARE_ENTITY);
                    dossier = JsonConvert.DeserializeObject<Dossier>(messageStatus.LocalReference);
                }
                #endregion

                #region [ STEP CREATE ENTITY ]
                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == STEP_CREATE_ENTITY))
                {
                    dossier.WorkflowName = dossierBuildModel.WorkflowName;
                    dossier.IdWorkflowActivity = dossierBuildModel.IdWorkflowActivity;
                    dossier.WorkflowAutoComplete = dossierBuildModel.WorkflowAutoComplete;
                    dossier.WorkflowActions = dossierBuildModel.WorkflowActions;

                    _logger.WriteDebug(new LogMessage(JsonConvert.SerializeObject(dossier, _serializerSettings)), LogCategories);
                    dossier = await _webApiClient.PostEntityAsync(dossier);
                    _logger.WriteInfo(new LogMessage($"Dossier {dossier.Year}/{dossier.Number:0000000} ({dossier.UniqueId}) has been created"), LogCategories);

                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = STEP_CREATE_ENTITY,
                        LocalReference = JsonConvert.SerializeObject(dossier, _serializerSettings)
                    });
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == STEP_CREATE_ENTITY);
                    dossier = JsonConvert.DeserializeObject<Dossier>(messageStatus.LocalReference);
                }
                #endregion

                #region [ STEP ADD DOSSIER DOCUMENTS ]
                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == STEP_ADD_DOCUMENTS))
                {
                    if (dossierModel.Documents != null && dossierModel.Documents.Any())
                    {
                        dossier.DossierDocuments = dossierModel.Documents.Select(docModel => new DossierDocument
                        {
                            IdArchiveChain = docModel.IdArchiveChain
                        }).ToList();
                    }
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == STEP_ADD_DOCUMENTS);
                    dossier = JsonConvert.DeserializeObject<Dossier>(messageStatus.LocalReference);
                }
                #endregion

                #region [ STEP UPDATE DOSSIER ]
                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == STEP_UPDATE_DOSSIER))
                {
                    dossier = await _webApiClient.PutEntityAsync(dossier);
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = STEP_UPDATE_DOSSIER,
                        LocalReference = JsonConvert.SerializeObject(dossier, _serializerSettings)
                    });
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == STEP_UPDATE_DOSSIER);
                    dossier = JsonConvert.DeserializeObject<Dossier>(messageStatus.LocalReference);
                }
                #endregion

                #region [ STEP COMPLETE DOSSIER BUILD]
                dossierModel.Year = dossier.Year;
                dossierModel.Number = dossier.Number;
                dossierModel.Title = $"Dossier {dossier.Year}/{dossier.Number:0000000}";
                dossierModel.RegistrationDate = dossier.RegistrationDate;
                dossierModel.RegistrationUser = dossier.RegistrationUser;
                dossierModel.LastChangedDate = dossier.LastChangedDate;
                dossierModel.LastChangedUser = dossier.LastChangedUser;
                dossierModel.StartDate = dossier.StartDate;
                dossierModel.Subject = dossier.Subject;
                dossierModel.DossierType = (DossierTypeModel)dossier.DossierType;

                dossierBuildModel.Dossier = dossierModel;
                IEventCompleteDossierBuild eventCompleteDossierBuild = new EventCompleteDossierBuild(Guid.NewGuid(), command.CorrelationId ?? dossierBuildModel.UniqueId,
                    command.TenantName, command.TenantId, command.TenantAOOId, command.Identity, dossierBuildModel, null);

                if (!await _webApiClient.PushEventAsync(eventCompleteDossierBuild))
                {
                    _logger.WriteError(new LogMessage($"EventCompleteDossierBuild {dossier.GetTitle()} has not been sent"), LogCategories);
                    throw new Exception("IEventCompleteDossierBuild not sent");
                }
                _logger.WriteInfo(new LogMessage($"EventCompleteDossierBuild {eventCompleteDossierBuild.Id} has been sent"), LogCategories);
                #endregion
            }
            catch (Exception ex)
            {
                RetryPolicyEvaluation.ReferenceModel = JsonConvert.SerializeObject(dossierModel, _serializerSettings);

                _logger.WriteError(ex, LogCategories);
                throw new ServiceBusEvaluationException(RetryPolicyEvaluation);
            }
        }

        private void ProcessPrepareEntityStepEvaluation(Dossier dossier, DossierModel dossierModel)
        {
            dossier.UniqueId = dossierModel.UniqueId;
            dossier.Note = dossierModel.Note;
            dossier.Subject = dossierModel.Subject;
            dossier.MetadataDesigner = dossierModel.MetadataDesigner;
            dossier.MetadataValues = dossierModel.MetadataValues;
            dossier.Container = new Container { EntityShortId = dossierModel.ContainerId };
            dossier.DossierType = (DossierType)dossierModel.DossierType;
            dossier.Status = DossierStatus.Open;

            if (dossierModel.Category != null)
            {
                dossier.Category = new Category { EntityShortId = (short)dossierModel.Category.IdCategory };
            }

            if (dossierModel.MetadataRepository != null)
            {
                dossier.MetadataRepository = new MetadataRepository() { UniqueId = dossierModel.MetadataRepository.Id };
            }

            if (dossierModel.Contacts != null && dossierModel.Contacts.Count > 0)
            {
                dossier.Contacts = new HashSet<Contact>();
                foreach (ContactModel contact in dossierModel.Contacts.Where(f => f.Id.HasValue))
                {
                    dossier.Contacts.Add(new Contact() { EntityId = contact.Id.Value });
                }
            }

            if (dossierModel.Roles != null)
            {
                dossier.DossierRoles = new HashSet<DossierRole>();

                foreach (DossierRoleModel dossierRoleModel in dossierModel.Roles.Where(drm => drm.Role != null && drm.Role.IdRole.HasValue))
                {
                    dossier.DossierRoles.Add(new DossierRole { Role = new Role { EntityShortId = dossierRoleModel.Role.IdRole.Value } });
                }
            }
        }
    }
}
