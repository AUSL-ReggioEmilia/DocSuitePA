using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Protocols
{
    public class ProtocolContactManualMap : EntityTypeConfiguration<ProtocolContactManual>
    {
        public ProtocolContactManualMap()
            : base()
        {
            ToTable("ProtocolContactManual");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(t => t.UniqueId)
                .HasColumnName("UniqueId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Year)
                .HasColumnName("Year")
                .IsRequired();

            Property(x => x.Number)
                .HasColumnName("Number")
                .IsRequired();
            
            Property(x => x.EntityId)
                .HasColumnName("Incremental")
                .IsRequired();

            Property(t => t.ComunicationType)
                .HasColumnName("ComunicationType")
                .IsRequired();

            Property(t => t.Description)
                .HasColumnName("Description")
                .IsOptional();

            Property(t => t.BirthDate)
                .HasColumnName("BirthDate")
                .IsOptional();

            Property(x => x.BirthPlace)
                .HasColumnName("BirthPlace")
                .IsOptional();

            Property(t => t.Code)
                .HasColumnName("Code")
                .IsOptional();

            Property(t => t.FiscalCode)
                .HasColumnName("FiscalCode")
                .IsOptional();

            Property(t => t.Address)
                .HasColumnName("Address")
                .IsOptional();

            Property(t => t.CivicNumber)
                .HasColumnName("CivicNumber")
                .IsOptional();

            Property(t => t.ZipCode)
                .HasColumnName("ZipCode")
                .IsOptional();

            Property(t => t.City)
                .HasColumnName("City")
                .IsOptional();

            Property(t => t.CityCode)
                .HasColumnName("CityCode")
                .IsOptional();

            Property(t => t.TelephoneNumber)
                .HasColumnName("TelephoneNumber")
                .IsOptional();

            Property(t => t.FaxNumber)
                .HasColumnName("FaxNumber")
                .IsOptional();

            Property(t => t.EMailAddress)
                .HasColumnName("EMailAddress")
                .IsOptional();

            Property(t => t.CertifydMail)
                .HasColumnName("CertifydMail")
                .IsOptional();

            Property(t => t.Note)
                .HasColumnName("Note")
                .IsOptional();

            Property(t => t.FullIncrementalPath)
                .HasColumnName("FullIncrementalPath")
                .IsOptional();

            Property(t => t.idAD)
                .HasColumnName("idAD")
                .IsOptional();

            Property(t => t.Type)
                .HasColumnName("Type")
                .IsOptional();

            Property(t => t.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(t => t.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(t => t.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(t => t.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(t => t.Timestamp)
               .HasColumnName("Timestamp")
               .IsRequired();

            Property(t => t.IdContactType)
                .HasColumnName("IdContactType")
                .IsRequired();

            Property(t => t.Nationality)
                .HasColumnName("Nationality")
                .IsOptional();

            Property(t => t.Language)
                .HasColumnName("Language")
                .IsOptional();

            Property(t => t.SDIIdentification)
                .HasColumnName("SDIIdentification")
                .IsOptional();

            Ignore(t => t.EntityShortId);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Protocol)
                .WithMany(t => t.ProtocolContactManuals)
                .Map(m => m.MapKey("UniqueIdProtocol"));

            HasOptional(t => t.ContactTitle)
                .WithMany(t => t.ProtocolContactManuals)
                .Map(m => m.MapKey("IdTitle"));

            HasOptional(t => t.ContactPlaceName)
                .WithMany(t => t.ProtocolContactManuals)
                .Map(m => m.MapKey("IdPlaceName"));
            #endregion
        }
    }
}
