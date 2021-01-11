using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Hosting;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.DocSuite.WebAPI.Common.Configurations
{
    [LogCategory(LogCategoryDefinition.GENERAL)]
    public static class WorkflowConfiguration
    {
        #region [ Fields ]
        private const string CONFIGURATION_FILE_PATH = "~/App_Data/ConfigurationFiles/Workflow.Client.Config.json";
        private static ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories = null;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(WorkflowConfiguration));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Methods ]
        public static string GetWorkflowConfiguration(ILogger logger)
        {
            _logger = logger;
            string pathFileConfig = HostingEnvironment.MapPath(CONFIGURATION_FILE_PATH);
            string configurationJson = string.Empty;
            IList<string> errorMessages = new List<string>();
            string configuration = string.Empty;
            try
            {
                configurationJson = File.ReadAllText(pathFileConfig, Encoding.UTF8);
                configuration = configurationJson;
            }
            catch (Exception ex)
            {
                logger.WriteError(new LogMessage(ex.Message), ex, LogCategories);
            }
            return configuration;
        }
        #endregion
    }
}