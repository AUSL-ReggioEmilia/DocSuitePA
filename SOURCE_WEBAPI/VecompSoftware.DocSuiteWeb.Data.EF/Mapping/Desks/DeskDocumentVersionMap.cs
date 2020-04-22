using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Desks
{
    public class DeskDocumentVersionMap : EntityTypeConfiguration<DeskDocumentVersion>
    {
        public DeskDocumentVersionMap()
            : base()
        {
            // Table
            ToTable("DeskDocumentVersions");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdDeskDocumentVersion")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Version)
                .HasColumnName("Version")
                .IsOptional();

            Property(t => t.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(t => t.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional()
                .HasMaxLength(256);

            Property(t => t.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsOptional()
                .HasMaxLength(256);

            Property(t => t.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);
            #endregion


            #region [ Configure Navigation Properties ]
            HasOptional(t => t.DeskDocument)
                .WithMany(t => t.DeskDocumentVersions)
                .Map(m => m.MapKey("IdDeskDocument"));
            #endregion
        }
    }
}
