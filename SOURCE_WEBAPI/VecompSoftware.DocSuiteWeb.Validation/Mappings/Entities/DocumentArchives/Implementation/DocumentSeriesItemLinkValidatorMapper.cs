using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentArchives 
{ 

    public class DocumentSeriesItemLinkValidatorMapper : BaseMapper<DocumentSeriesItemLink, DocumentSeriesItemLinkValidator>, IDocumentSeriesItemLinkValidatorMapper
    {
        public DocumentSeriesItemLinkValidatorMapper() { }

        public override DocumentSeriesItemLinkValidator Map(DocumentSeriesItemLink entity, DocumentSeriesItemLinkValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.LinkType = entity.LinkType;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.DocumentSeriesItem = entity.DocumentSeriesItem;
            entityTransformed.Resolution = entity.Resolution;
            #endregion

            return entityTransformed;
        }

    }
}
