using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class MailDTO : IMailDTO
    {
        #region [ Properties ]

        public string Id { get; set; }

        public bool RegisteredLetter { get; set; }

        public bool IncludeAttachments { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<DocumentDTO, IDocumentDTO>))]
        public IDocumentDTO[] PolAttachments { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<MailboxDTO, IMailboxDTO>))]
        public IMailboxDTO Mailbox { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<ContactDTO, IContactDTO>))]
        public IContactDTO Sender { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<ContactDTO, IContactDTO>))]
        public IContactDTO[] Recipients { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<ContactDTO, IContactDTO>))]
        public IContactDTO[] RecipientsCc { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<ContactDTO, IContactDTO>))]
        public IContactDTO[] RecipientsBcc { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<DocumentDTO, IDocumentDTO>))]
        public IDocumentDTO[] Attachments { get; set; }

        #endregion

        #region [ Methods ]

        public bool HasId()
        {
            return !string.IsNullOrWhiteSpace(this.Id);
        }

        public bool HasRecipients()
        {
            return this.Recipients != null && this.Recipients.Length > 0;
        }

        public bool HasRecipientsCc()
        {
            return this.RecipientsCc != null && this.RecipientsCc.Length > 0;
        }

        public bool HasRecipientsBcc()
        {
            return this.RecipientsBcc != null && this.RecipientsBcc.Length > 0;
        }

        public bool HasAnyRecipient()
        {
            return this.HasRecipients() || this.HasRecipientsCc() || this.HasRecipientsBcc();
        }

        public bool HasAttachments()
        {
            return this.Attachments != null && this.Attachments.Length > 0;
        }

        public MailDTO AddRecipient(IContactDTO dto)
        {
            var list = this.Recipients != null ? this.Recipients.ToList() : new List<IContactDTO>();
            list.Add(dto);
            this.Recipients = list.ToArray();
            return this;
        }

        public MailDTO AddRecipient(string emailAddress)
        {
            return this.AddRecipient(new ContactDTO(emailAddress));
        }

        public MailDTO AddRecipients(IEnumerable<IContactDTO> dtos)
        {
            dtos.ToList().ForEach(d => this.AddRecipient(d));
            return this;
        }

        public MailDTO AddRecipients(IEnumerable<string> emailAddresses)
        {
            emailAddresses.ToList().ForEach(a => this.AddRecipient(a));
            return this;
        }

        public MailDTO AddRecipientCc(IContactDTO dto)
        {
            var list = this.RecipientsCc != null ? this.RecipientsCc.ToList() : new List<IContactDTO>();
            list.Add(dto);
            this.RecipientsCc = list.ToArray();
            return this;
        }

        public MailDTO AddRecipientCc(string emailAddress)
        {
            return this.AddRecipientCc(new ContactDTO(emailAddress));
        }

        public MailDTO AddRecipientsCc(IEnumerable<IContactDTO> dtos)
        {
            dtos.ToList().ForEach(d => this.AddRecipientCc(d));
            return this;
        }

        public MailDTO AddRecipientsCc(IEnumerable<string> emailAddresses)
        {
            emailAddresses.ToList().ForEach(a => this.AddRecipientCc(a));
            return this;
        }

        public MailDTO AddRecipientBcc(IContactDTO dto)
        {
            var list = this.RecipientsBcc != null ? this.RecipientsBcc.ToList() : new List<IContactDTO>();
            list.Add(dto);
            this.RecipientsBcc = list.ToArray();
            return this;
        }

        public MailDTO AddRecipientBcc(string emailAddress)
        {
            return this.AddRecipientBcc(new ContactDTO(emailAddress));
        }

        public MailDTO AddRecipientsBcc(IEnumerable<IContactDTO> dtos)
        {
            dtos.ToList().ForEach(d => this.AddRecipientBcc(d));
            return this;
        }

        public MailDTO AddRecipientsBcc(IEnumerable<string> emailAddresses)
        {
            emailAddresses.ToList().ForEach(a => this.AddRecipientBcc(a));
            return this;
        }

        public MailDTO AddAttachment(IDocumentDTO dto)
        {
            var list = this.Attachments != null ? this.Attachments.ToList() : new List<IDocumentDTO>();
            list.Add(dto);
            this.Attachments = list.ToArray();
            return this;
        }

        public MailDTO AddAttachment(string fullName)
        {
            return this.AddAttachment(new DocumentDTO(fullName));
        }

        public MailDTO AddAttachment(string name, byte[] content)
        {
            return this.AddAttachment(new DocumentDTO(name, content));
        }

        public MailDTO AddAttachmentBytes(string fullName)
        {
            var content = File.ReadAllBytes(fullName);
            return this.AddAttachment(fullName, content);
        }

        public MailDTO AddAttachments(IEnumerable<IDocumentDTO> dtos)
        {
            dtos.ToList().ForEach(d => this.AddAttachment(d));
            return this;
        }

        public MailDTO AddAttachments(IEnumerable<string> fullNames)
        {
            fullNames.ToList().ForEach(n => this.AddAttachment(n));
            return this;
        }

        public MailDTO AddAttachmentsBytes(IEnumerable<string> fullNames)
        {
            fullNames.ToList().ForEach(n => this.AddAttachmentBytes(n));
            return this;
        }

        public IEnumerable<IContactDTO> GetAllRecipients()
        {
            if (!this.HasAnyRecipient())
                return null;

            var list = new List<IContactDTO>();
            if (this.HasRecipients())
                list.AddRange(this.Recipients);
            if (this.HasRecipientsCc())
                list.AddRange(this.RecipientsCc);
            if (this.HasRecipientsBcc())
                list.AddRange(this.RecipientsBcc);

            return list.GroupBy(r => r.EmailAddress).Select(g => g.First());
        }

        public string StringifyRecipients()
        {
            if (!this.HasRecipients())
                return null;

            return string.Join("; ", this.Recipients
                .Where(r => !string.IsNullOrEmpty(r.EmailAddress))
                .Select(r => r.EmailAddress.Trim()));
        }

        public string StringifyRecipientsCc()
        {
            if (!this.HasRecipientsCc())
                return null;

            return string.Join("; ", this.RecipientsCc
                .Where(r => !string.IsNullOrEmpty(r.EmailAddress))
                .Select(r => r.EmailAddress.Trim()));
        }

        public string StringifyRecipientsBcc()
        {
            if (!this.HasRecipientsBcc())
                return null;

            return string.Join("; ", this.RecipientsBcc
                .Where(r => !string.IsNullOrEmpty(r.EmailAddress))
                .Select(r => r.EmailAddress.Trim()));
        }

        public MailDTO CopyFrom(IProtocolDTO protocolDTO)
        {
            var protocol = (ProtocolDTO)protocolDTO;
            if (!protocol.HasAnyDocument())
                return this;

            if (protocol.Document != null)
                this.AddAttachment(protocol.Document);
            if (protocol.HasAttachments())
                this.AddAttachments(protocol.Attachments);
            if (protocol.HasAnnexes())
                this.AddAttachments(protocol.Annexes);
            return this;
        }

        #endregion
    }
}
