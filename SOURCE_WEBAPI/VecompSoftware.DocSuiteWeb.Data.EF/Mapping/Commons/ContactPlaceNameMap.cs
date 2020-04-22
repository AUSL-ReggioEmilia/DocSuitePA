using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class ContactPlaceNameMap : EntityTypeConfiguration<ContactPlaceName>
    {
        public ContactPlaceNameMap()
            : base()
        {
            ToTable("ContactPlaceName");
            //Primary Key
            HasKey(t => t.EntityShortId);

            #region [ Configure Properties ]

            Property(x => x.EntityShortId)
                .HasColumnName("idPlaceName")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Description)
                .HasColumnName("Description")
                .IsOptional();

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .IsRequired();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.RegistrationDate)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.EntityId);
            #endregion

            #region [ Configure Navigation Properties ]

            #endregion
        }
    }
}
