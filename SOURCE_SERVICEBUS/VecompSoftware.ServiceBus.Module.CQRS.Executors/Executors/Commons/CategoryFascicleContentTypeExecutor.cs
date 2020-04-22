using System;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Core.Command.CQRS.Events.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Commands.Entities.Commons;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Commons
{
    public class CategoryFascicleContentTypeExecutor : BaseCommonExecutor, ICategoryFascicleContentTypeExecutor
    {
        #region [ Fields ]
        private readonly IWebAPIClient _webApiClient;
        private readonly BiblosClient _biblosClient;
        private readonly ILogger _logger;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]


        public CategoryFascicleContentTypeExecutor(ILogger logger, IWebAPIClient webApiClient, BiblosClient biblosClient)
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
            CategoryFascicle categoryFascicle;
            try
            {
                categoryFascicle = ((ICommandCreateCategoryFascicle)command).ContentType.ContentTypeValue;
                evt = new EventCreateCategoryFascicle(command.TenantName, command.TenantId, command.Identity, categoryFascicle);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("CategoryFascicle, CreateInsertEvent Error: ", command.GetType())), ex, LogCategories);
                throw ex;
            }
            return evt;
        }

        internal override IEvent CreateUpdateEvent(ICommandCQRSUpdate command, DocumentUnit documentUnit = null)
        {

            IEvent evt = null;
            CategoryFascicle categoryFascicle;
            try
            {
                categoryFascicle = ((ICommandDeleteCategoryFascicle)command).ContentType.ContentTypeValue;
                evt = new EventDeleteCategoryFascicle(command.TenantName, command.TenantId, command.Identity, categoryFascicle);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("CategoryFascicle, CreateUpdateEvent Error: ", command.GetType())), ex, LogCategories);
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
