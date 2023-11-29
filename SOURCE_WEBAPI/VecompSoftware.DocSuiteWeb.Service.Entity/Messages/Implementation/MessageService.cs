using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Messages
{
    public class MessageService : BaseService<Message>, IMessageService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IDecryptedParameterEnvService _parameterEnvService;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]

        public MessageService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IMessageRuleset messageRuleset, IMapperUnitOfWork mapperUnitOfWork, IDecryptedParameterEnvService parameterEnvService, ISecurity security)
            : base(unitOfWork, logger, validationService, messageRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _parameterEnvService = parameterEnvService;
        }

        #endregion

        #region [ Methods ]
        protected override Message BeforeCreate(Message entity)
        {
            short messageLocationId = _parameterEnvService.MessageLocationId;
            Location messageLocation = _unitOfWork.Repository<Location>().Find(messageLocationId);
            if (messageLocation != null)
            {
                entity.Location = messageLocation;
            }

            if (entity.MessageAttachments != null && entity.MessageAttachments.Count > 0)
            {
                _unitOfWork.Repository<MessageAttachment>().InsertRange(entity.MessageAttachments);
            }

            if (entity.MessageContacts != null && entity.MessageContacts.Count > 0)
            {
                foreach (MessageContact item in entity.MessageContacts)
                {
                    if (item.MessageContactEmail != null)
                    {
                        _unitOfWork.Repository<MessageContactEmail>().InsertRange(item.MessageContactEmail);
                    }
                }
                _unitOfWork.Repository<MessageContact>().InsertRange(entity.MessageContacts);
            }

            if (entity.MessageEmails != null && entity.MessageEmails.Count > 0)
            {
                _unitOfWork.Repository<MessageEmail>().InsertRange(entity.MessageEmails);
            }

            if (entity.MessageLogs != null && entity.MessageLogs.Count > 0)
            {
                _unitOfWork.Repository<MessageLog>().InsertRange(entity.MessageLogs);
            }
            return base.BeforeCreate(entity);
        }
        #endregion
    }
}
