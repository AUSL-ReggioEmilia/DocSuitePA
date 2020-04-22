using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Fascicles
{
    public class FascicleRoleMap : EntityTypeConfiguration<FascicleRole>
    {
        public FascicleRoleMap()
            : base()
        {
            ToTable("FascicleRoles");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdFascicleRole")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.AuthorizationRoleType)
                .HasColumnName("RoleAuthorizationType")
                .IsRequired();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Property(x => x.IsMaster)
                .HasColumnName("IsMaster")
                .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Fascicle)
                .WithMany(t => t.FascicleRoles)
                .Map(p => p.MapKey("IdFascicle"));

            HasRequired(t => t.Role)
                .WithMany(t => t.FascicleRoles)
                .Map(p => p.MapKey("IdRole"));

            #endregion
        }
    }
}
