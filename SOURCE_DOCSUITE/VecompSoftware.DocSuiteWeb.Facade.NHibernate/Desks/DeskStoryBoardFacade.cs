using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Desks;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks
{
    public class DeskStoryBoardFacade : BaseProtocolFacade<DeskStoryBoard, Guid, DeskStoryBoardDao>
    {
        #region [ Fields ]
         private readonly string _userName;

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskStoryBoardFacade(string userName) : base()
        {
            _userName = userName;
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        public void AddCommentToStoryBoard(string comment, Desk desk, DeskRoleUser user, DeskDocumentVersion docVersion, DeskStoryBoardType commentType = DeskStoryBoardType.TextComment)
        {
            AddCommentToStoryBoard(comment, CommonUtil.GetInstance().UserDescription, desk, user, docVersion, commentType);
        }

        public void AddCommentToStoryBoard(string comment, string authorName, Desk desk, DeskRoleUser user, DeskDocumentVersion docVersion, DeskStoryBoardType commentType = DeskStoryBoardType.TextComment)
        {
            AddCommentToStoryBoard(comment, authorName, DateTime.Now, desk, user, docVersion, commentType);
        }


        public void AddCommentToStoryBoard(string comment, string authorName, DateTime commentDate, Desk desk, DeskRoleUser user, DeskDocumentVersion docVersion, DeskStoryBoardType commentType = DeskStoryBoardType.TextComment)
        {
            DeskStoryBoard storyBoardComment = new DeskStoryBoard(_userName)
            {
                Author = authorName,
                BoardType = commentType,
                DateBoard = commentDate.ToLocalTime(),
                Comment = comment,
                Desk = desk,
                DeskDocumentVersion = docVersion
            };

            if (user != null)
                storyBoardComment.DeskRoleUser = user;


            Save(ref storyBoardComment);
        }

        public DeskStoryBoard GetLastStoryBoard(DeskDocumentVersion deskDocumentVersion)
        {
            return _dao.GetLastStoryBoard(deskDocumentVersion.Id);
        }

        #endregion [ Methods ]
    }
}
