using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Resolutions
{
    public class ResolutionRoleMap : EntityTypeConfiguration<ResolutionRole>
    {
        public ResolutionRoleMap()
            : base()
        {
            ToTable("ResolutionRole");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .IsRequired();

            Property(x => x.IdResolutionRoleType)
                .HasColumnName("IdResolutionRoleType")
                .IsOptional();

            Property(t => t.RegistrationDate)
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

            Ignore(x => x.EntityId);
            Ignore(x => x.EntityShortId);

            #endregion

            #region [ Navigation Properties ]

            HasRequired(t => t.Role)
                .WithMany(t => t.ResolutionRoles)
                .Map(m => m.MapKey("idRole"));

            HasRequired(t => t.Resolution)
                .WithMany(t => t.ResolutionRoles)
                .Map(m => m.MapKey("UniqueIdResolution"));
            #endregion
        }
    }
}
