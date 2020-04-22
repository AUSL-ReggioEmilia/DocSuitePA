using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Desks
{
    public class DeskDocumentEndorsementMap : EntityTypeConfiguration<DeskDocumentEndorsement>
    {
        public DeskDocumentEndorsementMap()
            : base()
        {
            // Table
            ToTable("DeskDocumentEndorsements");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdDeskDocumentEndorsement")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Endorsement)
                .HasColumnName("Endorsement")
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.EntityId)
                .Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId);
            #endregion


            #region [ Configure Navigation Properties ]
            HasOptional(t => t.DeskRoleUser)
                .WithMany(t => t.DeskDocumentEndorsements)
                .Map(m => m.MapKey("idDeskRoleUser"));

            HasOptional(t => t.DeskDocumentVersion)
                .WithMany(t => t.DeskDocumentEndorsements)
                .Map(m => m.MapKey("IdDeskDocumentVersion"));
            #endregion
        }
    }
}
