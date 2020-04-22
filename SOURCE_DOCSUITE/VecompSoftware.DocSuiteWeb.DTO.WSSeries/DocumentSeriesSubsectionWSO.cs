using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.DTO.WSSeries
{
    [XmlRoot("DocumentSeriesSubsection")]
    public class DocumentSeriesSubsectionWSO
    {
        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public int SortOrder { get; set; }

        [XmlElement]
        public string Notes { get; set; }       
    }
}
