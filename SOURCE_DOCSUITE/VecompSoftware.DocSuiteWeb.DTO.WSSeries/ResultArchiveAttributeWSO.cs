using System.Collections.Generic;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.DTO.WSSeries
{
    [XmlRoot("ResultArchiveAttribute")]
    public class ResultArchiveAttributeWSO
    {
        [XmlArray("ArchiveAttributes"), XmlArrayItem("ArchiveAttribute")]
        public List<ArchiveAttributeWSO> ArchiveAttributes { get; set; }
    }
}
