using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Messages;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Messages
{
    public class MessageContactEmailMap : EntityTypeConfiguration<MessageContactEmail>
    {
        public MessageContactEmailMap()
            : base()
        {
            // Table
            ToTable("MessageContactEmail");

            // Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]
            Property(x => x.EntityId)
                .HasColumnName("IDMessageContactEmail")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.User)
                .HasColumnName("User")
                .IsRequired()
                .HasMaxLength(50);

            Property(x => x.Email)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(500);

            Property(x => x.Description)
                .HasColumnName("Description")
                .IsRequired()
                .HasMaxLength(500);

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .IsRequired();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.MessageContact)
               .WithMany(t => t.MessageContactEmail)
               .Map(k => k.MapKey("IDMessageContact"));

            #endregion
        }
    }
}
