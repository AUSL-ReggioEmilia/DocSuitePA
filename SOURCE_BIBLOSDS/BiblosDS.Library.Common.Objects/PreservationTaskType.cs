using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects.Enums;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationTaskType", Namespace = "http://BiblosDS/2009/10/PreservationTaskType")]
    public class PreservationTaskType : BiblosDSObject
    {
        [DataMember]
        public Guid IdPreservationTaskType { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public short Period { get; set; }

        [DataMember]
        public PreservationTaskTypes Type { get; set; }

        [DataMember]
        public BindingList<PreservationRole> Roles { get; set; }

        [DataMember]
        public BindingList<PreservationScheduleTaskType> ScheduleTaskTypes { get; set; }
    }
}
