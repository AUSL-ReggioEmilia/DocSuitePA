using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.Service.Roslyn;

namespace VecompSoftware.ServiceBus.Module.UDS.Roslyn.Generators
{
    [LogCategory(LogCategoryDefinition.GENERAL)]
    public abstract class BaseGenerator
    {
        #region [ Fields ]
        protected static IEnumerable<LogCategory> _logCategories = null;

        private readonly string _solutionPath;
        private readonly string _projectName;
        private readonly ILogger _logger;
        private MSBuildWorkspace _workspace;
        private Solution _solution;
        private Project _project;

        #endregion

        #region [ Properties ]
        public string ProjectFilePath => _project == null ? string.Empty : _project.FilePath;

        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(BaseGenerator));
                }
                return _logCategories;
            }
        }

        protected MSBuildWorkspace Workspace => _workspace;

        protected Solution Solution => _solution;

        protected Project Project => _project;
        #endregion

        #region [ Constructor ]

        public BaseGenerator(ILogger logger,
            string solutionPath, string projectName)
        {
            _logger = logger;
            _solutionPath = solutionPath;
            _projectName = projectName;
        }
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Crea uno workspace di lavoro
        /// </summary>
        protected void CreateWorkspace()
        {
            _workspace = MSBuildWorkspace.Create();
            _workspace.Options.WithChangedOption(CSharpFormattingOptions.IndentBraces, true);
        }

        /// <summary>
        /// Apre la solution in lettura
        /// </summary>
        /// <param name="solutionPath"></param>
        /// <returns></returns>
        protected async Task OpenSolutionAsync()
        {
            _solution = await _workspace.OpenSolutionByWorkspaceAsync(_solutionPath);
        }

        /// <summary>
        /// Apre il progetto in lettura dato un nome
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        protected async Task OpenProjectAsync()
        {
            _project = await _solution.GetProjectByNameAsync(_projectName);
        }
        #endregion
    }
}
