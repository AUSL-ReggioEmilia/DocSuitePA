using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BiblosDS.Library.Common.Objects
{
  [DataContract(Name = "DocumentUnitAggregateExt", Namespace = "http://BiblosDS/2009/10/DocumentUnit")]
  public partial class DocumentUnitAggregateExt
  {
    [DataMember]
    public DocumentUnitAggregate Aggregate { get; set; }

    [DataMember]
    public bool IsReadOnly { get; set; }

    [DataMember]
    public bool IsPreserved { get; set; }
  }
}
