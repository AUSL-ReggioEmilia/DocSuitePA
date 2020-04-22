using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers
{
    public class DossierDocumentService : BaseDossierService<DossierDocument>, IDossierDocumentService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]

        public DossierDocumentService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IDossierRuleset dossierRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, dossierRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        protected override DossierDocument BeforeCreate(DossierDocument entity)
        {
            if (entity.Dossier != null)
            {
                entity.Dossier = _unitOfWork.Repository<Dossier>().Find(entity.Dossier.UniqueId);
            }

            _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entity.Dossier, null, DossierLogType.Modify, "Modifica del Dossier", CurrentDomainUser.Account));

            return base.BeforeCreate(entity);
        }
        #endregion
    }
}