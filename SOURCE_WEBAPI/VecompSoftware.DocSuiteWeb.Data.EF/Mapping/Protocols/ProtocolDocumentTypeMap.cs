using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Protocols
{
    public class ProtocolDocumentTypeMap : EntityTypeConfiguration<ProtocolDocumentType>
    {
        public ProtocolDocumentTypeMap()
            : base()
        {
            ToTable("TableDocType");
            //Primary Key
            HasKey(t => t.EntityShortId);

            #region [ Configure Properties ]

            Property(x => x.EntityShortId)
                .HasColumnName("idDocType")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Code)
                .HasColumnName("Code")
                .IsOptional();

            Property(x => x.Description)
                .HasColumnName("Description")
                .IsOptional();

            Property(x => x.IsActive)
                .HasColumnName("isActive")
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

            Property(x => x.HiddenFields)
                .HasColumnName("HiddenFields")
                .IsOptional();

            Property(x => x.CommonUser)
                .HasColumnName("CommonUser")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.UniqueId)
                .Ignore(x => x.EntityId);

            #endregion
        }
    }
}
