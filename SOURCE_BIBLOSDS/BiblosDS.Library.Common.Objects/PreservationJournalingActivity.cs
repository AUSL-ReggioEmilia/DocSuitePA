using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationJournalingActivity", Namespace = "http://BiblosDS/2009/10/PreservationJournalingActivity")]
    public class PreservationJournalingActivity
    {
        [DataMember]
        public Guid IdPreservationJournalingActivity { get; set; }

        [DataMember]
        public string KeyCode { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool IsUserActivity { get; set; }
    }
}
