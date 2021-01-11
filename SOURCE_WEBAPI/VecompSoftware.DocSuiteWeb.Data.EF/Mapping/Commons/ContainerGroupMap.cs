using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class ContainerGroupMap : EntityTypeConfiguration<ContainerGroup>
    {
        public ContainerGroupMap()
            : base()
        {
            ToTable("ContainerGroup");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdContainerGroup")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.GroupName)
                .HasColumnName("GroupName")
                .IsRequired();

            Property(x => x.ProtocolRights)
                .HasColumnName("Rights")
                .IsRequired();

            Property(x => x.ResolutionRights)
                .HasColumnName("ResolutionRights")
                .IsOptional();

            Property(x => x.DocumentRights)
                .HasColumnName("DocumentRights")
                .IsOptional();

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

            Property(x => x.DeskRights)
                .HasColumnName("DeskRights")
                .IsOptional();

            Property(x => x.UDSRights)
                .HasColumnName("UDSRights")
                .IsOptional();

            Property(x => x.DocumentSeriesRights)
               .HasColumnName("DocumentSeriesRights")
               .IsOptional();

            Property(x => x.PrivacyLevel)
              .HasColumnName("PrivacyLevel")
              .IsRequired();

            Property(x => x.FascicleRights)
              .HasColumnName("FascicleRights")
              .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Container)
                .WithMany(t => t.ContainerGroups)
                .Map(m => m.MapKey("idContainer"));

            HasOptional(t => t.SecurityGroup)
                .WithMany(t => t.ContainerGroups)
                .Map(m => m.MapKey("idGroup"));

            #endregion
        }
    }
}
