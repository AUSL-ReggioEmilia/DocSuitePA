using System.Xml.Serialization;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.Services.WSProt.DTO
{
    [XmlRoot("ProtocolStatus")]
    public class ProtocolStatusDto : IProtocolStatusDto
    {
        #region [ Costructor ]
        public ProtocolStatusDto() { }
        #endregion

        #region [ Properties ]
        [XmlAttribute("Incremental")]
        public int Incremental { get; set; }

        [XmlAttribute("StatusCode")]
        public string StatusCode { get; set; }

        [XmlAttribute("StatusDescription")]
        public string StatusDescription { get; set; }
        #endregion

        #region [ Methods ]
        public ProtocolStatusDto MappingFromEntity(ProtocolStatus entity)
        {
            ProtocolStatusDto dto = new ProtocolStatusDto
            {
                Incremental = entity.Incremental.HasValue ? entity.Incremental.Value : 0,
                StatusCode = entity.Id,
                StatusDescription = entity.Description
            };
            return dto;
        }
        #endregion        
    }
}
