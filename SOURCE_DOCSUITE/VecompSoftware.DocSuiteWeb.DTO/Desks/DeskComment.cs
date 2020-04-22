using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.DTO.Desks
{
    public class DeskComment
    {
        public DeskComment() { }

        public Guid? DeskId { get; set; }
        public Guid? DeskCommentId { get; set; }
        public Guid? IdDocument { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public DeskStoryBoardType? CommentType { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
