using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class CategoryFascicleMap : EntityTypeConfiguration<CategoryFascicle>
    {
        public CategoryFascicleMap()
            : base()
        {
            ToTable("CategoryFascicles");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdCategoryFascicle")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.FascicleType)
                .HasColumnName("FascicleType")
                .IsRequired();

            Property(x => x.DSWEnvironment)
                .HasColumnName("DSWEnvironment")
                .IsRequired();

            Property(x => x.CustomActions)
                .HasColumnName("CustomActions")
                .IsOptional();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
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

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(x => x.Category)
               .WithMany(x => x.CategoryFascicles)
               .Map(x => x.MapKey("IdCategory"));

            HasOptional(x => x.FasciclePeriod)
               .WithMany(x => x.CategoryFascicles)
               .Map(x => x.MapKey("IdFasciclePeriod"));

            HasOptional(x => x.Manager)
                .WithMany(x => x.CategoryFascicles)
                .Map(x => x.MapKey("Manager"));

            #endregion
        }
    }
}
