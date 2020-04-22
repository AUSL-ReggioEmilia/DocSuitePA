using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class MetadataRepositoryMap : EntityTypeConfiguration<MetadataRepository>
    {
        public MetadataRepositoryMap()
            : base()

        {
            ToTable("MetadataRepositories");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdMetadataRepository")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            Property(x => x.JsonMetadata)
                .HasColumnName("JsonMetadata")
                .IsRequired();

            Property(x => x.Status)
                .HasColumnName("Status")
                .IsRequired();

            Property(x => x.Version)
                .HasColumnName("Version")
                .IsRequired();

            Property(x => x.DateFrom)
                .HasColumnName("DateFrom")
                .IsRequired();

            Property(x => x.DateTo)
                .HasColumnName("DateTo")
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

            #region[ Configure Navigation Propoerties ]                
            #endregion
        }
    }
}
