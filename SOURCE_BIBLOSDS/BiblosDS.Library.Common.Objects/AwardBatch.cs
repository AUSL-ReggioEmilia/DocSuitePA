using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "AwardBatch", Namespace = "http://BiblosDS/2009/10/AwardBatch")]
    public class AwardBatch : BiblosDSObject
    {
        [DataMember]
        public Guid IdAwardBatch { get; set; }

        [DataMember]
        public Guid? IdParentBatch { get; set; }

        [DataMember]
        public Guid IdArchive { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime DateFrom { get; set; }

        [DataMember]
        public DateTime? DateTo { get; set; }

        [DataMember]
        public bool IsOpen { get; set; }

        [DataMember]
        public bool IsAuto { get; set; }

        [DataMember]
        public Guid? IdPDVDocument { get; set; }

        [DataMember]
        public Guid? IdRDVDocument { get; set; }

        [DataMember]
        public bool? IsRDVSigned { get; set; }
    }
}
