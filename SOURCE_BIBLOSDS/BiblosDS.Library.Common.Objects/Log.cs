using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using BiblosDS.Library.Common.Enums;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "Log", Namespace = "http://BiblosDS/2009/10/RuleOperator")]
    public class Log : BiblosDSObject
    {
        [DataMember]
        public Guid IdEntry { get; set; }
        [DataMember]
        public LoggingOperationType IdOperationType { get; set; }
        [DataMember]
        public Guid IdArchive { get; set; }
        [DataMember]
        public Nullable<DateTime> TimeStamp { get; set; }
        [DataMember]
        public Guid IdStorage { get; set; }
        [DataMember]
        public Nullable<Guid> IdCorrelation { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Server { get; set; }
        [DataMember]
        public string Client { get; set; }
        #region Ctors
        public Log() { }
        #endregion

        public string Level { get; set; }
    }
}
