using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BiblosDS.Library.Common.Objects
{
  [DataContract(Name = "DocumentUnit", Namespace = "http://BiblosDS/2009/10/DocumentUnit")]
  public partial class DocumentUnit : BiblosDSObject
  {
    [DataMember]
    public Guid IdDocumentUnit { get; set; }

    [DataMember]
    public String Identifier { get; set; }

    [DataMember]
    public DateTime InsertDate { get; set; }

    [DataMember]
    public Nullable<DateTime> CloseDate { get; set; }

    [DataMember]
    public String Subject { get; set; }

    [DataMember]
    public String Classification { get; set; }

    [DataMember]
    public String UriFascicle { get; set; }

    [DataMember]
    public String XmlDoc { get; set; }
  }

}
