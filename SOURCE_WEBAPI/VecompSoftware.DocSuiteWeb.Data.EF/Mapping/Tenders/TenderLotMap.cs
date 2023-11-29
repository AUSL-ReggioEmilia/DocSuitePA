using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Tenders;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Tenders
{
    public class TenderLotMap : EntityTypeConfiguration<TenderLot>
    {
        public TenderLotMap()
            : base()
        {
            // Table
            ToTable("TenderLot");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.CIG)
                .HasColumnName("CIG")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired()
                .HasMaxLength(256);

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .HasMaxLength(256)
                .IsOptional();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.Timestamp);
            #endregion

            #region [ Configure Navigation Properties ]
            HasRequired(t => t.TenderHeader)
                .WithMany(t => t.TenderLots)
                .Map(m => m.MapKey("IdTenderHeader"));
            #endregion
        }
    }
}
