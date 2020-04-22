using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Dossiers
{
    public class DossierMap : EntityTypeConfiguration<Dossier>
    {
        public DossierMap()
            : base()
        {
            ToTable("Dossiers");
            HasKey(t => t.UniqueId);

            #region [ Confifure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdDossier")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Year)
                .HasColumnName("Year")
                .IsRequired();

            Property(x => x.Number)
                .HasColumnName("Number")
                .IsRequired();

            Property(x => x.Subject)
                .HasColumnName("Subject")
                .IsRequired();

            Property(x => x.Note)
                .HasColumnName("Note")
                .IsOptional();

            Property(x => x.StartDate)
                .HasColumnName("StartDate")
                .IsRequired();

            Property(x => x.EndDate)
                .HasColumnName("EndDate")
                .IsOptional();

            Property(x => x.JsonMetadata)
                .HasColumnName("JsonMetadata")
                .IsOptional();

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

            HasRequired(t => t.Container)
                .WithMany(t => t.Dossiers)
                .Map(m => m.MapKey("IdContainer"));

            HasMany(t => t.Contacts)
               .WithMany(t => t.Dossiers)
               .Map(m =>
               {
                   m.ToTable("DossierContacts");
                   m.MapLeftKey("IdDossier");
                   m.MapRightKey("IdContact");
               });

            HasMany(t => t.Messages)
                .WithMany(t => t.Dossiers)
                .Map(m =>
                {
                    m.ToTable("DossierMessages");
                    m.MapLeftKey("IdDossier");
                    m.MapRightKey("IdMessage");
                });

            HasOptional(t => t.MetadataRepository)
                .WithMany(t => t.Dossiers)
                .Map(m => m.MapKey("IdMetadataRepository"));
            #endregion


        }
    }
}
