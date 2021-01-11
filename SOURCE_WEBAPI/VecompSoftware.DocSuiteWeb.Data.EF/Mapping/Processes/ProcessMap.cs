using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Processes;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Processes
{
    public class ProcessMap: EntityTypeConfiguration<Process>
    {
        public ProcessMap(): base()
        {
            ToTable("Processes", "dbo");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdProcess")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();
            Property(x => x.StartDate)
                .HasColumnName("StartDate")
                .IsRequired();
            Property(x => x.EndDate)
                .HasColumnName("EndDate")
                .IsOptional();
            Property(x => x.Note)
                .HasColumnName("Note")
                .IsRequired();
            Property(x => x.ProcessType)
                .HasColumnName("ProcessType")
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
            Ignore(x => x.EntityShortId);
            Ignore(x => x.EntityId);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Category)
                .WithMany(t => t.Processes)
                .Map(m => m.MapKey("IdCategory"));
            HasRequired(t => t.Dossier)
                .WithMany(t => t.Processes)
                .Map(m => m.MapKey("IdDossier"));
            HasMany(t => t.Roles)
                .WithMany(t => t.Processes)
                .Map(m =>
                {
                    m.ToTable("ProcessRoles");
                    m.MapLeftKey("IdProcess");
                    m.MapRightKey("IdRole");
                });

            #endregion
        }
    }
}
