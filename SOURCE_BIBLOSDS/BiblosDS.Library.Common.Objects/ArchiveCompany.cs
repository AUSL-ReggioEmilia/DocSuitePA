using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "ArchiveCompany", Namespace = "http://BiblosDS/2009/10/ArchiveCompany")]
    public class ArchiveCompany : BiblosDSObject
    {
        public Guid IdArchive { get; set; }

        public Guid IdCompany { get; set; }

        [DataMember]
        public Company Company { get; set; }

        [DataMember]
        public string CompanyName { get; set; }

        [DataMember]
        public DocumentArchive Archive { get; set; }

        [DataMember]
        public string WorkingDir { get; set; }

        [DataMember]
        public string XmlFileTemplatePath { get; set; }

        [DataMember]
        public string TemplateXSLTFile { get; set; }

        [DataMember]
        public string AwardBatchXSLTFile { get; set; }
    }
}
