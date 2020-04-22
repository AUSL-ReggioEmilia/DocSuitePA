using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;
using VecompSoftware.Services.Command.CQRS.Events.Entities.PECMails;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.ZEN.Mappers
{
    public class PECCreatedEventMapper : IMapper<IEventCreatePECMail, DocSuiteEvent>
    {
        #region [ Fields ]
        private readonly ProtocolReferenceMapper _protocolReferenceEventMapper;
        #endregion

        #region [ Constructor ]
        public PECCreatedEventMapper()
        {
            _protocolReferenceEventMapper = new ProtocolReferenceMapper();
        }
        #endregion

        #region [ Methods ]
        public DocSuiteEvent Map(IEventCreatePECMail @event)
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
                ModelStatus = DocSuiteStatus.Sended
            };

            docSuiteEvent.ReferenceModel = _protocolReferenceEventMapper.Map(@event.CustomProperties);
            return docSuiteEvent;
        }
        #endregion
    }
}
