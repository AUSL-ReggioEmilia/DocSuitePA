using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "ArchiveServerConfig", Namespace = "http://BiblosDS/2009/10/ArchiveServerConfig")]
    public class ArchiveServerConfig : BiblosDSObject
    {
        [DataMember]
        public Guid IdArchiveServerConfig { get; set; }

        [DataMember]
        public DocumentArchive Archive { get; set; }

        [DataMember]
        public Server Server { get; set; }

        [DataMember]
        public bool TransitEnabled { get; set; }

        [DataMember]
        public string TransitPath { get; set; }
    }
}
