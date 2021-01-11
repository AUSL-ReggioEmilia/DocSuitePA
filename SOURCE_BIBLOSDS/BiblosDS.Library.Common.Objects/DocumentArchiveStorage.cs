using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "ArchiveStorage", Namespace = "http://BiblosDS/2009/10/ArchiveStorage")]
    public partial class DocumentArchiveStorage : BiblosDSObject
    {
        /// <summary>
        /// Archivio in cui salvare i documenti
        /// </summary>      
        [DataMember]
        public DocumentArchive Archive { get; set; }

                /// <summary>
        /// Storage in cui salvare i documenti
        /// </summary>
        [DataMember]
        public DocumentStorage Storage { get; set; }
        [DataMember]
        public bool Active { get; set; }


        #region Constructor

        public DocumentArchiveStorage()
        {
            this.Archive = new DocumentArchive();
            this.Storage = new DocumentStorage();
        }

        public DocumentArchiveStorage(DocumentArchive archive, DocumentStorage storage)
        {
            this.Archive = archive;
            this.Storage = storage;
        }

        #endregion
    }
}
