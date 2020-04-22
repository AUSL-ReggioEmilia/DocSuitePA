using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "FileTableModel", Namespace = "http://BiblosDS/2009/10/FileTableModel")]
    public class FileTableModel : BiblosDSObject
    {
        [DataMember]
        public Guid? StreamId { get; set; }

        [DataMember]
        public byte[] FileStream { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Path { get; set; }
    }
}
