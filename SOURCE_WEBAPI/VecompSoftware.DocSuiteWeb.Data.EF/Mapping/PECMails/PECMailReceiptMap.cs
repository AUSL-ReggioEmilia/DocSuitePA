using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.PECMails
{
    public class PECMailReceiptMap : EntityTypeConfiguration<PECMailReceipt>
    {
        public PECMailReceiptMap()
            : base()
        {
            // Table
            ToTable("PECMailReceipt");
            // Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]

            Property(x => x.EntityId)
                .HasColumnName("ID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.ReceiptType)
                .HasColumnName("ReceiptType")
                .IsOptional();

            Property(x => x.ErrorShort)
                .HasColumnName("ErrorShort")
                .IsOptional();

            Property(x => x.ErrorDescription)
                .HasColumnName("ErrorDescription")
                .IsOptional();

            Property(x => x.DateZone)
                .HasColumnName("DateZone")
                .IsOptional();

            Property(x => x.ReceiptDate)
                .HasColumnName("ReceiptDate")
                .IsOptional();

            Property(x => x.Sender)
                .HasColumnName("Sender")
                .IsOptional();

            Property(x => x.ReceiverType)
                .HasColumnName("ReceiverType")
                .IsOptional();

            Property(x => x.Receiver)
                .HasColumnName("Receiver")
                .IsOptional();

            Property(x => x.Subject)
                .HasColumnName("Subject")
                .IsOptional();

            Property(x => x.Provider)
                .HasColumnName("Provider")
                .IsOptional();

            Property(x => x.Identification)
                .HasColumnName("Identification")
                .IsOptional();

            Property(x => x.MSGID)
                .HasColumnName("MSGID")
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
                .IsRequired();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]            

            HasOptional(t => t.PECMailParent)
                .WithMany(t => t.PECMailChildrenReceipts)
                .Map(m => m.MapKey("IDParent"));

            HasOptional(t => t.PECMail)
                .WithMany(t => t.PECMailReceipts)
                .Map(m => m.MapKey("IDPECMail"));

            #endregion
        }
    }
}
