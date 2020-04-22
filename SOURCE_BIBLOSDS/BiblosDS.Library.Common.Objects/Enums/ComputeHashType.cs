using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects.Enums
{
    [DataContract(Name = "ComputeHashType", Namespace = "http://BiblosDS/2009/10/ComputeHashType")]
    public enum ComputeHashType
    {
        [EnumMember]
        Default = 0,
        [EnumMember]
        SHA1 = 1,
        [EnumMember]
        SHA256 = 256
    }
}
