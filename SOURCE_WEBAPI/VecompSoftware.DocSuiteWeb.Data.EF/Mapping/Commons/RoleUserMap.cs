using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class RoleUserMap : EntityTypeConfiguration<RoleUser>
    {
        public RoleUserMap()
            : base()
        {
            ToTable("RoleUser");
            //Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]            

            Property(x => x.EntityId)
                .HasColumnName("Incremental")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Type)
                .HasColumnName("Type")
                .IsOptional();

            Property(x => x.Description)
                .HasColumnName("Description")
                .IsOptional();

            Property(x => x.Account)
                .HasColumnName("Account")
                .IsOptional();

            Property(x => x.Enabled)
                .HasColumnName("Enabled")
                .IsOptional();

            Property(x => x.Email)
                .HasColumnName("Email")
                .IsOptional();

            Property(x => x.IsMainRole)
                .HasColumnName("IsMainRole")
                .IsOptional();

            Property(x => x.DSWEnvironment)
                .HasColumnName("DSWEnvironment")
                .IsRequired();

            Property(x => x.RegistrationDate)
                 .HasColumnName("RegistrationDate")
                 .IsRequired();

            Property(x => x.RegistrationUser)
                 .HasColumnName("RegistrationUser")
                 .IsOptional();

            Property(x => x.LastChangedDate)
                 .HasColumnName("LastChangedDate")
                 .IsOptional();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId);

            HasOptional(t => t.Role)
                .WithMany(t => t.RoleUsers)
                .Map(k => k.MapKey("idRole"));

            #endregion
        }
    }
}
