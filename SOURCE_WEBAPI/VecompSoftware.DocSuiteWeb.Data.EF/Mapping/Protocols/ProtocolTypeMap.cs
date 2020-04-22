using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Protocols
{
    public class ProtocolTypeMap : EntityTypeConfiguration<ProtocolType>
    {
        public ProtocolTypeMap()
            : base()
        {
            ToTable("Type");
            //Primary Key
            HasKey(t => t.EntityShortId);

            #region [ Configure Properties ]

            Property(x => x.EntityShortId)
                .HasColumnName("idType")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Description)
                .HasColumnName("Description")
                .IsRequired();

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .IsRequired();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.RegistrationDate)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.EntityId);

            #endregion
        }
    }
}
