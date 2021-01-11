using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;

namespace VecompSoftware.DocSuiteWeb.Service.Entity
{
    [LogCategory(LogCategoryDefinition.SERVICEENTITY)]
    public abstract class BaseService<TEntity> : IEntityBaseService<TEntity>
        where TEntity : DSWBaseEntity, new()
    {
        #region [ Fields ]
        private readonly IRepositoryAsync<TEntity> _repositoryAsync;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        private readonly ILogger _logger;
        private readonly IValidatorService _validationService;
        private readonly Guid _instanceId;
        private readonly IValidatorRuleset _validatorRuleset;
        private readonly IDomainMapper<TEntity, TEntity> _mapper;
        private UpdateActionType? _updateActionType = null;
        private InsertActionType? _insertActionType = null;
        private DeleteActionType? _deleteActionType = null;
        private static IEnumerable<LogCategory> _logCategories = null;
        private bool _disposed;
        public const string EXCEPTION_MESSAGE = "Operation not allowed.";
        private readonly DomainUserModel _currentDomainUser;

        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            MaxDepth = 10,
        };
        #endregion

        #region [ Properties ]
        public static JsonSerializerSettings DefaultJsonSerializer => _serializerSettings;

        protected DomainUserModel CurrentDomainUser => _currentDomainUser;

        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(BaseService<>));
                }
                return _logCategories;
            }
        }
        protected UpdateActionType? CurrentUpdateActionType => _updateActionType;
        protected InsertActionType? CurrentInsertActionType => _insertActionType;
        protected DeleteActionType? CurrentDeleteActionType => _deleteActionType;
        #endregion

        #region [ Constructor ]

        protected BaseService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IValidatorRuleset validatorRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
        {
            _unitOfWork = unitOfWork;
            _mapperUnitOfWork = mapperUnitOfWork;
            _repositoryAsync = unitOfWork.Repository<TEntity>();
            _logger = logger;
            _validationService = validationService;
            _instanceId = Guid.NewGuid();
            _validatorRuleset = validatorRuleset;
            _mapper = mapperUnitOfWork.Repository<IDomainMapper<TEntity, TEntity>>();
            _currentDomainUser = security.GetCurrentUser();
        }

        #endregion Constructor

        #region [ Dispose ]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _unitOfWork.Dispose();
            }
            _disposed = true;
        }
        #endregion

        #region [ Methods ]

        #region [ Sync Methods ]
        public virtual TEntity Create(TEntity entity)
        {
            return CreateAsync(entity).Result;
        }

        public virtual TEntity Update(TEntity entity)
        {
            return UpdateAsync(entity).Result;
        }

        public virtual bool Delete(TEntity entity)
        {
            return DeleteAsync(entity).Result;
        }

        public IQueryable<TEntity> Queryable(bool optimization = false)
        {
            return _repositoryAsync.Queryable(optimization);
        }

        #endregion

        #region [ Async Methods ]
        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            return await CreateAsync(entity, _validatorRuleset.INSERT);
        }
        public virtual async Task<TEntity> CreateAsync(TEntity entity, InsertActionType actionType)
        {
            _insertActionType = actionType;
            string ruleset = EnumHelper.GetDescription(actionType);
            if (string.IsNullOrEmpty(ruleset))
            {
                throw new DSWException($"ServiceCreate: Invalid ActionType {actionType}", null, DSWExceptionCode.VA_RulesetValidation);
            }
            return await CreateAsync(entity, ruleset);
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity, string ruleset)
        {
            return await ServiceHelper.TryCatchWithLogger(async () =>
            {
                await Task.Delay(1);
                entity = BeforeCreate(entity);
                _validationService.Validate(entity, ruleset);
                _repositoryAsync.Insert(entity);
                entity = AfterCreate(entity);
                return entity;
            }, _logger, LogCategories);
        }

        private async Task<TEntity> UpdateAsync(TEntity entity, string ruleset)
        {
            return await ServiceHelper.TryCatchWithLogger(async () =>
            {
                await Task.Delay(1);
                TEntity entityTransformed = SetEntityIncludeOnUpdate(_repositoryAsync.Query(f => f.UniqueId == entity.UniqueId)).Select().Single();
                if (ExecuteMappingBeforeUpdate())
                {
                    entityTransformed = _mapper.Map(entity, entityTransformed);
                }
                entityTransformed = BeforeUpdate(entity, entityTransformed);
                _validationService.Validate(entityTransformed, ruleset);
                _repositoryAsync.Update(entityTransformed);
                entityTransformed = AfterUpdate(entity, entityTransformed);
                return entityTransformed;
            }, _logger, LogCategories);
        }
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            return await UpdateAsync(entity, _validatorRuleset.UPDATE);
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, UpdateActionType actionType)
        {
            _updateActionType = actionType;
            string ruleset = EnumHelper.GetDescription(actionType);
            if (string.IsNullOrEmpty(ruleset))
            {
                throw new DSWException(string.Concat("ServiceUpdate: Invalid ActionType ", actionType), null, DSWExceptionCode.VA_RulesetValidation);
            }
            return await UpdateAsync(entity, ruleset);
        }

        private async Task<bool> DeleteAsync(TEntity entity, string ruleset)
        {
            return await ServiceHelper.TryCatchWithLogger(async () =>
            {
                await Task.Delay(1);
                TEntity entityTransformed = (SetEntityIncludeOnDelete(_repositoryAsync.Query(f => f.UniqueId == entity.UniqueId)).Select()).SingleOrDefault();
                entityTransformed = BeforeDelete(entity, entityTransformed);
                _validationService.Validate(entityTransformed, ruleset);
                if (ExecuteDelete())
                {
                    _repositoryAsync.Delete(entityTransformed);
                }
                entityTransformed = AfterDelete(entityTransformed);
                return true;
            }, _logger, LogCategories);
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity)
        {
            return await DeleteAsync(entity, _validatorRuleset.DELETE);
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity, DeleteActionType actionType)
        {
            _deleteActionType = actionType;
            string ruleset = EnumHelper.GetDescription(actionType);
            if (string.IsNullOrEmpty(ruleset))
            {
                throw new DSWException(string.Concat("ServiceDelete: Invalid ActionType ", actionType), null, DSWExceptionCode.VA_RulesetValidation);
            }
            return await DeleteAsync(entity, ruleset);
        }

        #endregion

        #region [ TriggerEntity ]
        protected virtual IQueryFluent<TEntity> SetEntityIncludeOnUpdate(IQueryFluent<TEntity> query)
        {
            return query;
        }

        protected virtual IQueryFluent<TEntity> SetEntityIncludeOnDelete(IQueryFluent<TEntity> query)
        {
            return query;
        }

        protected virtual TEntity BeforeCreate(TEntity entity)
        {
            return entity;
        }

        protected virtual TEntity AfterCreate(TEntity entity)
        {
            return entity;
        }

        protected virtual bool ExecuteMappingBeforeUpdate()
        {
            return true;
        }

        protected virtual bool ExecuteDelete()
        {
            return true;
        }

        protected virtual TEntity BeforeUpdate(TEntity entity, TEntity entityTransformed)
        {
            return entityTransformed;
        }

        protected virtual TEntity AfterUpdate(TEntity entity, TEntity entityTransformed)
        {
            return entityTransformed;
        }

        protected virtual TEntity BeforeDelete(TEntity entity, TEntity entityTransformed)
        {
            return entityTransformed;
        }

        protected virtual TEntity AfterDelete(TEntity entity)
        {
            return entity;
        }

        #endregion

        #endregion
    }
}