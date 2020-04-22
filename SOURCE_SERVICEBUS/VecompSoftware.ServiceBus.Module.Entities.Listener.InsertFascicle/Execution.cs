using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command.CQRS.Events.Models.Fascicle;
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
        }
        #endregion

        public async Task ExecuteAsync(ICommandBuildFascicle command)
        {
            Fascicle fascicle = new Fascicle();
            FascicleBuildModel fascicleBuildModel = command.ContentType.ContentTypeValue;
            FascicleModel fascicleModel = fascicleBuildModel.Fascicle;

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
                        throw new ArgumentNullException("fascicleModel.Category.IdCategory", "IdCategory must be define");
                    }

                    fascicle.MetadataValues = fascicleModel.MetadataValues;
                    fascicle.UniqueId = fascicleModel.UniqueId;
                    fascicle.Rack = fascicleModel.Rack;
                    fascicle.Note = fascicleModel.Note;
                    fascicle.Manager = fascicleModel.Manager;
                    fascicle.FascicleObject = fascicleModel.FascicleObject;

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
                    fascicle.Contacts = new List<Contact>();
                    if (fascicleModel.Contacts != null && fascicleModel.Contacts.Count > 0)
                    {
                        foreach (ContactModel contact in fascicleModel.Contacts.Where(f => f.Id.HasValue))
                        {
                            fascicle.Contacts.Add(new Contact() { EntityId = contact.Id.Value });
                        }
                    }

                    fascicle.Category = new Category() { EntityShortId = (short)fascicleModel.Category.IdCategory.Value };

                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "ENTITY",
                        LocalReference = JsonConvert.SerializeObject(fascicle, _serializerSettings)
                    });
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY"); //Posso fare first direttamente perche sono nell else
                    fascicle = JsonConvert.DeserializeObject<Fascicle>(messageStatus.LocalReference);
                }

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY_CREATED"))
                {
                    fascicle.WorkflowName = fascicleBuildModel.WorkflowName;
                    fascicle.IdWorkflowActivity = fascicleBuildModel.IdWorkflowActivity;
                    foreach (IWorkflowAction workflowAction in fascicleBuildModel.WorkflowActions)
                    {
                        fascicle.WorkflowActions.Add(workflowAction);
                    }

                    fascicle = await _webApiClient.PostEntityAsync(fascicle);
                    _logger.WriteInfo(new LogMessage($"Fascicle {fascicle.GetTitle()} has been created"), LogCategories);                    

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
                        FascicleFolder folder;
                        foreach (FascicleFolderModel fascicleFolder in fascicleModel.FascicleFolders)
                        {
                            folder = new FascicleFolder()
                            {
                                UniqueId = fascicleFolder.UniqueId,
                                Fascicle = fascicle,
                                ParentInsertId = defaultFolder.UniqueId,
                                Name = fascicleFolder.Name,
                                Status = (DocSuiteWeb.Entity.Fascicles.FascicleFolderStatus)fascicleFolder.Status,
                                Typology = (DocSuiteWeb.Entity.Fascicles.FascicleFolderTypology)fascicleFolder.Typology
                            };
                            await _webApiClient.PostEntityAsync(folder);
                            _logger.WriteInfo(new LogMessage($"Fascicle folder {folder.Name} has been created"), LogCategories);
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

                //Attraverso il layer di BiblosDS salvare tutti gli inserti con relativa segnatura (metadato)
                #region [ Salvare tutti gli allegati]
                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "CREATE_BIBLOS") && fascicleModel.DocumentUnits.Any())
                {
                    Guid documentUnitName = fascicleModel.FascicleDocuments.First().IdArchiveChain;
                    Archive documentAttachmentAndAnnexedArchive = _biblosArchives.Single(f => f.IdArchive.Equals(documentUnitName));

                    List<BiblosDS.BiblosDS.Attribute> attachmentAttributes = _biblosClient.Document.GetAttributesDefinition(documentAttachmentAndAnnexedArchive.Name);
                    foreach (FascicleDocumentModel documentUnitModel in fascicleModel.FascicleDocuments)
                    {

                        //CREO CATENA IDENTIFICATIVA
                        Guid? attachmentChainId = documentUnitModel.Document.ChainId;
                        if (!attachmentChainId.HasValue)
                        {
                            //cerchi attachmentChainId dagli Attachments/Annexed
                            attachmentChainId = _biblosClient.Document.CreateDocumentChain(documentAttachmentAndAnnexedArchive.Name, new List<AttributeValue>());
                            documentUnitModel.Document.ChainId = attachmentChainId;
                        }
                        List<AttributeValue> attachmentAttributeValues;
                        int pos = fascicleModel.FascicleDocuments.Count(f => f.Document.DocumentId.HasValue);

                        if (documentUnitModel.Document.DocumentId.HasValue)
                        {
                            DocumentModel attachment = documentUnitModel.Document;
                            attachmentAttributeValues = new List<AttributeValue>()
                        {
                            new AttributeValue()
                            {
                                Attribute = attachmentAttributes.Single(f =>
                                    f.Name.Equals(AttributeHelper.AttributeName_Filename,
                                        StringComparison.InvariantCultureIgnoreCase)),
                                Value = attachment.FileName,
                            },
                            new AttributeValue()
                            {
                                Attribute = attachmentAttributes.Single(f =>
                                    f.Name.Equals(AttributeHelper.AttributeName_Signature,
                                        StringComparison.InvariantCultureIgnoreCase)),
                                Value = attachment.Segnature,
                            },
                            new AttributeValue()
                            {
                                Attribute = attachmentAttributes.Single(f =>
                                    f.Name.Equals(AttributeHelper.AttributeName_PrivacyLevel,
                                        StringComparison.InvariantCultureIgnoreCase)),
                                Value = 0,
                            }
                            };
                            attachment.ChainId = attachmentChainId;
                            documentUnitModel.IdArchiveChain = attachmentChainId.Value;
                            _logger.WriteDebug(new LogMessage(string.Concat("biblos attachment archive name is ", documentAttachmentAndAnnexedArchive.Name)), LogCategories);

                            //CREO IL DOCUMENTO
                            Document attachmentFascicleDocument = new Document
                            {
                                Archive = documentAttachmentAndAnnexedArchive,
                                Content = new Content { Blob = attachment.ContentStream },
                                Name = attachment.FileName,
                                IsVisible = true,
                                AttributeValues = attachmentAttributeValues
                            };

                            //ASSOCIO IL DOCUMENTO ALLA SUA CATENA DI COMPETENZA
                            attachmentFascicleDocument = _biblosClient.Document.AddDocumentToChain(attachmentFascicleDocument, attachmentChainId, ContentFormat.Binary);
                            attachment.DocumentId = attachmentFascicleDocument.IdDocument;
                            _logger.WriteInfo(new LogMessage(string.Concat("inserted document ", attachmentFascicleDocument.IdDocument.ToString(), " in archive ", documentAttachmentAndAnnexedArchive.IdArchive.ToString())), LogCategories);

                            //Se fa questo ad ogni iterazione lo sovrascriva ma non crea problemi in quanto DocumentParent è sempre lo stesso
                            //message.MessageAttachments.FirstOrDefault(x => x.ChainId == messageAttachmentModel.ChainId).ChainId = attachmentMessageDocument.DocumentParent.IdBiblos.Value;
                            fascicle.FascicleLogs.Add(new FascicleLog() { LogType = FascicleLogType.DocumentInsert, LogDescription = $"Allegato (Add): {attachment.FileName}" });
                        }
                    }
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "CREATE_BIBLOS",
                        LocalReference = JsonConvert.SerializeObject(fascicle, _serializerSettings)
                    });
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.FirstOrDefault(f => f.Name == "CREATE_BIBLOS");
                    if (messageStatus != null)
                    {
                        fascicle = JsonConvert.DeserializeObject<Fascicle>(messageStatus.LocalReference);
                    }
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
                    fascicle = await _webApiClient.PutEntityAsync(fascicle);
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

                fascicleModel.RegistrationUser = fascicle.RegistrationUser;
                fascicleModel.Year = fascicle.Year;
                fascicleModel.Title = fascicle.Title;
                fascicleModel.StartDate = fascicle.StartDate;
                fascicleModel.Number = fascicle.Number;

                fascicleBuildModel.Fascicle = fascicleModel;

                IEventCompleteFascicleBuild eventCompleteFascicleBuild = new EventCompleteFascicleBuild(Guid.NewGuid(), fascicleBuildModel.UniqueId, 
                    command.TenantName, command.TenantId, command.Identity, fascicleBuildModel, null);

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
