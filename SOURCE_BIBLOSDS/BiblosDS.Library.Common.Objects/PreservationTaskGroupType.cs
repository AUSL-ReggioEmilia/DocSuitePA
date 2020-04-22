using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationTaskGroupType", Namespace = "http://BiblosDS/2009/10/PreservationTaskGroupType")]
    public class PreservationTaskGroupType
    {
        [DataMember]
        public Guid IdPreservationTaskGroupType { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}
