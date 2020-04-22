using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Commons
{
    public class ContainerPropertyService : BaseService<ContainerProperty>, IContainerPropertyService
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public ContainerPropertyService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IContainerRuleset containerRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, containerRuleset, mapperUnitOfWork, security)
        {

        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}
