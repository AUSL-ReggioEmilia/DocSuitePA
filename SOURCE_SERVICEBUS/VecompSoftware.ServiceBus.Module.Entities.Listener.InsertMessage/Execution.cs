using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command.CQRS.Events.Models.Messages;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.Messages;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.BiblosDS.BiblosDS;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.Receiver.Base.Exceptions;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Messages;
using VecompSoftware.Services.Command.CQRS.Events.Models.Messages;

namespace VecompSoftware.ServiceBus.Module.Entities.Listener.InsertMessage
{
    public class Execution : IListenerExecution<ICommandBuildMessage>
    {
        #region [ Fields ]
        private const int _retry_tentative = 5;
        private readonly TimeSpan _threadWaiting = TimeSpan.FromSeconds(2);

        private readonly ILogger _logger;
        private readonly BiblosClient _biblosClient;
        private readonly IWebAPIClient _webApiClient;
        private readonly JsonSerializerSettings _serializerSettings;
        protected static IEnumerable<LogCategory> _logCategories = null;
        private readonly List<Archive> _biblosArchives;
        private readonly Location _attachementLocation;
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
            _biblosArchives = _biblosClient.Document.GetArchives();

            short locationId = _webApiClient.GetParameterMessageLocation().Result.Value;
            _attachementLocation = this._webApiClient.GetLocationAsync(locationId).Result;
        }

        #endregion

        public async Task ExecuteAsync(ICommandBuildMessage command)
        {
            Message message = new Message();

            MessageBuildModel messageBuildModel = command.ContentType.ContentTypeValue;
            MessageModel messageModel = messageBuildModel.Message;
            IdWorkflowActivity = messageBuildModel.IdWorkflowActivity;
            try
            {
                if (RetryPolicyEvaluation != null && !string.IsNullOrEmpty(RetryPolicyEvaluation.ReferenceModel))
                {
                    messageModel = JsonConvert.DeserializeObject<MessageModel>(RetryPolicyEvaluation.ReferenceModel, _serializerSettings);
                }

                else
                {
                    RetryPolicyEvaluation = new EvaluationModel();
                }


                //Attraverso le WebAPI comunicando col verbo POST col controller Rest Message creare l'entità Message con Status=MessageStatus.Draft
                #region Creazione Message in stato bozza


                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY"))
                {

                    foreach (MessageContactModel model in messageModel.MessageContacts)
                    {
                        ICollection<MessageContactEmail> messageContactEmails = new List<MessageContactEmail>();
                        foreach (MessageContactEmailModel messageContactEmailModel in model.MessageContactEmail)
                        {
                            messageContactEmails.Add(new MessageContactEmail()
                            {
                                Description = messageContactEmailModel.Description,
                                Email = messageContactEmailModel.Email,
                                User = messageContactEmailModel.User
                            });
                        }
                        message.MessageContacts.Add(new MessageContact()
                        {
                            ContactPosition = (MessageContactPosition)model.ContactPosition,
                            ContactType = (DocSuiteWeb.Entity.Messages.MessageContactType)model.ContactType,
                            Description = model.Description,
                            MessageContactEmail = messageContactEmails
                        });
                    }
                    foreach (MessageEmailModel model in messageModel.MessageEmails)
                    { 
                        message.MessageEmails.Add(new MessageEmail()
                        {
                            Body = model.Body,
                            EmlDocumentId = model.EmlDocumentId,
                            IsDispositionNotification = model.IsDispositionNotification,
                            Priority = model.Priority,
                            SentDate = model.SentDate,
                            Subject = model.Subject
                        });
                    }
                    message.MessageType = (DocSuiteWeb.Entity.Messages.MessageType)messageModel.MessageType;
                    message.Status = DocSuiteWeb.Entity.Messages.MessageStatus.Draft;
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "ENTITY",
                        LocalReference = JsonConvert.SerializeObject(message, _serializerSettings)
                    });
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY"); //Posso fare first direttamente perche sono nell else
                    message = JsonConvert.DeserializeObject<Message>(messageStatus.LocalReference);
                }
                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY_CREATED"))
                {
                    message = await _webApiClient.PostEntityAsync(message);
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "ENTITY_CREATED",
                        LocalReference = JsonConvert.SerializeObject(message, _serializerSettings)
                    });
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY_CREATED"); //Posso fare first direttamente perche sono nell else
                    message = JsonConvert.DeserializeObject<Message>(messageStatus.LocalReference);
                }

                #endregion

                //Attraverso il layer di BiblosDS salvare tutti gli allegati con relativa segnatura (metadato)

                #region Creazione Documenti Allegati (Attachments OPTIONAL)

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "CREATE_ATTACHMENTS") && messageModel.MessageAttachments.Any(f => f.Document != null && f.Document.DocumentToStoreId.HasValue))
                {
                    //CREO CATENA IDENTIFICATIVA
                    Guid? documentChainId;
                    Content documentContent;
                    List<AttributeValue> attachmentAttributeValues;

                    Archive messageArchive = _biblosArchives.Single(f => f.Name.Equals(_attachementLocation.ProtocolArchive, StringComparison.InvariantCultureIgnoreCase));
                    _logger.WriteDebug(new LogMessage($"biblos attachment archive name is {messageArchive.Name}"), LogCategories);

                    List<BiblosDS.BiblosDS.Attribute> attachmentAttributes = _biblosClient.Document.GetAttributesDefinition(messageArchive.Name);
                    int index = 0;

                    foreach (MessageAttachmentModel messageAttachmentModel in messageModel.MessageAttachments.Where(f => f.Document != null && !f.Document.DocumentId.HasValue && f.Document.DocumentToStoreId.HasValue))
                    {
                        //CREO CATENA IDENTIFICATIVA
                        documentChainId = messageAttachmentModel.Document.ChainId; //Fix

                        if (!documentChainId.HasValue)
                        {
                            documentChainId = _biblosClient.Document.CreateDocumentChain(messageArchive.Name, new List<AttributeValue>());
                        }

                        messageAttachmentModel.Document.ChainId = documentChainId;

                        if (!messageAttachmentModel.Document.DocumentId.HasValue)
                        {
                            attachmentAttributeValues = new List<AttributeValue>()
                            {
                                new AttributeValue()
                                {
                                    Attribute = attachmentAttributes.Single(f => f.Name.Equals(AttributeHelper.AttributeName_Filename, StringComparison.InvariantCultureIgnoreCase)),
                                    Value = messageAttachmentModel.Document.FileName,
                                },
                                new AttributeValue()
                                {
                                    Attribute = attachmentAttributes.Single(f => f.Name.Equals(AttributeHelper.AttributeName_Signature, StringComparison.InvariantCultureIgnoreCase)),
                                    Value = messageAttachmentModel.Document.Segnature,
                                }
                            };

                            _logger.WriteInfo(new LogMessage($"reading document content {messageAttachmentModel.Document.DocumentToStoreId} ..."), LogCategories);
                            documentContent = RetryingPolicyAction(() => _biblosClient.Document.GetDocumentContentById(messageAttachmentModel.Document.DocumentToStoreId.Value));

                            //CREO IL DOCUMENTO
                            Document attachmentCollaborationDocument = new Document
                            {
                                Archive = messageArchive,
                                Content = new Content { Blob = documentContent.Blob },
                                Name = messageAttachmentModel.Document.FileName,
                                IsVisible = true,
                                AttributeValues = attachmentAttributeValues
                            };

                            //ASSOCIO IL DOCUMENTO ALLA SUA CATENA DI COMPETENZA
                            attachmentCollaborationDocument = _biblosClient.Document.AddDocumentToChain(attachmentCollaborationDocument, documentChainId, ContentFormat.Binary);
                            messageAttachmentModel.Document.DocumentId = attachmentCollaborationDocument.IdDocument;
                            messageAttachmentModel.Archive = messageArchive.Name;
                            messageAttachmentModel.DocumentEnum = index++;

                            _logger.WriteDebug(new LogMessage($"biblos document {messageAttachmentModel.Document.FileName} archived into {messageArchive.Name}"), LogCategories);

                            message.MessageAttachments.Add(new MessageAttachment()
                            {
                                Archive = messageAttachmentModel.Archive,
                                ChainId = attachmentCollaborationDocument.IdBiblos.Value,
                                DocumentEnum = messageAttachmentModel.DocumentEnum,
                                Extension = messageAttachmentModel.Extension
                            });

                            _logger.WriteInfo(new LogMessage($"reading document content {messageAttachmentModel.Document.DocumentToStoreId} ..."), LogCategories);
                            documentContent = RetryingPolicyAction(() => _biblosClient.Document.GetDocumentContentById(messageAttachmentModel.Document.DocumentToStoreId.Value));

                            //CREO IL DOCUMENTO
                            Document attachmentMessageDocument = new Document
                            {
                                Archive = messageArchive,
                                Content = new Content { Blob = documentContent.Blob },
                                Name = messageAttachmentModel.Document.FileName,
                                IsVisible = true,
                                AttributeValues = attachmentAttributeValues
                            };

                            message.MessageLogs.Add(new MessageLog() { LogType = MessageLogType.Created, LogDescription = $"Allegato (Add): {messageAttachmentModel.Document.FileName}" });
                        }
                    }
                    RetryPolicyEvaluation.Steps.Add(new StepModel()

                    {
                        Name = "CREATE_ATTACHMENTS",
                        LocalReference = JsonConvert.SerializeObject(message, _serializerSettings)
                    });
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.FirstOrDefault(f => f.Name == "CREATE_ATTACHMENTS");
                    if (messageStatus != null)
                    {
                        message = JsonConvert.DeserializeObject<Message>(messageStatus.LocalReference);
                    }
                }

                #endregion

                #region Aggiornare Message

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY_UPDATED"))
                {
                    message.Status = DocSuiteWeb.Entity.Messages.MessageStatus.Active;
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY_UPDATED");
                    message = JsonConvert.DeserializeObject<Message>(messageStatus.LocalReference);
                }
                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY_UPDATED"))
                {
                    message.WorkflowName = messageBuildModel.WorkflowName;
                    message.IdWorkflowActivity = messageBuildModel.IdWorkflowActivity;
                    message.WorkflowAutoComplete = messageBuildModel.WorkflowAutoComplete;
                    foreach (IWorkflowAction workflowAction in messageBuildModel.WorkflowActions)
                    {
                        message.WorkflowActions.Add(workflowAction);
                    }
                    message = await _webApiClient.PutEntityAsync(message);
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "ENTITY_UPDATED",
                        LocalReference = JsonConvert.SerializeObject(message, _serializerSettings)
                    });
                }
                else
                {
                    StepModel messageStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY_UPDATED");
                    message = JsonConvert.DeserializeObject<Message>(messageStatus.LocalReference);
                }

                #endregion

                #region [ EventCompleteMessageBuild ]
                messageBuildModel.Message = messageModel;
                IEventCompleteMessageBuild eventCompleteMessageBuild = new EventCompleteMessageBuild(Guid.NewGuid(), messageBuildModel.UniqueId,
                    command.TenantName, command.TenantId, command.TenantAOOId, command.Identity, messageBuildModel, null);
                if (!await _webApiClient.PushEventAsync(eventCompleteMessageBuild))
                {
                    _logger.WriteError(new LogMessage($"EventCompleteMessageBuild {message.GetTitle()} has not been sended"), LogCategories);
                    throw new Exception("IEventCompleteMessageBuild not sended");
                }
                _logger.WriteInfo(new LogMessage($"EventCompleteMessageBuild {eventCompleteMessageBuild.Id} has been sended"), LogCategories);
                #endregion

                #region Detach documenti archivio workflow
                foreach (MessageAttachmentModel attachment in messageModel.MessageAttachments.Where(f => f.Document != null && f.Document.DocumentToStoreId.HasValue))
                {
                    _logger.WriteInfo(new LogMessage($"detaching workflow document {attachment.Document.DocumentToStoreId} ..."), LogCategories);
                    RetryingPolicyAction(() => _biblosClient.Document.DocumentDetach(new Document() { IdDocument = attachment.Document.DocumentToStoreId.Value }));
                }
                #endregion

            }
            catch (Exception ex)
            {
                RetryPolicyEvaluation.ReferenceModel = JsonConvert.SerializeObject(messageModel, _serializerSettings);

                _logger.WriteError(ex, LogCategories);
                throw new ServiceBusEvaluationException(RetryPolicyEvaluation);
            }
        }

        private T RetryingPolicyAction<T>(Func<T> func, int step = 1)
        {
            _logger.WriteDebug(new LogMessage($"RetryingPolicyAction : tentative {step}/{_retry_tentative} in progress..."), LogCategories);
            if (step >= _retry_tentative)
            {
                _logger.WriteError(new LogMessage("VecompSoftware.ServiceBus.Module.Entities.Listener.InsertMessage.RetryingPolicyAction: retry policy expired maximum tentatives"), LogCategories);
                throw new Exception("InsertPECMail retry policy expired maximum tentatives");
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
