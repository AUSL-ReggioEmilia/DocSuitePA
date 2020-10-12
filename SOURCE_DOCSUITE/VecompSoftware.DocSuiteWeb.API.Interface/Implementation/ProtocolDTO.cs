using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class ProtocolDTO : IProtocolDTO
    {
        #region [ Constructors ]

        public ProtocolDTO()
        {
        }

        public ProtocolDTO(short? year, int? number)
        {
            this.Year = year;
            this.Number = number;
        }

        #endregion

        #region [ Properties ]
        public Guid? UniqueId { get; set; }

        public short? Year { get; set; }

        public int? Number { get; set; }

        public int? Direction { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<DocumentDTO, IDocumentDTO>))]
        public IDocumentDTO Document { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<DocumentDTO, IDocumentDTO>))]
        public IDocumentDTO[] Attachments { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<DocumentDTO, IDocumentDTO>))]
        public IDocumentDTO[] Annexes { get; set; }

        public int? IdDocumentType { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<ContainerDTO, IContainerDTO>))]
        public IContainerDTO Container { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<ContainerDTO, IContainerDTO>))]
        public IContainerDTO LinkReferenceContainer { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<ContactDTO, IContactDTO>))]
        public IContactDTO[] Senders { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<ContactDTO, IContactDTO>))]
        public IContactDTO[] SendersManual { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<ContactDTO, IContactDTO>))]
        public IContactDTO[] Recipients { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<ContactDTO, IContactDTO>))]
        public IContactDTO[] RecipientsManual { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<ContactDTO, IContactDTO>))]
        public IContactDTO[] Fascicles { get; set; }

        public string Subject { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<CategoryDTO, ICategoryDTO>))]
        public ICategoryDTO Category { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<CategoryDTO, ICategoryDTO>))]
        public ICategoryDTO SubCategory { get; set; }

        public short? IdProtocolKind { get; set; }

        public string Note { get; set; }

        public char PackageOrigin { get; set; }

        public int? Package { get; set; }

        public int? PackageLot { get; set; }

        public int? PackageIncremental { get; set; }

        public string InvoiceNumber { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public Double? InvoiceTotal { get; set; }

        public int? AccountingSectionalNumber { get; set; }

        public short? AccountingYear { get; set; }

        public DateTime? AccountingDate { get; set; }

        public int? AccountingNumber { get; set; }

        public string IdentificationSDI { get; set; }

        public  string RegistrationUser { get; set; }

        public bool UseProtocolReserve { get; set; }

        public DateTime? ProtocolReserveFrom { get; set; }

        public DateTime? ProtocolReserveTo { get; set; }

        public Guid IdTenantAOO { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<WorkflowActionDTO, IWorkflowActionDTO>))]
        public IWorkflowActionDTO[] WorkflowActions { get; set; }

        public int? IdServiceCategory { get; set; }
        #endregion

        #region [ Methods ]

        public bool HasId()
        {
            return this.UniqueId.HasValue;
        }

        public bool HasAttachments()
        {
            return this.Attachments != null && this.Attachments.Length > 0;
        }

        public bool HasAnnexes()
        {
            return this.Annexes != null && this.Annexes.Length > 0;
        }

        public bool HasAnyDocument()
        {
            return this.Document != null || this.HasAttachments() || this.HasAnnexes();
        }

        public bool HasSenders()
        {
            return this.Senders != null && this.Senders.Length > 0;
        }

        public bool HasSendersManual()
        {
            return this.SendersManual != null && this.SendersManual.Length > 0;
        }

        public bool HasRecipients()
        {
            return this.Recipients != null && this.Recipients.Length > 0;
        }

        public bool HasRecipientsManual()
        {
            return this.RecipientsManual != null && this.RecipientsManual.Length > 0;
        }

        public bool HasFascicles()
        {
            return this.Fascicles != null && this.Fascicles.Length > 0;
        }

        public bool HasKind()
        {
            return this.IdProtocolKind != null && this.IdProtocolKind > 0;
        }

        public IProtocolDTO AddDocument(string fullName)
        {
            this.Document = new DocumentDTO(fullName);
            return this;
        }

        public IProtocolDTO AddDocument(string name, byte[] content)
        {
            this.Document = new DocumentDTO(name, content);
            return this;
        }

        public IProtocolDTO AddDocumentBytes(string fullName)
        {
            var content = File.ReadAllBytes(fullName);
            return this.AddDocument(fullName, content);
        }

        public IProtocolDTO AddBiblosDocument(string biblosArchive, int biblosId)
        {
            this.Document = new DocumentDTO(biblosArchive, biblosId);
            return this;
        }

        public IProtocolDTO AddBiblosDocument(Guid biblosGuid)
        {
            this.Document = new DocumentDTO(biblosGuid);
            return this;
        }

        public IProtocolDTO AddAttachment(IDocumentDTO dto)
        {
            var list = this.Attachments != null ? this.Attachments.ToList() : new List<IDocumentDTO>();
            list.Add(dto);
            this.Attachments = list.ToArray();
            return this;
        }

        public IProtocolDTO AddAttachment(string fullName)
        {
            return this.AddAttachment(new DocumentDTO(fullName));
        }

        public IProtocolDTO AddAttachment(string name, byte[] content)
        {
            return this.AddAttachment(new DocumentDTO(name, content));
        }

        public IProtocolDTO AddBiblosAttachment(string biblosArchive, int biblosId)
        {
            return this.AddAttachment(new DocumentDTO(biblosArchive, biblosId));
        }

        public IProtocolDTO AddBiblosAttachment(Guid biblosGuid)
        {
            return this.AddAttachment(new DocumentDTO(biblosGuid));
        }

        public IProtocolDTO AddAttachmentBytes(string fullName)
        {
            var content = File.ReadAllBytes(fullName);
            return this.AddAttachment(fullName, content);
        }

        public IProtocolDTO AddAttachments(IEnumerable<IDocumentDTO> dtos)
        {
            dtos.ToList().ForEach(d => this.AddAttachment(d));
            return this;
        }

        public IProtocolDTO AddAttachments(IEnumerable<string> fullNames)
        {
            fullNames.ToList().ForEach(n => this.AddAttachment(n));
            return this;
        }

        public IProtocolDTO AddAttachmentsBytes(IEnumerable<string> fullNames)
        {
            fullNames.ToList().ForEach(n => this.AddAttachmentBytes(n));
            return this;
        }

        public IProtocolDTO AddAnnexed(IDocumentDTO dto)
        {
            var list = this.Annexes != null ? this.Annexes.ToList() : new List<IDocumentDTO>();
            list.Add(dto);
            this.Annexes = list.ToArray();
            return this;
        }

        public IProtocolDTO AddAnnexed(string fullName)
        {
            return this.AddAnnexed(new DocumentDTO(fullName));
        }

        public IProtocolDTO AddAnnexed(string name, byte[] content)
        {
            return this.AddAnnexed(new DocumentDTO(name, content));
        }

        public IProtocolDTO AddBiblosAnnexed(string biblosArchive, int biblosId)
        {
            return this.AddAnnexed(new DocumentDTO(biblosArchive, biblosId));
        }

        public IProtocolDTO AddBiblosAnnexed(Guid biblosGuid)
        {
            return this.AddAnnexed(new DocumentDTO(biblosGuid));
        }

        public IProtocolDTO AddAnnexedBytes(string fullName)
        {
            var content = File.ReadAllBytes(fullName);
            return this.AddAnnexed(fullName, content);
        }

        public IProtocolDTO AddAnnexes(IEnumerable<IDocumentDTO> dtos)
        {
            dtos.ToList().ForEach(d => this.AddAnnexed(d));
            return this;
        }

        public IProtocolDTO AddAnnexes(IEnumerable<string> fullNames)
        {
            fullNames.ToList().ForEach(n => this.AddAnnexed(n));
            return this;
        }

        public IProtocolDTO AddAnnexesBytes(IEnumerable<string> fullNames)
        {
            fullNames.ToList().ForEach(n => this.AddAnnexedBytes(n));
            return this;
        }

        public IProtocolDTO AddSender(IContactDTO dto)
        {
            var list = this.Senders != null ? this.Senders.ToList() : new List<IContactDTO>();
            list.Add(dto);
            this.Senders = list.ToArray();
            return this;
        }

        public IProtocolDTO AddSender(string emailAddress)
        {
            return this.AddSender(new ContactDTO(emailAddress));
        }

        public IProtocolDTO AddSenders(IEnumerable<IContactDTO> dtos)
        {
            dtos.ToList().ForEach(d => this.AddSender(d));
            return this;
        }

        public IProtocolDTO AddSenderManual(IContactDTO dto)
        {
            var list = this.SendersManual != null ? this.SendersManual.ToList() : new List<IContactDTO>();
            list.Add(dto);
            this.SendersManual = list.ToArray();
            return this;
        }

        public IProtocolDTO AddSenderManual(string description)
        {
            return this.AddSenderManual(new ContactDTO() { Description = description });
        }

        public IProtocolDTO AddSendersManual(IEnumerable<IContactDTO> dtos)
        {
            dtos.ToList().ForEach(d => this.AddSenderManual(d));
            return this;
        }

        public IProtocolDTO AddSenders(IEnumerable<string> emailAddresses)
        {
            emailAddresses.ToList().ForEach(a => this.AddSender(a));
            return this;
        }

        public IProtocolDTO AddRecipient(IContactDTO dto)
        {
            var list = this.Recipients != null ? this.Recipients.ToList() : new List<IContactDTO>();
            list.Add(dto);
            this.Recipients = list.ToArray();
            return this;
        }

        public IProtocolDTO AddRecipient(string emailAddress)
        {
            return this.AddRecipient(new ContactDTO(emailAddress));
        }

        public IProtocolDTO AddRecipients(IEnumerable<IContactDTO> dtos)
        {
            dtos.ToList().ForEach(d => this.AddRecipient(d));
            return this;
        }

        public IProtocolDTO AddRecipients(IEnumerable<string> emailAddresses)
        {
            emailAddresses.ToList().ForEach(a => this.AddRecipient(a));
            return this;
        }

        public IProtocolDTO AddRecipientManual(IContactDTO dto)
        {
            var list = this.RecipientsManual != null ? this.RecipientsManual.ToList() : new List<IContactDTO>();
            list.Add(dto);
            this.RecipientsManual = list.ToArray();
            return this;
        }

        public IProtocolDTO AddRecipientManual(string description, string fiscalCode)
        {
            return this.AddRecipientManual(new ContactDTO() { Description = description, FiscalCode = fiscalCode });
        }

        public IProtocolDTO AddRecipientsManual(IEnumerable<IContactDTO> dtos)
        {
            dtos.ToList().ForEach(d => this.AddRecipientManual(d));
            return this;
        }

        public IProtocolDTO AddFascicle(IContactDTO dto)
        {
            var list = this.Fascicles != null ? this.Fascicles.ToList() : new List<IContactDTO>();
            list.Add(dto);
            this.Fascicles = list.ToArray();
            return this;
        }

        public IProtocolDTO AddFascicles(IEnumerable<IContactDTO> dtos)
        {
            dtos.ToList().ForEach(d => this.AddFascicle(d));
            return this;
        }

        public IProtocolDTO AddContainer(int? id)
        {
            this.Container = new ContainerDTO(id);
            return this;
        }

        public IProtocolDTO AddLinkReferenceContainer(int? id)
        {
            this.LinkReferenceContainer = new ContainerDTO(id);
            return this;
        }

        public IProtocolDTO AddCategory(int? id)
        {
            this.Category = new CategoryDTO(id);
            return this;
        }

        #endregion
    }
}