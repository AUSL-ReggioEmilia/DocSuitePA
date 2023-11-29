using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Parameters;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Parameters
{
    public class ParameterMap : EntityTypeConfiguration<Parameter>
    {
        public ParameterMap()
            : base()
        {
            // Table
            ToTable("Parameter");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Incremental)
                .HasColumnName("Incremental")
                .IsRequired();

            Property(x => x.LastUsedYear)
                .HasColumnName("LastUsedYear")
                .IsRequired();

            Property(x => x.LastUsedNumber)
                .HasColumnName("LastUsedNumber")
                .IsRequired();

            Property(x => x.Locked)
                .HasColumnName("Locked")
                .IsRequired();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.LastUsedIdCategory)
                .HasColumnName("LastUsedidCategory")
                .IsRequired();

            Property(x => x.LastUsedIdRole)
                .HasColumnName("LastUsedidRole")
                .IsRequired();

            Property(x => x.LastUsedIdRoleUser)
                .HasColumnName("LastUsedIdRoleUser")
                .IsOptional();

            Property(x => x.LastUsedIdResolution)
                .HasColumnName("LastUsedidResolution")
                .IsOptional();

            Property(x => x.LastUsedResolutionYear)
                .HasColumnName("LastUsedResolutionYear")
                .IsOptional();

            Property(x => x.LastUsedResolutionNumber)
                .HasColumnName("LastUsedResolutionNumber")
                .IsOptional();

            Property(x => x.LastUsedBillNumber)
                .HasColumnName("LastUsedBillNumber")
                .IsOptional();

            Property(x => x.LastUsedResolutionNumber)
                .HasColumnName("LastUsedResolutionNumber")
                .IsOptional();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);

            MapToStoredProcedures();
            #endregion

            #region [ Configure navigation properties ]
            HasRequired(t => t.TenantAOO)
               .WithMany(t => t.Parameters)
               .Map(m => m.MapKey("IdTenantAOO"));
            #endregion
        }
    }
}
