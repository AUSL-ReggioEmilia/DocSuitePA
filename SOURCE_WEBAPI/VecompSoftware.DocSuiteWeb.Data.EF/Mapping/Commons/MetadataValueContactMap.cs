using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class MetadataValueContactMap : EntityTypeConfiguration<MetadataValueContact>
    {
        public MetadataValueContactMap()
            : base()

        {
            ToTable("MetadataValueContacts");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdMetadataValueContact")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            Property(x => x.ContactManual)
                .HasColumnName("ContactManual")
                .IsOptional();           

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
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

            Ignore(x => x.EntityShortId)
                .Ignore(x => x.EntityId);

            #endregion

            #region[ Configure Navigation Properties ]   
            HasOptional(t => t.Fascicle)
                .WithMany(t => t.MetadataValueContacts)
                .Map(m => m.MapKey("IdFascicle"));

            HasOptional(t => t.Dossier)
                .WithMany(t => t.MetadataValueContacts)
                .Map(m => m.MapKey("IdDossier"));

            HasOptional(t => t.Contact)
                .WithMany(t => t.MetadataValueContacts)
                .Map(m => m.MapKey("IdContact"));
            #endregion
        }
    }
}
