using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects.Response
{
    [DataContract(Name = "LogResponse", Namespace = "http://BiblosDS/2009/10/LogResponse")]
    [KnownType(typeof(ResponseBase))]
    public class LogResponse: ResponseBase
    {
        /// <summary>
        /// Documenti ritornati dalla ricerca
        /// </summary>
        [DataMember]
        public BindingList<Log> Logs { get; set; }
    }
}
