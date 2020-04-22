using System.Collections.Generic;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.DTO.WSSeries
{
    [XmlRoot("ResultDocumentSeriesFamily")]
    public class ResultDocumentSeriesFamilyWSO
    {
        [XmlArray("DocumentSeriesFamilies"), XmlArrayItem("DocumentSeriesFamily")]
        public List<DocumentSeriesFamilyWSO> DocumentSeriesFamilies { get; set; }
    }
}