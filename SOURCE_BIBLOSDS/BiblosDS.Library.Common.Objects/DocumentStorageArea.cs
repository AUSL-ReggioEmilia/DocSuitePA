using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "StorageArea", Namespace = "http://BiblosDS/2009/10/StorageArea")]
    public partial class DocumentStorageArea : BiblosDSObject
    {
        [DataMember]
        public Guid IdStorageArea { get; set; }
        [DataMember]
        public Status Status { get; set; }
        [DataMember]
        public DocumentStorage Storage { get; set; }
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public long MaxSize { get; set; }
        [DataMember]
        public long CurrentSize { get; set; }
        [DataMember]
        public long MaxFileNumber { get; set; }
        [DataMember]
        public long? CurrentFileNumber { get; set; }
        [DataMember]
        public bool Enable { get; set; }
        [DataMember]
        public DocumentArchive Archive { get; set; }

        public DocumentStorageArea()
        {
        }

        public DocumentStorageArea(Guid IdStorageArea)
        {
            this.IdStorageArea = IdStorageArea;
        }

        public DocumentStorageArea(Guid IdStorageArea, Status Status, DocumentStorage Storage, String Path, string Name)
        {
            this.IdStorageArea = IdStorageArea;
            this.Status = Status;
            this.Storage = Storage;
            this.Path = Path;
            this.Name = Name;
        }
    }
}
