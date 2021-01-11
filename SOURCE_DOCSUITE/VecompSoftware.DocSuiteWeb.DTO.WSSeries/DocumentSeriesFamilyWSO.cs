using System.Collections.Generic;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.DTO.WSSeries
{
    [XmlRoot("DocumentSeriesFamily")]
    public class DocumentSeriesFamilyWSO
    {
        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlArray("DocumentSeriesList"), XmlArrayItem("DocumentSeries")]
        public List<DocumentSeriesWSO> DocumentSeries { get; set; }
    }
}
