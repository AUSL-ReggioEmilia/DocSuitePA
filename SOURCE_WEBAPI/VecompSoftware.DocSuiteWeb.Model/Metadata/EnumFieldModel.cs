using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Metadata
{
    [Serializable()]
    public class EnumFieldModel : BaseFieldModel
    {
        public IDictionary<int, string> Options { get; set; }
    }
}
