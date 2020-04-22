using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Parameters;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Parameters
{
    public class ParameterEnvMap : EntityTypeConfiguration<ParameterEnv>
    {
        public ParameterEnvMap()
            : base()
        {
            ToTable("ParameterEnv");
            HasKey(t => t.UniqueId);

            Property(p => p.Name)
                .HasColumnName("KeyName")
                .IsRequired();

            Property(p => p.Value)
                .HasColumnName("KeyValue")
                .IsRequired();

            Ignore(i => i.EntityId)
                .Ignore(i => i.EntityShortId);
        }
    }
}
