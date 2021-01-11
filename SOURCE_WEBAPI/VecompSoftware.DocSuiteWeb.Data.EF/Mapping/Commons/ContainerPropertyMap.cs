using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class ContainerPropertyMap : EntityTypeConfiguration<ContainerProperty>
    {
        public ContainerPropertyMap()
            : base()
        {
            ToTable("ContainerProperties");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdContainerProperty")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            Property(x => x.ContainerType)
                .HasColumnName("ContainerType")
                .IsRequired();

            Property(x => x.ValueInt)
                .HasColumnName("ValueInt")
                .IsOptional();

            Property(x => x.ValueDate)
                .HasColumnName("ValueDate")
                .IsOptional();

            Property(x => x.ValueDouble)
                .HasColumnName("ValueDouble")
                .IsOptional();

            Property(x => x.ValueBoolean)
                .HasColumnName("ValueBoolean")
                .IsOptional();

            Property(x => x.ValueGuid)
                .HasColumnName("ValueGuid")
                .IsOptional();

            Property(x => x.ValueString)
                .HasColumnName("ValueString")
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

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);

            MapToStoredProcedures();

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Container)
                .WithMany(t => t.ContainerProperties)
                .Map(m => m.MapKey("idContainer"));

            #endregion
        }
    }
}
