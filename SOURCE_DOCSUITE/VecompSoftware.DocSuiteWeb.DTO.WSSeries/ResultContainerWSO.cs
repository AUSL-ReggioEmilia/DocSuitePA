using System.Collections.Generic;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.DTO.WSSeries
{
    [XmlRoot("ResultContainer")]
    public class ResultContainerWSO
    {
        [XmlArray("Containers"), XmlArrayItem("Container")]
        public List<ContainerWSO> Containers { get; set; }
    }
}