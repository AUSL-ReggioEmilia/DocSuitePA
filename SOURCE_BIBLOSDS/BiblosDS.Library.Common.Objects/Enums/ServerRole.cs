using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Enums
{
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
        Master = 1,        
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        FullProxy = 2,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Proxy = 3
    }
}
