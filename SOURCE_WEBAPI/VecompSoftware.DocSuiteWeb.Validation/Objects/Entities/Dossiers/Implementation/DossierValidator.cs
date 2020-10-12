using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers
{
    public class DossierValidator : ObjectValidator<Dossier, DossierValidator>, IDossierValidator
    {
        #region [ Constructor ]

        public DossierValidator(ILogger logger, IDossierValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }
        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public short Year { get; set; }
        public int Number { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Subject { get; set; }
        public string Note { get; set; }
        public string MetadataDesigner { get; set; }
        public string MetadataValues { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }
        public DossierStatus Status { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public Container Container { get; set; }
        public MetadataRepository MetadataRepository { get; set; }
        public ICollection<Process> Processes { get; set; }
        public ICollection<DossierRole> DossierRoles { get; set; }
        public ICollection<Contact> Contacts { get; set; }
        public ICollection<DossierLog> DossierLogs { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<DossierDocument> DossierDocuments { get; set; }
        public ICollection<DossierComment> DossierComments { get; set; }
        public ICollection<DossierFolder> DossierFolders { get; set; }
        public ICollection<WorkflowInstance> WorkflowInstances { get; set; }
        public ICollection<DossierLink> DossierLinks { get; set; }
        public ICollection<MetadataValue> SourceMetadataValues { get; set; }
        public ICollection<MetadataValueContact> MetadataValueContacts { get; set; }
        #endregion
    }
}
