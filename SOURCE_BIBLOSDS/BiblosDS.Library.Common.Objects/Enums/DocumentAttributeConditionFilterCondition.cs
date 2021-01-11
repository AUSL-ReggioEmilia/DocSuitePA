using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Enums
{
    [DataContract(Name = "FilterCondition", Namespace = "http://BiblosDS/2009/10/FilterCondition")]
    public enum DocumentConditionFilterCondition
    {
        [EnumMember]
        And = 0,
        [EnumMember]
        Or = 1,
    }
}
