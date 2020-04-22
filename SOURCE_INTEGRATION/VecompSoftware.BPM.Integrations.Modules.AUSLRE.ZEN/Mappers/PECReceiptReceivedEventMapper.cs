using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;
using VecompSoftware.Services.Command.CQRS.Events.Entities.PECMails;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.ZEN.Mappers
{
    public class PECReceiptReceivedEventMapper : IMapper<IEventReceivedReceiptPECMail, DocSuiteEvent>
    {
        #region [ Fields ]
        private readonly ProtocolReferenceMapper _protocolReferenceEventMapper;
        #endregion

        #region [ Constructor ]
        public PECReceiptReceivedEventMapper()
        {
            _protocolReferenceEventMapper = new ProtocolReferenceMapper();
        }
        #endregion

        #region [ Methods ]
        public DocSuiteEvent Map(IEventReceivedReceiptPECMail @event)
        {
            DocSuiteEvent docSuiteEvent = new DocSuiteEvent();
            PECMail pecMail = @event.ContentType.ContentTypeValue;
            docSuiteEvent.EventDate = pecMail.RegistrationDate;
            docSuiteEvent.WorkflowReferenceId = null; //Fase 0 da non impostare
            docSuiteEvent.EventModel = new DocSuiteModel()
            {
                Title = string.Format(DocSuiteModel.PEC_TITLE_FORMAT, pecMail.EntityId),
                UniqueId = pecMail.UniqueId,
                EntityId = pecMail.EntityId,
                ModelType = DocSuiteType.PEC,
                ModelStatus = MapReceiptStatus(pecMail),
                CustomProperties = new Dictionary<string, string>() { { "Sender", pecMail.MailRecipients } }
            };

            docSuiteEvent.ReferenceModel = _protocolReferenceEventMapper.Map(@event.CustomProperties);
            return docSuiteEvent;
        }

        private DocSuiteStatus MapReceiptStatus(PECMail receipt)
        {
            if (receipt == null || string.IsNullOrEmpty(receipt.MailType))
            {
                throw new ArgumentNullException(nameof(PECMail.MailType), "MapReceiptStatus -> received PEC message without mail type.");
            }

            switch (receipt.MailType)
            {
                case "avvenuta-consegna":
                    return DocSuiteStatus.Received;
                default:
                    return DocSuiteStatus.Rejected;
            }
        }
        #endregion
    }
}
