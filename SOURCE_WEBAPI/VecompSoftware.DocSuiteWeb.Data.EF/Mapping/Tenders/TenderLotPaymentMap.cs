using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Tenders;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Tenders
{
    public class TenderLotPaymentMap : EntityTypeConfiguration<TenderLotPayment>
    {
        public TenderLotPaymentMap()
            : base()
        {
            // Table
            ToTable("TenderLotPayment");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.PaymentKey)
                .HasColumnName("PaymentKey")
                .IsRequired();

            Property(x => x.Amount)
                .HasColumnName("Amount")
                .IsOptional();

            Ignore(x => x.RegistrationDate)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.Timestamp);
            #endregion

            #region [ Configure Navigation Properties ]
            HasRequired(t => t.TenderLot)
                .WithMany(t => t.TenderLotPayments)
                .Map(m => m.MapKey("IdTenderLot"));
            #endregion
        }
    }
}
