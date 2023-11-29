using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Tenders;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Tenders
{
    public class TenderHeaderMap : EntityTypeConfiguration<TenderHeader>
    {
        public TenderHeaderMap()
            : base()
        {
            // Table
            ToTable("TenderHeader");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Title)
                .HasColumnName("Title")
                .IsOptional();

            Property(x => x.Abstract)
                .HasColumnName("Abstract")
                .IsOptional();

            Property(x => x.Year)
                .HasColumnName("Year")
                .IsOptional();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired()
                .HasMaxLength(256);

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .HasMaxLength(256)
                .IsOptional();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.Timestamp);
            #endregion

            #region [ Configure Navigation Properties ]
            HasOptional(t => t.DocumentSeriesItem)
                .WithMany(t => t.TenderHeaders)
                .Map(m => m.MapKey("UniqueIdDocumentSeriesItem"));

            HasOptional(t => t.Resolution)
                .WithMany(t => t.TenderHeaders)
                .Map(m => m.MapKey("IdResolution"));
            #endregion
        }
    }
}
