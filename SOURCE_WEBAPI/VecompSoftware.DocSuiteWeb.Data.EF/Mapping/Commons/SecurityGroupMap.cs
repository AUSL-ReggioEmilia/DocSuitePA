using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class SecurityGroupMap : EntityTypeConfiguration<SecurityGroup>
    {
        public SecurityGroupMap()
            : base()
        {
            // Table
            ToTable("SecurityGroups");
            // Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]
            Property(x => x.EntityId)
                .HasColumnName("idGroup")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.GroupName)
                .HasColumnName("GroupName")
                .IsOptional()
                .HasMaxLength(256);

            Property(x => x.LogDescription)
                .HasColumnName("LogDescription")
                 .IsOptional();

            Property(x => x.IsAllUsers)
                .HasColumnName("AllUsers")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsOptional()
                .HasMaxLength(256);

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .HasMaxLength(256);

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .IsRequired();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]

            HasMany(t => t.SecurityUsers)
               .WithOptional(t => t.Group)
               .Map(m => m.MapKey("idGroup"));

            #endregion

        }
    }
}
