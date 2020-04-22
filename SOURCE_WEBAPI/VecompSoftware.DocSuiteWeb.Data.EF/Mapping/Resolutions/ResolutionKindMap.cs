using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Resolutions
{
    public class ResolutionKindMap : EntityTypeConfiguration<ResolutionKind>
    {
        public ResolutionKindMap()
            : base()
        {
            ToTable("ResolutionKinds");
            HasKey(t => t.UniqueId);

            #region [ Confifure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdResolutionKind")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            Property(x => x.IsActive)
                .HasColumnName("IsActive")
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

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);

            #endregion

            #region [ Configure Navigation Properties ]

            #endregion
        }
    }
}
