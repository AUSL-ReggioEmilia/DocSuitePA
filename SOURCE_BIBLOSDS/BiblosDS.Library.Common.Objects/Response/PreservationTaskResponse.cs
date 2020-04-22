using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects.Response
{
    [DataContract(Name = "PreservationTaskResponse", Namespace = "http://BiblosDS/2009/10/PreservationTaskResponse")]
    [KnownType(typeof(ResponseBase))]
    public class PreservationTaskResponse : ResponseBase
    {
        /// <summary>
        /// Documenti ritornati dalla ricerca
        /// </summary>
        [DataMember]
        public BindingList<PreservationTask> Tasks { get; set; }
    }
}
