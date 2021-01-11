using System;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APICollaboration = VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using NHibernate;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Collaborations
{
    public class MapperCollaborationVersioningModel : BaseEntityMapper<DSW.CollaborationVersioning, APICollaboration.CollaborationVersioningModel>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public MapperCollaborationVersioningModel()
        {

        }
        #endregion

        #region [ Methods ]
        protected override IQueryOver<DSW.CollaborationVersioning, DSW.CollaborationVersioning> MappingProjection(IQueryOver<DSW.CollaborationVersioning, DSW.CollaborationVersioning> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APICollaboration.CollaborationVersioningModel TransformDTO(DSW.CollaborationVersioning entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare CollaborationVersioning se l'entità non è inizializzata");

            APICollaboration.CollaborationVersioningModel entityTransformed = new APICollaboration.CollaborationVersioningModel();
            entityTransformed.CheckedOut = entity.CheckedOut;
            entityTransformed.CheckOutDate = entity.CheckOutDate;
            entityTransformed.CheckOutUser = entity.CheckOutUser;
            entityTransformed.CollaborationIncremental = entity.CollaborationIncremental;
            entityTransformed.DocumentName = entity.DocumentName;
            entityTransformed.IdCollaborationVersioning = entity.Id;
            entityTransformed.IdDocument = entity.IdDocument;
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.IsActive = Convert.ToBoolean(entity.IsActive);
            entityTransformed.DocumentGroup = entity.DocumentGroup;
            entityTransformed.RegistrationUser = entity.RegistrationUser;

            return entityTransformed;
        }
        #endregion
    }
}