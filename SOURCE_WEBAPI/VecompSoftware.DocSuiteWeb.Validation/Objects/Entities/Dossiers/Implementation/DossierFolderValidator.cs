using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers
{
    public class DossierFolderValidator : ObjectValidator<DossierFolder, DossierFolderValidator>, IDossierFolderValidator
    {
        #region [ Constructor ]

        public DossierFolderValidator(ILogger logger, IDossierFolderValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public string Name { get; set; }

        public DossierFolderStatus Status { get; set; }

        public string JsonMetadata { get; set; }

        public byte[] Timestamp { get; set; }

        public string DossierFolderPath { get; set; }

        public short DossierFolderLevel { get; set; }

        public Guid? ParentInsertId { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public Dossier Dossier { get; set; }
        public Category Category { get; set; }
        public Fascicle Fascicle { get; set; }

        public ICollection<DossierComment> DossierComments { get; set; }

        public ICollection<DossierFolderRole> DossierFolderRoles { get; set; }

        public ICollection<DossierLog> DossierLogs { get; set; }

        public ICollection<ProcessFascicleTemplate> FascicleTemplates { get; set; }

        public ICollection<ProcessFascicleWorkflowRepository> FascicleWorkflowRepositories { get; set; }
        #endregion
    }
}

