using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects.Response
{
    [DataContract(Name = "PreservationArchiveInfoResponse", Namespace = "http://BiblosDS/2009/10/PreservationArchiveInfoResponse")]
    public class PreservationArchiveInfoResponse : ResponseBase
    {
        [DataMember]
        public DocumentArchive Archive { get; set; }
        [DataMember]
        public List<PreservationRole> UserArchiveRole { get; set; }
    }
}
