using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.DocumentUnits
{
    public class DocumentUnitFascicleHistoricizedCategoryMapper : BaseEntityMapper<DocumentUnitFascicleHistoricizedCategory, DocumentUnitFascicleHistoricizedCategory>, IDocumentUnitFascicleHistoricizedCategoryMapper
    {
        public DocumentUnitFascicleHistoricizedCategoryMapper()
        {

        }

        public override DocumentUnitFascicleHistoricizedCategory Map(DocumentUnitFascicleHistoricizedCategory entity, DocumentUnitFascicleHistoricizedCategory entityTransformed)
        {
            #region [ Base ]

            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.ObjectState = entity.ObjectState;
            entityTransformed.ReferencedFascicle = entity.ReferencedFascicle;
            entityTransformed.UnfascicolatedDate = entity.UnfascicolatedDate;

            #endregion

            return entityTransformed;
        }

    }
}
