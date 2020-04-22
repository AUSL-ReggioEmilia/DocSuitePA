using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Protocols
{

    /// <summary> classe per il parsing XML dell'oggetto Protocol. </summary>
    [XmlRoot("Protocol")]
    public class ProtocolDraftModel
    {
        #region [ Constructor ]

        public ProtocolDraftModel()
        {
            Authorizations = new List<int>();
            Recipients = new List<ContactBagDraftModel>();
            Senders = new List<ContactBagDraftModel>();
        }

        #endregion

        #region [ Properties ]

        [XmlElement("Assignee")]
        public string Assignee { get; set; }

        [XmlElement("Category")]
        public int Category { get; set; }

        [XmlElement("Container")]
        public int Container { get; set; }

        [XmlElement("Data")]
        public string Data { get; set; }

        [XmlElement("DocumentType")]
        public int DocumentType { get; set; }

        [XmlElement("IdStatus")]
        public int IdStatus { get; set; }

        [XmlElement("Notes")]
        public string Notes { get; set; }

        [XmlAttribute("Year")]
        public int Year { get; set; }

        [XmlAttribute("Number")]
        public int Number { get; set; }

        [XmlElement("Object")]
        public string Object { get; set; }

        [XmlElement("RegistrationDate")]
        public DateTimeOffset RegistrationDate { get; set; }

        [XmlElement("ServiceCode")]
        public string ServiceCode { get; set; }

        [XmlElement("Type")]
        public int Type { get; set; }

        [XmlArray("Authorizations")]
        [XmlArrayItem("RoleId")]
        public List<int> Authorizations { get; set; }

        [XmlArray("Recipients")]
        [XmlArrayItem("ContactBag")]
        public List<ContactBagDraftModel> Recipients { get; set; }

        [XmlArray("Senders")]
        [XmlArrayItem("ContactBag")]
        public List<ContactBagDraftModel> Senders { get; set; }

        #endregion

    }
}
