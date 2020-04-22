using System;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.ServiceBus.Module.UDS.Roslyn.Generators.Controllers
{
    public class CreateUDSControllerNamespace : BaseGenerator
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly UDSEntity _udsBaseEntity;

        #endregion

        #region [ Properties ]

        #endregion

        public CreateUDSControllerNamespace(ILogger logger, UDSEntity uds,
            string solutionPath, string projectName)
            : base(logger, solutionPath, projectName)
        {
            _udsBaseEntity = uds;
            _logger = logger;
        }

        /// <summary>
        /// Crea una classe per ogni tabelle UDS di relazione. Aggiunge le foreign key alla tabella principale delle UDS. Salva il file nel progetto
        /// Crea una classe per la tabella principale. Aggiunge i metadati. 
        /// Salva il file di progetto.
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
                CreateUDSController controller;
                foreach (UDSControllerTypes controllerType in Enum.GetValues(typeof(UDSControllerTypes)))
                {
                    // UDSClassName ==> Nome tabella
                    controller = new CreateUDSController(_logger, Workspace, Project, controllerType.ToString(), _udsBaseEntity.TableName, _udsBaseEntity.Namespace);
                    await controller.Execute();
                    await controller.Save();
                }
                result = true;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
            return result;
        }

    }
}
