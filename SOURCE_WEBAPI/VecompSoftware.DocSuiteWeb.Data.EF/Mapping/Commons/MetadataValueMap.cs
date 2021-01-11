using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class MetadataValueMap : EntityTypeConfiguration<MetadataValue>
    {
        public MetadataValueMap()
            : base()

        {
            ToTable("MetadataValues");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdMetadataValue")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            Property(x => x.PropertyType)
                .HasColumnName("PropertyType")
                .IsRequired();

            Property(x => x.ValueInt)
                .HasColumnName("ValueInt")
                .IsOptional();

            Property(x => x.ValueDate)
                .HasColumnName("ValueDate")
                .IsOptional();

            Property(x => x.ValueDouble)
                .HasColumnName("ValueDouble")
                .IsOptional();

            Property(x => x.ValueBoolean)
                .HasColumnName("ValueBoolean")
                .IsOptional();

            Property(x => x.ValueGuid)
                .HasColumnName("ValueGuid")
                .IsOptional();

            Property(x => x.ValueString)
               .HasColumnName("ValueString")
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
                .WithMany(t => t.SourceMetadataValues)
                .Map(m => m.MapKey("IdFascicle"));

            HasOptional(t => t.Dossier)
                .WithMany(t => t.SourceMetadataValues)
                .Map(m => m.MapKey("IdDossier"));
            #endregion
        }
    }
}
