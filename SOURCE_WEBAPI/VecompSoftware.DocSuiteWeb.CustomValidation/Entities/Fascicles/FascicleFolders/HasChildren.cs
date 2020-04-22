using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.FascicleFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasChildren : BaseValidator<FascicleFolder, FascicleFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasChildren(NameValueCollection attributes)
            : base("Non è possibile eliminare una cartella di un fascicolo che ha sottocartelle.", nameof(HasChildren))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleFolderValidator objectToValidate)
        {
            bool result = objectToValidate.Typology == FascicleFolderTypology.SubFascicle && CurrentUnitOfWork.Repository<FascicleFolder>().CountChildren(objectToValidate.UniqueId) > 0;
            if (result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
