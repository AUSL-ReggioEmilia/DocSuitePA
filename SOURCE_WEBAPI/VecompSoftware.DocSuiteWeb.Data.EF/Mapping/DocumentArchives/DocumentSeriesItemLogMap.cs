using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.DocumentArchives
{
    public class DocumentSeriesItemLogMap : EntityTypeConfiguration<DocumentSeriesItemLog>
    {
        public DocumentSeriesItemLogMap()
            : base()
        {
            ToTable("DocumentSeriesItemLog");
            //primary key
            HasKey(t => t.UniqueId);

            #region [ Configure Navigation Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.EntityId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

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

            Property(x => x.Hash)
              .HasColumnName("Hash")
              .IsOptional();

            Property(x => x.IdDocumentSeriesItem)
                .HasColumnName("IdDocumentSeriesItem")
                .IsRequired();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.RegistrationDate)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.EntityShortId);

            #endregion

            #region [ Configure Navigation Properties ] 

            HasRequired(t => t.Entity)
                .WithMany(t => t.DocumentSeriesItemLogs)
                .Map(m => m.MapKey("UniqueIdDocumentSeriesItem"));
            #endregion
        }
    }
}
