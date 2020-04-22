using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.FALCK.Data.Entities;

namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Data.Mappings
{
    public class WorkflowMetadataMap : EntityTypeConfiguration<WorkflowMetadata>
    {
        public WorkflowMetadataMap() : base()
        {
            // Table
            ToTable("DocSuite_Workflow_Metadatas");
            // Primary Key
            HasKey(t => t.MetadataId);

            #region [ Configure Properties ]
            Property(x => x.MetadataId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.CompanyCode)
                .IsRequired();

            Property(x => x.CompanyName)
                .IsRequired();

            Property(x => x.DocumentType)
                .IsRequired();

            Property(x => x.DocumentNumber)
                .IsRequired();

            Property(x => x.DocumentDescription)
                .IsRequired();

            Property(x => x.ReferenceDocType)
                .IsRequired();

            Property(x => x.ReferenceDocNumber)
                .IsRequired();

            Property(x => x.SourceType)
                .IsRequired();

            Property(x => x.SourceNumber)
                .IsRequired();

            Property(x => x.SourceName)
                .IsRequired();

            Property(x => x.SourceVatRegNumber)
                .IsRequired();

            Property(x => x.SourceFiscalCode)
                .IsRequired();

            Property(x => x.SourceEmail)
                .IsRequired();

            Property(x => x.SourceCertifiedEmail)
                .IsRequired();

            Property(x => x.SourceLegalForm)
                .IsRequired();

            Property(x => x.CreationUser)
                .IsRequired();

            Property(x => x.CreationDateTime)
                .IsRequired();

            Property(x => x.IsExternalPartner)
                .IsRequired();

            Property(x => x.JobCode)
                .IsRequired();

            Property(x => x.JobDescription)
                .IsRequired();

            Property(x => x.JobType)
                .IsRequired();

            Property(x => x.JobDelegateAmountLCY)
                .IsRequired();

            Property(x => x.IsItemSpecialCategory)
                .IsRequired();

            Property(x => x.SpecialCategoryCode)
                .IsRequired();

            Property(x => x.SpecialCategoryDescription)
                .IsRequired();

            Property(x => x.IsItemEmergency)
                .IsRequired();

            Property(x => x.IsItemStandardWorkflow)
                .IsRequired();

            Property(x => x.AmountLCY)
                .IsRequired();

            Property(x => x.CurrencyCode)
                .IsRequired();

            Property(x => x.IsInternalPO)
                .IsRequired();

            Property(x => x.POResponsibleLevel1)
                .IsRequired();

            Property(x => x.POResponsibleLevel2)
                .IsRequired();

            Property(x => x.POResponsibleLevel3)
                .IsRequired();

            Property(x => x.POResponsibleSpecialCategory)
                .IsRequired();

            Property(x => x.IsLegalChecked)
                .IsRequired();

            Property(x => x.Note1)
                .IsRequired();

            Property(x => x.Note2)
                .IsRequired();

            Property(x => x.WorkflowInstanceID)
                .IsRequired();

            Property(x => x.WorkflowCompletedID)
                .IsRequired();

            Property(x => x.WorkflowStatus)
                .IsRequired();

            Property(x => x.IsWorkflowApproved)
                .IsRequired();

            Property(x => x.IsWorkflowAutoapproval)
                .IsRequired();

            Property(x => x.DocSuiteURL)
                .IsRequired();

            Property(x => x.WorkflowUpdateDate)
                .IsRequired();

            Property(x => x.Timestamp)
                .HasColumnName("timestamp")
                .IsRequired();
            #endregion

            #region [ Configure Navigation Properties ]
            #endregion
        }
    }
}
