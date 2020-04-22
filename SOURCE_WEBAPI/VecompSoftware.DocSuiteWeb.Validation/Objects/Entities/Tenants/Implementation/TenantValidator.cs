using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenants
{
    public class TenantValidator : ObjectValidator<Tenant, TenantValidator>, ITenantValidator
    {
        #region [ Constructor ]
        public TenantValidator(ILogger logger, ITenantValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string TenantName { get; set; }
        public string CompanyName { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Note { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }

        #endregion

        #region[ Navigation Properties ]

        public ICollection<TenantConfiguration> Configurations { get; set; }
        public ICollection<Container> Containers { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<PECMailBox> PECMailBoxes { get; set; }
        public ICollection<TenantWorkflowRepository> TenantWorkflowRepositories { get; set; }
        public ICollection<Contact> Contacts { get; set; }

        #endregion
    }
}
