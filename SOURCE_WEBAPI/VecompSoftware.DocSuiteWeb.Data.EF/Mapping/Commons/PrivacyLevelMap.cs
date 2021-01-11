using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class PrivacyLevelMap : EntityTypeConfiguration<PrivacyLevel>
    {
        public PrivacyLevelMap()
            : base()
        {
            ToTable("PrivacyLevels");

            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdPrivacyLevel")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Level)
                .HasColumnName("Level")
                .IsRequired();

            Property(x => x.Description)
                .HasColumnName("Description")
                .IsRequired();

            Property(x => x.Colour)
                .HasColumnName("Colour")
                .IsOptional();

            Property(x => x.IsActive)
                .HasColumnName("IsActive")
                .IsRequired();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
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
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]         

            #endregion
        }
    }
}
