using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.DocumentArchives
{
    public class DocumentSeriesMap : EntityTypeConfiguration<DocumentSeries>
    {
        public DocumentSeriesMap()
            : base()
        {
            ToTable("DocumentSeries");
            //Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]

            Property(x => x.EntityId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsOptional();

            Property(x => x.PublicationEnabled)
                .HasColumnName("PublicationEnabled")
                .IsRequired();

            Property(x => x.AttributeSorting)
                .HasColumnName("AttributeSorting")
                .IsOptional();

            Property(x => x.AttributeCache)
                .HasColumnName("AttributeCache")
                .IsOptional();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsOptional();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.SubsectionEnabled)
                .HasColumnName("SubsectionEnabled")
                .IsOptional();

            Property(x => x.IdDocumentSeriesFamily)
                .HasColumnName("IdDocumentSeriesFamily")
                .IsOptional();

            Property(x => x.RoleEnabled)
               .HasColumnName("RoleEnabled")
               .IsOptional();

            Property(x => x.SortOrder)
               .HasColumnName("SortOrder")
               .IsOptional();

            Property(x => x.AllowAddDocument)
                .HasColumnName("AllowAddDocument")
                .IsOptional();

            Property(x => x.AllowNoDocument)
                .HasColumnName("AllowNoDocument")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.UniqueId);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Container)
                .WithMany(t => t.DocumentSeries)
                .Map(m => m.MapKey("IdContainer"));

            #endregion
        }
    }
}
