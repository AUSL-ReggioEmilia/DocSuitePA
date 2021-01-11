using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class SecurityUserMap : EntityTypeConfiguration<SecurityUser>
    {
        public SecurityUserMap()
            : base()
        {
            // Table
            ToTable("SecurityUsers");
            // Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]

            Property(x => x.EntityId)
                .HasColumnName("idUser")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Account)
                .HasColumnName("Account")
                .IsOptional();

            Property(x => x.Description)
                .HasColumnName("Description")
                .IsOptional();

            Property(x => x.UserDomain)
                .HasColumnName("UserDomain")
                .IsOptional()
                .HasMaxLength(256);

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsOptional()
                .HasMaxLength(256);

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional()
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


            #endregion
        }
    }
}
