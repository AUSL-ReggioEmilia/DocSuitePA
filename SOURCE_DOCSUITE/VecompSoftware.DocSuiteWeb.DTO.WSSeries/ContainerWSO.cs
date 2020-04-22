using System.Collections.Generic;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.DTO.WSSeries
{
    [XmlRoot("Container")]
    public class ContainerWSO
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("IdDocumentSeries")]
        public int? IdDocumentSeries { get; set; }

        [XmlElement("PublicationEnabled")]
        public bool? PublicationEnabled { get; set; }

        [XmlArray("DocumentSeriesSubsections"), XmlArrayItem("DocumentSeriesSubsection")]
        public List<DocumentSeriesSubsectionWSO> DocumentSeriesSubsections { get; set; }
    }
}