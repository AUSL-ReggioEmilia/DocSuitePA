using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Messages;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Messages
{
    public class MessageAttachmentMap : EntityTypeConfiguration<MessageAttachment>
    {
        public MessageAttachmentMap()
        {
            // Table
            ToTable("MessageAttachment");
            // Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]
            Property(x => x.EntityId)
                .HasColumnName("IDMessageAttachment")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Server)
                .HasColumnName("Server")
                .IsRequired()
                .HasMaxLength(255);

            Property(x => x.Archive)
                .HasColumnName("Archive")
                .IsRequired()
                .HasMaxLength(255);

            Property(x => x.ChainId)
                .HasColumnName("ChainId")
                .IsRequired();

            Property(x => x.DocumentEnum)
                .HasColumnName("DocumentEnum")
                .IsOptional();

            Property(x => x.Extension)
                .HasColumnName("Extension")
                .HasMaxLength(256)
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
                .WithMany(t => t.MessageAttachments)
                .Map(k => k.MapKey("IDMessage"));

            #endregion
        }
    }
}
