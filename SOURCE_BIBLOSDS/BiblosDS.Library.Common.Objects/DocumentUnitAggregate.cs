using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BiblosDS.Library.Common.Objects
{
  [DataContract(Name = "DocumentUnitAggregate", Namespace = "http://BiblosDS/2009/10/DocumentUnit")]
  public partial class DocumentUnitAggregate : BiblosDSObject
  {
    [DataMember]
    public Guid IdAggregate { get; set; }

    [DataMember]
    public String XmlFascicle { get; set; }

    [DataMember]
    public Nullable<DateTime> CloseDate { get; set; }

    [DataMember]
    public short AggregationType { get; set; }

    [DataMember]
    public Nullable<DateTime> PreservationDate { get; set; }
  }
}
