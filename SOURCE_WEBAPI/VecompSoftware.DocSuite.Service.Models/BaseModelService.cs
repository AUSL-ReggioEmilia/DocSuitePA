using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Mapper;

namespace VecompSoftware.DocSuite.Service.Models
{
    [LogCategory(LogCategoryDefinition.SERVICEMODEL)]
    public abstract class BaseModelService<TModel> : IModelService<TModel>
        where TModel : class, new()
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories = null;
        public const string EXCEPTION_MESSAGE = "Operation not allowed.";

        #endregion

        #region [ Properties ]

        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(BaseModelService<>));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]

        protected BaseModelService(IDataUnitOfWork unitOfWork, ILogger logger, IMapperUnitOfWork mapperUnitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapperUnitOfWork = mapperUnitOfWork;
            _logger = logger;
        }

        #endregion Constructor

        #region [ Methods ]

        #endregion
    }
}