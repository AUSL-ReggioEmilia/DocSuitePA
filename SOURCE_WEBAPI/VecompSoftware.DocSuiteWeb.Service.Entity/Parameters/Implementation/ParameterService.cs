using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Parameters;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Parameters
{
    public class ParameterService : BaseService<Parameter>, IParameterService
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region Constructor
        public ParameterService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
                    IParameterRuleset parameterRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, parameterRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        protected override Parameter BeforeCreate(Parameter entity)
        {
            return base.BeforeCreate(entity);
        }

        protected override Parameter BeforeUpdate(Parameter entity, Parameter entityTransformed)
        {
            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion
    }
}
