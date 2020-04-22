using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationExceptionType", Namespace = "http://BiblosDS/2009/10/PreservationExceptionType")]
    public class PreservationExceptionType : BiblosDSObject
    {
        [DataMember]
        public Guid IdPreservationExceptionType { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool IsFail { get; set; }
    }
}
