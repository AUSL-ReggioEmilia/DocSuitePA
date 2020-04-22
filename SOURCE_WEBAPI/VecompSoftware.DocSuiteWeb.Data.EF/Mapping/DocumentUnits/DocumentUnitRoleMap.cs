using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.DocumentUnits
{
    public class DocumentUnitRoleMap : EntityTypeConfiguration<DocumentUnitRole>
    {
        public DocumentUnitRoleMap()
            : base()
        {
            ToTable("DocumentUnitRoles", "cqrs");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdDocumentUnitRole")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.RoleLabel)
                .HasColumnName("RoleLabel")
                .IsOptional();

            Property(x => x.UniqueIdRole)
                .HasColumnName("UniqueIdRole")
                .IsRequired();

            Property(x => x.AssignUser)
                .HasColumnName("AssignUser")
                .IsOptional();

            Property(x => x.AuthorizationRoleType)
                .HasColumnName("RoleAuthorizationType")
                .IsRequired();

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

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityShortId)
                .Ignore(x => x.EntityId);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.DocumentUnit)
                .WithMany(t => t.DocumentUnitRoles)
                .Map(m => m.MapKey("IdDocumentUnit"));

            #endregion
        }
    }
}
