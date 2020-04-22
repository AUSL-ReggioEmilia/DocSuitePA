using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    [XmlRoot("Contact")]
    public class ContactDraftModel
    {
        #region [ Constructor ]

        public ContactDraftModel()
        {

        }

        #endregion

        #region [ Properties ]

        [XmlElement("Address")]
        public ContactAddressDraftModel Address { get; set; }

        [XmlElement("BirthDate")]
        public string BirthDate { get; set; }
        [XmlElement("BirthPlace")]
        public string BirthPlace { get; set; }

        [XmlAttribute("cc")]
        public bool Cc { get; set; }

        [XmlElement("CertifiedMail")]
        public string CertifiedMail { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlElement("Fax")]
        public string Fax { get; set; }

        [XmlElement("FiscalCode")]
        public string FiscalCode { get; set; }

        [XmlElement("Id")]
        public int Id { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Notes")]
        public string Notes { get; set; }

        [XmlElement("StandardMail")]
        public string StandardMail { get; set; }

        [XmlElement("Surname")]
        public string Surname { get; set; }

        [XmlElement("Telephone")]
        public string Telephone { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        #endregion

    }
}
