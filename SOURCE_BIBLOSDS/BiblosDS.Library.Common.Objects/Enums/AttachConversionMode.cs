using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Enums
{
    [DataContract(Name = "AttachConversionMode", Namespace = "http://BiblosDS/2009/10/AttachConversionMode")]
    public enum AttachConversionMode
    {
        /// <summary>
        /// Salva tutto (intestazioni mail, corpo della mail ed eventuali allegati).
        /// </summary>
        [EnumMember]
        Default = 0,
        /// <summary>
        /// Salva tutto ad eccezione degli allegati.
        /// </summary>
        [EnumMember]
        NoAttach = 1,
        /// <summary>
        /// Tutto ad eccezione delle intestazioni (mittente, destinatario, ecc.).
        /// </summary>
        [EnumMember]
        NoHeaders = 2,
        /// <summary>
        /// Salva tutto ad eccezione del corpo della mail.
        /// </summary>
        [EnumMember]
        NoBody = 4,
    }
}
