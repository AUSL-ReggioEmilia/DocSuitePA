using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities.Common;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Mappings
{
    public class DocSuiteEventMap : EntityTypeConfiguration<DocSuiteEvent>
    {
        public DocSuiteEventMap()
        {
            //Table
            ToTable("DOCSUITE_EVENTS");
            // Primary Key
            HasKey(t => t.Id);

            #region [ Configure Properties ]
            Property(x => x.Id)
                .HasColumnName("IdDocSuiteEvent")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(p => p.Year)
                .HasColumnName("Anno")
                .IsRequired();

            Property(p => p.Number)
                .HasColumnName("Numero")
                .HasMaxLength(256)
                .IsRequired();

            Property(p => p.CategoryName)
                .HasColumnName("Classificazione")
                .HasMaxLength(2000)
                .IsRequired();

            Property(p => p.Subject)
                .HasColumnName("Oggetto")
                .HasMaxLength(4000)
                .IsRequired();

            Property(p => p.Date)
                .HasColumnName("Data")
                .IsRequired();

            Property(p => p.WFERPStarted)
                .HasColumnName("WF_DocSuiteStarted")
                .IsOptional();

            Property(p => p.WFERPStatus)
                .HasColumnName("WF_DocSuiteStatus")
                .HasColumnType("smallint")
                .IsOptional();

            Map<ResolutionEvent>(m => {
                m.ToTable("DOCSUITE_EVENTS");
                m.Requires("Discriminator").HasValue(1);
            });
            #endregion
        }
    }
}
