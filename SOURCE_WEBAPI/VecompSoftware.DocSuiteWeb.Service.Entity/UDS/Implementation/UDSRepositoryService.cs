using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Finder.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.UDS
{
    public class UDSRepositoryService : BaseService<UDSRepository>, IUDSRepositoryService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private const string SCHEMA_REPOSITORY_NOT_FOUND_MESSAGE = "Nessun schema repository trovato.";
        #endregion

        #region [ Constructor ]
        public UDSRepositoryService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IUDSRuleset udsRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, udsRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        protected override UDSRepository BeforeCreate(UDSRepository entity)
        {
            entity.SequenceCurrentYear = 0;
            entity.SequenceCurrentNumber = 0;
            entity.ActiveDate = DateTimeOffset.UtcNow;
            entity.Status = UDSRepositoryStatus.Draft;
            UDSRepository existingRepository = _unitOfWork.Repository<UDSRepository>().GetByName(entity.Name);
            if (existingRepository != null)
            {
                entity.DSWEnvironment = existingRepository.DSWEnvironment;
            }
            else
            {
                int maxEnvironment = _unitOfWork.Repository<UDSRepository>().GetMaxEnvironment().Select(s => s.DSWEnvironment).FirstOrDefault();
                entity.DSWEnvironment = (maxEnvironment == default(int)) ? 100 : maxEnvironment + 1;
            }
            entity.SchemaRepository = _unitOfWork.Repository<UDSSchemaRepository>().GetCurrentSchema().SingleOrDefault();
            if (entity.SchemaRepository == null)
            {
                throw new DSWException(SCHEMA_REPOSITORY_NOT_FOUND_MESSAGE, null, DSWExceptionCode.DB_Anomaly);
            }

            if (entity.Container != null)
            {
                entity.Container = _unitOfWork.Repository<Container>().Find(entity.Container.EntityShortId);
            }
            return entity;
        }

        protected override UDSRepository BeforeUpdate(UDSRepository entity, UDSRepository entityTransformed)
        {

            if (entity.Container != null)
            {
                entityTransformed.Container = _unitOfWork.Repository<Container>().Find(entity.Container.EntityShortId);
            }

            UDSRepository oldVersion = _unitOfWork.Repository<UDSRepository>().GetPreviousVersionWithUDSTypologies(entity);

            if (oldVersion != null && entity.ExpiredDate == null && entity.Status == UDSRepositoryStatus.Confirmed)
            {
                foreach (UDSTypology tipology in oldVersion.UDSTypologies)
                {
                    entityTransformed.UDSTypologies.Add(_unitOfWork.Repository<UDSTypology>().Find(tipology.UniqueId));
                }

                oldVersion.UDSTypologies.Clear();
            }

            return entityTransformed;
        }

        protected override IQueryFluent<UDSRepository> SetEntityIncludeOnUpdate(IQueryFluent<UDSRepository> query)
        {
            query.Include(x => x.UDSTypologies);
            return query;
        }

        #endregion

    }
}
