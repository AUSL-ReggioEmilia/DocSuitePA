using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Mappings
{
    public class EventMap : EntityTypeConfiguration<Event>
    {
        public EventMap() : base()
        {
            //Table
            ToTable("XX_SKYRC_EVENTS", "APPS");
            // Primary Key
            HasKey(t => t.EventId);

            #region [ Configure Properties ]
            Property(x => x.EventId)
                .HasColumnName("EVENT_ID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.CorrelationId)
                .HasColumnName("CORRELATION_ID")
                .IsOptional();

            Property(x => x.Subject)
                .HasColumnName("SUBJECT")
                .IsOptional();

            Property(x => x.Verb)
                .HasColumnName("VERB")
                .IsOptional();

            Property(x => x.Year)
                .HasColumnName("YEAR")
                .IsOptional();

            Property(x => x.Number)
                .HasColumnName("SUBJECT_NUMBER")
                .IsOptional();

            Property(x => x.InsertTime)
                .HasColumnName("INSERTTIME")
                .IsRequired();

            Property(x => x.ProcessedTime)
                .HasColumnName("PROCESSED_TIME")
                .IsOptional();

            Property(x => x.DigitalSigners)
                .HasColumnName("FIRMA_DIGITALE")
                .IsOptional();
            #endregion
        }
    }
}
