using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Workflows
{
    public class WorkflowRepositoryMap : EntityTypeConfiguration<WorkflowRepository>
    {
        public WorkflowRepositoryMap()
            : base()
        {
            // Table
            ToTable("WorkflowRepositories");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdWorkflowRepository")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            Property(x => x.Version)
                .HasColumnName("Version")
                .IsRequired();

            Property(x => x.ActiveFrom)
                .HasColumnName("ActiveFrom")
                .IsRequired();

            Property(x => x.ActiveTo)
                .HasColumnName("ActiveTo")
                .IsOptional();

            Property(x => x.Xaml)
                .HasColumnName("Xaml");

            Property(x => x.Json)
                .HasColumnName("Json");

            Property(x => x.Status)
               .HasColumnName("Status")
               .IsRequired();

            Property(x => x.CustomActivities)
               .HasColumnName("CustomActivities");

            Property(x => x.DSWEnvironment)
                .HasColumnName("DSWEnvironment")
                .IsRequired();

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

            HasMany(t => t.Roles)
                .WithMany(t => t.WorkflowRepositories)
                .Map(m =>
                {
                    m.ToTable("WorkflowRoles");
                    m.MapLeftKey("IdWorkflowRepository");
                    m.MapRightKey("IdRole");
                });

            #endregion
        }
    }
}
