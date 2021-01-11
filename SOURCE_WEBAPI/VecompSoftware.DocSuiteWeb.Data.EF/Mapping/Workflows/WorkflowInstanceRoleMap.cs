using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Workflows
{
    public class WorkflowInstanceRoleMap : EntityTypeConfiguration<WorkflowInstanceRole>
    {
        public WorkflowInstanceRoleMap()
            : base()
        {
            // Table
            ToTable("WorkflowInstanceRoles");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdWorkflowInstanceRole")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.AuthorizationType)
                .HasColumnName("AuthorizationType")
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
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

            HasRequired(t => t.WorkflowInstance)
                 .WithMany(t => t.WorkflowInstanceRoles)
                 .Map(m => m.MapKey("IdWorkflowInstance"));

            HasRequired(t => t.Role)
                 .WithMany(t => t.WorkflowInstanceRoles)
                 .Map(m => m.MapKey("IdRole"));
            #endregion
        }
    }
}