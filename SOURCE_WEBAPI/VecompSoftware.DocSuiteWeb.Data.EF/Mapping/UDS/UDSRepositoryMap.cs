using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.UDS
{
    public class UDSRepositoryMap : EntityTypeConfiguration<UDSRepository>
    {
        public UDSRepositoryMap() : base()
        {
            ToTable("UDSRepositories", "uds");

            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdUDSRepository")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.ModuleXML)
                .HasColumnName("ModuleXML")
                .IsRequired();

            Property(x => x.Version)
                .HasColumnName("Version")
                .IsRequired();

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            Property(x => x.Alias)
                .HasColumnName("Alias")
                .IsRequired();

            Property(x => x.DSWEnvironment)
                .HasColumnName("DSWEnvironment")
                .IsOptional();


            Property(x => x.SequenceCurrentYear)
                .HasColumnName("SequenceCurrentYear")
                .IsRequired();

            Property(x => x.SequenceCurrentNumber)
                .HasColumnName("SequenceCurrentNumber")
                .IsRequired();

            Property(x => x.ActiveDate)
                .HasColumnName("ActiveDate")
                .IsRequired();

            Property(x => x.Status)
                .HasColumnName("Status")
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

            HasOptional(t => t.Container)
                .WithMany(t => t.UDSRepositories)
                .Map(m => m.MapKey("IdContainer"));

            HasRequired(t => t.SchemaRepository)
                .WithMany(t => t.Repositories)
                .Map(m => m.MapKey("IdUDSSchemaRepository"));

            #endregion
        }
    }
}
