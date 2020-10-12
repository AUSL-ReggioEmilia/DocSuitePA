using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Protocols;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.AVELCO.Mappers
{
    public class ProtocolCanceledEventMapper : IMapper<IEventCancelProtocol, DocSuiteEvent>
    {
        #region [ Fields ]
        private readonly CollaborationReferenceMapper _collaborationReferenceEventMapper;
        #endregion

        #region [ Constructor ]
        public ProtocolCanceledEventMapper()
        {
            _collaborationReferenceEventMapper = new CollaborationReferenceMapper();
        }
        #endregion

        #region [ Methods ]
        public DocSuiteEvent Map(IEventCancelProtocol @event)
        {
            DocSuiteEvent docSuiteEvent = new DocSuiteEvent();
            Protocol protocol = @event.ContentType.ContentTypeValue;
            docSuiteEvent.EventDate = protocol.LastChangedDate.Value;
            docSuiteEvent.WorkflowReferenceId = null; //Fase 0 da non impostare
            docSuiteEvent.EventModel = new DocSuiteModel()
            {
                Title = string.Format(DocSuiteModel.PROTOCOL_TITLE_FORMAT, protocol.Year, protocol.Number),
                UniqueId = protocol.UniqueId,
                Year = protocol.Year,
                Number = protocol.Number,
                ModelType = DocSuiteType.Protocol,
                ModelStatus = DocSuiteStatus.Canceled
            };
            docSuiteEvent.ReferenceModel = _collaborationReferenceEventMapper.Map(@event.CustomProperties);
            return docSuiteEvent;
        }
        #endregion
    }
}
