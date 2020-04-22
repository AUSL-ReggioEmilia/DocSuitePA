using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles
{
    public class FascicleDocumentService : BaseService<FascicleDocument>, IFascicleDocumentService
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public FascicleDocumentService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IFascicleRuleset fascicleRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, fascicleRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        protected override FascicleDocument BeforeCreate(FascicleDocument entity)
        {
            if (entity.Fascicle != null)
            {
                entity.Fascicle = _unitOfWork.Repository<Fascicle>().Find(entity.Fascicle.UniqueId);
            }

            _unitOfWork.Repository<FascicleLog>().Insert(FascicleService.CreateLog(entity.Fascicle, FascicleLogType.Modify, $"Modifica sulla tipologia documento (-{entity.ChainType}-{entity.IdArchiveChain}) del fascicolo", CurrentDomainUser.Account));

            return base.BeforeCreate(entity);
        }

        protected override FascicleDocument AfterUpdate(FascicleDocument entity, FascicleDocument entityTransformed)
        {
            if (entityTransformed != null)
            {
                DocSuite.Document.BiblosDS.DocumentBiblosDS documentBiblosDS = new DocSuite.Document.BiblosDS.DocumentBiblosDS(_logger);
                if (!documentBiblosDS.HasActiveDocuments(entityTransformed.IdArchiveChain))
                {
                    _unitOfWork.Repository<FascicleDocument>().Delete(entityTransformed);
                }
            }
            return base.AfterUpdate(entity, entityTransformed);
        }

        #endregion

    }
}
