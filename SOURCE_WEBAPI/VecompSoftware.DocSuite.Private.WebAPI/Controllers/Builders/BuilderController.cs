using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Finder.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Mapper.Model.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.UDS;
using WebApiHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

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
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion

        #region [ Constructor ]
        public BuilderController(ILogger logger, ISecurity security, /*IServiceUnitOfWork serviceUnitOfWork,*/ IDataUnitOfWork unitOfWork, IUDSRoleService udsAuthorizationService, IUDSUserService udsUSerService,
            IUDSPECMailService udsPECMailService, IUDSMessageService udsMessageService, IUDSDocumentUnitService udsDocumentUnitService, IUDSContactService udsContactService, IUDSCollaborationService udsCollaborationService,
            IMapperUnitOfWork mapperUnitOfWork)
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
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
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
        public async Task<IHttpActionResult> Post([FromBody] BuildActionModel model, Guid idRepository)
        {
            return await WebApiHelpers.ActionHelper.TryCatchWithLoggerAsync(async () =>
            {
                UDSRepository repository = _unitOfWork.Repository<UDSRepository>().Find(idRepository);
                if (repository != null && model != null && !string.IsNullOrEmpty(model.Model))
                {
                    UDSRelationModel relationModel = JsonConvert.DeserializeObject<UDSRelationModel>(model.Model);
                    ICollection<UDSRole> rolesToManage = GetEntityRoles(relationModel.Roles, repository);
                    ICollection<UDSUser> usersToManage = GetEntityUsers(relationModel.Users, repository);
                    ICollection<UDSPECMail> pecMailsToManage = GetEntityPECMails(relationModel.PECMails, repository);
                    ICollection<UDSMessage> messagesToManage = GetEntityMessages(relationModel.Messages, repository);
                    ICollection<UDSDocumentUnit> documentUnitsToManage = GetEntityDocumentUnits(relationModel.DocumentUnits, repository);
                    ICollection<UDSContact> contactsToManage = GetEntityContacts(relationModel.Contacts, repository);
                    ICollection<UDSCollaboration> collaborationsToManage = GetEntityCollaborations(relationModel.Collaborations, repository);

                    if (model.BuildType == BuildActionType.Build)
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
                    }

                    if (model.BuildType == BuildActionType.Synchronize)
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
                    }
                }
                return Ok(model);

            }, BadRequest, Content, InternalServerError, _logger, LogCategories);
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
                await WebApiHelpers.ActionHelper.RetryingPolicyActionAsync<Task>(async ()=> { await _udsDocumentUnitService.CreateAsync(item); }, _logger, LogCategories);
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
        #endregion

        #endregion
    }
}