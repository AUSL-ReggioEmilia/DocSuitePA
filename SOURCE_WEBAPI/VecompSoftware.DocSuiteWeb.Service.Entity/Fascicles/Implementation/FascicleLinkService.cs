using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles
{
    public class FascicleLinkService : BaseService<FascicleLink>, IFascicleLinkService
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public FascicleLinkService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IFascicleRuleset fascicleRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, fascicleRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        protected override FascicleLink BeforeCreate(FascicleLink entity)
        {
            if (entity.Fascicle != null)
            {
                entity.Fascicle = _unitOfWork.Repository<Fascicle>().Find(entity.Fascicle.UniqueId);
            }

            if (entity.FascicleLinked != null)
            {
                entity.FascicleLinked = _unitOfWork.Repository<Fascicle>().Find(entity.FascicleLinked.UniqueId);
            }

            if (entity.FascicleLinked != null && entity.Fascicle != null)
            {
                FascicleLink simmetricFascicleLink = new FascicleLink()
                {
                    Fascicle = entity.FascicleLinked,
                    FascicleLinked = entity.Fascicle,
                    FascicleLinkType = entity.FascicleLinkType
                };

                _unitOfWork.Repository<FascicleLink>().Insert(simmetricFascicleLink);
            }

            return base.BeforeCreate(entity);
        }

        protected override FascicleLink BeforeDelete(FascicleLink entity, FascicleLink entityTransformed)
        {

            if (entity.Fascicle != null)
            {
                entityTransformed.Fascicle = _unitOfWork.Repository<Fascicle>().Find(entity.Fascicle.UniqueId);
            }

            if (entity.FascicleLinked != null)
            {
                entityTransformed.FascicleLinked = _unitOfWork.Repository<Fascicle>().Find(entity.FascicleLinked.UniqueId);
            }

            if (entity.FascicleLinked != null && entity.Fascicle != null)
            {

                FascicleLink simmetricFascicleLink = _unitOfWork.Repository<FascicleLink>().GetByFascicleLink(entity.FascicleLinked, entity.Fascicle).SingleOrDefault();
                _unitOfWork.Repository<FascicleLink>().Delete(simmetricFascicleLink);
            }

            return base.BeforeDelete(entity, entityTransformed);
        }

        #endregion

    }
}
