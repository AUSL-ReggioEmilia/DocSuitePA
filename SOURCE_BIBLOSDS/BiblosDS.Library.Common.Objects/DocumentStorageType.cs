using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "StorageType", Namespace = "http://BiblosDS/2009/10/StorageType")]
    public partial class DocumentStorageType : BiblosDSObject
    {
        [DataMember]
        public Guid IdStorageType { get; set; }

        /// <summary>
        /// Storage Type Description
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Assembly Name to load
        /// </summary>
        [DataMember]
        public string StorageAssembly { get; set; }

        /// <summary>
        /// Class Name to load
        /// </summary>
        [DataMember]
        public string StorageClassName { get; set; }

        public DocumentStorageType()
        {
        }

        public DocumentStorageType(Guid IdStorageType, string StorageAssembly, string StorageClassName, string StorageType)
        {
            this.IdStorageType = IdStorageType;
            this.StorageAssembly = StorageAssembly;
            this.StorageClassName = StorageClassName;
            this.Type = StorageType;
        }

    }
}
