using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Desks
{
    public class DeskRoleUsersMap : EntityTypeConfiguration<DeskRoleUser>
    {
        public DeskRoleUsersMap()
        {

            ToTable("DeskRoleUsers");

            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdDeskRoleUser")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.AccountName)
                .HasColumnName("AccountName")
                .IsOptional();

            Property(x => x.PermissionType)
                .HasColumnName("PermissionType")
                .IsRequired();

            Property(t => t.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityId)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]
            HasRequired(t => t.Desk)
                .WithMany(t => t.DeskRoleUsers)
                .Map(m => m.MapKey("IdDesk"));
            #endregion
        }
    }
}
