using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.DocumentUnits
{
    public class DocumentUnitFascicleCategoryMap : EntityTypeConfiguration<DocumentUnitFascicleCategory>
    {
        public DocumentUnitFascicleCategoryMap() : base()
        {
            ToTable("DocumentUnitFascicleCategories", "cqrs");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdDocumentUnitFascicleCategory")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

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


            Ignore(x => x.EntityShortId);

            Ignore(x => x.EntityId);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.DocumentUnit)
                .WithMany(t => t.DocumentUnitFascicleCategories)
                .Map(m => m.MapKey("IdDocumentUnit"));

            HasRequired(t => t.Category)
                .WithMany(t => t.DocumentUnitFascicleCategories)
                .Map(m => m.MapKey("IdCategory"));

            HasRequired(t => t.Fascicle)
                .WithMany(t => t.DocumentUnitFascicleCategories)
                .Map(m => m.MapKey("IdFascicle"));

            #endregion

        }
    }
}
