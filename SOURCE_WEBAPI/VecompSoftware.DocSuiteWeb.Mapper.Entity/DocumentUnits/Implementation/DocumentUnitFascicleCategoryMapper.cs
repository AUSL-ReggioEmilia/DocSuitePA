using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.DocumentUnits
{
    public class DocumentUnitFascicleCategoryMapper : BaseEntityMapper<DocumentUnitFascicleCategory, DocumentUnitFascicleCategory>, IDocumentUnitFascicleCategoryMapper
    {
        public DocumentUnitFascicleCategoryMapper()
        {

        }
        public override DocumentUnitFascicleCategory Map(DocumentUnitFascicleCategory entity, DocumentUnitFascicleCategory entityTransformed)
        {
            #region [ Base ]

            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.ObjectState = entity.ObjectState;

            #endregion

            return entityTransformed;
        }
    }
}
