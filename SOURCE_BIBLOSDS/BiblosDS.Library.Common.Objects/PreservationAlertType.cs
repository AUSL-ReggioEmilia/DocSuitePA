using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationAlertType", Namespace = "http://BiblosDS/2009/10/PreservationAlertType")]
    public class PreservationAlertType : BiblosDSObject
    {
        [DataMember]
        public Guid IdPreservationAlertType { get; set; }

        [DataMember]
        public BindingList<PreservationAlert> Alerts { get; set; }

        [DataMember]
        public string AlertText { get; set; }

        [DataMember]
        public short Offset { get; set; }

        [DataMember]
        public BindingList<PreservationTaskType> TaskTypes { get; set; }

        [DataMember]
        public PreservationRole Role { get; set; }
    }
}
