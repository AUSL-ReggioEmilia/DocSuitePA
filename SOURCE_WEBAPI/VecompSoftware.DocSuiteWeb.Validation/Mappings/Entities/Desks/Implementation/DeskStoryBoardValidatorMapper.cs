using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks
{
    public class DeskStoryBoardValidatorMapper : BaseMapper<DeskStoryBoard, DeskStoryBoardValidator>, IDeskStoryBoardValidatorMapper
    {
        #region [ Constructor ]
        public DeskStoryBoardValidatorMapper() { }

        #endregion

        #region [ Methods ]

        public override DeskStoryBoardValidator Map(DeskStoryBoard entity, DeskStoryBoardValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Author = entity.Author;
            entityTransformed.BoardType = entity.BoardType;
            entityTransformed.Comment = entity.Comment;
            entityTransformed.DateBoard = entity.DateBoard;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Desk = entity.Desk;
            entityTransformed.DeskDocumentVersion = entity.DeskDocumentVersion;
            entityTransformed.DeskRoleUser = entity.DeskRoleUser;
            #endregion

            return entityTransformed;
        }
        #endregion

    }
}
