using System;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Core.Command.CQRS.Events.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Commands.Entities.Collaborations;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Collaborations
{
    public class CollaborationContentTypeExecutor : BaseCommonExecutor, ICollaborationContentTypeExecutor
    {
        #region [ Fields ]
        private readonly IWebAPIClient _webApiClient;
        private readonly BiblosDS.BiblosClient _biblosClient;
        private readonly ILogger _logger;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public CollaborationContentTypeExecutor(ILogger logger, IWebAPIClient webApiClient, BiblosClient biblosClient, ServiceBus.ServiceBusClient serviceBusClient)
            : base(logger, webApiClient, biblosClient, serviceBusClient)
        {
            _logger = logger;
            _webApiClient = webApiClient;
            _biblosClient = biblosClient;
        }

        #endregion

        #region [ Methods ]

        internal override IEvent CreateInsertEvent(ICommandCQRSCreate command, DocumentUnit documentUnit = null)
        {
            IEvent evt = null;
            Collaboration collaboration;
            try
            {
                collaboration = ((ICommandCreateCollaboration)command).ContentType.ContentTypeValue;
                evt = new EventCreateCollaboration(command.Id, command.CorrelationId, command.TenantName, command.TenantId, command.TenantAOOId, command.Identity, collaboration, command.ScheduledTime);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("Collaboration, CreateInsertEvent Error: ", command.GetType())),
                    ex, LogCategories);
                throw ex;
            }
            return evt;
        }
        internal override IEvent CreateUpdateEvent(ICommandCQRSUpdate command, DocumentUnit documentUnit = null)
        {
            return null;
        }

        internal override Task<DocumentUnit> MappingInsertAsync(IContentBase entity, IIdentityContext identity)
        {
            return null;
        }

        internal override Task<DocumentUnit> MappingUpdateAsync(IContentBase entity, DocumentUnit documentUnit, IIdentityContext identity)
        {
            return null;
        }

        #endregion
    }
}
