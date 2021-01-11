using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Monitors;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Monitors
{
    public class TransparentAdministrationMonitorLogMap : EntityTypeConfiguration<TransparentAdministrationMonitorLog>
    {
        public TransparentAdministrationMonitorLogMap()
            : base()
        {
            ToTable("TransparentAdministrationMonitorLogs");
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdTransparentAdministrationMonitorLog")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.DocumentUnitName)
                .HasColumnName("DocumentUnitName")
                .IsRequired();

            Property(x => x.Date)
                .HasColumnName("Date")
                .IsRequired();

            Property(x => x.Note)
                .HasColumnName("Note")
                .IsOptional();

            Property(x => x.Rating)
                .HasColumnName("Rating")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);

            #endregion

            #region [ Configure Navigation Properties ]
            HasRequired(t => t.DocumentUnit)
                .WithMany(t => t.TransparentAdministrationMonitorLogs)
                .Map(m => m.MapKey("IdDocumentUnit"));

            HasOptional(t => t.Role)
                .WithMany(t => t.TransparentAdministrationMonitorLogs)
                .Map(m => m.MapKey("IdRole"));

            #endregion
        }
    }
}
