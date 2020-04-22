using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APICollaboration = VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using NHibernate;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Collaborations
{
    public class MapperCollaborationSignsModel : BaseEntityMapper<DSW.CollaborationSign, APICollaboration.CollaborationSignModel>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public MapperCollaborationSignsModel()
        {

        }
        #endregion

        #region [ Methods ]
        protected override IQueryOver<DSW.CollaborationSign, DSW.CollaborationSign> MappingProjection(IQueryOver<DSW.CollaborationSign, DSW.CollaborationSign> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APICollaboration.CollaborationSignModel TransformDTO(DSW.CollaborationSign entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare CollaborationSign se l'entità non è inizializzata");

            APICollaboration.CollaborationSignModel entityTransformed = new APICollaboration.CollaborationSignModel();
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.IdCollaborationSign = entity.UniqueId;
            entityTransformed.IsActive = Convert.ToBoolean(entity.IsActive);
            entityTransformed.IsRequired = entity.IsRequired;
            entityTransformed.SignDate = entity.SignDate;
            entityTransformed.SignUser = entity.SignUser;
            entityTransformed.SignName = entity.SignName;

            return entityTransformed;
        }
        #endregion
    }
}
