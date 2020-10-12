using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities.HR;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Mappings.HR
{
    public class SkyDocDocumentMap : EntityTypeConfiguration<SkyDocDocument>
    {
        public SkyDocDocumentMap()
        {
            //Table
            ToTable("SKYDOC_DOCUMENTS");
            // Primary Key
            HasKey(t => t.Id);


            #region [ Configure Properties ]
            Property(x => x.Id)
                .HasColumnName("IdDocument")
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

            Property(p => p.WFSkyDocStarted)
                .HasColumnName("WF_SkyDocStarted")
                .IsOptional();

            Property(p => p.WFSkyDocId)
                .HasColumnName("WF_SkyDocID")
                .HasColumnType("uniqueidentifier")
                .IsOptional();

            Property(p => p.WFSkyDocStatus)
                .HasColumnName("WF_SkyDocStatus")
                .HasColumnType("smallint")
                .IsOptional();

            Property(x => x.DocumentType)
              .HasColumnName("Discriminator")
              .IsRequired();
            #endregion
        }
    }
}
