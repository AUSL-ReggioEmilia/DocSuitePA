using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Templates;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Templates
{
    public class TemplateReportMap : EntityTypeConfiguration<TemplateReport>
    {
        public TemplateReportMap() : base()
        {
            ToTable("TemplateReports");
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdTemplateReport")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            Property(x => x.IdArchiveChain)
                .HasColumnName("IdArchiveChain")
                .IsRequired();

            Property(x => x.Status)
                .HasColumnName("Status")
                .IsRequired();

            Property(x => x.Environment)
                .HasColumnName("Environment")
                .IsRequired();

            Property(x => x.ReportBuilderJsonModel)
                .HasColumnName("ReportBuilderJsonModel")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
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

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Navigation Properties ]

            #endregion
        }
    }
}
