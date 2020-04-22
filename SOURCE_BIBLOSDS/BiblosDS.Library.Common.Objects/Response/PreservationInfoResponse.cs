using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects.Response
{
    [DataContract(Name = "PreservationInfoResponse", Namespace = "http://BiblosDS/2009/10/PreservationInfoResponse")]
    public class PreservationInfoResponse : PreservationResponse
    {
        [DataMember]
        public Guid? IdPreservation { get; set; }

        [DataMember]
        public bool HasPendingDocument { get; set; }

        [DataMember]
        public DateTime? StartDocumentDate { get; set; }

        [DataMember]
        public DateTime? EndDocumentDate { get; set; }

        [DataMember]
        public DateTime? DateExpire { get; set; }

        [DataMember]
        public string DocumentType { get; set; }

        [DataMember]
        public string SharedPathName { get; set; }

        [DataMember]
        public BindingList<Document> Documents { get; set; }

        [DataMember]
        public Guid? IdPreservationTaskGroup { get; set; }

        [DataMember]
        public IDictionary<Guid, string> AwardBatchesXml { get; set; }
    }
}
