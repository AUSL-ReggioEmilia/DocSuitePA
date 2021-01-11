using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Finder.Tenants;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Tenants
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsTenantAOONameUnique : BaseValidator<TenantAOO, TenantAOOValidator>
    {
        #region [ Fields ]
        #endregion

        public IsTenantAOONameUnique(NameValueCollection attributes)
                : base("L'azienda è già stata creata.", nameof(IsTenantAOONameUnique))
        {
        }

        protected override void ValidateObject(TenantAOOValidator objectToValidate)
        {
            TenantAOO tenantAOO = CurrentUnitOfWork.Repository<TenantAOO>().GetByTenantAOOName(objectToValidate.Name, optimization: true).FirstOrDefault();

            if (tenantAOO != null)
            {
                GenerateInvalidateResult();
            }
        }
    }
}
