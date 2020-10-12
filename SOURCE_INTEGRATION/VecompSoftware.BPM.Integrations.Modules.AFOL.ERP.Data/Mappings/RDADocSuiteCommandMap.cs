using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Mappings
{
    public class RDADocSuiteCommandMap : EntityTypeConfiguration<RDADocSuiteCommand>
    {
        public RDADocSuiteCommandMap()
        {
            #region [ Configure Properties ]
            Property(p => p.Typology)
                .HasColumnName("Tipologia")
                .HasMaxLength(256)
                .IsOptional();

            Property(p => p.ApplicantArea)
                .HasColumnName("Area_Richiedente")
                .HasMaxLength(256)
                .IsOptional();
            #endregion
        }
    }
}
