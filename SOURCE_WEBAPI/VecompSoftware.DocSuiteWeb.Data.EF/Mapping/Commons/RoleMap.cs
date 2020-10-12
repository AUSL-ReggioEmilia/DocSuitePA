using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class RoleMap : EntityTypeConfiguration<Role>
    {
        public RoleMap()
            : base()
        {
            ToTable("Role");
            //Primary Key
            HasKey(t => t.EntityShortId);

            #region [ Configure Properties ]

            Property(x => x.EntityShortId)
                .HasColumnName("idRole")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsOptional();

            Property(x => x.IsActive)
                .HasColumnName("isActive")
                .IsOptional();

            Property(x => x.ActiveFrom)
                .HasColumnName("ActiveFrom")
                .IsOptional();

            Property(x => x.ActiveTo)
                .HasColumnName("ActiveTo")
                .IsOptional();

            Property(x => x.FullIncrementalPath)
                .HasColumnName("FullIncrementalPath")
                .IsOptional();

            Property(x => x.Collapsed)
                .HasColumnName("Collapsed")
                .IsOptional();

            Property(x => x.EMailAddress)
                .HasColumnName("EMailAddress")
                .IsOptional();

            Property(x => x.ServiceCode)
                .HasColumnName("ServiceCode")
                .IsOptional();

            Property(x => x.UniqueId)
              .HasColumnName("UniqueId")
              .IsRequired();

            Property(x => x.RegistrationDate)
                 .HasColumnName("RegistrationDate")
                 .IsRequired();

            Property(x => x.RegistrationUser)
                 .HasColumnName("RegistrationUser")
                 .IsOptional();

            Property(x => x.LastChangedDate)
                 .HasColumnName("LastChangedDate")
                 .IsOptional();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.ServiceCode)
                .HasColumnName("ServiceCode")
                .IsOptional();

            Property(x => x.TenantId)
                .HasColumnName("TenantId")
                .IsRequired();

            Property(x => x.IdRoleTenant)
                .HasColumnName("IdRoleTenant")
                .IsRequired();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityId);
            #endregion

            #region [ Configure Navigation Properties ]
            HasOptional(x => x.Father)
               .WithMany()
               .Map(x => x.MapKey("idRoleFather"));

            #endregion
        }
    }
}
