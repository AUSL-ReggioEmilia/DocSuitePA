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
    public class IsTenantNameUnique : BaseValidator<Tenant, TenantValidator>
    {
        #region [ Fields ]
        #endregion

        public IsTenantNameUnique(NameValueCollection attributes)
                : base("L'azienda è già stata creata.", nameof(IsTenantNameUnique))
        {
        }

        protected override void ValidateObject(TenantValidator objectToValidate)
        {
            Tenant tenant = CurrentUnitOfWork.Repository<Tenant>().GetByTenantName(objectToValidate.TenantName, optimization:true).FirstOrDefault();

            if (tenant != null)
            {
                GenerateInvalidateResult();
            }
        }
    }
}
