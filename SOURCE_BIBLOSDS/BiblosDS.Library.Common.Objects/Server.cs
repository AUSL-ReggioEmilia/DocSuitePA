using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using BiblosDS.Library.Common.Enums;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "Server", Namespace = "http://BiblosDS/2009/10/Server")]
    public class Server : BiblosDSObject
    {
        [DataMember]
        public Guid IdServer { get; set; }

        [DataMember]
        public string ServerName { get; set; }

        [DataMember]
        public ServerRole ServerRole { get; set; }

        [DataMember]
        public string DocumentServiceUrl { get; set; }

        [DataMember]
        public string DocumentServiceBinding { get; set; }

        [DataMember]
        public string DocumentServiceBindingConfiguration { get; set; }

        [DataMember]
        public string StorageServiceUrl { get; set; }

        [DataMember]
        public string StorageServiceBinding { get; set; }

        [DataMember]
        public string StorageServiceBindingConfiguration { get; set; }
    }
}
