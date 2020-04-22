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

            Property(x => x.EntityId)
                .HasColumnName("idResolution")
                .IsRequired();

            Property(x => x.IdResolutionRoleType)
                .HasColumnName("IdResolutionRoleType")
                .IsOptional();

            Property(t => t.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(t => t.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(t => t.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(t => t.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId);

            #endregion

            #region [ Navigation Properties ]

            HasRequired(t => t.Role)
                .WithMany(t => t.ResolutionRoles)
                .Map(m => m.MapKey("idRole"));

            #endregion
        }
    }
}
