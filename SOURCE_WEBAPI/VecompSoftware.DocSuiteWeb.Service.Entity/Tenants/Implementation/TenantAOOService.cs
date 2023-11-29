using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Parameters;
using VecompSoftware.DocSuiteWeb.Finder.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Tenants
{
    public class TenantAOOService : BaseService<TenantAOO>, ITenantAOOService
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        #endregion

        #region [ Constructor ]
        public TenantAOOService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            ITenantAOORuleset tenantAOORuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security) 
            : base(unitOfWork, logger, validationService, tenantAOORuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        #endregion

        #region [ Methods ]
        protected override TenantAOO BeforeCreate(TenantAOO entity)
        {
            if (entity.TenantTypology == TenantTypologyType.InternalTenant)
            {
                Parameter parameter = CreateTenantAOOParameter(entity);
                _unitOfWork.Repository<Parameter>().Insert(parameter);

                Category defaultCategory = new Category
                {
                    Name = entity.Name,
                    Code = 0,
                    StartDate = DateTimeOffset.UtcNow.Date,
                    IsActive = true
                };

                defaultCategory.CategorySchema = _unitOfWork.Repository<CategorySchema>().FindActiveCategorySchema();
                _unitOfWork.Repository<Category>().Insert(defaultCategory);
                entity.Categories.Add(defaultCategory);
                _logger.WriteDebug(new LogMessage($"Generated automatically new category {defaultCategory.Name}({defaultCategory.UniqueId}) to tenantaoo {entity.Name} ({entity.UniqueId})"), LogCategories);
                _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entity.UniqueId, null, TableLogEvent.INSERT, $"Inserita nuova AOO {entity.Name}", typeof(TenantAOO).Name, CurrentDomainUser.Account));
            }
            return base.BeforeCreate(entity);
        }

        /// <summary>
        /// TODO: La creazione di una nuova Parameter non influisce sul funzionamento del modulo Atti
        /// che non è soggetto alla gestione del MultiTenant
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private Parameter CreateTenantAOOParameter(TenantAOO entity)
        {
            int lastIdResolution = _unitOfWork.Repository<Resolution>().LastIdResolution();
            short lastResolutionYear = _unitOfWork.Repository<Resolution>().LastResolutionYear();
            int lastResolutionNumber = _unitOfWork.Repository<Resolution>().LastResolutionNumber(lastResolutionYear);
            int lastResolutionBillNumber = _unitOfWork.Repository<Resolution>().LastResolutionBillNumber(lastResolutionYear);

            Parameter toDuplicateParameter = _unitOfWork.Repository<Parameter>().GetParameters().OrderByDescending(o => o.Incremental).First();

            Parameter parameter = _mapperUnitOfWork.Repository<IDomainMapper<Parameter, Parameter>>().Map(toDuplicateParameter, new Parameter());
            parameter.LastUsedIdCategory++;
            parameter.LastUsedNumber = 0;
            parameter.Locked = false;
            parameter.LastUsedResolutionYear = lastResolutionYear;
            parameter.LastUsedResolutionNumber = Convert.ToInt16(lastResolutionNumber + 1);
            parameter.LastUsedBillNumber = Convert.ToInt16(lastResolutionBillNumber + 1);
            
            parameter.TenantAOO = entity;

            return parameter;
        }
        #endregion
    }
}
