using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Workflows
{
    public class WorkflowActivityMap : EntityTypeConfiguration<WorkflowActivity>
    {
        public WorkflowActivityMap()
            : base()
        {
            // Table
            ToTable("WorkflowActivities");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdWorkflowActivity")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsOptional();

            Property(x => x.ActivityType)
                .HasColumnName("ActivityType")
                .IsRequired();

            Property(x => x.ActivityAction)
                .HasColumnName("ActivityAction")
                .IsRequired();

            Property(x => x.ActivityArea)
                .HasColumnName("ActivityArea")
                .IsRequired();

            Property(x => x.Status)
                .HasColumnName("Status")
                .IsRequired();

            Property(x => x.DueDate)
                .HasColumnName("DueDate")
                .IsOptional();

            Property(x => x.Subject)
               .HasColumnName("Subject")
               .IsOptional();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired()
                .HasMaxLength(256);

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .HasMaxLength(256)
                .IsOptional();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Property(x => x.IdArchiveChain)
                .HasColumnName("IdArchiveChain")
                .IsOptional();

            Property(x => x.Priority)
                .HasColumnName("Priority")
                .IsOptional();

            Property(x => x.Note)
                .HasColumnName("Note")
                .IsOptional();

            Property(x => x.IsVisible)
                .HasColumnName("IsVisible")
                .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);
            #endregion


            #region [ Configure Navigation Properties ]

            HasRequired(t => t.WorkflowInstance)
                .WithMany(t => t.WorkflowActivities)
                .Map(m => m.MapKey("IdWorkflowInstance"));

            HasOptional(t => t.DocumentUnitReferenced)
                .WithMany(t => t.WorkflowActivities)
                .Map(m => m.MapKey("DocumentUnitReferencedId"));

            HasOptional(t => t.Tenant)
                .WithMany(t => t.WorkflowActivities)
                .Map(m => m.MapKey("IdTenant"));

            #endregion
        }
    }
}
