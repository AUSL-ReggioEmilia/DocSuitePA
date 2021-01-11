using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData
{
    [LogCategory(LogCategoryDefinition.ODATAAPI)]
    [EnableQuery]
    public abstract class BaseODataController<TEntity, TService> : ODataController
        where TEntity : DSWBaseEntity
        where TService : IServiceAsync<TEntity, TEntity>
    {
        #region [ Fields ]

        private static IEnumerable<LogCategory> _logCategories = null;
        private readonly TService _service;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly Guid _instanceId;

        #endregion

        #region [ Properties ]

        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(BaseODataController<,>));
                }
                return _logCategories;
            }
        }

        protected string Username { get; }

        protected string Domain { get; }

        #endregion

        #region [ Constructor ]
        protected BaseODataController(TService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base()
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _instanceId = Guid.NewGuid();
            DomainUserModel currentUser = security.GetCurrentUser();
            Username = currentUser?.Name;
            Domain = currentUser?.Domain;

            if (string.IsNullOrEmpty(Username))
            {
                _logger.WriteError(new LogMessage("BaseODataController -> Lo Username non è presente o non è corretto."), LogCategories);
                throw new DSWException(string.Concat("Il nome utente non è presente o non è corretto."), null, DSWExceptionCode.SC_InvalidAccount);
            }

            if (string.IsNullOrEmpty(Domain))
            {
                _logger.WriteError(new LogMessage("BaseODataController -> l Domain non è presente o non è corretto."), LogCategories);
                throw new DSWException(string.Concat("Il dominio non è presente o non è corretto."), null, DSWExceptionCode.SC_InvalidAccount);
            }
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
        [EnableQuery(MaxExpansionDepth = 4, PageSize = 100, MaxAnyAllExpressionDepth = 2)]
        public IQueryable<TEntity> Get()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                return _service.Queryable(true);
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 4, PageSize = 100)]
        public SingleResult<TEntity> Get([FromODataUri] Guid key)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                return SingleResult.Create(_service.Queryable(true).Where(p => p.UniqueId == key));
            }, _logger, LogCategories);
        }
        #endregion
    }
}
