using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Protocols
{
    public class ProtocolUserMap : EntityTypeConfiguration<ProtocolUser>
    {
        public ProtocolUserMap()
            : base()
        {
            ToTable("ProtocolUsers");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(t => t.UniqueId)
                .HasColumnName("IdProtocolUser")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Year)
                .HasColumnName("Year")
                .IsRequired();

            Property(x => x.Number)
                .HasColumnName("Number")
                .IsRequired();

            Property(t => t.Account)
                .HasColumnName("Account")
                .IsRequired();

            Property(t => t.Type)
                .HasColumnName("Type")
                .IsRequired();

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

            Property(t => t.Note)
                .HasColumnName("Note")
                .IsOptional();

            Ignore(t => t.EntityId)
                .Ignore(t => t.EntityShortId);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Protocol)
                .WithMany(t => t.ProtocolUsers)
                .Map(m => m.MapKey("UniqueIdProtocol"));

            #endregion
        }
    }
}
