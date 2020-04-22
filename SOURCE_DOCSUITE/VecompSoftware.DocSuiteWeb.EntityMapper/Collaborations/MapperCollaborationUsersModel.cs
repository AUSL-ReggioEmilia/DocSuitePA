using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APICollaboration = VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using NHibernate;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Collaborations
{
    public class MapperCollaborationUsersModel : BaseEntityMapper<DSW.CollaborationUser, APICollaboration.CollaborationUserModel>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public MapperCollaborationUsersModel()
        {

        }
        #endregion

        #region [ Methods ]
        protected override IQueryOver<DSW.CollaborationUser, DSW.CollaborationUser> MappingProjection(IQueryOver<DSW.CollaborationUser, DSW.CollaborationUser> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APICollaboration.CollaborationUserModel TransformDTO(DSW.CollaborationUser entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare CollaborationUser se l'entità non è inizializzata");

            APICollaboration.CollaborationUserModel entityTransformed = new APICollaboration.CollaborationUserModel();
            entityTransformed.Account = entity.Account;
            entityTransformed.IdCollaborationUser = entity.UniqueId;
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.DestinationFirst = entity.DestinationFirst;
            entityTransformed.DestinationName = entity.DestinationName;

            return entityTransformed;
        }
        #endregion
    }
}
