using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationSchedule", Namespace = "http://BiblosDS/2009/10/PreservationSchedule")]
    public class PreservationSchedule : BiblosDSObject
    {
        [DataMember]
        public Guid IdPreservationSchedule { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Period { get; set; }

        [DataMember]
        public string ValidWeekDays { get; set; }

        [DataMember]
        public short FrequencyType { get; set; }

        [DataMember]
        public bool Active { get; set; }

        [DataMember]
        public bool Default { get; set; }

        public bool IsArchiveDefault { get; set; }

        [DataMember]
        public BindingList<PreservationScheduleTaskType> PreservationScheduleTaskTypes { get; set; }
    }
}
