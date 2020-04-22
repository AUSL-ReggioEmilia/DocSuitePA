using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Resolutions
{
    public class ResolutionKindDocumentSeriesService : BaseService<ResolutionKindDocumentSeries>, IResolutionKindDocumentSeriesService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public ResolutionKindDocumentSeriesService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IResolutionRuleset resolutionRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, resolutionRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        protected override ResolutionKindDocumentSeries BeforeCreate(ResolutionKindDocumentSeries entity)
        {
            if (entity.DocumentSeriesConstraint != null)
            {
                entity.DocumentSeriesConstraint = _unitOfWork.Repository<DocumentSeriesConstraint>().Find(entity.DocumentSeriesConstraint.UniqueId);
            }

            if (entity.DocumentSeries != null)
            {
                entity.DocumentSeries = _unitOfWork.Repository<DocumentSeries>().Find(entity.DocumentSeries.EntityId);
            }

            if (entity.ResolutionKind != null)
            {
                entity.ResolutionKind = _unitOfWork.Repository<ResolutionKind>().Find(entity.ResolutionKind.UniqueId);
            }

            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<ResolutionKindDocumentSeries> SetEntityIncludeOnUpdate(IQueryFluent<ResolutionKindDocumentSeries> query)
        {
            return query.Include(x => x.DocumentSeries)
                    .Include(x => x.ResolutionKind)
                    .Include(x => x.DocumentSeriesConstraint);
        }

        protected override ResolutionKindDocumentSeries BeforeUpdate(ResolutionKindDocumentSeries entity, ResolutionKindDocumentSeries entityTransformed)
        {
            entityTransformed.DocumentSeriesConstraint = null;
            if (entity.DocumentSeriesConstraint != null)
            {
                entityTransformed.DocumentSeriesConstraint = _unitOfWork.Repository<DocumentSeriesConstraint>().Find(entity.DocumentSeriesConstraint.UniqueId);
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion
    }
}
