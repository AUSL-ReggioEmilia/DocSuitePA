using System;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationTaskGroup", Namespace = "http://BiblosDS/2009/10/PreservationTaskGroup")]
    public class PreservationTaskGroup
    {
        [DataMember]
        public Guid IdPreservationTaskGroup { get; set; }

        public Guid IdPreservationTaskGroupType { get; set; }
        public Guid IdPreservationUser { get; set; }
        public Guid IdPreservationSchedule { get; set; }
        public Guid IdArchive { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime Expiry { get; set; }

        [DataMember]
        public Nullable<DateTime> EstimatedExpiry { get; set; }

        [DataMember]
        public Nullable<DateTime> Closed { get; set; }

        [DataMember]
        public PreservationTaskGroupType GroupType { get; set; }

        [DataMember]
        public PreservationUser User { get; set; }

        [DataMember]
        public PreservationSchedule Schedule { get; set; }

        [DataMember]
        public BindingList<PreservationTask> Tasks { get; set; }

        [DataMember]
        public DocumentArchive Archive { get; set; }
    }
}
