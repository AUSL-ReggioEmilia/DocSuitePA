using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.OCharts
{
    public class OChartItemMap : EntityTypeConfiguration<OChartItem>
    {
        public OChartItemMap()
            : base()
        {
            // Table
            ToTable("OChartItem");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Enabled)
                .HasColumnName("Enabled")
                .IsOptional();

            Property(x => x.Code)
                .HasColumnName("Code")
                .IsOptional();

            Property(x => x.FullCode)
               .HasColumnName("FullCode")
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

            Property(x => x.Email)
                .HasColumnName("Email")
                .IsOptional();

            Property(x => x.Acronym)
                .HasColumnName("Acronym")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.EntityId);
            #endregion

            #region [ Configure Navigation Properties ]

            HasOptional(t => t.OChart)
                .WithMany(t => t.OChartItems)
                .Map(m => m.MapKey("IdOChart"));

            HasOptional(t => t.Parent)
                .WithMany(t => t.Children)
                .Map(m => m.MapKey("IdParent"));

            HasMany(t => t.Contacts)
                .WithMany(t => t.OChartItems)
                .Map(m =>
                {
                    m.ToTable("OChartItemContact");
                    m.MapLeftKey("IdOChartItem");
                    m.MapRightKey("IdContact");
                });

            HasMany(t => t.Roles)
                .WithMany(t => t.OChartItems)
                .Map(m =>
                {
                    m.ToTable("OChartItemRole");
                    m.MapLeftKey("IdOChartItem");
                    m.MapRightKey("IdRole");
                });

            HasMany(t => t.Mailboxes)
                .WithMany(t => t.OChartItems)
                .Map(m =>
                {
                    m.ToTable("OChartItemMailbox");
                    m.MapLeftKey("IdOChartItem");
                    m.MapRightKey("IDPECMailBox");
                });


            #endregion
        }
    }
}
