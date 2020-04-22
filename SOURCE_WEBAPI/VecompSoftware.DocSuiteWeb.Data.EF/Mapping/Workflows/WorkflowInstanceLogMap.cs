using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Workflows
{
    public class WorkflowInstanceLogMap : EntityTypeConfiguration<WorkflowInstanceLog>
    {
        public WorkflowInstanceLogMap()
            : base()
        {
            ToTable("WorkflowInstanceLogs");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdWorkflowInstanceLog")
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
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.Hash);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Entity)
                .WithMany(t => t.WorkflowInstanceLogs)
                .Map(m => m.MapKey("IdWorkflowInstance"));

            #endregion
        }
    }
}
