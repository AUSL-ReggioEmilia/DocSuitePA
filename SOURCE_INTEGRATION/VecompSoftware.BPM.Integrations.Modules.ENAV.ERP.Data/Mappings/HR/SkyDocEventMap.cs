using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities.HR;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Mappings.HR
{
    public class SkyDocEventMap : EntityTypeConfiguration<SkyDocEvent>
    {
        public SkyDocEventMap()
        {
            //Table
            ToTable("SKYDOC_EVENTS");
            // Primary Key
            HasKey(t => t.Id);

            #region [ Configure Properties ]
            Property(x => x.Id)
                .HasColumnName("IdEvent")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(p => p.Year)
                .HasColumnName("Anno")
                .IsRequired();

            Property(p => p.Number)
                .HasColumnName("Numero")
                .HasMaxLength(256)
                .IsRequired();

            Property(p => p.EntityUniqueId)
                .HasColumnName("IdentificativoUnivoco")
                .IsRequired();

            Property(p => p.CategoryName)
                .HasColumnName("Classificazione")
                .HasMaxLength(2000)
                .IsOptional();

            Property(p => p.Subject)
                .HasColumnName("Oggetto")
                .HasMaxLength(4000)
                .IsRequired();

            Property(p => p.Date)
                .HasColumnName("Data")
                .IsRequired();

            Property(p => p.WFEHCStarted)
                .HasColumnName("WF_EHCStarted")
                .IsOptional();

            Property(p => p.WFEHCStatus)
                .HasColumnName("WF_EHCStatus")
                .HasColumnType("smallint")
                .IsOptional();

            Property(x => x.EventType)
              .HasColumnName("Discriminator")
              .IsRequired();
            #endregion

            #region [ Navigation Properties ]
            HasOptional(p => p.Command)
                .WithMany(c => c.Events)
                .Map(p => p.MapKey("IdCommand"));
            #endregion
        }
    }
}
