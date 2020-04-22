using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Mappings
{
    public class CommandMap : EntityTypeConfiguration<Command>
    {
        public CommandMap() : base()
        {
            //Table
            ToTable("XX_SKYRC_COMMAND", "APPS");
            // Primary Key
            HasKey(t => t.RequestId);

            #region [ Configure Properties ]
            Property(x => x.RequestId)
                .HasColumnName("ID_RICHIESTA")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Action)
                .HasColumnName("ACTION")
                .IsOptional();

            Property(x => x.DocumentType)
                .HasColumnName("TYPE_DOC")
                .IsOptional();

            Property(x => x.Owner)
                .HasColumnName("OWNER")
                .IsOptional();

            Property(x => x.FascicleId)
                .HasColumnName("ID_FASCICOLO")
                .IsOptional();

            Property(x => x.ODA)
                .HasColumnName("NUM_ODA")
                .IsOptional();

            Property(x => x.Contract)
                .HasColumnName("CONTRATTO")
                .IsOptional();

            Property(x => x.Contact)
                .HasColumnName("CONTATTO")
                .IsOptional();

            Property(x => x.ContactPEC)
                .HasColumnName("PECCONTACT")
                .IsOptional();

            Property(x => x.InsertTime)
                .HasColumnName("INSERTTIME")
                .IsRequired();

            Property(x => x.ProcessedTime)
                .HasColumnName("PROCESSED_TIME")
                .IsOptional();

            Property(x => x.FascicleYear)
                .HasColumnName("YEAR_FASCICOLO")
                .IsOptional();

            Property(x => x.ODAId)
                .HasColumnName("ID_ODA")
                .IsOptional();

            Property(x => x.ContractId)
                .HasColumnName("ID_CONTRATTO")
                .IsOptional();

            Property(x => x.VendorId)
                .HasColumnName("VENDOR_ID")
                .IsOptional();

            Property(x => x.ProtId)
                .HasColumnName("PROT_ID")
                .IsOptional();

            Property(x => x.CIG)
                .HasColumnName("CIG")
                .IsOptional();
            #endregion
        }
    }
}
