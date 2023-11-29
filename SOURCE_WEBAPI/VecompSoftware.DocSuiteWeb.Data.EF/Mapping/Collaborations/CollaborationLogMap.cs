using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Collaborations
{
    public class CollaborationLogMap : EntityTypeConfiguration<CollaborationLog>
    {
        public CollaborationLogMap()
            : base()
        {
            ToTable("CollaborationLog");
            //Primary Key
            HasKey(k => k.EntityId);

            #region [ Configure Properties ]

            Property(x => x.EntityId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.CollaborationIncremental)
                .HasColumnName("CollaborationIncremental")
                .IsOptional();

            Property(x => x.Incremental)
                .HasColumnName("Incremental")
                .IsOptional();

            Property(x => x.IdChain)
                .HasColumnName("IdChain")
                .IsOptional();

            Property(x => x.LogDate)
                .HasColumnName("LogDate")
                .IsOptional();

            Property(x => x.SystemComputer)
                .HasColumnName("SystemComputer")
                .IsOptional();

            Property(x => x.RegistrationUser)
                .HasColumnName("SystemUser")
                .IsOptional();

            Property(x => x.Program)
                .HasColumnName("Program")
                .IsOptional();

            Property(x => x.LogType)
                .HasColumnName("LogType")
                .IsOptional();

            Property(x => x.LogDescription)
                .HasColumnName("LogDescription")
                .IsOptional();

            Property(x => x.Severity)
                .HasColumnName("Severity")
                .IsOptional();

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .IsRequired();

            Ignore(x => x.Timestamp)
                 .Ignore(x => x.RegistrationDate)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.Hash)
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]

            HasOptional(t => t.Entity)
                .WithMany(t => t.CollaborationLogs)
                .Map(m => m.MapKey("IdCollaboration"));
            #endregion
        }
    }
}
