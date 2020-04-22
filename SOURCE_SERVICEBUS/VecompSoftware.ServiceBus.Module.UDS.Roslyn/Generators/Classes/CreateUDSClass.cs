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

namespace VecompSoftware.ServiceBus.Module.UDS.Roslyn.Generators.Classes
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class CreateUDSClass
    {
        #region [ Roslyn Object]
        private readonly MSBuildWorkspace _workspace;

        private readonly string _dbSchema;

        private readonly Dictionary<UDSRelationType, RoslynMapper> _roslynMappings;

        #endregion
        #region [ Base Class Relation ]
        private const string UDSId = "UDSId";
        private readonly ClassContainer _base;

        private readonly string _baseClassName;

        private readonly string _baseEntityName;

        private readonly string _propertyFieldName;

        private readonly string _udsTableAnnotation;

        #endregion

        #region [ UDS Class New Class ]

        private ClassContainer _uds { get; set; }

        private string _udsClassName { get; set; }

        private string _udsNamespace { get; set; }

        #endregion


        #region [ Properties ]

        private DocSuiteWeb.Common.Loggers.ILogger _log { get; set; }
        protected static IEnumerable<LogCategory> _logCategories = null;
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(CreateUDSClass));
                }
                return _logCategories;
            }
        }
        #endregion

        public CreateUDSClass(ILogger log, MSBuildWorkspace workspace, Project prj,
            Dictionary<UDSRelationType, RoslynMapper> roslynMappings,
            string dbSchema, string baseClassName, string udsClassName,
            string udsNamespace, string propertyFieldName, string udsTableAnnotation,
            string baseEntityName = "")
        {
            _log = log;
            _log.WriteInfo(new LogMessage(string.Concat("Creazione nuova classe C# ", udsClassName)), LogCategories);
            _roslynMappings = roslynMappings;

            _baseClassName = baseClassName;
            _baseEntityName = baseEntityName;
            _propertyFieldName = propertyFieldName;
            _udsTableAnnotation = udsTableAnnotation;

            _udsClassName = udsClassName;
            _udsNamespace = udsNamespace;
            _dbSchema = dbSchema;

            _workspace = workspace;
            _base = new ClassContainer() { WorkingProject = prj };
            _uds = new ClassContainer() { WorkingProject = prj };
        }

        public async Task Execute(bool addForeignKeyToBaseEntity = false, List<Metadata> metadata = null)
        {
            await LoadBaseObject();
            await PreparareUDSObject(addForeignKeyToBaseEntity, metadata);
        }

        /// <summary>
        /// Salva il file sul disco e lo include nel progetto
        /// </summary>
        /// <returns></returns>
        public async Task Save()
        {
            string udsNameSpacePath = string.Concat("Entities\\", _udsNamespace);
            _log.WriteInfo(new LogMessage(string.Format("Salvataggio file: {0}", udsNameSpacePath)), LogCategories);
            await WriteDocument.WriteToDiskAsync(_uds.WorkingProject, _uds.UnitSyntax, udsNameSpacePath, _udsClassName);
            _uds.Document = _uds.WorkingProject.AddDocument(string.Concat(_udsClassName, ".", Languages.cs.ToString()), _uds.UnitSyntax);
            await WriteDocument.WriteToProjectAsync(udsNameSpacePath, _udsClassName, _uds.WorkingProject, _workspace);
        }

        /// <summary>
        /// Apro il documento e prelevo le informazioni base:
        /// Namespace
        /// Using
        /// Properties
        /// </summary>
        /// <returns></returns>
        private async Task LoadBaseObject()
        {
            _log.WriteInfo(new LogMessage(string.Concat("Caricamento dati da classe ", _baseClassName)), LogCategories);
            _base.Document = await _base.Document.GetDocumentAsync(_base.WorkingProject, _baseClassName);
            _base.UnitSyntax = await _base.Document.GetCompilationUnitAsync();
            _base.Namespace = SyntaxFactory.NamespaceDeclaration(_base.UnitSyntax.GetNamespacesFromClassAsync().Result.Single().Name);
            _base.Usings = await _base.UnitSyntax.GetUsingClassAsync();
            _base.Properties = await _base.UnitSyntax.GetPropertiesAsync();
        }

        /// <summary>
        /// Carico gli using dall'oggetto base
        /// Aggiungo lo using dell'oggetto base. Mi serve per ereditare dall'oggetto base.
        /// Aggiungo le property dell'oggetto base
        /// Creo la nuova classe. devo ereditare dalla classe astratta base.
        /// Aggiungo la data annotation con il nome della tabella.
        /// Aggiungo le property appena create alla classe.
        /// Aggiungo la data annotatio della foreign key se necessario.
        /// Aggiungo le proprietà dinamiche dell'oggetto base se necessario.
        /// Aggiungo la classe al namespace
        /// Aggiungo il namespace alla CompilationUnit
        /// </summary>
        /// <returns></returns>
        private async Task PreparareUDSObject(bool addForeignKeyToBaseEntity, List<Metadata> metadatas)
        {
            _log.WriteInfo(new LogMessage(string.Concat("Creazione classe ", _udsClassName)), LogCategories);
            _uds.Usings = _base.Usings;
            _uds.Usings.Add(await _base.Usings.FirstOrDefault().CreateUsingAsync("VecompSoftware.DocSuiteWeb.UDS.Entities", "Base"));

            _uds.ClassSyntax = _uds.ClassSyntax
                .CreatePublicClass(_udsClassName)
                .AddInheritanceClass(_baseClassName)
                .AddDataAnnotation("Table", string.Concat("\"", _udsTableAnnotation, "\""), "Schema", _dbSchema);

            if (addForeignKeyToBaseEntity)
            {
                _log.WriteInfo(new LogMessage(string.Concat("Aggiunta foreign key in ", _udsClassName)), LogCategories);
                PropertyDeclarationSyntax p = null;
                p = p.CreateProperty(_baseEntityName, _baseEntityName);
                p = AddDataAnnotationAsync(p, "ForeignKey", UDSId).Result;
                _uds.Properties.Add(p);
                _uds.ClassSyntax = _uds.ClassSyntax.AddMember(p);
            }
            else
            {
                // aggiungo l'inizializzazione delle collezioni.
                ICollection<KeyValuePair<string, string>> relation = _roslynMappings.Select(s => new KeyValuePair<string, string>(s.Value.PropertyFieldName, string.Concat(_udsClassName, "_", s.Value.ClassName))).ToList();
                _uds.ClassSyntax = await _uds.ClassSyntax.AddBaseEntityConstructorAsync(_udsClassName, relation);
            }

            if (metadatas != null)
            {
                PropertyDeclarationSyntax pMeta = null;
                foreach (Metadata m in metadatas)
                {
                    _log.WriteInfo(new LogMessage(string.Format("Aggiunta metadata {0} in {1}", m.PropertyName, _udsClassName)), LogCategories);

                    pMeta = m.Nullable ? pMeta.CreateNullableProperty(m.PropertyType, m.PropertyName) : pMeta.CreateProperty(m.PropertyType, m.PropertyName);
                    if (m.Required)
                    {
                        _log.WriteInfo(new LogMessage(string.Format("Aggiunta notation require in {0} per la classe {1}", m.PropertyName, _udsClassName)), LogCategories);
                        pMeta = pMeta.AddSimpleDataAnnotation("Required");
                    }

                    _uds.Properties.Add(pMeta);
                    _uds.ClassSyntax = _uds.ClassSyntax.AddMember(pMeta);
                }
            }
            _uds.Namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(string.Concat("VecompSoftware.DocSuiteWeb.UDS.Entities.", _udsNamespace)));
            _uds.Namespace = _uds.Namespace.AddMembers(_uds.ClassSyntax);

            _uds.UnitSyntax = SyntaxFactory.CompilationUnit();
            _uds.UnitSyntax = _uds.UnitSyntax.AddMembers(_uds.Namespace);
            _uds.UnitSyntax = _uds.UnitSyntax.AddUsings(_uds.Usings.ToArray());
            _uds.UnitSyntax = (CompilationUnitSyntax)Formatter.Format(_uds.UnitSyntax, _workspace);
        }

        public async Task AddNavigationPropertyAsync(ICollection<KeyValuePair<string, string>> navigationsTypes)
        {
            await Task.Run(async () =>
            {
                ClassContainer classContainer = new ClassContainer
                {
                    WorkingProject = _uds.WorkingProject,
                    Document = _uds.Document,
                    Usings = _uds.Usings
                };
                classContainer.Usings.Add(await classContainer.Usings.FirstOrDefault().CreateUsingAsync("System.Collections", "Generic"));

                PropertyDeclarationSyntax p = null;
                navigationsTypes.ToList().ForEach(t =>
                {
                    string collectionOfT = string.Format("ICollection<{0}>", t.Value);
                    _log.WriteInfo(new LogMessage(string.Format("Aggiunta collezione {0}", collectionOfT)), LogCategories);
                    p = p.CreateProperty(collectionOfT, t.Key);
                    _uds.Properties.Add(p);
                    _uds.ClassSyntax = _uds.ClassSyntax.AddMember(p);
                });
                classContainer.Properties = _uds.Properties;
                classContainer.ClassSyntax = _uds.ClassSyntax;
                _uds.UnitSyntax.Members.OfType<NamespaceDeclarationSyntax>().ToList().ForEach(ns =>
                {
                    if (classContainer.Namespace != null)
                    {
                        classContainer.Namespace.AddMembers(ns);
                    }
                    else
                    {
                        classContainer.Namespace = SyntaxFactory.NamespaceDeclaration(ns.Name);
                    }
                });
                classContainer.Namespace = classContainer.Namespace.AddMembers(classContainer.ClassSyntax);

                classContainer.UnitSyntax = SyntaxFactory.CompilationUnit();
                classContainer.UnitSyntax = classContainer.UnitSyntax.AddMembers(classContainer.Namespace);
                classContainer.UnitSyntax = classContainer.UnitSyntax.AddUsings(classContainer.Usings.ToArray());
                classContainer.UnitSyntax = (CompilationUnitSyntax)Formatter.Format(classContainer.UnitSyntax, _workspace);

                _uds = classContainer;
            });
        }

        /// <summary>
        /// Aggiungo la data annotatio alla proprietà
        /// </summary>
        /// <param name="p">Proprietà</param>
        /// <param name="dataAnnotation">Nome della data annotation. Esempio: "ForeignKey"</param>
        /// <param name="pkClass">Nome dell'argomento della data annotation. Esempio: "NomeColonna".</param>
        /// <returns></returns>
        private async Task<PropertyDeclarationSyntax> AddDataAnnotationAsync(PropertyDeclarationSyntax p, string dataAnnotation, string pkClass)
        {
            return await Task.Run(() =>
            {
                p = p.AddDataAnnotation(dataAnnotation, string.Concat("\"", pkClass, "\""));
                return p;
            });
        }
    }
}
