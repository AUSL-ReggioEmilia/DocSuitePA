using System;
using System.Xml.Serialization;

namespace VecompSoftware.JeepService.LogConservation.Models
{
    [Serializable]
    public class ReferenceModel
    {
        [XmlElement("Anno", IsNullable = true)]
        public int? Year { get; set; }

        [XmlIgnore]
        public bool YearSpecified
        {
            get
            {
                return Year.HasValue;
            }
        }

        [XmlElement("Numero", IsNullable = true)]
        public int? Number { get; set; }

        [XmlIgnore]
        public bool NumberSpecified
        {
            get
            {
                return Number.HasValue;
            }
        }

        [XmlElement("IdentificativoUnivoco")]
        public Guid? ReferenceUniqueId { get; set; }

        [XmlElement("Oggetto")]
        public string Subject { get; set; }
    }
}
