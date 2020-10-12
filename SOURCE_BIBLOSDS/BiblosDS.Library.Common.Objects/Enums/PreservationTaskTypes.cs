using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects.Enums
{
    [DataContract(Name = "PreservationTaskTypes", Namespace = "http://BiblosDS/2009/10/PreservationTaskTypes")]
    public enum PreservationTaskTypes
    {
        /// <summary>
        /// Task non gestito
        /// </summary>
        [EnumMember]
        Unknown = 0,
        /// <summary>
        /// Task di conservazione.
        /// </summary>
        [EnumMember]
        Preservation = 1,
        /// <summary>
        /// Task di verifica.
        /// </summary>
        [EnumMember]
        Verify = 3,
        /// <summary>
        /// Task di verifica del supporto di una conservazione 
        /// </summary>
        [EnumMember]
        VerifyPreservation = 4,
        /// <summary>
        /// Task di conservazione ciclo passivo
        /// </summary>
        [EnumMember]
        PassiveCycleConservation = 5,
        /// <summary>
        /// Task conservazione di chiusura anno
        /// </summary>
        [EnumMember]
        CloseAnnualPreservation = 6
    }
}
