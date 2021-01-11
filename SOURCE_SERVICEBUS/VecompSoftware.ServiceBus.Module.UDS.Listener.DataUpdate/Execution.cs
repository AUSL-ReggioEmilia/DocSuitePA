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

namespace VecompSoftware.ServiceBus.Module.UDS.Listener.DataUpdate
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class Execution : UDSBaseExecution<ICommandUpdateUDSData>
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
        public override async Task ExecuteAsync(ICommandUpdateUDSData command)
        {
            _logger.WriteInfo(new LogMessage($"{command.CommandName} is arrived"), LogCategories);

            Guid? collaborationUniqueId;
            string collaborationTemplateName;
            int? collaborationId;

            try
            {
                UDSEntityModel udsEntityModel = await UpdateData(command.ContentType.ContentTypeValue, command.Identity.User, command.CreationTime);
                ResetModelXML(command.ContentType.ContentTypeValue);

                collaborationId = udsEntityModel.Relations.Collaborations.FirstOrDefault()?.IdCollaboration;
                collaborationUniqueId = udsEntityModel.Relations.Collaborations.FirstOrDefault()?.CollaborationUniqueId;
                collaborationTemplateName = udsEntityModel.Relations.Collaborations.FirstOrDefault()?.CollaborationTemplateName;

                IEventUpdateUDSData evt = new EventUpdateUDSData(Guid.NewGuid(), command.CorrelationId, command.TenantName, command.TenantId, command.TenantAOOId, command.Identity,
                    command.ContentType.ContentTypeValue, null);
                evt.CorrelatedMessages.Add(command);
                if (!await PushEventAsync(evt))
                {
                    throw new Exception("EventUpdateUDSData not sent");
                }

                CategoryFascicle categoryFascicle = await GetPeriodicCategoryFascicleByEnvironment(udsEntityModel.IdCategory.Value, command.ContentType.ContentTypeValue.UDSRepository.DSWEnvironment);
                if (categoryFascicle == null)
                {
                    categoryFascicle = await GetDefaultCategoryFascicle(udsEntityModel.IdCategory.Value);
                }

                UDSBuildModel udsBuildModel = MapUDSModel(command.ContentType.ContentTypeValue, udsEntityModel);

                ICommandCQRSUpdateUDSData commandCQRS = new CommandCQRSUpdateUDSData(command.TenantName, command.TenantId, command.TenantAOOId, command.Identity, udsBuildModel, categoryFascicle, null,
                    collaborationUniqueId, collaborationId, collaborationTemplateName);

                if (command.CorrelationId.HasValue)
                {
                    commandCQRS = new CommandCQRSUpdateUDSData(command.CorrelationId.Value, command.TenantName, command.TenantId, command.TenantAOOId, command.Identity, udsBuildModel, categoryFascicle, null,
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
                    throw new Exception("CommandCQRSUpdateUDSData not sent");
                }
            }
            catch (Exception ex)
            {
                ResetModelXML(command.ContentType.ContentTypeValue);
                IEventError evt = new EventError(command.CorrelationId, command.TenantName, command.TenantId, command.TenantAOOId, command.Identity,
                    new ContentTypeString($"Errore in fase di aggiornamento nell'archivio [{ex.Message}]"), null);
                evt.CorrelatedMessages.Add(command);
                if (!await PushEventAsync(evt))
                {
                    throw new Exception("EventError not sent");
                }
                throw ex;
            }
        }

        private async Task<UDSEntityModel> UpdateData(UDSBuildModel model, string userName, DateTimeOffset creationTime)
        {
            UDSEntityModel udsModel;
            try
            {
                UDSDataFacade df = new UDSDataFacade(_logger, _biblosClient, model.XMLContent, CurrentUDSSchemaRepository.SchemaXML, DBSchema);
                udsModel = df.UpdateUDS(ConnectionString, model.UniqueId, model.Roles.Where(f => f.UniqueId.HasValue && f.IdRole.HasValue).Select(f => new AuthorizationInstance()
                {
                    AuthorizationInstanceType = AuthorizationInstanceType.Role,
                    AuthorizationType = (AuthorizationType)f.AuthorizationType,
                    IdAuthorization = f.IdRole.Value,
                    UniqueId = f.UniqueId.Value.ToString(),
                }), userName, creationTime);
                await UpdateRelationsAsync(udsModel, model.UDSRepository.Id, model.UDSRepository.DSWEnvironment, userName, creationTime);
                await InsertLogAsync(udsModel, model.UDSRepository.Id, model.UDSRepository.DSWEnvironment);
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
