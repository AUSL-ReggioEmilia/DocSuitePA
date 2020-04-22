using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Messages
{
    public class MessageContactEmailService : BaseService<MessageContactEmail>, IMessageContactEmailService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public MessageContactEmailService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService, IMessageRuleset messageRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, messageRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        #endregion

    }
}
