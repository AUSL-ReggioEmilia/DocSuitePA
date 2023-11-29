using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Protocols
{
    public class ProtocolJournalMap : EntityTypeConfiguration<ProtocolJournal>
    {
        public ProtocolJournalMap()
            : base()
        {
            ToTable("ProtocolJournalLog");
            //Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]
            Property(x => x.EntityId)
                .HasColumnName("IdProtocolJournalLog")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.ProtocolJournalDate)
               .HasColumnName("ProtocolJournalDate")
               .IsRequired();

            Property(x => x.LogDate)
               .HasColumnName("LogDate")
               .IsRequired();

            Property(x => x.SystemComputer)
               .HasColumnName("SystemComputer")
               .IsOptional();

            Property(x => x.SystemUser)
               .HasColumnName("SystemUser")
               .IsOptional();

            Property(x => x.StartDate)
               .HasColumnName("StartDate")
               .IsOptional();

            Property(x => x.EndDate)
               .HasColumnName("EndDate")
               .IsOptional();

            Property(x => x.ProtocolTotal)
               .HasColumnName("ProtocolTotal")
               .IsOptional();

            Property(x => x.ProtocolRegister)
               .HasColumnName("ProtocolRegister")
               .IsOptional();

            Property(x => x.ProtocolError)
               .HasColumnName("ProtocolError")
               .IsOptional();

            Property(x => x.ProtocolCancelled)
               .HasColumnName("ProtocolCancelled")
               .IsOptional();

            Property(x => x.ProtocolActive)
               .HasColumnName("ProtocolActive")
               .IsOptional();

            Property(x => x.ProtocolOthers)
               .HasColumnName("ProtocolOthers")
               .IsRequired();

            Property(x => x.IdDocument)
               .HasColumnName("IdDocument")
               .IsOptional();

            Property(x => x.LogDescription)
               .HasColumnName("LogDescription")
               .IsOptional();

            Property(x => x.LogDescription)
               .HasColumnName("LogDescription")
               .IsOptional();

            Property(x => x.UniqueId)
               .HasColumnName("UniqueId")
               .IsRequired();

            Property(x => x.RegistrationDate)
               .HasColumnName("RegistrationDate")
               .IsRequired();

            Property(x => x.RegistrationUser)
               .HasColumnName("RegistrationUser")
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

            Ignore(x => x.EntityShortId)
                .Ignore(x => x.WorkflowAutoComplete)
                .Ignore(x => x.WorkflowName)
                .Ignore(x => x.IdWorkflowActivity)
                .Ignore(x => x.WorkflowActions); ;
            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.TenantAOO)
                .WithMany(t => t.ProtocolJournals)
                .Map(m => m.MapKey("IdTenantAOO"));

            HasRequired(t => t.Location)
                .WithMany(t => t.ProtocolJournals)
                .Map(m => m.MapKey("IdLocation"));
            #endregion
        }
    }
}
