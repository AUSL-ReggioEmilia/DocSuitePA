using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Fascicles
{
    public class FascicleDocumentUnitMap : EntityTypeConfiguration<FascicleDocumentUnit>
    {
        public FascicleDocumentUnitMap()
        {
            ToTable("FascicleDocumentUnits");
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdFascicleDocumentUnit")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.ReferenceType)
                .HasColumnName("ReferenceType")
                .IsRequired();
            
            Property(x => x.SequenceNumber)
               .HasColumnName("SequenceNumber")
               .IsRequired();

            Property(x => x.RegistrationUser)
               .HasColumnName("RegistrationUser")
               .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
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
            Property(x => x.SequenceNumber)
               .HasColumnName("SequenceNumber")
               .IsRequired();

            Ignore(x => x.EntityId)
               .Ignore(x => x.EntityShortId);

            #endregion
        }
    }
}
