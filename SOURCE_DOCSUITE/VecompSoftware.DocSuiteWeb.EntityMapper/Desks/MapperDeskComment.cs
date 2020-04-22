using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.DTO.Desks;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Desks
{
    public class MapperDeskComment : BaseEntityMapper<DeskStoryBoard, DeskComment>
    {
        public MapperDeskComment() : base() { }
        protected override DeskComment TransformDTO(DeskStoryBoard entity)
        {
            if(entity == null)
                throw new ArgumentException("Impossibile trasformare DeskResult se l'entità non è inizializzata");

            DeskComment dto = new DeskComment();
            dto.Author = entity.Author;
            dto.CommentType = entity.BoardType;
            dto.CreationDate = entity.DateBoard;
            dto.Description = entity.Comment;
            dto.DeskCommentId = entity.Id;
            dto.DeskId = entity.Desk.Id;
            dto.IdDocument = entity.DeskDocumentVersion.DeskDocument.IdDocument;

            return dto;
        }

        protected override IQueryOver<DeskStoryBoard, DeskStoryBoard> MappingProjection(IQueryOver<DeskStoryBoard, DeskStoryBoard> queryOver)
        {
            DeskComment deskComment = null;
            DeskDocument deskDocument = null;

            queryOver
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => deskComment.DeskCommentId)
                    .Select(x => x.Comment).WithAlias(() => deskComment.Description)
                    .Select(x => x.Author).WithAlias(() => deskComment.Author)
                    .Select(x => x.DateBoard).WithAlias(() => deskComment.CreationDate)
                    .Select(x => x.BoardType).WithAlias(() => deskComment.CommentType)
                    .Select(x => x.Desk.Id).WithAlias(() => deskComment.DeskId)
                    .Select(() => deskDocument.IdDocument).WithAlias(() => deskComment.IdDocument));

            return queryOver;
        }
    }
}
