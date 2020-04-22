using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Mappings
{
    public class PayableInvoiceMap : EntityTypeConfiguration<PayableInvoice>
    {
        public PayableInvoiceMap() : base()
        {
            //Table
            ToTable("FATTURE_ATTIVE");
            // Primary Key
            HasKey(t => t.RequestId);

            #region [ Configure Properties ]
            Property(x => x.RequestId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Customer)
                .HasColumnName("Cliente")
                .IsOptional();

            Property(x => x.InvoiceNumber)
                .HasColumnName("Numero_Fattura")
                .IsOptional();

            Property(x => x.InvoiceDate)
                .HasColumnName("Data_Fattura")
                .IsOptional();

            Property(x => x.SectionalVAT)
                .HasColumnName("Registro_IVA")
                .IsOptional();

            Property(x => x.ProtocolNumberVAT)
                .HasColumnName("Protocollo_IVA")
                .IsOptional();

            Property(x => x.DateVAT)
                .HasColumnName("Data_IVA")
                .IsOptional();

            Property(x => x.YearVAT)
                .HasColumnName("Anno_IVA")
                .IsOptional();

            Property(x => x.InvoiceFilename)
                .HasColumnName("FatturaFilename")
                .IsOptional();

            Property(x => x.Invoice)
                .HasColumnName("FatturaXML")
                .IsOptional();

            Property(x => x.InsertDate)
                .HasColumnName("DataInserimento")
                .IsOptional();

            Property(x => x.WorkflowStarted)
                .HasColumnName("WF_DocSuiteStarted")
                .IsOptional();

            Property(x => x.WorkflowId)
               .HasColumnName("WF_DocSuiteID")
               .IsOptional();

            Property(x => x.WorkflowStatus)
               .HasColumnName("WF_DocSuiteStatus")
               .IsOptional();

            Property(x => x.SDIIdentification)
                .HasColumnName("Identificativo_SDI")
                .IsOptional();

            Property(x => x.SDIResult)
                .HasColumnName("Esito_SDI")
                .IsOptional();

            Property(x => x.SDIDate)
                .HasColumnName("DataEsito_SDI")
                .IsOptional();

            Property(x => x.SDIResultDescription)
                .HasColumnName("DescrizioneEsito_SDI")
                .IsOptional();

            Property(x => x.DocSuiteProtocolYear)
                .HasColumnName("Anno_Protocollo")
                .IsOptional();

            Property(x => x.DocSuiteProtocolNumber)
                .HasColumnName("Numero_Protocollo")
                .IsOptional();

            #endregion

        }
    }
}
