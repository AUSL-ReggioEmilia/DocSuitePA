using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Protocols
{
    public class ProtocolLogMap : EntityTypeConfiguration<ProtocolLog>
    {
        public ProtocolLogMap()
            : base()
        {
            ToTable("ProtocolLog");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.EntityId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Year)
               .HasColumnName("Year")
               .IsRequired();

            Property(x => x.Number)
               .HasColumnName("Number")
               .IsRequired();

            Property(x => x.LogDate)
               .HasColumnName("LogDate")
               .IsRequired();

            Property(x => x.SystemComputer)
               .HasColumnName("SystemComputer")
               .IsRequired();

            Property(x => x.RegistrationUser)
               .HasColumnName("SystemUser")
               .IsRequired();

            Property(x => x.Program)
               .HasColumnName("Program")
               .IsRequired();

            Property(x => x.LogType)
               .HasColumnName("LogType")
               .IsRequired();

            Property(x => x.LogDescription)
               .HasColumnName("LogDescription")
               .IsRequired();

            Property(x => x.Severity)
               .HasColumnName("Severity")
               .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.RegistrationDate)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Entity)
                .WithMany(t => t.ProtocolLogs)
                .Map(m => m.MapKey("UniqueIdProtocol"));

            #endregion
        }
    }
}
