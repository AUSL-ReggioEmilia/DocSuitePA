using System;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Core.Command.CQRS.Events.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Commands.Entities.Fascicles;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Fascicles
{
    public class FascicleContentTypeExecutor : BaseCommonExecutor, IFascicleContentTypeExecutor
    {
        #region [ Fields ]
        private readonly IWebAPIClient _webApiClient;
        private readonly BiblosClient _biblosClient;
        private readonly ILogger _logger;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        public FascicleContentTypeExecutor(ILogger logger, IWebAPIClient webApiClient, BiblosClient biblosClient)
            : base(logger, webApiClient, biblosClient)
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
            Fascicle fascicle;
            try
            {
                fascicle = ((ICommandCreateFascicle)command).ContentType.ContentTypeValue;
                evt = new EventCreateFascicle(command.TenantName, command.TenantId, command.Identity, fascicle);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("Fascicle, CreateInsertEvent Error: ", command.GetType())), ex, LogCategories);
                throw ex;
            }
            return evt;
        }

        internal override IEvent CreateUpdateEvent(ICommandCQRSUpdate command, DocumentUnit documentUnit = null)
        {
            IEvent evt = null;
            Fascicle fascicle;
            try
            {
                fascicle = ((ICommandUpdateFascicle)command).ContentType.ContentTypeValue;
                evt = new EventUpdateFascicle(command.TenantName, command.TenantId, command.Identity, fascicle);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("Fascicle, CreateUpdateEvent Error: ", command.GetType())), ex, LogCategories);
                throw ex;
            }
            return evt;

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
