using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{  
    [DataContract(Name = "Company", Namespace = "http://BiblosDS/2009/10/Company")]
    public class Company : BiblosDSObject
    {
        [DataMember]
        public Guid IdArchive { get; set; }

        [DataMember]
        public Guid IdCompany { get; set; }

        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public string FiscalCode { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string TemplateCloseFile { get; set; }
        [DataMember]
        public string TemplateIndexFile { get; set; }
        [DataMember]
        public string TemplateADEFile { get; set; }
        [DataMember]
        public string PECEmail { get; set; }
        
    }
}
