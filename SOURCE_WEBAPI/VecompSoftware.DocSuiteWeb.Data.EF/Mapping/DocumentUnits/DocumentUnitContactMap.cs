using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.DocumentUnits
{
    public class DocumentUnitContactMap : EntityTypeConfiguration<DocumentUnitContact>
    {
        public DocumentUnitContactMap()
            : base()
        {
            ToTable("DocumentUnitContacts", "cqrs");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdDocumentUnitContact")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.ContactManual)
                .HasColumnName("ContactManual")
                .IsOptional();

            Property(x => x.ContactType)
                .HasColumnName("ContactType")
                .IsRequired();

            Property(x => x.ContactLabel)
                .HasColumnName("ContactLabel")
                .IsOptional();

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

            HasOptional(t => t.Contact)
                .WithMany(t => t.DocumentUnitContacts)
                .Map(m => m.MapKey("IDContact"));

            HasRequired(t => t.DocumentUnit)
                .WithMany(t => t.DocumentUnitContacts)
                .Map(m => m.MapKey("IdDocumentUnit"));

            #endregion
        }
    }
}
