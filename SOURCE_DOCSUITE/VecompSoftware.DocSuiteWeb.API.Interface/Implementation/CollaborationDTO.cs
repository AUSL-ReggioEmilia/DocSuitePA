using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class CollaborationDTO : ICollaborationDTO
    {
        #region Constructor
        public CollaborationDTO()
        {            
        }

        public CollaborationDTO(int Id)
        {
            this.Id = Id;
        }
        #endregion

        #region Properties
        public int? Id { get; set; }
        public string IdStatus { get; set; }
        public string DocumentType { get; set; }
        public string IdPriority { get; set; }
        [JsonConverter(typeof(APIArgumentConverter<DocumentDTO, IDocumentDTO>))]
        public IDocumentDTO Document { get; set; }
        [JsonConverter(typeof(APIArgumentConverter<DocumentDTO, IDocumentDTO>))]
        public IDocumentDTO[] Attachments { get; set; }
        [JsonConverter(typeof(APIArgumentConverter<DocumentDTO, IDocumentDTO>))]
        public IDocumentDTO[] Annexes { get; set; }
        public short? SignCount { get; set; }
        public DateTime? MemorandumDate { get; set; }
        public DateTime? AlertDate { get; set; }
        public string CollaborationObject { get; set; }
        public string Note { get; set; }
        public int? Year { get; set; }
        public int? Number { get; set; }
        public int? IdResolution { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string PublicationUser { get; set; }
        public DateTime? RegistrationDate { get; set; }
        [JsonConverter(typeof(APIArgumentConverter<ContactDTO, IContactDTO>))]
        public IContactDTO Proposer { get; set; }
        [JsonConverter(typeof(APIArgumentConverter<ContactDTO, IContactDTO>))]
        public IContactDTO Signer { get; set; }
        [JsonConverter(typeof(APIArgumentConverter<ContactDTO, IContactDTO>))]
        public IContactDTO[] Secretaries { get; set; }
        #endregion

        #region Methods

        public bool HasId()
        {
            return this.Id.HasValue;
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

        public ICollaborationDTO AddDocument(string name, byte[] content)
        {
            this.Document = new DocumentDTO(name, content);
            return this;
        }

        public ICollaborationDTO AddAttachment(IDocumentDTO dto)
        {
            var list = this.Attachments != null ? this.Attachments.ToList() : new List<IDocumentDTO>();
            list.Add(dto);
            this.Attachments = list.ToArray();
            return this;
        }

        public ICollaborationDTO AddAttachment(string name, byte[] content)
        {
            return this.AddAttachment(new DocumentDTO(name, content));
        }

        public ICollaborationDTO AddAnnexes(IDocumentDTO dto)
        {
            var list = this.Annexes != null ? this.Annexes.ToList() : new List<IDocumentDTO>();
            list.Add(dto);
            this.Annexes = list.ToArray();
            return this;
        }

        public ICollaborationDTO AddAnnexes(string name, byte[] content)
        {
            return this.AddAnnexes(new DocumentDTO(name, content));
        }

        public ICollaborationDTO AddProposer(IContactDTO dto)
        {
            this.Proposer = dto;
            return this;
        }

        public ICollaborationDTO AddProposer(string emailAddress)
        {
            return this.AddProposer(new ContactDTO(emailAddress));
        }

        public ICollaborationDTO AddSigner(IContactDTO dto)
        {
            this.Signer = dto;
            return this;
        }

        public ICollaborationDTO AddSigner(string emailAddress)
        {
            return this.AddSigner(new ContactDTO(emailAddress));
        }

        public ICollaborationDTO AddSecretaries(IContactDTO dto)
        {
            var list = this.Secretaries != null ? this.Secretaries.ToList() : new List<IContactDTO>();
            list.Add(dto);
            this.Secretaries = list.ToArray();
            return this;
        }

        public ICollaborationDTO AddSecretaries(string emailAddress)
        {
            return this.AddSecretaries(new ContactDTO(emailAddress));
        }
        #endregion
    }
}
