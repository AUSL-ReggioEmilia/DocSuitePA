using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Processes;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Processes
{
    public class ProcessFascicleTemplateMap: EntityTypeConfiguration<ProcessFascicleTemplate>
    {
        public ProcessFascicleTemplateMap(): base()
        {
            ToTable("ProcessFascicleTemplates", "dbo");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdProcessFascicleTemplate")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();
            Property(x => x.JsonModel)
                .HasColumnName("JsonModel")
                .IsRequired();
            Property(x => x.StartDate)
                .HasColumnName("StartDate")
                .IsRequired();
            Property(x => x.EndDate)
                .HasColumnName("EndDate")
                .IsOptional();
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
                .WithMany(t => t.FascicleTemplates)
                .Map(m => m.MapKey("IdProcess"));
            HasRequired(t => t.DossierFolder)
                .WithMany(t => t.FascicleTemplates)
                .Map(m => m.MapKey("IdDossierFolder"));

            #endregion
        }
    }
}
