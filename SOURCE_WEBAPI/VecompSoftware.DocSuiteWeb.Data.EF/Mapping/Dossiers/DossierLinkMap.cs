using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Dossiers
{
    public class DossierLinkMap : EntityTypeConfiguration<DossierLink>
    {
        public DossierLinkMap()
            : base()
        {
            ToTable("DossierLinks");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties]

            Property(x => x.UniqueId)
                .HasColumnName("IdDossierLink")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.DossierLinkType)
                .HasColumnName("DossierLinkType")
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

            #region [ Configure Navigation Properties]

            HasRequired(t => t.Dossier)
                .WithMany(t => t.DossierLinks)
                .Map(p => p.MapKey("IdDossierParent"));

            HasRequired(t => t.DossierLinked)
                .WithMany(t => t.LinkedDossiers)
                .Map(p => p.MapKey("IdDossierSon"));

            #endregion
        }
    }
}
