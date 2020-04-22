using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.Service.Roslyn;

namespace VecompSoftware.ServiceBus.Module.UDS.Roslyn.Generators.Controllers
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class CreateUDSController
    {
        #region [ Roslyn Object]
        private MSBuildWorkspace _workspace { get; set; }
        private static readonly ParameterSyntax[] _parameterLists = new ParameterSyntax[]{
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("IDSWDataContext dswDataContext")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("DomainUserModel domainUserModel")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("bool applySecurity")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("bool onlyToRead"))};

        private const string _bodyStatement = @"
            IQueryable<{0}> results = dswDataContext.DataSet<{0}>();
            if (applySecurity)
            {
                results = results.Where(f =>
                      f.UDSRepository.Container.ContainerGroups.Any(t => t.UDSRights.Contains(""1"") && (t.SecurityGroup.IsAllUsers || t.SecurityGroup.SecurityUsers.Any(s => s.Account == domainUserModel.Name && s.UserDomain == domainUserModel.Domain))) ||
                      f.UDSRepository.UDSUsers.Any(t => t.IdUDS == f.UDSId && t.Account == domainUserModel.Account) ||
                      f.UDSRepository.UDSRoles.Any(t => t.IdUDS == f.UDSId && t.Relation.RoleGroups.Any(g => g.DocumentSeriesRights.Contains(""1"") && g.SecurityGroup.SecurityUsers.Any(s => s.Account == domainUserModel.Name && s.UserDomain == domainUserModel.Domain))));
            }
            if (onlyToRead)
            {
                results = results.Where(f => !f.UDSRepository.UDSLogs.Any(t => t.IdUDS == f.UDSId && t.RegistrationUser == domainUserModel.Account && t.LogType == Entity.UDS.UDSLogType.SummaryView));
            }
            return results; 
        ";
        #endregion
        #region [ Base Class Relation ]
        private ClassContainer _baseController { get; set; }
        private string _baseClassName { get; set; }
        private string _baseEntityName { get; set; }
        #endregion
        #region [ UDS Class New Class ]
        private ClassContainer _udsController { get; set; }
        private string _udsClassName { get; set; }
        private string _udsControllerClassName { get; set; }
        private string _udsNamespace { get; set; }
        #endregion
        #region [ Properties ]
        private ILogger _log { get; set; }

        protected static IEnumerable<LogCategory> _logCategories = null;
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(CreateUDSController));
                }
                return _logCategories;
            }
        }
        #endregion
        public CreateUDSController(ILogger log, MSBuildWorkspace workspace, Project prj, string baseClassName, string udsClassName, string udsNamespace)
        {
            _log = log;
            _baseClassName = baseClassName;
            _udsClassName = udsClassName;
            _udsNamespace = udsNamespace;
            _udsControllerClassName = string.Concat(_udsNamespace, "Controller");
            _log.WriteInfo(new LogMessage(string.Format("Creazione nuovo controller: {0}", _udsControllerClassName)), LogCategories);

            _workspace = workspace;
            _baseController = new ClassContainer() { WorkingProject = prj };
            _udsController = new ClassContainer() { WorkingProject = prj };
        }

        public async Task Execute()
        {
            await LoadBaseObject();
            await PreparareUDSObject();
        }

        public async Task Save()
        {
            string udsNameSpacePath = string.Concat("Entities\\", _udsNamespace);
            _log.WriteInfo(new LogMessage(string.Format("Salvataggio file: {0}", udsNameSpacePath)), LogCategories);
            await WriteDocument.WriteToDiskAsync(_udsController.WorkingProject, _udsController.UnitSyntax, udsNameSpacePath, _udsControllerClassName);
            _udsController.Document = _udsController.WorkingProject.AddDocument(string.Concat(_udsClassName, ".", Languages.cs.ToString()), _udsController.UnitSyntax);
            await WriteDocument.WriteToProjectAsync(udsNameSpacePath, _udsControllerClassName, _udsController.WorkingProject, _workspace);
        }

        private async Task LoadBaseObject()
        {
            _log.WriteInfo(new LogMessage(string.Concat("Caricamento classe base", _baseClassName)), LogCategories);
            _baseController.Document = await _baseController.Document.GetDocumentAsync(_baseController.WorkingProject, _baseClassName);
            _baseController.UnitSyntax = await _baseController.Document.GetCompilationUnitAsync();
            _baseController.Namespace = SyntaxFactory.NamespaceDeclaration(_baseController.UnitSyntax.GetNamespacesFromClassAsync().Result.Single().Name);
            _baseController.Usings = await _baseController.UnitSyntax.GetUsingClassAsync();
            _baseController.Properties = await _baseController.UnitSyntax.GetPropertiesAsync();
        }

        private async Task PreparareUDSObject()
        {
            _udsController.Usings = _baseController.Usings;
            _udsController.Usings.Add(await _udsController.Usings.FirstOrDefault().CreateUsingAsync("VecompSoftware.DocSuiteWeb.UDS", "Controllers"));

            _log.WriteInfo(new LogMessage(string.Format("Creazione nuova classe : {0}", _udsClassName)), LogCategories);
            _udsController.ClassSyntax = _udsController.ClassSyntax
                .CreatePublicClass(_udsControllerClassName)
                .AddInheritanceGenericClass(_baseClassName, _udsClassName)
                .AddConstructor(_udsControllerClassName, new List<string>() { "IDSWDataContext dswDataContext", "ISecurity security" },
                    new List<string>() { "dswDataContext", "security" })
                .AddInternalOverrideMethod("GetDataSet", SyntaxFactory.ParseStatement(_bodyStatement.Replace("{0}", _udsClassName)),
                    _parameterLists, SyntaxFactory.ParseTypeName(string.Concat("IQueryable<", _udsClassName, ">")));

            _udsController.Namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(string.Concat("VecompSoftware.DocSuiteWeb.UDS.Entities.", _udsNamespace)));
            _udsController.Namespace = _udsController.Namespace.AddMembers(_udsController.ClassSyntax);

            _udsController.UnitSyntax = SyntaxFactory.CompilationUnit();
            _udsController.UnitSyntax = _udsController.UnitSyntax.AddMembers(_udsController.Namespace);
            _udsController.UnitSyntax = _udsController.UnitSyntax.AddUsings(_udsController.Usings.ToArray());
            _udsController.UnitSyntax = (CompilationUnitSyntax)Formatter.Format(_udsController.UnitSyntax, _workspace);
        }

        private async Task<ClassDeclarationSyntax> AddConstructor(string constructor)
        {
            return await Task.Run(() => _udsController.ClassSyntax.AddModifiers(SyntaxFactory.Identifier(constructor)));
        }
    }
}
