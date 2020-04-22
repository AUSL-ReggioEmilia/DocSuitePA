using System;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.DTO.WSSeries
{
    [XmlRoot("ArchiveAttribute")]
    public class ArchiveAttributeWSO
    {
        [XmlElement]
        public string Format { get; set; }

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public bool Required { get; set; }

        [XmlElement]
        public string DataType { get; set; }

        [XmlElement]
        public string DefaultValue { get; set; }

        [XmlElement]
        public string Description { get; set; }

        [XmlElement]
        public bool Disabled { get; set; }

        [XmlElement]
        public Guid Id { get; set; }

        [XmlElement]
        public Boolean AutoIncremental { get; set; }

        [XmlElement]
        public int? MaxLength { get; set; }
    }
}