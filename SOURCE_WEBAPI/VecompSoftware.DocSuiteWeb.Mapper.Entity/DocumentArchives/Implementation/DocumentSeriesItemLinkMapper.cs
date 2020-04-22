using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.DocumentArchives
{

    public class DocumentSeriesItemLinkMapper : BaseEntityMapper<DocumentSeriesItemLink, DocumentSeriesItemLink>, IDocumentSeriesItemLinkMapper
    {
        public DocumentSeriesItemLinkMapper()
        { }

        public override DocumentSeriesItemLink Map(DocumentSeriesItemLink entity, DocumentSeriesItemLink entityTransformed)
        {
            #region [ Base ]

            entityTransformed.LinkType = entity.LinkType;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.DocumentSeriesItem = entity.DocumentSeriesItem;
            entityTransformed.Resolution = entity.Resolution;

            #endregion

            return entityTransformed;
        }

    }
}
