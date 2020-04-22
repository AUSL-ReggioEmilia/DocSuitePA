using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationScheduleTaskType", Namespace = "http://BiblosDS/2009/10/PreservationScheduleTaskType")]
    public class PreservationScheduleTaskType : BiblosDSObject
    {
        public Guid IdPreservationSchedule { get; set; }

        public Guid IdPreservationTaskType { get; set; }

        [DataMember]
        public PreservationSchedule Schedule { get; set; }

        [DataMember]
        public PreservationTaskType TaskType { get; set; }

        [DataMember]
        public short Offset { get; set; }
    }
}
