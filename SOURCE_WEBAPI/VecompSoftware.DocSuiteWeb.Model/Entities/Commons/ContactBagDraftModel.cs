using System.Collections.Generic;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    [XmlRoot("ContactBag")]
    public class ContactBagDraftModel
    {
        #region [ Constructor ]

        public ContactBagDraftModel()
        {
            Contacts = new List<ContactDraftModel>();
        }

        #endregion

        #region [ Properties ]

        [XmlAttribute("sourceType")]
        public int SourceType { get; set; }

        [XmlElement("Contact")]
        public List<ContactDraftModel> Contacts { get; set; }

        #endregion

    }
}
