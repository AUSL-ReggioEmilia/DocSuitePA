using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.BiblosDS.BiblosDS;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.Receiver.Base.Exceptions;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Fascicles;

namespace VecompSoftware.ServiceBus.Module.Entities.Listener.UpdateFascicle
{
    public class Execution : IListenerExecution<ICommandUpdateFascicleData>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly BiblosClient _biblosClient;
        private readonly IWebAPIClient _webApiClient;
        protected static IEnumerable<LogCategory> _logCategories = null;
        private readonly List<Archive> _biblosArchives;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly Location _fascicleLocation;
        private const int _retry_tentative = 5;
        private readonly TimeSpan _threadWaiting = TimeSpan.FromSeconds(2);
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
            _biblosArchives = _biblosClient.Document.GetArchives();
            _serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
            try
            {
                short? fascicleLocationId = _webApiClient.GetParameterFascicleMiscellaneaLocation().Result;
                if (fascicleLocationId.HasValue)
                {
                    _fascicleLocation = _webApiClient.GetLocationAsync(fascicleLocationId.Value).Result;
                }
                if (_fascicleLocation == null)
                {
                    throw new ArgumentException("Fascicle Location is empty", "FascicleLocation");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("error orrouring in get fascicle parameters"), ex, LogCategories);
                throw;
            }
        }
        #endregion

        public async Task ExecuteAsync(ICommandUpdateFascicleData command)
        {
            _logger.WriteInfo(new LogMessage($"{command.CommandName} is arrived"), LogCategories);

            FascicleBuildModel fascicleBuildModel = command.ContentType.ContentTypeValue;
            FascicleModel fascicleModel = fascicleBuildModel.Fascicle;

            try
            {
                if (RetryPolicyEvaluation != null && !string.IsNullOrEmpty(RetryPolicyEvaluation.ReferenceModel))
                {
                    _logger.WriteDebug(new LogMessage("Load reference model from RetryPolicyEvaluation"), LogCategories);
                    fascicleModel = JsonConvert.DeserializeObject<FascicleModel>(RetryPolicyEvaluation.ReferenceModel, _serializerSettings);
                }
                else
                {
                    _logger.WriteDebug(new LogMessage("Generate new RetryPolicyEvaluation model"), LogCategories);
                    RetryPolicyEvaluation = new EvaluationModel();
                }
                _logger.WriteInfo(new LogMessage($"Cancel requested for fascicle {fascicleModel.Title}/{fascicleModel.UniqueId}-{fascicleModel.FascicleObject}"), LogCategories);
                _logger.WriteDebug(new LogMessage($"Cancel requested from WorkflowName {fascicleBuildModel.WorkflowName} and IdWorkflowActivity {fascicleBuildModel.IdWorkflowActivity}"), LogCategories);

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY_UPDATED"))
                {
                    Fascicle updatedFascicle = await UpdateData(fascicleBuildModel, fascicleModel);
                    _logger.WriteInfo(new LogMessage($"Fascicle {updatedFascicle.GetTitle()} has been updated"), LogCategories);
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "ENTITY_UPDATED",
                        LocalReference = JsonConvert.SerializeObject(fascicleModel, _serializerSettings)
                    });
                    _logger.WriteDebug(new LogMessage("Set ENTITY_UPDATED RetryPolicyEvaluation"), LogCategories);
                }
                else
                {
                    StepModel fascicleStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY_UPDATED");
                    fascicleModel = JsonConvert.DeserializeObject<FascicleModel>(fascicleStatus.LocalReference);
                    _logger.WriteDebug(new LogMessage("Load fascicle entity from RetryPolicyEvaluation ENTITY_UPDATED"), LogCategories);
                }
            }
            catch (Exception ex)
            {
                RetryPolicyEvaluation.ReferenceModel = JsonConvert.SerializeObject(fascicleModel, _serializerSettings);
                _logger.WriteError(ex, LogCategories);
                throw new ServiceBusEvaluationException(RetryPolicyEvaluation);
            }
        }

        private async Task<Fascicle> UpdateData(FascicleBuildModel fascicleBuildModel, FascicleModel entity)
        {
            Fascicle entityTransformed;
            try
            {
                entityTransformed = await _webApiClient.GetFascicleByIdAsync(entity.UniqueId);

                if (entityTransformed == null)
                {
                    _logger.WriteError(new LogMessage($"Fascicle with id {entity.UniqueId} not found"), LogCategories);
                    throw new Exception("Fascicle not found");
                }

                entityTransformed.WorkflowName = fascicleBuildModel.WorkflowName;
                entityTransformed.IdWorkflowActivity = fascicleBuildModel.IdWorkflowActivity;
                entityTransformed.WorkflowAutoComplete = fascicleBuildModel.WorkflowAutoComplete;
                foreach (IWorkflowAction workflowAction in fascicleBuildModel.WorkflowActions)
                {
                    entityTransformed.WorkflowActions.Add(workflowAction);
                }

                entityTransformed.Name = entity.Name;
                entityTransformed.Note = entity.Note;
                entityTransformed.Rack = entity.Rack;
                entityTransformed.FascicleObject = entity.FascicleObject;
                if (entityTransformed.FascicleType == DocSuiteWeb.Entity.Fascicles.FascicleType.Legacy)
                {
                    entityTransformed.Manager = entity.Manager;
                }
                if (entityTransformed.FascicleType != DocSuiteWeb.Entity.Fascicles.FascicleType.Activity && entity.Contacts != null)
                {
                    foreach (Contact item in entityTransformed.Contacts.Where(f => !entity.Contacts.Any(c => c.EntityId == f.EntityId)).ToList())
                    {
                        entityTransformed.Contacts.Remove(item);
                    }
                    foreach (ContactModel item in entity.Contacts.Where(f => !entityTransformed.Contacts.Any(c => c.EntityId == f.EntityId)))
                    {
                        entityTransformed.Contacts.Add(new Contact() { EntityId = item.EntityId.Value });
                    }
                }
                if (entity.FascicleDocuments != null && entity.FascicleDocuments.Count > 0)
                {
                    Archive fascicleMiscellaneaArchive = _biblosArchives.Single(f => f.Name.Equals(_fascicleLocation.ProtocolArchive, StringComparison.InvariantCultureIgnoreCase));
                    List<BiblosDS.BiblosDS.Attribute> miscellaneaAttributes = _biblosClient.Document.GetAttributesDefinition(fascicleMiscellaneaArchive.Name);
                    DocumentModel documentInserted;
                    FascicleFolder fascicleFolder;
                    FascicleDocument currentFascicleDocument;
                    foreach (FascicleDocumentModel item in entity.FascicleDocuments.Where(f => !entityTransformed.FascicleDocuments.Any(c => c.UniqueId == f.UniqueId)))
                    {
                        if (item.Document == null || !item.Document.DocumentToStoreId.HasValue)
                        {
                            _logger.WriteError(new LogMessage($"Document not defined for fascicle document {item.UniqueId}."), LogCategories);
                            continue;
                        }
                        fascicleFolder = null;
                        if (item.FascicleFolder != null)
                        {
                            fascicleFolder = await _webApiClient.GetFascicleFolderAsync(item.FascicleFolder.UniqueId);
                        }
                        if (fascicleFolder == null)
                        {
                            fascicleFolder = await _webApiClient.GetDefaultFascicleFolderAsync(entityTransformed.UniqueId);
                        }
                        _logger.WriteInfo(new LogMessage($"Adding document {item.Document.FileName} into fascicle folder {fascicleFolder.Name}({fascicleFolder.UniqueId})."), LogCategories);
                        currentFascicleDocument = await _webApiClient.GetFascicleDocumentByFolderAsync(fascicleFolder.UniqueId);
                        item.Document.ChainId = currentFascicleDocument?.IdArchiveChain;
                        documentInserted = ArchiveFascicleMiscellanea(item, fascicleMiscellaneaArchive, miscellaneaAttributes);
                        if (currentFascicleDocument == null)
                        {
                            entityTransformed.FascicleDocuments.Add(new FascicleDocument()
                            {
                                IdArchiveChain = documentInserted.ChainId.Value,
                                ChainType = DocSuiteWeb.Entity.DocumentUnits.ChainType.Miscellanea,
                                FascicleFolder = fascicleFolder
                            });
                        }                        
                    }
                }
                if (!string.IsNullOrEmpty(entity.MetadataDesigner))
                {
                    entityTransformed.MetadataDesigner = entity.MetadataDesigner;
                }
                if (!string.IsNullOrEmpty(entity.MetadataValues))
                {
                    entityTransformed.MetadataValues = entity.MetadataValues;
                }

                entityTransformed = await _webApiClient.PutEntityAsync(entityTransformed);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
            return entityTransformed;
        }

        private DocumentModel ArchiveFascicleMiscellanea(FascicleDocumentModel fascicleDocumentModel, Archive miscellaneaArchive, List<BiblosDS.BiblosDS.Attribute> miscellaneaAttributes)
        {
            Guid? documentChainId = fascicleDocumentModel.Document.ChainId;
            if (!documentChainId.HasValue)
            {
                documentChainId = _biblosClient.Document.CreateDocumentChain(miscellaneaArchive.Name, new List<AttributeValue>());
                fascicleDocumentModel.Document.ChainId = documentChainId;
            }
            List<AttributeValue> miscellaneaAttributeValues;

            DocumentModel document = fascicleDocumentModel.Document;
            miscellaneaAttributeValues = new List<AttributeValue>()
                            {
                                new AttributeValue()
                                {
                                    Attribute = miscellaneaAttributes.Single(f => f.Name.Equals(AttributeHelper.AttributeName_Filename, StringComparison.InvariantCultureIgnoreCase)),
                                    Value = document.FileName,
                                }
                            };
            document.ChainId = documentChainId;
            _logger.WriteInfo(new LogMessage($"reading document content {fascicleDocumentModel.Document.DocumentToStoreId} ..."), LogCategories);
            Content documentContent = RetryingPolicyAction(() => _biblosClient.Document.GetDocumentContentById(fascicleDocumentModel.Document.DocumentToStoreId.Value));

            Document miscellaneaDocument = new Document
            {
                Archive = miscellaneaArchive,
                Content = new Content { Blob = documentContent.Blob },
                Name = document.FileName,
                IsVisible = true,
                AttributeValues = miscellaneaAttributeValues
            };

            miscellaneaDocument = _biblosClient.Document.AddDocumentToChain(miscellaneaDocument, documentChainId, ContentFormat.Binary);
            document.DocumentId = miscellaneaDocument.IdDocument;
            _logger.WriteDebug(new LogMessage($"biblos document {document.FileName} archived into {miscellaneaArchive.Name}"), LogCategories);
            return document;
        }

        private T RetryingPolicyAction<T>(Func<T> func, int step = 1)
        {
            _logger.WriteDebug(new LogMessage($"RetryingPolicyAction : tentative {step}/{_retry_tentative} in progress..."), LogCategories);
            if (step >= _retry_tentative)
            {
                _logger.WriteError(new LogMessage("VecompSoftware.ServiceBus.Module.Entities.Listener.UpdateFascicle.RetryingPolicyAction: retry policy expired maximum tentatives"), LogCategories);
                throw new Exception("UpdateFascicle retry policy expired maximum tentatives");
            }
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                _logger.WriteWarning(new LogMessage($"SafeActionWithRetryPolicy : tentative {step}/{_retry_tentative} faild. Waiting {_threadWaiting} second before retrying action"), ex, LogCategories);
                Task.Delay(_threadWaiting).Wait();
                return RetryingPolicyAction(func, ++step);
            }
        }
    }
}
