using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationAlert", Namespace = "http://BiblosDS/2009/10/PreservationAlert")]
    public class PreservationAlert : BiblosDSObject
    {
        [DataMember]
        public Guid IdPreservationAlert { get; set; }

        [DataMember]
        public PreservationAlertType AlertType { get; set; }

        [DataMember]
        public PreservationTask Task { get; set; }

        [DataMember]
        public Nullable<DateTime> MadeDate { get; set; }

        [DataMember]
        public Nullable<DateTime> AlertDate { get; set; }

        [DataMember]
        public Nullable<short> ForwardFrequency { get; set; }
    }
}
