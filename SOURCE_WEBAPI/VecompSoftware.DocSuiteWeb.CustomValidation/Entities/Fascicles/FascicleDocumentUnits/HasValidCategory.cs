using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.FascicleDocumentUnits
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasValidCategory : BaseValidator<FascicleDocumentUnit, FascicleDocumentUnitValidator>
    {

        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasValidCategory(NameValueCollection attributes)
            : base("Il classificatore dell'unità documentaria che si vuole fascicolare non è coerente con il classificatore del fascicolo.", nameof(HasValidCategory))
        {
        }
        #endregion

        #region [ Methods ]
        /// <summary>
        /// protocol cat = faccicle cat || protocol cat contine fasscicle cat
        /// </summary>
        /// <param name="objectToValidate"></param>
        protected override void ValidateObject(FascicleDocumentUnitValidator objectToValidate)
        {
            if (objectToValidate.ReferenceType != ReferenceType.Fascicle)
            {
                return;
            }

            bool result = false;

            FascicleFolder fascicleFolder = objectToValidate.FascicleFolder == null ? null : CurrentUnitOfWork.Repository<FascicleFolder>().GetByFolderId(objectToValidate.FascicleFolder.UniqueId);
            DocumentUnit documentUnit = objectToValidate.DocumentUnit == null ? null : CurrentUnitOfWork.Repository<DocumentUnit>().GetById(objectToValidate.DocumentUnit.UniqueId).FirstOrDefault();
            Fascicle fascicle = objectToValidate.Fascicle == null ? null : CurrentUnitOfWork.Repository<Fascicle>().GetByUniqueId(objectToValidate.Fascicle.UniqueId);
            ICollection<CategoryFascicleTableValuedModel> categoryFascicles = CurrentUnitOfWork.Repository<CategoryFascicle>().GetCategorySubFascicles(fascicle.Category.EntityShortId);

            if (documentUnit != null && documentUnit.Category != null && fascicle != null && fascicle.Category != null
                && (documentUnit.Category.UniqueId == fascicle.Category.UniqueId || (categoryFascicles.Any() && categoryFascicles.Any(c => c.IdCategory == documentUnit.Category.EntityShortId))))
            {
                result = true;
            }

            if (!result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
