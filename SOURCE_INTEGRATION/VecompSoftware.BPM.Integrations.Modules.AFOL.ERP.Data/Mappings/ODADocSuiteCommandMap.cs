using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Mappings
{
    public class ODADocSuiteCommandMap : EntityTypeConfiguration<ODADocSuiteCommand>
    {
        public ODADocSuiteCommandMap()
        {
            #region [ Configure Properties ] 
            Property(p => p.CIG)
                .HasColumnName("CIG")
                .HasMaxLength(256)
                .IsOptional();

            Property(p => p.RDAReference)
                .HasColumnName("Riferimento_RDA")
                .HasMaxLength(256)
                .IsOptional();
            #endregion
        }
    }
}
