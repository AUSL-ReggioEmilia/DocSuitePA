using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Enums
{
    [DataContract(Name = "AttributeGroupType", Namespace = "http://BiblosDS/2009/10/FilterOperator")]
    public enum AttributeGroupType
    {
        /// <summary>
        /// Attributi non primari o di catena
        /// </summary>
        [EnumMember]
        Undefined = 0,
        /// <summary>
        /// Attributi di catena
        /// </summary>
        [EnumMember]
        Chain = 1,
        /// <summary>
        /// Attributi primari
        /// </summary>
        [EnumMember]
        Primary = 2,
    }
}