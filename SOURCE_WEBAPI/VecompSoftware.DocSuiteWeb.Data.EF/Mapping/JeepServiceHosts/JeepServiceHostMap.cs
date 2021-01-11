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

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser);

            #endregion

            #region [ Configure Navigation Properties ]

            #endregion
        }
    }
}
