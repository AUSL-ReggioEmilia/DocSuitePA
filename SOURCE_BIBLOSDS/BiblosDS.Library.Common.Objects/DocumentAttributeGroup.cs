using System;
using System.Runtime.Serialization;
using BiblosDS.Library.Common.Enums;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "AttributeGroup", Namespace = "http://BiblosDS/2009/10/AttributeGroup")]
    public partial class DocumentAttributeGroup: BiblosDSObject
    {
        [DataMember(IsRequired = true)]
        public Guid IdAttributeGroup { get; set; }
        [DataMember]
        public Guid IdArchive { get; set; }
        [DataMember]
        public AttributeGroupType GroupType { get { return (AttributeGroupType)IdAttributeType; } set { IdAttributeType = (int)value; } }
        public int IdAttributeType { get; set; }
        [DataMember(IsRequired = true)]
        public string  Description { get; set; }
        [DataMember]
        public bool? IsVisible { get; set; }

        #region Constructors
        public DocumentAttributeGroup() { }
        public DocumentAttributeGroup(Guid idattributegroup, Guid idarchive, AttributeGroupType type, string descr)
        {
            this.IdArchive = idarchive;
            this.IdAttributeGroup = idattributegroup;
            this.GroupType = type;
            this.Description = descr;
        }
        #endregion
    }
}