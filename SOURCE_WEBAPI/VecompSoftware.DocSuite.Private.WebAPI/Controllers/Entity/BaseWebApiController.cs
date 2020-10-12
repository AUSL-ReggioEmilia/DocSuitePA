using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Service.Entity;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity
{
    [Authorize]
    [LogCategory(LogCategoryDefinition.WEBAPIENTITY)]
    public abstract class BaseWebApiController<TEntity, TService> : ApiController, IWebApiController<TEntity, TService>
        where TEntity : DSWBaseEntity, new()
        where TService : IEntityBaseService<TEntity>
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories = null;
        private readonly TService _service;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly Guid _instanceId;

        private InsertActionType? _insertActionType = null;
        private UpdateActionType? _updateActionType = null;
        private DeleteActionType? _deleteActionType = null;
        #endregion

        #region [ Properties ]
        protected virtual IsolationLevel PostIsolationLevel { get; set; }

        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(BaseWebApiController<,>));
                }
                return _logCategories;
            }
        }
        protected UpdateActionType? CurrentUpdateActionType => _updateActionType;
        protected InsertActionType? CurrentInsertActionType => _insertActionType;
        protected DeleteActionType? CurrentDeleteActionType => _deleteActionType;
        protected ICollection<IWorkflowAction> WorkflowActions { get; set; }
        protected string WorkflowName { get; set; }
        protected Guid? IdWorkflowActivity { get; set; }
        protected bool WorkflowAutoComplete { get; set; }

        #endregion

        #region [ Constructor ]
        protected BaseWebApiController(TService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base()
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _instanceId = Guid.NewGuid();
            PostIsolationLevel = IsolationLevel.ReadCommitted;
            WorkflowActions = new List<IWorkflowAction>();
        }
        #endregion

        #region [ Dispose ]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _service.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region [ Methods ]

        [AcceptVerbs("OPTIONS")]
        [AllowAnonymous]
        public IHttpActionResult Options()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IQueryable<TEntity>> GetAsync(ODataQueryOptions opts)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return opts.ApplyTo(_service.Queryable(true)) as IQueryable<TEntity>;
                }
                catch (AggregateException ae)
                {
                    foreach (Exception ie in ae.Flatten().InnerExceptions)
                    {
                        _logger.WriteError(ie, LogCategories);
                    }
                    BadRequest(ae.Message);
                }
                catch (DSWException ex)
                {
                    _logger.WriteError(ex, LogCategories);
                    BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.WriteError(ex, LogCategories);
                    BadRequest(ex.Message);
                }
                return new List<TEntity>().AsQueryable();
            });
        }

        private async Task<IHttpActionResult> PostAsync(TEntity entity, Func<TEntity, Task<TEntity>> lambda)
        {
            return await ActionHelper.TryCatchWithLoggerAsync(async () =>
            {
            _unitOfWork.BeginTransaction(PostIsolationLevel);
            if (entity is IWorkflowContentBase)
            {
                    IWorkflowContentBase workflow = (IWorkflowContentBase)entity;
                    WorkflowActions = workflow.WorkflowActions ?? new List<IWorkflowAction>();
                    WorkflowName = workflow.WorkflowName;
                    IdWorkflowActivity = workflow.IdWorkflowActivity;
                    WorkflowAutoComplete = workflow.WorkflowAutoComplete;
                }
                entity = await lambda(entity);
                bool result = await _unitOfWork.SaveAsync();

                if (result)
                {
                    AfterSave(entity);
                    return Ok(entity);
                }
                return InternalServerError();
            }, BadRequest, Content, InternalServerError, _logger, LogCategories);
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostAsync([FromBody]TEntity entity)
        {
            _insertActionType = InsertActionType.None;
            return await PostAsync(entity, async (e) => await _service.CreateAsync(e));
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostAsync([FromBody]TEntity entity, InsertActionType actionType)
        {
            _insertActionType = actionType;
            return await PostAsync(entity, async (e) => await _service.CreateAsync(e, actionType));
        }

        private async Task<IHttpActionResult> PutAsync(TEntity entity, Func<TEntity, Task<TEntity>> lambda)
        {
            return await ActionHelper.TryCatchWithLoggerAsync(async () =>
            {
                if (!_service.Queryable().Any(f => f.UniqueId == entity.UniqueId))
                {
                    return NotFound();
                }

                _unitOfWork.BeginTransaction();
                if (entity is IWorkflowContentBase)
                {
                    IWorkflowContentBase workflow = (IWorkflowContentBase)entity;
                    WorkflowActions = workflow.WorkflowActions ?? new List<IWorkflowAction>();
                    WorkflowName = workflow.WorkflowName;
                    IdWorkflowActivity = workflow.IdWorkflowActivity;
                    WorkflowAutoComplete = workflow.WorkflowAutoComplete;
                }
                entity = await lambda(entity);
                bool result = await _unitOfWork.SaveAsync();

                if (result)
                {
                    AfterSave(entity);
                    return Ok(entity);
                }
                return InternalServerError();
            }, BadRequest, Content, InternalServerError, _logger, LogCategories);
        }

        [HttpPut]
        public async Task<IHttpActionResult> PutAsync([FromBody]TEntity entity)
        {
            _updateActionType = UpdateActionType.None;
            return await PutAsync(entity, async (e) => await _service.UpdateAsync(e));
        }

        [HttpPut]
        public async Task<IHttpActionResult> PutAsync([FromBody]TEntity entity, UpdateActionType actionType)
        {
            _updateActionType = actionType;
            return await PutAsync(entity, async (e) => await _service.UpdateAsync(e, actionType));
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteAsync([FromBody]TEntity entity)
        {
            _deleteActionType = DeleteActionType.None;
            return await DeleteAsync(entity, async (e) => await _service.DeleteAsync(e));
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteAsync([FromBody]TEntity entity, DeleteActionType actionType)
        {
            _deleteActionType = actionType;
            return await DeleteAsync(entity, async (e) => await _service.DeleteAsync(e, actionType));
        }

        private async Task<IHttpActionResult> DeleteAsync([FromBody]TEntity entity, Func<TEntity, Task<bool>> lambda)
        {
            return await ActionHelper.TryCatchWithLoggerAsync(async () =>
            {
                TEntity entityToDelete = (SetEntityIncludeOnDelete(_unitOfWork.Repository<TEntity>().Query(f => f.UniqueId == entity.UniqueId, true)).SelectAsQueryable()).SingleOrDefault();
                if (entityToDelete == null)
                {
                    return NotFound();
                }

                _unitOfWork.BeginTransaction();
                if (entity is IWorkflowContentBase)
                {
                    IWorkflowContentBase workflow = (IWorkflowContentBase)entity;
                    WorkflowActions = workflow.WorkflowActions ?? new List<IWorkflowAction>();
                    WorkflowName = workflow.WorkflowName;
                    IdWorkflowActivity = workflow.IdWorkflowActivity;
                    WorkflowAutoComplete = workflow.WorkflowAutoComplete;
                }
                await lambda(entity);
                await _unitOfWork.SaveAsync();
                AfterSave(entityToDelete);

                return Ok(entity);
            }, BadRequest, Content, InternalServerError, _logger, LogCategories);
        }

        protected virtual IQueryFluent<TEntity> SetEntityIncludeOnDelete(IQueryFluent<TEntity> query)
        {
            return query;
        }

        protected virtual void AfterSave(TEntity entity)
        {
        }

        #endregion
    }
}
