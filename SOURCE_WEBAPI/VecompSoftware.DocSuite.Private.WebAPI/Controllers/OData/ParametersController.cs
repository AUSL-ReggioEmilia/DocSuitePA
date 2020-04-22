using Microsoft.AspNet.OData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData
{
    [LogCategory(LogCategoryDefinition.ODATAAPI)]
    [EnableQuery]

    public class ParametersController : ODataController
    {
        #region [ Fields ]

        private static IEnumerable<LogCategory> _logCategories = null;
        private readonly IParameterEnvService _parameterEnvService;
        private readonly ILogger _logger;

        private static List<ODataParameterModel> _models = null;
        #endregion

        #region [ Properties ]

        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ParametersController));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public ParametersController(IParameterEnvService parameterEnvService, ILogger logger)
        {
            _parameterEnvService = parameterEnvService;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        [EnableQuery(MaxExpansionDepth = 4, PageSize = 100)]
        public IQueryable<ODataParameterModel> Get()
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                if (_models == null)
                {
                    InitModels();
                }
                return _models.AsQueryable();
            }, _logger, LogCategories);
        }

        public static ODataParameterModel GetPropValue(PropertyInfo item, IParameterEnvService src)
        {
            dynamic value = null;
            try
            {
                value = item.GetValue(src, null);
            }
            catch (Exception) { }
            if (value == null)
            {
                return null;
            }
            return new ODataParameterModel() { Key = item.Name, Value = JsonConvert.SerializeObject(value, JsonSerializerConfig.SerializerSettings) };
        }

        private void InitModels()
        {
            if (_models != null)
            {
                _models.Clear();
                _models = null;
            }
            _models = new List<ODataParameterModel>();
            _models.AddRange(_parameterEnvService.GetType().GetProperties().Select(f => GetPropValue(f, _parameterEnvService)).Where(f => f != null));
        }

        #endregion
    }
}