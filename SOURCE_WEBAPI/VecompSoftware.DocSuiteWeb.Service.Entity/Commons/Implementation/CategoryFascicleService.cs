using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Commons
{
    public class CategoryFascicleService : BaseService<CategoryFascicle>, ICategoryFascicleService
    {
        #region [ Fields ]

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IParameterEnvService _parameterEnvService;
        #endregion

        #region [ Constructor ]

        public CategoryFascicleService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            ICategoryFascicleRuleset categoryFascicleRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security,
            IParameterEnvService parameterEnvService)
            : base(unitOfWork, logger, validationService, categoryFascicleRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _parameterEnvService = parameterEnvService;
        }

        #endregion

        #region [ Methods ]

        protected override CategoryFascicle BeforeCreate(CategoryFascicle entity)
        {
            if (entity.FasciclePeriod != null)
            {
                entity.FasciclePeriod = _unitOfWork.Repository<FasciclePeriod>().Find(entity.FasciclePeriod.UniqueId);
            }

            if (entity.Category != null)
            {
                entity.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            if (entity.Manager != null)
            {
                entity.Manager = _unitOfWork.Repository<Contact>().Find(entity.Manager.EntityId);
            }

            if (entity.FascicleType == FascicleType.Period)
            {
                entity.CategoryFascicleRights = _unitOfWork.Repository<CategoryFascicleRight>().GetByProcedureCategoryId(entity.Category.EntityShortId, true).ToList();
            }

            if (entity.FascicleType == FascicleType.Period || entity.FascicleType == FascicleType.Procedure)
            {
                HashSet<CategoryFascicleRight> categoryFascicleRights = new HashSet<CategoryFascicleRight>();
                if (entity.CategoryFascicleRights?.Count > 0)
                {
                    CategoryFascicleRight tmpFascicleRight;
                    foreach (CategoryFascicleRight categoryFascicleRight in entity.CategoryFascicleRights)
                    {
                        tmpFascicleRight = new CategoryFascicleRight()
                        {                            
                            Role = _unitOfWork.Repository<Role>().Find(categoryFascicleRight.Role.EntityShortId)
                        };
                        categoryFascicleRights.Add(tmpFascicleRight);
                    }
                }

                if ((entity.CategoryFascicleRights == null || entity.CategoryFascicleRights.Count == 0) && _parameterEnvService.FascicleContainerEnabled)
                {
                    entity.CategoryFascicleRights = new List<CategoryFascicleRight>();
                    CategoryFascicleRight specialRight = new CategoryFascicleRight()
                    {
                        CategoryFascicle = entity,
                        Role = _unitOfWork.Repository<Role>().GetByUniqueId(Guid.Empty).Single()
                    };
                    _unitOfWork.Repository<CategoryFascicleRight>().Insert(specialRight);
                    categoryFascicleRights.Add(specialRight);
                }
                entity.CategoryFascicleRights = categoryFascicleRights.ToList();
                _unitOfWork.Repository<CategoryFascicleRight>().InsertRange(entity.CategoryFascicleRights);

                if (entity.CategoryFascicleRights?.Count > 0 && entity.FascicleType == FascicleType.Procedure)
                {
                    ICollection<CategoryFascicle> periodicCategoryFascicleReferences = _unitOfWork.Repository<CategoryFascicle>().GetAllPeriodicByCategory(entity.Category.EntityShortId).ToList();
                    CategoryFascicleRight tmpFascicleRight;
                    foreach (CategoryFascicle periodicCategoryFascicle in periodicCategoryFascicleReferences)
                    {
                        foreach (CategoryFascicleRight fascicleRight in entity.CategoryFascicleRights.Where(x => !periodicCategoryFascicle.CategoryFascicleRights.Any(xx => x.Role.EntityShortId == xx.Role.EntityShortId)))
                        {
                            tmpFascicleRight = new CategoryFascicleRight()
                            {
                                CategoryFascicle = periodicCategoryFascicle,
                                Role = _unitOfWork.Repository<Role>().Find(fascicleRight.Role.EntityShortId)
                            };
                            _unitOfWork.Repository<CategoryFascicleRight>().Insert(tmpFascicleRight);
                        }
                    }
                }
            }

            return base.BeforeCreate(entity);
        }

        protected override CategoryFascicle BeforeUpdate(CategoryFascicle entity, CategoryFascicle entityTransformed)
        {
            entityTransformed.FasciclePeriod = null;

            if (entity.FasciclePeriod != null)
            {
                entityTransformed.FasciclePeriod = _unitOfWork.Repository<FasciclePeriod>().Find(entity.FasciclePeriod.UniqueId);
            }

            if (entity.Category != null)
            {
                entityTransformed.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            entityTransformed.Manager = null;
            if (entity.Manager != null)
            {
                entityTransformed.Manager = _unitOfWork.Repository<Contact>().Find(entity.Manager.EntityId);
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override IQueryFluent<CategoryFascicle> SetEntityIncludeOnUpdate(IQueryFluent<CategoryFascicle> query)
        {
            query.Include(c => c.Category);
            return query;
        }

        protected override IQueryFluent<CategoryFascicle> SetEntityIncludeOnDelete(IQueryFluent<CategoryFascicle> query)
        {
            query.Include(c => c.Category)
                .Include(c => c.CategoryFascicleRights);
            return query;
        }

        protected override CategoryFascicle BeforeDelete(CategoryFascicle entity, CategoryFascicle entityTransformed)
        {
            if (CurrentDeleteActionType != Common.Infrastructures.DeleteActionType.DeleteCategoryFascicle)
            {
                throw new DSWException(EXCEPTION_MESSAGE, null, DSWExceptionCode.SS_NotAllowedOperation);
            }

            if (entityTransformed.FascicleType == FascicleType.Procedure)
            {
                ICollection<CategoryFascicleTableValuedModel> subCategoryFascicles = _unitOfWork.Repository<CategoryFascicle>().GetCategorySubFascicles(entityTransformed.Category.EntityShortId);
                CategoryFascicle categoryFascicle;
                foreach (CategoryFascicleTableValuedModel toDelete in subCategoryFascicles)
                {
                    categoryFascicle = _unitOfWork.Repository<CategoryFascicle>().FindIncludeRights(toDelete.IdCategoryFascicle).Single();
                    _unitOfWork.Repository<CategoryFascicleRight>().DeleteRange(categoryFascicle.CategoryFascicleRights.ToList());
                    _unitOfWork.Repository<CategoryFascicle>().Delete(categoryFascicle);
                }

                ICollection<CategoryFascicle> periodicCategoryFascicleReferences = _unitOfWork.Repository<CategoryFascicle>().GetAllPeriodicByCategory(entityTransformed.Category.EntityShortId).ToList();
                CategoryFascicleRight specialRight;
                foreach (CategoryFascicle periodicCategoryFascicleReference in periodicCategoryFascicleReferences)
                {
                    _unitOfWork.Repository<CategoryFascicleRight>().DeleteRange(periodicCategoryFascicleReference.CategoryFascicleRights.ToList());
                    if (_parameterEnvService.FascicleContainerEnabled)
                    {
                        specialRight = new CategoryFascicleRight()
                        {
                            CategoryFascicle = periodicCategoryFascicleReference,
                            Role = _unitOfWork.Repository<Role>().GetByUniqueId(Guid.Empty).Single()
                        };
                        _unitOfWork.Repository<CategoryFascicleRight>().Insert(specialRight);
                    }                    
                }
            }

            if (entityTransformed.CategoryFascicleRights != null && entityTransformed.CategoryFascicleRights.Count > 0)
            {
                _unitOfWork.Repository<CategoryFascicleRight>().DeleteRange(entityTransformed.CategoryFascicleRights.ToList());
            }

            TableLog tableLog = TableLogService.CreateLog(entity.UniqueId, null, TableLogEvent.DELETE, $"Eliminato piano di fascicolazione di tipo {entityTransformed.FascicleType.ToString()} con environment {entityTransformed.DSWEnvironment}", nameof(CategoryFascicle), CurrentDomainUser.Account);
            _unitOfWork.Repository<TableLog>().Insert(tableLog);

            return base.BeforeDelete(entity, entityTransformed);
        }
        #endregion

    }
}