using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.DTO.Desks;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Desks
{
    public class MapperDeskDocumentResult : BaseEntityMapper<DeskDocument, DeskDocumentResult>
    {
        #region Constructor
        public MapperDeskDocumentResult() : base() { }
        #endregion

        protected override DeskDocumentResult TransformDTO(DeskDocument entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare DeskDocument se l'entità non è inizializzata");

            DeskDocumentResult dto = new DeskDocumentResult();
            dto.IdDesk = entity.Desk.Id;
            dto.IdDeskDocument = entity.Id;
            dto.RegistrationDate = entity.RegistrationDate;
            dto.LastChangedDate = entity.LastChangedDate;
            dto.LastComment = string.Empty;
            dto.IsActive = entity.IsActive ? (short)1 : (short)0;

            return dto;
        }

        protected override IQueryOver<DeskDocument, DeskDocument> MappingProjection(IQueryOver<DeskDocument, DeskDocument> queryOver)
        {
            throw new NotImplementedException();
        }
    }
}
