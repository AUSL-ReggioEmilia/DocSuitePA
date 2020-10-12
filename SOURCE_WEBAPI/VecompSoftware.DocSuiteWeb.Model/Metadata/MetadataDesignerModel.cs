using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Metadata
{
    [Serializable()]
    public class MetadataDesignerModel
    {
        public bool SETIFieldEnabled { get; set; }
        public MetadataDesignerModel()
        {
            TextFields = new HashSet<TextFieldModel>();
            NumberFields = new HashSet<BaseFieldModel>();
            DateFields = new HashSet<BaseFieldModel>();
            BoolFields = new HashSet<BaseFieldModel>();
            EnumFields = new HashSet<EnumFieldModel>();
            DiscussionFields = new HashSet<DiscussionFieldModel>();
            ContactFields = new HashSet<BaseFieldModel>();
        }

        public ICollection<TextFieldModel> TextFields { get; set; }
        public ICollection<BaseFieldModel> NumberFields { get; set; }
        public ICollection<BaseFieldModel> DateFields { get; set; }
        public ICollection<BaseFieldModel> BoolFields { get; set; }
        public ICollection<EnumFieldModel> EnumFields { get; set; }
        public ICollection<DiscussionFieldModel> DiscussionFields { get; set; }
        public ICollection<BaseFieldModel> ContactFields { get; set; }

    }
}
