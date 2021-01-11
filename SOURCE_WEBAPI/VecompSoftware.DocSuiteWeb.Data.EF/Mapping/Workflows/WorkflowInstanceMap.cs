using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Workflows
{
    public class WorkflowInstanceMap : EntityTypeConfiguration<WorkflowInstance>
    {
        public WorkflowInstanceMap()
            : base()
        {
            // Table
            ToTable("WorkflowInstances");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdWorkflowInstance")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Status)
               .HasColumnName("Status")
               .IsRequired();

            Property(x => x.InstanceId)
               .HasColumnName("InstanceId")
               .IsOptional();

            Property(x => x.Json)
               .HasColumnName("Json")
               .IsOptional();

            Property(x => x.Subject)
               .HasColumnName("Subject")
               .IsOptional();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired()
                .HasMaxLength(256);

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional()
                .HasMaxLength(256);

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);

            #endregion


            #region [ Configure Navigation Properties ]

            HasRequired(t => t.WorkflowRepository)
                .WithMany(t => t.WorkflowInstances)
                .Map(m => m.MapKey("IdWorkflowRepository"));

            HasMany(t => t.Dossiers)
                .WithMany(t => t.WorkflowInstances)
                .Map(p =>
                    {
                        p.ToTable("DossierWorkflowInstances");
                        p.MapLeftKey("IdWorkflowInstance");
                        p.MapRightKey("IdDossier");
                    });

            HasMany(t => t.Fascicles)
                .WithMany(t => t.WorkflowInstances)
                .Map(p =>
                    {
                        p.ToTable("FascicleWorkflowInstances");
                        p.MapLeftKey("IdWorkflowInstance");
                        p.MapRightKey("IdFascicle");
                    });
            #endregion
        }
    }
}