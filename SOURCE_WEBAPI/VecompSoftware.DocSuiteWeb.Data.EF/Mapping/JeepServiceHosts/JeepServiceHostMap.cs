using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.JeepServiceHosts;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.JeepServiceHosts
{
    public class JeepServiceHostMap : EntityTypeConfiguration<JeepServiceHost>
    {
        public JeepServiceHostMap()
            : base()
        {
            // Table
            ToTable("JeepServiceHosts");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdJeepServiceHost")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Hostname)
                .HasColumnName("Hostname")
                .IsRequired();

            Property(x => x.IsActive)
                .HasColumnName("IsActive")
                .IsRequired();

            Property(x => x.IsDefault)
                .HasColumnName("IsDefault")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsOptional();

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

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);

            #endregion

            #region [ Configure Navigation Properties ]

            #endregion
        }
    }
}
