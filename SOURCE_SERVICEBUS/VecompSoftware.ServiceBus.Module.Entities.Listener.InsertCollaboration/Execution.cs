using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command.CQRS.Events.Models.Collaborations;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.BiblosDS.BiblosDS;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.Receiver.Base.Exceptions;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Collaborations;
using VecompSoftware.Services.Command.CQRS.Events.Models.Collaborations;

namespace VecompSoftware.ServiceBus.Module.Entities.Listener.InsertCollaboration
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class Execution : IListenerExecution<ICommandBuildCollaboration>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly BiblosClient _biblosClient;
        private readonly IWebAPIClient _webApiClient;
        protected static IEnumerable<LogCategory> _logCategories = null;
        private readonly StampaConforme.StampaConformeClient _stampaConformeClient;
        private readonly List<Archive> _biblosArchives;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly string _corporateAcronym;
        private readonly string _corporateName;
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
            //_stampaConformeClient = stampaConformeClient;
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
                _corporateAcronym = _webApiClient.GetParameterCorporateAcronymAsync().Result;
                _corporateName = _webApiClient.GetParameterCorporateNameAsync().Result;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("error orrouring in get signature parameters"), ex, LogCategories);
                throw;
            }
        }

        #endregion

        #region [ Methods ]

        public async Task ExecuteAsync(ICommandBuildCollaboration command)
        {
            Collaboration collaboration = new Collaboration();

            CollaborationBuildModel collaborationBuildModel = command.ContentType.ContentTypeValue;
            CollaborationModel collaborationModel = collaborationBuildModel.Collaboration;
            try
            {
                if (RetryPolicyEvaluation != null && !string.IsNullOrEmpty(RetryPolicyEvaluation.ReferenceModel))
                {
                    collaborationModel = JsonConvert.DeserializeObject<CollaborationModel>(RetryPolicyEvaluation.ReferenceModel, _serializerSettings);
                }
                else
                {
                    RetryPolicyEvaluation = new EvaluationModel();
                }

                //Attraverso le WebAPI comunicando col verbo POST col controller Rest Collaboration creare l'entità Collaboration
                #region Creazione l'entità Collaboration

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY"))
                {
                    collaboration.UniqueId = Guid.NewGuid();
                    collaboration.TemplateName = collaborationModel.TemplateName;
                    collaboration.Subject = collaborationModel.Subject;
                    collaboration.SignCount = (short?)collaborationModel.CollaborationSigns.Count();
                    collaboration.PublicationUser = collaborationModel.PublicationUser;
                    collaboration.Note = collaborationModel.Note;
                    collaboration.MemorandumDate = collaborationModel.MemorandumDate;
                    collaboration.IdStatus = collaborationModel.IdStatus;
                    collaboration.IdPriority = collaborationModel.IdPriority;
                    collaboration.DocumentType = collaborationModel.DocumentType;
                    collaboration.AlertDate = collaborationModel.AlertDate;
                    collaboration.PublicationDate = collaborationModel.PublicationDate;

                    short signerNumber = 0;
                    foreach (CollaborationSignModel model in collaborationModel.CollaborationSigns)
                    {
                        signerNumber += 1;
                        collaboration.CollaborationSigns.Add(new CollaborationSign()
                        {
                            Incremental = signerNumber,
                            IsActive = model.IsActive,
                            SignUser = model.SignUser,
                            SignName = model.SignName,
                            SignEmail = model.SignEmail,
                            SignDate = model.SignDate,
                            IsRequired = model.IsRequired,
                            IsAbsent = model.IsAbsent
                        });
                    }

                    signerNumber = 0;
                    foreach (CollaborationUserModel model in collaborationModel.CollaborationUsers)
                    {
                        signerNumber += 1;
                        collaboration.CollaborationUsers.Add(new CollaborationUser()
                        {
                            Incremental = signerNumber,
                            DestinationFirst = model.DestinationFirst,
                            DestinationType = model.DestinationType,
                            DestinationName = model.DestinationName,
                            DestinationEmail = model.DestinationEmail,
                            Account = model.Account,
                            Role = model.IdRole.HasValue ? new Role()
                            {
                                EntityShortId = model.IdRole.Value
                            } : null

                        });
                    }
                }
                else
                {
                    StepModel collaborationStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY"); //Posso fare first direttamente perche sono nell else
                    collaboration = JsonConvert.DeserializeObject<Collaboration>(collaborationStatus.LocalReference);
                }
                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY_CREATED"))
                {
                    collaboration.WorkflowName = collaborationBuildModel.WorkflowName;
                    collaboration.IdWorkflowActivity = collaborationBuildModel.IdWorkflowActivity;
                    foreach (IWorkflowAction workflowAction in collaborationBuildModel.WorkflowActions)
                    {
                        collaboration.WorkflowActions.Add(workflowAction);
                    }

                    collaboration = await _webApiClient.PostEntityAsync(collaboration);
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "ENTITY_CREATED",
                        LocalReference = JsonConvert.SerializeObject(collaboration, _serializerSettings)
                    });
                }
                else
                {
                    StepModel collaborationStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY_CREATED"); //Posso fare first direttamente perche sono nell else
                    collaboration = JsonConvert.DeserializeObject<Collaboration>(collaborationStatus.LocalReference);
                }

                #endregion

                //Attraverso il layer di BiblosDS salvare tutti gli documenti con relativa segnatura (metadato)
                #region Creazione Documenti

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "CREATE_DOCUMENT") && collaborationModel.CollaborationVersionings.Any())
                {
                    string docName = collaborationModel.CollaborationVersionings.First().Document.FileName;
                    Archive collaborationDocument = _biblosArchives.Single(f => f.Name.Equals(docName, StringComparison.InvariantCultureIgnoreCase));
                    List<BiblosDS.BiblosDS.Attribute> attachmentAttributes = _biblosClient.Document.GetAttributesDefinition(collaborationDocument.Name);

                    foreach (CollaborationVersioningModel collaborationVersioningModel in collaborationModel.CollaborationVersionings)
                    {
                        //CREO CATENA IDENTIFICATIVA
                        Guid? documentChainId = collaborationVersioningModel.Document.ChainId;
                        if (!documentChainId.HasValue)
                        {
                            documentChainId = _biblosClient.Document.CreateDocumentChain(collaborationDocument.Name, new List<AttributeValue>());
                            collaborationVersioningModel.Document.ChainId = documentChainId;
                        }
                        List<AttributeValue> attachmentAttributeValues;
                        int pos = collaborationModel.CollaborationVersionings.Count(f => f.Document.DocumentId.HasValue);

                        if (collaborationVersioningModel.Document.DocumentId.HasValue)
                        {
                            DocumentModel attachment = collaborationVersioningModel.Document;
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
                            attachment.ChainId = documentChainId;
                            _logger.WriteDebug(new LogMessage(string.Concat("biblos attachment archive name is ", collaborationDocument.Name)), LogCategories);

                            //CREO IL DOCUMENTO
                            Document attachmentCollaborationDocument = new Document
                            {
                                Archive = collaborationDocument,
                                Content = new Content { Blob = attachment.ContentStream },
                                Name = attachment.FileName,
                                IsVisible = true,
                                AttributeValues = attachmentAttributeValues
                            };

                            //ASSOCIO IL DOCUMENTO ALLA SUA CATENA DI COMPETENZA
                            attachmentCollaborationDocument = _biblosClient.Document.AddDocumentToChain(attachmentCollaborationDocument, documentChainId, ContentFormat.Binary);
                            attachment.DocumentId = attachmentCollaborationDocument.IdDocument;
                            _logger.WriteInfo(new LogMessage(string.Concat("inserted document ", attachmentCollaborationDocument.IdDocument.ToString(), " in archive ", collaborationDocument.IdArchive.ToString())), LogCategories);

                            collaboration.CollaborationLogs.Add(new CollaborationLog() { LogType = CollaborationLogType.MODIFICA_SEMPLICE, LogDescription = $"Documento (Add): {attachment.FileName}" });
                        }

                    }

                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "CREATE_DOCUMENT",
                        LocalReference = JsonConvert.SerializeObject(collaboration, _serializerSettings)
                    });
                }

                else
                {
                    StepModel messageCollaboration = RetryPolicyEvaluation.Steps.FirstOrDefault(f => f.Name == "CREATE_DOCUMENT");
                    if (messageCollaboration != null)
                    {
                        collaboration = JsonConvert.DeserializeObject<Collaboration>(messageCollaboration.LocalReference);
                    }
                }


                #endregion

                //Attraverso le WebAPI comunicando col verbo POST col controller Rest CollaborationVersioning e creare le entità CollaborationVersioning figlie di Collaboration popolando la navigation property CollaborationVersionings (CollaborationVersionin table) coi relativi identificativi INT di Biblos

                #region Creare Collaboration

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY_CollaborationCREATED"))
                {
                    foreach (CollaborationVersioningModel model in collaborationModel.CollaborationVersionings)
                    {
                        collaboration.CollaborationVersionings.Add(new CollaborationVersioning()
                        {
                            CollaborationIncremental = model.CollaborationIncremental,
                            Incremental = model.Incremental,
                            IdDocument = model.IdDocument,
                            DocumentName = model.DocumentName,
                            CheckedOut = model.CheckedOut,
                            CheckOutUser = model.CheckOutUser,
                            CheckOutDate = model.CheckOutDate,
                            DocumentGroup = model.DocumentGroup,
                        });
                    }
                }
                else
                {
                    StepModel collaborationStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY_CollaborationCREATED");
                    collaboration = JsonConvert.DeserializeObject<Collaboration>(collaborationStatus.LocalReference);
                }
                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY_CollaborationCREATED"))
                {
                    collaboration = await _webApiClient.PostEntityAsync(collaboration);
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "ENTITY_CollaborationCREATED",
                        LocalReference = JsonConvert.SerializeObject(collaboration, _serializerSettings)
                    });
                }
                else
                {
                    StepModel collaborationStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY_CollaborationCREATED");
                    collaboration = JsonConvert.DeserializeObject<Collaboration>(collaborationStatus.LocalReference);
                }

                #endregion

                //Attraverso le WebAPI comunicando col verbo POST inviare l'evento EventoCompleteCollaborationBuild

                #region [ EventCompleteCollaborationBuild ]
                collaborationBuildModel.Collaboration.IdCollaboration = collaborationModel.IdCollaboration;
                IEventCompleteCollaborationBuild eventCompleteCollaborationBuild = new EventCompleteCollaborationBuild(Guid.NewGuid(), collaborationBuildModel.UniqueId,
                    command.TenantName, command.TenantId, command.Identity, collaborationBuildModel, null);
                if (!await _webApiClient.PushEventAsync(eventCompleteCollaborationBuild))
                {
                    _logger.WriteError(new LogMessage($"EventCompleteCollaborationBuild {collaboration.GetTitle()} has not been sended"), LogCategories);
                    throw new Exception("IEventCompleteCollaborationBuild not sended");
                }
                _logger.WriteInfo(new LogMessage($"EventCompleteCollaborationBuild {eventCompleteCollaborationBuild.Id} has been sended"), LogCategories);
                #endregion

            }
            catch (Exception ex)
            {
                RetryPolicyEvaluation.ReferenceModel = JsonConvert.SerializeObject(collaborationModel, _serializerSettings);

                _logger.WriteError(ex, LogCategories);
                throw new ServiceBusEvaluationException(RetryPolicyEvaluation);
            }
        }

        #endregion
    }
}
