using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationException", Namespace = "http://BiblosDS/2009/10/PreservationException")]
    public class PreservationException : BiblosDSObject
    {
        [DataMember]
        public Guid IdPreservationException { get; set; }

        [DataMember]
        public Nullable<Guid> IdPreservationExceptionType { get; set; }

        [DataMember]
        public Nullable<Guid> IdPreservationExceptionCorrelated { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool IsBlocked { get; set; }

        [DataMember]
        public PreservationExceptionType ExceptionType { get; set; }
    }
}
