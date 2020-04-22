using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.UDS
{
    public class UDSLogMap : EntityTypeConfiguration<UDSLog>
    {
        public UDSLogMap()
            : base()
        {
            ToTable("UDSLogs", "uds");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdUDSLog")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.IdUDS)
                .HasColumnName("IdUDS")
                .IsRequired();

            Property(x => x.Environment)
                .HasColumnName("Environment")
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("LogDate")
                .IsRequired();

            Property(x => x.SystemComputer)
                .HasColumnName("SystemComputer")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("SystemUser")
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

            Property(x => x.Hash)
               .HasColumnName("Hash")
               .IsOptional();

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

            HasRequired(t => t.Entity)
                .WithMany(t => t.UDSLogs)
                .Map(m => m.MapKey("IdUDSRepository"));
            #endregion
        }
    }
}
