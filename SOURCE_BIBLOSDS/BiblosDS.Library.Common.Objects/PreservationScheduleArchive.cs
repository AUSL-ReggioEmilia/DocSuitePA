using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationScheduleArchive", Namespace = "http://BiblosDS/2009/10/PreservationScheduleArchive")]
    public class PreservationScheduleArchive : BiblosDSObject
    {
        public Guid IdArchive { get; set; }
        public Guid IdSchedule { get; set; }

        [DataMember]
        public DocumentArchive Archive { get; set; }

        [DataMember]
        public PreservationSchedule Schedule { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public DateTime? ModifiedDate { get; set; }
    }
}
