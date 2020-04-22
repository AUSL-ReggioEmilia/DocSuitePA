using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "Content", Namespace = "http://BiblosDS/2009/10/Content")]
    public partial class DocumentContent : BiblosDSObject
    {
        [DataMember(IsRequired = true)]
        public byte[] Blob { get; set; }

        [DataMember]
        public string BlobString { get; set; }

        [DataMember]
        public string Description { get; set; }

        public DocumentContent()
        {
        }

        public DocumentContent(byte[] Content, string description = "")
        {
            this.Blob = Content;
            this.Description = description;
        }
    }
}
