using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class ContactMap : EntityTypeConfiguration<Contact>
    {
        public ContactMap()
            : base()
        {
            ToTable("Contact");
            //Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]

            Property(x => x.EntityId)
                .HasColumnName("Incremental");

            Property(x => x.IdContactType)
                .HasColumnName("IdContactType")
                .IsRequired();

            Property(x => x.IncrementalFather)
                .HasColumnName("IncrementalFather")
                .IsOptional();

            Property(x => x.Description)
                .HasColumnName("Description")
                .IsOptional();

            Property(x => x.BirthDate)
                .HasColumnName("BirthDate")
                .IsOptional();

            Property(x => x.BirthPlace)
                .HasColumnName("BirthPlace")
                .IsOptional();

            Property(x => x.Code)
                .HasColumnName("Code")
                .IsOptional();

            Property(x => x.SearchCode)
                .HasColumnName("SearchCode")
                .IsOptional();

            Property(x => x.FiscalCode)
                .HasColumnName("FiscalCode")
                .IsOptional();

            Property(x => x.Address)
                .HasColumnName("Address")
                .IsOptional();

            Property(x => x.CivicNumber)
                .HasColumnName("CivicNumber")
                .IsOptional();

            Property(x => x.ZipCode)
                .HasColumnName("ZipCode")
                .IsOptional();

            Property(x => x.City)
                .HasColumnName("City")
                .IsOptional();

            Property(x => x.CityCode)
                .HasColumnName("CityCode")
                .IsOptional();

            Property(x => x.TelephoneNumber)
                .HasColumnName("TelephoneNumber")
                .IsOptional();

            Property(x => x.FaxNumber)
                .HasColumnName("FaxNumber")
                .IsOptional();

            Property(x => x.EmailAddress)
                .HasColumnName("EMailAddress")
                .IsOptional();

            Property(x => x.CertifiedMail)
                .HasColumnName("CertifydMail")
                .IsOptional();

            Property(x => x.Note)
                .HasColumnName("Note")
                .IsOptional();

            Property(x => x.IsActive)
                .HasColumnName("isActive")
                .IsRequired();

            Property(x => x.IsLocked)
                .HasColumnName("isLocked")
                .IsOptional();

            Property(x => x.IsNotExpandable)
                .HasColumnName("isNotExpandable")
                .IsOptional();

            Property(x => x.FullIncrementalPath)
                .HasColumnName("FullIncrementalPath")
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

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Property(x => x.ActiveFrom)
             .HasColumnName("ActiveFrom")
             .IsOptional();

            Property(x => x.ActiveTo)
             .HasColumnName("ActiveTo")
             .IsOptional();

            Property(x => x.IsChanged)
             .HasColumnName("isChanged")
             .IsOptional();

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .IsRequired();

            Property(x => x.Nationality)
                .HasColumnName("Nationality")
                .IsOptional();

            Property(x => x.Language)
                .HasColumnName("Language")
                .IsOptional();

            Property(x => x.SDIIdentification)
                .HasColumnName("SDIIdentification")
                .IsOptional();

            Ignore(x => x.EntityShortId);

            MapToStoredProcedures();

            #endregion

            #region [ Configure Navigation Properties ]
            HasOptional(t => t.PlaceName)
                .WithMany(t => t.Contacts)
                .Map(k => k.MapKey("IdPlaceName"));

            HasOptional(t => t.Title)
                .WithMany(t => t.Contacts)
                .Map(k => k.MapKey("IdTitle"));

            HasOptional(t => t.Role)
                .WithMany(t => t.Contacts)
                .Map(k => k.MapKey("idRole"));

            HasOptional(t => t.RoleRootContact)
                .WithMany(t => t.RootContacts)
                .Map(k => k.MapKey("idRoleRootContact"));

            #endregion
        }
    }
}
