using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "Attribute", Namespace = "http://BiblosDS/2009/10/Attribute")]
    public partial class DocumentAttribute : BiblosDSObject
    {
        [DataMember(IsRequired=true)]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember(IsRequired = true)]
        public Guid IdAttribute { get; set; }

        [DataMember]
        public DocumentArchive Archive { get; set; }

        [DataMember]
        public DocumentAttributeGroup AttributeGroup { get; set; }

        [DataMember(IsRequired = true)]
        public bool IsRequired { get; set; }

        [DataMember]
        public short? KeyOrder { get; set; }
        
        [DataMember]
        public DocumentAttributeMode Mode { get; set; }

        [DataMember]
        public bool? IsMainDate { get; set; }

        [DataMember]
        public bool? IsEnumerator { get; set; }

        [DataMember]
        public bool? IsAutoInc { get; set; }

        [DataMember]
        public bool? IsUnique { get; set; }

        [DataMember]
        public bool? IsChainAttribute { get; set; }

        [DataMember]
        public bool? IsVisible { get; set; }

        [DataMember]
        public bool? IsSectional { get; set; }

        [DataMember]
        public string AttributeType { get; set; }

        [DataMember]
        public short? ConservationPosition { get; set; }
        
        [DataMember]
        public string KeyFilter { get; set; }

        [DataMember]
        public string KeyFormat { get; set; }

        [DataMember]
        public string Validation { get; set; }

        [DataMember]
        public string Format { get; set; }

        [DataMember]
        public string DefaultValue { get; set; }

        [DataMember]
        public int? MaxLenght { get; set; }

        [DataMember]
        public bool? IsRequiredForPreservation { get; set; }

        [DataMember]
        public bool? IsVisibleForUser { get; set; }        

        #region Extended Property

        /// <summary>
        /// Verify if the Mode is in the state "Medify"
        /// <remarks>Value:3</remarks>
        /// </summary>
        public bool Editable
        {
            get
            {
                return Mode.IdMode.Equals(3);
            }          
        }

        /// <summary>
        /// Verify if the Mode is in the state "Disabled"
        /// <remarks>Value:0</remarks>
        /// </summary>
        [DataMember]
        public bool Disabled
        {
            get
            {
                return Mode != null && Mode.IdMode.Equals(0);
            }
            set { /* EMPTY */}
        }

        public string DisplayName => string.IsNullOrEmpty(Description) ? Name : Description;        

        #endregion

        #region Constructor

        public DocumentAttribute()
        {
        }

        public DocumentAttribute(Guid idAttribute, string name, short isRequired, short? keyOrder, 
            DocumentAttributeMode mode, short? isMainDate, short? isEnumerator, string attributeType, short? conservationPosition,
            string keyFilter, string keyFormat)
        {
            this.IdAttribute = idAttribute; 
            this.Name = name;
            this.IsRequired = isRequired.Equals(1);
            this.KeyOrder = keyOrder;
            this.Mode = mode;
            this.IsMainDate = isMainDate.Equals(1);
            this.IsEnumerator = isEnumerator.Equals(1);
            this.AttributeType = attributeType;
            this.ConservationPosition = conservationPosition;
            this.KeyFilter = keyFilter;
            this.KeyFormat = keyFormat;            

        }

        public DocumentAttribute(Guid idAttribute, string name, bool isRequired, short? keyOrder,
            DocumentAttributeMode mode, bool? isMainDate, bool? isEnumerator, string attributeType, short? conservationPosition,
            string keyFilter, string keyFormat)
        {
            this.IdAttribute = idAttribute;
            this.Name = name;
            this.IsRequired = isRequired.Equals(1);
            this.KeyOrder = keyOrder;
            this.Mode = mode;
            this.IsMainDate = isMainDate.Equals(1);
            this.IsEnumerator = isEnumerator.Equals(1);
            this.AttributeType = attributeType;
            this.ConservationPosition = conservationPosition;
            this.KeyFilter = keyFilter;
            this.KeyFormat = keyFormat;

        }

        #endregion
    }
}
