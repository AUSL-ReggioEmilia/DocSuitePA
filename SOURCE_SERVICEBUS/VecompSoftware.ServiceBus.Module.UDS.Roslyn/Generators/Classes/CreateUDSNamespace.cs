using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.ServiceBus.Module.UDS.Roslyn.Generators.Classes
{
    public class CreateUDSNamespace : BaseGenerator
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly UDSEntity _udsBaseEntity;
        private readonly string _dbSchema;
        private readonly Dictionary<UDSRelationType, RoslynMapper> _roslynMappings;

        #endregion

        #region [ Properties ]

        #endregion

        public CreateUDSNamespace(ILogger logger, UDSEntity uds,
            string solutionPath, string projectName, string dbSchema)
            : base(logger, solutionPath, projectName)
        {
            _dbSchema = dbSchema;
            _udsBaseEntity = uds;
            _logger = logger;
            _roslynMappings = JsonConvert.DeserializeObject<Dictionary<UDSRelationType, RoslynMapper>>(Properties.Resources.UDSEntityRelations);
        }

        /// <summary>
        /// Crea una classe per ogni tabelle UDS di relazione. Aggiunge le foreign key alla tabella principale delle UDS. Salva il file nel progetto
        /// Crea una classe per la tabella principale. Aggiunge i metadati. Salva il file di progetto.
        /// Compila il progetto e restituisce il risultato.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ExecuteAsync()
        {
            bool result = false;
            try
            {
                _logger.WriteInfo(new LogMessage("Create workspace"), LogCategories);
                CreateWorkspace();
                _logger.WriteInfo(new LogMessage("Open solution"), LogCategories);
                await OpenSolutionAsync();
                _logger.WriteInfo(new LogMessage("Open project"), LogCategories);
                await OpenProjectAsync();

                ICollection<KeyValuePair<string, string>> types = _roslynMappings.Select(s => new KeyValuePair<string, string>(s.Key.ToString(), string.Concat(_udsBaseEntity.TableName, "_", s.Value.ClassName))).ToList();

                string udsClassName = string.Empty;
                foreach (UDSEntityTypes entityType in Enum.GetValues(typeof(UDSEntityTypes)))
                {
                    // UDSClassName ==> Nome tabella
                    udsClassName = _udsBaseEntity.TableName;
                    CreateUDSClass udsClass = new CreateUDSClass(_logger, Workspace, Project, _roslynMappings,
                        _dbSchema, entityType.ToString(), udsClassName, _udsBaseEntity.Namespace, string.Empty, udsClassName);
                    await udsClass.Execute(false, _udsBaseEntity.MetaData);
                    await udsClass.AddNavigationPropertyAsync(types);
                    await udsClass.Save();
                }

                foreach (KeyValuePair<UDSRelationType, RoslynMapper> currentMapper in _roslynMappings)
                {
                    // UDSClassName ==> Nome tabella
                    string udsRelationClassName = string.Concat(_udsBaseEntity.TableName, "_", currentMapper.Value.ClassName);
                    string udsTableAnnotation = string.Concat(_udsBaseEntity.TableName, "_", currentMapper.Value.TableAnnotation);
                    CreateUDSClass relation = new CreateUDSClass(_logger, Workspace, Project, _roslynMappings,
                        _dbSchema, currentMapper.Value.BaseClassName, udsRelationClassName,
                        _udsBaseEntity.Namespace, _udsBaseEntity.TableName, udsTableAnnotation, udsClassName);
                    await relation.Execute(true);
                    await relation.Save();
                }
                result = true;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
            }
            return result;
        }
    }
}
