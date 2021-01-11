using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects.Response
{
    [DataContract(Name = "PreservationTaskDatesResponse", Namespace = "http://BiblosDS/2009/10/PreservationTaskDatesResponse")]
    [KnownType(typeof(ResponseBase))]
    public class PreservationTaskDatesResponse : ResponseBase
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid IdArchive { get; set; }
        /// <summary>
        /// [get/set] Data prevista esecuzione.
        /// </summary>
        [DataMember]
        public DateTime EstimatedExecution { get; set; }
        /// <summary>
        /// [get/set] Data esecuzione.
        /// </summary>
        [DataMember]
        public DateTime ExecutionDate { get; set; }
        /// <summary>
        /// [get/set] Flag che specifica se il task cui si riferiscono le date è un task di Verifica (flag = TRUE) oppure di Conservazione (flag = FALSE).
        /// </summary>
        [DataMember]
        public bool IsVerifyTask { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? EndDate { get; set; }
    }
}
