using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.FALCK.Data.Entities;

namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Data.Mappings
{
    public class WorkflowAttachmentMap : EntityTypeConfiguration<WorkflowAttachment>
    {
        public WorkflowAttachmentMap() : base()
        {
            // Table
            ToTable("DocSuite_Workflow_Attachments");
            // Primary Key
            HasKey(t => t.AttachmentId);

            #region [ Configure Properties ]
            Property(x => x.AttachmentId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.InternalFileName)
                .IsRequired();

            Property(x => x.OriginalFileName)
                .IsRequired();

            Property(x => x.IsMainDocument)
                .IsRequired();
            #endregion

            #region [ Configure Navigation Properties ]
            HasRequired(t => t.WorkflowMetadata)
                .WithMany(t => t.WorkflowAttachments)
                .Map(m => m.MapKey("MetadataId"));
            #endregion
        }
    }
}
