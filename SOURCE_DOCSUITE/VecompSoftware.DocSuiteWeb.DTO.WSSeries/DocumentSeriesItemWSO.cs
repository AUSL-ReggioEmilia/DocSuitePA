using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.DTO.WSSeries
{
    /// <summary>
    /// Wrapper di DocumentSeriesItem per il trasporto dei dati in xml
    /// </summary>
    [XmlRootAttribute("DocumentSeriesItem")]
    public class DocumentSeriesItemWSO
    {
        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public int IdDocumentSeries { get; set; }

        [XmlElement]
        public int? IdDocumentSeriesSubsection { get; set; }

        [XmlElement]
        public int? Year { get; set; }

        [XmlElement]
        public int? Number { get; set; }

        [XmlElement]
        public int IdLocation { get; set; }

        [XmlElement]
        public int IdLocationAnnexed { get; set; }

        [XmlElement]
        public DateTime? PublishingDate { get; set; }

        [XmlElement]
        public int? PublishingYear
        {
            get
            {
                if (PublishingDate.HasValue)
                {
                    return PublishingDate.Value.Year;
                }
                return null;
            }
        }

        [XmlElement]
        public DateTime? RetireDate { get; set; }

        [XmlElement]
        public string RegistrationUser { get; set; }

        [XmlElement]
        public DateTime? RegistrationDate { get; set; }

        [XmlElement]
        public string LastChangedUser { get; set; }

        [XmlElement]
        public DateTime? LastChangedDate { get; set; }

        [XmlElement]
        public short? Status { get; set; }

        [XmlElement]
        public string Subject { get; set; }

        [XmlElement]
        public int? IdCategory { get; set; }

        [XmlElement]
        public int? IdSubCategory { get; set; }

        [XmlElement]
        public bool? Priority { get; set; }

        [XmlElement("Container")]
        public ContainerWSO Container { get; set; }

        [XmlArray("DynamicData"), XmlArrayItem("Attribute")]
        public List<AttributeWSO> DynamicData { get; set; }

        [XmlArray("MainDocs"), XmlArrayItem("Doc")]
        public List<DocWSO> MainDocs { get; set; }

        [XmlArray("AnnexedDocs"), XmlArrayItem("Doc")]
        public List<DocWSO> AnnexedDocs { get; set; }
        [XmlArray("UnPublishedDocs"), XmlArrayItem("Doc")]
        public List<DocWSO> UnPublishedDocs { get; set; }

    }
}


