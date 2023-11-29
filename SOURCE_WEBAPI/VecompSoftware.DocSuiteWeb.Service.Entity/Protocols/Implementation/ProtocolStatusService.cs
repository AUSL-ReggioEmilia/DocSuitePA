using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Validation;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Protocols.Implementation
{
    public class ProtocolStatusService : BaseService<ProtocolStatus>, IProtocolStatusService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public ProtocolStatusService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IProtocolRuleset protocolRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, protocolRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        #endregion
    }
}
