using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles
{
    public class FascicleValidator : ObjectValidator<Fascicle, FascicleValidator>, IFascicleValidator
    {
        #region [ Constructor ]
        public FascicleValidator(ILogger logger, IFascicleValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        {
            ICollection<FascicleDocumentUnit> FascicleDocumentUnits = new Collection<FascicleDocumentUnit>();
            ICollection<Contact> Contacts = new Collection<Contact>();
            ICollection<FascicleLog> FascicleLogs = new Collection<FascicleLog>();
            ICollection<FascicleLink> FascicleLinks = new Collection<FascicleLink>();
            ICollection<DocumentUnit> DocumentUnits = new Collection<DocumentUnit>();
            ICollection<FascicleDocument> FascicleDocuments = new Collection<FascicleDocument>();
            ICollection<FascicleRole> FascicleRoles = new Collection<FascicleRole>();
            ICollection<DossierFolder> DossierFolders = new Collection<DossierFolder>();
            ICollection<WorkflowInstance> WorkflowInstances = new Collection<WorkflowInstance>();
            ICollection<FascicleFolder> FascicleFolders = new Collection<FascicleFolder>();
        }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        /// <summary>
        /// Get or set Year
        /// </summary>
        public short Year { get; set; }

        /// <summary>
        /// Get or set Number
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Get or set Conservation
        /// </summary>
        public short? Conservation { get; set; }

        /// <summary>
        /// Get or set StartDate
        /// </summary>
        public DateTimeOffset StartDate { get; set; }

        /// <summary>
        /// Get or set EndDate
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }

        /// <summary>
        /// Get or set Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Get or set Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get or set FascicleObject
        /// </summary>
        public string FascicleObject { get; set; }

        /// <summary>
        /// Get or set Manager
        /// </summary>
        public string Manager { get; set; }

        /// <summary>
        /// Get or set Rack
        /// </summary>
        public string Rack { get; set; }

        /// <summary>
        /// Get or set Note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Get or set RegistrationUser
        /// </summary>
        public string RegistrationUser { get; set; }

        /// <summary>
        /// Get or set RegistrationDate
        /// </summary>
        public DateTimeOffset RegistrationDate { get; set; }

        /// <summary>
        /// Get or set LastChangedUser
        /// </summary>
        public string LastChangedUser { get; set; }

        /// <summary>
        /// Get or set LastChangedDate
        /// </summary>
        public DateTimeOffset? LastChangedDate { get; set; }

        /// <summary>
        /// Get or set FascicleType
        /// </summary>
        public FascicleType FascicleType { get; set; }
        /// <summary>
        /// Get or set VisibilityType
        /// </summary>
        public VisibilityType VisibilityType { get; set; }

        public int? DSWEnvironment { get; set; }

        public string MetadataValues { get; set; }
        /// <summary>
        /// Get or set Timestamp
        /// </summary>
        /// 
        public byte[] Timestamp { get; set; }


        #endregion

        #region [ Navigation Properties ]

        /// <summary>
        /// Get or set Category reference
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// Get or set Container reference
        /// </summary>
        public Container Container { get; set; }

        /// <summary>
        /// Get or set MetadataRepository reference
        /// </summary>
        public MetadataRepository MetadataRepository { get; set; }

        /// <summary>
        /// Get or set ProcessFascicleTemplate reference
        /// </summary>
        public ProcessFascicleTemplate ProcessFascicleTemplate { get; set; }



        public ICollection<FascicleDocumentUnit> FascicleDocumentUnits { get; set; }

        public ICollection<Contact> Contacts { get; set; }

        public ICollection<FascicleLog> FascicleLogs { get; set; }

        public ICollection<FascicleLink> FascicleLinks { get; set; }

        public ICollection<DocumentUnit> DocumentUnits { get; set; }

        public ICollection<FascicleDocument> FascicleDocuments { get; set; }

        public ICollection<FascicleRole> FascicleRoles { get; set; }

        public ICollection<DossierFolder> DossierFolders { get; set; }

        public ICollection<WorkflowInstance> WorkflowInstances { get; set; }

        public ICollection<FascicleFolder> FascicleFolders { get; set; }

        public ICollection<DocumentUnitFascicleCategory> DocumentUnitFascicleCategories { get; set; }
        #endregion
    }
}
