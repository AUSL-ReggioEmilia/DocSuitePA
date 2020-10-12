using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.UDS
{
    public class UDSMessageMap : EntityTypeConfiguration<UDSMessage>
    {
        public UDSMessageMap()
            : base()
        {

            ToTable("UDSMessages", "uds");
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdUDSMessage")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.IdUDS)
                .HasColumnName("IdUDS")
                .IsRequired();

            Property(x => x.Environment)
                .HasColumnName("Environment")
                .IsRequired();

            Property(x => x.RelationType)
                .HasColumnName("RelationType")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired()
                .HasMaxLength(256);

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .HasMaxLength(256)
                .IsOptional();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Relation)
                .WithMany(t => t.UDSMessages)
                .Map(m => m.MapKey("IdMessage"));

            HasRequired(t => t.Repository)
                .WithMany(t => t.UDSMessages)
                .Map(m => m.MapKey("IdUDSRepository"));

            HasRequired(t => t.SourceUDS)
                .WithMany(t => t.UDSMessages)
                .HasForeignKey(f => f.IdUDS);

            #endregion

        }
    }
}
