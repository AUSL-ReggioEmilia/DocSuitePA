using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationUserRole", Namespace = "http://BiblosDS/2009/10/PreservationUserRole")]
    public class PreservationUserRole : BiblosDSObject
    {
        public Guid IdPreservationUserRole { get; set; }

        [DataMember]
        public Guid IdPreservationRole { get; set; }

        public Guid IdPreservationUser { get; set; }

        public Nullable<Guid> IdArchive { get; set; }

        [DataMember]
        public PreservationRole PreservationRole { get; set; }

        [DataMember]
        public PreservationUser PreservationUser { get; set; }

        [DataMember]
        public DocumentArchive Archive { get; set; }
    }
}
