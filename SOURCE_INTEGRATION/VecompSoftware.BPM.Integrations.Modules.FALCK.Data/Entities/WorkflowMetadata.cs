using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Data.Entities
{
    public class WorkflowMetadata
    {
        #region [ Constructor ]
        public WorkflowMetadata()
        {
            WorkflowAttachments = new HashSet<WorkflowAttachment>();
        }
        #endregion

        #region [ Properties ]
        public int MetadataId { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string DocumentDescription { get; set; }
        public DocumentType ReferenceDocType { get; set; }
        public string ReferenceDocNumber { get; set; }
        public SourceType SourceType { get; set; }
        public string SourceNumber { get; set; }
        public string SourceName { get; set; }
        public string SourceVatRegNumber { get; set; }
        public string SourceFiscalCode { get; set; }
        public string SourceEmail { get; set; }
        public string SourceCertifiedEmail { get; set; }
        public string SourceLegalForm { get; set; }
        public string CreationUser { get; set; }
        public DateTime CreationDateTime { get; set; }
        public byte IsExternalPartner { get; set; }
        public string JobCode { get; set; }
        public string JobDescription { get; set; }
        public string JobType { get; set; }
        public decimal JobDelegateAmountLCY { get; set; }
        public byte IsItemSpecialCategory { get; set; }
        public string SpecialCategoryCode { get; set; }
        public string SpecialCategoryDescription { get; set; }
        public byte IsItemEmergency { get; set; }
        public byte IsItemStandardWorkflow { get; set; }
        public decimal AmountLCY { get; set; }
        public string CurrencyCode { get; set; }
        public byte IsInternalPO { get; set; }
        public string POResponsibleLevel1 { get; set; }
        public string POResponsibleLevel2 { get; set; }
        public string POResponsibleLevel3 { get; set; }
        public string POResponsibleSpecialCategory { get; set; }
        public byte IsLegalChecked { get; set; }
        public string Note1 { get; set; }
        public string Note2 { get; set; }
        public string WorkflowInstanceID { get; set; }
        public string WorkflowCompletedID { get; set; }
        public WorkflowStatusType WorkflowStatus { get; set; }
        public byte IsWorkflowApproved { get; set; }
        public byte IsWorkflowAutoapproval { get; set; }
        public string DocSuiteURL { get; set; }
        public DateTime WorkflowUpdateDate { get; set; }
        [Timestamp]
        [ConcurrencyCheck]
        public byte[] Timestamp { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual ICollection<WorkflowAttachment> WorkflowAttachments { get; set; }
        #endregion

        #region [ Constructor ]
        public string GetMappingTag()
        {
            return string.Concat(JobCode, "_", CompanyCode);
        }
        #endregion

    }
}
