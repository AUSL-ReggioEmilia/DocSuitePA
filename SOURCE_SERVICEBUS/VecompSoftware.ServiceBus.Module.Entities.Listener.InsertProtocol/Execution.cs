using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command.CQRS.Events.Models.Protocols;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.BiblosDS.BiblosDS;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.Receiver.Base.Exceptions;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.ServiceBus.WebAPI.Exceptions;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Protocols;
using VecompSoftware.Services.Command.CQRS.Events.Models.Protocols;
using ComunicationType = VecompSoftware.DocSuiteWeb.Entity.Protocols.ComunicationType;
using LanguageType = VecompSoftware.DocSuiteWeb.Entity.Commons.LanguageType;
using ProtocolRoleNoteType = VecompSoftware.DocSuiteWeb.Entity.Protocols.ProtocolRoleNoteType;
using ProtocolRoleStatus = VecompSoftware.DocSuiteWeb.Entity.Protocols.ProtocolRoleStatus;
using ProtocolTypology = VecompSoftware.DocSuiteWeb.Model.Entities.Protocols.ProtocolTypology;

namespace VecompSoftware.ServiceBus.Module.Entities.Listener.InsertProtocol
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class Execution : IListenerExecution<ICommandBuildProtocol>
    {
        #region [ Fields ]
        private const string VALIDATION_KEY_ISPROTOCOLCREATABLE = "IsProtocolCreatable";
        private readonly ILogger _logger;
        private readonly BiblosClient _biblosClient;
        private readonly IWebAPIClient _webApiClient;
        protected static IEnumerable<LogCategory> _logCategories = null;
        private readonly StampaConforme.StampaConformeClient _stampaConformeClient;
        private readonly List<Archive> _biblosArchives;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly short _signatureProtocolType;
        private readonly string _signatureProtocolString;
        private readonly string _signatureProtocolMainFormat;
        private readonly string _signatureProtocolAttachmentFormat;
        private readonly string _signatureProtocolAnnexedFormat;
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
                _signatureProtocolType = _webApiClient.GetParameterSignatureProtocolTypeAsync().Result;
                _signatureProtocolString = _webApiClient.GetParameterSignatureProtocolStringAsync().Result;
                _signatureProtocolMainFormat = _webApiClient.GetParameterSignatureProtocolMainFormatAsync().Result;
                _signatureProtocolAttachmentFormat = _webApiClient.GetParameterSignatureProtocolAttachmentFormatAsync().Result;
                _signatureProtocolAnnexedFormat = _webApiClient.GetParameterSignatureProtocolAnnexedFormatAsync().Result;
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

        public async Task ExecuteAsync(ICommandBuildProtocol command)
        {
            Protocol protocol = new Protocol();

            ProtocolBuildModel protocolBuildModel = command.ContentType.ContentTypeValue;
            ProtocolModel protocolModel = protocolBuildModel.Protocol;
            try
            {
                if (RetryPolicyEvaluation != null && !string.IsNullOrEmpty(RetryPolicyEvaluation.ReferenceModel))
                {
                    _logger.WriteDebug(new LogMessage("Load reference model from RetryPolicyEvaluation"), LogCategories);
                    protocolModel = JsonConvert.DeserializeObject<ProtocolModel>(RetryPolicyEvaluation.ReferenceModel, _serializerSettings);
                }
                else
                {
                    _logger.WriteDebug(new LogMessage("Generate new RetryPolicyEvaluation model"), LogCategories);
                    RetryPolicyEvaluation = new EvaluationModel();
                }

                #region Creazione Protocollo in stato non attivo

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY"))
                {
                    _logger.WriteDebug(new LogMessage($"Preparing protocol entity {protocolModel.UniqueId}"), LogCategories);
                    protocol.UniqueId = protocolModel.UniqueId;
                    protocol.AdvancedProtocol = new AdvancedProtocol
                    {
                        IdentificationSdi = protocolModel.SDIIdentification
                    };
                    protocol.Category = new Category() { EntityShortId = (short)protocolModel.Category.IdCategory.Value };
                    protocol.Container = await _webApiClient.GetContainerAsync(protocolModel.Container.IdContainer.Value);
                    protocol.Location = new Location() { EntityShortId = protocolModel.Container.ProtLocation.IdLocation.Value };
                    protocol.AttachLocation = new Location() { EntityShortId = (protocolModel.Container.ProtocolAttachmentLocation ?? protocolModel.Container.ProtLocation).IdLocation.Value };
                    protocol.ProtocolType = new ProtocolType() { EntityShortId = protocolModel.ProtocolType.EntityShortId };
                    protocol.Object = protocolModel.Object;
                    protocol.DocumentCode = protocolModel.MainDocument.FileName;
                    foreach (ProtocolContactModel model in protocolModel.Contacts)
                    {
                        protocol.ProtocolContacts.Add(new ProtocolContact()
                        {
                            EntityId = model.IdContact,
                            ComunicationType = model.ComunicationType == DocSuiteWeb.Model.Entities.Commons.ComunicationType.Sender ? ComunicationType.Sender : ComunicationType.Recipient,
                            Contact = new Contact() { EntityId = model.IdContact }
                        });
                    }
                    ProtocolContactManual protocolContactManual;
                    foreach (ProtocolContactManualModel model in protocolModel.ContactManuals)
                    {
                        protocolContactManual = new ProtocolContactManual()
                        {
                            Address = model.Address,
                            BirthDate = model.BirthDate,
                            BirthPlace = model.BirthPlace,
                            CertifydMail = model.CertifiedEmail,
                            City = model.City,
                            CityCode = model.CityCode,
                            CivicNumber = model.CivicNumber,
                            Code = model.Code,
                            Description = model.Description,
                            EMailAddress = model.EMail,
                            FaxNumber = model.FaxNumber,
                            FiscalCode = model.FiscalCode,
                            Language = model.Language.HasValue ? (LanguageType)model.Language.Value : default(LanguageType),
                            Nationality = model.Nationality,
                            Note = model.Note,
                            SDIIdentification = model.SDIIdentification,
                            ZipCode = model.ZipCode,
                            TelephoneNumber = model.TelephoneNumber,
                        };
                        switch (model.BaseContactType)
                        {
                            case DocSuiteWeb.Model.Entities.Commons.ContactType.Administration:
                                {
                                    protocolContactManual.IdContactType = DocSuiteWeb.Entity.Commons.ContactType.Administration;
                                    break;
                                }
                            case DocSuiteWeb.Model.Entities.Commons.ContactType.AOO:
                                {
                                    protocolContactManual.IdContactType = DocSuiteWeb.Entity.Commons.ContactType.AOO;
                                    break;
                                }
                            case DocSuiteWeb.Model.Entities.Commons.ContactType.AO:
                                {
                                    protocolContactManual.IdContactType = DocSuiteWeb.Entity.Commons.ContactType.AO;
                                    break;
                                }
                            case DocSuiteWeb.Model.Entities.Commons.ContactType.Role:
                                {
                                    protocolContactManual.IdContactType = DocSuiteWeb.Entity.Commons.ContactType.Role;
                                    break;
                                }
                            case DocSuiteWeb.Model.Entities.Commons.ContactType.Group:
                                {
                                    protocolContactManual.IdContactType = DocSuiteWeb.Entity.Commons.ContactType.Group;
                                    break;
                                }
                            case DocSuiteWeb.Model.Entities.Commons.ContactType.Sector:
                                {
                                    protocolContactManual.IdContactType = DocSuiteWeb.Entity.Commons.ContactType.Sector;
                                    break;
                                }
                            case DocSuiteWeb.Model.Entities.Commons.ContactType.Citizen:
                                {
                                    protocolContactManual.IdContactType = DocSuiteWeb.Entity.Commons.ContactType.Citizen;
                                    break;
                                }
                            case DocSuiteWeb.Model.Entities.Commons.ContactType.IPA:
                                {
                                    protocolContactManual.IdContactType = DocSuiteWeb.Entity.Commons.ContactType.IPA;
                                    break;
                                }
                            case DocSuiteWeb.Model.Entities.Commons.ContactType.CitizenManual:
                                {
                                    protocolContactManual.IdContactType = DocSuiteWeb.Entity.Commons.ContactType.CitizenManual;
                                    break;
                                }
                            case DocSuiteWeb.Model.Entities.Commons.ContactType.AOOManual:
                                {
                                    protocolContactManual.IdContactType = DocSuiteWeb.Entity.Commons.ContactType.AOOManual;
                                    break;
                                }
                        }
                        switch (model.ComunicationType)
                        {
                            case DocSuiteWeb.Model.Entities.Commons.ComunicationType.Sender:
                                {
                                    protocolContactManual.ComunicationType = ComunicationType.Sender;
                                    break;
                                }
                            case DocSuiteWeb.Model.Entities.Commons.ComunicationType.Recipient:
                                {
                                    protocolContactManual.ComunicationType = ComunicationType.Recipient;
                                    break;
                                }
                        }
                        switch (model.ContactType)
                        {
                            case DocSuiteWeb.Model.Entities.Protocols.ProtocolContactType.CarbonCopy:
                                {
                                    protocolContactManual.Type = DocSuiteWeb.Entity.Protocols.ProtocolContactType.CarbonCopy;
                                    break;
                                }
                        }
                        protocol.ProtocolContactManuals.Add(protocolContactManual);
                    }
                    ProtocolRole protocolRole = null;
                    foreach (ProtocolRoleModel model in protocolModel.Roles)
                    {
                        protocolRole = new ProtocolRole()
                        {
                            DistributionType = null,
                            Note = model.Note,
                            NoteType = model.NoteType.HasValue ? (ProtocolRoleNoteType)model.NoteType.Value : default(ProtocolRoleNoteType),
                            Rights = null,
                            Role = new Role() { EntityShortId = model.Role.IdRole.Value },
                            Type = null,
                            Status = (ProtocolRoleStatus)model.Status
                        };
                        if (model.DistributionType.HasValue)
                        {
                            switch (model.DistributionType.Value)
                            {
                                case DocSuiteWeb.Model.Entities.Protocols.ProtocolRoleDistributionType.Explicit:
                                    {
                                        protocolRole.DistributionType = DocSuiteWeb.Entity.Protocols.ProtocolRoleDistributionType.Explicit;
                                        break;
                                    };
                                case DocSuiteWeb.Model.Entities.Protocols.ProtocolRoleDistributionType.Implicit:
                                    {
                                        protocolRole.DistributionType = DocSuiteWeb.Entity.Protocols.ProtocolRoleDistributionType.Implicit;
                                        break;
                                    };
                            }
                        }
                        if (model.Type.HasValue)
                        {
                            switch (model.Type.Value)
                            {
                                case DocSuiteWeb.Model.Entities.Protocols.ProtocolRoleType.CarbonCopy:
                                    {
                                        protocolRole.Type = DocSuiteWeb.Entity.Protocols.ProtocolRoleType.CarbonCopy;
                                        break;
                                    };
                                case DocSuiteWeb.Model.Entities.Protocols.ProtocolRoleType.Privacy:
                                    {
                                        protocolRole.Type = DocSuiteWeb.Entity.Protocols.ProtocolRoleType.Privacy;
                                        break;
                                    };
                            }
                        }
                        protocol.ProtocolRoles.Add(protocolRole);
                    }

                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "ENTITY",
                        LocalReference = JsonConvert.SerializeObject(protocol, _serializerSettings)
                    });
                    _logger.WriteDebug(new LogMessage("Set ENTITY RetryPolicyEvaluation"), LogCategories);
                }
                else
                {
                    _logger.WriteDebug(new LogMessage("Load protocol entity from RetryPolicyEvaluation ENTITY"), LogCategories);
                    StepModel protocolStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY");
                    protocol = JsonConvert.DeserializeObject<Protocol>(protocolStatus.LocalReference);
                }

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY_CREATED"))
                {
                    try
                    {
                        protocol = await _webApiClient.PostEntityAsync(protocol, InsertActionType.CreateProtocol.ToString());
                    }
                    catch (ValidationException ex)
                    {
                        if (!ex.ValidationErrors.Any(f => f.Key == VALIDATION_KEY_ISPROTOCOLCREATABLE))
                        {
                            throw;
                        }
                        Protocol currentProtocol = await _webApiClient.GetProtocolAsync(protocol.UniqueId);
                        if (currentProtocol.IdStatus != -5)
                        {
                            _logger.WriteWarning(new LogMessage($"Protocol {protocol.UniqueId} cannot be recevery with IdStatus {currentProtocol.IdStatus}."), LogCategories);
                            throw;
                        }
                        _logger.WriteDebug(new LogMessage($"Protocol {protocol.UniqueId} automatically recovery. See limitation of BrokeredMessage properties bag cannot exceed 32 kilobytes."), LogCategories);
                        protocol.Year = currentProtocol.Year;
                        protocol.Number = currentProtocol.Number;
                        protocol.RegistrationDate = currentProtocol.RegistrationDate;
                        protocol.RegistrationUser = currentProtocol.RegistrationUser;
                        protocol.Timestamp = currentProtocol.Timestamp;
                    }
                    _logger.WriteDebug(new LogMessage($"Protocol {protocol.GetTitle()}/{protocol.UniqueId}-{protocol.Object} has been created but not already activated"), LogCategories);
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "ENTITY_CREATED",
                        LocalReference = JsonConvert.SerializeObject(protocol, _serializerSettings)
                    });
                    _logger.WriteDebug(new LogMessage("Set ENTITY_CREATED RetryPolicyEvaluation"), LogCategories);
                }
                else
                {
                    _logger.WriteDebug(new LogMessage("Load protocol entity from RetryPolicyEvaluation ENTITY_CREATED"), LogCategories);
                    StepModel protocolStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY_CREATED");
                    protocol = JsonConvert.DeserializeObject<Protocol>(protocolStatus.LocalReference);
                }

                SegnatureModel formatterModel = new SegnatureModel
                {
                    AttachmentsCount = protocolModel.Attachments.Count(),
                    ContainerId = protocol.Container.EntityShortId,
                    ContainerName = protocol.Container.Name,
                    ContainerNote = protocol.Container.Note,
                    CorporateAcronym = _corporateAcronym,
                    CorporateName = _corporateName,
                    Number = protocol.Number,
                    Typology = (ProtocolTypology)protocol.ProtocolType.EntityShortId,
                    RegistrationDate = protocol.RegistrationDate
                };
                formatterModel.RoleServiceCodes.AddRange(protocol.ProtocolRoles.Where(f => !string.IsNullOrEmpty(f.Role.ServiceCode)).Select(f => f.Role.ServiceCode));
                formatterModel.Year = protocol.Year;
                #endregion

                //Attraverso il layer di BiblosDS salvare lo il file del documento con relativa segnatura(metadato)

                #region Creazione Documeto Principale REQUIRED 
                _logger.WriteDebug(new LogMessage($"Looking _biblosArchives {protocolModel.Container.ProtLocation.ProtocolArchive} ..."), LogCategories);
                Archive protocolMainArchive = _biblosArchives.Single(f => f.Name.Equals(protocolModel.Container.ProtLocation.ProtocolArchive, StringComparison.InvariantCultureIgnoreCase));

                List<BiblosDS.BiblosDS.Attribute> mainAttributes = _biblosClient.Document.GetAttributesDefinition(protocolMainArchive.Name);
                formatterModel.DocumentType = SegnatureDocumentType.Main;
                protocolModel.MainDocument.Segnature = GenerateSegnature(formatterModel);
                //CAPIRE QUALI SONO GLI ATTRIBUTI
                _logger.WriteDebug(new LogMessage($"Looking attribute {AttributeHelper.AttributeName_Filename} ... {mainAttributes.Any(f => f.Name.Equals(AttributeHelper.AttributeName_Filename, StringComparison.InvariantCultureIgnoreCase))}"), LogCategories);
                _logger.WriteDebug(new LogMessage($"Looking attribute {AttributeHelper.AttributeName_Signature} ...{mainAttributes.Any(f => f.Name.Equals(AttributeHelper.AttributeName_Signature, StringComparison.InvariantCultureIgnoreCase))}"), LogCategories);
                _logger.WriteDebug(new LogMessage($"Looking attribute {AttributeHelper.AttributeName_PrivacyLevel} ...{mainAttributes.Any(f => f.Name.Equals(AttributeHelper.AttributeName_PrivacyLevel, StringComparison.InvariantCultureIgnoreCase))}"), LogCategories);
                List<AttributeValue> mainDocumentAttributeValues = new List<AttributeValue>()
                    {
                        new AttributeValue()
                        {
                            Attribute = mainAttributes.Single(f => f.Name.Equals(AttributeHelper.AttributeName_Filename, StringComparison.InvariantCultureIgnoreCase)),
                            Value = protocolModel.MainDocument.FileName,
                        },
                        new AttributeValue()
                        {
                            Attribute = mainAttributes.Single(f => f.Name.Equals(AttributeHelper.AttributeName_Signature, StringComparison.InvariantCultureIgnoreCase)),
                            Value = protocolModel.MainDocument.Segnature,
                        },
                    };
                if (mainAttributes.Any(f => f.Name.Equals(AttributeHelper.AttributeName_PrivacyLevel, StringComparison.InvariantCultureIgnoreCase)))
                {
                    mainDocumentAttributeValues.Add(new AttributeValue()
                    {
                        Attribute = mainAttributes.Single(f => f.Name.Equals(AttributeHelper.AttributeName_PrivacyLevel, StringComparison.InvariantCultureIgnoreCase)),
                        Value = 0,
                    });
                }
                //CREO CATENA IDENTIFICATIVA
                Guid mainChainId = protocolModel.MainDocument.ChainId ?? Guid.Empty; //se c'e' lo prende dal giro precedente in quanto protocolModel e' preso dal RetryPolicyEvaluation all inizio

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "MAIN_CHAIN_ID"))
                {
                    protocolModel.MainDocument.ChainId = _biblosClient.Document.CreateDocumentChain(protocolMainArchive.Name, mainDocumentAttributeValues);
                    mainChainId = protocolModel.MainDocument.ChainId.Value;
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "MAIN_CHAIN_ID",
                    });
                    _logger.WriteDebug(new LogMessage("Set MAIN_CHAIN_ID RetryPolicyEvaluation"), LogCategories);
                }

                _logger.WriteDebug(new LogMessage(string.Concat("biblos main chain ", mainChainId, " in archive name ", protocolMainArchive.Name)), LogCategories);

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "CREATE_MAIN_DOCUMENT"))
                {
                    //CREO IL DOCUMENTO
                    Document mainProtocolDocument = new Document
                    {
                        Archive = protocolMainArchive,
                        Content = new Content { Blob = protocolModel.MainDocument.ContentStream },
                        Name = protocolModel.MainDocument.FileName,
                        IsVisible = true,
                        AttributeValues = mainDocumentAttributeValues
                    };

                    //ASSOCIO IL DOCUMENTO ALLA SUA CATENA DI COMPETENZA
                    mainProtocolDocument = _biblosClient.Document.AddDocumentToChain(mainProtocolDocument, mainChainId, ContentFormat.Binary);
                    protocolModel.MainDocument.DocumentId = mainProtocolDocument.DocumentParent.IdDocument;
                    protocol.IdDocument = mainProtocolDocument.DocumentParent.IdBiblos.Value;
                    protocol.ProtocolLogs.Add(new ProtocolLog() { LogType = "PM", LogDescription = $"Documento (Add): {protocolModel.MainDocument.FileName}" });
                    _logger.WriteInfo(new LogMessage($"inserted document {mainProtocolDocument.IdDocument.ToString()} in archive {protocolMainArchive.IdArchive}"), LogCategories);

                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "CREATE_MAIN_DOCUMENT",
                        LocalReference = JsonConvert.SerializeObject(protocol, _serializerSettings)
                    });
                    _logger.WriteDebug(new LogMessage("Set CREATE_MAIN_DOCUMENT RetryPolicyEvaluation"), LogCategories);
                }
                else
                {
                    StepModel protocolStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "CREATE_MAIN_DOCUMENT");
                    protocol = JsonConvert.DeserializeObject<Protocol>(protocolStatus.LocalReference);
                    _logger.WriteDebug(new LogMessage("Load protocol entity from RetryPolicyEvaluation CREATE_MAIN_DOCUMENT"), LogCategories);
                }

                #endregion

                //Attraverso il layer di BiblosDS salvare tutti gli allegati con relativa segnatura (metadato)

                #region Creazione Documenti Allegati (Attachments OPTIONAL)

                string attachName = protocolModel.Container.ProtocolAttachmentLocation != null ? protocolModel.Container.ProtocolAttachmentLocation.ProtocolArchive : protocolModel.Container.ProtLocation.ProtocolArchive;
                Archive protocolAttachmentAndAnnexedArchive = _biblosArchives.Single(f => f.Name.Equals(attachName, StringComparison.InvariantCultureIgnoreCase));

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "CREATE_ATTACHMENTS") && protocolModel.Attachments.Any())
                {
                    List<BiblosDS.BiblosDS.Attribute> attachmentAttributes = _biblosClient.Document.GetAttributesDefinition(protocolAttachmentAndAnnexedArchive.Name);

                    //CREO CATENA IDENTIFICATIVA
                    Guid? attachmentChainId = protocolModel.IdChainAttachment;
                    if (!attachmentChainId.HasValue)
                    {
                        //cerchi attachmentChainId dagli Attachments/Annexed
                        attachmentChainId = _biblosClient.Document.CreateDocumentChain(protocolAttachmentAndAnnexedArchive.Name, new List<AttributeValue>());
                        protocolModel.IdChainAttachment = attachmentChainId;
                    }
                    List<AttributeValue> attachmentAttributeValues;
                    int pos = protocolModel.Attachments.Count(f => f.DocumentId.HasValue);
                    _logger.WriteDebug(new LogMessage($"Looking attachment attribute {AttributeHelper.AttributeName_Filename} ... {attachmentAttributes.Any(f => f.Name.Equals(AttributeHelper.AttributeName_Filename, StringComparison.InvariantCultureIgnoreCase))}"), LogCategories);
                    _logger.WriteDebug(new LogMessage($"Looking attachment attribute {AttributeHelper.AttributeName_Signature} ...{attachmentAttributes.Any(f => f.Name.Equals(AttributeHelper.AttributeName_Signature, StringComparison.InvariantCultureIgnoreCase))}"), LogCategories);
                    _logger.WriteDebug(new LogMessage($"Looking attachment attribute {AttributeHelper.AttributeName_PrivacyLevel} ...{attachmentAttributes.Any(f => f.Name.Equals(AttributeHelper.AttributeName_PrivacyLevel, StringComparison.InvariantCultureIgnoreCase))}"), LogCategories);

                    foreach (DocumentModel attachment in protocolModel.Attachments.Where(f => !f.DocumentId.HasValue))
                    {
                        formatterModel.DocumentNumber = pos++;
                        formatterModel.DocumentType = SegnatureDocumentType.Attachment;
                        attachment.Segnature = GenerateSegnature(formatterModel);
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
                            }
                        };
                        if (attachmentAttributes.Any(f => f.Name.Equals(AttributeHelper.AttributeName_PrivacyLevel, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            attachmentAttributeValues.Add(new AttributeValue()
                            {
                                Attribute = attachmentAttributes.Single(f => f.Name.Equals(AttributeHelper.AttributeName_PrivacyLevel, StringComparison.InvariantCultureIgnoreCase)),
                                Value = 0,
                            });
                        }
                        attachment.ChainId = attachmentChainId;
                        _logger.WriteDebug(new LogMessage(string.Concat("biblos attachment archive name is ", protocolAttachmentAndAnnexedArchive.Name)), LogCategories);

                        //CREO IL DOCUMENTO
                        Document attachmentProtocolDocument = new Document
                        {
                            Archive = protocolAttachmentAndAnnexedArchive,
                            Content = new Content { Blob = attachment.ContentStream },
                            Name = attachment.FileName,
                            IsVisible = true,
                            AttributeValues = attachmentAttributeValues
                        };

                        //ASSOCIO IL DOCUMENTO ALLA SUA CATENA DI COMPETENZA
                        attachmentProtocolDocument = _biblosClient.Document.AddDocumentToChain(attachmentProtocolDocument, attachmentChainId, ContentFormat.Binary);
                        attachment.DocumentId = attachmentProtocolDocument.IdDocument;
                        _logger.WriteInfo(new LogMessage(string.Concat("inserted document ", attachmentProtocolDocument.IdDocument.ToString(), " in archive ", protocolAttachmentAndAnnexedArchive.IdArchive.ToString())), LogCategories);

                        //Se fa questo ad ogni iterazione lo sovrascriva ma non crea problemi in quanto DocumentParent è sempre lo stesso
                        protocol.IdAttachments = attachmentProtocolDocument.DocumentParent.IdBiblos.Value;
                        protocol.ProtocolLogs.Add(new ProtocolLog() { LogType = "PM", LogDescription = $"Allegato (Add): {attachment.FileName}" });
                    }
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "CREATE_ATTACHMENTS",
                        LocalReference = JsonConvert.SerializeObject(protocol, _serializerSettings)
                    });
                    _logger.WriteDebug(new LogMessage("Set CREATE_ATTACHMENTS RetryPolicyEvaluation"), LogCategories);
                }
                else
                {
                    StepModel protocolStatus = RetryPolicyEvaluation.Steps.FirstOrDefault(f => f.Name == "CREATE_ATTACHMENTS");
                    _logger.WriteDebug(new LogMessage("RetryPolicyEvaluation CREATE_ATTACHMENTS"), LogCategories);
                    if (protocolStatus != null)
                    {
                        protocol = JsonConvert.DeserializeObject<Protocol>(protocolStatus.LocalReference);
                        _logger.WriteDebug(new LogMessage("Load protocol entity from RetryPolicyEvaluation CREATE_ATTACHMENTS"), LogCategories);
                    }
                }

                #endregion

                //Attraverso il layer di BiblosDS salvare tutti gli annessi con relativa segnatura (metadato)

                #region Creazione Documeti Annessi (Annexed OPTIONAL)

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "CREATE_ANNEXED") && protocolModel.Annexes.Any())
                {
                    List<BiblosDS.BiblosDS.Attribute> annexedAttributes = _biblosClient.Document.GetAttributesDefinition(protocolAttachmentAndAnnexedArchive.Name);

                    //CREO CATENA IDENTIFICATIVA
                    Guid? annexedChainId = protocolModel.IdChainAnnexed;
                    if (!annexedChainId.HasValue)
                    {
                        annexedChainId = _biblosClient.Document.CreateDocumentChain(protocolAttachmentAndAnnexedArchive.Name, new List<AttributeValue>());
                        protocolModel.IdChainAnnexed = annexedChainId;
                    }

                    int pos = protocolModel.Annexes.Count(f => f.DocumentId.HasValue);
                    List<AttributeValue> annexedAttributeValues;
                    _logger.WriteDebug(new LogMessage($"Looking annexed attribute {AttributeHelper.AttributeName_Filename} ... {annexedAttributes.Any(f => f.Name.Equals(AttributeHelper.AttributeName_Filename, StringComparison.InvariantCultureIgnoreCase))}"), LogCategories);
                    _logger.WriteDebug(new LogMessage($"Looking annexed attribute {AttributeHelper.AttributeName_Signature} ...{annexedAttributes.Any(f => f.Name.Equals(AttributeHelper.AttributeName_Signature, StringComparison.InvariantCultureIgnoreCase))}"), LogCategories);
                    _logger.WriteDebug(new LogMessage($"Looking annexed attribute {AttributeHelper.AttributeName_PrivacyLevel} ...{annexedAttributes.Any(f => f.Name.Equals(AttributeHelper.AttributeName_PrivacyLevel, StringComparison.InvariantCultureIgnoreCase))}"), LogCategories);

                    foreach (DocumentModel annexedItem in protocolModel.Annexes.Where(f => !f.DocumentId.HasValue))
                    {
                        formatterModel.DocumentNumber = pos++;
                        formatterModel.DocumentType = SegnatureDocumentType.Annexed;
                        annexedItem.Segnature = GenerateSegnature(formatterModel);
                        annexedAttributeValues = new List<AttributeValue>()
                        {
                            new AttributeValue()
                            {
                                Attribute = annexedAttributes.Single(f => f.Name.Equals(AttributeHelper.AttributeName_Filename, StringComparison.InvariantCultureIgnoreCase)),
                                Value = annexedItem.FileName,
                            },
                            new AttributeValue()
                            {
                                Attribute = annexedAttributes.Single(f => f.Name.Equals(AttributeHelper.AttributeName_Signature, StringComparison.InvariantCultureIgnoreCase)),
                                Value = annexedItem.Segnature,
                            }
                        };
                        if (annexedAttributes.Any(f => f.Name.Equals(AttributeHelper.AttributeName_PrivacyLevel, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            annexedAttributeValues.Add(new AttributeValue()
                            {
                                Attribute = annexedAttributes.Single(f => f.Name.Equals(AttributeHelper.AttributeName_PrivacyLevel, StringComparison.InvariantCultureIgnoreCase)),
                                Value = 0,
                            });
                        }
                        annexedItem.ChainId = annexedChainId;
                        _logger.WriteDebug(new LogMessage(string.Concat("biblos anexed archive name is ", protocolAttachmentAndAnnexedArchive.Name)), LogCategories);

                        //CREO IL DOCUMENTO
                        Document annexedProtocolDocument = new Document
                        {
                            Archive = protocolAttachmentAndAnnexedArchive,
                            Content = new Content { Blob = annexedItem.ContentStream },
                            Name = annexedItem.FileName,
                            IsVisible = true,
                            AttributeValues = annexedAttributeValues
                        };

                        //ASSOCIO IL DOCUMENTO ALLA SUA CATENA DI COMPETENZA
                        annexedProtocolDocument = _biblosClient.Document.AddDocumentToChain(annexedProtocolDocument, annexedChainId, ContentFormat.Binary);

                        _logger.WriteInfo(new LogMessage(string.Concat("inserted document ", annexedProtocolDocument.IdDocument.ToString(), " in archive ",
                            protocolAttachmentAndAnnexedArchive.IdArchive.ToString())), LogCategories);
                        annexedItem.DocumentId = annexedProtocolDocument.IdDocument;
                        protocol.ProtocolLogs.Add(new ProtocolLog() { LogType = "PM", LogDescription = $"Annesso (Add): {annexedItem.FileName}" });
                    }
                    protocol.IdAnnexed = annexedChainId;
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "CREATE_ANNEXED",
                        LocalReference = JsonConvert.SerializeObject(protocol, _serializerSettings)
                    });
                    _logger.WriteDebug(new LogMessage("Set CREATE_ANNEXED RetryPolicyEvaluation"), LogCategories);
                }
                else
                {
                    StepModel protocolStatus = RetryPolicyEvaluation.Steps.FirstOrDefault(f => f.Name == "CREATE_ANNEXED");
                    if (protocolStatus != null)
                    {
                        protocol = JsonConvert.DeserializeObject<Protocol>(protocolStatus.LocalReference);
                        _logger.WriteDebug(new LogMessage("Load protocol entity from RetryPolicyEvaluation CREATE_ANNEXED"), LogCategories);
                    }
                    _logger.WriteDebug(new LogMessage("RetryPolicyEvaluation CREATE_ANNEXED"), LogCategories);
                }

                #endregion

                //Attraverso le WebAPI comunicando col verbo PUT col controller Rest Protocol usando il valore UpdateActionType "ActivateProtocol",, aggiornare l'entità di protocollo coi relativi identificativi di Biblos delle colonne idDocument,idAttachments,idAnnexed

                #region Attivazione del protocollo

                //Non so quanto abbia senso farlo anche qui essendo l'ultimo stato ma per sviluppi futuri potrebbe essere utile
                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY_UPDATED"))
                {
                    protocol.WorkflowName = protocolBuildModel.WorkflowName;
                    protocol.IdWorkflowActivity = protocolBuildModel.IdWorkflowActivity;
                    foreach (IWorkflowAction workflowAction in protocolBuildModel.WorkflowActions)
                    {
                        protocol.WorkflowActions.Add(workflowAction);
                    }

                    protocol = await _webApiClient.PutEntityAsync(protocol, UpdateActionType.ActivateProtocol.ToString());
                    _logger.WriteInfo(new LogMessage($"protocol {protocol.GetTitle()}/{protocol.UniqueId}-{protocol.Object} has been activated"), LogCategories);
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "ENTITY_UPDATED",
                        LocalReference = JsonConvert.SerializeObject(protocol, _serializerSettings)
                    });
                    _logger.WriteDebug(new LogMessage("Set ENTITY_UPDATED RetryPolicyEvaluation"), LogCategories);
                }
                else
                {
                    StepModel protocolStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY_UPDATED");
                    protocol = JsonConvert.DeserializeObject<Protocol>(protocolStatus.LocalReference);
                    _logger.WriteDebug(new LogMessage("Load protocol entity from RetryPolicyEvaluation ENTITY_UPDATED"), LogCategories);
                }

                #endregion

                #region [ EventCompleteProtocolBuild ]
                protocolModel.Year = protocol.Year;
                protocolModel.Number = protocol.Number;
                protocolModel.RegistrationDate = protocol.RegistrationDate;
                protocolBuildModel.Protocol = protocolModel;
                protocolModel.MainDocument.ContentStream = null;
                foreach (DocumentModel item in protocolModel.Attachments)
                {
                    item.ContentStream = null;
                }
                foreach (DocumentModel item in protocolModel.Annexes)
                {
                    item.ContentStream = null;
                }
                IEventCompleteProtocolBuild eventCompleteProtocolBuild = new EventCompleteProtocolBuild(Guid.NewGuid(), protocolBuildModel.UniqueId, command.TenantName, command.TenantId,
                    command.Identity, protocolBuildModel, null);
                if (!await _webApiClient.PushEventAsync(eventCompleteProtocolBuild))
                {
                    _logger.WriteError(new LogMessage($"EventCompleteProtocolBuild {protocol.GetTitle()} has not been sended"), LogCategories);
                    throw new Exception("IEventCompleteProtocolBuild not sended");
                }
                _logger.WriteInfo(new LogMessage($"EventCompleteProtocolBuild {eventCompleteProtocolBuild.Id} has been sended"), LogCategories);
                #endregion
            }
            catch (Exception ex)
            {
                RetryPolicyEvaluation.ReferenceModel = JsonConvert.SerializeObject(protocolModel, _serializerSettings);

                _logger.WriteError(ex, LogCategories);
                throw new ServiceBusEvaluationException(RetryPolicyEvaluation);
            }
        }

        private string GenerateSegnature(SegnatureModel formatterModel)
        {
            StringBuilder signature = new StringBuilder();
            signature.AppendFormat("{0} {1}/{2:0000000} del {3:dd/MM/yyyy}", _signatureProtocolString, formatterModel.Year, formatterModel.Number, formatterModel.RegistrationDate.ToLocalTime().Date);
            switch (_signatureProtocolType)
            {
                case 0:
                case 1:
                    {
                        signature.Insert(0, " ");
                        signature.Insert(0, formatterModel.CorporateAcronym);
                        switch (formatterModel.DocumentType)
                        {
                            case SegnatureDocumentType.Attachment:
                                {
                                    signature.Append(" (Allegato)");
                                    break;
                                }
                            case SegnatureDocumentType.Annexed:
                                {
                                    signature.Append(" (Annesso)");
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                case 2:
                    {
                        signature.Insert(0, " ");
                        signature.Insert(0, formatterModel.CorporateAcronym);
                        signature.AppendFormat(" {0}", formatterModel.ContainerName);
                        break;
                    }
                case 3:
                    {
                        signature.Insert(0, " ");
                        signature.Insert(0, formatterModel.CorporateAcronym);
                        switch (formatterModel.Typology)
                        {
                            case ProtocolTypology.Inbound:
                                {
                                    signature.Append(" Ingresso");
                                    break;
                                }
                            case ProtocolTypology.Internal:
                                {
                                    signature.Append(" Tra uffici");
                                    break;
                                }
                            case ProtocolTypology.Outgoing:
                                {
                                    signature.Append(" Uscita");
                                    break;
                                }
                            default:
                                break;
                        }
                        signature.AppendFormat(" {0}", formatterModel.ContainerName);
                        break;
                    }
                case 4:
                    {
                        signature.Insert(0, " ");
                        signature.Insert(0, formatterModel.CorporateAcronym);
                        signature.AppendFormat(" {0}", formatterModel.ContainerName);
                        signature.AppendFormat(" [{0}]", string.Join("-", formatterModel.RoleServiceCodes.Where(f => !string.IsNullOrEmpty(f)).Select(f => f.ToUpper())));
                        break;
                    }
                case 5:
                    {
                        string format = string.Empty;
                        switch (formatterModel.DocumentType)
                        {
                            case SegnatureDocumentType.Main:
                                {
                                    format = _signatureProtocolMainFormat;
                                    break;
                                }
                            case SegnatureDocumentType.Attachment:
                                {
                                    format = _signatureProtocolAttachmentFormat;
                                    break;
                                }
                            case SegnatureDocumentType.Annexed:
                                {
                                    format = _signatureProtocolAnnexedFormat;
                                    break;
                                }
                            default:
                                break;
                        }
                        signature = new StringBuilder();
                        signature.AppendFormat(new SegnatureFormatter(), format, formatterModel, formatterModel, formatterModel);
                        break;
                    }
                default:
                    break;
            }

            return signature.ToString();
        }

        #endregion
    }
}
