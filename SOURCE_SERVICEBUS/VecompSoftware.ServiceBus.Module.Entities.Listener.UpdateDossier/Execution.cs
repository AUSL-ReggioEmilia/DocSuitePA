using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.Receiver.Base.Exceptions;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Entities.Dossiers;

namespace VecompSoftware.ServiceBus.Module.Entities.Listener.UpdateDossier
{
    public class Execution : IListenerExecution<ICommandUpdateDossierData>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webApiClient;
        protected static IEnumerable<LogCategory> _logCategories = null;
        private readonly JsonSerializerSettings _serializerSettings;

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

        public async Task ExecuteAsync(ICommandUpdateDossierData command)
        {
            _logger.WriteInfo(new LogMessage($"{command.CommandName} is arrived"), LogCategories);

            Dossier dossier = command.ContentType.ContentTypeValue;
            try
            {
                if (RetryPolicyEvaluation != null && !string.IsNullOrEmpty(RetryPolicyEvaluation.ReferenceModel))
                {
                    _logger.WriteDebug(new LogMessage("Load reference model from RetryPolicyEvaluation"), LogCategories);
                    dossier = JsonConvert.DeserializeObject<Dossier>(RetryPolicyEvaluation.ReferenceModel, _serializerSettings);
                }
                else
                {
                    _logger.WriteDebug(new LogMessage("Generate new RetryPolicyEvaluation model"), LogCategories);
                    RetryPolicyEvaluation = new EvaluationModel();
                }

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == STEP_UPDATE_DOSSIER))
                {
                    Dossier updatedDossier = await UpdateDossierEntity(dossier);
                    _logger.WriteInfo(new LogMessage($"Dossier {updatedDossier.GetTitle()} has been updated"), LogCategories);
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = STEP_UPDATE_DOSSIER,
                        LocalReference = JsonConvert.SerializeObject(updatedDossier, _serializerSettings)
                    });
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == STEP_UPDATE_DOSSIER);
                    dossier = JsonConvert.DeserializeObject<Dossier>(messageStatus.LocalReference);
                }

            }
            catch (Exception ex)
            {
                RetryPolicyEvaluation.ReferenceModel = JsonConvert.SerializeObject(dossier, _serializerSettings);
                _logger.WriteError(ex, LogCategories);
                throw new ServiceBusEvaluationException(RetryPolicyEvaluation);
            }
        }

        private async Task<Dossier> UpdateDossierEntity(Dossier entity)
        {
            try
            {
                Dossier entityTransformed = await _webApiClient.GetDossierByIdAsync(entity.UniqueId);

                if (entityTransformed == null)
                {
                    _logger.WriteError(new LogMessage($"Dossier with id {entity.UniqueId} not found"), LogCategories);
                    throw new Exception("Dossier not found");
                }

                entityTransformed.UniqueId = entity.UniqueId;
                entityTransformed.WorkflowName = entity.WorkflowName;
                entityTransformed.IdWorkflowActivity = entity.IdWorkflowActivity;
                entityTransformed.WorkflowAutoComplete = entity.WorkflowAutoComplete;
                entityTransformed.WorkflowActions = entity.WorkflowActions;
                entityTransformed.Container = entity.Container;
                entityTransformed.Contacts = entity.Contacts;
                entityTransformed.DossierDocuments = entity.DossierDocuments;
                entityTransformed.EndDate = entity.EndDate;
                entityTransformed.MetadataDesigner = entity.MetadataDesigner;
                entityTransformed.MetadataRepository = entity.MetadataRepository;
                entityTransformed.SourceMetadataValues = entity.SourceMetadataValues;
                entityTransformed.MetadataValueContacts = entity.MetadataValueContacts;
                entityTransformed.MetadataValues = entity.MetadataValues;
                entityTransformed.Note = entity.Note;
                entityTransformed.Subject = entity.Subject;
                entityTransformed.StartDate = entity.StartDate;
                entityTransformed.Category = entity.Category;
                entityTransformed.DossierType = entity.DossierType;
                entityTransformed.Status = entity.Status;

                entityTransformed = await _webApiClient.PutEntityAsync(entityTransformed);

                return entityTransformed;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }
    }
}
