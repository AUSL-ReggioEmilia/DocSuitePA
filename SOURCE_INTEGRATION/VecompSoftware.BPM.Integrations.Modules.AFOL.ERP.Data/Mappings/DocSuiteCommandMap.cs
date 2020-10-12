using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities.Common;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Mappings
{
    public class DocSuiteCommandMap : EntityTypeConfiguration<DocSuiteCommand>
    {
        public DocSuiteCommandMap()
        {
            //Table
            ToTable("DOCSUITE_COMMANDS");
            // Primary Key
            HasKey(t => t.Id);

            #region [ Configure Properties ]
            Property(x => x.Id)
                .HasColumnName("IdDocSuiteCommand")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(p => p.SupplierName)
                .HasColumnName("Fornitore_Denominazione")
                .HasMaxLength(256)
                .IsRequired();

            Property(p => p.SupplierPIVACF)
                .HasColumnName("Fornitore_PIVACF")
                .HasMaxLength(256)
                .IsRequired();

            Property(p => p.Number)
                .HasColumnName("Numero")
                .HasMaxLength(256)
                .IsRequired();

            Property(p => p.Date)
                .HasColumnName("Data")
                .IsRequired();

            Property(p => p.CostCenter)
                .HasColumnName("Centro_Costo")
                .HasMaxLength(256)
                .IsOptional();

            Property(p => p.InsertDate)
                .HasColumnName("DataInserimento")
                .IsRequired();

            Property(p => p.TenantId)
                .HasColumnName("Tenant_ID")
                .HasColumnType("uniqueidentifier")
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

            Property(p => p.UDSId)
                .HasColumnName("Identificativo_UDS")
                .HasColumnType("uniqueidentifier")
                .IsOptional();

            Property(x => x.Timestamp)
              .HasColumnName("Timestamp")
              .IsRequired();

            Map<ODADocSuiteCommand>(m => {
                m.ToTable("DOCSUITE_COMMANDS");
                m.Requires("Discriminator").HasValue((short)DocSuiteCommandType.ODA);
            });

            Map<RDADocSuiteCommand>(m => {
                m.ToTable("DOCSUITE_COMMANDS");
                m.Requires("Discriminator").HasValue((short)DocSuiteCommandType.RDA);
            });

            Map<PreventivoDocSuiteCommand>(m => {
                m.ToTable("DOCSUITE_COMMANDS");
                m.Requires("Discriminator").HasValue((short)DocSuiteCommandType.Preventivo);
            });
            #endregion

            #region [ Navigation Properties ]
            HasMany(p => p.Documents)
                .WithRequired(d => d.Command)
                .Map(d => d.MapKey("IdDocSuiteCommand"));
            #endregion
        }
    }
}
