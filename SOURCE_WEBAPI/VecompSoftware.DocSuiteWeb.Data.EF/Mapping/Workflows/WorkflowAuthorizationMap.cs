using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Workflows
{
    public class WorkflowAuthorizationMap : EntityTypeConfiguration<WorkflowAuthorization>
    {
        public WorkflowAuthorizationMap()
            : base()
        {
            // Table
            ToTable("WorkflowAuthorizations");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdWorkflowAuthorization")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Account)
                .HasColumnName("Account")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired()
                .HasMaxLength(256);

            Property(x => x.IsHandler)
                .HasColumnName("IsHandler")
                .IsRequired();

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

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);
            #endregion


            #region [ Configure Navigation Properties ]

            HasRequired(t => t.WorkflowActivity)
                .WithMany(t => t.WorkflowAuthorizations)
                .Map(m => m.MapKey("IdWorkflowActivity"));

            #endregion
        }
    }
}
