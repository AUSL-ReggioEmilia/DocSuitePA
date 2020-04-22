using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers
{
    public class DossierFolderRoleService : BaseDossierService<DossierFolderRole>, IDossierFolderRoleService
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public DossierFolderRoleService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IDossierRuleset dossierRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, dossierRuleset, mapperUnitOfWork, security)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}