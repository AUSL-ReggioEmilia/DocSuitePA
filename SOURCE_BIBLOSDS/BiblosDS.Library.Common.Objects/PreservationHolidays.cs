using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationHoliday", Namespace = "http://BiblosDS/2009/10/PreservationHoliday")]
    public class PreservationHoliday : BiblosDSObject
    {
        [DataMember]
        public Guid IdPreservationHolidays { get; set; }

        [DataMember]
        public DateTime HolidayDate { get; set; }
        
        [DataMember]
        public string Description { get; set; }
    }
}
