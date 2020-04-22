using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationTask", Namespace = "http://BiblosDS/2009/10/PreservationTask")]
    public class PreservationTask : BiblosDSObject
    {
        [DataMember]
        public Guid IdPreservationTask { get; set; }

        [DataMember]
        public DateTime EstimatedDate { get; set; }

        [DataMember]
        public Nullable<DateTime> ExecutedDate { get; set; }

        [DataMember]
        public Nullable<DateTime> StartDocumentDate { get; set; }

        [DataMember]
        public Nullable<DateTime> EndDocumentDate { get; set; }

        [DataMember]
        public PreservationTaskType TaskType { get; set; }

        [DataMember]
        public DocumentArchive Archive { get; set; }

        [DataMember]
        public PreservationUser User { get; set; }

        [DataMember]
        public PreservationTaskGroup TaskGroup { get; set; }

        [DataMember]
        public BindingList<PreservationAlert> Alerts { get; set; }

        [DataMember]
        public BindingList<PreservationTask> CorrelatedTasks { get; set; }

        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public bool Executed { get; set; }

        [DataMember]
        public bool HasError { get; set; }

        [DataMember]
        public string ErrorMessages { get; set; }

        [DataMember]
        public Guid? ActivationPin { get; set; }

        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public DateTime? StartDate { get; set; }

        [DataMember]
        public Guid? IdPreservation { get; set; }

        public string PreservationCloseFile { get; set; }

        public Guid? IdCorrelatedPreservationTask { get; set; }

        public bool CanExecute { get; set; }

        public string VerifyPath { get; set; }

        public DateTime? LockDate { get; set; }
    }
}
