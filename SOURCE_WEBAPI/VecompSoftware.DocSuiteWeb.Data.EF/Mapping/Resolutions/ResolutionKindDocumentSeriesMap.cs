using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Resolutions
{
    public class ResolutionKindDocumentSeriesMap : EntityTypeConfiguration<ResolutionKindDocumentSeries>
    {
        public ResolutionKindDocumentSeriesMap()
            : base()
        {
            ToTable("ResolutionKindDocumentSeries");
            HasKey(t => t.UniqueId);

            #region [ Confifure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdResolutionKindDocumentSeries")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.DocumentRequired)
                .HasColumnName("DocumentRequired")
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

            #region [ Configure Navigation Properties ]
            HasRequired(x => x.ResolutionKind)
                .WithMany(r => r.ResolutionKindDocumentSeries)
                .Map(m => m.MapKey("IdResolutionKind"));

            HasRequired(x => x.DocumentSeries)
                .WithMany(d => d.ResolutionKindDocumentSeries)
                .Map(m => m.MapKey("IdDocumentSeries"));

            HasOptional(x => x.DocumentSeriesConstraint)
                .WithMany(d => d.ResolutionKindDocumentSeries)
                .Map(m => m.MapKey("IdDocumentSeriesConstraint"));
            #endregion
        }
    }
}
