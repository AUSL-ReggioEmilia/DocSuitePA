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
    public class DocumentUnitFascicleHistoricizedCategoryValidatorMapper : BaseMapper<DocumentUnitFascicleHistoricizedCategory, DocumentUnitFascicleHistoricizedCategoryValidator>, IDocumentUnitFascicleHistoricizedCategoryValidatorMapper
    {
        public DocumentUnitFascicleHistoricizedCategoryValidatorMapper()
        {

        }
        public override DocumentUnitFascicleHistoricizedCategoryValidator Map(DocumentUnitFascicleHistoricizedCategory entity, DocumentUnitFascicleHistoricizedCategoryValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.ReferencedFascicle = entity.ReferencedFascicle;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Timestamp = entity.Timestamp;
            entityTransformed.UnfascicolatedDate = entity.UnfascicolatedDate;

            #endregion

            #region [ Navigation Properties ]
            entityTransformed.DocumentUnit = entity.DocumentUnit;
            entityTransformed.Category = entity.Category;
            #endregion

            return entityTransformed;
        }
    }
}
