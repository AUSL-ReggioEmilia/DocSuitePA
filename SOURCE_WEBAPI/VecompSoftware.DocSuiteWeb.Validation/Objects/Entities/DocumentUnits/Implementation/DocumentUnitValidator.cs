using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Monitors;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentUnits
{
    public class DocumentUnitValidator : ObjectValidator<DocumentUnit, DocumentUnitValidator>, IDocumentUnitValidator
    {
        #region [ Constructor ]
        public DocumentUnitValidator(ILogger logger, IDocumentUnitValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public int EntityId { get; set; }

        public short Year { get; set; }

        public int Number { get; set; }

        public string Title { get; set; }

        public int Environment { get; set; }

        public string DocumentUnitName { get; set; }

        public string Subject { get; set; }

        public DocumentUnitStatus Status { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }

        public Guid IdTenantAOO { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public Category Category { get; set; }

        public Container Container { get; set; }

        public Fascicle Fascicle { get; set; }

        public UDSRepository UDSRepository { get; set; }

        public TenantAOO TenantAOO { get; set; }

        public ICollection<FascicleDocumentUnit> FascicleDocumentUnits { get; set; }

        public ICollection<DocumentUnitRole> DocumentUnitRoles { get; set; }

        public ICollection<DocumentUnitChain> DocumentUnitChains { get; set; }

        public ICollection<DocumentUnitUser> DocumentUnitUsers { get; set; }

        public ICollection<DocumentUnitFascicleHistoricizedCategory> DocumentUnitFascicleHistoricizedCategories { get; set; }

        public ICollection<DocumentUnitFascicleCategory> DocumentUnitFascicleCategories { get; set; }

        public ICollection<TransparentAdministrationMonitorLog> TransparentAdministrationMonitorLogs { get; set; }
        public ICollection<UDSDocumentUnit> UDSDocumentUnits { get; set; }
        #endregion
    }
}