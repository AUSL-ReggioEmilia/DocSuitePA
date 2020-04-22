using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Commons
{
    public class CategoryFascicleRightService : BaseService<CategoryFascicleRight>, ICategoryFascicleRightService
    {
        #region [ Fields ]

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IParameterEnvService _parameterEnvService;
        #endregion

        #region [ Constructor ]

        public CategoryFascicleRightService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            ICategoryFascicleRightRuleset categoryFascicleRightRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security,
            IParameterEnvService parameterEnvService)
            : base(unitOfWork, logger, validationService, categoryFascicleRightRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _parameterEnvService = parameterEnvService;
        }

        #endregion

        #region [ Methods ]
        protected override CategoryFascicleRight BeforeCreate(CategoryFascicleRight entity)
        {
            if (entity.CategoryFascicle != null)
            {
                entity.CategoryFascicle = _unitOfWork.Repository<CategoryFascicle>().FindIncludeCategory(entity.CategoryFascicle.UniqueId).SingleOrDefault();
            }

            if (entity.Role != null)
            {
                entity.Role = _unitOfWork.Repository<Role>().Find(entity.Role.EntityShortId);
            }

            if (entity.CategoryFascicle?.FascicleType == FascicleType.Procedure)
            {
                if (_parameterEnvService.FascicleContainerEnabled)
                {
                    //Remove special role if present
                    Role fascicleSpecialRole = GetSpecialRole();
                    CategoryFascicleRight specialRight = _unitOfWork.Repository<CategoryFascicleRight>().GetByProcedureCategoryAndRole(entity.CategoryFascicle.Category.EntityShortId, fascicleSpecialRole.EntityShortId).SingleOrDefault();
                    if (specialRight != null)
                    {
                        _unitOfWork.Repository<CategoryFascicleRight>().Delete(specialRight);
                    }
                    
                    IQueryable<CategoryFascicleRight> toDeleteReferences = _unitOfWork.Repository<CategoryFascicleRight>().GetPeriodicRightsByCategoryAndRole(entity.CategoryFascicle.Category.EntityShortId, fascicleSpecialRole.EntityShortId);
                    _unitOfWork.Repository<CategoryFascicleRight>().DeleteRange(toDeleteReferences.ToList());
                }

                AddRightToPeriodicDefinitions(entity.CategoryFascicle.Category.EntityShortId, entity.Role);                
            }

            return base.BeforeCreate(entity);
        }

        private void AddRightToPeriodicDefinitions(short idCategory, Role role)
        {
            ICollection<CategoryFascicle> periodicCategoryFascicles = _unitOfWork.Repository<CategoryFascicle>().GetAllPeriodicByCategory(idCategory).ToList();
            foreach (CategoryFascicle periodicCategoryFascicle in periodicCategoryFascicles)
            {
                _unitOfWork.Repository<CategoryFascicleRight>().Insert(
                    new CategoryFascicleRight()
                    {
                        CategoryFascicle = periodicCategoryFascicle,
                        Role = role
                    });
            }
        }

        private Role GetSpecialRole()
        {
            return _unitOfWork.Repository<Role>().GetByUniqueId(Guid.Empty).Single();
        }

        protected override IQueryFluent<CategoryFascicleRight> SetEntityIncludeOnDelete(IQueryFluent<CategoryFascicleRight> query)
        {
            return query.Include(i => i.CategoryFascicle.Category)
                .Include(i => i.Role);
        }

        protected override CategoryFascicleRight BeforeDelete(CategoryFascicleRight entity, CategoryFascicleRight entityTransformed)
        {
            if (entityTransformed.CategoryFascicle.FascicleType == FascicleType.Procedure)
            {
                IQueryable<CategoryFascicleRight> toDeleteReferences = _unitOfWork.Repository<CategoryFascicleRight>().GetPeriodicRightsByCategoryAndRole(entityTransformed.CategoryFascicle.Category.EntityShortId, entityTransformed.Role.EntityShortId);
                _unitOfWork.Repository<CategoryFascicleRight>().DeleteRange(toDeleteReferences.ToList());

                if (_parameterEnvService.FascicleContainerEnabled)
                {
                    bool hasRights = _unitOfWork.Repository<CategoryFascicleRight>().GetByProcedureCategoryId(entityTransformed.CategoryFascicle.Category.EntityShortId, true).Any(x => x.UniqueId != entityTransformed.UniqueId);
                    if (!hasRights)
                    {
                        Role fascicleSpecialRole = GetSpecialRole();
                        _unitOfWork.Repository<CategoryFascicleRight>().Insert(
                            new CategoryFascicleRight()
                            {
                                CategoryFascicle = entityTransformed.CategoryFascicle,
                                Role = fascicleSpecialRole
                            });

                        AddRightToPeriodicDefinitions(entityTransformed.CategoryFascicle.Category.EntityShortId, fascicleSpecialRole);
                    }
                }
            }

            return base.BeforeDelete(entity, entityTransformed);
        }
        #endregion
    }
}
