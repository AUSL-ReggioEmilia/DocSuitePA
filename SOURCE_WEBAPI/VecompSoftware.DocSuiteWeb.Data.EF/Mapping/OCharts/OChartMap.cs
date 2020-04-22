using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.OCharts
{
    public class OChartMap : EntityTypeConfiguration<OChart>
    {
        public OChartMap()
            : base()
        {
            // Table
            ToTable("OChart");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Enabled)
                .HasColumnName("Enabled")
                .IsOptional();

            Property(x => x.StartDate)
                .HasColumnName("StartDate")
                .IsOptional();

            Property(x => x.EndDate)
               .HasColumnName("EndDate")
               .IsOptional();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsOptional();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.Title)
                .HasColumnName("Title")
                .IsOptional();

            Property(x => x.Description)
                .HasColumnName("Description")
                .IsOptional();

            Property(x => x.Imported)
                .HasColumnName("Imported")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.EntityId);
            #endregion

            #region [ Configure Navigation Properties ]
            #endregion
        }
    }
}
