using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects.Response
{
    [DataContract(Name = "PreservationExpireResponse", Namespace = "http://BiblosDS/2009/10/PreservationExpireResponse")]
    public class PreservationExpireResponse
    {
        [DataMember]
        public DateTime? Expiry { get; set; }

        [DataMember]
        public DateTime? EstimatedExpiry { get; set; }

    }
}
