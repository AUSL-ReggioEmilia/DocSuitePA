using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.PECMails
{
    public class PECMailAttachmentMap : EntityTypeConfiguration<PECMailAttachment>
    {
        public PECMailAttachmentMap()
            : base()
        {
            // Table
            ToTable("PECMailAttachment");
            // Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]

            Property(x => x.EntityId)
                .HasColumnName("IDPECMailAttachment")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.AttachmentName)
                .HasColumnName("AttachmentName")
                .IsRequired();

            Property(x => x.IsMain)
                .HasColumnName("IsMain")
                .IsRequired();

            Property(x => x.IDDocument)
                .HasColumnName("IDDocument")
                .IsOptional();

            Property(x => x.Size)
                .HasColumnName("Size")
                .IsOptional();

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired()
                .HasMaxLength(256);

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .HasMaxLength(256)
                .IsOptional();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.PECMail)
                .WithMany(t => t.Attachments)
                .Map(m => m.MapKey("IDPECMail"));

            HasOptional(t => t.Parent)
                .WithOptionalDependent(t => t.Child)
                .Map(m => m.MapKey("IDParent"));

            #endregion


        }
    }
}
