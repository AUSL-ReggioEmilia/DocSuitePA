using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Dossiers
{
    public class DossierLogMap : EntityTypeConfiguration<DossierLog>
    {
        public DossierLogMap()
            : base()
        {
            ToTable("DossierLogs");
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdDossierLog")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

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
                .WithMany(t => t.DossierLogs)
                .Map(m => m.MapKey("IdDossier"));

            HasOptional(t => t.DossierFolder)
                .WithMany(t => t.DossierLogs)
                .Map(m => m.MapKey("IdDossierFolder"));
            #endregion


        }
    }
}
