using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Workflows
{
    public class WorkflowEvaluationPropertyMap : EntityTypeConfiguration<WorkflowEvaluationProperty>
    {
        public WorkflowEvaluationPropertyMap()
            : base()
        {
            // Table
            ToTable("WorkflowEvaluationProperties");
            // Primary key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdWorkflowEvaluationProperty")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
               .HasColumnName("Name")
               .IsRequired()
               .HasMaxLength(256);

            Property(x => x.WorkflowType)
               .HasColumnName("WorkflowType")
               .IsRequired();

            Property(x => x.PropertyType)
               .HasColumnName("PropertyType")
               .IsRequired();

            Property(x => x.ValueInt)
                .HasColumnName("ValueInt")
                .IsOptional();

            Property(x => x.ValueDate)
                .HasColumnName("ValueDate")
                .IsOptional();

            Property(x => x.ValueDouble)
                .HasColumnName("ValueDouble")
                .IsOptional();

            Property(x => x.ValueBoolean)
                .HasColumnName("ValueBoolean")
                .IsOptional();

            Property(x => x.ValueGuid)
                .HasColumnName("ValueGuid")
                .IsOptional();

            Property(x => x.ValueString)
                .HasColumnName("ValueString")
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
                .WithMany(t => t.WorkflowEvaluationProperties)
                .Map(m => m.MapKey("IdWorkflowRepository"));

            #endregion
        }
    }
}
