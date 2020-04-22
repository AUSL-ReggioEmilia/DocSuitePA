using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.DTO.WSSeries
{
    /// <summary>
    /// Wrapper per il trasporto xml degli attributi di Biblos
    /// </summary>
    [XmlRoot("Attribute")]
    public class AttributeWSO
    {
        [XmlAttribute("Key")]
        public string Key { get; set; }

        [XmlAttribute("Value")]
        public string Value { get; set; }

        [XmlAttribute("Operator")]
        public string Operator { get; set; }
    }
}