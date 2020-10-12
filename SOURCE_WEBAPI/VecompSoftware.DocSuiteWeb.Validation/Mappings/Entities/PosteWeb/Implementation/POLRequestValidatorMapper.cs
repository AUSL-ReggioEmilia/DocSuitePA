using System;
using VecompSoftware.DocSuiteWeb.Entity.PosteWeb;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PosteWeb;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.PosteWeb
{
    public class POLRequestValidatorMapper : BaseMapper<PosteOnLineRequest, POLRequestValidator>, IPOLRequestValidatorMapper
    {
        public POLRequestValidatorMapper() { }

        public override POLRequestValidator Map(PosteOnLineRequest entity, POLRequestValidator entityTransformed)
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

            if (entity is TOLRequest)
            {
                entityTransformed.Testo = (entity as TOLRequest).Testo;
            }
            #endregion

            #region [ Child Derivate - LOLRequest/SOLRequest/ROLRequest ]

            if (entity is LOLRequest)
            {
                entityTransformed.DocumentName = (entity as LOLRequest).DocumentName;
                entityTransformed.DocumentMD5 = (entity as LOLRequest).DocumentMD5;
                entityTransformed.DocumentPosteMD5 = (entity as LOLRequest).DocumentPosteMD5;
                entityTransformed.DocumentPosteFileType = (entity as LOLRequest).DocumentPosteFileType;

                if ((entity as LOLRequest).IdArchiveChain.HasValue && (entity as LOLRequest).IdArchiveChain != Guid.Empty)
                {
                    entityTransformed.IdArchiveChain = (entity as LOLRequest).IdArchiveChain.Value;
                }
                if ((entity as LOLRequest).IdArchiveChainPoste.HasValue && (entity as LOLRequest).IdArchiveChainPoste != Guid.Empty)
                {
                    entityTransformed.IdArchiveChainPoste = (entity as LOLRequest).IdArchiveChainPoste.Value;
                }
            }

            #endregion

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.DocumentUnit = entity.DocumentUnit;

            #endregion

            return entityTransformed;
        }
    }
}
