using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Resolutions
{
    public class ResolutionKindService : BaseService<ResolutionKind>, IResolutionKindService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public ResolutionKindService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IResolutionRuleset resolutionRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, resolutionRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        protected override ResolutionKind BeforeCreate(ResolutionKind entity)
        {
            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entity.UniqueId, null, TableLogEvent.INSERT, string.Concat("Inserimento tipologia atto ", entity.Name), typeof(ResolutionKind).Name, CurrentDomainUser.Account));

            return base.BeforeCreate(entity);
        }

        protected override ResolutionKind BeforeUpdate(ResolutionKind entity, ResolutionKind entityTransformed)
        {
            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.UPDATE, string.Concat("Modificata tipologia atto ", entity.Name, " con ID ", entity.UniqueId), typeof(ResolutionKind).Name, CurrentDomainUser.Account));

            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override IQueryFluent<ResolutionKind> SetEntityIncludeOnDelete(IQueryFluent<ResolutionKind> query)
        {
            return query.Include(x => x.ResolutionKindDocumentSeries);
        }

        protected override ResolutionKind BeforeDelete(ResolutionKind entity, ResolutionKind entityTransformed)
        {
            if (entityTransformed.ResolutionKindDocumentSeries != null)
            {
                _unitOfWork.Repository<ResolutionKindDocumentSeries>().DeleteRange(entityTransformed.ResolutionKindDocumentSeries.ToList());
            }
            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.DELETE, string.Concat("Eliminata tipologia atto ", entity.Name), typeof(ResolutionKind).Name, CurrentDomainUser.Account));

            return base.BeforeDelete(entity, entityTransformed);
        }
        #endregion
    }
}
