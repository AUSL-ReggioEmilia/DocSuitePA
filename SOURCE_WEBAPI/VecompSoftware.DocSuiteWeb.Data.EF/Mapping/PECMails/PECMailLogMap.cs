using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.PECMails
{
    public class PECMailLogMap : EntityTypeConfiguration<PECMailLog>
    {
        public PECMailLogMap()
            : base()
        {
            // Table
            ToTable("PECMailLog");
            // Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]

            Property(x => x.EntityId)
                .HasColumnName("ID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Description)
                .HasColumnName("Description")
                .IsRequired();

            Property(x => x.LogType)
                .HasColumnName("Type")
                .IsRequired();

            Property(x => x.LogDate)
                .HasColumnName("Date")
                .IsRequired();

            Property(x => x.SystemComputer)
                .HasColumnName("SystemComputer")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("SystemUser")
                .IsOptional();

            Property(x => x.Severity)
                .HasColumnName("Severity")
                .IsOptional();

            Property(x => x.Hash)
               .HasColumnName("Hash")
               .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.RegistrationDate)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser);
            #endregion

            #region [ Configure Navigation Properties ]            

            HasOptional(t => t.PECMail)
                .WithMany(t => t.PECMailLogs)
                .Map(m => m.MapKey("IDMail"));

            #endregion
        }
    }
}
