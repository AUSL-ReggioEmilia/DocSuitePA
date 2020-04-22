using System;

namespace VecompSoftware.DocSuiteWeb.API
{
    public interface IProtocolDTO : IAPIArgument
    {
        #region [ Properties ]

        short? Year { get; set; }

        int? Number { get; set; }

        int? Direction { get; set; }

        IDocumentDTO Document { get; set; }

        IDocumentDTO[] Attachments { get; set; }

        IDocumentDTO[] Annexes { get; set; }

        int? IdDocumentType { get; set; }

        IContainerDTO Container { get; set; }

        IContainerDTO LinkReferenceContainer { get; set; }

        string Subject { get; set; }

        IContactDTO[] Senders { get; set; }

        IContactDTO[] Recipients { get; set; }

        IContactDTO[] Fascicles { get; set; }

        ICategoryDTO Category { get; set; }

        short? IdProtocolKind { get; set; }

        string Note { get; set; }

        char PackageOrigin { get; set; }

        int? Package { get; set; }

        int? PackageLot { get; set; }

        int? PackageIncremental { get; set; }

        string InvoiceNumber { get; set; }

        DateTime? InvoiceDate { get; set; }

        Double? InvoiceTotal { get; set; }

        int? AccountingSectionalNumber { get; set; }

        short? AccountingYear { get; set; }

        DateTime? AccountingDate { get; set; }

        int? AccountingNumber { get; set; }

        string IdentificationSDI { get; set; }

        string RegistrationUser { get; set; }

        bool UseProtocolReserve { get; set; }

        DateTime? ProtocolReserveFrom { get; set; }

        DateTime? ProtocolReserveTo { get; set; }


        #endregion
    }
}
