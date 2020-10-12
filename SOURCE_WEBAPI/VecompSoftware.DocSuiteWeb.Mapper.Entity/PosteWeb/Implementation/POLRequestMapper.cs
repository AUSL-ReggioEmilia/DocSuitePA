using System;
using VecompSoftware.DocSuiteWeb.Entity.PosteWeb;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.PosteWeb
{
    public class POLRequestMapper : BaseEntityMapper<PosteOnLineRequest, PosteOnLineRequest>, IPOLRequestMapper
    {
        public override PosteOnLineRequest Map(PosteOnLineRequest entity, PosteOnLineRequest entityTransformed)
        {
            #region [ Base ]

            entityTransformed.RequestId = entity.RequestId;
            entityTransformed.GuidPoste = entity.GuidPoste;
            entityTransformed.IdOrdine = entity.IdOrdine;
            entityTransformed.Status = entity.Status;
            entityTransformed.StatusDescription = entity.StatusDescription;
            entityTransformed.ErrorMessage = entity.ErrorMessage;
            entityTransformed.TotalCost = entity.TotalCost;
            entityTransformed.ExtendedProperties = entity.ExtendedProperties;

            #region [ Child Derivate - TOLRequest ]

            if (entity is TOLRequest && entityTransformed is TOLRequest)
            {
                
                (entityTransformed as TOLRequest).Testo = (entity as TOLRequest).Testo;
            }
            #endregion

            #region [ Child Derivate - LOLRequest/SOLRequest/ROLRequest ]

            if (entity is LOLRequest && entityTransformed is LOLRequest)
            {
                (entityTransformed as LOLRequest).DocumentName = (entity as LOLRequest).DocumentName;
                (entityTransformed as LOLRequest).DocumentMD5 = (entity as LOLRequest).DocumentMD5;
                (entityTransformed as LOLRequest).DocumentPosteMD5 = (entity as LOLRequest).DocumentPosteMD5;
                (entityTransformed as LOLRequest).DocumentPosteFileType = (entity as LOLRequest).DocumentPosteFileType;

                if ((entity as LOLRequest).IdArchiveChain.HasValue && (entity as LOLRequest).IdArchiveChain != Guid.Empty)
                {
                    (entityTransformed as LOLRequest).IdArchiveChain = (entity as LOLRequest).IdArchiveChain.Value;
                }
                if ((entity as LOLRequest).IdArchiveChainPoste.HasValue && (entity as LOLRequest).IdArchiveChainPoste != Guid.Empty)
                {
                    (entityTransformed as LOLRequest).IdArchiveChainPoste = (entity as LOLRequest).IdArchiveChainPoste.Value;
                }
            }
            #endregion
            
            #endregion

            return entityTransformed;
        }
    }
}
