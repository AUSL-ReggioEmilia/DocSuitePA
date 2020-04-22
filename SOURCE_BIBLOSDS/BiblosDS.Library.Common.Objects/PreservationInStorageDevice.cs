using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects
{

    [DataContract(Name = "PreservationInStorageDevice", Namespace = "http://BiblosDS/2009/10/PreservationInStorageDevice")]
    public class PreservationInStorageDevice : BiblosDSObject
    {
        [DataMember]
        public Preservation Preservation { get; set; }

        [DataMember]
        public PreservationStorageDevice Device { get; set; }

        [DataMember]
        public string Path { get; set; }
    }
}
