using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Desks
{
    public class DeskLogMap : EntityTypeConfiguration<DeskLog>
    {
        public DeskLogMap()
            : base()
        {
            // Table
            ToTable("DeskLogs");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdDeskLog")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.LogDate)
                .HasColumnName("LogDate")
                .IsRequired();

            Property(x => x.LogDescription)
                .HasColumnName("LogDescription")
                .IsRequired();

            Property(x => x.LogType)
                .HasColumnName("LogType")
                .IsRequired();

            Property(x => x.Severity)
                .HasColumnName("Severity")
                .IsOptional();

            Property(x => x.SystemComputer)
                .HasColumnName("SystemComputer")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("SystemUser")
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityId)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.Hash)
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]
            HasRequired(t => t.Entity)
                .WithMany(t => t.DeskLogs)
                .Map(m => m.MapKey("IdDesk"));
            #endregion
        }
    }
}
