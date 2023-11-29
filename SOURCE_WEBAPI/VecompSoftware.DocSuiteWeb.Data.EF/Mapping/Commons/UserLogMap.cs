using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class UserLogMap : EntityTypeConfiguration<UserLog>
    {
        public UserLogMap()
            : base()
        {
            ToTable("UserLog");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdUserLog")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.SystemUser)
                .HasColumnName("SystemUser")
                .IsOptional();

            Property(x => x.SystemServer)
                .HasColumnName("SystemServer")
                .IsOptional();

            Property(x => x.SystemComputer)
                .HasColumnName("SystemComputer")
                .IsOptional();

            Property(x => x.AccessNumber)
                .HasColumnName("AccessNumber")
                .IsOptional();

            Property(x => x.PrevOperationDate)
                .HasColumnName("PrevOperationDate")
                .IsOptional();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.SessionId)
                .HasColumnName("SessionId")
                .IsOptional();

            Property(x => x.AdvancedScanner)
                .HasColumnName("AdvancedScanner")
                .IsOptional();

            Property(x => x.AdvancedViewer)
                .HasColumnName("AdvancedViewer")
                .IsOptional();

            Property(x => x.UserMail)
                .HasColumnName("UserMail")
                .IsOptional();

            Property(x => x.MobilePhone)
                .HasColumnName("MobilePhone")
                .IsOptional();


            Property(x => x.DefaultAdaptiveSearchControls)
                .HasColumnName("DefaultAdaptiveSearchControls")
                .IsOptional();

            Property(x => x.AdaptiveSearchStatistics)
                .HasColumnName("AdaptiveSearchStatistics")
                .IsOptional();

            Property(x => x.AdaptiveSearchEvaluated)
                .HasColumnName("AdaptiveSearchEvaluated")
                .IsOptional();

            Property(x => x.PrivacyLevel)
                .HasColumnName("PrivacyLevel")
                .IsRequired();

            Property(x => x.CurrentTenantId)
               .HasColumnName("CurrentTenantId")
               .IsRequired();

            Property(x => x.UserProfile)
               .HasColumnName("UserProfile")
               .IsOptional();

            Property(x => x.UserPrincipalName)
               .HasColumnName("UserPrincipalName")
               .IsOptional();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);

            #endregion

            #region [ Configure Navigation Properties ]
            #endregion
        }
    }
}
