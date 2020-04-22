using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Tenants
{
    public class TenantMap : EntityTypeConfiguration<Tenant>
    {
        public TenantMap()
            : base()
        {
            // Table
            ToTable("Tenants");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdTenant")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.TenantName)
                .HasColumnName("TenantName")
                .HasMaxLength(10)
                .IsRequired();

            Property(x => x.CompanyName)
                .HasColumnName("CompanyName")
                .HasMaxLength(256)
                .IsRequired();

            Property(x => x.StartDate)
                .HasColumnName("StartDate")
                .IsRequired();

            Property(x => x.EndDate)
                .HasColumnName("EndDate")
                .IsOptional();

            Property(x => x.Note)
                .HasColumnName("Note")
                .HasMaxLength(4000)
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

            HasMany(t => t.Containers)
                .WithMany(t => t.Tenants)
                .Map(p =>
                {
                    p.ToTable("TenantContainers");
                    p.MapLeftKey("IdTenant");
                    p.MapRightKey("EntityShortId");
                });

            HasMany(t => t.Roles)
                .WithMany(t => t.Tenants)
                .Map(p =>
                {
                    p.ToTable("TenantRoles");
                    p.MapLeftKey("IdTenant");
                    p.MapRightKey("EntityShortId");
                });

            HasMany(t => t.PECMailBoxes)
                .WithMany(t => t.Tenants)
                .Map(p =>
                {
                    p.ToTable("TenantPECMailBoxes");
                    p.MapLeftKey("IdTenant");
                    p.MapRightKey("EntityShortId");
                });

            HasMany(t => t.Contacts)
                .WithMany(c => c.Tenants)
                .Map(p =>
                {
                    p.ToTable("TenantContacts");
                    p.MapLeftKey("IdTenant");
                    p.MapRightKey("EntityId");
                });
            #endregion
        }
    }
}
