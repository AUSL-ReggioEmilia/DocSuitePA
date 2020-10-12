using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class CategoryMap : EntityTypeConfiguration<Category>
    {
        public CategoryMap()
            : base()
        {
            ToTable("Category");
            //Primary Key
            HasKey(t => t.EntityShortId);

            #region [ Configure Properties ]

            Property(x => x.EntityShortId)
                .HasColumnName("idCategory")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            Property(x => x.IsActive)
                .HasColumnName("isActive")
                .IsOptional();

            Property(x => x.Code)
                .HasColumnName("Code")
                .IsOptional();

            Property(x => x.FullIncrementalPath)
                .HasColumnName("FullIncrementalPath")
                .IsOptional();

            Property(x => x.FullCode)
                .HasColumnName("FullCode")
                .IsOptional();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsOptional();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.FullSearchComputed)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
                .HasColumnName("FullSearchComputed")
                .IsOptional();

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .IsRequired();

            Property(x => x.StartDate)
                .HasColumnName("StartDate")
                .IsRequired();


            Property(x => x.EndDate)
                .HasColumnName("EndDate")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityId);

            MapToStoredProcedures();
            #endregion

            #region [ Configure Navigation Properties ]

            HasOptional(x => x.Parent)
                .WithMany(x => x.Categories)
                .Map(x => x.MapKey("idParent"));

            HasOptional(x => x.MassimarioScarto)
                .WithMany(x => x.Categories)
                .Map(x => x.MapKey("IdMassimarioScarto"));

            HasRequired(x => x.CategorySchema)
                .WithMany(x => x.Categories)
                .Map(x => x.MapKey("IdCategorySchema"));

            HasOptional(x => x.MetadataRepository)
                .WithMany(x => x.Categories)
                .Map(x => x.MapKey("IdMetadataRepository"));

            HasRequired(t => t.TenantAOO)
                .WithMany(t => t.Categories)
                .Map(m => m.MapKey("IdTenantAOO"));

            #endregion
        }
    }
}
