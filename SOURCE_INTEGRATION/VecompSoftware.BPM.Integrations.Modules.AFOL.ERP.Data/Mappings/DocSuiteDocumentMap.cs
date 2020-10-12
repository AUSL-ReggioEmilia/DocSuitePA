using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities.Common;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Mappings
{
    public class DocSuiteDocumentMap : EntityTypeConfiguration<DocSuiteDocument>
    {
        public DocSuiteDocumentMap()
        {
            //Table
            ToTable("DOCSUITE_DOCUMENTS");
            // Primary Key
            HasKey(t => t.Id);


            #region [ Configure Properties ]
            Property(x => x.Id)
                .HasColumnName("IdDocSuiteDocument")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(p => p.FileName)
                .HasColumnName("Nome_File")
                .HasMaxLength(256)
                .IsRequired();

            Property(p => p.Content)
                .HasColumnName("Content")
                .IsRequired();

            Property(p => p.InsertDate)
                .HasColumnName("DataInserimento")
                .IsRequired();

            Property(p => p.WFDocSuiteStarted)
                .HasColumnName("WF_DocSuiteStarted")
                .IsOptional();

            Property(p => p.WFDocSuiteId)
                .HasColumnName("WF_DocSuiteID")
                .HasColumnType("uniqueidentifier")
                .IsOptional();

            Property(p => p.WFDocSuiteStatus)
                .HasColumnName("WF_DocSuiteStatus")
                .HasColumnType("smallint")
                .IsOptional();

            Map<MainDocument>(m => {
                m.ToTable("DOCSUITE_DOCUMENTS");
                m.Requires("Discriminator").HasValue((short)DocSuiteDocumentType.MainDocument);
            });

            Map<AttachedDocument>(m => {
                m.ToTable("DOCSUITE_DOCUMENTS");
                m.Requires("Discriminator").HasValue((short)DocSuiteDocumentType.AttachedDocument);
            });
            #endregion
        }
    }
}
