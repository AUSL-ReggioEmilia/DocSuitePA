using System;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Core.Command.CQRS.Events.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Commands.Entities.Dossiers;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Dossiers
{
    public class DossierContentTypeExecutor : BaseCommonExecutor, IDossierContentTypeExecutor
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]
        public DossierContentTypeExecutor(ILogger logger, IWebAPIClient webApiClient, BiblosClient biblosClient, ServiceBus.ServiceBusClient serviceBusClient)
            : base(logger, webApiClient, biblosClient, serviceBusClient)
        {
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        internal override IEvent CreateInsertEvent(ICommandCQRSCreate command, DocumentUnit documentUnit = null)
        {
            IEvent evt = null;
            Dossier dossier;
            try
            {
                dossier = ((ICommandCreateDossier)command).ContentType.ContentTypeValue;
                evt = new EventCreateDossier(command.TenantName, command.TenantId, command.TenantAOOId, command.Identity, dossier);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"Dossier, CreateInsertEvent Error: {command.GetType()}"), ex, LogCategories);
                throw ex;
            }
            return evt;
        }

        internal override IEvent CreateUpdateEvent(ICommandCQRSUpdate command, DocumentUnit documentUnit = null)
        {
            IEvent evt = null;
            Dossier dossier;
            try
            {
                dossier = ((ICommandUpdateDossier)command).ContentType.ContentTypeValue;
                evt = new EventUpdateDossier(command.TenantName, command.TenantId, command.TenantAOOId, command.Identity, dossier);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"Dossier, CreateUpdateEvent Error: {command.GetType()}"), ex, LogCategories);
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
