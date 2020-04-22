using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.PECMails
{
    public class PECMailBoxMap : EntityTypeConfiguration<PECMailBox>
    {
        public PECMailBoxMap()
            : base()
        {
            // Table
            ToTable("PECMailBox");
            // Primary Key
            HasKey(t => t.EntityShortId);

            #region [ Configure Properties ]

            Property(x => x.EntityShortId)
                .HasColumnName("IDPECMailBox")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.MailBoxRecipient)
                .HasColumnName("MailBoxRecipient")
                .IsRequired();

            Property(x => x.IncomingServer)
                .HasColumnName("IncomingServer")
                .IsOptional();

            Property(x => x.IncomingServerProtocol)
                .HasColumnName("IncomingServerProtocol")
                .IsOptional();

            Property(x => x.IncomingServerPort)
                .HasColumnName("IncomingServerPort")
                .IsOptional();

            Property(x => x.IncomingServerUseSsl)
                .HasColumnName("IncomingServerUseSsl")
                .IsOptional();

            Property(x => x.OutgoingServer)
                .HasColumnName("OutgoingServer")
                .IsOptional();

            Property(x => x.OutgoingServerPort)
                .HasColumnName("OutgoingServerPort")
                .IsOptional();

            Property(x => x.OutgoingServerUseSsl)
                .HasColumnName("OutgoingServerUseSsl")
                .IsOptional();

            Property(x => x.Username)
                .HasColumnName("Username")
                .IsRequired();

            Property(x => x.Password)
                .HasColumnName("Password")
                .IsRequired();

            Property(x => x.Managed)
                .HasColumnName("Managed")
                .IsOptional();

            Property(x => x.Unmanaged)
                .HasColumnName("Unmanaged")
                .IsOptional();

            Property(x => x.IsDestinationEnabled)
                .HasColumnName("IsDestinationEnabled")
                .IsOptional();

            Property(x => x.IsForInterop)
                .HasColumnName("IsForInterop")
                .IsRequired();

            Property(x => x.IdConfiguration)
                .HasColumnName("IdConfiguration")
                .IsOptional();

            Property(x => x.DeleteMailFromServer)
                .HasColumnName("DeleteMailFromServer")
                .IsOptional();

            Property(x => x.ReceiveDaysCap)
                .HasColumnName("ReceiveDaysCap")
                .IsOptional();

            Property(x => x.RedirectAnomaliesSMTP)
                .HasColumnName("RedirectAnomaliesSMTP")
                .IsOptional();

            Property(x => x.RedirectAnomaliesUsername)
                .HasColumnName("RedirectAnomaliesUsername")
                .IsOptional();

            Property(x => x.RedirectAnomaliesPassword)
                .HasColumnName("RedirectAnomaliesPassword")
                .IsOptional();

            Property(x => x.RedirectAnomaliesRecipient)
                .HasColumnName("RedirectAnomaliesRecipient")
                .IsOptional();

            Property(x => x.RedirectStorageUsername)
                .HasColumnName("RedirectStorageUsername")
                .IsOptional();

            Property(x => x.RedirectStoragePassword)
                .HasColumnName("RedirectStoragePassword")
                .IsOptional();

            Property(x => x.RedirectStorageRecipient)
                .HasColumnName("RedirectStorageRecipient")
                .IsOptional();

            Property(x => x.IsHandleEnabled)
                .HasColumnName("IsHandleEnabled")
                .IsOptional();

            Property(x => x.IsProtocolBox)
                .HasColumnName("IsProtocolBox")
                .IsOptional();

            Property(x => x.IsProtocolBoxExplicit)
                .HasColumnName("IsProtocolBoxExplicit")
                .IsOptional();

            Property(x => x.IdJeepServiceIncomingHost)
                .HasColumnName("IdJeepServiceIncomingHost")
                .IsOptional();

            Property(x => x.IdJeepServiceOutgoingHost)
                .HasColumnName("IdJeepServiceOutgoingHost")
                .IsOptional();

            Property(x => x.RulesetDefinition)
                .HasColumnName("RulesetDefinition")
                .IsOptional();

            Property(x => x.InvoiceType)
                .HasColumnName("InvoiceType")
                .IsOptional();

            Property(x => x.HumanEnabled)
                .HasColumnName("HumanEnabled")
                .IsRequired();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityId)
                .Ignore(x => x.RegistrationDate)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser);
            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Location)
                .WithMany(t => t.PECMailBoxes)
                .Map(m => m.MapKey("idLocation"));


            #endregion
        }
    }
}
