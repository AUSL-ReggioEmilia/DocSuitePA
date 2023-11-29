using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class RoleMap : EntityTypeConfiguration<Role>
    {
        public RoleMap()
            : base()
        {
            ToTable("Role");
            //Primary Key
            HasKey(t => t.EntityShortId);

            #region [ Configure Properties ]

            Property(x => x.EntityShortId)
                .HasColumnName("idRole")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsOptional();

            Property(x => x.IsActive)
                .HasColumnName("isActive")
                .IsRequired();

            Property(x => x.FullIncrementalPath)
                .HasColumnName("FullIncrementalPath")
                .IsOptional();

            Property(x => x.Collapsed)
                .HasColumnName("Collapsed")
                .IsOptional();

            Property(x => x.EMailAddress)
                .HasColumnName("EMailAddress")
                .IsOptional();

            Property(x => x.ServiceCode)
                .HasColumnName("ServiceCode")
                .IsOptional();

            Property(x => x.UniqueId)
              .HasColumnName("UniqueId")
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

            Property(x => x.RoleTypology)
                .HasColumnName("RoleTypology")
                .IsRequired();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.WorkflowAutoComplete)
                .Ignore(x => x.WorkflowName)
                .Ignore(x => x.IdWorkflowActivity)
                .Ignore(x => x.WorkflowActions);

            MapToStoredProcedures();
            #endregion

            #region [ Configure Navigation Properties ]
            HasOptional(x => x.Father)
               .WithMany()
               .Map(x => x.MapKey("idRoleFather"));

            HasRequired(x => x.TenantAOO)
                .WithMany(x => x.Roles)
                    .Map(m => m.MapKey("IdTenantAOO"));
            #endregion
        }
    }
}
