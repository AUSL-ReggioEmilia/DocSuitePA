using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations
{
    public class CollaborationVersioningService : BaseService<CollaborationVersioning>, ICollaborationVersioningService
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public CollaborationVersioningService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            ICollaborationRuleset collaborationRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, collaborationRuleset, mapperUnitOfWork, security)
        {
        }

        #endregion

        #region [ Methods ]

        #endregion

    }
}
