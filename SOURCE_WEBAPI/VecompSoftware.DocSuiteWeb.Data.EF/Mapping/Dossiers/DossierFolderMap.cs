using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Dossiers
{
    public class DossierFolderMap : EntityTypeConfiguration<DossierFolder>
    {
        public DossierFolderMap()
            : base()
        {
            ToTable("DossierFolders");
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
               .HasColumnName("IdDossierFolder")
               .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            Property(x => x.Status)
                .HasColumnName("Status")
                .IsRequired();

            Property(x => x.JsonMetadata)
                .HasColumnName("JsonMetadata")
                .IsOptional();

            Property(x => x.DossierFolderLevel)
                .HasColumnName("DossierFolderLevel")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
                .IsOptional();

            Property(x => x.DossierFolderPath)
                .HasColumnName("DossierFolderPath")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
                .IsOptional();

            Property(x => x.ParentInsertId)
                .HasColumnName("ParentInsertId")
                .IsOptional();

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

            MapToStoredProcedures();

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Dossier)
                .WithMany(t => t.DossierFolders)
                .Map(p => p.MapKey("IdDossier"));

            HasOptional(t => t.Category)
                .WithMany(t => t.DossierFolders)
                .Map(p => p.MapKey("IdCategory"));

            HasOptional(t => t.Fascicle)
                .WithMany(t => t.DossierFolders)
                .Map(p => p.MapKey("IdFascicle"));
            #endregion
        }
    }
}
