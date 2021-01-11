using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Enums
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "ServerRole", Namespace = "http://BiblosDS/2009/10/ServerRole")]    
    public enum ServerRole
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Undefined = 0,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Remote = 1
    }
}
