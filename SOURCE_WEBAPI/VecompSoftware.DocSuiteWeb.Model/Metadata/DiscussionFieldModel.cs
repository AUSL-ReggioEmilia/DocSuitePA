using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Metadata
{
    [Serializable()]
    public class DiscussionFieldModel : BaseFieldModel
    {
        public DiscussionFieldModel()
        {
            Comments = new HashSet<CommentFieldModel>();
        }

        public ICollection<CommentFieldModel> Comments { get; set; }
    }
}
