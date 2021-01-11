using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Enums
{
    [DataContract(Name = "PreservationStatus", Namespace = "http://BiblosDS/2009/10/PreservationStatus")]
    public enum PreservationStatus
    {
        [EnumMember]
        Copiato = 1,
        [EnumMember]
        Chiuso = 2,
    }
}