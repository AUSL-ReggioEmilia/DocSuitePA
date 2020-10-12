using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.PECMails
{
    public class PECMailMap : EntityTypeConfiguration<PECMail>
    {
        public PECMailMap()
            : base()
        {
            // Table
            ToTable("PECMail");
            // Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]

            Property(x => x.EntityId)
                .HasColumnName("IDPECMail")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Direction)
                .HasColumnName("Direction")
                .IsRequired();

            Property(x => x.Year)
                .HasColumnName("Year")
                .IsOptional();

            Property(x => x.Number)
                .HasColumnName("Number")
                .IsOptional();

            Property(x => x.MailUID)
                .HasColumnName("MailUID")
                .IsOptional();

            Property(x => x.MailContent)
                .HasColumnName("MailContent")
                .IsOptional();

            Property(x => x.MailSubject)
                .HasColumnName("MailSubject")
                .IsOptional();

            Property(x => x.MailSenders)
                .HasColumnName("MailSenders")
                .IsOptional();

            Property(x => x.MailRecipients)
                .HasColumnName("MailRecipients")
                .IsOptional();

            Property(x => x.MailDate)
                .HasColumnName("MailDate")
                .IsOptional();

            Property(x => x.MailType)
                .HasColumnName("MailType")
                .IsOptional();

            Property(x => x.MailError)
                .HasColumnName("MailError")
                .IsOptional();

            Property(x => x.MailPriority)
                .HasColumnName("MailPriority")
                .IsOptional();

            Property(x => x.XTrasporto)
                .HasColumnName("xTrasporto")
                .IsOptional();

            Property(x => x.MessageID)
                .HasColumnName("messageID")
                .IsOptional();

            Property(x => x.XRiferimentoMessageID)
                .HasColumnName("xRiferimentoMessageID")
                .IsOptional();

            Property(x => x.Segnatura)
                .HasColumnName("Segnatura")
                .IsOptional();

            Property(x => x.MessaggioRitornoName)
                .HasColumnName("MessaggioRitornoName")
                .IsOptional();

            Property(x => x.MessaggioRitornoStream)
                .HasColumnName("MessaggioRitornoStream")
                .IsOptional();

            Property(x => x.MailBody)
                .HasColumnName("MailBody")
                .IsOptional();

            Property(x => x.RecordedInDocSuite)
                .HasColumnName("RecordedInDocSuite")
                .IsOptional();

            Property(x => x.ContentLength)
                .HasColumnName("ContentLength")
                .IsOptional();

            Property(x => x.IsToForward)
                .HasColumnName("IsToForward")
                .IsRequired();

            Property(x => x.IsValidForInterop)
                .HasColumnName("IsValidForInterop")
                .IsRequired();

            Property(x => x.IsActive)
                .HasColumnName("IsActive")
                .IsRequired();

            Property(x => x.MailStatus)
                .HasColumnName("MailStatus")
                .IsOptional();

            Property(x => x.IsDestinated)
                .HasColumnName("IsDestinated")
                .IsOptional();

            Property(x => x.DestinationNote)
                .HasColumnName("DestinationNote")
                .IsOptional();

            Property(x => x.Handler)
                .HasColumnName("Handler")
                .IsOptional();

            Property(x => x.IDAttachments)
                .HasColumnName("IDAttachments")
                .IsOptional();

            Property(x => x.IDDaticert)
                .HasColumnName("IDDaticert")
                .IsOptional();

            Property(x => x.IDEnvelope)
                .HasColumnName("IDEnvelope")
                .IsOptional();

            Property(x => x.IDMailContent)
                .HasColumnName("IDMailContent")
                .IsOptional();

            Property(x => x.IDPostacert)
                .HasColumnName("IDPostacert")
                .IsOptional();

            Property(x => x.IDSegnatura)
                .HasColumnName("IDSegnatura")
                .IsOptional();

            Property(x => x.IDSmime)
                .HasColumnName("IDSmime")
                .IsOptional();

            Property(x => x.PECType)
                .HasColumnName("PECType")
                .IsOptional();

            Property(x => x.Checksum)
                .HasColumnName("Checksum")
                .IsOptional();

            Property(x => x.Multiple)
                .HasColumnName("Multiple")
                .IsRequired();

            Property(x => x.SplittedFrom)
                .HasColumnName("SplittedFrom")
                .IsOptional();

            Property(x => x.OriginalRecipient)
                .HasColumnName("OriginalRecipient")
                .IsOptional();

            Property(x => x.HeaderChecksum)
                .HasColumnName("HeaderChecksum")
                .IsOptional();

            Property(x => x.ProcessStatus)
                .HasColumnName("ProcessStatus")
                .IsOptional();

            Property(x => x.MailRecipientsCc)
                .HasColumnName("MailRecipientsCc")
                .IsOptional();

            Property(x => x.ReceivedAsCc)
                .HasColumnName("ReceivedAsCc")
                .IsOptional();

            Property(x => x.Size)
                .HasColumnName("Size")
                .IsOptional();

            Property(x => x.MultipleType)
                .HasColumnName("MultipleType")
                .IsOptional();

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

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .IsRequired();

            Property(x => x.InvoiceStatus)
                .HasColumnName("InvoiceStatus")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.WorkflowAutoComplete)
                .Ignore(x => x.WorkflowName)
                .Ignore(x => x.IdWorkflowActivity)
                .Ignore(x => x.WorkflowActions);
            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Location)
                .WithMany(t => t.PECMails)
                .Map(m => m.MapKey("IDLocation"));

            HasOptional(t => t.PECMailBox)
                .WithMany(t => t.PECMails)
                .Map(m => m.MapKey("IDPECMailBox"));

            HasOptional(t => t.DocumentUnit)
                .WithMany(t => t.PecMails)
                .Map(m => m.MapKey("IdDocumentUnit"));

            #endregion
        }
    }
}
