using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Processes;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Processes
{
    public class ProcessFascicleWorkflowRepositoryMap : EntityTypeConfiguration<ProcessFascicleWorkflowRepository>
    {
        public ProcessFascicleWorkflowRepositoryMap() : base()
        {
            ToTable("ProcessFascicleWorkflowRepositories", "dbo");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdProcessFascicleWorkflowRepository")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);            
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
            Ignore(x => x.EntityShortId);
            Ignore(x => x.EntityId);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Process)
                .WithMany(t => t.FascicleWorkflowRepositories)
                .Map(m => m.MapKey("IdProcess"));
            HasRequired(t => t.DossierFolder)
                .WithMany(t => t.FascicleWorkflowRepositories)
                .Map(m => m.MapKey("IdDossierFolder"));
            HasRequired(t => t.WorkflowRepository)
                .WithMany(t => t.FascicleWorkflowRepositories)
                .Map(m => m.MapKey("IdWorkflowRepository"));
            #endregion
        }
    }
}
