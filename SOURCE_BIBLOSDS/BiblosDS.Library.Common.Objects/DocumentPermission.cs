using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BiblosDS.Library.Common.Enums;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "Permission", Namespace = "http://BiblosDS/2009/10/Permission")]
    public class DocumentPermission : BiblosDSObject
    {
        [DataMember(IsRequired=true)]
        public string Name { get; set; }
        [DataMember(IsRequired = true)]
        public DocumentPermissionMode Mode { get; set; }
        [DataMember]
        public bool IsGroup { get; set; }
    }
}
