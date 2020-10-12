using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenants
{
    public class TenantAOOValidator : ObjectValidator<TenantAOO, TenantAOOValidator>, ITenantAOOValidator
    {
        #region [ Constructor ]
        public TenantAOOValidator(ILogger logger, ITenantAOOValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        {
        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public string CategorySuffix { get; set; }
        public TenantTypologyType TenantTypologyType { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public ICollection<Tenant> Tenants { get; set; }
        public ICollection<Category> Categories { get; set; }
        #endregion
    }
}
