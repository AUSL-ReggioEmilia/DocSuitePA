using System;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.DTO.WSSeries
{
    /// <summary>
    /// Wrapper per il trasporto di documenti con guid e rispettivo stream
    /// </summary>
    [XmlRoot("Doc")]
    public class DocWSO
    {
        [XmlAttribute("Server")]
        public string Server { get; set; }

        [XmlAttribute("Id")]
        public Guid Id { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlText]
        public string Stream { get; set; }
    }
}