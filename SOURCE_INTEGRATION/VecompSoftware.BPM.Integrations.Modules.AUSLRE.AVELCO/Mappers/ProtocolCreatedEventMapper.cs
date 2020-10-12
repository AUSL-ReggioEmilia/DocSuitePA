using System.Collections.Generic;
using System.Linq;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Protocols;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.AVELCO.Mappers
{
    public class ProtocolCreatedEventMapper : IMapper<IEventCreateProtocol, DocSuiteEvent>
    {
        #region [ Fields ]
        private readonly CollaborationReferenceMapper _collaborationReferenceEventMapper;
        private readonly IWebAPIClient _webAPIClient;
        #endregion

        #region [ Constructor ]
        public ProtocolCreatedEventMapper(IWebAPIClient webAPIClient)
        {
            _collaborationReferenceEventMapper = new CollaborationReferenceMapper();
            _webAPIClient = webAPIClient;
        }
        #endregion

        #region [ Methods ]
        public DocSuiteEvent Map(IEventCreateProtocol @event)
        {
            Protocol protocol = @event.ContentType.ContentTypeValue;
            protocol = (_webAPIClient.GetProtocolAsync($"$filter=UniqueId eq {protocol.UniqueId}&$expand=ProtocolType").Result).SingleOrDefault();

            DocSuiteEvent docSuiteEvent = new DocSuiteEvent();
            docSuiteEvent.EventDate = protocol.RegistrationDate;
            docSuiteEvent.WorkflowReferenceId = null;
            docSuiteEvent.EventModel = new DocSuiteModel()
            {
                Title = string.Format(DocSuiteModel.PROTOCOL_TITLE_FORMAT, protocol.Year, protocol.Number),
                UniqueId = protocol.UniqueId,
                Year = protocol.Year,
                Number = protocol.Number,
                ModelType = DocSuiteType.Protocol,
                ModelStatus = DocSuiteStatus.Activated
            };
            docSuiteEvent.EventModel.CustomProperties = new Dictionary<string, string>
            {
                { "ProtocolDirection", ((ProtocolTypology)protocol.ProtocolType.EntityShortId).ToString() }
            };
            docSuiteEvent.ReferenceModel = _collaborationReferenceEventMapper.Map(@event.CustomProperties);
            return docSuiteEvent;
        }
        #endregion
    }
}
