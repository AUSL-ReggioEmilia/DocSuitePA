using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Mappings
{
    public class PreventivoDocSuiteCommandMap : EntityTypeConfiguration<PreventivoDocSuiteCommand>
    {
        public PreventivoDocSuiteCommandMap()
        {
            #region [ Configure Properties ]
            Property(p => p.RDAReference)
                .HasColumnName("Riferimento_RDA")
                .HasMaxLength(256)
                .IsOptional();
            #endregion
        }
    }
}
