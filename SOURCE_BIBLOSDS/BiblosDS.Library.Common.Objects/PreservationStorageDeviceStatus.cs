using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationStorageDeviceStatus", Namespace = "http://BiblosDS/2009/10/PreservationStorageDeviceStatus")]
    public class PreservationStorageDeviceStatus
    {
        [DataMember]
        public Guid IdPreservationStorageDeviceStatus { get; set; }

        [DataMember]
        public string KeyCode { get; set; }

        [DataMember]
        public string Value { get; set; }
    }
}
