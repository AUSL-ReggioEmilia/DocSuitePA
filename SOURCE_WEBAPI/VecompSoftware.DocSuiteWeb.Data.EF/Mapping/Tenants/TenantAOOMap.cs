using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Tenants
{
    public class TenantAOOMap : EntityTypeConfiguration<TenantAOO>
    {
        public TenantAOOMap() : base()
        {
            //Table
            ToTable("TenantAOO");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdTenantAOO")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
                .HasColumnName("Name")
                .HasMaxLength(256)
                .IsRequired();

            Property(x => x.Note)
              .HasColumnName("Note")
              .HasMaxLength(4000);

            Property(x => x.CategorySuffix)
                .HasColumnName("CategorySuffix")
                .HasMaxLength(20);

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

            Property(x => x.TenantTypology)
                .HasColumnName("TenantTypology")
                .IsRequired();

            Ignore(x => x.EntityId)
            .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]
            #endregion

        }
    }
}
