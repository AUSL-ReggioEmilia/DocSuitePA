using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Enums
{
    [DataContract(Name = "ContentFormat", Namespace = "http://BiblosDS/2009/10/ContentFormat")]
    public enum DocumentContentFormat
    {
        [EnumMember]        
        Binary = 0,
        /// <summary>
        /// Fomrmato binario
        /// </summary>
        [EnumMember]
        [Obsolete("Use Binary", true)]
        Bynary = 100,
        /// <summary>
        /// File zippato
        /// </summary>
        [EnumMember]
        Zip = 1,
        /// <summary>
        /// Base 64 sytring
        /// </summary>
        [EnumMember]
        Base64 = 2,
        /// <summary>
        /// Formato binario del documento conforme
        /// </summary>
        [EnumMember]
        ConformBinary = 3,
    }
}
