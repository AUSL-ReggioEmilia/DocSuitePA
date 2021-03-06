﻿using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.UDS
{
    public class UDSCollaborationEntityMapper : BaseModelMapper<UDSCollaborationModel, UDSCollaboration>, IUDSCollaborationEntityMapper
    {
        #region [ Methods ]
        public override UDSCollaboration Map(UDSCollaborationModel entity, UDSCollaboration entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Environment = entity.Environment;
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.RelationType = (Entity.UDS.UDSRelationType)entity.RelationType;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }
        #endregion
    }
}
