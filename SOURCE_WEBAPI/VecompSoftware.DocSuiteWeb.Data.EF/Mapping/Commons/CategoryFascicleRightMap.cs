using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class CategoryFascicleRightMap : EntityTypeConfiguration<CategoryFascicleRight>
    {
        public CategoryFascicleRightMap()
            : base()
        {
            ToTable("CategoryFascicleRights");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdCategoryFascicleRight")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(x => x.CategoryFascicle)
               .WithMany(x => x.CategoryFascicleRights)
               .Map(x => x.MapKey("IdCategoryFascicle"));

            HasOptional(x => x.Role)
               .WithMany(x => x.CategoryFascicleRights)
               .Map(x => x.MapKey("IdRole"));

            HasOptional(x => x.Container)
                .WithMany(x => x.CategoryFascicleRights)
                .Map(x => x.MapKey("IdContainer"));

            #endregion
        }
    }
}