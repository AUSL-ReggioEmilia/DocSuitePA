using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Enums
{
    [DataContract(Name = "FilterOperator", Namespace = "http://BiblosDS/2009/10/FilterOperator")]
    public enum DocumentConditionFilterOperator
    {
        [EnumMember]
        IsEqualTo = 0,
        [EnumMember]
        Contains = 1,
        [EnumMember]
        IsNullOrEmpty = 2,
    }
}
