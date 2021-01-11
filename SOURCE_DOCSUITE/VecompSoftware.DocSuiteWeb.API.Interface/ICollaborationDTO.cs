using System;

namespace VecompSoftware.DocSuiteWeb.API
{
    public interface ICollaborationDTO : IAPIArgument
    {
        #region Properties
        int? Id { get; set; }
        string IdStatus { get; set; }
        string DocumentType { get; set; }
        string IdPriority { get; set; }
        IDocumentDTO Document { get; set; }
        IDocumentDTO[] Attachments { get; set; }
        IDocumentDTO[] Annexes { get; set; }
        short? SignCount { get; set; }
        DateTime? MemorandumDate { get; set; }
        DateTime? AlertDate { get; set; }
        string CollaborationObject { get; set; }
        string Note { get; set; }
        int? Year { get; set; }
        int? Number { get; set; }
        int? IdResolution { get; set; }
        DateTime? PublicationDate { get; set; }
        string PublicationUser { get; set; }
        DateTime? RegistrationDate { get; set; }
        IContactDTO Proposer { get; set; }
        IContactDTO Signer { get; set; }
        IContactDTO[] Secretaries { get; set; }
        #endregion
    }
}
