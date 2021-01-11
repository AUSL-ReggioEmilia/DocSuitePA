using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "AttributeMode", Namespace = "http://BiblosDS/2009/10/AttributeMode")]
    public partial class DocumentAttributeMode : BiblosDSObject
    {
        [DataMember]
        public int IdMode { get; set; }

        [DataMember]
        public string Description { get; set; }

        #region Constructor
        public DocumentAttributeMode()
        {
        }

        public DocumentAttributeMode(int idMode)
        {
            this.IdMode = idMode;
        }

        public DocumentAttributeMode(int idMode, string description)
        {
            this.IdMode = idMode;
            this.Description = description;
        }

        #endregion
    }
}
