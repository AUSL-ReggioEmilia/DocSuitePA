using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.OCharts
{
    public class OChartItemContainerMap : EntityTypeConfiguration<OChartItemContainer>
    {
        public OChartItemContainerMap()
            : base()
        {
            // Table
            ToTable("OChartItemContainer");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);


            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.Master)
                .HasColumnName("Master")
                .IsOptional();

            Property(x => x.Rejection)
                .HasColumnName("Rejection")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.EntityId);
            #endregion

            #region [ Configure Navigation Properties ]

            HasOptional(t => t.OChartItem)
                .WithMany(t => t.OChartItemContainers)
                .Map(m => m.MapKey("IdOChartItem"));

            HasOptional(t => t.Container)
                .WithMany(t => t.OChartItemContainers)
                .Map(m => m.MapKey("IdContainer"));


            #endregion
        }
    }
}
