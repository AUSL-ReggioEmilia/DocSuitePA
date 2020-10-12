using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects.Enums
{
    [DataContract(Name = "PreservationTaskStatus", Namespace = "http://BiblosDS/2009/10/PreservationTaskStatus")]
    public enum PreservationTaskStatus
    {
        [EnumMember]
        Done = 1,
        [EnumMember]
        Error = 2,
        [EnumMember]
        NoDocuments = 3,
        [EnumMember]
        ExistNoConservatedDocuments = 4
    }
}
