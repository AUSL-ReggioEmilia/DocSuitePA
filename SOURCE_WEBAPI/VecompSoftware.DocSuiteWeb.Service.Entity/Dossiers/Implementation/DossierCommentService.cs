using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers
{
    public class DossierCommentService : BaseDossierService<DossierComment>, IDossierCommentService
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public DossierCommentService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationservice,
            IDossierRuleset dossierRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationservice, dossierRuleset, mapperUnitOfWork, security)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
