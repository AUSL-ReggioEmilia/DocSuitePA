using System.Collections.Generic;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.Services.WSProt.DTO
{
    [XmlRoot("ProtocolStatuses")]
    [XmlInclude(typeof(ProtocolStatusDto))]
    public class ProtocolStatusesDto : IProtocolStatusesDto
    {
        #region [ Constructor ]

        public ProtocolStatusesDto()
        {
            this.ProtocolStatusDtos = new List<ProtocolStatusDto>();
        }
        #endregion

        #region [ Properties ]
        [XmlElement("ProtocolStatus")]
        public List<ProtocolStatusDto> ProtocolStatusDtos { get; set; }
        #endregion        

        #region [ Methods ]

        public void AddProtocolStatus(ProtocolStatusDto protocolStatus)
        {
            this.ProtocolStatusDtos.Add(protocolStatus);
        }
        #endregion
    }
}
