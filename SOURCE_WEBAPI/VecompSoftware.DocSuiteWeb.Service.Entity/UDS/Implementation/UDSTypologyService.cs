using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.UDS
{
    public class UDSTypologyService : BaseService<UDSTypology>, IUDSTypologyService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]
        public UDSTypologyService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IUDSRuleset udsRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, udsRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        protected override UDSTypology BeforeCreate(UDSTypology entity)
        {
            entity.Status = UDSTypologyStatus.Active;
            return base.BeforeCreate(entity);
        }

        protected override UDSTypology BeforeUpdate(UDSTypology entity, UDSTypology entityTransformed)
        {
            if (entity.Name != null)
            {
                entityTransformed.Name = entity.Name;
            }

            if (entity.UDSRepositories != null)
            {
                foreach (UDSRepository item in entityTransformed.UDSRepositories.Where(f => !entity.UDSRepositories.Any(u => u.UniqueId == f.UniqueId)).ToList())
                {
                    entityTransformed.UDSRepositories.Remove(item);
                }
                foreach (UDSRepository item in entity.UDSRepositories.Where(f => !entityTransformed.UDSRepositories.Any(c => c.UniqueId == f.UniqueId)))
                {
                    entityTransformed.UDSRepositories.Add(_unitOfWork.Repository<UDSRepository>().Find(item.UniqueId));
                }
            }
            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override IQueryFluent<UDSTypology> SetEntityIncludeOnUpdate(IQueryFluent<UDSTypology> query)
        {
            query.Include(t => t.UDSRepositories);
            return query;
        }

        #endregion

    }
}
