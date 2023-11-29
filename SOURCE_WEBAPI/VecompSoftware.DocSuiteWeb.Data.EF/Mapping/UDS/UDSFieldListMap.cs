using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.UDS
{
    public class UDSFieldListMap : EntityTypeConfiguration<UDSFieldList>
    {
        public UDSFieldListMap()
            : base()
        {

            ToTable("UDSFieldLists", "uds");
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdUDSFieldList")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.FieldName)
                .HasColumnName("FieldName")
                .IsRequired();

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            Property(x => x.Environment)
                .HasColumnName("Environment")
                .IsRequired();

            Property(x => x.Status)
                .HasColumnName("Status")
                .IsRequired();

            Property(x => x.ParentInsertId)
                .HasColumnName("UDSFieldListInsertId")
                .IsOptional();

            Property(x => x.UDSFieldListPath)
                .HasColumnName("UDSFieldListPath")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
                .IsOptional();

            Property(x => x.UDSFieldListLevel)
                .HasColumnName("UDSFieldListLevel")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
                .IsOptional();

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

            MapToStoredProcedures();
            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Repository)
                .WithMany(t => t.UDSFieldLists)
                .Map(m => m.MapKey("IdUDSRepository"));
            #endregion

        }
    }
}
