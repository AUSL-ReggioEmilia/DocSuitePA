using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Dossiers
{
    public class DossierRoleMap : EntityTypeConfiguration<DossierRole>
    {
        public DossierRoleMap()
            : base()
        {
            ToTable("DossierRoles");
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdDossierRole")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Status)
                .HasColumnName("Status")
                .IsRequired();

            Property(x => x.AuthorizationRoleType)
                .HasColumnName("RoleAuthorizationType")
                .IsRequired();

            Property(x => x.IsMaster)
                .HasColumnName("IsMaster")
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);

            #endregion

            #region [ Navigation Properties ]

            HasRequired(t => t.Dossier)
                .WithMany(t => t.DossierRoles)
                .Map(m => m.MapKey("IdDossier"));

            HasRequired(t => t.Role)
                .WithMany(t => t.DossierRoles)
                .Map(m => m.MapKey("IdRole"));
            #endregion
        }
    }
}
