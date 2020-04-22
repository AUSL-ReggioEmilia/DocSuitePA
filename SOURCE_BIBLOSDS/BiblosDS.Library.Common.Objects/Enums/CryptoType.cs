using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects.Enums
{
    [DataContract(Name = "CryptoType", Namespace = "http://BiblosDS/2009/10/CryptoType")]
    public enum CryptoType : byte
    {
        /// <summary>
        /// Non e' stata applicata alcuna cifratura OPPURE non e' stato possibile determinarne il tipo.
        /// </summary>
        [EnumMember]
        TYPE_UNKNOWN = 0x00,
        /// <summary>
        /// Lunghezza flusso: 160 bit -> 160 / 8 = 20 byte
        /// </summary>
        [EnumMember]
        TYPE_SHA1 = 0x01,
        /// <summary>
        /// Lunghezza flusso: 256 bit -> 256 / 8 = 32 byte
        /// </summary>
        [EnumMember]
        TYPE_SHA256 = 0x02,
    }
}
