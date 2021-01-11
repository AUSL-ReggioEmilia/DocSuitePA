using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Dossiers
{
    public class DossierCommentMap : EntityTypeConfiguration<DossierComment>
    {
        public DossierCommentMap()
            : base()
        {

            ToTable("DossierComments");
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdDossierComment")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Author)
                .HasColumnName("Author")
                .IsRequired();

            Property(x => x.Comment)
                .HasColumnName("Comment")
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
                .WithMany(t => t.DossierComments)
                .Map(m => m.MapKey("IdDossier"));

            HasOptional(t => t.DossierFolder)
                .WithMany(t => t.DossierComments)
                .Map(m => m.MapKey("IdDossierFolder"));

            #endregion


        }
    }
}
