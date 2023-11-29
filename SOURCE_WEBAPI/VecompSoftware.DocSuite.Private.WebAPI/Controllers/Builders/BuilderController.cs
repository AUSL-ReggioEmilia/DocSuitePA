using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.DocSuite.Document;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Finder.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Mapper.Model.UDS;
using VecompSoftware.DocSuiteWeb.Model.Documents;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Validations;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Service.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Validation;
using WebApiHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Model.Entities.Messages;
using VecompSoftware.Core.Command.CQRS.Commands.Models.Messages;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Services.Command;
using VecompSoftware.Core.Command;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using System.IO;
using System.Web.UI;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Collaborations;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Builders
{
    [LogCategory(LogCategoryDefinition.WEBAPIBUILDER)]
    public class BuilderController : ApiController
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IUDSRoleService _udsAuthorizationService;
        private readonly IUDSUserService _udsUserService;
        private readonly IUDSPECMailService _udsPECMailService;
        private readonly IUDSMessageService _udsMessageService;
        private readonly IUDSDocumentUnitService _udsDocumentUnitService;
        private readonly IUDSContactService _udsContactService;
        private readonly IUDSCollaborationService _udsCollaborationService;
        private readonly IDocumentContext<ModelDocument.Document, ArchiveDocument> _documentService;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        private readonly ICollaborationVersioningService _collaborationVersioningService;
        private readonly ICollaborationSignService _collaborationSignService;
        private readonly ICollaborationLogService _collaborationLogService;
        private readonly ICollaborationService _collaborationService;
        private readonly IDecryptedParameterEnvService _parameterEnvBaseService;
        private readonly ICurrentIdentity _currentIdentity;
        private readonly ICQRSMessageMapper _cqrsMapper;
        private readonly IQueueService _queueService;
        #endregion

        #region [ Constructor ]
        public BuilderController(ILogger logger, ISecurity security, /*IServiceUnitOfWork serviceUnitOfWork,*/ IDataUnitOfWork unitOfWork, IUDSRoleService udsAuthorizationService, IUDSUserService udsUSerService,
            IUDSPECMailService udsPECMailService, IUDSMessageService udsMessageService, IUDSDocumentUnitService udsDocumentUnitService, IUDSContactService udsContactService, IUDSCollaborationService udsCollaborationService,
            IDocumentContext<ModelDocument.Document, ArchiveDocument> documentService, IMapperUnitOfWork mapperUnitOfWork, ICollaborationVersioningService collaborationVersioningService, ICollaborationSignService collaborationSignService, ICollaborationLogService collaborationLogService, ICollaborationService collaborationService, IDecryptedParameterEnvService parameterEnvBaseService, ICurrentIdentity currentIdentity, IQueueService queueService, ICQRSMessageMapper cqrsMapper
            )
            : base()
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _udsAuthorizationService = udsAuthorizationService;
            _udsUserService = udsUSerService;
            _udsPECMailService = udsPECMailService;
            _udsMessageService = udsMessageService;
            _udsDocumentUnitService = udsDocumentUnitService;
            _udsContactService = udsContactService;
            _udsCollaborationService = udsCollaborationService;
            _documentService = documentService;
            _mapperUnitOfWork = mapperUnitOfWork;
            _collaborationVersioningService = collaborationVersioningService;
            _collaborationSignService = collaborationSignService;
            _collaborationLogService = collaborationLogService;
            _collaborationService = collaborationService;
            _parameterEnvBaseService = parameterEnvBaseService;
            _currentIdentity = currentIdentity;
            _queueService = queueService;
            _cqrsMapper = cqrsMapper;
        }
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories {
            get {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(BuilderController));
                }
                return _logCategories;
            }
        }

        #endregion

        #region [ Methods ]

        [AcceptVerbs("OPTIONS")]
        [AllowAnonymous]
        public IHttpActionResult Options()
        {
            return Ok();
        }

        public IHttpActionResult Get()
        {
            return Ok("Web API is alive.");
        }


        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] BuildActionModel model)
        {
            return await WebApiHelpers.ActionHelper.TryCatchWithLoggerAsync(async () =>
            {
                _logger.WriteInfo(new LogMessage($"Builder controller receive request type {model?.ReferenceType}/{model?.BuildType}"), LogCategories);

                if (model == null || string.IsNullOrEmpty(model.Model))
                {
                    throw new DSWValidationException("Evaluate model validation error",
                        new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "BuildActionModel", Message = $"Model '{model?.Model}' is not valid " } },
                        null, DSWExceptionCode.VA_RulesetValidation);
                }

                switch (model.ReferenceType)
                {
                    case ReferenceBuildModelType.None:
                    case ReferenceBuildModelType.Protocol:
                    case ReferenceBuildModelType.Resolution:
                    case ReferenceBuildModelType.DocumentSeries:
                    case ReferenceBuildModelType.Desk:
                    case ReferenceBuildModelType.Workflow:
                    case ReferenceBuildModelType.Dossier:
                    case ReferenceBuildModelType.Build:
                    case ReferenceBuildModelType.PECMail:
                    case ReferenceBuildModelType.Message:
                        {
                            throw new DSWValidationException("Evaluate model validation error",
                                new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "BuildActionModel", Message = $"ReferenceType {model?.ReferenceType} is not supported" } },
                                null, DSWExceptionCode.VA_RulesetValidation);
                        }
                    case ReferenceBuildModelType.Document:
                        {
                            await DocumentEvaluation(model);
                        }
                        break;
                    case ReferenceBuildModelType.UDS:
                        {
                            await UDSEvaluation(model);
                        }
                        break;
                    case ReferenceBuildModelType.Collaboration:
                        {
                            await CollaborationEvaluation(model);
                        }
                        break;
                    case ReferenceBuildModelType.Fascicle:
                        {
                            await FascicleEvaluation(model);
                        }
                        break;
                }
                return Ok(model);

            }, BadRequest, Content, InternalServerError, _logger, LogCategories);
        }

        private async Task DocumentEvaluation(BuildActionModel model)
        {
            ArchiveDocument archiveDocument = JsonConvert.DeserializeObject<ArchiveDocument>(model.Model);

            switch (model.BuildType)
            {
                case BuildActionType.Director:
                case BuildActionType.Evaluate:
                case BuildActionType.Destroy:
                case BuildActionType.None:
                case BuildActionType.Synchronize:
                    {
                        throw new DSWValidationException("Evaluate Document model validation error",
                            new List<ValidationMessageModel>() { new ValidationMessageModel
                            {
                                Key = "BuildActionModel",
                                Message = $"BuildType {model?.BuildType} is not supported" } },
                            null, DSWExceptionCode.VA_RulesetValidation);
                    }
                case BuildActionType.Build:
                    {
                        archiveDocument = await _documentService.InsertDocumentAsync(archiveDocument);
                        model.Model = JsonConvert.SerializeObject(archiveDocument);
                        break;
                    }
            }
        }

        private async Task UDSEvaluation(BuildActionModel model)
        {
            UDSRelationModel relationModel = JsonConvert.DeserializeObject<UDSRelationModel>(model.Model);
            UDSRepository repository = _unitOfWork.Repository<UDSRepository>().Find(relationModel.UDSRepository.Id);
            ICollection<UDSRole> rolesToManage = GetEntityRoles(relationModel.Roles, repository);
            ICollection<UDSUser> usersToManage = GetEntityUsers(relationModel.Users, repository);
            ICollection<UDSPECMail> pecMailsToManage = GetEntityPECMails(relationModel.PECMails, repository);
            ICollection<UDSMessage> messagesToManage = GetEntityMessages(relationModel.Messages, repository);
            ICollection<UDSDocumentUnit> documentUnitsToManage = GetEntityDocumentUnits(relationModel.DocumentUnits, repository);
            ICollection<UDSContact> contactsToManage = GetEntityContacts(relationModel.Contacts, repository);
            ICollection<UDSCollaboration> collaborationsToManage = GetEntityCollaborations(relationModel.Collaborations, repository);
            switch (model.BuildType)
            {
                case BuildActionType.Director:
                case BuildActionType.Evaluate:
                case BuildActionType.Destroy:
                case BuildActionType.None:
                    {
                        throw new DSWValidationException("Evaluate UDS model validation error",
                            new List<ValidationMessageModel>() { new ValidationMessageModel
                            {
                                Key = "BuildActionModel",
                                Message = $"BuildType {model?.BuildType} is not supported" } },
                            null, DSWExceptionCode.VA_RulesetValidation);
                    }
                case BuildActionType.Build:
                    {
                        _unitOfWork.BeginTransaction();

                        await BuildUDSRoleAsync(rolesToManage);
                        await BuildUDSUserAsync(usersToManage);
                        await BuildUDSPECMailAsync(pecMailsToManage);
                        await BuildUDSMessageAsync(messagesToManage);
                        await BuildUDSDocumentUnitAsync(documentUnitsToManage);
                        await BuildUDSContactAsync(contactsToManage);
                        await BuildUDSCollaborationAsync(collaborationsToManage);

                        await _unitOfWork.SaveAsync();
                        break;
                    }
                case BuildActionType.Synchronize:
                    {
                        _unitOfWork.BeginTransaction();

                        await SynchroniseUDSRoleAsync(rolesToManage, model.ReferenceId, repository);
                        await SynchroniseUDSUserAsync(usersToManage, model.ReferenceId, repository);
                        await SynchroniseUDSPECMailAsync(pecMailsToManage, model.ReferenceId, repository);
                        await SynchroniseUDSMessageAsync(messagesToManage, model.ReferenceId, repository);
                        await SynchroniseUDSDocumentUnitAsync(documentUnitsToManage, model.ReferenceId, repository);
                        await SynchroniseUDSContactAsync(contactsToManage, model.ReferenceId, repository);
                        await SynchroniseUDSCollaborationAsync(collaborationsToManage, model.ReferenceId, repository);

                        await _unitOfWork.SaveAsync();
                        break;
                    }
            }
        }

        private async Task CollaborationEvaluation(BuildActionModel model)
        {
            _logger.WriteInfo(new LogMessage($"{model?.ReferenceType}/{model?.BuildType} Begin build action."), LogCategories);

            switch (model.BuildType)
            {
                case BuildActionType.Director:
                case BuildActionType.Destroy:
                case BuildActionType.None:
                case BuildActionType.Build:
                    {
                        throw new DSWValidationException("Evaluate Document model validation error",
                            new List<ValidationMessageModel>() { new ValidationMessageModel
                            {
                                Key = "BuildActionModel",
                                Message = $"BuildType {model?.BuildType} is not supported" } },
                            null, DSWExceptionCode.VA_RulesetValidation);
                    }
                case BuildActionType.Evaluate:
                    {
                        EvaluateCollaborationModel evaluateCollaborationModel = JsonConvert.DeserializeObject<EvaluateCollaborationModel>(model.Model);

                        _unitOfWork.BeginTransaction();

                        await EvaluateCollaborationAsync(evaluateCollaborationModel);

                        await _unitOfWork.SaveAsync();

                        break;
                    }
                case BuildActionType.Synchronize:
                    {
                        SynchronizeCollaborationModel synchronizeCollaborationModel = JsonConvert.DeserializeObject<SynchronizeCollaborationModel>(model.Model);

                        _unitOfWork.BeginTransaction();

                        await SynchroniseCollaborationsAsync(synchronizeCollaborationModel);

                        await _unitOfWork.SaveAsync();

                        break;
                    }
            }

            _logger.WriteInfo(new LogMessage($"{model?.ReferenceType}/{model?.BuildType} Build action terminated correctly"), LogCategories);
        }

        private async Task FascicleEvaluation(BuildActionModel model)
        {
            _logger.WriteInfo(new LogMessage($"{model?.ReferenceType}/{model?.BuildType} Begin build action."), LogCategories);

            switch (model.BuildType)
            {
                case BuildActionType.Director:
                case BuildActionType.Destroy:
                case BuildActionType.None:
                case BuildActionType.Build:
                case BuildActionType.Synchronize:
                    {
                        throw new DSWValidationException("Evaluate Document model validation error",
                            new List<ValidationMessageModel>() { new ValidationMessageModel
                            {
                                Key = "BuildActionModel",
                                Message = $"BuildType {model?.BuildType} is not supported" } },
                            null, DSWExceptionCode.VA_RulesetValidation);
                    }
                case BuildActionType.Evaluate:
                    {
                        EvaluateFascicleModel evaluateFascicleModel = JsonConvert.DeserializeObject<EvaluateFascicleModel>(model.Model);

                        _unitOfWork.BeginTransaction();

                        await EvaluateFascicleAsync(evaluateFascicleModel);

                        await _unitOfWork.SaveAsync();

                        break;
                    }
            }

            _logger.WriteInfo(new LogMessage($"{model?.ReferenceType}/{model?.BuildType} Build action terminated correctly"), LogCategories);
        }

        private ICollection<UDSRole> GetEntityRoles(ICollection<UDSRoleModel> roleModels, UDSRepository repository)
        {
            ICollection<UDSRole> roles = new List<UDSRole>();
            UDSRole udsRole;
            foreach (UDSRoleModel roleModel in roleModels)
            {
                udsRole = _mapperUnitOfWork.Repository<IUDSRoleEntityMapper>().Map(roleModel, new UDSRole());
                udsRole.Relation = _unitOfWork.Repository<Role>().Find(roleModel.IdRole);
                udsRole.Environment = repository.DSWEnvironment;
                udsRole.Repository = repository;
                roles.Add(udsRole);
            }
            return roles;
        }

        private ICollection<UDSUser> GetEntityUsers(ICollection<UDSUserModel> udsUserModels, UDSRepository repository)
        {
            ICollection<UDSUser> users = new List<UDSUser>();
            UDSUser udsUser;
            foreach (UDSUserModel userModel in udsUserModels)
            {
                udsUser = _mapperUnitOfWork.Repository<IUDSUserEntityMapper>().Map(userModel, new UDSUser());
                udsUser.Environment = repository.DSWEnvironment;
                udsUser.Repository = repository;
                users.Add(udsUser);
            }
            return users;
        }

        private ICollection<UDSPECMail> GetEntityPECMails(ICollection<UDSPECMailModel> udsPecMailModels, UDSRepository repository)
        {
            ICollection<UDSPECMail> pecMails = new List<UDSPECMail>();
            UDSPECMail udsPECMail;
            foreach (UDSPECMailModel pecMail in udsPecMailModels)
            {
                udsPECMail = _mapperUnitOfWork.Repository<IUDSPECMailEntityMapper>().Map(pecMail, new UDSPECMail());
                udsPECMail.Relation = _unitOfWork.Repository<PECMail>().Find(pecMail.IdPECMail);
                udsPECMail.Environment = repository.DSWEnvironment;
                udsPECMail.Repository = repository;
                pecMails.Add(udsPECMail);
            }
            return pecMails;
        }

        private ICollection<UDSMessage> GetEntityMessages(ICollection<UDSMessageModel> udsMessageModels, UDSRepository repository)
        {
            ICollection<UDSMessage> messages = new List<UDSMessage>();
            UDSMessage udsMessage;
            foreach (UDSMessageModel message in udsMessageModels)
            {
                udsMessage = _mapperUnitOfWork.Repository<IUDSMessageEntityMapper>().Map(message, new UDSMessage());
                udsMessage.Relation = _unitOfWork.Repository<DocSuiteWeb.Entity.Messages.Message>().Find(message.IdMessage);
                udsMessage.Environment = repository.DSWEnvironment;
                udsMessage.Repository = repository;
                messages.Add(udsMessage);
            }
            return messages;
        }

        private ICollection<UDSDocumentUnit> GetEntityDocumentUnits(ICollection<UDSDocumentUnitModel> udsDocumentUnitModels, UDSRepository repository)
        {
            ICollection<UDSDocumentUnit> documentUnits = new List<UDSDocumentUnit>();
            UDSDocumentUnit udsDocumentUnit;
            foreach (UDSDocumentUnitModel documentUnit in udsDocumentUnitModels)
            {
                udsDocumentUnit = _mapperUnitOfWork.Repository<IUDSDocumentUnitEntityMapper>().Map(documentUnit, new UDSDocumentUnit());
                udsDocumentUnit.Relation = _unitOfWork.Repository<DocumentUnit>().Find(documentUnit.IdDocumentUnit);
                udsDocumentUnit.Environment = repository.DSWEnvironment;
                udsDocumentUnit.Repository = repository;
                documentUnits.Add(udsDocumentUnit);
            }
            return documentUnits;
        }

        private ICollection<UDSContact> GetEntityContacts(ICollection<UDSContactModel> udsContactModels, UDSRepository repository)
        {
            ICollection<UDSContact> contacts = new List<UDSContact>();
            UDSContact udsContact;
            foreach (UDSContactModel contact in udsContactModels)
            {
                udsContact = _mapperUnitOfWork.Repository<IUDSContactEntityMapper>().Map(contact, new UDSContact());
                udsContact.Environment = repository.DSWEnvironment;
                udsContact.Repository = repository;
                if (udsContact.ContactType == (short)UDSContactType.Normal)
                {
                    udsContact.Relation = _unitOfWork.Repository<Contact>().Find(contact.IdContact);
                }
                contacts.Add(udsContact);
            }
            return contacts;
        }

        private ICollection<UDSCollaboration> GetEntityCollaborations(ICollection<UDSCollaborationModel> udsCollaborationModels, UDSRepository repository)
        {
            ICollection<UDSCollaboration> collaborations = new List<UDSCollaboration>();
            UDSCollaboration udsCollaboration;
            foreach (UDSCollaborationModel collaboration in udsCollaborationModels)
            {
                udsCollaboration = _mapperUnitOfWork.Repository<IUDSCollaborationEntityMapper>().Map(collaboration, new UDSCollaboration());
                udsCollaboration.Relation = _unitOfWork.Repository<Collaboration>().Find(collaboration.IdCollaboration);
                udsCollaboration.Environment = repository.DSWEnvironment;
                udsCollaboration.Repository = repository;
                collaborations.Add(udsCollaboration);
            }
            return collaborations;
        }

        private async Task CheckOut(CollaborationVersioning collVersioning)
        {
            if (collVersioning.CheckedOut != null && collVersioning.CheckedOut.Value)
            {
                if (collVersioning.CheckOutUser.Equals(_currentIdentity.FullUserName))
                {
                    return;
                }
                else
                {
                    throw new DSWException($"The Collaboration Versioning {collVersioning.UniqueId} is already checked-out by {collVersioning.CheckOutUser}", null, DSWExceptionCode.Invalid);
                }
            }

            collVersioning.CheckedOut = true;
            collVersioning.CheckOutUser = _currentIdentity.FullUserName;
            collVersioning.CheckOutSessionId = Guid.NewGuid().ToString();
            collVersioning.CheckOutDate = DateTime.Now;

            await _collaborationVersioningService.UpdateAsync(collVersioning);
        }

        private async Task CheckIn(CollaborationVersioning collVersioning, short nextIncremental, ArchiveDocument archiveDocument)
        {
            if (collVersioning.CheckedOut == null || collVersioning.CheckedOut.Value == false)
            {
                throw new DSWException($"The Collaboration Versioning {collVersioning.UniqueId} is not checked-out", null, DSWExceptionCode.Invalid);
            }

            if (collVersioning.CheckOutUser.Equals(_currentIdentity.FullUserName) == false)
            {
                throw new DSWException($"The Collaboration Versioning {collVersioning.UniqueId} is already checked-out by {collVersioning.CheckOutUser}", null, DSWExceptionCode.Invalid);
            }

            CollaborationVersioning newCollVersioning = new CollaborationVersioning();
            newCollVersioning.Collaboration = collVersioning.Collaboration;
            newCollVersioning.CollaborationIncremental = collVersioning.CollaborationIncremental;
            newCollVersioning.Incremental = nextIncremental;
            newCollVersioning.IdDocument = archiveDocument.IdLegacyChain;
            newCollVersioning.DocumentName = archiveDocument.Name;
            newCollVersioning.RegistrationUser = _currentIdentity.FullUserName;
            newCollVersioning.RegistrationDate = DateTime.Now;
            newCollVersioning.CheckedOut = false;
            newCollVersioning.DocumentChecksum = null;
            newCollVersioning.IsActive = true;
            newCollVersioning.DocumentGroup = collVersioning.DocumentGroup;

            await _collaborationVersioningService.CreateAsync(newCollVersioning);
        }

        private async Task UndoCheckOut(CollaborationVersioning collaborationVersioning)
        {
            if (collaborationVersioning.CheckedOut == null || collaborationVersioning.CheckedOut.Value == false)
            {
                return;
            }

            if (collaborationVersioning.CheckOutUser.Equals(_currentIdentity.FullUserName) == false)
            {
                throw new DSWException($"The Collaboration Versioning {collaborationVersioning.UniqueId} is checked-out by {collaborationVersioning.CheckOutUser}", null, DSWExceptionCode.Invalid);
            };

            collaborationVersioning.CheckedOut = false;
            collaborationVersioning.CheckOutUser = null;
            collaborationVersioning.CheckOutSessionId = null;
            collaborationVersioning.CheckOutDate = null;

            await _collaborationVersioningService.UpdateAsync(collaborationVersioning);
        }

        private void SendMail(Collaboration collaboration, string action, Guid idTenantAOO, CollaborationSign currentSigner, CollaborationSign nextSigner = null)
        {
            if (CheckSendMailPrecondition(collaboration) == false)
            {
                _logger.WriteInfo(new LogMessage($"[SynchroniseCollaboration] Emails will not be sent."), LogCategories);
                return;
            }

            _logger.WriteInfo(new LogMessage($"[SynchroniseCollaboration] Sending emails."), LogCategories);

            string moduleName = GetModuleName(collaboration.DocumentType);
            string signature = GetSignature(collaboration, moduleName);
            string messageSubject = GetSubject(action, moduleName, signature);
            List<string> recipientsEmail = new List<string>();
            List<string> recipientsName = new List<string>();

            if (action.Equals(CollaborationActionType.DaVisionareFirmare))
            {
                recipientsEmail.Add(nextSigner.SignEmail);
                recipientsName.Add(nextSigner.SignName);
            }
            else
            {
                foreach (CollaborationUser user in collaboration.CollaborationUsers)
                {
                    recipientsName.Add(user.DestinationName);

                    if (user.DestinationType.Equals(CollaborationDestinationType.P.ToString()))
                    {
                        recipientsEmail.Add(user.DestinationEmail);
                    }
                    else
                    {
                        recipientsEmail.AddRange(user.Role.EMailAddress?.Split(';'));
                    }
                }
            }

            string messageBody = GetMessageBody(collaboration, action, moduleName, currentSigner.SignName, recipientsName, signature);

            MessageBuildModel messageBuildModel = CreateMessageBuildModel(collaboration.UniqueId, messageBody, messageSubject,
                recipientsEmail, currentSigner.SignEmail, DocSuiteWeb.Model.Entities.Messages.MessageContactType.User, collaboration.IdPriority);

            if (messageBuildModel == null)
            {
                _logger.WriteError(new LogMessage($"Email has not been sended for Collaboration instance: {collaboration.UniqueId}"), LogCategories);
            }

            IIdentityContext identity = new IdentityContext(_currentIdentity.FullUserName);

            CommandBuildMessage command = new CommandBuildMessage(_parameterEnvBaseService.CurrentTenantName, _parameterEnvBaseService.CurrentTenantId,
                idTenantAOO, identity, messageBuildModel);

            ServiceBusMessage message = _cqrsMapper.Map(command, new ServiceBusMessage());
            if (message == null || string.IsNullOrEmpty(message.ChannelName))
            {
                throw new DSWException($"Queue name to command [{command}] is not mapped", null, DSWExceptionCode.SC_Mapper);
            }
            Task.Run(async () =>
            {
                await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
            }).Wait();
        }

        private bool CheckSendMailPrecondition(Collaboration collaboration)
        {
            if (_parameterEnvBaseService.CollaborationMailEnabled == false)
            {
                return false;
            }

            return true;
        }

        private string GetModuleName(string documentType)
        {
            if (string.IsNullOrEmpty(documentType))
            {
                return string.Empty;
            }

            switch (documentType)
            {
                case CollaborationDocumentType.Protocol:
                    return "Protocollo";
                case CollaborationDocumentType.ResolutionDelibera:
                    return "Delibera";
                case CollaborationDocumentType.ResolutionDetermina:
                    return "Atto";
            }

            return string.Empty;
        }

        private string GetSignature(Collaboration collaboration, string moduleName)
        {
            if (_parameterEnvBaseService.CollaborationSignatureType == 99)
            {
                return string.Empty;
            }

            string signature = $"Collaborazione {moduleName} n. {collaboration.EntityId} del {collaboration.RegistrationDate.DateTime:dd/MM/yyyy}";

            if (_parameterEnvBaseService.CollaborationSignatureType == 1)
            {
                signature = $"{_parameterEnvBaseService.CorporateAcronym}{signature}";
            }

            return signature;
        }

        private string GetSubject(string action, string moduleName, string signature)
        {
            string subject = GetStatusSubject(action);

            subject = $"DocSuite {signature} - {moduleName} {subject}";

            return subject;
        }

        private string GetStatusSubject(string action)
        {
            if (string.IsNullOrEmpty(action))
            {
                return string.Empty;
            }

            switch (action)
            {
                case CollaborationActionType.DaProtocollareGestire:
                    return "Documento da Protocollare";
                case CollaborationActionType.DaVisionareFirmare:
                    return "Inoltro Documento per Visione/Firma";
            }

            return string.Empty;
        }

        private string GetMessageBody(Collaboration collaboration, string action, string moduleName, string sender, List<string> recepients, string signature)
        {
            using (StringWriter sw = new StringWriter())
            using (HtmlTextWriter body = new HtmlTextWriter(sw))
            {
                //Tipologia richiesta
                string statusSubject = GetStatusSubject(action);
                body.Write("Tipologia Richiesta:&nbsp;");
                body.RenderBeginTag("b");
                body.WriteEncodedText($"{moduleName} - {statusSubject}");
                body.RenderEndTag();
                body.WriteBeginTag("br");
                body.Write(HtmlTextWriter.SelfClosingTagEnd);
                body.WriteBeginTag("br");
                body.Write(HtmlTextWriter.SelfClosingTagEnd);

                //Da
                body.Write("Da:&nbsp;");
                body.RenderBeginTag("b");
                body.WriteEncodedText(sender);
                body.RenderEndTag();
                body.WriteBeginTag("br");
                body.Write(HtmlTextWriter.SelfClosingTagEnd);

                //A
                body.Write("A:&nbsp;");
                body.RenderBeginTag("b");
                body.WriteEncodedText(string.Join("; ", recepients));
                body.RenderEndTag();
                body.WriteBeginTag("br");
                body.Write(HtmlTextWriter.SelfClosingTagEnd);
                body.WriteBeginTag("br");
                body.Write(HtmlTextWriter.SelfClosingTagEnd);


                //Tipologia documento
                body.Write("Tipologia Documento:&nbsp;");
                body.RenderBeginTag("b");
                body.WriteEncodedText(moduleName);
                body.RenderEndTag();
                body.WriteBeginTag("br");
                body.Write(HtmlTextWriter.SelfClosingTagEnd);

                //Oggetto
                body.Write("Oggetto:&nbsp;");
                body.RenderBeginTag("b");
                body.WriteEncodedText(collaboration.Subject);
                body.RenderEndTag();
                body.WriteBeginTag("br");
                body.Write(HtmlTextWriter.SelfClosingTagEnd);

                //Note
                if (string.IsNullOrEmpty(collaboration.Note) == false)
                {
                    body.Write("Note:&nbsp;");
                    body.RenderBeginTag("b");
                    body.WriteEncodedText(collaboration.Note);
                    body.RenderEndTag();
                }

                body.WriteBeginTag("br");
                body.Write(HtmlTextWriter.SelfClosingTagEnd);
                body.WriteBeginTag("br");
                body.Write(HtmlTextWriter.SelfClosingTagEnd);

                //Link
                if (string.IsNullOrEmpty(signature))
                {
                    signature = "Collegamento";
                }

                if (action.Equals(CollaborationActionType.CancellazioneDocumento) == false)
                {
                    string url = $"{_parameterEnvBaseService.CurrentTenantModel.DSWUrl}?Tipo=Coll&Azione={action}&Identificativo={collaboration.EntityId}&Stato={action}";
                    body.AddAttribute(HtmlTextWriterAttribute.Href, url);
                    body.RenderBeginTag(HtmlTextWriterTag.A);
                    body.Write(signature);
                    body.RenderEndTag();
                }

                body.Flush();
                return sw.ToString();
            }
        }

        private MessageBuildModel CreateMessageBuildModel(Guid collaborationId, string messageBody, string messageSubject, List<string> recipientEmails,
            string senderEmail, DocSuiteWeb.Model.Entities.Messages.MessageContactType contactType, string priority)
        {
            IList<MessageContactModel> recipients = new List<MessageContactModel>();
            recipientEmails = recipientEmails ?? new List<string>();

            if (recipientEmails.All(f => string.IsNullOrEmpty(f)))
            {
                _logger.WriteWarning(new LogMessage($"Email recipients has not been defined in collaboration: {collaborationId}"), LogCategories);
            }
            if (string.IsNullOrEmpty(senderEmail))
            {
                _logger.WriteWarning(new LogMessage($"Email sender has not been defined in collaboration: {collaborationId}"), LogCategories);
            }

            MessageContactEmailModel contactEmail;
            MessageContactModel contact;

            #region Sender
            if (!string.IsNullOrEmpty(senderEmail))
            {
                contactEmail = new MessageContactEmailModel()
                {
                    Description = senderEmail,
                    Email = senderEmail,
                    User = _currentIdentity.FullUserName
                };

                contact = new MessageContactModel()
                {
                    ContactPosition = MessageContantTypology.Sender,
                    ContactType = contactType,
                    Description = senderEmail,
                    MessageContactEmail = new List<MessageContactEmailModel>() { contactEmail },
                };
                recipients.Add(contact);
            }

            #endregion

            #region Recipients
            foreach (string recipient in recipientEmails.Where(f => !string.IsNullOrEmpty(f)))
            {
                contactEmail = new MessageContactEmailModel()
                {
                    Description = recipient,
                    Email = recipient,
                    User = _currentIdentity.FullUserName
                };
                contact = new MessageContactModel()
                {
                    ContactPosition = MessageContantTypology.Recipient,
                    ContactType = contactType,
                    Description = recipient,
                    MessageContactEmail = new List<MessageContactEmailModel>() { contactEmail },
                };
                recipients.Add(contact);
            }

            #endregion

            #region Message
            string currentPriority = MessagePriorityType.Normal;
            switch (priority)
            {
                case "B":
                    {
                        currentPriority = MessagePriorityType.Low;
                        break;
                    }
                case "A":
                    {
                        currentPriority = MessagePriorityType.High;
                        break;
                    }
                default:
                    break;
            }

            MessageEmailModel email = new MessageEmailModel()
            {
                Body = messageBody,
                Subject = messageSubject,
                Priority = currentPriority
            };

            MessageModel message = new MessageModel()
            {
                MessageType = DocSuiteWeb.Model.Entities.Messages.MessageType.Email,
                MessageContacts = recipients,
                Status = DocSuiteWeb.Model.Entities.Messages.MessageStatus.Active,
                MessageEmails = new List<MessageEmailModel>() { email }
            };

            #endregion

            MessageBuildModel messageBuildModel = new MessageBuildModel
            {
                Message = message,
                WorkflowAutoComplete = true
            };

            return messageBuildModel;
        }

        #region [ Build Methods ]
        private async Task BuildUDSRoleAsync(ICollection<UDSRole> rolesToManage)
        {
            foreach (UDSRole item in rolesToManage)
            {
                await _udsAuthorizationService.CreateAsync(item);
            }
        }

        private async Task BuildUDSUserAsync(ICollection<UDSUser> usersToManage)
        {
            foreach (UDSUser item in usersToManage)
            {
                await _udsUserService.CreateAsync(item);
            }
        }

        private async Task BuildUDSPECMailAsync(ICollection<UDSPECMail> pecMailsToManage)
        {
            foreach (UDSPECMail item in pecMailsToManage)
            {
                await _udsPECMailService.CreateAsync(item);
            }
        }

        private async Task BuildUDSMessageAsync(ICollection<UDSMessage> messagesToManage)
        {
            foreach (UDSMessage item in messagesToManage)
            {
                await _udsMessageService.CreateAsync(item);
            }
        }

        private async Task BuildUDSDocumentUnitAsync(ICollection<UDSDocumentUnit> documentUnitsToManage)
        {
            foreach (UDSDocumentUnit item in documentUnitsToManage)
            {
                await WebApiHelpers.ActionHelper.RetryingPolicyActionAsync<Task>(async () => { await _udsDocumentUnitService.CreateAsync(item); }, _logger, LogCategories);
            }
        }

        private async Task BuildUDSContactAsync(ICollection<UDSContact> contactsToManage)
        {
            foreach (UDSContact item in contactsToManage)
            {
                await _udsContactService.CreateAsync(item);
            }
        }

        private async Task BuildUDSCollaborationAsync(ICollection<UDSCollaboration> collaborationsToManage)
        {
            foreach (UDSCollaboration item in collaborationsToManage)
            {
                await _udsCollaborationService.CreateAsync(item);
            }
        }

        #endregion

        #region [ Evaluete Methods ]

        private async Task EvaluateCollaborationAsync(EvaluateCollaborationModel evaluateCollaborationModel)
        {
            foreach (EvaluateCollaborationDocumentModel documentModel in evaluateCollaborationModel.DocumentModels)
            {
                Collaboration collaboration = _unitOfWork.Repository<Collaboration>().GetByUniqueIdWithVersioningAndSign(documentModel.IdCollaboration).FirstOrDefault();

                CollaborationVersioning collaborationVersioning = collaboration.CollaborationVersionings.FirstOrDefault(x => x.UniqueId == documentModel.IdCollaborationVersioning);
                CollaborationSign collaborationSign = collaboration.CollaborationSigns.FirstOrDefault(x => x.IsActive && x.SignUser.Equals(_currentIdentity.FullUserName, StringComparison.InvariantCultureIgnoreCase));
                short nextIncremental = (short)(collaboration.CollaborationVersionings.Where(x => x.CollaborationIncremental == collaborationVersioning.CollaborationIncremental).Max(x => x.Incremental) + 1);

                _logger.WriteInfo(
                    new LogMessage($"[EvaluateCollaboration] Collaboration: {collaboration.EntityId} - Versioning: {collaborationVersioning.EntityId} - Sign: {collaborationSign.EntityId}."),
                    LogCategories);
                _logger.WriteInfo(new LogMessage($"[EvaluateCollaboration] Checking out/in document."), LogCategories);

                await CheckOut(collaborationVersioning);

                Guid idChain = await _documentService.GetDocumentIdAsync(collaborationVersioning.IdDocument, documentModel.ArchiveDocument.Archive);
                ModelDocument.Document versioningDocument = (await _documentService.GetDocumentLatestVersionFromChainAsync(idChain)).FirstOrDefault();

                if (evaluateCollaborationModel.FromWorkflow)
                {
                    ModelDocument.Document document = (await _documentService.GetDocumentLatestVersionFromChainAsync(documentModel.ArchiveDocument.IdChain.Value)).FirstOrDefault();
                    documentModel.ArchiveDocument.Name = document.Name;
                    documentModel.ArchiveDocument.ContentStream = await _documentService.GetDocumentContentAsync(document.IdDocument);
                }

                ModelDocument.AttributeValue signatureAttribute = versioningDocument.AttributeValues.FirstOrDefault(x => x.AttributeName == AttributeValue.ATTRIBUTE_SIGNATURE);
				if (string.IsNullOrWhiteSpace(documentModel.ArchiveDocument.Signature) && signatureAttribute != null)
                {
                    documentModel.ArchiveDocument.Signature = signatureAttribute.ValueString;
				}                
                ArchiveDocument archiveDocument = await _documentService.InsertDocumentAsync(documentModel.ArchiveDocument);

                await CheckIn(collaborationVersioning, nextIncremental, archiveDocument);

                await _collaborationLogService.CreateAsync(CollaborationService.CreateLog(collaboration, CollaborationLogType.MODIFICA_SEMPLICE, $"File Modificato [{archiveDocument.Name}].",
                    _currentIdentity.FullUserName, archiveDocument.IdLegacyChain, nextIncremental, collaborationVersioning.CollaborationIncremental));

                await UndoCheckOut(collaborationVersioning);
                collaborationSign.SignDate = DateTime.Now;

                _logger.WriteInfo(new LogMessage($"[EvaluateCollaboration] Updating sign date."), LogCategories);
                await _collaborationSignService.UpdateAsync(collaborationSign);

                await _collaborationLogService.CreateAsync(CollaborationService.CreateLog(collaboration, CollaborationLogType.FIRMA_DOCUMENTO, $"Firma Documento [{archiveDocument.Name}].",
                    _currentIdentity.FullUserName, archiveDocument.IdLegacyChain, collaborationVersioning.Incremental, collaborationVersioning.CollaborationIncremental));
            }
        }

        private async Task EvaluateFascicleAsync(EvaluateFascicleModel evaluateFascicleModel)
        {
            ModelDocument.Document orignalDocument = await _documentService.GetDocumentAsync(evaluateFascicleModel.ArchiveDocument.IdDocument);
            byte[] documentContent = await _documentService.GetDocumentContentAsync(evaluateFascicleModel.ArchiveDocument.IdDocument);

            ArchiveDocument clonedDocument = new ArchiveDocument
            {
                ContentStream = documentContent,
                Name = orignalDocument.Name,
                Archive = orignalDocument.ArchiveName
            };

            await _documentService.InsertDocumentsAsync(new List<ArchiveDocument>() { clonedDocument }, idChain: orignalDocument.IdChain);

            ArchiveDocument signedDocument = new ArchiveDocument
            {
                IdDocument = orignalDocument.IdDocument,
                ContentStream = evaluateFascicleModel.ArchiveDocument.ContentStream,
                Name = evaluateFascicleModel.ArchiveDocument.Name,
                Archive = orignalDocument.ArchiveName,
            };

            Dictionary<string, string> attributes = new Dictionary<string, string>();

            foreach (AttributeValue attribute in orignalDocument.AttributeValues)
            {
                attributes.Add(attribute.AttributeName, attribute.ValueString);
            }

            if (attributes.Any(f => f.Key.Equals(AttributeValue.ATTRIBUTE_ISSIGNED, StringComparison.InvariantCultureIgnoreCase)) == true)
            {
                attributes[AttributeValue.ATTRIBUTE_ISSIGNED] = "True";
            }
            else
            {
                attributes.Add(AttributeValue.ATTRIBUTE_ISSIGNED, "True");
            }

            if (attributes.Any(f => f.Key.Equals(AttributeValue.ATTRIBUTE_FILENAME, StringComparison.InvariantCultureIgnoreCase)) == true)
            {
                attributes[AttributeValue.ATTRIBUTE_FILENAME] = evaluateFascicleModel.ArchiveDocument.Name;
            }
            else
            {
                attributes.Add(AttributeValue.ATTRIBUTE_FILENAME, evaluateFascicleModel.ArchiveDocument.Name);
            }

            await _documentService.UpdateDocumentAsync(signedDocument, attributes);
        }

        #endregion

        #region [ Synchronise Methods ]
        private async Task SynchroniseUDSRoleAsync(ICollection<UDSRole> rolesToManage, Guid udsId, UDSRepository repository)
        {
            ICollection<UDSRole> oldAuthorizations = _unitOfWork.Repository<UDSRole>().GetByUDSId(udsId).ToList();
            foreach (UDSRole item in oldAuthorizations.Where(f => !rolesToManage.Any(c => c.Relation.EntityShortId == f.Relation.EntityShortId)))
            {
                item.Repository = repository;
                await _udsAuthorizationService.DeleteAsync(item);
            }
            foreach (UDSRole item in rolesToManage.Where(f => !oldAuthorizations.Any(c => c.Relation.EntityShortId == f.Relation.EntityShortId)))
            {
                await _udsAuthorizationService.CreateAsync(item);
            }
        }

        private async Task SynchroniseUDSUserAsync(ICollection<UDSUser> usersToManage, Guid udsId, UDSRepository repository)
        {
            ICollection<UDSUser> oldUsers = _unitOfWork.Repository<UDSUser>().GetByUDSId(udsId).ToList();
            foreach (UDSUser item in oldUsers.Where(f => !usersToManage.Any(c => c.Account == f.Account)))
            {
                item.Repository = repository;
                await _udsUserService.DeleteAsync(item);
            }
            foreach (UDSUser item in usersToManage.Where(f => !oldUsers.Any(c => c.Account == f.Account)))
            {
                await _udsUserService.CreateAsync(item);
            }
        }

        private async Task SynchroniseUDSPECMailAsync(ICollection<UDSPECMail> pecMailsToManage, Guid udsId, UDSRepository repository)
        {
            ICollection<UDSPECMail> oldPECMails = _unitOfWork.Repository<UDSPECMail>().GetByUDSId(udsId).ToList();
            foreach (UDSPECMail item in oldPECMails.Where(f => !pecMailsToManage.Any(c => c.Relation.EntityId == f.Relation.EntityId)))
            {
                item.Repository = repository;
                await _udsPECMailService.DeleteAsync(item);
            }
            foreach (UDSPECMail item in pecMailsToManage.Where(f => !oldPECMails.Any(c => c.Relation.EntityId == f.Relation.EntityId)))
            {
                await _udsPECMailService.CreateAsync(item);
            }
        }

        private async Task SynchroniseUDSMessageAsync(ICollection<UDSMessage> messagesToManage, Guid udsId, UDSRepository repository)
        {
            ICollection<UDSMessage> oldMessages = _unitOfWork.Repository<UDSMessage>().GetByUDSId(udsId).ToList();
            foreach (UDSMessage item in oldMessages.Where(f => !messagesToManage.Any(c => c.Relation.EntityId == f.Relation.EntityId)))
            {
                item.Repository = repository;
                await _udsMessageService.DeleteAsync(item);
            }
            foreach (UDSMessage item in messagesToManage.Where(f => !oldMessages.Any(c => c.Relation.EntityId == f.Relation.EntityId)))
            {
                await _udsMessageService.CreateAsync(item);
            }
        }

        private async Task SynchroniseUDSDocumentUnitAsync(ICollection<UDSDocumentUnit> documentUnitsToManage, Guid udsId, UDSRepository repository)
        {
            ICollection<UDSDocumentUnit> oldDocumentUnits = _unitOfWork.Repository<UDSDocumentUnit>().GetByUDSId(udsId).ToList();
            foreach (UDSDocumentUnit item in oldDocumentUnits.Where(f => !documentUnitsToManage.Any(c => c.Relation.UniqueId == f.Relation.UniqueId)))
            {
                item.Repository = repository;
                await _udsDocumentUnitService.DeleteAsync(item);
            }
            foreach (UDSDocumentUnit item in documentUnitsToManage.Where(f => !oldDocumentUnits.Any(c => c.Relation.UniqueId == f.Relation.UniqueId)))
            {
                await _udsDocumentUnitService.CreateAsync(item);
            }
        }

        private async Task SynchroniseUDSContactAsync(ICollection<UDSContact> contactsToManage, Guid udsId, UDSRepository repository)
        {
            ICollection<UDSContact> oldContacts = _unitOfWork.Repository<UDSContact>().GetByUDSId(udsId).ToList();
            foreach (UDSContact item in oldContacts.Where(f => !contactsToManage.Any(c => (c.ContactType == (short)UDSContactType.Normal && c.Relation != null && f.Relation != null && c.Relation.EntityId == f.Relation.EntityId) || (c.ContactType == (short)UDSContactType.Manual && c.ContactManual == f.ContactManual))))
            {
                item.Repository = repository;
                await _udsContactService.DeleteAsync(item);
            }
            foreach (UDSContact item in contactsToManage.Where(f => !oldContacts.Any(c => (c.ContactType == (short)UDSContactType.Normal && c.Relation != null && f.Relation != null && c.Relation.EntityId == f.Relation.EntityId) || (c.ContactType == (short)UDSContactType.Manual && c.ContactManual == f.ContactManual))))
            {
                await _udsContactService.CreateAsync(item);
            }
        }

        private async Task SynchroniseUDSCollaborationAsync(ICollection<UDSCollaboration> collaborationsToManage, Guid udsId, UDSRepository repository)
        {
            ICollection<UDSCollaboration> oldCollaborations = _unitOfWork.Repository<UDSCollaboration>().GetByUDSId(udsId).ToList();
            foreach (UDSCollaboration item in oldCollaborations.Where(f => !collaborationsToManage.Any(c => c.Relation.EntityId == f.Relation.EntityId)))
            {
                item.Repository = repository;
                await _udsCollaborationService.DeleteAsync(item);
            }
            foreach (UDSCollaboration item in collaborationsToManage.Where(f => !oldCollaborations.Any(c => c.Relation.EntityId == f.Relation.EntityId)))
            {
                await _udsCollaborationService.CreateAsync(item);
            }
        }

        private async Task SynchroniseCollaborationsAsync(SynchronizeCollaborationModel collaborationsModel)
        {
            foreach (Guid idCollaboration in collaborationsModel.IdCollaborations.Distinct())
            {
                Collaboration collaboration = _unitOfWork.Repository<Collaboration>().Query(x => x.UniqueId == idCollaboration)
                    .Include(i => i.CollaborationSigns)
                    .Include(i => i.CollaborationVersionings)
                    .Include(i => i.CollaborationUsers.Select(s => s.Role))
                    .SelectAsQueryable().ToList().FirstOrDefault();

                if (collaboration == null)
                {
                    continue;
                }

                _logger.WriteInfo(new LogMessage($"[SynchroniseCollaboration] Collaboration: {collaboration.EntityId}."), LogCategories);

                if (collaboration.SignCount.HasValue && collaboration.SignCount.Value > 0 && _parameterEnvBaseService.ForceProsecutableEnabled)
                {
                    _logger.WriteInfo(new LogMessage($"[SynchroniseCollaboration] ForceProsecutable is enabled. Removing mandatory signs"), LogCategories);

                    ICollection<CollaborationSign> mandatorySigns = collaboration.CollaborationSigns.Where(x => x.IsActive == true && x.IsRequired.HasValue && x.IsRequired.Value).ToList();
                    foreach (CollaborationSign mandatorySign in mandatorySigns)
                    {
                        mandatorySign.IsRequired = false;
                        await _collaborationSignService.UpdateAsync(mandatorySign);
                    }

                    if (mandatorySigns.Count > 0)
                    {
                        string signUserRemoved = string.Join(", ", mandatorySigns.Select(x => x.SignUser).ToList());

                        await _collaborationLogService.CreateAsync(CollaborationService.CreateLog(collaboration,
                            CollaborationLogType.MODIFICA_SEMPLICE, $"Rimossa obbligatorietà di firma da: {signUserRemoved}.", _currentIdentity.FullUserName));
                    }
                }

                IEnumerable<CollaborationSign> nextSigners = collaboration.CollaborationSigns
                    .Where(x => x.SignDate.HasValue == false && x.IsActive == false && (x.IsAbsent.HasValue == false || x.IsAbsent.Value == false));

                CollaborationSign currentSigner = collaboration.CollaborationSigns.Where(x => x.IsActive == true).FirstOrDefault();

                if (collaboration.SignCount > 1 && nextSigners != null && nextSigners.Count() > 0)
                {
                    _logger.WriteInfo(new LogMessage($"[SynchroniseCollaboration] Collaboration has a next signer. Setting next signer."), LogCategories);

                    currentSigner.IsActive = false;

                    CollaborationSign nextSigner = collaboration.CollaborationSigns.Where(x => x.Incremental == nextSigners.Min(y => y.Incremental)).FirstOrDefault();
                    nextSigner.IsActive = true;

                    await _collaborationSignService.UpdateAsync(currentSigner);
                    await _collaborationSignService.UpdateAsync(nextSigner);

                    await _collaborationLogService.CreateAsync(CollaborationService.CreateLog(collaboration,
                        CollaborationLogType.MODIFICA_SEMPLICE, "Prosegui al destinatario successivo.", _currentIdentity.FullUserName));

                    SendMail(collaboration, CollaborationActionType.DaVisionareFirmare, collaborationsModel.IdTenantAOO, currentSigner, nextSigner);
                }
                else
                {
                    _logger.WriteInfo(new LogMessage($"[SynchroniseCollaboration] Collaboration has no more signers. Changing collaboration status to ToProtocol."), LogCategories);

                    collaboration.IdStatus = CollaborationStatusType.ToProtocol;
                    collaboration.LastChangedUser = _currentIdentity.FullUserName;
                    collaboration.LastChangedDate = DateTime.Now;

                    await _collaborationService.UpdateAsync(collaboration);

                    ICollection<CollaborationSign> updateCollaborationSignsDl = collaboration.CollaborationSigns
                        .Where(x => x.IsActive == true && x.SignUser.Equals(_currentIdentity.FullUserName))
                        .ToList();

                    foreach (CollaborationSign collaborationSign in updateCollaborationSignsDl)
                    {
                        collaborationSign.LastChangedDate = DateTime.Now;
                        collaborationSign.LastChangedUser = _currentIdentity.FullUserName;
                        collaborationSign.IdStatus = CollaborationStatusType.ToRead;
                        await _collaborationSignService.UpdateAsync(collaborationSign);
                    }
                    await _collaborationLogService.CreateAsync(CollaborationService.CreateLog(collaboration,
                        CollaborationLogType.MODIFICA_SEMPLICE, "Avanzamento al protocollo/segreteria.", _currentIdentity.FullUserName));

                    SendMail(collaboration, CollaborationActionType.DaProtocollareGestire, collaborationsModel.IdTenantAOO, currentSigner);
                }
            }
        }

        #endregion

        #endregion
    }
}