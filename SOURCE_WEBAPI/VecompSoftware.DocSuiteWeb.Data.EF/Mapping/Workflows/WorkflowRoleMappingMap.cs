using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Workflows
{
    public class WorkflowRoleMappingMap : EntityTypeConfiguration<WorkflowRoleMapping>
    {
        public WorkflowRoleMappingMap()
            : base()
        {
            // Table
            ToTable("WorkflowRoleMappings");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdWorkflowRoleMapping")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.MappingTag)
                .HasColumnName("MappingTag")
                .IsOptional();

            Property(x => x.IdInternalActivity)
                .HasColumnName("IdInternalActivity")
                .IsOptional();

            Property(x => x.AccountName)
                .HasColumnName("AccountName")
                .IsOptional();

            Property(x => x.AuthorizationType)
                .HasColumnName("AuthorizationType")
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

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.WorkflowRepository)
                .WithMany(t => t.WorkflowRoleMappings)
                .Map(m => m.MapKey("IdWorkflowRepository"));

            HasOptional(t => t.Role)
                .WithMany(t => t.WorkflowRoleMappings)
                .Map(m => m.MapKey("IdRole"));

            #endregion
        }
    }
}
