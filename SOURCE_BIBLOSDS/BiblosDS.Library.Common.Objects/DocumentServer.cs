using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "DocumentServer", Namespace = "http://BiblosDS/2009/10/DocumentServer")]
    public partial class DocumentServer : BiblosDSObject
    {
        [DataMember]
        public Server Server { get; set; }

        [DataMember]
        public DocumentStorage Storage { get; set; }

        [DataMember]
        public DocumentStorageArea StorageArea { get; set; }

        [DataMember]
        public Status Status { get; set; }

        [DataMember]
        public string DocumentHash { get; set; }

        [DataMember]
        public DateTime? DateCreated { get; set; }

        [DataMember]
        public Document Document { get; set; }

    }
}