using System;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.Core.Command.CQRS.Events;
using VecompSoftware.Core.Command.CQRS.Events.Models.UDS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.ServiceBus.Module.UDS.Storage;
using VecompSoftware.ServiceBus.Module.UDS.Storage.Relations;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.Services.Command.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Events.Models.UDS;

namespace VecompSoftware.ServiceBus.Module.UDS.Listener.DataInsert
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class Execution : UDSBaseExecution<ICommandInsertUDSData>
    {
        #region [ Fields ]

        private readonly ILogger _logger;

        private readonly BiblosDS.BiblosClient _biblosClient;

        #endregion

        #region [ Properties ]

        #endregion

        public Execution(ILogger logger, IWebAPIClient webApiClient,
            BiblosDS.BiblosClient biblosClient) : base(logger, webApiClient)
        {
            _logger = logger;
            _biblosClient = biblosClient;
        }

        public override async Task ExecuteAsync(ICommandInsertUDSData command)
        {
            _logger.WriteInfo(new LogMessage(string.Concat(command.CommandName, " is arrived")), LogCategories);

            Guid? collaborationUniqueId;
            string collaborationTemplateName;
            int? collaborationId;

            try
            {
                _logger.WriteInfo(new LogMessage(string.Concat(command.ContentType.Id, " model evaluating ... ")), LogCategories);
                UDSEntityModel udsEntityModel = await InsertDataAsync(command.ContentType.ContentTypeValue, command.Identity.User, command.CreationTime);
                command.ContentType.ContentTypeValue.UniqueId = udsEntityModel.IdUDS;
                ResetModelXML(command.ContentType.ContentTypeValue);

                collaborationId = udsEntityModel.Relations.Collaborations.FirstOrDefault()?.IdCollaboration;
                collaborationUniqueId = udsEntityModel.Relations.Collaborations.FirstOrDefault()?.CollaborationUniqueId;
                collaborationTemplateName = udsEntityModel.Relations.Collaborations.FirstOrDefault()?.CollaborationTemplateName;

                IEventInsertUDSData evt = new EventInsertUDSData(command.CorrelationId, command.TenantName, command.TenantId, command.Identity,
                    command.ContentType.ContentTypeValue);
                evt.CorrelatedCommands.Add(command);
                if (!await PushEventAsync(evt))
                {
                    throw new Exception("EventInsertUDSData not sent");
                }

                UDSBuildModel udsBuildModel = MapUDSModel(command.ContentType.ContentTypeValue, udsEntityModel);
                CategoryFascicle categoryFascicle = await GetPeriodicCategoryFascicleByEnvironment(udsEntityModel.IdCategory.Value, command.ContentType.ContentTypeValue.UDSRepository.DSWEnvironment);
                if (categoryFascicle == null)
                {
                    categoryFascicle = await GetDefaultCategoryFascicle(udsEntityModel.IdCategory.Value);
                }
                ICommandCQRSCreateUDSData commandCQRS = new CommandCQRSCreateUDSData(command.TenantName, command.TenantId, command.Identity, udsBuildModel, categoryFascicle, null,
                    collaborationUniqueId, collaborationId, collaborationTemplateName);
                if (command.CorrelationId.HasValue)
                {
                    commandCQRS = new CommandCQRSCreateUDSData(command.CorrelationId.Value, command.TenantName, command.TenantId, command.Identity, udsBuildModel, categoryFascicle, null,
                         collaborationUniqueId, collaborationId, collaborationTemplateName);
                }
                if (udsBuildModel.WorkflowActions != null)
                {
                    foreach (IWorkflowAction workflowAction in udsBuildModel.WorkflowActions)
                    {
                        commandCQRS.WorkflowActions.Add(workflowAction);
                    }
                }
                if (!await PushCommandAsync(commandCQRS))
                {
                    throw new Exception("CommandCQRSCreateUDSData not sent");
                }
                #region [ EventCompleteUDSBuild ]
                IEventCompleteUDSBuild eventCompleteUDSBuild = new EventCompleteUDSBuild(Guid.NewGuid(), command.CorrelationId, command.TenantName, command.TenantId,
                    command.Identity, udsBuildModel, null);
                if (!await PushEventAsync(eventCompleteUDSBuild))
                {
                    _logger.WriteError(new LogMessage($"EventCompleteUDSBuild {udsBuildModel.UniqueId} has not been sended"), LogCategories);
                    throw new Exception("IEventCompleteUDSBuild not sended");
                }
                _logger.WriteInfo(new LogMessage($"EventCompleteUDSBuild {udsBuildModel.UniqueId} has been sended"), LogCategories);
                #endregion
            }
            catch (Exception ex)
            {
                ResetModelXML(command.ContentType.ContentTypeValue);
                IEventError evt = new EventError(command.CorrelationId, command.TenantName, command.TenantId, command.Identity,
                    new ContentTypeString($"Errore in fase di inserimento nell'archivio [{ex.Message}]"), null);
                evt.CorrelatedCommands.Add(command);
                if (!await PushEventAsync(evt))
                {
                    throw new Exception("EventError not sent");
                }
                throw ex;
            }

            _logger.WriteInfo(new LogMessage("message completed."), LogCategories);
        }

        private async Task<UDSEntityModel> InsertDataAsync(UDSBuildModel model, string userName, DateTimeOffset creationTime)
        {
            UDSEntityModel udsModel;
            try
            {
                UDSDataFacade udsDataFacade = new UDSDataFacade(_logger, _biblosClient, model.XMLContent, CurrentUDSSchemaRepository.SchemaXML, DBSchema);
                udsModel = udsDataFacade.AddUDS(ConnectionString, model.UDSRepository.Id, model.UDSRepository.Name, model.Roles.Where(f => f.UniqueId.HasValue && f.IdRole.HasValue).Select(f => new AuthorizationInstance()
                {
                    AuthorizationInstanceType = AuthorizationInstanceType.Role,
                    AuthorizationType = (AuthorizationType)f.AuthorizationType,
                    IdAuthorization = f.IdRole.Value,
                    UniqueId = f.UniqueId.Value.ToString(),
                }), userName, creationTime, null, model.Year, model.Number);
                
                ///TODO: Tali attività devono essere integrate in transazione con l'inserimento della UDS. In questo momento sono fuori in quanto non risulta possibile gestire la 
                ///transazione della UDS e delle attività nelle web api                    
                await InsertRelationsAsync(udsModel, model.UDSRepository.Id, model.UDSRepository.DSWEnvironment, userName, creationTime);
                await InsertLogAsync(udsModel, model.UDSRepository.Id, model.UDSRepository.DSWEnvironment);
                await ArchivePECMailAsync(udsModel, model.UDSRepository.Id);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
            return udsModel;
        }
    }
}
