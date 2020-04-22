using AutoMapper;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuite.Public.Core.Models.Domains;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Domains
{
    [LogCategory(LogCategoryDefinition.ODATAAPI)]
    [EnableQuery]
    public abstract class BaseODataController<TModel, TEntity> : ODataController
        where TModel : DomainModel
        where TEntity : DSWBaseEntity
    {
        #region [ Fields ]

        private static IEnumerable<LogCategory> _logCategories = null;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
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
        #endregion

        #region [ Constructor ]
        protected BaseODataController(IDataUnitOfWork unitOfWork, ILogger logger, IMapper mapper)
            : base()
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _instanceId = Guid.NewGuid();
        }
        #endregion

        #region [ Dispose ]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
            base.Dispose(disposing);
        }
        #endregion

        #region [ Methods ]
        [EnableQuery(MaxExpansionDepth = 4, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        public IQueryable<TModel> Get()
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                //Non esistono metodi generici di lettura di una intera collezione di entità.
                return new List<TModel>().AsQueryable();
            }, _logger, LogCategories);
        }

        #endregion
    }
}
