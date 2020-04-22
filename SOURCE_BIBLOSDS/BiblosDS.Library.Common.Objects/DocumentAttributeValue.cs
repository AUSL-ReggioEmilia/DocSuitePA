using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using BiblosDS.Library.Common.Enums;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "AttributeValue", Namespace = "http://BiblosDS/2009/10/AttributeValue")]
    public partial class DocumentAttributeValue : BiblosDSObject
    {
        /// <summary>
        /// Reference attribute
        /// </summary>
        [DataMember(IsRequired=true)]
        public DocumentAttribute Attribute { get; set; }
        /// <summary>
        /// IdUnivoco dell'attributo
        /// </summary>
        [DataMember()]
        public Guid IdAttribute { get; set; }
        /// <summary>
        /// Value of the attribute
        /// </summary>
        [DataMember(IsRequired=true)]                
        public object Value { get; set; }
                

        public DocumentAttributeValue()
        {
        }

        public DocumentAttributeValue(DocumentAttribute Attribute, object Value)
        {
            this.Attribute = Attribute;
            this.Value = Value;
        }
    }

    [DataContract(Name = "Condition", Namespace = "http://BiblosDS/2009/10/Condition", IsReference=true)]
    public partial class DocumentCondition
    {      
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public object Value { get; set; }
        [DataMember(Name = "Operator")]
        public DocumentConditionFilterOperator Operator { get; set; }
        [DataMember(Name = "LogicalCondition")]
        public DocumentConditionFilterCondition Condition { get; set; }
        [DataMember(Name = "Conditions")]
        public BindingList<DocumentCondition> DocumentAttributeConditions { get; set; }
    }

    [DataContract(Name = "SortCondition", Namespace = "http://BiblosDS/2009/10/SortCondition", IsReference = true)]
    public partial class DocumentSortCondition
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public object Dir { get; set; }     
    }  
}
