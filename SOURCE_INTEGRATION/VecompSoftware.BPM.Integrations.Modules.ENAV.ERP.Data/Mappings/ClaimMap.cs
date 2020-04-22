using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.ENAV.ERP.Data.Entities;

namespace VecompSoftware.BPM.Integrations.Modules.ENAV.ERP.Data.Mappings
{
    public class ClaimMap : EntityTypeConfiguration<Claim>
    {
        public ClaimMap() : base()
        {
            //Table
            ToTable("XX_SKYRC_CLAIM", "APPS");
            // Primary Key
            HasKey(t => t.Token);

            #region [ Configure Properties ]
            Property(x => x.Token)
                .HasColumnName("CLAIM")
                .HasMaxLength(36)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            #endregion
        }
    }
}
