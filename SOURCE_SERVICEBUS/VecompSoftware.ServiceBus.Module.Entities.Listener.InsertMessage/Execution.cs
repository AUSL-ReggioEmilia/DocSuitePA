using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command.CQRS.Events.Models.Messages;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
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
        private readonly ILogger _logger;
        private readonly BiblosClient _biblosClient;
        private readonly IWebAPIClient _webApiClient;
        private readonly JsonSerializerSettings _serializerSettings;
        protected static IEnumerable<LogCategory> _logCategories = null;
        private readonly List<Archive> _biblosArchives;
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
            _serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
            _biblosArchives = _biblosClient.Document.GetArchives();
        }
        #endregion

        public async Task ExecuteAsync(ICommandBuildMessage command)
        {
            Message message = new Message();

            MessageBuildModel messageBuildModel = command.ContentType.ContentTypeValue;
            MessageModel messageModel = messageBuildModel.Message;
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
                    foreach (MessageAttachmentModel model in messageModel.MessageAttachments)
                    {
                        message.MessageAttachments.Add(new MessageAttachment()
                        {
                            Archive = model.Archive,
                            ChainId = model.ChainId.Value,
                            DocumentEnum = model.DocumentEnum,
                            Extension = model.Extension,
                            Server = model.Server
                        });
                    }
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

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "CREATE_ATTACHMENTS") && messageModel.MessageAttachments.Any())
                {
                    string attachName = messageModel.MessageAttachments.First().Archive;
                    Archive messageAttachmentAndAnnexedArchive = _biblosArchives.Single(f => f.Name.Equals(attachName, StringComparison.InvariantCultureIgnoreCase));

                    List<BiblosDS.BiblosDS.Attribute> attachmentAttributes = _biblosClient.Document.GetAttributesDefinition(messageAttachmentAndAnnexedArchive.Name);

                    foreach (MessageAttachmentModel messageAttachmentModel in messageModel.MessageAttachments)
                    {
                        //CREO CATENA IDENTIFICATIVA
                        Guid? attachmentChainId = messageAttachmentModel.Document.ChainId;
                        if (!attachmentChainId.HasValue)
                        {
                            //cerchi attachmentChainId dagli Attachments/Annexed
                            attachmentChainId = _biblosClient.Document.CreateDocumentChain(messageAttachmentAndAnnexedArchive.Name, new List<AttributeValue>());
                            messageAttachmentModel.Document.ChainId = attachmentChainId;
                        }
                        List<AttributeValue> attachmentAttributeValues;
                        int pos = messageModel.MessageAttachments.Count(f => f.Document.DocumentId.HasValue);

                        if (messageAttachmentModel.Document.DocumentId.HasValue)
                        {
                            DocumentModel attachment = messageAttachmentModel.Document;
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
                            _logger.WriteDebug(new LogMessage(string.Concat("biblos attachment archive name is ", messageAttachmentAndAnnexedArchive.Name)), LogCategories);

                            //CREO IL DOCUMENTO
                            Document attachmentMessageDocument = new Document
                            {
                                Archive = messageAttachmentAndAnnexedArchive,
                                Content = new Content { Blob = attachment.ContentStream },
                                Name = attachment.FileName,
                                IsVisible = true,
                                AttributeValues = attachmentAttributeValues
                            };

                            //ASSOCIO IL DOCUMENTO ALLA SUA CATENA DI COMPETENZA
                            attachmentMessageDocument = _biblosClient.Document.AddDocumentToChain(attachmentMessageDocument, attachmentChainId, ContentFormat.Binary);
                            attachment.DocumentId = attachmentMessageDocument.IdDocument;
                            _logger.WriteInfo(new LogMessage(string.Concat("inserted document ", attachmentMessageDocument.IdDocument.ToString(), " in archive ", messageAttachmentAndAnnexedArchive.IdArchive.ToString())), LogCategories);

                            //Se fa questo ad ogni iterazione lo sovrascriva ma non crea problemi in quanto DocumentParent è sempre lo stesso
                            //message.MessageAttachments.FirstOrDefault(x => x.ChainId == messageAttachmentModel.ChainId).ChainId = attachmentMessageDocument.DocumentParent.IdBiblos.Value;
                            message.MessageLogs.Add(new MessageLog() { LogType = MessageLogType.Created, LogDescription = $"Allegato (Add): {attachment.FileName}" });
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

                //Attraverso le WebAPI comunicando col verbo PUT col controller Rest Message e aggiornare l'entità di Message popolando la navigation property MessageAttachments (MessageAttachment table) coi relativi identificativi INT di Biblos e attivare il Messaggio con Status =MessageStatus.Active

                #region Aggiornare Message

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY_UPDATED"))
                {
                    foreach (MessageAttachmentModel model in messageModel.MessageAttachments)
                    {
                        message.MessageAttachments.Add(new MessageAttachment()
                        {
                            Archive = model.Archive,
                            ChainId = model.ChainId.Value,
                            DocumentEnum = model.DocumentEnum,
                            Extension = model.Extension,
                            Server = model.Server
                        });
                    }
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

                //Attraverso le WebAPI comunicando col verbo POST inviare l'evento EventoCompleteMessageBuild

                #region [ EventCompleteMessageBuild ]
                messageBuildModel.Message = messageModel;
                foreach (MessageAttachmentModel item in messageModel.MessageAttachments)
                {
                    item.Document.ContentStream = null;
                }
                IEventCompleteMessageBuild eventCompleteMessageBuild = new EventCompleteMessageBuild(Guid.NewGuid(), messageBuildModel.UniqueId,
                    command.TenantName, command.TenantId, command.Identity, messageBuildModel, null);
                if (!await _webApiClient.PushEventAsync(eventCompleteMessageBuild))
                {
                    _logger.WriteError(new LogMessage($"EventCompleteMessageBuild {message.GetTitle()} has not been sended"), LogCategories);
                    throw new Exception("IEventCompleteMessageBuild not sended");
                }
                _logger.WriteInfo(new LogMessage($"EventCompleteMessageBuild {eventCompleteMessageBuild.Id} has been sended"), LogCategories);
                #endregion

            }
            catch (Exception ex)
            {
                RetryPolicyEvaluation.ReferenceModel = JsonConvert.SerializeObject(messageModel, _serializerSettings);

                _logger.WriteError(ex, LogCategories);
                throw new ServiceBusEvaluationException(RetryPolicyEvaluation);
            }
        }
    }
}
