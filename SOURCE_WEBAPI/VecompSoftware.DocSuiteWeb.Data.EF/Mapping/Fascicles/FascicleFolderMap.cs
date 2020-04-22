using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Fascicles
{
    public class FascicleFolderMap : EntityTypeConfiguration<FascicleFolder>
    {
        public FascicleFolderMap()
            : base()
        {
            ToTable("FascicleFolders");
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
               .HasColumnName("IdFascicleFolder")
               .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            Property(x => x.Status)
                .HasColumnName("Status")
                .IsRequired();

            Property(x => x.Typology)
                .HasColumnName("Typology")
                .IsOptional();

            Property(x => x.FascicleFolderLevel)
                .HasColumnName("FascicleFolderLevel")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
                .IsOptional();

            Property(x => x.FascicleFolderPath)
                .HasColumnName("FascicleFolderPath")
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

            HasRequired(t => t.Fascicle)
                .WithMany(t => t.FascicleFolders)
                .Map(p => p.MapKey("IdFascicle"));

            HasMany(x => x.FascicleDocumentUnits)
                .WithRequired(x => x.FascicleFolder)
                .Map(x => x.MapKey("IdFascicleFolder"));

            HasOptional(t => t.Category)
                .WithMany(t => t.FascicleFolders)
                .Map(p => p.MapKey("IdCategory"));

            #endregion
        }
    }
}
