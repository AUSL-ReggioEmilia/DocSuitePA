using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Messages;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Messages
{
    public class MessageEmailMap : EntityTypeConfiguration<MessageEmail>
    {
        public MessageEmailMap()
            : base()
        {
            // Table
            ToTable("MessageEmail");

            //Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]

            Property(x => x.EntityId)
               .HasColumnName("IDMessageEmail")
               .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.SentDate)
                .HasColumnName("SentDate")
                .IsOptional();

            Property(x => x.Subject)
                .HasColumnName("Subject")
                .IsRequired()
                .HasMaxLength(500);

            Property(x => x.Body)
                .HasColumnName("Body")
                .IsRequired();

            Property(x => x.Priority)
                .HasColumnName("Priority")
                .IsOptional()
                .HasMaxLength(256);

            Property(x => x.EmlDocumentId)
                .HasColumnName("EmlDocumentId")
                .IsOptional();

            Property(x => x.IsDispositionNotification)
                .HasColumnName("IsDispositionNotification")
                .IsOptional();

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .IsRequired();

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

            Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Message)
                .WithMany(t => t.MessageEmails)
                .Map(k => k.MapKey("IDMessage"));

            #endregion
        }
    }
}
