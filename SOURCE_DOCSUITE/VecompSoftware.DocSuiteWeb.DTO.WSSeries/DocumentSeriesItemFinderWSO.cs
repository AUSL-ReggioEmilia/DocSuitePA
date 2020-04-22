using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.DTO.WSSeries
{
    /// <summary>
    /// Wrapper 
    /// </summary>
    [XmlRootAttribute("DocumentSeriesItemFinder")]
    public class DocumentSeriesItemFinderWSO
    {
        [XmlAttribute]
        public int IdDocumentSeries { get; set; }

        [XmlAttribute]
        public string ImpersonatingUser { get; set; }

        [XmlArray("IdDocumentSeriesSubsections"), XmlArrayItem("IdDocumentSeriesSubsection")]
        public List<int> IdDocumentSeriesSubsections { get; set; }

        [XmlElement]
        public int? Year { get; set; }

        [XmlElement]
        public int? NumberFrom { get; set; }

        [XmlElement]
        public int? NumberTo { get; set; }

        [XmlElement]
        public string SubjectContains { get; set; }

        [XmlElement]
        public string SubjectStartsWith { get; set; }

        [XmlElement]
        public string CategoryPath { get; set; }

        [XmlElement]
        public DateTime? RegistrationDateFrom { get; set; }

        [XmlElement]
        public DateTime? RegistrationDateTo { get; set; }

        [XmlElement]
        public DateTime? RetireDateFrom { get; set; }

        [XmlElement]
        public DateTime? RetireDateTo { get; set; }

        [XmlElement]
        public DateTime? PublishingDateFrom { get; set; }

        [XmlElement]
        public DateTime? PublishingDateTo { get; set; }

        [XmlElement]
        public int? PublishingYear { get; set; }

        [XmlElement]
        public bool? EnablePaging { get; set; }
        
        [XmlElement]
        public bool? IsPublished { get; set; }

        [XmlElement]
        public bool? IsRetired { get; set; }

        [XmlElement]
        public int? Skip { get; set; }

        [XmlElement]
        public int? Take { get; set; }

        [XmlElement]
        public bool? IsPriority { get; set; }

        [XmlElement]
        public bool IncludeSubsections { get; set; }

        [XmlElement]
        public bool? LastModifiedSortingView { get; set; }

        [XmlElement]
        public bool? FindByConstraints { get; set; }

        [XmlElement]
        public string Constraint { get; set; }

        [XmlArray("DynamicData"), XmlArrayItem("Attribute")]
        public List<AttributeWSO> DynamicData{get; set;}
    }

    [XmlRootAttribute("DocumentSeriesItemResult")]
    public class DocumentSeriesItemResultWSO
    {
        [XmlElement]
        public int TotalRowCount { get; set; }

        [XmlArray("DocumentSeriesItems"), XmlArrayItem("DocumentSeriesItem")]
        public List<DocumentSeriesItemWSO> DocumentSeriesItems { get; set; }
    }
}