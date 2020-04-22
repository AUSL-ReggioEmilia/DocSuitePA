using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Dossiers
{
    public class DossierFolderRoleMap : EntityTypeConfiguration<DossierFolderRole>
    {
        public DossierFolderRoleMap()
            : base()
        {
            ToTable("DossierFolderRoles");
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
               .HasColumnName("IdDossierFolderRole")
               .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.AuthorizationRoleType)
               .HasColumnName("RoleAuthorizationType")
               .IsRequired();

            Property(x => x.IsMaster)
              .HasColumnName("IsMaster")
              .IsRequired();

            Property(x => x.Status)
              .HasColumnName("Status")
              .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
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

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.DossierFolder)
                .WithMany(t => t.DossierFolderRoles)
                .Map(p => p.MapKey("IdDossierFolder"));

            HasRequired(t => t.Role)
               .WithMany(t => t.DossierFolderRoles)
               .Map(p => p.MapKey("IdRole"));
            #endregion
        }
    }
}
