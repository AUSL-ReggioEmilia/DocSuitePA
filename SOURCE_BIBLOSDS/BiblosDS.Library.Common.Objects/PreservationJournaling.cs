using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationJournaling", Namespace = "http://BiblosDS/2009/10/PreservationJournaling")]
    public class PreservationJournaling : BiblosDSObject
    {
        public Nullable<Guid> IdPreservation { get; set; }

        public Guid IdPreservationJournalingActivity { get; set; }

        [DataMember]
        public Guid IdPreservationJournaling { get; set; }

        [DataMember]
        public Preservation Preservation { get; set; }

        [DataMember]
        public PreservationJournalingActivity PreservationJournalingActivity { get; set; }

        [DataMember]
        public Nullable<DateTime> DateCreated { get; set; }

        [DataMember]
        public Nullable<DateTime> DateActivity { get; set; }

        [DataMember]
        public string Notes { get; set; }

        [DataMember]
        public PreservationUser User { get; set; }

        [DataMember]
        public string DomainUser { get; set; }
    }
}
