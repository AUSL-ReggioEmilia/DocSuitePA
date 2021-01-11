using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Tenants
{
    public class TenantWorkflowRepositoryMap : EntityTypeConfiguration<TenantWorkflowRepository>
    {
        public TenantWorkflowRepositoryMap()
            : base()
        {
            ToTable("TenantWorkflowRepositories");

            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(t => t.UniqueId)
                .HasColumnName("IdTenantWorkflowRepository")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.ConfigurationType)
                .HasColumnName("ConfigurationType")
                .IsRequired();

            Property(x => x.JsonValue)
                .HasColumnName("JsonValue")
                .IsMaxLength()
                .IsRequired();

            Property(x => x.IntegrationModuleName)
                .HasColumnName("IntegrationModuleName")
                .IsOptional();

            Property(x => x.Conditions)
                .HasColumnName("Conditions")
                .IsOptional();

            Property(x => x.StartDate)
                .HasColumnName("StartDate")
                .IsRequired();

            Property(x => x.EndDate)
                .HasColumnName("EndDate")
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
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.Note);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Tenant)
                .WithMany(t => t.TenantWorkflowRepositories)
                .Map(m => m.MapKey("IdTenant"));

            HasRequired(t => t.WorkflowRepository)
                .WithMany(t => t.TenantWorkflowRepositories)
                .Map(m => m.MapKey("IdWorkflowRepository"));

            #endregion
        }
    }
}
