using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Resolutions
{
    public class ResolutionDocumentSeriesItemMap : EntityTypeConfiguration<ResolutionDocumentSeriesItem>
    {
        public ResolutionDocumentSeriesItemMap() : base()
        {
            ToTable("ResolutionDocumentSeriesItem");

            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.EntityId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

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

            Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]
            HasRequired(t => t.DocumentSeriesItem)
                .WithMany(t => t.ResolutionDocumentSeriesItems)
                .Map(m => m.MapKey("UniqueIdDocumentSeriesItem"));

            HasRequired(t => t.Resolution)
                .WithMany(t => t.ResolutionDocumentSeriesItems)
                .Map(m => m.MapKey("UniqueIdResolution"));
            #endregion

        }
    }
}
