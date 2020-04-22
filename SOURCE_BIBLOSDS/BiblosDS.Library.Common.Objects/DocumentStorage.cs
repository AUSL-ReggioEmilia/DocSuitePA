using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    /// <summary>
    /// Store configuration of an <see cref="Archive">Archive</see> 
    /// </summary>
    [DataContract(Name = "Storage", Namespace = "http://BiblosDS/2009/10/Storage")]
    public partial class DocumentStorage : BiblosDSObject
    {
        [DataMember]
        public Guid IdStorage { get; set; }

        [DataMember]
        public DocumentStorageType StorageType { get; set; }

        [DataMember]
        public string MainPath { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string StorageRuleAssembly { get; set; }

        [DataMember]
        public string StorageRuleClassName { get; set; }

        [DataMember]
        public Nullable<int> Priority { get; set; }

        [DataMember]
        public bool EnableFulText { get; set; }

        [DataMember]
        public string AuthenticationKey { get; set; }

        [DataMember]
        public string AuthenticationPassword { get; set; }

        [DataMember]
        public bool? IsVisible { get; set; }

        [DataMember]
        public Server Server { get; set; }

        private BindingList<DocumentStorageRule> StorageRules { get; set; }
     

        #region Constructor
        
        public DocumentStorage()
        {
        }

        public DocumentStorage(Guid IdStorage)
        {
            this.IdStorage = IdStorage;
        }

        public DocumentStorage(Guid IdStorage, string MainPath, string Name, string StorageAssembly, DocumentStorageType StorageType)
        {
            this.IdStorage = IdStorage;
            this.MainPath = MainPath;
            this.Name = Name;
            this.StorageRuleAssembly = StorageAssembly;
            this.StorageType = StorageType;
        }

        #endregion

    }
}
