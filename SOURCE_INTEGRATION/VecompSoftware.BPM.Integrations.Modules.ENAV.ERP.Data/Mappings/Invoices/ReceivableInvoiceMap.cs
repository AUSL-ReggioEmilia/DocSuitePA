using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities.Invoices;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Mappings.Invoices
{
    public class ReceivableInvoiceMap : EntityTypeConfiguration<ReceivableInvoice>
    {
        public ReceivableInvoiceMap() : base()
        {
            //Table
            ToTable("XX_SKYFTE_PASSIVE", "XXEN");
            // Primary Key
            HasKey(t => t.WorkflowId);

            #region [ Configure Properties ]
            Property(x => x.WorkflowId)
                .HasColumnName("WF_DOCSUITE_ID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Supplier)
                .HasColumnName("FORNITORE")
                .IsOptional();

            Property(x => x.PIVACF)
                .HasColumnName("PIVA")
                .IsOptional();

            Property(x => x.InvoiceNumber)
                .HasColumnName("NUMERO_FATTURA")
                .IsOptional();

            Property(x => x.InvoiceDate)
                .HasColumnName("DATA_FATTURA")
                .IsOptional();

            Property(x => x.CIG)
                .HasColumnName("CODICE_CIG")
                .IsOptional();

            Property(x => x.ODA)
                .HasColumnName("CODICE_CIG_ODA")
                .IsOptional();

            Property(x => x.Invoice)
                .HasColumnName("FATTURAXML")
                .IsOptional();

            Property(x => x.InvoiceFilename)
               .HasColumnName("FATTURAFILENAME")
               .IsOptional();

            Property(x => x.AutoInvoice)
                .HasColumnName("AUTOFATTURAXML")
                .IsOptional();

            Property(x => x.AutoInvoiceFilename)
               .HasColumnName("AUTOFATTURAFILENAME")
               .IsOptional();

            Property(x => x.SDIIdentification)
               .HasColumnName("IDENTIFICATIVO_SDI")
               .IsOptional();

            Property(x => x.SDIResult)
                .HasColumnName("ESITO_SDI")
                .IsOptional();

            Property(x => x.SDIDate)
                .HasColumnName("DATA_ESITO")
                .IsOptional();

            Property(x => x.SDIResultDescription)
                .HasColumnName("DESCRIZIONE_ESITO")
                .IsOptional();

            Property(x => x.DocSuiteProtocolYear)
                .HasColumnName("ANNO_PROTOCOLLO")
                .IsOptional();

            Property(x => x.DocSuiteProtocolNumber)
                .HasColumnName("NUMERO_PROTOCOLLO")
                .IsOptional();

            Property(x => x.DocSuiteProtocolDate)
                .HasColumnName("DATA_PROTOCOLLO")
                .IsOptional();

            Property(x => x.WorkflowProcessed)
                .HasColumnName("WF_DOCSUITEPROCESSED")
                .IsOptional();

            Property(x => x.WorkflowStatus)
               .HasColumnName("WF_DOCSUITESTATUS")
               .IsOptional();

            Property(x => x.SectionalVAT)
                .HasColumnName("REGISTRO_IVA")
                .IsOptional();

            Property(x => x.ProtocolNumberVAT)
                .HasColumnName("PROTOCOLLO_IVA")
                .IsOptional();

            Property(x => x.DateVAT)
                .HasColumnName("DATA_IVA")
                .IsOptional();

            Property(x => x.YearVAT)
                .HasColumnName("ANNO_IVA")
                .IsOptional();

            Property(x => x.ERPUpdatedDate)
                .HasColumnName("DATA_AGGIORNAMENTO")
                .IsOptional();

            Property(x => x.RecordOwner)
                .HasColumnName("RESPONSABILE_RECORD")
                .IsOptional();

            Property(x => x.TenantId)
               .HasColumnName("TENANT_ID")
               .IsRequired();

            #endregion
        }
    }
}
