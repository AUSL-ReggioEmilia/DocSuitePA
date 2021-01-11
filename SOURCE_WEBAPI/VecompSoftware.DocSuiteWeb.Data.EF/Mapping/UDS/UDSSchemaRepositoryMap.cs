using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.UDS
{
    public class UDSSchemaRepositoryMap : EntityTypeConfiguration<UDSSchemaRepository>
    {
        public UDSSchemaRepositoryMap() : base()
        {
            // Table
            ToTable("UDSSchemaRepositories", "uds");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdUDSSchemaRepository")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.SchemaXML)
                .HasColumnName("SchemaXML")
                .IsOptional();

            Property(x => x.Version)
                .HasColumnName("Version")
                .IsRequired();

            Property(x => x.ActiveDate)
                .HasColumnName("ActiveDate")
                .IsRequired();

            Property(x => x.ExpiredDate)
                .HasColumnName("ExpiredDate")
                .IsOptional();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

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
                .Ignore(x => x.EntityShortId);
            #endregion


            #region [ Configure Navigation Properties ]

            #endregion
        }
    }
}
