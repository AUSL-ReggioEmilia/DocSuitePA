using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class ContactTitleMap : EntityTypeConfiguration<ContactTitle>
    {
        public ContactTitleMap()
            : base()
        {
            ToTable("ContactTitle");
            //Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]

            Property(x => x.EntityId)
                .HasColumnName("IdTitle")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Description)
                .HasColumnName("Description")
                .IsOptional();

            Property(x => x.Code)
                .HasColumnName("Code")
                .IsRequired();

            Property(x => x.isActive)
                .HasColumnName("isActive")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsOptional();


            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();


            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();


            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .IsRequired();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]
            #endregion
        }
    }
}
