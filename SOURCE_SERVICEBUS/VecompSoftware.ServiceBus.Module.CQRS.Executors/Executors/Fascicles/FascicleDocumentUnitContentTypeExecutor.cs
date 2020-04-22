using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Core.Command.CQRS.Events.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Commands.Entities.Fascicles;
using VecompSoftware.Services.Command.CQRS.Events;
using ReferenceType = VecompSoftware.DocSuiteWeb.Entity.Fascicles.ReferenceType;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Fascicles
{
    public class FascicleDocumentUnitContentTypeExecutor : BaseCommonExecutor, IFascicleDocumentUnitContentTypeExecutor
    {
        #region [ Fields ]

        private readonly IWebAPIClient _webApiClient;
        private readonly BiblosClient _biblosClient;
        private readonly ILogger _logger;

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        public FascicleDocumentUnitContentTypeExecutor(ILogger logger, IWebAPIClient webApiClient, BiblosClient biblosClient) 
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
            FascicleDocumentUnit fascicleDocumentUnit;
            try
            {
                fascicleDocumentUnit = ((ICommandCreateFascicleDocumentUnit)command).ContentType.ContentTypeValue;
                if (fascicleDocumentUnit.ReferenceType == ReferenceType.Reference)
                {
                    Task.Run(async () => await InsertFascicleDocumentUnitCategory(fascicleDocumentUnit));
                }
                evt = new EventCreateFascicleDocumentUnit(command.TenantName, command.TenantId, command.Identity, fascicleDocumentUnit);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"FascicleDocumentUnit, CreateInsertEvent Error: {command.GetType()}"), ex, LogCategories);
                throw ex;
            }
            return evt;
        }

        internal override IEvent CreateUpdateEvent(ICommandCQRSUpdate command, DocumentUnit documentUnit = null)
        {
            IEvent evt = null;
            FascicleDocumentUnit fascicleDocumentUnit;
            try
            {
                fascicleDocumentUnit = ((ICommandDeleteFascicleDocumentUnit)command).ContentType.ContentTypeValue;
                if (fascicleDocumentUnit.ReferenceType == ReferenceType.Reference)
                {
                    Task.Run(async () => await DeleteFascicleDocumentUnitCategory(fascicleDocumentUnit));
                }
                evt = new EventDeleteFascicleDocumentUnit(command.TenantName, command.TenantId, command.Identity, fascicleDocumentUnit);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"FascicleDocumentUnit, CreateUpdateEvent Error: {command.GetType()}"), ex, LogCategories);
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

        private async Task InsertFascicleDocumentUnitCategory(FascicleDocumentUnit fascicleDocumentUnit)
        {
            try
            {
                DocumentUnitFascicleCategory documentUnitFascicleCategory = new DocumentUnitFascicleCategory
                {
                    Category = fascicleDocumentUnit.Fascicle.Category,
                    DocumentUnit = fascicleDocumentUnit.DocumentUnit,
                    Fascicle = fascicleDocumentUnit.Fascicle
                };
                DocumentUnitFascicleHistoricizedCategory documentUnitFascicleHistoricizedCategory = new DocumentUnitFascicleHistoricizedCategory
                {
                    UnfascicolatedDate = fascicleDocumentUnit.RegistrationDate,
                    ReferencedFascicle = $"{fascicleDocumentUnit.Fascicle.Title} - {fascicleDocumentUnit.Fascicle.FascicleObject}",
                    Category = fascicleDocumentUnit.Fascicle.Category,
                    DocumentUnit = fascicleDocumentUnit.DocumentUnit
                };

                await PostDocumentUnitFascicleCategory(documentUnitFascicleCategory);
                _logger.WriteDebug(new LogMessage($"DocumentUnitFascicleCategory - {documentUnitFascicleCategory.GetType()} - {documentUnitFascicleCategory.UniqueId} has been successfully created."), LogCategories);

                await PostDocumentUnitFascicleHistoricizedCategory(documentUnitFascicleHistoricizedCategory);
                _logger.WriteDebug(new LogMessage($"DocumentUnitFascicleHistoricizedCategory - {documentUnitFascicleHistoricizedCategory.GetType()} - {documentUnitFascicleHistoricizedCategory.UniqueId} has been successfully created."), LogCategories);

            }
            catch(Exception ex)
            {
                _logger.WriteError(new LogMessage("FascicleDocumentUnit, InsertFascicleDocumentUnitCategory Error: "), ex, LogCategories);
                throw ex;
            }
        }

        private async Task DeleteFascicleDocumentUnitCategory(FascicleDocumentUnit fascicleDocumentUnit)
        {
            try
            {
                DocumentUnitFascicleCategory documentUnitFascicleCategory = await GetDocumentUnitFascicleCategory(fascicleDocumentUnit);

                if (documentUnitFascicleCategory == null)
                {
                    _logger.WriteWarning(new LogMessage($"DocumentUnitFascicleCategory was not found for FascicleDocumentUnitId - {fascicleDocumentUnit.UniqueId}"), LogCategories);
                    throw new Exception($"DocumentUnitFascicleCategory was not found for FascicleDocumentUnitId - {fascicleDocumentUnit.UniqueId}");
                }

                await DeleteDocumentUnitFascicleCategory(documentUnitFascicleCategory);
                _logger.WriteDebug(new LogMessage($"DocumentUnitFascicleHistoricizedCategory - {documentUnitFascicleCategory.GetType()} - {documentUnitFascicleCategory.UniqueId} has been successfully deleted."), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("FascicleDocumentUnit, DeleteFascicleDocumentUnitCategory Error: "), ex, LogCategories);
                throw ex;
            }
        }

        #endregion
    }
}
