using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.PECMails
{
    public class PECMailBoxConfigurationMap : EntityTypeConfiguration<PECMailBoxConfiguration>
    {
        public PECMailBoxConfigurationMap()
            : base()
        {
            // Table
            ToTable("PECMailBoxConfiguration");
            // Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]

            Property(x => x.EntityId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            Property(x => x.MaxReadForSession)
                .HasColumnName("MaxReadForSession")
                .IsOptional();

            Property(x => x.MaxSendForSession)
                .HasColumnName("MaxSendForSession")
                .IsOptional();

            Property(x => x.UnzipAttachments)
                .HasColumnName("UnzipAttachments")
                .IsOptional();

            Property(x => x.SslPort)
                .HasColumnName("SslPort")
                .IsOptional();

            Property(x => x.ImapEnabled)
                .HasColumnName("ImapEnabled")
                .IsOptional();

            Property(x => x.UseImapSsl)
                .HasColumnName("UseImapSsl")
                .IsOptional();

            Property(x => x.ImapPort)
                .HasColumnName("ImapPort")
                .IsOptional();

            Property(x => x.MarkAsRead)
                .HasColumnName("MarkAsRead")
                .IsOptional();

            Property(x => x.MoveToFolder)
                .HasColumnName("MoveToFolder")
                .IsOptional();

            Property(x => x.MoveErrorToFolder)
                .HasColumnName("MoveErrorToFolder")
                .IsOptional();

            Property(x => x.InboxFolder)
                .HasColumnName("InboxFolder")
                .IsOptional();

            Property(x => x.UploadSent)
                .HasColumnName("UploadSent")
                .IsOptional();

            Property(x => x.FolderSent)
                .HasColumnName("FolderSent")
                .IsOptional();

            Property(x => x.ImapSearchFlag)
                .HasColumnName("ImapSearchFlag")
                .IsOptional();

            Property(x => x.ImapStartDate)
                .HasColumnName("ImapStartDate")
                .IsOptional();

            Property(x => x.ImapEndDate)
                .HasColumnName("ImapEndDate")
                .IsOptional();

            Property(x => x.NoSubjectDefaultText)
                .HasColumnName("NoSubjectDefaultText")
                .IsOptional();

            Property(x => x.DeleteMailFromServer)
                .HasColumnName("DeleteMailFromServer")
                .IsOptional();

            Property(x => x.ReceiveDaysCap)
                .HasColumnName("ReceiveDaysCap")
                .IsOptional();

            Property(x => x.MaxReceiveByteSize)
                .HasColumnName("MaxReceiveByteSize")
                .IsOptional();

            Property(x => x.MaxSendByteSize)
                .HasColumnName("MaxSendByteSize")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.RegistrationDate)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.UniqueId);
            #endregion

            #region [ Configure Navigation Properties ]

            #endregion
        }
    }
}
