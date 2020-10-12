using System;
using System.Threading.Tasks;
using VecompSoftware.Core.Command.CQRS.Events.Models.UDS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.ServiceBus.Module.UDS.Storage;
using VecompSoftware.ServiceBus.Module.UDS.Storage.Relations;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.Services.Command.CQRS.Events.Models.UDS;

namespace VecompSoftware.ServiceBus.Module.UDS.Listener.DataDelete
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class Execution : UDSBaseExecution<ICommandDeleteUDSData>
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

        public override async Task ExecuteAsync(ICommandDeleteUDSData command)
        {
            _logger.WriteInfo(new LogMessage(string.Concat(command.CommandName, " is arrived")), LogCategories);
            UDSEntityModel udsEntityModel = await CancelDataAsync(command.ContentType.ContentTypeValue, command.Identity.User, command.CreationTime);
            IEventDeleteUDSData evt = new EventDeleteUDSData(Guid.NewGuid(), command.CorrelationId, command.TenantName, command.TenantId, command.TenantAOOId, command.Identity,
                command.ContentType.ContentTypeValue, null);
            evt.CorrelatedMessages.Add(command);
            if (!await PushEventAsync(evt))
            {
                throw new Exception("EventDeleteUDSData not sent");
            }

            UDSBuildModel udsBuildModel = MapUDSModel(command.ContentType.ContentTypeValue, udsEntityModel);

            IEventCompleteUDSDelete eventCompleteUDSDelete = new EventCompleteUDSDelete(Guid.NewGuid(), command.CorrelationId, command.TenantName, command.TenantId,
                command.TenantAOOId, command.Identity, udsBuildModel, null);
            if (!await PushEventAsync(eventCompleteUDSDelete))
            {
                _logger.WriteError(new LogMessage($"EventCompleteUDSDelete {udsBuildModel.UniqueId} has not been sended"), LogCategories);
                throw new Exception("IEventCompleteUDSDelete not sended");
            }
            _logger.WriteInfo(new LogMessage($"EventCompleteUDSDelete {udsBuildModel.UniqueId} has been sended"), LogCategories);
        }

        private async Task<UDSEntityModel> CancelDataAsync(UDSBuildModel model, string userName, DateTimeOffset creationTime)
        {
            UDSEntityModel udsEntityModel;
            try
            {
                UDSDataFacade udsDataFacade = new UDSDataFacade(_logger, _biblosClient, model.XMLContent, CurrentUDSSchemaRepository.SchemaXML, DBSchema);
                udsEntityModel = udsDataFacade.CancelUDS(ConnectionString, model.UniqueId, userName, creationTime, model.CancelMotivation);
                await InsertLogAsync(udsEntityModel, model.UDSRepository.Id, model.UDSRepository.DSWEnvironment);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
            return udsEntityModel;
        }
    }
}
