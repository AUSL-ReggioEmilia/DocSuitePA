using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Messages;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Messages
{
    public class MessageContactMap : EntityTypeConfiguration<MessageContact>
    {
        public MessageContactMap()
            : base()
        {
            // Table
            ToTable("MessageContact");
            //Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]
            Property(x => x.EntityId)
                .HasColumnName("IDMessageContact")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.ContactType)
                .HasColumnName("ContactType")
                .IsRequired();

            Property(x => x.ContactPosition)
                .HasColumnName("ContactPosition")
                .IsRequired();

            Property(x => x.Description)
                .HasColumnName("Description")
                .IsRequired()
                .HasMaxLength(250);


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
                .WithMany(t => t.MessageContacts)
                .Map(k => k.MapKey("IDMessage"));

            #endregion
        }
    }
}
