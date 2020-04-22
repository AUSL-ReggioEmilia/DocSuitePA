using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    [XmlRoot("Address")]
    public class ContactAddressDraftModel
    {
        #region [ Constructor ]

        public ContactAddressDraftModel()
        {
        }

        #endregion

        #region [ Properties ]

        [XmlAttribute("cap")]
        public string Cap { get; set; }

        [XmlAttribute("city")]
        public string City { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("number")]
        public string Number { get; set; }

        [XmlAttribute("prov")]
        public string Prov { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        #endregion

    }
}
