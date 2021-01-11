using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.Service.Roslyn;

namespace VecompSoftware.ServiceBus.Module.UDS.Roslyn
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class CompilerBuilder
    {
        #region [ Fields ]

        private readonly DocSuiteWeb.Common.Loggers.ILogger _logger;
        private readonly BasicFileLogger _builderLogger;
        private readonly string _projectFilePath;
        private readonly string _binFolder;
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> _logCategories = null;

        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(CompilerBuilder));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]

        public CompilerBuilder(DocSuiteWeb.Common.Loggers.ILogger logger, string compilerLoggerPath, string projectFilePath, string binFolder)
        {
            try
            {
                _logger = logger;
                DirectoryInfo dirToLog = new DirectoryInfo(new DirectoryInfo(compilerLoggerPath).Parent.FullName);
                if (!dirToLog.Exists)
                {
                    dirToLog.Create();
                }
                _builderLogger = new BasicFileLogger()
                {
                    Verbosity = LoggerVerbosity.Normal,
                    Parameters = compilerLoggerPath
                };
                _projectFilePath = projectFilePath;
                _binFolder = binFolder;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        #endregion

        #region [ Methods ]

        public async Task<bool> BuildAsync(string mode = "Release")
        {
            bool result = false;
            try
            {
                _logger.WriteInfo(new LogMessage("Inizio compilazione"), LogCategories);
                CompileProject compile = new CompileProject();
                IDictionary<string, string> buildOption = compile.BuildOption(_binFolder, CompilationType.Release);
                IDictionary<string, TargetResult> compilerResult = await compile.ExecuteAsync(_projectFilePath, buildOption, _builderLogger);
                // Il risultato di compilazione si trova qui:
                if (compilerResult["Build"].Items.FirstOrDefault() != null)
                {
                    _logger.WriteInfo(new LogMessage("Compilazione avvenuta con successo"), LogCategories);
                    result = true;
                    return result;
                }
                _logger.WriteInfo(new LogMessage("Errore di compilazione, verificare il log di compilazione"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
            return result;
        }

        #endregion
    }
}
