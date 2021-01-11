using System;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.Core.Command.CQRS.Events.Entities.PECMails;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Commands.Entities.PECMails;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.PECMails
{
    public class PECMailContentTypeExecutor : BaseCommonExecutor, IPECMailContentTypeExecutor
    {
        #region [ Fields ]
        private readonly IWebAPIClient _webApiClient;
        private readonly BiblosClient _biblosClient;
        private readonly ILogger _logger;
        #endregion

        #region [ Properties ]
        public Guid? CollaborationUniqueId { get; set; }
        public int? CollaborationId { get; set; }
        public string CollaborationTemplateName { get; set; }
        public Guid? ProtocolUniqueId { get; set; }
        public short? ProtocolYear { get; set; }
        public int? ProtocolNumber { get; set; }
        #endregion

        #region [ Constructor ]


        public PECMailContentTypeExecutor(ILogger logger, IWebAPIClient webApiClient, BiblosClient biblosClient, ServiceBus.ServiceBusClient serviceBusClient)
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
            PECMail pecMail;
            try
            {
                pecMail = ((ICommandCreatePECMail)command).ContentType.ContentTypeValue;

                CollaborationUniqueId = null;
                if (command.CustomProperties.Where(x => x.Key == CustomPropertyName.COLLABORATION_UNIQUE_ID).Any() && Guid.TryParse(command.CustomProperties.Where(x => x.Key == CustomPropertyName.COLLABORATION_UNIQUE_ID).FirstOrDefault().Value.ToString(), out Guid guidResult))
                {
                    CollaborationUniqueId = guidResult;
                }
                CollaborationId = null;
                if (command.CustomProperties.Where(x => x.Key == CustomPropertyName.COLLABORATION_ID).Any() && int.TryParse(command.CustomProperties.Where(x => x.Key == CustomPropertyName.COLLABORATION_ID).FirstOrDefault().Value.ToString(), out int intResult))
                {
                    CollaborationId = intResult;
                }
                CollaborationTemplateName = string.Empty;
                if (command.CustomProperties.Where(x => x.Key == CustomPropertyName.COLLABORATION_TEMPLATE_NAME).Any())
                {
                    CollaborationTemplateName = command.CustomProperties.Where(x => x.Key == CustomPropertyName.COLLABORATION_TEMPLATE_NAME).FirstOrDefault().Value.ToString();
                }
                ProtocolUniqueId = null;
                if (command.CustomProperties.Where(x => x.Key == CustomPropertyName.PROTOCOL_UNIQUE_ID).Any() && Guid.TryParse(command.CustomProperties.Where(x => x.Key == CustomPropertyName.PROTOCOL_UNIQUE_ID).FirstOrDefault().Value.ToString(), out guidResult))
                {
                    ProtocolUniqueId = guidResult;
                }
                ProtocolNumber = null;
                if (command.CustomProperties.Where(x => x.Key == CustomPropertyName.PROTOCOL_NUMBER).Any() && int.TryParse(command.CustomProperties.Where(x => x.Key == CustomPropertyName.PROTOCOL_NUMBER).FirstOrDefault().Value.ToString(), out intResult))
                {
                    ProtocolNumber = intResult;
                }
                ProtocolYear = null;
                if (command.CustomProperties.Where(x => x.Key == CustomPropertyName.PROTOCOL_YEAR).Any() && short.TryParse(command.CustomProperties.Where(x => x.Key == CustomPropertyName.PROTOCOL_YEAR).FirstOrDefault().Value.ToString(), out short shortResult))
                {
                    ProtocolYear = shortResult;
                }

                evt = new EventCreatePECMail(command.TenantName, command.TenantId, command.TenantAOOId, CollaborationUniqueId, CollaborationId, CollaborationTemplateName, ProtocolUniqueId, ProtocolYear, ProtocolNumber, false, command.Identity, pecMail, null);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("PEC, CreateInsertEvent Error: ", command.GetType())), ex, LogCategories);
                throw ex;
            }
            return evt;
        }

        internal override IEvent CreateUpdateEvent(ICommandCQRSUpdate command, DocumentUnit documentUnit = null)
        {

            IEvent evt = null;
            PECMail pecMail;
            try
            {
                pecMail = ((ICommandUpdatePECMail)command).ContentType.ContentTypeValue;

                CollaborationUniqueId = null;
                if (command.CustomProperties.Any(x => x.Key == CustomPropertyName.COLLABORATION_UNIQUE_ID) && Guid.TryParse(command.CustomProperties.Single(x => x.Key == CustomPropertyName.COLLABORATION_UNIQUE_ID).Value.ToString(), out Guid guidResult))
                {
                    CollaborationUniqueId = guidResult;
                }
                CollaborationId = null;
                if (command.CustomProperties.Any(x => x.Key == CustomPropertyName.COLLABORATION_ID) && int.TryParse(command.CustomProperties.Where(x => x.Key == CustomPropertyName.COLLABORATION_ID).FirstOrDefault().Value.ToString(), out int intResult))
                {
                    CollaborationId = intResult;
                }
                CollaborationTemplateName = string.Empty;
                if (command.CustomProperties.Any(x => x.Key == CustomPropertyName.COLLABORATION_TEMPLATE_NAME))
                {
                    CollaborationTemplateName = command.CustomProperties.Single(x => x.Key == CustomPropertyName.COLLABORATION_TEMPLATE_NAME).Value.ToString();
                }
                ProtocolUniqueId = null;
                if (command.CustomProperties.Any(x => x.Key == CustomPropertyName.PROTOCOL_UNIQUE_ID) && Guid.TryParse(command.CustomProperties.Single(x => x.Key == CustomPropertyName.PROTOCOL_UNIQUE_ID).Value.ToString(), out guidResult))
                {
                    ProtocolUniqueId = guidResult;
                }
                ProtocolNumber = null;
                if (command.CustomProperties.Any(x => x.Key == CustomPropertyName.PROTOCOL_NUMBER) && int.TryParse(command.CustomProperties.Single(x => x.Key == CustomPropertyName.PROTOCOL_NUMBER).Value.ToString(), out intResult))
                {
                    ProtocolNumber = intResult;
                }
                ProtocolYear = null;
                if (command.CustomProperties.Any(x => x.Key == CustomPropertyName.PROTOCOL_YEAR) && short.TryParse(command.CustomProperties.Single(x => x.Key == CustomPropertyName.PROTOCOL_YEAR).Value.ToString(), out short shortResult))
                {
                    ProtocolYear = shortResult;
                }

                evt = new EventUpdatePECMail(command.TenantName, command.TenantId, command.TenantAOOId, CollaborationUniqueId, CollaborationId, CollaborationTemplateName, ProtocolUniqueId, ProtocolYear, ProtocolNumber, false, command.Identity, pecMail, null);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("PEC, CreateUpdateEvent Error: ", command.GetType())), ex, LogCategories);
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
