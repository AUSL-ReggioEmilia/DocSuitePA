using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Protocols;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.ZEN.Mappers
{
    public class ProtocolCreatedEventMapper : IMapper<IEventCreateProtocol, DocSuiteEvent>
    {
        #region [ Fields ]
        private readonly CollaborationReferenceMapper _collaborationReferenceEventMapper;
        #endregion

        #region [ Constructor ]
        public ProtocolCreatedEventMapper()
        {
            _collaborationReferenceEventMapper = new CollaborationReferenceMapper();
        }
        #endregion

        #region [ Methods ]
        public DocSuiteEvent Map(IEventCreateProtocol @event)
        {
            DocSuiteEvent docSuiteEvent = new DocSuiteEvent();
            Protocol protocol = @event.ContentType.ContentTypeValue;
            docSuiteEvent.EventDate = protocol.RegistrationDate;
            docSuiteEvent.WorkflowReferenceId = null; //Fase 0 da non impostare
            docSuiteEvent.EventModel = new DocSuiteModel()
            {
                Title = string.Format(DocSuiteModel.PROTOCOL_TITLE_FORMAT, protocol.Year, protocol.Number),
                UniqueId = protocol.UniqueId,
                Year = protocol.Year,
                Number = protocol.Number,
                ModelType = DocSuiteType.Protocol,
                ModelStatus = DocSuiteStatus.Activated
            };
            docSuiteEvent.ReferenceModel = _collaborationReferenceEventMapper.Map(@event.CustomProperties);
            return docSuiteEvent;
        }
        #endregion
    }
}
