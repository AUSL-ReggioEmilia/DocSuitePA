using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Fascicles
{
    public class FasciclePeriodMap : EntityTypeConfiguration<FasciclePeriod>
    {
        public FasciclePeriodMap()
            : base()
        {
            ToTable("FasciclePeriods");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdFasciclePeriod")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.PeriodName)
                .HasColumnName("PeriodName")
                .IsRequired();

            Property(x => x.PeriodDays)
                .HasColumnName("PeriodDays")
                .IsRequired();

            Property(x => x.IsActive)
                .HasColumnName("IsActive")
                .IsRequired();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.RegistrationDate)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser);

            #endregion

            #region [ Configure Navigation Properties ]

            #endregion
        }
    }
}
