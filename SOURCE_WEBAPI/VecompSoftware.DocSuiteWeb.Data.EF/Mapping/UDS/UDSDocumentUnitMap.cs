using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.UDS
{
    public class UDSDocumentUnitMap : EntityTypeConfiguration<UDSDocumentUnit>
    {
        public UDSDocumentUnitMap() : base()
        {
            ToTable("UDSDocumentUnits", "uds");

            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdUDSDocumentUnit")
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
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsOptional();

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

            HasRequired(t => t.Repository)
                .WithMany(t => t.UDSDocumentUnits)
                .Map(m => m.MapKey("IdUDSRepository"));

            HasRequired(t => t.Relation)
                .WithMany(t => t.UDSDocumentUnits)
                .Map(m => m.MapKey("IdDocumentUnit"));

            #endregion
        }
    }
}
