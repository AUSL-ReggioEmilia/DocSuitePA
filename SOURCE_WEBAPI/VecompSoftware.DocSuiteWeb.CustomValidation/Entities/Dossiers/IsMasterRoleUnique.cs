using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsMasterRoleUnique : BaseValidator<Dossier, DossierValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsMasterRoleUnique(NameValueCollection attributes)
            : base("Il settore Responsabile deve essere unico.", nameof(IsMasterRoleUnique))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierValidator objectToValidate)
        {
            if (objectToValidate.DossierRoles.Any() && objectToValidate.DossierRoles.Count(x => x.IsMaster == true) > 1)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
