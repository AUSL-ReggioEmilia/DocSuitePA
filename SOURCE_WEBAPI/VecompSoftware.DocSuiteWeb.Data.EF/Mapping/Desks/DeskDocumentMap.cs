using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Desks
{
    public class DeskDocumentMap : EntityTypeConfiguration<DeskDocument>
    {
        public DeskDocumentMap()
            : base()
        {
            // Table
            ToTable("DeskDocuments");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdDeskDocument")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.IsActive)
                .HasColumnName("IsActive")
                .IsRequired();

            Property(t => t.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(t => t.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .HasMaxLength(256);

            Property(t => t.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsOptional()
                .HasMaxLength(256);

            Property(t => t.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.DocumentType)
                .HasColumnName("DocumentType")
                .IsRequired();

            Property(x => x.IdDocument)
                .HasColumnName("IdDocument")
                .IsOptional();

            Ignore(x => x.EntityId)
                .Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId);
            #endregion


            #region [ Configure Navigation Properties ]
            HasOptional(t => t.Desk)
                .WithMany(t => t.DeskDocuments)
                .Map(m => m.MapKey("IdDesk"));

            #endregion
        }
    }
}
