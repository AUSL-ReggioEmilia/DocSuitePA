using System.Collections.Generic;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.DTO.WSSeries
{
    /// <summary>
    /// Wrapper per incapsulare una lista di classi Doc
    /// </summary>
    [XmlRootAttribute("DocResult")]
    public class DocResultWSO
    {
        [XmlArray("Docs"), XmlArrayItem("Doc")]
        public List<DocWSO> Docs { get; set; }
    }
}
