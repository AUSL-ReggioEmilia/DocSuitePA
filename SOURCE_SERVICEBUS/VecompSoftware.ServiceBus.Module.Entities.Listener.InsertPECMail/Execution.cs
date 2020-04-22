using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Core.Command.CQRS.Events.Models.PECMails;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.PECMails;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.BiblosDS.BiblosDS;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.Receiver.Base.Exceptions;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.PECMails;
using VecompSoftware.Services.Command.CQRS.Events.Models.PECMails;
using InvoiceStatus = VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceStatus;
using PECMailDirection = VecompSoftware.DocSuiteWeb.Entity.PECMails.PECMailDirection;
using PECMailPriority = VecompSoftware.DocSuiteWeb.Entity.PECMails.PECMailPriority;

namespace VecompSoftware.ServiceBus.Module.Entities.Listener.InsertPECMail
{
    public class Execution : IListenerExecution<ICommandBuildPECMail>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly BiblosClient _biblosClient;
        private readonly IWebAPIClient _webApiClient;
        protected static IEnumerable<LogCategory> _logCategories = null;
        //private readonly StampaConforme.StampaConformeClient _stampaConformeClient;
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
            //_stampaConformeClient = stampaConformeClient;
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

        public async Task ExecuteAsync(ICommandBuildPECMail command)
        {
            PECMail pecMail = new PECMail
            {
                UniqueId = Guid.NewGuid(),
                Direction = PECMailDirection.Outgoing,
                IsActive = DocSuiteWeb.Entity.PECMails.PECMailActiveType.Active,
            };

            PECMailBuildModel pecMailBuildModel = command.ContentType.ContentTypeValue;
            PECMailModel pecMailModel = pecMailBuildModel.PECMail;

            try
            {

                if (RetryPolicyEvaluation != null && !string.IsNullOrEmpty(RetryPolicyEvaluation.ReferenceModel))
                {
                    pecMailModel = JsonConvert.DeserializeObject<PECMailModel>(RetryPolicyEvaluation.ReferenceModel, _serializerSettings);
                }
                else
                {
                    RetryPolicyEvaluation = new EvaluationModel();
                }

                #region Creazione Documeti Allegati (Attachments OPTIONAL)

                Archive pecMailArchiveAttachment = _biblosArchives.Single(f => f.Name.Equals(pecMailModel.PECMailBox.Location.ProtocolArchive, StringComparison.InvariantCultureIgnoreCase));
                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "CREATE_ATTACHMENTS") && pecMailModel.Attachments.Any())
                {

                    List<BiblosDS.BiblosDS.Attribute> attachmentAttributes = _biblosClient.Document.GetAttributesDefinition(pecMailArchiveAttachment.Name);

                    //CREO CATENA IDENTIFICATIVA
                    Guid? attachmentChainId = pecMailModel.IDAttachments;
                    if (!attachmentChainId.HasValue)
                    {
                        attachmentChainId = _biblosClient.Document.CreateDocumentChain(pecMailArchiveAttachment.Name, new List<AttributeValue>());
                        pecMailModel.IDAttachments = attachmentChainId;
                    }
                    pecMail.IDAttachments = attachmentChainId.Value;

                    List<AttributeValue> attachmentAttributeValues;
                    foreach (DocumentModel attachment in pecMailModel.Attachments.Where(f => !f.DocumentId.HasValue))
                    {
                        attachmentAttributeValues = new List<AttributeValue>()
                        {
                            new AttributeValue()
                            {
                                Attribute = attachmentAttributes.Single(f => f.Name.Equals(AttributeHelper.AttributeName_Filename, StringComparison.InvariantCultureIgnoreCase)),
                                Value = attachment.FileName,
                            },
                            new AttributeValue()
                            {
                                Attribute = attachmentAttributes.Single(f => f.Name.Equals(AttributeHelper.AttributeName_Signature, StringComparison.InvariantCultureIgnoreCase)),
                                Value = attachment.Segnature,
                            },
                        };
                        attachment.ChainId = attachmentChainId;
                        _logger.WriteDebug(new LogMessage(string.Concat("biblos attachment archive name is ", pecMailArchiveAttachment.Name)), LogCategories);

                        //CREO IL DOCUMENTO
                        Document attachmentPECMailDocument = new Document
                        {
                            Archive = pecMailArchiveAttachment,
                            Content = new Content { Blob = attachment.ContentStream },
                            Name = attachment.FileName,
                            IsVisible = true,
                            AttributeValues = attachmentAttributeValues
                        };

                        //ASSOCIO IL DOCUMENTO ALLA SUA CATENA DI COMPETENZA
                        attachmentPECMailDocument = _biblosClient.Document.AddDocumentToChain(attachmentPECMailDocument, attachmentChainId, ContentFormat.Binary);

                        _logger.WriteInfo(new LogMessage(string.Concat("inserted document ", attachmentPECMailDocument.IdDocument.ToString(), " in archive ",
                            pecMailArchiveAttachment.IdArchive.ToString())), LogCategories);
                        attachment.DocumentId = attachmentPECMailDocument.IdDocument;

                    }

                    //Assegno gli allegati all'entita
                    foreach (DocumentModel attachment in pecMailModel.Attachments)
                    {
                        PECMailAttachment pecMailAttachment = new PECMailAttachment
                        {
                            IDDocument = attachment.DocumentId,
                            AttachmentName = attachment.FileName,
                            Parent = null,
                            IsMain = false,
                            Size = attachment.ContentStream.Length
                        };
                        pecMail.Attachments.Add(pecMailAttachment);
                    }

                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "CREATE_ATTACHMENTS",
                        LocalReference = JsonConvert.SerializeObject(pecMail, _serializerSettings)
                    });

                }
                else
                {
                    StepModel pecMailStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "CREATE_ATTACHMENTS");
                    pecMail = JsonConvert.DeserializeObject<PECMail>(pecMailStatus.LocalReference);
                }

                #endregion

                #region Creazione PECMail con stato attivo

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY"))
                {
                    pecMail.InvoiceStatus = pecMailModel.InvoiceStatus == DocSuiteWeb.Model.Entities.PECMails.InvoiceStatus.None ? default(InvoiceStatus?) : (InvoiceStatus)pecMailModel.InvoiceStatus;
                    pecMail.MailBody = pecMailModel.MailBody;
                    pecMail.MailPriority = pecMailModel.MailPriority.HasValue ? (PECMailPriority)pecMailModel.MailPriority.Value : PECMailPriority.Normal;
                    pecMail.MailRecipients = pecMailModel.MailRecipients;
                    pecMail.MailRecipientsCc = pecMailModel.MailRecipientsCc;
                    pecMail.MailSenders = pecMailModel.MailSenders;
                    pecMail.MailSubject = pecMailModel.MailSubject;
                    pecMail.PECType = null;
                    pecMail.MailType = "invio";
                    pecMail.SplittedFrom = 0;
                    pecMail.ProcessStatus = 0;
                    pecMail.MultipleType = DocSuiteWeb.Entity.PECMails.PECMailMultipleType.NoSplit;
                    pecMail.IsValidForInterop = false;
                    pecMail.Location = new Location() { EntityShortId = pecMailModel.PECMailBox.Location.IdLocation.Value };
                    pecMail.PECMailBox = new PECMailBox() { EntityShortId = (short)pecMailModel.PECMailBox.PECMailBoxId.Value };
                    pecMail.IsActive = DocSuiteWeb.Entity.PECMails.PECMailActiveType.Active;
                    WorkflowActionDocumentUnitLinkModel workflowActionDocumentUnitLinkModel = null;
                    Protocol protocol = null;
                    if ((workflowActionDocumentUnitLinkModel = pecMailBuildModel.WorkflowActions
                            .OfType<WorkflowActionDocumentUnitLinkModel>()
                            .FirstOrDefault(f => f.GetDestinationLink().Environment == (int)DocSuiteWeb.Entity.Commons.DSWEnvironmentType.PECMail)) != null &&
                       (protocol = await _webApiClient.GetProtocolAsync(workflowActionDocumentUnitLinkModel.GetReferenced().UniqueId)) != null)
                    {
                        pecMail.Year = protocol.Year;
                        pecMail.Number = protocol.Number;
                        pecMail.DocumentUnitType = DocSuiteWeb.Entity.Commons.DSWEnvironmentType.Protocol;
                        pecMail.RecordedInDocSuite = null;
                    }
                    //foreach (IWorkflowAction workflowAction in pecMailBuildModel.WorkflowActions)
                    //{
                    //    pecMail.WorkflowActions.Add(workflowAction);
                    //}
                    pecMail.WorkflowName = pecMailBuildModel.WorkflowName;
                    pecMail.IdWorkflowActivity = pecMailBuildModel.IdWorkflowActivity;
                    pecMail = await _webApiClient.PostEntityAsync(pecMail);
                }
                else
                {
                    StepModel pecMailStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY");
                    pecMail = JsonConvert.DeserializeObject<PECMail>(pecMailStatus.LocalReference);
                }
                #endregion

                #region [ EventCompletePECMailBuild ]
                foreach (DocumentModel item in pecMailModel.Attachments)
                {
                    item.ContentStream = null;
                }
                pecMailBuildModel.PECMail = pecMailModel;
                IEventCompletePECMailBuild eventCompletePECMailBuild = new EventCompletePECMailBuild(Guid.NewGuid(), pecMailBuildModel.UniqueId, command.TenantName, command.TenantId,
                    command.Identity, pecMailBuildModel, null);
                if (!await _webApiClient.PushEventAsync(eventCompletePECMailBuild))
                {
                    _logger.WriteError(new LogMessage($"EventCompletePECMailBuild {pecMail.EntityId} has not been sended"), LogCategories);
                    throw new Exception("IEventCompletePECMailBuild not sended");
                }
                _logger.WriteInfo(new LogMessage($"IEventCompletePECMailBuild {eventCompletePECMailBuild.Id} has been sended"), LogCategories);
                #endregion
            }
            catch (Exception ex)
            {
                RetryPolicyEvaluation.ReferenceModel = JsonConvert.SerializeObject(pecMailModel, _serializerSettings);

                _logger.WriteError(ex, LogCategories);
                throw new ServiceBusEvaluationException(RetryPolicyEvaluation);
            }
        }
    }
}
