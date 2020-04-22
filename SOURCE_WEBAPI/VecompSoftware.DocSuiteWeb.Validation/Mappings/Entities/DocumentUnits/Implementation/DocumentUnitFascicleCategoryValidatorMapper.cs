using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentUnits
{
    public class DocumentUnitFascicleCategoryValidatorMapper : BaseMapper<DocumentUnitFascicleCategory, DocumentUnitFascicleCategoryValidator>, IDocumentUnitFascicleCategoryValidatorMapper
    {
        public DocumentUnitFascicleCategoryValidatorMapper()
        {

        }

        public override DocumentUnitFascicleCategoryValidator Map(DocumentUnitFascicleCategory entity, DocumentUnitFascicleCategoryValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Timestamp = entity.Timestamp;

            #endregion

            #region [ Navigation Properties ]
            entityTransformed.DocumentUnit = entity.DocumentUnit;
            entityTransformed.Category = entity.Category;
            entityTransformed.Fascicle = entity.Fascicle;
            #endregion

            return entityTransformed;
        }
    }
}
