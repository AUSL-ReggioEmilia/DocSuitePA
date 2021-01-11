using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BiblosDS.Library.Common.Objects
{
  [DataContract(Name = "DocumentUnitChain", Namespace = "http://BiblosDS/2009/10/DocumentUnitChain")]
  public partial class DocumentUnitChain : BiblosDSObject
  {
    [DataMember]
    public Guid IdDocumentUnit { get; set; }

    [DataMember]
    public Guid IdParentBiblos { get; set; }

    [DataMember]
    public String Name { get; set; }

    [DataMember]
    public Document Document { get; set; }

    [DataMember]
    public DocumentUnit DocumentUnit { get; set; }
  }
}
