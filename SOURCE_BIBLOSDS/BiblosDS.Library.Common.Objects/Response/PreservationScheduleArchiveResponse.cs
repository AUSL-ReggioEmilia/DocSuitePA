using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects.Response
{
    [DataContract(Name = "PreservationScheduleArchiveResponse", Namespace = "http://BiblosDS/2009/10/PreservationScheduleArchiveResponse")]
    public class PreservationScheduleArchiveResponse : ResponseBase
    {
        public BindingList<PreservationScheduleArchive> Items { get; set; }
    }
}
