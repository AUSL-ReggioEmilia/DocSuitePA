using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APICollaboration = VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using NHibernate;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Collaborations
{
    public class MapperCollaborationModel : BaseEntityMapper<DSW.Collaboration, APICollaboration.CollaborationModel>
    {
        #region [ Fields ]
        private readonly MapperCollaborationSignsModel _mapperCollaborationSignsModel;
        private readonly MapperCollaborationUsersModel _mapperCollaborationUsersModel;
        private readonly MapperCollaborationVersioningModel _mapperCollaborationVersioningModel;
        #endregion

        #region [ Constructor ]
        public MapperCollaborationModel()
        {
            _mapperCollaborationSignsModel = new MapperCollaborationSignsModel();
            _mapperCollaborationUsersModel = new MapperCollaborationUsersModel();
            _mapperCollaborationVersioningModel = new MapperCollaborationVersioningModel();
        }
        #endregion

        #region [ Methods ]
        protected override IQueryOver<DSW.Collaboration, DSW.Collaboration> MappingProjection(IQueryOver<DSW.Collaboration, DSW.Collaboration> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APICollaboration.CollaborationModel TransformDTO(DSW.Collaboration entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare Collaboration se l'entità non è inizializzata");

            APICollaboration.CollaborationModel entityTransformed = new APICollaboration.CollaborationModel();
            entityTransformed.IdCollaboration = entity.Id;
            entityTransformed.AlertDate = entity.AlertDate;
            entityTransformed.DocumentType = entity.DocumentType;
            entityTransformed.IdPriority = entity.IdPriority;
            entityTransformed.IdStatus = entity.IdStatus;
            entityTransformed.MemorandumDate = entity.MemorandumDate;
            entityTransformed.Note = entity.Note;
            entityTransformed.Number = entity.Number;
            entityTransformed.PublicationDate = entity.PublicationDate;
            entityTransformed.PublicationUser = entity.PublicationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.RegistrationName = entity.RegistrationName;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.SignCount = entity.SignCount;
            entityTransformed.Subject = entity.CollaborationObject;
            entityTransformed.Year = entity.Year;
            entityTransformed.CollaborationVersionings = entity.CollaborationVersioning.Select(s => _mapperCollaborationVersioningModel.MappingDTO(s)).ToList();
            entityTransformed.CollaborationUsers = entity.CollaborationUsers.Select(s => _mapperCollaborationUsersModel.MappingDTO(s)).ToList();
            entityTransformed.CollaborationSigns = entity.CollaborationSigns.Select(s => _mapperCollaborationSignsModel.MappingDTO(s)).ToList();

            return entityTransformed;
        }
        #endregion
    }
}
