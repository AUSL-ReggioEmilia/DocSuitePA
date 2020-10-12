using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities.Invoices;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Mappings.Invoices
{
    public class PayableInvoiceMap : EntityTypeConfiguration<PayableInvoice>
    {
        public PayableInvoiceMap() : base()
        {
            //Table
            ToTable("XX_SKYFTE_ATTIVE", "XXEN");
            // Primary Key
            HasKey(t => t.RequestId);

            #region [ Configure Properties ]
            Property(x => x.RequestId)
                .HasColumnName("ID_RICHIESTA")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Customer)
                .HasColumnName("CLIENTE")
                .IsOptional();

            Property(x => x.TenantId)
                .HasColumnName("TENANT_ID")
                .IsRequired();

            Property(x => x.InvoiceNumber)
                .HasColumnName("NUMERO_FATTURA")
                .IsOptional();

            Property(x => x.InvoiceDate)
                .HasColumnName("DATA_FATTURA")
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

            Property(x => x.InvoiceFilename)
                .HasColumnName("FATTURAFILENAME")
                .IsOptional();

            Property(x => x.Invoice)
                .HasColumnName("FATTURAXML")
                .IsOptional();

            Property(x => x.WorkflowStarted)
                .HasColumnName("WF_DOCSUITESTARTED")
                .IsOptional();

            Property(x => x.WorkflowId)
               .HasColumnName("WF_DOCSUITEID")
               .IsOptional();

            Property(x => x.WorkflowStatus)
               .HasColumnName("WF_DOCSUITESTATUS")
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

            #endregion

        }
    }
}
