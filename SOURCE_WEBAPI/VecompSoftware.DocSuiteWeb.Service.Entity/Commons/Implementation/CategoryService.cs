using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Commons
{
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        #region [ Fields ]

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public CategoryService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            ICategoryRuleset categoryRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, categoryRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        protected override Category BeforeCreate(Category entity)
        {
            if (CurrentInsertActionType != Common.Infrastructures.InsertActionType.InsertCategory)
            {
                throw new DSWException(EXCEPTION_MESSAGE, null, DSWExceptionCode.SS_NotAllowedOperation);
            }

            if (entity.MetadataRepository != null)
            {
                entity.MetadataRepository = _unitOfWork.Repository<MetadataRepository>().Find(entity.MetadataRepository.UniqueId);
            }
            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<Category> SetEntityIncludeOnUpdate(IQueryFluent<Category> query)
        {
            return query.Include(i => i.MetadataRepository);
        }

        protected override Category BeforeUpdate(Category entity, Category entityTransformed)
        {
            if (CurrentUpdateActionType != Common.Infrastructures.UpdateActionType.UpdateCategory)
            {
                throw new DSWException(EXCEPTION_MESSAGE, null, DSWExceptionCode.SS_NotAllowedOperation);
            }

            entityTransformed.MetadataRepository = null;
            if (entity.MetadataRepository != null)
            {
                entityTransformed.MetadataRepository = _unitOfWork.Repository<MetadataRepository>().Find(entity.MetadataRepository.UniqueId);
            }

            if (entity.MassimarioScarto != null)
            {
                entityTransformed.MassimarioScarto = _unitOfWork.Repository<MassimarioScarto>().Find(entity.MassimarioScarto.UniqueId);
            }
            if (entity.CategorySchema != null)
            {
                entityTransformed.CategorySchema = _unitOfWork.Repository<CategorySchema>().Find(entity.CategorySchema.UniqueId);
            }
            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override Category BeforeDelete(Category entity, Category entityTransformed)
        {
            if (CurrentDeleteActionType != Common.Infrastructures.DeleteActionType.DeleteCategory)
            {
                throw new DSWException(EXCEPTION_MESSAGE, null, DSWExceptionCode.SS_NotAllowedOperation);
            }
            return base.BeforeDelete(entity, entityTransformed);
        }
        #endregion
    }
}