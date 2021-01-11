using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Protocols
{
    public class AdvancedProtocolMap : EntityTypeConfiguration<AdvancedProtocol>
    {
        public AdvancedProtocolMap()
        : base()
        {
            // Table
            ToTable("AdvancedProtocol");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Year)
                .HasColumnName("Year")
                .IsRequired();

            Property(x => x.Number)
                .HasColumnName("Number")
                .IsRequired();

            Property(x => x.ServiceCategory)
                .HasColumnName("ServiceCategory")
                .IsOptional();

            Property(x => x.Subject)
                .HasColumnName("Subject")
                .IsOptional();

            Property(x => x.ServiceField)
                .HasColumnName("ServiceField")
                .IsOptional();

            Property(x => x.Note)
                .HasColumnName("Note")
                .IsOptional();

            Property(x => x.Origin)
                .HasColumnName("Origin")
                .IsOptional();

            Property(x => x.Package)
                .HasColumnName("Package")
                .IsOptional();

            Property(x => x.Lot)
                .HasColumnName("Lot")
                .IsOptional();

            Property(x => x.Incremental)
                .HasColumnName("Incremental")
                .IsOptional();

            Property(x => x.InvoiceNumber)
                .HasColumnName("InvoiceNumber")
                .IsOptional();

            Property(x => x.InvoiceDate)
                .HasColumnName("InvoiceDate")
                .IsOptional();

            Property(x => x.InvoiceTotal)
                .HasColumnName("InvoiceTotal")
                .IsOptional();

            Property(x => x.AccountingSectional)
                .HasColumnName("AccountingSectional")
                .IsOptional();

            Property(x => x.AccountingYear)
                .HasColumnName("AccountingYear")
                .IsOptional();

            Property(x => x.AccountingDate)
                .HasColumnName("AccountingDate")
                .IsOptional();

            Property(x => x.AccountingNumber)
                .HasColumnName("AccountingNumber")
                .IsOptional();

            Property(x => x.IsClaim)
                .HasColumnName("IsClaim")
                .IsOptional();

            Property(x => x.ProtocolStatus)
                .HasColumnName("ProtocolStatus")
                .IsOptional();

            Property(x => x.IdentificationSdi)
                .HasColumnName("IdentificationSDI")
                .IsOptional();

            Property(x => x.AccountingSectionalNumber)
                .HasColumnName("AccountingSectionalNumber")
                .IsOptional();

            Property(x => x.InvoiceYear)
                .HasColumnName("InvoiceYear")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
                .IsOptional();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
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

            HasRequired(t => t.Protocol)
                .WithRequiredDependent(t => t.AdvancedProtocol)
                .Map(m => m.MapKey("UniqueIdProtocol"));

            #endregion

        }
    }
}
