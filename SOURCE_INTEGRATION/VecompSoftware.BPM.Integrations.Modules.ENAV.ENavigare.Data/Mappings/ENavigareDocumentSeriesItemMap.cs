using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.ENAV.ENavigare.Data.Entities;

namespace VecompSoftware.BPM.Integrations.Modules.ENAV.ENavigare.Data.Mappings
{
    internal class ENavigareDocumentSeriesItemMap : EntityTypeConfiguration<ENavigareDocumentSeriesItem>
    {
        public ENavigareDocumentSeriesItemMap() : base()
        {
            //Table
            ToTable("SkyDOC_DocumentSeries");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("UniqueID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.DataValiditaScheda)
                .HasColumnName("DataValiditaScheda")
                .IsOptional();

            Property(x => x.DataUltimoAggiornamento)
                .HasColumnName("DataUltimoAggiornamento")
                .IsOptional();

            Property(x => x.DataPubblicazione)
                .HasColumnName("DataPubblicazione")
                .IsOptional();

            Property(x => x.DataRitiro)
                .HasColumnName("DataRitiro")
                .IsOptional();

            Property(x => x.Codice)
                .HasColumnName("Codice")
                .IsRequired();

            Property(x => x.Oggetto)
                .HasColumnName("Oggetto")
                .IsRequired();

            Property(x => x.Abstract)
                .HasColumnName("Abstract")
                .IsRequired();

            Property(x => x.ProceduraNomiFile)
                .HasColumnName("ProceduraNomiFile")
                .IsOptional();

            Property(x => x.ProceduraPosizioni)
                .HasColumnName("ProceduraPosizioni")
                .IsOptional();

            Property(x => x.LineeGuidaNomiFile)
                .HasColumnName("LineeGuidaNomiFile")
                .IsOptional();

            Property(x => x.LineeGuidaPosizioni)
                .HasColumnName("LineeGuidaPosizioni")
                .IsOptional();

            Property(x => x.ModulisticaNomiFile)
                .HasColumnName("ModulisticaNomiFile")
                .IsOptional();

            Property(x => x.ModulusticaPosizioni)
                .HasColumnName("ModulisticaPosizioni")
                .IsOptional();

            Property(x => x.Url)
                .HasColumnName("Url")
                .IsOptional();

            Property(x => x.InEvidenza)
                .HasColumnName("InEvidenza")
                .IsRequired();

            #endregion
        }
    }
}
