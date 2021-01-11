using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "Preservation", Namespace = "http://BiblosDS/2009/10/Preservation")]
    public class Preservation : BiblosDSObject
    {
        #region fields                                
        private PreservationUser user;
        private string path;
        private string label;
        private DateTime? preservationDate;
        private DateTime? startDate;
        private DateTime? endDate;
        private DateTime? closeDate;
        private string indexHash;
        private byte[] closeContent;
        private DateTime? lastVerifiedDate;
        private string name;
        private bool isPreservationInStorageDevice;
        private string lastSectionalValue;
        private Guid? idDocumentIndexFile;
        private Guid? idDocumentCloseFile;
        private Guid? idDocumentIndexFileXML;
        private Guid? idDocumentIndexFileXSLT;   
        private Guid? idDocumentSignedIndexFile;
        private Guid? idDocumentSignedCloseFile;
        private string pathHash;
        private long? preservationSize;
        private Guid? idArchiveBiblosStore;
        private bool? lockOnDocumentInsert;
        #endregion

        [DataMember]
        public Guid IdPreservation { get; set; }

        public Guid IdArchive { get; set; }
        [Obsolete("Usare solo il task.", false)]
        public Nullable<Guid> IdPreservationTaskGroup { get; set; }
        
        public Nullable<Guid> IdPreservationUser { get; set; }

        [DataMember]
        public DocumentArchive Archive{ get; set; }
        
        [DataMember]
        public BindingList<Document> Documents { get; set; }

        [DataMember]
        [Obsolete("Usare solo il task.", false)]
        public PreservationTaskGroup TaskGroup { get; set; }

        [DataMember]
        [Obsolete("Conservazione referenziata nei task.", false)]
        public PreservationTask Task  { get; set; }

        [DataMember]
        public PreservationUser User { get { return user; } set { user = value; ModifiedField.Add("User"); } }

        [DataMember]
        public string Path { get { return path; } set { path = value; ModifiedField.Add("Path"); } }

        [DataMember]
        public string Label { get { return label; } set { label = value; ModifiedField.Add("Label"); } }

        [DataMember]
        public Nullable<DateTime> PreservationDate { get { return preservationDate; } set { preservationDate = value; ModifiedField.Add("PreservationDate"); } }

        [DataMember]
        public Nullable<DateTime> StartDate { get { return startDate; } set { startDate = value; ModifiedField.Add("StartDate"); } }

        [DataMember]
        public Nullable<DateTime> EndDate { get { return endDate; } set { endDate = value; ModifiedField.Add("EndDate"); } }

        [DataMember]
        public Nullable<DateTime> CloseDate { get { return closeDate; } set { closeDate = value; ModifiedField.Add("CloseDate"); } }

        [DataMember]
        public string IndexHash { get { return indexHash; } set { indexHash = value; ModifiedField.Add("IndexHash"); } }

        [DataMember]
        public byte[] CloseContent { get { return closeContent; } set { closeContent = value; ModifiedField.Add("CloseContent"); } }

        [DataMember]
        public Nullable<DateTime> LastVerifiedDate { get { return lastVerifiedDate; } set { lastVerifiedDate = value; ModifiedField.Add("LastVerifiedDate"); } }

        [DataMember]
        public string Name { get { return name; } set { name = value; ModifiedField.Add("Name"); } }

        [DataMember]
        public BindingList<PreservationJournaling> PreservationJournalings { get; set; }

        /// <summary>
        /// Questo flag specifica qualora la conservazione sia gia' stata "associata" ad un supporto.
        /// </summary>
        [DataMember]
        [DefaultValue(false)]
        public bool IsPreservationInStorageDevice { get { return isPreservationInStorageDevice; } set { isPreservationInStorageDevice = value; ModifiedField.Add("IsPreservationInStorageDevice"); } }

        public string LastSectionalValue { get { return lastSectionalValue; } set { lastSectionalValue = value; ModifiedField.Add("LastSectionalValue"); } }

        public long? PreservationSize { get { return preservationSize; } set { preservationSize = value; ModifiedField.Add("PreservationSize"); } }
        [DataMember]
        public Guid? IdDocumentIndexFile { get { return idDocumentIndexFile; } set { idDocumentIndexFile = value; ModifiedField.Add("IdDocumentIndexFile"); } }
        [DataMember]
        public Guid? IdDocumentCloseFile { get { return idDocumentCloseFile; } set { idDocumentCloseFile = value; ModifiedField.Add("IdDocumentCloseFile"); } }
        [DataMember]
        public Guid? IdDocumentIndexFileXML { get { return idDocumentIndexFileXML; } set { idDocumentIndexFileXML = value; ModifiedField.Add("IdDocumentIndexFileXML"); } }
        [DataMember]
        public Guid? IdDocumentIndexFileXSLT { get { return idDocumentIndexFileXSLT; } set { idDocumentIndexFileXSLT = value; ModifiedField.Add("IdDocumentIndexFileXSLT"); } }        
        [DataMember]
        public Guid? IdDocumentSignedIndexFile { get { return idDocumentSignedIndexFile; } set { idDocumentSignedIndexFile = value; ModifiedField.Add("IdDocumentSignedIndexFile"); } }
        [DataMember]
        public Guid? IdDocumentSignedCloseFile { get { return idDocumentSignedCloseFile; } set { idDocumentSignedCloseFile = value; ModifiedField.Add("IdDocumentSignedCloseFile"); } }
        [DataMember]
        public string PathHash { get { return pathHash; } set { pathHash = value; ModifiedField.Add("PathHash"); } }
        [DataMember]
        public Guid? IdArchiveBiblosStore { get { return idArchiveBiblosStore; } set { idArchiveBiblosStore = value; ModifiedField.Add("IdArchiveBiblosStore"); } }
        [DataMember]
        public bool? LockOnDocumentInsert { get { return lockOnDocumentInsert; } set { lockOnDocumentInsert = value; ModifiedField.Add("LockOnDocumentInsert"); } }
        
    }
}
