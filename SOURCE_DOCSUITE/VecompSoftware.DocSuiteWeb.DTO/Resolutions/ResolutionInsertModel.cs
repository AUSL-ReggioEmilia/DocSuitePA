using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.DTO.Resolutions
{
    public class ResolutionInsertModel
    {
        #region [ Constructor ]

        public ResolutionInsertModel()
        {
            this.MainDocuments = new Collection<string>();
            this.MainDocumentOmissis = new Collection<string>();
            this.Attachments = new Collection<string>();
            this.AttachmentOmissis = new Collection<string>();
            this.Annexes = new Collection<string>();
            this.Recipients = new List<ContactDTO>();
            this.Proposers = new List<ContactDTO>();
            this.CollaborationAuthorizations = new List<Role>();
            this.Authorizations = new List<Role>();
            this.Category = new List<int>();
        }
        #endregion

        #region [ Properties ]

        public int? ProposalType { get; set; }
        public ICollection<string> MainDocuments { get; set; }
        public ICollection<string> MainDocumentOmissis { get; set; }
        public ICollection<string> Attachments { get; set; }
        public ICollection<string> AttachmentOmissis { get; set; }
        public ICollection<string> Annexes { get; set; }
        public int? Container { get; set; }
        public int? IdCollaboration { get; set; }
        public Guid? ResolutionKind { get; set; }
        public bool? ImmediatelyExecutive { get; set; }
        public bool? AutomaticAdoption { get; set; }
        public DateTime? AdoptionDate { get; set; }
        public string AdoptionNumber { get; set; }
        public IList<ContactDTO> Recipients { get; set; }
        public string AlternativeRecipient { get; set; }
        public IList<ContactDTO> Proposers { get; set; }
        public string AlternativeProposer { get; set; }
        public Role RoleProposer { get; set; }
        public IList<Role> CollaborationAuthorizations { get; set; }
        public ContactDTO Assignee { get; set; }
        public string AlternativeAssignee { get; set; }
        public ContactDTO Responsible { get; set; }
        public string AlternativeResponsible { get; set; }
        public string PrivacySubject { get; set; }
        public string Subject { get; set; }
        public string Note { get; set; }
        public ICollection<Role> Authorizations { get; set; }
        public bool? OcSupervisoryBoard { get; set; }
        public bool? OcConfSindaci { get; set; }
        public bool? OcRegion { get; set; }
        public bool? OcManagement { get; set; }
        public bool? OcCorteConti { get; set; }
        public bool? OcOther { get; set; }
        public string OcOtherDescription { get; set; }
        public IList<int> Category { get; set; }
        public bool? DelSoggetta { get; set; }
        public bool? DelNonSoggetta { get; set; }

        #endregion
    }
}
