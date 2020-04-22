using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BiblosDS.Library.Common.Objects
{
  [DataContract(Name = "DocumentUnitExt", Namespace = "http://BiblosDS/2009/10/DocumentUnit")]
  public partial class DocumentUnitExt
  {
    [DataMember]
    public DocumentUnit Unit { get; set; }

    [DataMember]
    public bool IsReadOnly { get; set; }

    [DataMember]
    public bool IsPreserved { get; set; }
  }


}
