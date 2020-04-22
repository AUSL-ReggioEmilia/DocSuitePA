using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.DocumentArchives
{
    public class DocumentSeriesConstraintService : BaseService<DocumentSeriesConstraint>, IDocumentSeriesConstraintService
    {
        #region [ Fields ]
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public DocumentSeriesConstraintService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IDocumentSeriesRuleset documentSeriesRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, documentSeriesRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region [ Methods ]
        protected override DocumentSeriesConstraint BeforeCreate(DocumentSeriesConstraint entity)
        {
            if (entity.DocumentSeries != null)
            {
                entity.DocumentSeries = _unitOfWork.Repository<DocumentSeries>().Find(entity.DocumentSeries.EntityId);
            }

            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<DocumentSeriesConstraint> SetEntityIncludeOnUpdate(IQueryFluent<DocumentSeriesConstraint> query)
        {
            query.Include(x => x.DocumentSeries);
            return query;
        }
        #endregion
    }
}
