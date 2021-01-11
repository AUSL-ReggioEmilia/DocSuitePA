namespace VecompSoftware.DocSuiteWeb.API
{
    public interface IMailDTO : IAPIArgument
    {
        #region [ Properties ]

        string Id { get; set; }

        IMailboxDTO Mailbox { get; set; }

        bool RegisteredLetter { get; set; }

        bool IncludeAttachments { get; set; }

        IDocumentDTO[] PolAttachments { get; set; }

        IContactDTO Sender { get; set; }

        IContactDTO[] Recipients { get; set; }

        IContactDTO[] RecipientsCc { get; set; }

        IContactDTO[] RecipientsBcc { get; set; }

        string Subject { get; set; }

        string Body { get; set; }

        IDocumentDTO[] Attachments { get; set; }

        #endregion
    }
}
