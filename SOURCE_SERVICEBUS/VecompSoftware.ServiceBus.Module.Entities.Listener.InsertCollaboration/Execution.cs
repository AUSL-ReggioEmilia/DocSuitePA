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
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Workflows;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.Workflow;
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
        private readonly List<Archive> _biblosArchives;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly Location _collaborationLocation;
        private const int _retry_tentative = 5;
        private readonly TimeSpan _threadWaiting = TimeSpan.FromSeconds(2);
        private const int ADDRESSBOOK_SOURCE_TYPE = 1;
        private const int MANUAL_SOURCE_TYPE = 0;
        private const string GENERIC_CONTACT_TYPE = "U";
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
                int? collaborationLocationId = _webApiClient.GetParameterCollaborationLocation().Result;
                if (collaborationLocationId.HasValue)
                {
                    _collaborationLocation = _webApiClient.GetLocationAsync((short)collaborationLocationId.Value).Result;
                }
                if (_collaborationLocation == null)
                {
                    throw new ArgumentException("Collaboration Location is empty", "CollaborationLocation");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("error orrouring in get collaboration parameters"), ex, LogCategories);
                throw;
            }
        }

        #endregion

        #region [ Methods ]

        public async Task ExecuteAsync(ICommandBuildCollaboration command)
        {
            _logger.WriteDebug(new LogMessage($"new collaboration request ..."), LogCategories);

            Collaboration collaboration = new Collaboration();

            CollaborationBuildModel collaborationBuildModel = command.ContentType.ContentTypeValue;
            CollaborationModel collaborationModel = collaborationBuildModel.Collaboration;
            IdWorkflowActivity = collaborationBuildModel.IdWorkflowActivity;
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
                            IsActive = false,
                            SignUser = model.SignUser,
                            SignName = model.SignName,
                            SignEmail = model.SignEmail,
                            SignDate = model.SignDate,
                            IsRequired = model.IsRequired,
                            IsAbsent = null
                        });
                    }
                    if (collaboration.CollaborationSigns.Count > 0)
                    {
                        collaboration.CollaborationSigns.First().IsActive = true;
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
                    collaboration.WorkflowAutoComplete = collaborationBuildModel.WorkflowAutoComplete;
                    foreach (IWorkflowAction workflowAction in collaborationBuildModel.WorkflowActions)
                    {
                        collaboration.WorkflowActions.Add(workflowAction);
                    }
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

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "CREATE_DOCUMENT"))
                {
                    Archive collaborationDocument = _biblosArchives.Single(f => f.Name.Equals(_collaborationLocation.ProtocolArchive, StringComparison.InvariantCultureIgnoreCase));
                    List<BiblosDS.BiblosDS.Attribute> attachmentAttributes = _biblosClient.Document.GetAttributesDefinition(collaborationDocument.Name);
                    Guid? documentChainId;
                    Content documentContent;
                    short versioningNumber = 0;
                    foreach (CollaborationVersioningModel collaborationVersioningModel in collaborationModel.CollaborationVersionings.Where(f => f.Document != null && f.Document.DocumentToStoreId.HasValue))
                    {
                        //CREO CATENA IDENTIFICATIVA
                        documentChainId = collaborationVersioningModel.Document.ChainId;
                        if (!documentChainId.HasValue)
                        {
                            documentChainId = _biblosClient.Document.CreateDocumentChain(collaborationDocument.Name, new List<AttributeValue>());
                            collaborationVersioningModel.Document.ChainId = documentChainId;
                        }
                        List<AttributeValue> attachmentAttributeValues;
                        int pos = collaborationModel.CollaborationVersionings.Count(f => f.Document.DocumentId.HasValue);

                        if (!collaborationVersioningModel.Document.DocumentId.HasValue)
                        {
                            DocumentModel document = collaborationVersioningModel.Document;
                            attachmentAttributeValues = new List<AttributeValue>()
                            {
                                new AttributeValue()
                                {
                                    Attribute = attachmentAttributes.Single(f => f.Name.Equals(AttributeHelper.AttributeName_Filename, StringComparison.InvariantCultureIgnoreCase)),
                                    Value = document.FileName,
                                },
                                new AttributeValue()
                                {
                                    Attribute = attachmentAttributes.Single(f => f.Name.Equals(AttributeHelper.AttributeName_Signature, StringComparison.InvariantCultureIgnoreCase)),
                                    Value = document.Segnature,
                                }
                            };
                            document.ChainId = documentChainId;
                            _logger.WriteInfo(new LogMessage($"reading document content {collaborationVersioningModel.Document.DocumentToStoreId} ..."), LogCategories);
                            documentContent = RetryingPolicyAction(() => _biblosClient.Document.GetDocumentContentById(collaborationVersioningModel.Document.DocumentToStoreId.Value));

                            //CREO IL DOCUMENTO
                            Document attachmentCollaborationDocument = new Document
                            {
                                Archive = collaborationDocument,
                                Content = new Content { Blob = documentContent.Blob },
                                Name = document.FileName,
                                IsVisible = true,
                                AttributeValues = attachmentAttributeValues
                            };

                            //ASSOCIO IL DOCUMENTO ALLA SUA CATENA DI COMPETENZA
                            attachmentCollaborationDocument = _biblosClient.Document.AddDocumentToChain(attachmentCollaborationDocument, documentChainId, ContentFormat.Binary);
                            document.DocumentId = attachmentCollaborationDocument.IdDocument;
                            _logger.WriteDebug(new LogMessage($"biblos document {document.FileName}/{collaborationVersioningModel.DocumentGroup} archived into {collaborationDocument.Name}"), LogCategories);

                            collaboration.CollaborationLogs.Add(new CollaborationLog() { LogType = CollaborationLogType.MODIFICA_SEMPLICE, LogDescription = $"Documento (Add): {document.FileName}" });
                            collaboration.CollaborationVersionings.Add(new CollaborationVersioning()
                            {
                                CollaborationIncremental = versioningNumber++,
                                DocumentGroup = collaborationVersioningModel.DocumentGroup,
                                DocumentName = document.FileName,
                                IdDocument = attachmentCollaborationDocument.DocumentParent.IdBiblos.Value,
                                Incremental = 1,
                                IsActive = true
                            });
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
                    StepModel stepModel = RetryPolicyEvaluation.Steps.FirstOrDefault(f => f.Name == "CREATE_DOCUMENT");
                    if (stepModel != null)
                    {
                        collaboration = JsonConvert.DeserializeObject<Collaboration>(stepModel.LocalReference);
                    }
                }

                #endregion

                #region Creare Collaboration

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY_CollaborationCREATED"))
                {
                    _logger.WriteDebug(new LogMessage($"Inserting ..."), LogCategories);

                    collaboration = await _webApiClient.PostEntityAsync(collaboration);
                    _logger.WriteDebug(new LogMessage($"Collaboration {collaboration.EntityId} has been created successfully."), LogCategories);
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

                #region [ Creare ProtocolDraft ]
                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY_ProtocolDraftCREATED"))
                {
                    if (collaborationModel.Protocol != null)
                    {
                        _logger.WriteDebug(new LogMessage($"Creating ProtocolDraft for collaboration {collaboration.UniqueId}..."), LogCategories);
                        ProtocolDraftModel protocolDraftModel = new ProtocolDraftModel();
                        protocolDraftModel.Object = collaborationModel.Protocol.Subject;
                        protocolDraftModel.Notes = collaborationModel.Protocol.Note;
                        if (collaborationModel.Protocol.ProtocolType != null)
                        {
                            protocolDraftModel.Type = collaborationModel.Protocol.ProtocolType.EntityShortId;
                        }
                        if (collaborationModel.Protocol.Container != null && collaborationModel.Protocol.Container.IdContainer.HasValue)
                        {
                            protocolDraftModel.Container = collaborationModel.Protocol.Container.IdContainer.Value;
                        }
                        if (collaborationModel.Protocol.Category != null && collaborationModel.Protocol.Category.UniqueId.HasValue)
                        {
                            protocolDraftModel.Category = collaborationModel.Protocol.Category.IdCategory.Value;
                        }
                        if (collaborationModel.Protocol.Roles != null && collaborationModel.Protocol.Roles.Count > 0)
                        {
                            protocolDraftModel.Authorizations.AddRange(collaborationModel.Protocol.Roles.Select(s => (int)s.IdRole.Value));
                        }
                        if (collaborationModel.Protocol.ProtocolContacts != null && collaborationModel.Protocol.ProtocolContacts.Count > 0)
                        {
                            ContactBagDraftModel senderContactBag = BuildContactBag(collaborationModel.Protocol.ProtocolContacts, DocSuiteWeb.Model.Entities.Commons.ComunicationType.Sender);
                            protocolDraftModel.Senders.Add(senderContactBag);
                            ContactBagDraftModel recipientContactBag = BuildContactBag(collaborationModel.Protocol.ProtocolContacts, DocSuiteWeb.Model.Entities.Commons.ComunicationType.Recipient);
                            protocolDraftModel.Recipients.Add(recipientContactBag);
                        }
                        if (collaborationModel.Protocol.ProtocolContactManuals != null && collaborationModel.Protocol.ProtocolContactManuals.Count > 0)
                        {
                            ContactBagDraftModel manualSenderContactBag = BuildManualContactBag(collaborationModel.Protocol.ProtocolContactManuals, DocSuiteWeb.Model.Entities.Commons.ComunicationType.Sender);
                            protocolDraftModel.Senders.Add(manualSenderContactBag);
                            ContactBagDraftModel manualRecipientContactBag = BuildManualContactBag(collaborationModel.Protocol.ProtocolContactManuals, DocSuiteWeb.Model.Entities.Commons.ComunicationType.Recipient);
                            protocolDraftModel.Recipients.Add(manualRecipientContactBag);
                        }
                        if (collaborationBuildModel.IdWorkflowActivity.HasValue)
                        {
                            WorkflowProperty toFascicleProperty = await _webApiClient.GetWorkflowActivityProperty(collaborationBuildModel.IdWorkflowActivity.Value, WorkflowPropertyHelper.DSW_ACTION_TO_FASCICLE);
                            if (toFascicleProperty != null && toFascicleProperty.ValueGuid.HasValue)
                            {
                                protocolDraftModel.WorkflowMetadatas.Add(new WorkflowMetadataDraftModel()
                                {
                                    Key = WorkflowPropertyHelper.DSW_ACTION_TO_FASCICLE,
                                    Value = toFascicleProperty.ValueGuid.ToString()
                                });
                            }
                        }
                        ProtocolDraft protocolDraft = new ProtocolDraft()
                        {
                            Collaboration = collaboration,
                            Data = SerializationHelper.SerializeToStringWithoutNamespace(protocolDraftModel),
                            Description = "Protocollo Precompilato da Collaborazione",
                            DraftType = 0,
                            IsActive = true
                        };
                        protocolDraft = await _webApiClient.PostEntityAsync(protocolDraft);
                        _logger.WriteDebug(new LogMessage($"ProtocolDraft {protocolDraft.UniqueId} has been created successfully."), LogCategories);
                        RetryPolicyEvaluation.Steps.Add(new StepModel()
                        {
                            Name = "ENTITY_ProtocolDraftCREATED",
                            LocalReference = JsonConvert.SerializeObject(protocolDraft, _serializerSettings)
                        });
                    }
                }
                #endregion

                #region [ Notify Collaboration ]
                if (collaborationBuildModel.IdWorkflowActivity.HasValue)
                {
                    _logger.WriteDebug(new LogMessage($"Notify IdCollaboration {collaboration.EntityId} for workflow activity {collaborationBuildModel.IdWorkflowActivity}."), LogCategories);
                    WorkflowNotify workflowNotify = new WorkflowNotify(collaborationBuildModel.IdWorkflowActivity.Value);
                    workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID, new WorkflowArgument
                    {
                        Name = WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID,
                        PropertyType = ArgumentType.Json,
                        ValueInt = collaboration.EntityId
                    });

                    await _webApiClient.PushWorkflowNotifyAsync(workflowNotify);
                }
                #endregion                

                //Attraverso le WebAPI comunicando col verbo POST inviare l'evento EventoCompleteCollaborationBuild                

                #region [ EventCompleteCollaborationBuild ]
                collaborationBuildModel.Collaboration.IdCollaboration = collaborationModel.IdCollaboration;
                IEventCompleteCollaborationBuild eventCompleteCollaborationBuild = new EventCompleteCollaborationBuild(Guid.NewGuid(), collaborationBuildModel.UniqueId,
                    command.TenantName, command.TenantId, command.TenantAOOId, command.Identity, collaborationBuildModel, null);
                if (!await _webApiClient.PushEventAsync(eventCompleteCollaborationBuild))
                {
                    _logger.WriteError(new LogMessage($"EventCompleteCollaborationBuild {collaboration.GetTitle()} has not been sended"), LogCategories);
                    throw new Exception("IEventCompleteCollaborationBuild not sended");
                }
                _logger.WriteInfo(new LogMessage($"EventCompleteCollaborationBuild {eventCompleteCollaborationBuild.Id} has been sended"), LogCategories);
                #endregion

                #region Detach documenti archivio workflow
                foreach (DocumentModel attachment in collaborationModel.CollaborationVersionings.Where(f => f.Document != null && f.Document.DocumentToStoreId.HasValue).Select(f => f.Document))
                {
                    _logger.WriteInfo(new LogMessage($"detaching workflow document {attachment.DocumentToStoreId} ..."), LogCategories);
                    RetryingPolicyAction(() => _biblosClient.Document.DocumentDetach(new Document() { IdDocument = attachment.DocumentToStoreId.Value }));
                }
                #endregion

            }
            catch (Exception ex)
            {
                RetryPolicyEvaluation.ReferenceModel = JsonConvert.SerializeObject(collaborationModel, _serializerSettings);

                _logger.WriteError(ex, LogCategories);
                throw new ServiceBusEvaluationException(RetryPolicyEvaluation);
            }
        }

        private T RetryingPolicyAction<T>(Func<T> func, int step = 1)
        {
            _logger.WriteDebug(new LogMessage($"RetryingPolicyAction : tentative {step}/{_retry_tentative} in progress..."), LogCategories);
            if (step >= _retry_tentative)
            {
                _logger.WriteError(new LogMessage("VecompSoftware.ServiceBus.Module.Entities.Listener.InsertCollaboration.RetryingPolicyAction: retry policy expired maximum tentatives"), LogCategories);
                throw new Exception("InsertCollaboration retry policy expired maximum tentatives");
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

        private ContactBagDraftModel BuildManualContactBag(ICollection<ProtocolContactManualModel> protocolContactManuals, DocSuiteWeb.Model.Entities.Commons.ComunicationType comunicationType)
        {
            ContactBagDraftModel manualContactBag = new ContactBagDraftModel();
            ContactDraftModel manualContact;
            foreach (ProtocolContactManualModel protocolManualContact in protocolContactManuals.Where(x => x.ComunicationType == comunicationType))
            {
                manualContact = new ContactDraftModel
                {
                    Description = protocolManualContact.Description,
                    Type = GENERIC_CONTACT_TYPE,
                    StandardMail = protocolManualContact.EMail,
                    BirthDate = protocolManualContact.BirthDate.HasValue ? protocolManualContact.BirthDate.ToString() : null,
                    CertifiedMail = protocolManualContact.CertifiedEmail,
                    FiscalCode = protocolManualContact.FiscalCode,
                    Telephone = protocolManualContact.TelephoneNumber,
                    Fax = protocolManualContact.FaxNumber,
                    Notes = protocolManualContact.Note,
                    Address = new ContactAddressDraftModel
                    {
                        Cap = protocolManualContact.ZipCode,
                        City = protocolManualContact.City,
                        Name = protocolManualContact.Address,
                        Number = protocolManualContact.CivicNumber,
                        Prov = protocolManualContact.CityCode,
                    }
                };
                manualContactBag.Contacts.Add(manualContact);
            }
            manualContactBag.SourceType = MANUAL_SOURCE_TYPE;
            return manualContactBag;
        }

        private ContactBagDraftModel BuildContactBag(ICollection<ProtocolContactModel> protocolContacts, DocSuiteWeb.Model.Entities.Commons.ComunicationType comunicationType)
        {
            ContactBagDraftModel contactBag = new ContactBagDraftModel();
            foreach (ProtocolContactModel protocolContact in protocolContacts.Where(x => x.ComunicationType == comunicationType))
            {
                contactBag.Contacts.Add(new ContactDraftModel
                {
                    Id = protocolContact.IdContact,
                    Type = GENERIC_CONTACT_TYPE,
                });
            }
            contactBag.SourceType = ADDRESSBOOK_SOURCE_TYPE;
            return contactBag;
        }
        #endregion
    }
}
